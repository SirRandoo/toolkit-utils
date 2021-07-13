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
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Workers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
#if RW13
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ToolkitSettingsPatch
    {
        private static bool _warned;

        public static bool Prepare()
        {
            return ModLister.GetModWithIdentifier("hodlhodl.twitchtoolkit")?.VersionCompatible != true;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(TwitchToolkit.TwitchToolkit), "DoSettingsWindowContents");
        }

        public static bool Prefix(Rect inRect)
        {
            if (!_warned)
            {
                LogHelper.Warn(
                    new StringBuilder()
                       .Append(
                            "While Utils patches Twitch Toolkit to be compatible with 1.3, it's still an unsupported "
                        )
                       .Append("version of the game. Do not submit bug reports while using 1.3. Do not expect ")
                       .Append("everything to work.")
                       .ToString()
                );
                _warned = true;
            }

            ToolkitSettingsWorker.Draw(inRect);
            return false;
        }
    }
#endif
}
