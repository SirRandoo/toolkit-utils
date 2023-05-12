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
using ToolkitUtils.Windows;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Patches
{
    /// <summary>
    ///     A Harmony patch for inserting a "Purge" button in the Viewers
    ///     dialog. The purge button is responsible for opening the
    ///     <see cref="PurgeViewersDialog"/>.
    /// </summary>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class PurgeButtonPatch
    {
        private static readonly IRimLogger Logger = new RimLogger("TKU.Patches.PurgeButton");
        
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Window_Viewers), nameof(Window_Viewers.DoWindowContents));
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

        private static void Postfix(Rect inRect)
        {
            string text = "TKUtils.Buttons.Purge".TranslateSimple();
            var canvas = new Rect(inRect.width - 60f, 0f, 60f, 28f);

            if (Widgets.ButtonText(canvas, text))
            {
                Find.WindowStack.Add(new PurgeViewersDialog());
            }
        }
    }
}
