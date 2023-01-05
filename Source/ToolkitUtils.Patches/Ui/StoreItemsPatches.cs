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
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using TwitchToolkit.Windows;
using StoreIncidentEditor = TwitchToolkit.Windows.StoreIncidentEditor;

namespace SirRandoo.ToolkitUtils.Patches
{
    /// <summary>
    ///     A Harmony patch for replacing Twitch Toolkit's item store dialog
    ///     with Utils'.
    /// </summary>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class SettingsStorePatch
    {
        private static ConstructorInfo _oldClassConstructor;
        private static ConstructorInfo _newClassConstructor;
        private static Type _oldClassType;
        private static Type _newClassType;

        private static bool Prepare()
        {
            _newClassType = typeof(StoreDialog);
            _oldClassType = typeof(StoreItemsWindow);
            _newClassConstructor = typeof(StoreDialog).GetConstructor(new Type[] { });
            _oldClassConstructor = typeof(StoreItemsWindow).GetConstructor(new Type[] { });

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
            yield return AccessTools.Method(typeof(Settings_Store), nameof(Settings_Store.DoWindowContents));
            yield return AccessTools.Method(typeof(StoreIncidentEditor), nameof(StoreIncidentEditor.DoWindowContents));
        }

        [ItemNotNull]
        private static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Is(OpCodes.Newobj, _oldClassConstructor))
                {
                    instruction.operand = _newClassConstructor;
                }
                else if (instruction.Is(OpCodes.Ldtoken, _oldClassType))
                {
                    instruction.operand = _newClassType;
                }

                yield return instruction;
            }
        }
    }
}
