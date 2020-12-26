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
