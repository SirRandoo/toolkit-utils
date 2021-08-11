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
using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class CrystallizedEgg : EasterEgg
    {
        public override bool IsPossible(StoreIncident incident, Viewer viewer)
        {
            return incident == IncidentDefOf.BuyPawn
                   && viewer.username.Equals("crystalroseeve", StringComparison.InvariantCultureIgnoreCase);
        }

        public override void Execute([NotNull] Viewer viewer, Pawn pawn)
        {
            if (!viewer.username.Equals("crystalroseeve", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            Pawn pet = PawnGenerator.GeneratePawn(
                new PawnGenerationRequest(PawnKindDef.Named("Fox_Fennec"), Faction.OfPlayer)
            );
            pet.training.Train(TrainableDefOf.Tameness, pawn, true);
            pawn.relations.AddDirectRelation(PawnRelationDefOf.Bond, pet);

            GenSpawn.Spawn(pet, pawn.Position, pawn.Map, pawn.Rotation);
        }
    }
}
