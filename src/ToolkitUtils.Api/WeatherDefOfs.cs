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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>
///     WeatherDefOfs class provides static references to various WeatherDef instances. These definitions describe
///     different weather conditions that can be used within the application, specifying types such as fog, rain, clear
///     skies, and various intensities of snow and thunderstorms.
/// </summary>
[DefOf]
[PublicAPI]
[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
public static class WeatherDefOfs
{
    public static WeatherDef Fog = null!;
    public static WeatherDef Rain = null!;
    public static WeatherDef Clear = null!;
    public static WeatherDef SnowHard = null!;
    public static WeatherDef FoggyRain = null!;
    public static WeatherDef SnowGentle = null!;
    public static WeatherDef DryThunderStorm = null!;
    public static WeatherDef RainyThunderStorm = null!;
}
