// MIT License
// 
// Copyright (c) 2023 SirRandoo
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
using JetBrains.Annotations;
using ToolkitUtils.Data.Models;
using TwitchToolkit.Incidents;

namespace ToolkitUtils.Api
{
    /// <summary>
    ///     A registry for housing <see cref="StoreIncident"/> implementations
    ///     for quick access.
    /// </summary>
    public static class IncidentRegistry
    {
        private static readonly Registry<IncidentWrapper> Registry = new Registry<IncidentWrapper>();

        [NotNull] public static IReadOnlyList<DefWrapper<StoreIncident>> AllIncidents => Registry.AllRegistrants;
        public static bool Register(IncidentWrapper wrapper) => Registry.Register(wrapper);
        public static bool Unregister(IncidentWrapper wrapper) => Registry.Unregister(wrapper);

        public static bool Register(StoreIncident incident)
        {
            if (Registry.Get(incident.defName) != null)
            {
                return false;
            }

            Registry.Register(new IncidentWrapper(incident));

            return true;
        }

        public static bool Unregister([NotNull] StoreIncident incident) => Registry.Unregister(incident.defName);

        public static bool UnregisterNamed(StoreIncident incident)
        {
            IncidentWrapper item = Registry.Get(o => string.Equals(o.Name, incident.label, StringComparison.OrdinalIgnoreCase));

            return item != null && Registry.Unregister(item);
        }

        public static bool UnregisterCoded(StoreIncident incident)
        {
            IncidentWrapper item = Registry.Get(o => string.Equals(o.Code, incident.abbreviation, StringComparison.OrdinalIgnoreCase));

            return item != null && Registry.Unregister(item);
        }
    }
}
