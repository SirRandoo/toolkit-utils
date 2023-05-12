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
using Verse;

namespace ToolkitUtils.Interfaces
{
    public interface IAlienCompatibilityProvider : ICompatibilityProvider
    {
        bool TryReassignBody([NotNull] Pawn pawn);
        bool IsTraitForced([NotNull] Pawn pawn, string defName, int degree);
        bool IsTraitDisallowed([NotNull] Pawn pawn, string defName, int degree);
        bool IsTraitAllowed([NotNull] Pawn pawn, TraitDef traitDef, int degree = -10);
    }
}
