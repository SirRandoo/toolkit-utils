﻿// ToolkitUtils
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Defs;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Commands.ViewerCommands;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Patches;

/// <summary>
///     A Harmony patch for ensuring shortcut commands are properly
///     executed.
/// </summary>
[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class BuyPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Buy), nameof(Buy.RunCommand));
    }

    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null)
        {
            return null;
        }

        TkUtils.Logger.Error($"Could not patch {original.FullDescription()} -- Things will not work properly!", exception.InnerException ?? exception);

        return null;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static bool Prefix(CommandDriver? __instance, ITwitchMessage twitchMessage)
    {
        if (__instance == null)
        {
            return true;
        }

        if (!TkSettings.StoreState)
        {
            return false;
        }

        Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
        ITwitchMessage message = twitchMessage;

        if (!__instance.command.defName.Equals("Buy"))
        {
            message = twitchMessage.WithMessage($"!{CommandDefOf.Buy.command} {twitchMessage.Message.Substring(1)}");
        }

        if (message!.Message.Split(' ').Length < 2)
        {
            return false;
        }

        Purchase_Handler.ResolvePurchase(viewer, message);

        return false;
    }
}