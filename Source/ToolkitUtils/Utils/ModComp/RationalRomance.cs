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
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    public static class RationalRomance
    {
        public static readonly bool Active;

        private static readonly List<string> TraitDefs =
            new List<string> { "Polyamorous", "Straight", "Gay", "Bisexual", "Asexual" };

        static RationalRomance()
        {
            Active = ModLister.GetActiveModWithIdentifier("Mlie.RationalRomance") != null;
        }

        public static bool IsTraitDisabled([NotNull] TraitDef traitDef)
        {
            return TraitDefs.Contains(traitDef.defName);
        }
    }
}
