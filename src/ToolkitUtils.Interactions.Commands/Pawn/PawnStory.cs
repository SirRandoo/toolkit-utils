// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Interactions.Commands. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NetEscapades.EnumGenerators;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Presentation;
using ToolkitUtils.Mod.Services;
using Verse;

[assembly: EnumExtensions<WorkTags>]

namespace ToolkitUtils.Interactions.Commands;

[Group("pawn")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnStory(ExecutionContext context) : CommandGroup
{
    [Command("story")]
    public async Task<Result> GetPawnStoryAsync()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        var parts = new List<string>
        {
            $"{TranslationService.Instance.GetTranslation("Backstory")}: {string.Join(separator: ", ", pawn.story.AllBackstories.Select(b => b.title.CapitalizeFirst()))}",
        };

        if (!pawn.story.title.NullOrEmpty()) parts.Add(pawn.story.TitleCap);

        bool isRoyal = pawn.royalty?.MostSeniorTitle != null;

        switch (pawn.gender)
        {
            case Gender.Female: parts.Add(isRoyal ? UnicodeCharacter.Princess.Codepoint : UnicodeCharacter.Female.Codepoint); break;
            case Gender.Male:   parts.Add(isRoyal ? UnicodeCharacter.Prince.Codepoint : UnicodeCharacter.Male.Codepoint); break;
            case Gender.None:   parts.Add(isRoyal ? UnicodeCharacter.Crown.Codepoint : UnicodeCharacter.MediumWhiteCircle.Codepoint); break;
            default:            parts.Add(isRoyal ? UnicodeCharacter.Crown.Codepoint : string.Empty); break;
        }

        parts.Add(string.Format(TranslationService.Instance.GetTranslation("AgeIndicator").CapitalizeFirst(), pawn.ageTracker.AgeNumberString));

        WorkTags workTags = pawn.story.DisabledWorkTagsBackstoryAndTraits;

        if (workTags != WorkTags.None)
        {
            var filteredTags = new List<string>();

            foreach (WorkTags tag in WorkTagHelper.GetAllSelectedItems(pawn.story.DisabledWorkTagsBackstoryAndTraits))
            {
                if (tag is WorkTags.None) continue;

                string translatedLabel = await RouterService.Instance.RouteToMainAsync(WorkTypeDefsUtility.LabelTranslated, tag);

                filteredTags.Add(translatedLabel.CapitalizeFirst());
            }

            parts.Add($"{TranslationService.Instance.GetTranslation("IncapableOf")}: {string.Join(separator: ", ", filteredTags)}");
        }

        List<Trait> traits = pawn.story.traits.allTraits;

        if (traits.Count > 0)
        {
            parts.Add(
                string.Format(
                    format: "{0}: {1}",
                    TranslationService.Instance.GetTranslation("Traits"),
                    string.Join(separator: ", ", traits.Select(t => RichTextHelper.StripTags(t.LabelCap)))
                )
            );
        }

        return Result.Ok(string.Join(separator: " | ", parts));
    }

    private static class WorkTagHelper
    {
        private static readonly WorkTags[] AllWorkTags = WorkTagsExtensions.GetValues();

        public static IEnumerable<WorkTags> GetAllSelectedItems(WorkTags tags)
        {
            for (var i = 0; i < AllWorkTags.Length; i++)
            {
                WorkTags flag = AllWorkTags[i];

                if (tags.HasFlagFast(flag)) yield return flag;
            }
        }
    }
}
