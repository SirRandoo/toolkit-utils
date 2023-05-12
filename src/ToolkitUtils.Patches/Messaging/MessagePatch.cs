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
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Entities;
using SirRandoo.CommonLib.Interfaces;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Events;
using Verse;

namespace ToolkitUtils.Patches
{
    /// <summary>
    ///     A Harmony patch for ensuring messages received from chat are
    ///     redirected to the <see cref="CommandRouter"/>.
    /// </summary>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class MessagePatch
    {
        private static readonly IRimLogger Logger = new RimLogger("TKU.Patches.Message");
        
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(TwitchWrapper), "OnMessageReceived");
        }

        private static bool Prefix([NotNull] OnMessageReceivedArgs e)
        {
            if (!TkSettings.CommandRouter)
            {
                return true;
            }

            if (e.ChatMessage.Bits > 0)
            {
                Log.Message($"Bits donated :: {e.ChatMessage.Bits.ToString()}");
            }

            if (Current.Game == null)
            {
                return false;
            }

            CommandRouter.CommandQueue.Enqueue(e.ChatMessage);
            MessageLog.LogMessage(e.ChatMessage);

            return false;
        }

        [CanBeNull]
        private static Exception Cleanup(MethodBase original, [CanBeNull] Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            Logger.Error($"Could not patch {original.FullDescription()} -- Things will not work properly!", exception.InnerException ?? exception);

            return null;
        }
    }
}
