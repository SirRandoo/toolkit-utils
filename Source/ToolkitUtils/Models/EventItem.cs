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
using JetBrains.Annotations;
using Newtonsoft.Json;
using SirRandoo.ToolkitUtils.Defs;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Models;

public class EventItem : IShopItemBase, IUsageItemBase
{
    private bool _checkedVariables;
    private EventData _data;
    private StoreIncident _incident;
    private IEventSettings _settingsEmbed;
    private StoreIncidentVariables _variables;

    [JsonIgnore]
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

    [CanBeNull] [JsonIgnore] public StoreIncidentVariables Variables => IsVariables ? _variables : null;

    [JsonProperty("isVariables")]
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

    [JsonProperty("hasSettings")] public bool HasSettings => (Variables?.customSettings ?? false) && Variables?.customSettingsHelper != null;

    [JsonIgnore] public bool HasSettingsEmbed => _settingsEmbed != null || Variables?.GetModExtension<EventExtension>()?.SettingsEmbed != null;

    [JsonIgnore]
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
    [JsonIgnore]
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

    [JsonProperty("data")]
    public EventData EventData
    {
        get => _data ??= (EventData)Data;
        set => Data = _data = value;
    }

    [JsonProperty("karmaType")]
    public KarmaType KarmaType
    {
        get => Incident.karmaType;
        set => Incident.karmaType = value;
    }

    [JsonProperty("eventCap")]
    public int EventCap
    {
        get => Incident.eventCap;
        set => Incident.eventCap = value;
    }

    [JsonProperty("maxWager")]
    public int MaxWager
    {
        get => Variables!.maxWager;
        set => Variables!.maxWager = value;
    }

    [JsonProperty("hasFixedCost")] public bool CostEditable => EventType == EventTypes.Default || EventType == EventTypes.Variable;

    [JsonProperty("eventType")] public EventTypes EventType { get; set; } = EventTypes.Default;

    [JsonProperty("price")]
    public int Cost
    {
        get => Incident.cost;
        set => Incident.cost = value;
    }

    [JsonIgnore] public IShopDataBase Data { get; set; }

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

    [JsonProperty("defName")]
    public string DefName
    {
        get => Incident.defName;
        set => Incident.defName = value;
    }

    [JsonProperty("enabled")]
    public bool Enabled
    {
        get => Incident.defName.Equals("Item") ? Incident.cost >= 0 : Incident.cost > 0;
        set => Incident.cost = value ? GetDefaultPrice() : -10;
    }

    [JsonProperty("abr")]
    public string Name
    {
        get => Incident.abbreviation;
        set => Incident.abbreviation = value;
    }

    [JsonIgnore] [CanBeNull] public IConfigurableUsageData UsageData => Data as IConfigurableUsageData;

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