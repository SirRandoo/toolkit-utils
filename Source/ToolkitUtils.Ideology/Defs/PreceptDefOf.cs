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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;

namespace SirRandoo.ToolkitUtils.Ideology.Defs;

[DefOf]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public static class PreceptDefOf
{
    public static PreceptDef SpouseCount_Male_MaxTwo = null!;
    public static PreceptDef SpouseCount_Female_MaxTwo = null!;
    public static PreceptDef SpouseCount_Male_MaxThree = null!;
    public static PreceptDef SpouseCount_Female_MaxThree = null!;
    public static PreceptDef SpouseCount_Male_MaxFour = null!;
    public static PreceptDef SpouseCount_Female_MaxFour = null!;
    public static PreceptDef SpouseCount_Male_Unlimited = null!;
    public static PreceptDef SpouseCount_Female_Unlimited = null!;
    public static PreceptDef Scarification_Extreme = null!;
    public static PreceptDef Scarification_Heavy = null!;
    public static PreceptDef Scarification_Minor = null!;
}
