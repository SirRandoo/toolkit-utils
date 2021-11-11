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

using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class IngestabilityUsabilityHandler : IUsabilityHandler
    {
        [NotNull] public string Id => "tkutils.handlers.usability.ingestability";

        public bool IsUsable([NotNull] ThingDef thing)
        {
            return thing.IsIngestible;
        }

        public void Use(Pawn pawn, ThingDef thingDef)
        {
            Thing thing = ThingMaker.MakeThing(thingDef);

            pawn.needs.food.CurLevel += thing.Ingested(pawn, pawn.needs.food.NutritionWanted);
        }
    }
}
