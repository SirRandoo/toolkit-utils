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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Incidents;

namespace SirRandoo.ToolkitUtils.Models
{
    public class EventItem : IShopItemBase
    {
        private EventData data;
        private bool? isVariables;
        private IEventSettings settingsEmbed;
        private StoreIncidentVariables variables;

        public StoreIncident Incident { get; set; }

        [CanBeNull] public StoreIncidentVariables Variables => IsVariables ? variables : null;

        public bool IsVariables
        {
            get
            {
                if (!isVariables.HasValue && Incident is StoreIncidentVariables var)
                {
                    variables = var;
                    isVariables = true;
                }
                else
                {
                    isVariables = false;
                }

                return isVariables.Value;
            }
        }

        public bool HasSettings => Variables!.customSettings && Variables!.customSettingsHelper != null;
        public bool HasSettingsEmbed => Variables!.GetModExtension<EventExtension>()?.SettingsEmbed != null;

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
        public IEventSettings SettingsEmbed
        {
            get
            {
                if (Settings is IEventSettings s)
                {
                    settingsEmbed = s;
                }

                return settingsEmbed ??= (IEventSettings) Activator.CreateInstance(
                    Variables!.GetModExtension<EventExtension>().SettingsEmbed
                );
            }
        }

        public EventData EventData
        {
            get => data ??= (EventData) Data;
            set => Data = data = value;
        }

        public KarmaType KarmaType
        {
            get => Incident.karmaType;
            set => Incident.karmaType = value;
        }

        public int EventCap
        {
            get => Incident.eventCap;
            set => Incident.eventCap = value;
        }

        public int MaxWager
        {
            get => Variables!.maxWager;
            set => Variables!.maxWager = value;
        }

        public EventTypes EventType { get; set; } = EventTypes.None;

        public int Cost
        {
            get => Incident.cost;
            set => Incident.cost = value;
        }

        public IShopDataBase Data { get; set; }

        public string DefName
        {
            get => Incident.defName;
            set => Incident.defName = value;
        }

        public bool Enabled
        {
            get => Incident.defName.Equals("Item") ? Incident.cost >= 0 : Incident.cost > 0;
            set
            {
                Incident.cost = 1;
                _ = value;
            }
        }

        public string Name
        {
            get => Incident.abbreviation;
            set => Incident.abbreviation = value;
        }
    }
}
