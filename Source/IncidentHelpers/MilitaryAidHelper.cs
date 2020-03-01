using System;
using System.Linq;

using RimWorld;

using TwitchToolkit.Store;

using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class MilitaryAidHelper : IncidentHelper
    {
        private Faction faction;
        private Map map;

        public override bool IsPossible()
        {
            var factions = Current.Game.World.factionManager;

            if(factions == null)
            {
                return false;
            }

            var friendly = factions.GetFactions(minTechLevel: TechLevel.Industrial)
                .Where(f => f.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Ally)
                .Where(f => f.def.allowedArrivalTemperatureRange.Includes(Current.Game.CurrentMap.mapTemperature.OutdoorTemp))
                .Where(f => f.def.allowedArrivalTemperatureRange.Includes(Current.Game.CurrentMap.mapTemperature.SeasonalTemp))
                .RandomElementWithFallback();

            if(friendly == null)
            {
                return false;
            }

            var map = Current.Game.RandomPlayerHomeMap;

            if(map == null)
            {
                return false;
            }

            this.faction = friendly;
            this.map = map;
            return true;
        }

        public override void TryExecute()
        {
            if(map == null)
            {
                Log.Message($"{TKUtils.ID} :: Could not execute military aid -- no map instance set");
                return;
            }

            if(faction == null)
            {
                Log.Message($"{TKUtils.ID} :: Could not execute military aid -- no faction instance set");
                return;
            }

            if(DiplomacyTuning.RequestedMilitaryAidPointsRange == null)
            {
                Log.Message($"{TKUtils.ID} :: Could not execute military aid -- no point range set");
            }

            try
            {
                var paramz = new IncidentParms
                {
                    target = map,
                    faction = faction,
                    raidArrivalModeForQuickMilitaryAid = true,
                    points = DiplomacyTuning.RequestedMilitaryAidPointsRange.RandomInRange
                };

                var worker = new IncidentWorker_RaidFriendly { def = IncidentDefOf.RaidFriendly };

                if(worker.TryExecute(paramz))
                {
                    faction.lastMilitaryAidRequestTick = Find.TickManager.TicksGame;
                }
                else
                {
                    Log.Message($"{TKUtils.ID} :: Could not execute military aid");
                }
            }
            catch(Exception e)
            {
                Log.Error($"{TKUtils.ID} :: Military aid helper failed with error: {e.Message}");
                Log.Error($"{TKUtils.ID} :: {e.StackTrace}");
            }
        }
    }
}
