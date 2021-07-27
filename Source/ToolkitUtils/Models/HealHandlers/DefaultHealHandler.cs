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
    public class DefaultHealHandler : IHealHandler
    {
        [NotNull] public string Id => "tkutils.handlers.heal";

        public bool IsHealable([NotNull] Hediff hediff)
        {
            var extension = hediff.def.GetModExtension<HealExtension>();
            return extension == null || extension.ShouldHeal;
        }

        public bool IsHealable([NotNull] BodyPartRecord bodyPart)
        {
            var extension = bodyPart.def.GetModExtension<HealExtension>();
            return extension == null || extension.ShouldHeal;
        }
    }
}
