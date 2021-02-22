// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
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
        [JsonIgnore] private string finalDescription;
        public string Name;
        [JsonIgnore] private TraitData traitData;
        [JsonIgnore] private TraitDef traitDef;

        public TraitData Data => traitData ??= new TraitData();

        // Legacy support
        [UsedImplicitly]
        public string Description
        {
            get
            {
                return finalDescription ??= TraitDef.DataAtDegree(Degree)
                   .description.Replace("PAWN_nameDef", "Timmy")
                   .Replace("PAWN_pronoun", "Prohe".Localize())
                   .Replace("PAWN_objective", "ProhimObj".Localize())
                   .Replace("PAWN_possessive", "Prohis".Localize())
                   .Replace("{", "")
                   .Replace("}", "")
                   .Replace("[", "")
                   .Replace("]", "");
            }
        }

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
                try
                {
                    container.AddRange(
                        degreeData.statOffsets.Where(s => s?.stat?.Worker != null)
                           .Select(o => $"{o.ValueToStringAsOffset} {o.stat.label}")
                    );
                }
                catch (Exception e)
                {
                    LogHelper.Warn(
                        $"Could not serialize stat offsets for the trait {degreeData.label}! -- {e.GetType().Name}({e.Message})"
                    );
                }
            }

            if (!degreeData.statFactors.NullOrEmpty())
            {
                try
                {
                    container.AddRange(
                        degreeData.statFactors.Where(s => s?.stat?.Worker != null)
                           .Select(f => $"{f.ToStringAsFactor} {f.stat.label}")
                    );
                }
                catch (Exception e)
                {
                    LogHelper.Warn(
                        $"Could not serialize stat factors for the {degreeData.label}! -- {e.GetType().Name}({e.Message})"
                    );
                }
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

            defaultName = TraitDef.DataAtDegree(Degree).label.StripTags().ToToolkit();
            return defaultName;
        }
    }
}
