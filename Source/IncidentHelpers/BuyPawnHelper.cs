using System;
using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IncidentHelpers;
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
        private bool separateChannel;
        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if(!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, storeIncident.cost))
            {
                return false;
            }

            if(CommandBase.GetOrFindPawn(viewer.username) != null)
            {
                CommandBase.SendCommandMessage(viewer.username, "TKUtils.Responses.Buy.HasPawn".Translate(), separateChannel);
                return false;
            }

            this.separateChannel = separateChannel;
            Viewer = viewer;

            var map = Helper.AnyPlayerMap;

            if(map == null)
            {
                CommandBase.SendCommandMessage(viewer.username, "TKUtils.Responses.Buy.NoMap".Translate(), separateChannel);
                return false;
            }

            paramz = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, map);
            this.map = map;

            if(!CellFinder.TryFindRandomEdgeCellWith(p => map.reachability.CanReachColony(p) && !GridsUtility.Fogged(p, map), map, CellFinder.EdgeRoadChance_Neutral, out loc))
            {
                return false;
            }

            var segments = CommandParser.Parse(message).Skip(2).ToArray();

            if(segments.Length > 0)
            {
                var keyed = CommandParser.ParseKeyed(segments);
                var race = keyed.Where(i => i.Key.EqualsIgnoreCase("--race"))
                    .Select(i => i.Value)
                    .FirstOrDefault();
                var raceDef = DefDatabase<PawnKindDef>.AllDefsListForReading
                    .Where(r => r.race.defName.EqualsIgnoreCase(race) || r.race.LabelCap.RawText.EqualsIgnoreCase(race))
                    .FirstOrDefault();

                if(raceDef != null)
                {
                    if(!raceDef.RaceProps.Humanlike)
                    {
                        CommandBase.SendCommandMessage(viewer.username, "TKUtils.Responses.Buy.OnlyHuman".Translate(), separateChannel);
                        return false;
                    }

                    this.kindDef = raceDef;
                }
                else
                {
                    CommandBase.SendCommandMessage(viewer.username, "TKUtils.Responses.Buy.NoRace".Translate(race.Named("QUERY")), separateChannel);
                    return false;
                }
            }

            return true;
        }

        public override void TryExecute()
        {
            try
            {
                var request = new PawnGenerationRequest(kindDef, Faction.OfPlayer, tile: map.Tile, allowFood: false, mustBeCapableOfViolence: true);
                var pawn = PawnGenerator.GeneratePawn(request);
                var name = pawn.Name as NameTriple;

                pawn.Name = new NameTriple(name.First, Viewer.username, name.Last);
                GenSpawn.Spawn(pawn, loc, map);
                var title = "TKUtils.Responses.Buy.Joined.Title".Translate();
                var text = "TKUtils.Responses.Buy.Joined.Text".Translate(Viewer.username.Named("VIEWER"));
                PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref title, pawn);

                Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.PositiveEvent, pawn);
                Current.Game.GetComponent<GameComponentPawns>().AssignUserToPawn(Viewer.username, pawn);

                Viewer.TakeViewerCoins(storeIncident.cost);
                Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost);

                VariablesHelpers.SendPurchaseMessage("TKUtils.Responses.Buy.PurchaseMessage".Translate());
            }
            catch(Exception e)
            {
                Logger.Error("Could not execute buy pawn", e);
            }
        }
    }
}
