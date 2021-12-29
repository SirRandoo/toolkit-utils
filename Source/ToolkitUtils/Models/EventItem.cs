// MIT License
//
// Copyright (c) 2021 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
        private bool checkedVariables;
        private EventData data;
        private StoreIncident incident;
        private IEventSettings settingsEmbed;
        private StoreIncidentVariables variables;

        [IgnoreDataMember]
        public StoreIncident Incident
        {
            get => incident;
            set
            {
                incident = value;
                _ = IsVariables;

                if (IsVariables)
                {
                    _ = SettingsEmbed;
                }

                EventType = incident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.Default;
            }
        }

        [CanBeNull] [IgnoreDataMember] public StoreIncidentVariables Variables => IsVariables ? variables : null;

        [DataMember(Name = "isVariables")]
        public bool IsVariables
        {
            get
            {
                if (!checkedVariables && Incident is StoreIncidentVariables var)
                {
                    variables = var;
                }

                checkedVariables = true;

                return variables != null;
            }
        }

        [DataMember(Name = "hasSettings")]
        public bool HasSettings => (Variables?.customSettings ?? false) && Variables?.customSettingsHelper != null;

        [IgnoreDataMember]
        public bool HasSettingsEmbed =>
            settingsEmbed != null || Variables?.GetModExtension<EventExtension>()?.SettingsEmbed != null;

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
                if (settingsEmbed == null && Settings is IEventSettings s)
                {
                    settingsEmbed = s;
                }

                if (settingsEmbed != null)
                {
                    return settingsEmbed;
                }

                Type ext = Variables?.GetModExtension<EventExtension>()?.SettingsEmbed;
                settingsEmbed = ext == null ? null : (IEventSettings)Activator.CreateInstance(ext);

                return settingsEmbed;
            }
        }

        [DataMember(Name = "data")]
        public EventData EventData
        {
            get => data ??= (EventData)Data;
            set => Data = data = value;
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

        [DataMember(Name = "hasFixedCost")]
        public bool CostEditable => EventType == EventTypes.Default || EventType == EventTypes.Variable;

        [DataMember(Name = "eventType")] public EventTypes EventType { get; set; } = EventTypes.Default;

        [DataMember(Name = "price")]
        public int Cost
        {
            get => Incident.cost;
            set => Incident.cost = value;
        }

        [IgnoreDataMember] public IShopDataBase Data { get; set; }

        public void ResetName() { }

        public void ResetPrice() { }

        public void ResetData() { }

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
                 ?.abbreviation
                   ?? Name;
        }

        private int GetDefaultPrice()
        {
            return (IsVariables ? Store_IncidentEditor.variableIncidentsBackup : Store_IncidentEditor.simpleIncidentsBackup.Select(i => (StoreIncident)i))
                  .FirstOrDefault(i => i.defName.Equals(DefName))
                 ?.cost
                   ?? 50;
        }
    }
}
