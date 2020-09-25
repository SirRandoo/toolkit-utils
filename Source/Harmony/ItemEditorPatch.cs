using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Store;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    [HarmonyPatch(typeof(Store_ItemEditor), nameof(Store_ItemEditor.UpdateStoreItemList))]
    public static class ItemEditorPatch
    {
        public static void Postfix()
        {
            if (Data.Items?.Count >= StoreInventory.items.Count)
            {
                return;
            }

            TkLogger.Info("ToolkitUtils' item containers were incomplete; rebuilding...");
            Data.Items = StoreDialog.GenerateContainers().ToList();
        }
    }
}
