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
using System.Collections.Concurrent;
using System.Text;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils.Helpers
{
    [StaticConstructorOnStartup]
    public static class Translator
    {
        private static readonly ConcurrentDictionary<string, string> TranslationProxy;

        static Translator()
        {
            TranslationProxy = new ConcurrentDictionary<string, string>();
            CopyKeys();
        }

        internal static void Invalidate()
        {
            TranslationProxy.Clear();
        }

        internal static void CopyKeys()
        {
            LoadedLanguage active = LanguageDatabase.activeLanguage;

            if (active == null)
            {
                return;
            }

            var builder = new StringBuilder();
            foreach ((string key, LoadedLanguage.KeyedReplacement value) in active.keyedReplacements)
            {
                if (!TranslationProxy.TryAdd(key, value.value))
                {
                    builder.AppendLine($"- {key}");
                }
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "Could not copy the following translations:");
            builder.AppendLine();
            builder.AppendLine("You may experience translation errors!");
            LogHelper.Warn(builder.ToString());
        }

        public static string Localize([NotNull] this string key)
        {
            return TranslationProxy.TryGetValue(key, out string value) ? value : key;
        }

        public static string Localize([NotNull] this string key, string backup)
        {
            if (TranslationProxy.TryGetValue(key, out string initial))
            {
                return initial;
            }

            return TranslationProxy.TryGetValue(backup, out string back) ? back : key;
        }

        [NotNull]
        public static string LocalizeKeyed([NotNull] this string key, params object[] args)
        {
            try
            {
                return string.Format(LocalizeKeyed(key), args);
            }
            catch (Exception)
            {
                return key;
            }
        }

        [NotNull]
        public static string LocalizeKeyed([NotNull] this string key, string backup, params object[] args)
        {
            try
            {
                return string.Format(Localize(key, backup), args);
            }
            catch (Exception)
            {
                return key;
            }
        }
    }
}
