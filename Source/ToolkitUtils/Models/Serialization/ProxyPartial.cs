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

using Newtonsoft.Json;
using SirRandoo.ToolkitUtils.Interfaces;

namespace SirRandoo.ToolkitUtils.Models;

public abstract class ProxyPartial : IShopItemBase
{
    [JsonProperty("defName")] public string DefName { get; set; }
    [JsonProperty("enabled")] public bool Enabled { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("price")] public int Cost { get; set; }
    [JsonIgnore] public IShopDataBase Data { get; set; }

    public void ResetName()
    {
        // Partials can't be reset.
        // This method exists solely because of the interface it implements.
        // You should not call this on proxies.
    }

    public void ResetPrice()
    {
        // Partials can't be reset.
        // This method exists solely because of the interface it implements.
        // You should not call this on proxies.
    }

    public void ResetData()
    {
        // Partials can't be reset.
        // This method exists solely because of the interface it implements.
        // You should not call this on proxies.
    }
}
