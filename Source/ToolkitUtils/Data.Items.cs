// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public static partial class Data
    {
        /// <summary>
        ///     A list of encapsulated <see cref="Item"/>s with Utils data
        ///     attached to them.
        /// </summary>
        public static List<ThingItem> Items { get; set; }

        /// <summary>
        ///     The associated data for items within Twitch Toolkit.
        /// </summary>
        /// <remarks>
        ///     Item data serves as a means of including additional data with
        ///     every item in a way Utils can manage independently of Twitch
        ///     Toolkit.
        /// </remarks>
        public static Dictionary<string, ItemData> ItemData { get; private set; }

        private static void ValidateItems()
        {
            Items = StoreDialog.GetTradeables().Select(t => new ThingItem { Thing = t }).ToList();
        }

        /// <summary>
        ///     Loads item data from the given file path.
        /// </summary>
        /// <param name="path">The file to load item data from</param>
        public static void LoadItemData(string path)
        {
            ItemData = LoadJson<Dictionary<string, ItemData>>(path, true) ?? new Dictionary<string, ItemData>();
        }

        /// <summary>
        ///     Loads item data from the given file path.
        /// </summary>
        /// <param name="path">The file to load item data from</param>
        public static async Task LoadItemDataAsync(string path)
        {
            ItemData = await LoadJsonAsync<Dictionary<string, ItemData>>(path, true) ?? new Dictionary<string, ItemData>();
        }

        /// <summary>
        ///     Saves item data to the given file.
        /// </summary>
        /// <param name="path">The file to save item data to</param>
        public static void SaveItemData(string path)
        {
            SaveJson(ItemData, path);
        }

        /// <summary>
        ///     Saves item data to the given file.
        /// </summary>
        /// <param name="path">The file to save item data to</param>
        public static async Task SaveItemDataAsync(string path)
        {
            await SaveJsonAsync(ItemData, path);
        }

        private static void ValidateItemData()
        {
            ItemData ??= new Dictionary<string, ItemData>();
            List<string> tradeables = StoreDialog.GetTradeables().Select(t => t.defName).ToList();
            List<string> toCull = ItemData.Keys.Where(dataKey => !tradeables.Contains(dataKey, StringComparer.OrdinalIgnoreCase)).ToList();

            foreach (string defName in toCull)
            {
                ItemData.Remove(defName);
            }

            foreach (string key in ItemData.Keys.ToList())
            {
                string caseSensitiveName = tradeables.Find(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(caseSensitiveName) && ItemData.Remove(key, out ItemData data))
                {
                    ItemData[caseSensitiveName] = data;
                }
            }

            var builder = new StringBuilder();

            foreach (ThingDef item in tradeables.Where(t => !ItemData.ContainsKey(t)).Select(i => DefDatabase<ThingDef>.GetNamed(i)))
            {
                ModContentPack contentPack = item.modContentPack;

                var data = new ItemData
                {
                    Version = Models.ItemData.CurrentVersion,
                    QuantityLimit = -1,
                    IsStuffAllowed = true,
                    IsUsable = GameHelper.GetDefaultUsability(item),
                    IsWearable = true,
                    IsEquippable = true
                };

                if (contentPack != null)
                {
                    data.Mod = contentPack.IsCoreMod ? "RimWorld" : contentPack.Name ?? "Unknown";
                }

                ItemData[item.defName] = data;

                try
                {
                    data.IsMelee = item.IsMeleeWeapon;
                    data.IsRanged = item.IsRangedWeapon;
                    data.IsWeapon = item.IsWeapon;
                }
                catch (Exception e)
                {
                    builder.Append($"Failed to gather weapon data for item '{item.label ?? "Unknown"}' from mod '{item.TryGetModName()}'");
                    builder.AppendLine($" -- Exception: {e.GetType().Name}({e.Message ?? "No message"})");
                }
            }

            foreach (KeyValuePair<string, ItemData> pair in ItemData.Where(data => data.Value.Version < Models.ItemData.CurrentVersion))
            {
                string defName = pair.Key;
                ItemData data = pair.Value;

                ThingItem item = Items.Find(i => i.DefName?.Equals(defName) == true);

                data.IsUsable = item?.Thing == null || GameHelper.GetDefaultUsability(item.Thing);
                data.IsWearable = true;
                data.IsEquippable = true;
                data.Version = Models.ItemData.CurrentVersion;
            }
        }

        /// <summary>
        ///     Loads items from the given partial data.
        /// </summary>
        /// <param name="partialData">A collection of partial data to load</param>
        public static void LoadItemPartial([NotNull] IEnumerable<ItemPartial> partialData)
        {
            var builder = new StringBuilder();

            foreach (ItemPartial partial in partialData)
            {
                if (partial.DefName == null)
                {
                    builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}\n");

                    continue;
                }

                ThingItem existing = Items.Find(i => i.DefName!.Equals(partial.DefName));

                if (existing == null)
                {
                    ThingDef thing = DefDatabase<ThingDef>.GetNamed(partial.DefName, false);
                    var item = Item.GetItemFromDefName(partial.DefName);

                    if (thing == null || item == null)
                    {
                        builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}\n");

                        continue;
                    }

                    item.price = partial.Cost;
                    Items.Add(new ThingItem { Thing = thing, Item = item, ItemData = partial.ItemData });

                    continue;
                }

                existing.Name = partial.Name;
                existing.Cost = partial.Cost;
                existing.ItemData = partial.ItemData;
                existing.Update();
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following items could not be loaded from the partial data provided:\n");
            TkUtils.Logger.Warn(builder.ToString());
        }
    }
}
