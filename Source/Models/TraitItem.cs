using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TraitItem
    {
        public bool CanAdd;
        public bool CanRemove;
        [JsonProperty("addPrice")] public int CostToAdd;

        [JsonProperty("removePrice")] public int CostToRemove;
        [CanBeNull] public TraitData Data;
        public string DefName;
        public int Degree;
        public string Name;

        // Legacy support
        public string[] Stats => Data?.Stats;
        public string[] Conflicts => Data?.Conflicts;
        public bool BypassLimit => Data?.CanBypassLimit ?? false;

        public bool CompareToInput(string input)
        {
            input = input.ToToolkit();

            if (input.Equals(Name.ToToolkit(), StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }

            return input.Equals(DefName, StringComparison.InvariantCulture);
        }

        public static TraitItem MigrateFrom(XmlTrait trait)
        {
            return new TraitItem
            {
                CanAdd = trait.CanAdd,
                CanRemove = trait.CanRemove,
                CostToAdd = trait.AddPrice,
                CostToRemove = trait.RemovePrice,
                DefName = trait.DefName,
                Degree = trait.Degree,
                Name = trait.Name,
                Data = new TraitData
                {
                    CanBypassLimit = trait.BypassLimit,
                    Conflicts = new string[] { },
                    CustomName = false,
                    Stats = new string[] { }
                }
            };
        }

        public string GetDefaultName()
        {
            TraitDef traitDef = DefDatabase<TraitDef>.AllDefs.FirstOrDefault(t => t.defName.Equals(DefName));

            return (traitDef?.degreeDatas != null ? traitDef.DataAtDegree(Degree).label : traitDef?.label) ?? DefName;
        }
    }
}
