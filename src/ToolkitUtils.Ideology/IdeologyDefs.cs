// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Ideology. If not, see <https://www.gnu.org/licenses/>.
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ToolkitUtils.Ideology;

[DefOf]
[PublicAPI]
[SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming")]
public static class IdeologyDefs
{
    public static MemeDef Blindsight;
    public static PreceptDef SpouseCount_Male_MaxTwo;
    public static PreceptDef SpouseCount_Female_MaxTwo;
    public static PreceptDef SpouseCount_Male_MaxThree;
    public static PreceptDef SpouseCount_Female_MaxThree;
    public static PreceptDef SpouseCount_Male_MaxFour;
    public static PreceptDef SpouseCount_Female_MaxFour;
    public static PreceptDef SpouseCount_Male_Unlimited;
    public static PreceptDef SpouseCount_Female_Unlimited;
    public static PreceptDef Scarification_Extreme;
    public static PreceptDef Scarification_Heavy;
    public static PreceptDef Scarification_Minor;
}
