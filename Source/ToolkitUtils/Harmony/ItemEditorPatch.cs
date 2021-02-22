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

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    [HarmonyPatch(typeof(Store_ItemEditor), nameof(Store_ItemEditor.UpdateStoreItemList))]
    public static class ItemEditorPatch
    {
        public static void Finalizer()
        {
            int? itemCount = Data.Items?.Count;
            int ttkItemCount = StoreInventory.items.Count;

            if (itemCount >= ttkItemCount)
            {
                if (itemCount > ttkItemCount)
                {
                    Data.Items = Data.Items.Where(i => StoreInventory.items.Contains(i.Item)).ToList();
                }

                return;
            }

            LogHelper.Info(
                "ToolkitUtils' item container count didn't match what Twitch Toolkit found; rebuilding list..."
            );
            Data.Items = StoreDialog.ValidateContainers().ToList();
        }
    }
}
