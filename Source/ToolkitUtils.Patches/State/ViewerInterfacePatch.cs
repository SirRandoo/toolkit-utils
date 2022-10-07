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
using ToolkitCore;
using ToolkitCore.Database;
using ToolkitCore.Models;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    /// <summary>
    ///     A Harmony patch for fixing ToolkitCore's viewer database being
    ///     nulled occasionally.
    /// </summary>
    [HarmonyPatch]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static class ViewerInterfacePatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.PropertyGetter(typeof(Viewers), nameof(Viewers.All));
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

        private static void Prefix()
        {
            if (ToolkitData.globalDatabase == null)
            {
                TkUtils.Logger.Warn("ToolkitCore's global database was null. Recreating...");
                ToolkitData.globalDatabase = new GlobalDatabase();
            }

            if (!ToolkitData.globalDatabase.viewers.NullOrEmpty())
            {
                return;
            }

            TkUtils.Logger.Warn("ToolkitCore's viewer data was null. Recreating...");
            ToolkitData.globalDatabase.viewers = new List<Viewer>();
        }
    }
}
