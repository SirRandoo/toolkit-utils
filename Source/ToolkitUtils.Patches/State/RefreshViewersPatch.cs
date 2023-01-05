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
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Patches
{
    /// <summary>
    ///     A Harmony patch for refreshing Twitch Toolkit's internal viewer
    ///     list.
    /// </summary>
    /// <remarks>
    ///     This patch also fixes a "hanging" situation with Twitch Toolkit,
    ///     where if ToolkitCore wasn't properly set up, the game would
    ///     freeze for a period of time.
    /// </remarks>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class RefreshViewersPatch
    {
        private const int ErrorId = 82938492;

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Viewers), nameof(Viewers.RefreshViewers));
        }

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

        private static bool Prefix()
        {
            if (ToolkitCoreSettings.channel_username.NullOrEmpty())
            {
                Log.ErrorOnce(@"<color=""#ff6b00"">ToolkitUtils :: ToolkitCore isn't fully set up. Your viewer list won't be refreshed.</color>", ErrorId);

                return false;
            }

            Task.Run(
                    async () =>
                    {
                        if (Viewers.jsonallviewers.NullOrEmpty())
                        {
                            Viewers.jsonallviewers =
                                @"{""_links"":{},""chatter_count"":0,""chatters"":{""broadcaster"":[],""vips"":[],""moderators"":[],""staff"":[],""admins"":[],""global_mods"":[],""viewers"":[]}}";
                        }

                        string result = await ViewerListWorker.GetChatters();

                        if (result.NullOrEmpty())
                        {
                            return;
                        }

                        Viewers.jsonallviewers = result;
                        TkUtils.Logger.Info(result);
                    }
                )
               .ConfigureAwait(false);

            return false;
        }
    }
}
