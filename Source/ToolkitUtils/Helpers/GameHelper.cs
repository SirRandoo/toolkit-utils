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
using SirRandoo.ToolkitUtils.Utils;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    public static class GameHelper
    {
        [CanBeNull]
        public static string SanitizedLabel([NotNull] this Def def)
        {
            return def.label == null ? null : Unrichify.StripTags(def.label);
        }

        [CanBeNull]
        public static string SanitizedLabel([NotNull] this Thing thing)
        {
            return thing.def?.label == null ? null : Unrichify.StripTags(thing.def.label);
        }

        [NotNull]
        public static string TryGetModName([CanBeNull] this ModContentPack content)
        {
            if (content?.IsCoreMod == true)
            {
                return "RimWorld";
            }

            return content?.Name ?? "Unknown";
        }

        [NotNull]
        public static string TryGetModName([CanBeNull] this Def def)
        {
            return def?.modContentPack?.TryGetModName() ?? "Unknown";
        }
    }
}
