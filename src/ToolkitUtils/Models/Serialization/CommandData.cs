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
using Newtonsoft.Json;
using ToolkitUtils.Interfaces;

namespace ToolkitUtils.Models.Serialization
{
    [UsedImplicitly]
    public class CommandData : IRimData, IConfigurableUsageData
    {
        [JsonProperty("isShortcut")] public bool IsShortcut { get; set; }
        [JsonProperty("isBuy")] public bool IsBuy { get; set; }
        [JsonProperty("isBalance")] public bool IsBalance { get; set; }
        [JsonProperty("hasGlobalCooldown")] public bool HasGlobalCooldown { get; set; }
        [JsonProperty("globalCooldown")] public int GlobalCooldown { get; set; }
        [JsonProperty("hasLocalCooldown")] public bool HasLocalCooldown { get; set; }
        [JsonProperty("localCooldown")] public int LocalCooldown { get; set; }
        [JsonProperty("mod")] public string Mod { get; set; }
    }
}
