using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SirRandoo.ToolkitUtils.Helpers;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
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
    }
}
