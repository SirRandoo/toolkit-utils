// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Messaging;
using Verse;

namespace ToolkitUtils.Mod.Patching;

/// <summary>A Harmony patch for intercepting the language selection process.</summary>
[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class SelectLanguagePatch
{
    private static readonly MethodBase SelectLanguageMethod = AccessTools.Method(typeof(LanguageDatabase), nameof(LanguageDatabase.SelectLanguage));
    private static GlobalEventSystem? _eventSystem;

    /// <summary>Initializes the SelectLanguagePatch with the specified global event system.</summary>
    /// <param name="eventSystem">The global event system used to manage event publication and subscription.</param>
    public static void Initialize(GlobalEventSystem eventSystem)
    {
        _eventSystem = eventSystem;
    }

    /// <summary>Returns an enumerable of method bases that identifies the methods targeted for patching.</summary>
    /// <returns>An enumerable containing the MethodBase instances for the selected language method.</returns>
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return SelectLanguageMethod;
    }

    /// <summary>Postfix method for <see cref="SelectLanguagePatch" /> that invalidates the translation server's cache.</summary>
    private static void Postfix()
    {
        _eventSystem?.Publish(new InvalidateTranslationCacheEventArgs());
    }
}
