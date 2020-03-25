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
        private IncidentParms paramz;
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

            paramz = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, anyPlayerMap);
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
            var race = keyed.Where(i => i.Key.EqualsIgnoreCase("--race"))
                .Select(i => i.Value)
                .FirstOrDefault();

            if (race.NullOrEmpty() || !TkSettings.Race)
            {
                return true;
            }

            var raceDef = DefDatabase<PawnKindDef>.AllDefsListForReading
                .FirstOrDefault(
                    r => r.race.defName.EqualsIgnoreCase(race)
                         || r.race.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(race.ToToolkit())
                );

            if (raceDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.NoRace".Translate(race));
                return false;
            }

            if (!raceDef.RaceProps.Humanlike)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.OnlyHuman".Translate());
                return false;
            }

            kindDef = raceDef;

            return true;
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

                Viewer.TakeViewerCoins(storeIncident.cost);
                Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

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
