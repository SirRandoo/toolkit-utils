// ToolkitUtils
// Copyright (C) 2021  SirRandoo
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;
using Verse.AI.Group;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class SanctuaryGameCondition : GameCondition
    {
        public override int TransitionTicks => 5000;

        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame % 1725f != 0)
            {
                return;
            }

            List<Map> maps = AffectedMaps;

            foreach (Map map in maps.InRandomOrder())
            {
                ProcessMap(map);
            }
        }

        private static void ProcessMap([NotNull] Map map)
        {
            List<Pawn> pawns = map.mapPawns.AllPawnsSpawned;

            foreach (Pawn pawn in pawns.InRandomOrder())
            {
                if (TryProcessPawn(pawn))
                {
                    return;
                }
            }
        }

        private static bool TryProcessPawn([NotNull] Pawn pawn)
        {
            if (pawn.HostileTo(Faction.OfPlayer))
            {
                return TryProcessHostilePawn(pawn);
            }

            if (pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid && pawn.Faction != Faction.OfPlayer)
            {
                return TryProcessHostilePawn(pawn);
            }

            if (pawn.HomeFaction == Faction.OfPlayer)
            {
                return TryProcessColonist(pawn);
            }

            return pawn.RaceProps.Animal && TryProcessAnimal(pawn);
        }

        private static bool TryProcessAnimal([NotNull] Pawn pawn)
        {
            if (pawn.mindState.mentalStateHandler.CurStateDef == MentalStateDefOf.Manhunter)
            {
                pawn.ClearMind();

                return true;
            }

            if (pawn.mindState.mentalStateHandler.CurStateDef == MentalStateDefOf.ManhunterPermanent)
            {
                pawn.Kill(new DamageInfo(DamageDefOf.Burn, 100000, 1f, category: DamageInfo.SourceCategory.ThingOrUnknown));

                return true;
            }

            if (pawn.Dead)
            {
                pawn.TryResurrect();

                return true;
            }

            if (!(HealHelper.GetPawnHealable(pawn) is { } injury))
            {
                return false;
            }

            switch (injury)
            {
                case Hediff hediff:
                    pawn.health.RemoveHediff(hediff);

                    return true;
                case BodyPartRecord record:
                    pawn.health.RestorePart(record);

                    return true;
            }

            return false;
        }

        private static bool TryProcessColonist([NotNull] Pawn pawn)
        {
            if (pawn.Dead)
            {
                pawn.TryResurrect();

                return true;
            }

            if (!(HealHelper.GetPawnHealable(pawn) is { } injury))
            {
                return false;
            }

            switch (injury)
            {
                case Hediff hediff:
                    pawn.health.RemoveHediff(hediff);

                    return true;
                case BodyPartRecord record:
                    pawn.health.RestorePart(record);

                    return true;
            }

            return false;
        }

        private static bool TryProcessHostilePawn([NotNull] Pawn pawn)
        {
            if (pawn.RaceProps.Humanlike)
            {
                LordToil fleeToil = pawn.Map.lordManager.LordOf(pawn)?.Graph.lordToils.Find(t => t is LordToil_PanicFlee);

                if (fleeToil != null)
                {
                    pawn.Map.lordManager.LordOf(pawn)?.GotoToil(fleeToil);

                    return true;
                }
            }

            pawn.Kill(new DamageInfo(DamageDefOf.Burn, 100000, 1f, category: DamageInfo.SourceCategory.ThingOrUnknown));

            return true;
        }
    }
}
