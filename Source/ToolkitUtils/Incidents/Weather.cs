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
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class Weather : IncidentHelper
    {
        private static readonly Dictionary<string, WeatherDef> IncidentMap;
        private Map map;
        private WeatherDef weather;

        static Weather()
        {
            IncidentMap = new Dictionary<string, WeatherDef>
            {
                {IncidentDefOf.ClearWeather.defName, WeatherDefOf.Clear},
                {IncidentDefOf.WeatherRain.defName, WeatherDefOf.Rain},
                {IncidentDefOf.VomitRain.defName, WeatherDefOf.VomitRain},
                {IncidentDefOf.FoggyRain.defName, WeatherDefOf.FoggyRain},
                {IncidentDefOf.RainyThunderStorm.defName, WeatherDefOf.RainyThunderstorm},
                {IncidentDefOf.DryThunderStorm.defName, WeatherDefOf.DryThunderstorm},
                {IncidentDefOf.SnowGentle.defName, WeatherDefOf.SnowGentle},
                {IncidentDefOf.SnowHard.defName, WeatherDefOf.SnowHard},
                {IncidentDefOf.Fog.defName, WeatherDefOf.Fog}
            };
        }

        public override bool IsPossible()
        {
            if (!IncidentMap.TryGetValue(storeIncident.defName, out weather))
            {
                return false;
            }

            foreach (Map playerMap in Find.Maps.FindAll(m => m.IsPlayerHome))
            {
                if (playerMap.weatherManager?.CurWeatherPerceived == weather)
                {
                    continue;
                }

                map = playerMap;
                break;
            }

            return map != null;
        }

        public override void TryExecute()
        {
            map.weatherManager.TransitionTo(weather);
        }
    }
}
