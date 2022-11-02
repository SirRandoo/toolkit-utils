﻿// ToolkitUtils
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
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PawnKindData : IShopDataBase
    {
        [JsonProperty("customName")] public bool CustomName;
        [JsonProperty("stats")] public string[] Stats { get; set; } = { };
        [JsonProperty("mod")] public string Mod { get; set; }
        [JsonProperty("karmaType")] public KarmaType? KarmaType { get; set; }

        public void Reset()
        {
            CustomName = false;
            KarmaType = null;
        }
    }
}
