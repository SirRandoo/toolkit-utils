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
// with ToolkitUtils.Patches. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using ToolkitCore;
using ToolkitCore.Database;
using ToolkitCore.Models;
using ToolkitUtils.Api;

namespace ToolkitUtils.Patches;

/// <summary>A Harmony patch for fixing ToolkitCore's viewer database being nulled occasionally.</summary>
/// <inheritdoc cref="DisablerPatch" path="/remarks[@id='patch']" />
[HarmonyPatch]
[UsedImplicitly(targetFlags: ImplicitUseTargetFlags.WithMembers)]
internal static class ViewerInterfacePatch
{
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(ViewerInterfacePatch));

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.PropertyGetter(typeof(Viewers), nameof(Viewers.All));
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null) return null;

        Logger.Error(exception, message: "Could not patch {Method} :: You may encounter errors about ToolkitCore's viewer database being null", original.FullDescription());

        return null;
    }

    [UsedImplicitly]
    private static void Prefix()
    {
        if (ToolkitData.globalDatabase == null)
        {
            Logger.Warn("ToolkitCore's global database was null. Recreating...");
            ToolkitData.globalDatabase = new GlobalDatabase();
        }

        if (ToolkitData.globalDatabase.viewers != null) return;

        Logger.Warn("ToolkitCore's viewer data was null. Recreating...");
        ToolkitData.globalDatabase.viewers = [];
    }
}
