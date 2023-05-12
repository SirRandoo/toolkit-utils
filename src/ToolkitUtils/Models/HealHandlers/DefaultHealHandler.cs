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
using ToolkitUtils.Defs;
using ToolkitUtils.Interfaces;
using Verse;

namespace ToolkitUtils.Models.HealHandlers
{
    [UsedImplicitly]
    public class DefaultHealHandler : IHealHandler
    {
        public bool CanHeal([NotNull] Hediff hediff)
        {
            var @override = hediff.def.GetModExtension<HealExtension>();
            
            return @override == null || @override.ShouldHeal;
        }

        public bool CanHeal([NotNull] BodyPartRecord bodyPart)
        {
            var @override = bodyPart.def.GetModExtension<HealExtension>();
            
            return @override == null || @override.ShouldHeal;
        }

        [NotNull] public string ModId => "sirrandoo.tku";
    }
}
