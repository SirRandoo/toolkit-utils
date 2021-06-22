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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TraitItem : IShopItemBase
    {
        [DataMember(Name = "canAdd")] public bool CanAdd;
        [DataMember(Name = "canRemove")] public bool CanRemove;
        [IgnoreDataMember] private int cost;
        [DataMember(Name = "addPrice")] public int CostToAdd;
        [DataMember(Name = "removePrice")] public int CostToRemove;
        [IgnoreDataMember] private TraitData data;
        [IgnoreDataMember] private string defaultFeminineName;
        [IgnoreDataMember] private string defaultMasculineName;
        [IgnoreDataMember] private string defaultName;

        [DataMember(Name = "degree")] public int Degree;
        [IgnoreDataMember] private string finalDescription;
        [IgnoreDataMember] private TraitDef traitDef;

        // Legacy support
        [CanBeNull]
        [DataMember(Name = "description")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public string Description
        {
            get
            {
                return finalDescription ??= TraitDef?.DataAtDegree(Degree)
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

        [NotNull]
        [DataMember(Name = "stats")]
        public string[] Stats
        {
            get
            {
                if (TraitData?.Stats.NullOrEmpty() ?? true)
                {
                    UpdateStats();
                }

                return TraitData?.Stats ?? new string[0];
            }
        }

        [NotNull]
        [DataMember(Name = "conflicts")]
        public string[] Conflicts
        {
            get
            {
                if (TraitData?.Conflicts.NullOrEmpty() ?? false)
                {
                    UpdateConflicts();
                }

                return TraitData?.Conflicts ?? new string[0];
            }
        }

        [DataMember(Name = "bypassLimit")] public bool BypassLimit => TraitData?.CanBypassLimit ?? false;

        [CanBeNull]
        [IgnoreDataMember]
        public TraitDef TraitDef =>
            traitDef ??= DefDatabase<TraitDef>.AllDefs.FirstOrDefault(t => t.defName.Equals(DefName));

        [CanBeNull]
        [DataMember(Name = "data")]
        public TraitData TraitData
        {
            get => data ??= new TraitData();
            set => data = value;
        }

        [DataMember(Name = "defName")] public string DefName { get; set; }

        [IgnoreDataMember]
        public bool Enabled
        {
            get => CanAdd || CanRemove;
            set => throw new ReadOnlyException();
        }

        [DataMember(Name = "name")] public string Name { get; set; }

        [IgnoreDataMember]
        public int Cost
        {
            get => cost;
            set
            {
                CostToAdd = value;
                CostToRemove = value;
                cost = value;
            }
        }

        [IgnoreDataMember]
        public IShopDataBase Data
        {
            get => data;
            set => data = value as TraitData;
        }

        public void ResetName()
        {
            if (TraitDef != null)
            {
                Name = TraitDef.label.ToToolkit();
            }
        }

        public void ResetPrice()
        {
            CostToAdd = 3500;
            CostToRemove = 5500;
        }

        public void ResetData()
        {
            TraitData?.Reset();
        }

        public void ResetAddPrice()
        {
            CostToAdd = 3500;
        }

        public void ResetRemovePrice()
        {
            CostToRemove = 5500;
        }

        private void UpdateStats()
        {
            if (TraitDef == null)
            {
                return;
            }

            TraitData ??= new TraitData();
            var container = new List<string>();
            var builder = new StringBuilder();

            TraitDegreeData degreeData = TraitDef.DataAtDegree(Degree);

            container.AddRange(GetDisallowedInspirations(degreeData));
            container.AddRange(GetDisallowedMentalStates(degreeData));
            container.AddRange(GetDisallowedMeditationTypes(degreeData));
            container.AddRange(GetAllowedMeditationTypes(degreeData));
            container.AddRange(GetOnlyAllowedMentalBreaks(degreeData));
            container.AddRange(GetSkillGains(degreeData));
            container.AddRange(GetStatOffsets(degreeData, builder));
            container.AddRange(GetStatFactors(degreeData, builder));

            if (builder.Length > 0)
            {
                builder.Insert(
                    0,
                    $@"Could not obtain all data for the trait ""{Name}"" -- Below are the errors encountered:\n"
                );
                LogHelper.Warn(builder.ToString());
            }

            TraitData.Stats = container.ToArray();
        }

        [NotNull]
        private static IEnumerable<string> GetDisallowedInspirations([NotNull] TraitDegreeData data)
        {
            return data.disallowedInspirations?.Select(
                       def => "TKUtils.Trait.Inspiration".Localize(def.label?.CapitalizeFirst() ?? def.defName)
                   )
                   ?? new string[0];
        }

        [NotNull]
        private static IEnumerable<string> GetDisallowedMentalStates([NotNull] TraitDegreeData data)
        {
            return data.disallowedMentalStates?.Select(
                       def => "TKUtils.Trait.MentalState".Localize(def.label?.CapitalizeFirst() ?? def.defName)
                   )
                   ?? new string[0];
        }

        private static IEnumerable<string> GetDisallowedMeditationTypes(TraitDegreeData data)
        {
            if (!ModLister.RoyaltyInstalled || data.disallowedMentalStates.NullOrEmpty())
            {
                yield break;
            }

            foreach (string s in data.disallowedMeditationFocusTypes.Select(
                def => "TKUtils.Trait.MeditationDisabled".Localize(def.label?.CapitalizeFirst() ?? def.defName)
            ))
            {
                yield return s;
            }
        }

        private static IEnumerable<string> GetAllowedMeditationTypes(TraitDegreeData data)
        {
            if (!ModLister.RoyaltyInstalled || data.allowedMeditationFocusTypes.NullOrEmpty())
            {
                yield break;
            }

            foreach (string s in data.allowedMeditationFocusTypes.Select(
                def => "TKUtils.Trait.MeditationEnabled".Localize(def.label?.CapitalizeFirst() ?? def.defName)
            ))
            {
                yield return s;
            }
        }

        [NotNull]
        private static IEnumerable<string> GetOnlyAllowedMentalBreaks([NotNull] TraitDegreeData data)
        {
            return data.theOnlyAllowedMentalBreaks?.Select(
                       def => "TKUtils.Trait.MentalBreak".Localize(def.label ?? def.defName)
                   )
                   ?? new string[0];
        }

        [NotNull]
        private static IEnumerable<string> GetSkillGains([NotNull] TraitDegreeData data)
        {
            if (data.skillGains == null)
            {
                yield break;
            }

            var builder = new StringBuilder();

            foreach ((SkillDef skill, int value) in data.skillGains)
            {
                string result = null;
                try
                {
                    result = $"{value.ToStringWithSign()} {skill.skillLabel ?? skill.defName}";
                }
                catch (Exception)
                {
                    builder.AppendLine($" - {skill.skillLabel ?? skill.defName ?? "UNPROCESSABLE"}");
                }

                if (!result.NullOrEmpty())
                {
                    yield return result;
                }
            }

            if (builder.Length <= 0)
            {
                yield break;
            }

            builder.Insert(0, $@"The following stats could not be processed for ""{data.label}"":\n");
            LogHelper.Warn(builder.ToString());
        }

        private static IEnumerable<string> GetStatOffsets([NotNull] TraitDegreeData data, StringBuilder messages)
        {
            if (data.statOffsets.NullOrEmpty())
            {
                yield break;
            }

            foreach (StatModifier modifier in data.statOffsets.Where(o => o?.stat?.Worker != null))
            {
                string m;

                try
                {
                    m = $"{modifier.ValueToStringAsOffset} {modifier.stat.label ?? modifier.stat.defName}";
                }
                catch (Exception e)
                {
                    messages.AppendLine(
                        $"  - Could not get stat offsets for the trait {data.label} -> {e.GetType().Name}({e.Message})"
                    );
                    messages.AppendLine(
                        $"    - Malformed stat: {modifier?.stat?.label ?? modifier?.stat?.defName ?? "{{NULL}}"}"
                    );
                    continue;
                }

                yield return m;
            }
        }

        private static IEnumerable<string> GetStatFactors([NotNull] TraitDegreeData data, StringBuilder messages)
        {
            if (data.statFactors.NullOrEmpty())
            {
                yield break;
            }

            foreach (StatModifier modifier in data.statFactors.Where(s => s?.stat?.Worker != null))
            {
                string m;

                try
                {
                    m = $"{modifier.ToStringAsFactor} {modifier.stat.label ?? modifier.stat.defName}";
                }
                catch (Exception e)
                {
                    messages.AppendLine(
                        $"  - Could not get stat factors for the trait {data.label} -> {e.GetType().Name}({e.Message})"
                    );
                    messages.AppendLine(
                        $"    - Malformed stat: {modifier?.stat?.label ?? modifier?.stat?.defName ?? "{{NULL}}"}"
                    );
                    continue;
                }

                yield return m;
            }
        }

        private void UpdateConflicts()
        {
            TraitData ??= new TraitData();

            if (TraitDef?.conflictingTraits == null)
            {
                return;
            }

            var container = new List<string>();

            foreach (TraitDef conflict in TraitDef.conflictingTraits)
            {
                container.AddRange(conflict.ToTraitItems().Select(t => t.Name));
            }

            TraitData.Conflicts = container.ToArray();
        }

        [CanBeNull]
        public string GetDefaultName(Gender? gender = null, bool invalidate = false)
        {
            switch (gender)
            {
                case Gender.Male:
                    if (invalidate || defaultMasculineName.NullOrEmpty())
                    {
                        defaultMasculineName = TraitDef?.DataAtDegree(Degree)
                           .GetLabelFor(Gender.Male)
                           .StripTags()
                           .ToToolkit();
                    }

                    return defaultMasculineName;
                case Gender.Female:
                    if (invalidate || defaultFeminineName.NullOrEmpty())
                    {
                        defaultFeminineName = TraitDef?.DataAtDegree(Degree)
                           .GetLabelFor(Gender.Female)
                           .StripTags()
                           .ToToolkit();
                    }

                    return defaultFeminineName;
                default:
                    if (invalidate || defaultName.NullOrEmpty())
                    {
                        defaultName = TraitDef?.DataAtDegree(Degree).GetLabelFor(Gender.None).StripTags().ToToolkit();
                    }

                    return defaultName;
            }
        }
    }
}
