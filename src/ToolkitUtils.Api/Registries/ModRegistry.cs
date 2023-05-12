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
using ToolkitUtils.Api.Extensions;
using ToolkitUtils.Data.Models;
using Verse;
using Mod = Verse.Mod;

namespace ToolkitUtils.Api
{
    public static class ModRegistry
    {
        private static readonly List<IMod> Registry = new List<IMod>();
        private static readonly Dictionary<string, IMod> RegistryKeyed = new Dictionary<string, IMod>();

        static ModRegistry()
        {
            foreach (ModMetaData metaData in ModsConfig.ActiveModsInLoadOrder)
            {
                if (RegistryKeyed.ContainsKey(metaData.PackageId))
                {
                    continue;
                }

                string version = metaData.ModVersion;

                if (string.IsNullOrEmpty(version))
                {
                    foreach (Mod handle in LoadedModManager.ModHandles)
                    {
                        if (!string.Equals(handle.Content.PackageId, metaData.PackageId, StringComparison.Ordinal))
                        {
                            continue;
                        }

                        version = handle.GetType().Assembly.GetVersion();

                        break;
                    }
                }

                var mod = new Data.Models.Mod(metaData.PackageId)
                {
                    Name = metaData.GetName(), Authors = metaData.GetAuthors(), Version = version, SteamId = metaData.GetWorkshopId()
                };

                Registry.Add(mod);
                RegistryKeyed.Add(mod.Id, mod);
            }
        }

        /// <summary>
        ///     Returns an enumerable of all the mods contained within the
        ///     registry.
        /// </summary>
        public static IEnumerable<IMod> AllMods => Registry;

        /// <summary>
        ///     Returns the given mod if it was found, or <see langword="null"/>
        /// </summary>
        /// <param name="id">The package id of the mod being obtained.</param>
        [CanBeNull]
        public static IMod Get([NotNull] string id) => RegistryKeyed.TryGetValue(id, out IMod mod) ? mod : default;
    }
}
