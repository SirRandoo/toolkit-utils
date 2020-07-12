using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    public class PawnKindData
    {
        public bool CustomName;

        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(null)]
        public KarmaType? KarmaType;
    }
}
