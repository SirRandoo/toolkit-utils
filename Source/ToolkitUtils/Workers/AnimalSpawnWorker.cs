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
    /// <summary>
    ///     A special incident worker that allows for more granular control
    ///     over the animals being spawned.
    /// </summary>
    public sealed class AnimalSpawnWorker : IncidentWorker_SpecificAnimalsWanderIn
    {
        /// <summary>
        ///     The label to be displayed on the subsequent <see cref="Letter"/>
        ///     created by this incident worker.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        ///     The <see cref="PawnKindDef"/> to spawn with this worker. This
        ///     should never be a human-like.
        /// </summary>
        public PawnKindDef AnimalDef { get; set; }

        /// <summary>
        ///     Whether the animals spawned through this incident worker should
        ///     be tamed.
        /// </summary>
        public bool SpawnTamed { get; set; }

        /// <summary>
        ///     Whether the animals spawned through this incident worker should
        ///     be manhunter.
        /// </summary>
        public bool SpawnManhunter { get; set; }

        /// <summary>
        ///     The amount of animals to spawn.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        ///     The optional fixed gender of the animals.
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        ///     The text to display within the <see cref="Letter"/> created by
        ///     this incident worker.
        /// </summary>
        /// <remarks>
        ///     Whereas <see cref="Label"/> is what the letter will be title,
        ///     this is the "body" of the letter.
        /// </remarks>
        public string Text { get; set; }

        /// <summary>
        ///     A hacky override to ensure this incident worker isn't used by
        ///     RimWorld.
        /// </summary>
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

        private Pawn GeneratePawn()
        {
            var request = new PawnGenerationRequest { KindDef = AnimalDef, FixedGender = Gender, Faction = SpawnTamed && !SpawnManhunter ? Faction.OfPlayer : null };

            Pawn pawn = PawnGenerator.GeneratePawn(request);

            return pawn;
        }
    }
}
