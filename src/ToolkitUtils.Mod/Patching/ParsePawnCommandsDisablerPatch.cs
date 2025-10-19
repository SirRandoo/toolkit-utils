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
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using ToolkitUtils.Mod.Logging;
using TwitchToolkit.PawnQueue;

namespace ToolkitUtils.Mod.Patching;

[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class ParsePawnCommandsDisablerPatch
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(PawnCommands), nameof(PawnCommands.ParseMessage));
    }

    private static bool Prefix() => false;

    private static Exception? Cleanup(MethodBase original, Exception? exception = null)
    {
        if (exception == null) return null;

        Logger.Error(
            exception,
            message: "Could not patch method '{Method}' :: The pawn commands will from Twitch Toolkit will clash with the pawn commands from ToolkitUtils.",
            original.FullDescription()
        );

        return null;
    }
}
