// MIT License
// 
// Copyright (c) 2022 SirRandoo
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public static partial class Data
    {
        /// <summary>
        ///     A list of encapsulated <see cref="StoreIncident"/>s with Utils
        ///     data attached to them.
        /// </summary>
        public static List<EventItem> Events { get; private set; }

        /// <summary>
        ///     Saves event data to the given file.
        /// </summary>
        /// <param name="path">The file to save event data to</param>
        public static void SaveEventData(string path)
        {
            SaveJson(Events.ToDictionary(e => e.Name, e => e.EventData), path);
        }

        /// <summary>
        ///     Saves event data to the given file.
        /// </summary>
        /// <param name="path">The file to save event data to</param>
        public static async Task SaveEventDataAsync(string path)
        {
            await SaveJsonAsync(Events.ToDictionary(e => e.Name, e => e.EventData), path);
        }

        private static void ValidateEventList()
        {
            Store_IncidentEditor.LoadCopies(); // Just to ensure the actual incidents are loaded.
            Events = DefDatabase<StoreIncident>.AllDefs.Select(i => new EventItem { Incident = i }).ToList();
        }

        private static void ValidateEventData()
        {
            var builder = new StringBuilder();

            foreach (EventItem ev in Events)
            {
                ev.EventData ??= new EventData();

                try
                {
                    ev.EventData.Mod = ev.Incident.TryGetModName();
                    ev.EventData.EventType = ev.Incident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.Default;
                }
                catch (Exception)
                {
                    builder.AppendLine($" - {ev.Name ?? ev.DefName}");
                }
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following events could not be processed:\n");
            TkUtils.Logger.Warn(builder.ToString());
        }

        /// <summary>
        ///     Loads events from the given partial data.
        /// </summary>
        /// <param name="partialData">A collection of partial data to load</param>
        public static void LoadEventPartial([NotNull] IEnumerable<EventPartial> partialData)
        {
            var builder = new StringBuilder();

            foreach (EventPartial partial in partialData)
            {
                EventItem existing = Events.Find(i => i.DefName.Equals(partial.DefName));

                if (existing == null)
                {
                    StoreIncident incident = DefDatabase<StoreIncident>.GetNamed(partial.DefName, false);

                    if (incident == null)
                    {
                        builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}\n");

                        continue;
                    }

                    var e = new EventItem { Incident = incident, Name = partial.Name, Cost = partial.Cost, EventCap = partial.EventCap, KarmaType = partial.KarmaType };

                    if (e.IsVariables)
                    {
                        e.MaxWager = partial.MaxWager;
                    }

                    e.EventData = partial.EventData;
                    Events.Add(e);

                    continue;
                }

                existing.Name = partial.Name;
                existing.Cost = partial.Cost;
                existing.EventCap = partial.EventCap;
                existing.KarmaType = partial.KarmaType;

                if (existing.IsVariables)
                {
                    existing.MaxWager = partial.MaxWager;
                }

                existing.EventData = partial.EventData;
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following events could not be loaded from the partial data provided:\n");
            TkUtils.Logger.Warn(builder.ToString());
        }
    }
}
