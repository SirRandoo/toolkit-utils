using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    [HarmonyPatch(typeof(Store_ItemEditor), nameof(Store_ItemEditor.UpdateStoreItemList))]
    public static class ItemEditorPatch
    {
        public static void Postfix()
        {
            if (!Data.Items.NullOrEmpty())
            {
                return;
            }

            TkLogger.Info("ToolkitUtils' item containers were null or empty; regenerating...");
            Data.Items = StoreDialog.GenerateContainers().ToList();
        }
    }
}
