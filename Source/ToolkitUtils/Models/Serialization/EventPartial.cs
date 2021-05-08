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
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    public class EventPartial : ProxyPartial
    {
        [DataMember(Name = "data")]
        public EventData EventData
        {
            get => (EventData) Data;
            set => Data = value;
        }

        [DataMember(Name = "karmaType")] public KarmaType KarmaType { get; set; }
        [DataMember(Name = "eventCap")] public int EventCap { get; set; }
        [DataMember(Name = "maxWager")] public int MaxWager { get; set; }
        [DataMember(Name = "eventType")] public EventTypes EventType { get; set; }

        [NotNull]
        public static EventPartial FromIncident([NotNull] EventItem ev)
        {
            var partial = new EventPartial
            {
                DefName = ev.DefName,
                Name = ev.Name,
                Cost = ev.Cost,
                Enabled = ev.Enabled,
                KarmaType = ev.KarmaType,
                EventCap = ev.EventCap,
                EventType = ev.EventType,
                EventData = ev.EventData
            };

            if (ev.IsVariables)
            {
                partial.MaxWager = ev.MaxWager;
            }

            return partial;
        }
    }
}
