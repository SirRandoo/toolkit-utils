using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TraitItem
    {
        public bool CanAdd;
        public bool CanRemove;
        [JsonProperty("addPrice")] public int CostToAdd;
        [JsonProperty("removePrice")] public int CostToRemove;
        [JsonIgnore] private string defaultName;

        public string DefName;
        public int Degree;
        public string Name;
        [JsonIgnore] private TraitData traitData;
        [JsonIgnore] private TraitDef traitDef;

        public TraitData Data => traitData ??= new TraitData();

        // Legacy support
        [UsedImplicitly] public string Description => TraitDef.DataAtDegree(Degree).description;

        [UsedImplicitly]
        public string[] Stats
        {
            get
            {
                if (Data.Stats.NullOrEmpty())
                {
                    FetchStats();
                }

                return Data.Stats;
            }
        }

        [UsedImplicitly]
        public string[] Conflicts
        {
            get
            {
                if (Data.Conflicts.NullOrEmpty())
                {
                    FetchConflicts();
                }

                return Data.Conflicts;
            }
        }

        public bool BypassLimit => Data.CanBypassLimit;

        [JsonIgnore]
        public TraitDef TraitDef =>
            traitDef ??= DefDatabase<TraitDef>.AllDefs.FirstOrDefault(t => t.defName.Equals(DefName));

        private void FetchStats()
        {
            var container = new List<string>();

            TraitDegreeData degreeData = TraitDef.DataAtDegree(Degree);

            if (!degreeData.disallowedInspirations.NullOrEmpty())
            {
                container.AddRange(
                    degreeData.disallowedInspirations.Select(i => "TKUtils.Trait.Inspiration".Localize(i.LabelCap))
                );
            }

            if (!degreeData.disallowedMentalStates.NullOrEmpty())
            {
                container.AddRange(
                    degreeData.disallowedMentalStates.Select(b => "TKUtils.Trait.MentalState".Localize(b.LabelCap))
                );
            }

            if (ModLister.RoyaltyInstalled)
            {
                if (!degreeData.disallowedMeditationFocusTypes.NullOrEmpty())
                {
                    container.AddRange(
                        degreeData.disallowedMeditationFocusTypes.Select(
                            f => "TKUtils.Trait.MeditationDisabled".Localize(f.LabelCap)
                        )
                    );
                }

                if (!degreeData.allowedMeditationFocusTypes.NullOrEmpty())
                {
                    container.AddRange(
                        degreeData.allowedMeditationFocusTypes.Select(
                            f => "TKUtils.Trait.MeditationEnabled".Localize(f.LabelCap)
                        )
                    );
                }
            }

            if (!degreeData.theOnlyAllowedMentalBreaks.NullOrEmpty())
            {
                container.AddRange(
                    degreeData.theOnlyAllowedMentalBreaks.Select(
                        b => "TKUtils.Trait.MentalBreak".Localize(b.ToString())
                    )
                );
            }

            if (degreeData.skillGains.Count > 0)
            {
                container.AddRange(
                    degreeData.skillGains.Select(pair => $"{pair.Value.ToStringWithSign()} {pair.Key.skillLabel}")
                );
            }

            if (!degreeData.statOffsets.NullOrEmpty())
            {
                container.AddRange(degreeData.statOffsets.Select(o => $"{o.ValueToStringAsOffset} {o.stat.label}"));
            }

            if (!degreeData.statFactors.NullOrEmpty())
            {
                container.AddRange(degreeData.statFactors.Select(f => $"{f.ToStringAsFactor} {f.stat.label}"));
            }

            Data.Stats = container.ToArray();
        }

        private void FetchConflicts()
        {
            var container = new List<string>();

            foreach (TraitDef conflict in TraitDef.conflictingTraits)
            {
                container.AddRange(conflict.ToTraitItems().Select(t => t.Name));
            }

            Data.Conflicts = container.ToArray();
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
                traitData = new TraitData
                {
                    CanBypassLimit = trait.BypassLimit,
                    Conflicts = new string[] { },
                    CustomName = false,
                    Stats = new string[] { }
                }
            };
        }

        public string GetDefaultName(bool invalidate = false)
        {
            if (defaultName != null && !invalidate)
            {
                return defaultName;
            }

            defaultName = (TraitDef.degreeDatas != null ? traitDef.DataAtDegree(Degree).label : TraitDef.label)
               .StripTags()
               .ToToolkit();
            return defaultName;
        }
    }
}
