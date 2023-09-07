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
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Patches;

/// <summary>
///     A Harmony patch for ensuring Utils' items match Twitch Toolkit's
///     items.
/// </summary>
/// <remarks>
///     Prior to this, a weird oddity would occur where Utils would have
///     more, or less, items than Twitch Toolkit.
/// </remarks>
[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class ItemEditorPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Store_ItemEditor), nameof(Store_ItemEditor.LoadStoreItemList));
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

    private static void Postfix()
    {
        int? itemCount = Data.Items?.Count;
        int ttkItemCount = StoreInventory.items.Count;

        if (itemCount >= ttkItemCount)
        {
            if (itemCount > ttkItemCount)
            {
                Data.Items = Data.Items!.Where(i => StoreInventory.items.Contains(i.Item)).ToList();
            }

            return;
        }

        TkUtils.Logger.Info("Utils' item list didn't match Twitch Toolkit's; rebuilding list...");
        Data.Items = StoreDialog.ValidateContainers().ToList();
    }
}
