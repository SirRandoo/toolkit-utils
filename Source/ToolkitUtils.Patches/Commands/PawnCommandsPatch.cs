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
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.PawnQueue;

namespace SirRandoo.ToolkitUtils.Harmony
{
    /// <summary>
    ///     A Harmony patch to disable Twitch Toolkit's pawn commands in
    ///     favor of Utils'.
    /// </summary>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class PawnCommandsPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(PawnCommands), nameof(PawnCommands.ParseMessage));
        }

        private static bool Prefix() => false;

        [CanBeNull]
        private static Exception Cleanup(MethodBase original, [CanBeNull] Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            TkUtils.Logger.Error($"Could not patch {original.FullDescription()} -- Things will not work properly!", exception.InnerException ?? exception);

            return null;
        }
    }
}
