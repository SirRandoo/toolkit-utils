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

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class CommandData : IRimData, IConfigurableUsageData
    {
        [DataMember(Name = "isShortcut")] public bool IsShortcut { get; set; }
        [DataMember(Name = "isBuy")] public bool IsBuy { get; set; }
        [DataMember(Name = "isBalance")] public bool IsBalance { get; set; }

        [DataMember(Name = "hasGlobalCooldown")]
        public bool HasGlobalCooldown { get; set; }

        [DataMember(Name = "globalCooldown")] public int GlobalCooldown { get; set; }

        [DataMember(Name = "hasLocalCooldown")]
        public bool HasLocalCooldown { get; set; }

        [DataMember(Name = "localCooldown")] public int LocalCooldown { get; set; }
        [DataMember(Name = "mod")] public string Mod { get; set; }
    }
}
