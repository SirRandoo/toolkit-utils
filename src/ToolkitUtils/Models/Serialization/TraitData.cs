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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ToolkitUtils.Interfaces;
using TwitchToolkit;

namespace ToolkitUtils.Models.Serialization
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TraitData : IShopDataBase
    {
        [JsonProperty("conflicts")] public string[] Conflicts = { };
        [JsonProperty("stats")] public string[] Stats = { };
        [JsonProperty("canBypassLimit")] public bool CanBypassLimit { get; set; }
        [JsonProperty("customName")] public bool CustomName { get; set; }
        [JsonProperty("karmaTypeForRemoving")] public KarmaType? KarmaTypeForRemoving { get; set; }
        [JsonProperty("mod")] public string Mod { get; set; }
        [JsonProperty("karmaTypeForAdding")] public KarmaType? KarmaType { get; set; }

        public void Reset()
        {
            CanBypassLimit = false;
            CustomName = false;
            KarmaType = null;
            KarmaTypeForRemoving = null;
        }
    }
}
