using System.Linq;
using HarmonyLib;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_ItemEditor), "UpdateStoreItemList")]
    public class StoreItemEditorPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            var inventory = StoreInventory.items;

            foreach (var item in inventory
                .Where(item => item.price > 0)
                .Where(item => TkUtils.ShopExpansion.Races.Any(r => r.DefName.Equals(item.defname))))
            {
                item.price = -10;
            }
        }
    }
}
