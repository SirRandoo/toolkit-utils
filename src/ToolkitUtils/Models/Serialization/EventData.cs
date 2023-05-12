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

using System;
using System.Data;
using System.Runtime.Serialization;
using ToolkitUtils.Defs;
using ToolkitUtils.Interfaces;
using TwitchToolkit;

namespace ToolkitUtils.Models.Serialization
{
    public class EventData : IShopDataBase, IConfigurableUsageData
    {
        public EventTypes EventType { get; set; }
        public bool HasGlobalCooldown { get; set; }
        public bool HasLocalCooldown { get; set; }
        public int GlobalCooldown { get; set; }
        public int LocalCooldown { get; set; }
        public string Mod { get; set; }

        [IgnoreDataMember]
        public KarmaType? KarmaType
        {
            get => throw new NotSupportedException();
            set => throw new ReadOnlyException();
        }

        public void Reset()
        {
            // Events should not be reset from here.
            // This method only exists because of the interface it implements.
        }
    }
}
