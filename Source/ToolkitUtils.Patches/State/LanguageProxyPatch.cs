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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;
using Translator = SirRandoo.ToolkitUtils.Helpers.Translator;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class LanguageProxyPatch
    {
        private static MethodBase _languageChangeMethod;

        private static bool Prepare()
        {
            _languageChangeMethod ??= AccessTools.Method(typeof(LanguageDatabase), nameof(LanguageDatabase.SelectLanguage));

            return _languageChangeMethod != null;
        }

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return _languageChangeMethod;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static void Prefix(LoadedLanguage lang, out bool __state)
        {
            __state = lang != LanguageDatabase.activeLanguage;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static void Postfix(bool __state)
        {
            if (!__state)
            {
                // The language requested was the currently active language.
                return;
            }

            Translator.Invalidate();
            Translator.CopyKeys();
        }
    }
}
