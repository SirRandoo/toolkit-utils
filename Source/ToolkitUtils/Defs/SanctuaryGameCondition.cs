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

using System;
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
            List<Map> maps = AffectedMaps;

            if (Find.TickManager.TicksGame % 3451 != 0)
            {
                return;
            }

            foreach (Pawn pawn in Find.ColonistBar.GetColonistsInOrder())
            {
                if (!(pawn is {Dead: true}))
                {
                    continue;
                }

                pawn.TryResurrect();
            }

            foreach (Map map in maps)
            {
                if (!map.IsPlayerHome)
                {
                    continue;
                }

                DoEffectOn(map);
            }
        }

        private static void DoEffectOn([NotNull] Map map)
        {
            List<Pawn> pawns = map.mapPawns.AllPawnsSpawned;

            try
            {
                foreach (Pawn pawn in pawns)
                {
                    DoEffectOn(pawn);
                }
            }
            catch (InvalidOperationException)
            {
                // Should only happen when killing a pawn.
            }
        }

        private static void DoEffectOn([NotNull] Pawn pawn)
        {
            if (pawn.HostileTo(Faction.OfPlayer)
                || pawn.RaceProps.FleshType == FleshTypeDefOf.Insectoid && pawn.Faction != Faction.OfPlayer)
            {
                ProcessHostilePawn(pawn);
                return;
            }

            if (pawn.IsColonistPlayerControlled && HealHelper.GetPawnHealable(pawn) is { } part)
            {
                switch (part)
                {
                    case Hediff hediff:
                        pawn.health.RemoveHediff(hediff);
                        return;
                    case BodyPartRecord record:
                        pawn.health.RestorePart(record);
                        return;
                }
            }
        }

        private static void ProcessHostilePawn([NotNull] Pawn pawn)
        {
            if (pawn.RaceProps.Humanlike)
            {
                LordToil fleeToil = pawn.Map.lordManager.LordOf(pawn)
                  ?.Graph.lordToils.Find(t => t is LordToil_PanicFlee);

                if (fleeToil != null)
                {
                    pawn.Map.lordManager.LordOf(pawn)?.GotoToil(fleeToil);
                    return;
                }
            }

            pawn.Kill(new DamageInfo(DamageDefOf.Burn, 100000, 1f, category: DamageInfo.SourceCategory.ThingOrUnknown));
        }
    }
}
