using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class ItemList
    {
        public List<Item> Items;
        public int Total => Items.Count;
    }
}
