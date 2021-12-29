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
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class AnimalSpawnWorker : IncidentWorker_SpecificAnimalsWanderIn
    {
        public string Label { get; set; }
        public PawnKindDef AnimalDef { get; set; }
        public bool SpawnTamed { get; set; }
        public bool SpawnManhunter { get; set; }
        public int Quantity { get; set; }
        public Gender? Gender { get; set; }
        public string Text { get; set; }

        public override float BaseChanceThisGame => 0f;

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (AnimalDef == null || !(parms.target is Map map))
            {
                return false;
            }

            if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 loc, map, CellFinder.EdgeRoadChance_Animal))
            {
                return false;
            }

            if (Quantity <= 0)
            {
                Quantity = Mathf.Clamp(GenMath.RoundRandom(2.5f / AnimalDef.RaceProps.baseBodySize), 2, 10);
            }

            var container = new List<Pawn>();

            for (var _ = 0; _ < Quantity; _++)
            {
                IntVec3 pos = CellFinder.RandomClosewalkCellNear(loc, map, 12);
                Pawn pawn = GeneratePawn();

                GenSpawn.Spawn(pawn, pos, map, Rot4.Random);

                if (Gender.HasValue && pawn.gender != Gender.Value)
                {
                    pawn.gender = Gender.Value;
                }

                if (SpawnManhunter)
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
                }

                container.Add(pawn);
            }

            SendStandardLetter(
                Label ?? "LetterLabelFarmAnimalsWanderIn".LocalizeKeyed(AnimalDef.GetLabelPlural()),
                Text ?? "LetterFarmAnimalsWanderIn".LocalizeKeyed(AnimalDef.GetLabelPlural()),
                SpawnManhunter ? LetterDefOf.NegativeEvent : LetterDefOf.NeutralEvent,
                parms,
                new LookTargets(container)
            );

            return true;
        }

        protected virtual Pawn GeneratePawn()
        {
            var request = PawnGenerationRequest.MakeDefault();
            request.KindDef = AnimalDef;
            request.FixedGender = Gender;
            request.Faction = SpawnTamed && !SpawnManhunter ? Faction.OfPlayer : null;

            Pawn pawn = PawnGenerator.GeneratePawn(request);

            return pawn;
        }
    }
}
