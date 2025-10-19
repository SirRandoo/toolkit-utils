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
// with ToolkitUtils.Interactions.Incidents. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Extensions;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class BuyBackstoryGroup
{
    /// <summary>The Backstory class provides command handlers for randomizing the backstory of a viewer's pawn in the game.</summary>
    [Group("backstory")]
    [StaticConstructorOnStartup]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class Backstory(ExecutionContext context, ILetterService letterService) : CommandGroup
    {
        private static IReadOnlyList<BackstoryDef> _adulthoodBackstories = DefDatabase<BackstoryDef>.AllDefs.Where(d => d.slot == BackstorySlot.Adulthood).ToList();
        private static IReadOnlyList<BackstoryDef> _childhoodBackstories = DefDatabase<BackstoryDef>.AllDefs.Where(d => d.slot == BackstorySlot.Childhood).ToList();

        /// <summary>Randomizes the adulthood backstory of a viewer's pawn and updates all relevant states.</summary>
        /// <returns>A task representing the asynchronous operation, containing the result of the randomization.</returns>
        [Command("adulthood")]
        public async Task<Result> RandomizeAdulthoodAsync()
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.GetPawnRequiredError(context.Invoker));

            IReadOnlyList<BackstoryDef> candidates = await FilterCandidates(_adulthoodBackstories, pawn.story.traits.allTraits);

            if (candidates.Count <= 0) return Result.Fail(TranslationService.Instance.GetAdulthoodConflictError(pawn));

            BackstoryDef newBackstory = candidates.RandomElement();
            BackstoryDef previous = pawn.story.Adulthood;
            pawn.story.Adulthood = newBackstory;

            await MainThreadExtensions.OnMainAsync(pawn.Notify_DisabledWorkTypesChanged);

            if (pawn.workSettings != null) await MainThreadExtensions.OnMainAsync(pawn.workSettings.Notify_DisabledWorkTypesChanged);
            if (pawn.skills != null) await MainThreadExtensions.OnMainAsync(pawn.skills.Notify_SkillDisablesChanged);

            context.Invoker.Charge(StoreIncidentDefOfs.RandomAdulthood);

            string backstoryTitleForPawn = await MainThreadExtensions.OnMainAsync(newBackstory.TitleCapFor, pawn.gender);
            string previousBackstoryTitleForPawn = await MainThreadExtensions.OnMainAsync(previous.TitleCapFor, pawn.gender);

            var formatParameters = new
            {
                ViewerName = context.Invoker.Name, PreviousBackstoryTitle = previousBackstoryTitleForPawn, NewBackstoryTitle = backstoryTitleForPawn,
            };

            await context.SendReplyAsync(TranslationService.Instance.GetTranslation("TKUtils.Responses.Backstory.Adulthood.Randomized").Format(formatParameters));
            await letterService.SendNeutralLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Backstory.Adulthood.Randomized.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Backstory.Adulthood.Randomized.Description").Format(formatParameters),
                pawn
            );

            return Result.Ok();
        }

        /// <summary>Randomizes the childhood backstory of a viewer's pawn and updates all relevant states.</summary>
        /// <returns>A task representing the asynchronous operation, containing the result of the randomization.</returns>
        [Command("childhood")]
        public async Task<IResult> RandomizeChildhoodAsync()
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.GetPawnRequiredError(context.Invoker));

            IReadOnlyList<BackstoryDef> candidates = await FilterCandidates(_childhoodBackstories, pawn.story.traits.allTraits);

            if (candidates.Count <= 0) return Result.Fail(TranslationService.Instance.GetChildhoodConflictError(pawn));

            BackstoryDef newBackstory = candidates.RandomElement();

            BackstoryDef previous = pawn.story.Childhood;
            pawn.story.Childhood = newBackstory;

            await MainThreadExtensions.OnMainAsync(pawn.Notify_DisabledWorkTypesChanged);

            if (pawn.workSettings != null) await MainThreadExtensions.OnMainAsync(pawn.workSettings.Notify_DisabledWorkTypesChanged);
            if (pawn.skills != null) await MainThreadExtensions.OnMainAsync(pawn.skills.Notify_SkillDisablesChanged);

            context.Invoker.Charge(StoreIncidentDefOfs.RandomChildhood);

            string backstoryTitleForPawn = await MainThreadExtensions.OnMainAsync(newBackstory.TitleCapFor, pawn.gender);
            string previousBackstoryTitleForPawn = await MainThreadExtensions.OnMainAsync(previous.TitleCapFor, pawn.gender);
            var formatParameters = new
            {
                ViewerName = context.Invoker.Name, PreviousBackstoryTitle = previousBackstoryTitleForPawn, NewBackstoryTitle = backstoryTitleForPawn,
            };

            await context.SendReplyAsync(TranslationService.Instance.GetTranslation("TKUtils.Responses.Backstory.Childhood.Randomized").Format(formatParameters));
            await letterService.SendNeutralLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Backstory.Childhood.Randomized.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Backstory.Childhood.Randomized.Description").Format(formatParameters),
                pawn
            );

            return Result.Ok();
        }

        private static ValueTask<IReadOnlyList<BackstoryDef>> FilterCandidates(IReadOnlyList<BackstoryDef> candidates, IReadOnlyList<Trait> pawnTraits)
        {
            var container = new List<BackstoryDef>();

            for (var i = 0; i < candidates.Count; i++)
            {
                BackstoryDef def = candidates[i];

                if (def.disallowedTraits.Count <= 0)
                {
                    container.Add(def);

                    continue;
                }

                for (var i1 = 0; i1 < def.disallowedTraits.Count; i1++)
                {
                    BackstoryTrait trait = def.disallowedTraits[i1];
                    Trait? pawnTrait = pawnTraits.FirstOrDefault(t => IsTraitDisallowed(trait, t));

                    if (pawnTrait == null) container.Add(def);
                }
            }

            return new ValueTask<IReadOnlyList<BackstoryDef>>(container);
        }

        private static bool IsTraitDisallowed(BackstoryTrait backstoryTrait, Trait trait)
        {
            if (backstoryTrait.def != trait.def) return false;

            return backstoryTrait.degree == trait.Degree;
        }
    }
}
