using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class Item
    {
        [JsonProperty("abr")] public string Abr;

        [JsonProperty("category")] public string Category;

        [JsonProperty("defname")] public string DefName;

        [JsonProperty("price")] public int Price;
    }
}
