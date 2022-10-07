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
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit;
using TwitchToolkit.Settings;

namespace SirRandoo.ToolkitUtils.Harmony
{
    /// <summary>
    ///     A Harmony patch for removing the lower limit on Twitch Toolkit's
    ///     karma minimum.
    /// </summary>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class KarmaMinimumPatch
    {
        private static FieldInfo _settingMarker;

        private static bool Prepare()
        {
            _settingMarker ??= AccessTools.Field(typeof(ToolkitSettings), nameof(ToolkitSettings.KarmaMinimum));

            return true;
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

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Settings_Karma), nameof(Settings_Karma.DoWindowContents));
        }

        [ItemNotNull]
        private static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var marker = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Is(OpCodes.Ldsflda, _settingMarker))
                {
                    marker = true;
                }

                if (instruction.opcode == OpCodes.Ldc_R4 && marker)
                {
                    instruction.operand = -1E+09F;
                    marker = false;
                }

                yield return instruction;
            }
        }
    }
}
