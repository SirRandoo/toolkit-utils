using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class PawnKindItem
    {
        [JsonProperty("price")] public int Cost;
        [JsonIgnore] private PawnKindData data;

        public string DefName;
        public bool Enabled;
        [JsonIgnore] private List<PawnKindDef> kinds;
        public string Name;

        [JsonIgnore]
        public IEnumerable<PawnKindDef> Kinds =>
            kinds ??= DefDatabase<PawnKindDef>.AllDefs.Where(k => k.race.defName.Equals(DefName)).ToList();

        [JsonIgnore]
        private PawnKindDef ColonistKindDef =>
            Kinds.FirstOrDefault(k => k.defaultFactionType.isPlayer) ?? Kinds.FirstOrFallback();

        public PawnKindData Data => data ??= new PawnKindData();


        public static PawnKindItem MigrateFrom(XmlRace race)
        {
            return new PawnKindItem
            {
                DefName = race.DefName, Enabled = race.Enabled, Name = race.Name, Cost = race.Price
            };
        }

        public string GetDefaultName()
        {
            return ColonistKindDef?.race.label.ToToolkit() ?? DefName;
        }
    }
}
