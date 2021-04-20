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

using System.Runtime.Serialization;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ItemData : IShopDataBase
    {
        [DataMember(Name = "CustomName")] public string CustomName { get; set; }

        [DataMember(Name = "HasQuantityLimit")]
        public bool HasQuantityLimit { get; set; }

        [DataMember(Name = "IsMelee")] public bool IsMelee { get; set; }
        [DataMember(Name = "IsRanged")] public bool IsRanged { get; set; }
        [DataMember(Name = "IsStuffAllowed")] public bool IsStuffAllowed { get; set; }
        [DataMember(Name = "IsWeapon")] public bool IsWeapon { get; set; }
        [DataMember(Name = "QuantityLimit")] public int QuantityLimit { get; set; } = 1;
        [DataMember(Name = "Weight")] public float Weight { get; set; } = 1f;
        [DataMember(Name = "Mod")] public string Mod { get; set; }
        [DataMember(Name = "KarmaType")] public KarmaType? KarmaType { get; set; }
    }
}
