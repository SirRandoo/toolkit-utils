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
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Models
{
    public class EventItem : IShopItemBase, IUsageItemBase
    {
        private bool _checkedVariables;
        private EventData _data;
        private StoreIncident _incident;
        private IEventSettings _settingsEmbed;
        private StoreIncidentVariables _variables;

        [IgnoreDataMember]
        public StoreIncident Incident
        {
            get => _incident;
            set
            {
                _incident = value;
                _ = IsVariables;

                if (IsVariables)
                {
                    _ = SettingsEmbed;
                }

                EventType = _incident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.Default;
            }
        }

        [CanBeNull] [IgnoreDataMember] public StoreIncidentVariables Variables => IsVariables ? _variables : null;

        [DataMember(Name = "isVariables")]
        public bool IsVariables
        {
            get
            {
                if (!_checkedVariables && Incident is StoreIncidentVariables var)
                {
                    _variables = var;
                }

                _checkedVariables = true;

                return _variables != null;
            }
        }

        [DataMember(Name = "hasSettings")] public bool HasSettings => (Variables?.customSettings ?? false) && Variables?.customSettingsHelper != null;

        [IgnoreDataMember] public bool HasSettingsEmbed => _settingsEmbed != null || Variables?.GetModExtension<EventExtension>()?.SettingsEmbed != null;

        [IgnoreDataMember]
        public IncidentHelperVariablesSettings Settings
        {
            get
            {
                if (Variables!.settings == null)
                {
                    Variables.RegisterCustomSettings();
                }

                return Variables.settings;
            }
        }

        [CanBeNull]
        [IgnoreDataMember]
        public IEventSettings SettingsEmbed
        {
            get
            {
                if (_settingsEmbed == null && Settings is IEventSettings s)
                {
                    _settingsEmbed = s;
                }

                if (_settingsEmbed != null)
                {
                    return _settingsEmbed;
                }

                Type ext = Variables?.GetModExtension<EventExtension>()?.SettingsEmbed;
                _settingsEmbed = ext == null ? null : (IEventSettings)Activator.CreateInstance(ext);

                return _settingsEmbed;
            }
        }

        [DataMember(Name = "data")]
        public EventData EventData
        {
            get => _data ??= (EventData)Data;
            set => Data = _data = value;
        }

        [DataMember(Name = "karmaType")]
        public KarmaType KarmaType
        {
            get => Incident.karmaType;
            set => Incident.karmaType = value;
        }

        [DataMember(Name = "eventCap")]
        public int EventCap
        {
            get => Incident.eventCap;
            set => Incident.eventCap = value;
        }

        [DataMember(Name = "maxWager")]
        public int MaxWager
        {
            get => Variables!.maxWager;
            set => Variables!.maxWager = value;
        }

        [DataMember(Name = "hasFixedCost")] public bool CostEditable => EventType == EventTypes.Default || EventType == EventTypes.Variable;

        [DataMember(Name = "eventType")] public EventTypes EventType { get; set; } = EventTypes.Default;

        [DataMember(Name = "price")]
        public int Cost
        {
            get => Incident.cost;
            set => Incident.cost = value;
        }

        [IgnoreDataMember] public IShopDataBase Data { get; set; }

        public void ResetName()
        {
            // Events should not be reset from here.
            // This method only exists due to the interface it implements.
        }

        public void ResetPrice()
        {
            // Events should not be reset from here.
            // This method only exists due to the interface it implements.
        }

        public void ResetData()
        {
            // Events should not be reset from here.
            // This method only exists due to the interface it implements.
        }

        [DataMember(Name = "defName")]
        public string DefName
        {
            get => Incident.defName;
            set => Incident.defName = value;
        }

        [DataMember(Name = "enabled")]
        public bool Enabled
        {
            get => Incident.defName.Equals("Item") ? Incident.cost >= 0 : Incident.cost > 0;
            set => Incident.cost = value ? GetDefaultPrice() : -10;
        }

        [DataMember(Name = "abr")]
        public string Name
        {
            get => Incident.abbreviation;
            set => Incident.abbreviation = value;
        }

        [IgnoreDataMember] [CanBeNull] public IConfigurableUsageData UsageData => Data as IConfigurableUsageData;

        public string GetDefaultAbbreviation()
        {
            return (IsVariables ? Store_IncidentEditor.variableIncidentsBackup : Store_IncidentEditor.simpleIncidentsBackup.Select(i => (StoreIncident)i))
               .FirstOrDefault(i => i.defName.Equals(DefName))
              ?.abbreviation ?? Name;
        }

        private int GetDefaultPrice()
        {
            return (IsVariables ? Store_IncidentEditor.variableIncidentsBackup : Store_IncidentEditor.simpleIncidentsBackup.Select(i => (StoreIncident)i))
               .FirstOrDefault(i => i.defName.Equals(DefName))
              ?.cost ?? 50;
        }
    }
}
