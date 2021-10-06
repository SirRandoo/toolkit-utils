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

using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class ConfigService
    {
        private static readonly Dictionary<string, bool> ConfigStore;
        private static readonly Dictionary<string, SettingRef> SettingRefs;

        static ConfigService()
        {
            SettingRefs = new Dictionary<string, SettingRef>();
            ConfigStore = Data.LoadJson<Dictionary<string, bool>>(Paths.ConfigStorePath) ?? new Dictionary<string, bool>();
        }

        public static bool Get([NotNull] string id)
        {
            return ConfigStore.TryGetValue(id.ToLowerInvariant(), out bool value) && value;
        }

        public static void Set([NotNull] string id, bool value)
        {
            ConfigStore[id.ToLowerInvariant()] = value;
        }

        public static bool EnsureExists([NotNull] string id, bool defaultValue)
        {
            if (ConfigStore.TryGetValue(id.ToLowerInvariant(), out bool _))
            {
                return true;
            }

            ConfigStore[id.ToLowerInvariant()] = defaultValue;
            return false;
        }

        public static void Save()
        {
            Data.SaveJson(ConfigStore, Paths.ConfigStorePath);
        }
    }
}
