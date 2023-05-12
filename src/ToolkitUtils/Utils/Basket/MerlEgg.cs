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
using ToolkitUtils.Attributes;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using Verse;

namespace ToolkitUtils.Utils.Basket
{
    [RequiresUser(Name = "MerlFox")]
    public class MerlEgg : EasterEgg
    {
        public override bool IsPossible([NotNull] StoreIncident incident, Viewer viewer) => incident.defName.Equals("BuyPawn", StringComparison.Ordinal);

        public override void Execute([NotNull] Viewer viewer, Pawn pawn)
        {
            if (!viewer.username.Equals("merlfox", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Pawn pet = PawnGenerator.GeneratePawn(
                new PawnGenerationRequest(PawnKindDef.Named("Husky"), Faction.OfPlayer, fixedBirthName: "Scav", fixedGender: Gender.Male)
            );

            pet.training.Train(TrainableDefOf.Tameness, pawn, true);
            pawn.relations.AddDirectRelation(PawnRelationDefOf.Bond, pet);

            GenSpawn.Spawn(pet, pawn.Position, pawn.Map, pawn.Rotation);
        }
    }
}
