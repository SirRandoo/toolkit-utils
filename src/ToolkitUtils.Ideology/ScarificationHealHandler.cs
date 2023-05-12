// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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
using ToolkitUtils.Interfaces;
using Verse;
using PreceptDefOf = ToolkitUtils.Ideology.Defs.PreceptDefOf;

namespace ToolkitUtils.Ideology
{
    [UsedImplicitly]
    public class ScarificationHealHandler : IHealHandler
    {
        /// <inheritdoc/>
        [NotNull]
        public string ModId => "Ludeon.Ideology";

        /// <inheritdoc/>
        public bool CanHeal([NotNull] Hediff hediff)
        {
            bool isScarification = hediff.def == HediffDefOf.Scarification;

            Ideo ideo = hediff.pawn.Ideo;

            if (ideo.HasPrecept(PreceptDefOf.Scarification_Minor))
            {
                return !isScarification;
            }

            if (ideo.HasPrecept(PreceptDefOf.Scarification_Heavy))
            {
                return !isScarification;
            }

            if (ideo.HasPrecept(PreceptDefOf.Scarification_Extreme))
            {
                return !isScarification;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool CanHeal(BodyPartRecord bodyPart) => true;
    }
}
