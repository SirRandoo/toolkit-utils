using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_ItemEditor), "UpdateStoreItemList")]
    public class Store_ItemEditor__UpdateStoreItemList
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (!TkSettings.ToolkitJson)
            {
                return true;
            }

            if (!Directory.Exists(Store_ItemEditor.dataPath))
            {
                Directory.CreateDirectory(Store_ItemEditor.dataPath);
            }

            if (StoreInventory.items == null)
            {
                return false;
            }

            var container = new List<string>();
            var builder = new StringBuilder();
            var things = DefDatabase<ThingDef>.AllDefsListForReading;
            foreach (var item in StoreInventory.items)
            {
                var thing = things.FirstOrDefault(t => t.defName.Equals(item.defname));

                if (thing == null)
                {
                    continue;
                }

                var category = thing.FirstThingCategory?.LabelCap.RawText ?? string.Empty;

                if (category.NullOrEmpty() && thing.race != null)
                {
                    category = "Animal";
                }

                builder.AppendLine("    {");
                builder.Append(@"      ""abr"": """)
                    .Append(item.abr)
                    .AppendLine(@""",");
                builder.Append(@"      ""price"": ")
                    .Append(item.price)
                    .AppendLine(",");
                builder.Append(@"      ""category"": """)
                    .Append(category)
                    .AppendLine(@""",");
                builder.Append(@"      ""defname"": """)
                    .Append(item.defname)
                    .AppendLine(@"""");
                builder.Append("    }");

                container.Add(builder.ToString());

                builder.Clear();
            }

            builder.Clear();
            builder.AppendLine("{");
            builder.AppendLine(@"  ""items"": [");
            builder.AppendLine(string.Join($",{Environment.NewLine}", container.ToArray()));
            builder.AppendLine("  ],");
            builder.Append(@"  ""total"": ")
                .Append(StoreInventory.items.Count.ToString())
                .AppendLine(",");
            builder.AppendLine(@"  ""utils"": true");
            builder.AppendLine("}");

            File.WriteAllText(Path.Combine(Store_ItemEditor.dataPath, "StoreItems.json"), builder.ToString());

            return false;
        }
    }
}
