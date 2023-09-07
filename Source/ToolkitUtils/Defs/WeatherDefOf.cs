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

namespace SirRandoo.ToolkitUtils.Defs;

[DefOf]
[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
public static class WeatherDefOf
{
    public static WeatherDef Clear;
    public static WeatherDef Rain;
    public static WeatherDef VomitRain;
    public static WeatherDef FoggyRain;
    public static WeatherDef RainyThunderstorm;
    public static WeatherDef DryThunderstorm;
    public static WeatherDef SnowGentle;
    public static WeatherDef SnowHard;
    public static WeatherDef Fog;
}