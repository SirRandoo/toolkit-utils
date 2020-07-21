using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class PawnKindItem
    {
        [JsonProperty("price")] public int Cost;
        [CanBeNull] public PawnKindData Data;
        public string DefName;
        public bool Enabled;
        public string Name;

        public static PawnKindItem MigrateFrom(XmlRace race)
        {
            return new PawnKindItem
            {
                DefName = race.DefName,
                Enabled = race.Enabled,
                Name = race.Name,
                Cost = race.Price,
                Data = new PawnKindData {CustomName = false}
            };
        }
    }
}
