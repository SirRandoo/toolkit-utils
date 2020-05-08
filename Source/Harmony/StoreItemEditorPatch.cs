using System.Linq;
using HarmonyLib;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Windows;
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
            var tradeables = StoreDialog.GetTradeables().ToHashSet();

            foreach (var item in inventory
                .Where(item => !item.defname.NullOrEmpty())
                .Where(item => item.price >= 0)
                .Where(item => TkUtils.ShopExpansion.Races.Any(r => r.DefName.Equals(item.defname))))
            {
                item.price = -10;
            }

            StoreInventory.items = inventory
                .Where(i => !i.defname.NullOrEmpty())
                .ToList();

            foreach (var item in StoreInventory.items.Where(i => i.abr.NullOrEmpty()))
            {
                var thing = tradeables.FirstOrDefault(i => i.defName.Equals(item.defname));

                if (thing == null)
                {
                    continue;
                }

                item.abr = thing.label.ToToolkit();
            }
        }
    }
}
