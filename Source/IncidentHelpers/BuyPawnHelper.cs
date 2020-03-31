using System;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class BuyPawnHelper : IncidentHelperVariables
    {
        private PawnKindDef kindDef = PawnKindDefOf.Colonist;
        private IntVec3 loc;
        private Map map;
        private XmlRace race;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, storeIncident.cost))
            {
                return false;
            }

            if (CommandBase.GetOrFindPawn(viewer.username) != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.HasPawn".Translate());
                return false;
            }

            Viewer = viewer;

            var anyPlayerMap = Helper.AnyPlayerMap;

            if (anyPlayerMap == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.NoMap".Translate());
                return false;
            }

            map = anyPlayerMap;

            if (!CellFinder.TryFindRandomEdgeCellWith(
                p => anyPlayerMap.reachability.CanReachColony(p) && !p.Fogged(anyPlayerMap),
                anyPlayerMap,
                CellFinder.EdgeRoadChance_Neutral,
                out loc
            ))
            {
                Logger.Warn("No reachable location to spawn a viewer pawn!");
                return false;
            }

            var segments = CommandParser.Parse(message).Skip(2).ToArray();

            if (segments.Length <= 0)
            {
                Logger.Warn("No command arguments specified!");
                return true;
            }

            var keyed = CommandParser.ParseKeyed(segments);
            var raceParam = keyed.Where(i => i.Key.EqualsIgnoreCase("--race") || i.Key.EqualsIgnoreCase("race"))
                .Select(i => i.Value)
                .FirstOrDefault();

            if (raceParam.NullOrEmpty() || !TkSettings.Race)
            {
                return true;
            }

            var raceDef = DefDatabase<PawnKindDef>.AllDefsListForReading
                .FirstOrDefault(
                    r => r.race.defName.ToToolkit().EqualsIgnoreCase(raceParam.ToToolkit())
                         || r.race.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(raceParam.ToToolkit())
                );

            if (raceDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.NoRace".Translate(raceParam));
                return false;
            }

            if (!raceDef.RaceProps.Humanlike)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.OnlyHuman".Translate());
                return false;
            }

            kindDef = raceDef;
            race = TkUtils.ShopExpansion.Races.FirstOrDefault(r => r.DefName.Equals(raceDef.defName));

            if (race == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoRace".Translate(raceParam));
                return false;
            }

            if (!race.Enabled)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.DisabledItem".Translate());
                return false;
            }

            if (Viewer.GetViewerCoins() >= race.Price)
            {
                return true;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.Responses.NotEnoughCoins".Translate(
                    race.Price.ToString("N0"),
                    Viewer.GetViewerCoins().ToString("N0")
                )
            );
            return false;

        }

        public override void TryExecute()
        {
            try
            {
                var request = new PawnGenerationRequest(
                    kindDef,
                    Faction.OfPlayer,
                    tile: map.Tile,
                    allowFood: false,
                    mustBeCapableOfViolence: true
                );
                var pawn = PawnGenerator.GeneratePawn(request);

                if (!(pawn.Name is NameTriple name))
                {
                    Logger.Warn("Pawn name is not a name triple!");
                    return;
                }

                pawn.Name = new NameTriple(name.First, Viewer.username, name.Last);
                GenSpawn.Spawn(pawn, loc, map);
                var title = "TKUtils.Responses.Buy.Joined.Title".Translate();
                var text = "TKUtils.Responses.Buy.Joined.Text".Translate(Viewer.username);
                PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref title, pawn);

                Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.PositiveEvent, pawn);
                Current.Game.GetComponent<GameComponentPawns>().AssignUserToPawn(Viewer.username, pawn);

                Viewer.TakeViewerCoins(race?.Price ?? storeIncident.cost);
                Viewer.CalculateNewKarma(storeIncident.karmaType, race?.Price ?? storeIncident.cost);

                if (ToolkitSettings.PurchaseConfirmations)
                {
                    MessageHelper.ReplyToUser(Viewer.username, "TKUtils.Responses.Buy.PurchaseMessage".Translate());
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not execute buy pawn", e);
            }
        }
    }
}
