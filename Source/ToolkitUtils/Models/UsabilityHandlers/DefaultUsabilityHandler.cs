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
using RimWorld;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class DefaultUsabilityHandler : UsabilityHandlerBase<CompUseEffect>
    {
        public DefaultUsabilityHandler() : base(typeof(CompUseEffect_DestroySelf), typeof(CompUseEffect_StartWick), typeof(CompUseEffect_PlaySound)) { }

        protected override bool IsUsable([NotNull] CompUseEffect comp, Pawn pawn, ThingDef thing, out string failReason) => comp.CanBeUsedBy(pawn, out failReason);

        protected override void Use([NotNull] CompUseEffect comp, Pawn pawn, Thing thing)
        {
            comp.DoEffect(pawn);
        }
    }
}
