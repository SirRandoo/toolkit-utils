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
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using ToolkitCore;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class StartAsyncPatch
    {
        private const int ErrorId = 938298212;

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(TwitchWrapper), "StartAsync");
        }

        public static bool Prefix()
        {
            if (!ToolkitCoreSettings.channel_username.NullOrEmpty() && !ToolkitCoreSettings.oauth_token.NullOrEmpty())
            {
                return true;
            }

            Log.ErrorOnce(
                @"<color=""#ff6b00"">ToolkitUtils :: Could not connect bot -- ToolkitCore isn't fully set up.</color>",
                ErrorId
            );
            return false;
        }
    }
}
