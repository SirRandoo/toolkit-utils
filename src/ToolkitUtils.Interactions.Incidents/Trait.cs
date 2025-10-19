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
using ToolkitUtils.Core.Extensions;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class TraitBuyGroup : CommandGroup
{
    [Group("trait")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class TraitGroup(ExecutionContext context, ILetterService letterService) : CommandGroup
    {
        [Command("add")]
        public async Task<Result> AddTraitAsync(TraitProduct product)
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

            Result canAddTrait = await CanAddTraitAsync(pawn, product);
            if (!canAddTrait.IsSuccess) return canAddTrait;

            var trait = new Trait(product.Def, product.Degree);

            await MainThreadExtensions.OnMainAsync(GivePawnTrait, pawn, trait);

            context.Invoker.Charge(StoreIncidentDefOfs.AddTrait);
            await context.SendReplyAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Trait.Add").Format(
                    new
                    {
                        TraitName = trait.Label,
                    }
                )
            );

            await letterService.SendNeutralLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Trait.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Trait.Add.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name, TraitName = trait.Label,
                    }
                ),
                pawn
            );

            return Result.Ok();
        }

        [Command("remove")]
        public async Task<Result> RemoveTraitAsync(TraitProduct product)
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
            if (pawn.story.traits.allTraits.Count <= 0) return Result.Fail(TranslationService.Instance.FormatNoTraitsForRemoval());

            Trait? targetTrait = pawn.story.traits.allTraits.Find(t => t.def == product.Def && t.Degree == product.Degree);

            if (targetTrait == null) return Result.Fail(TranslationService.Instance.FormatNoTraitFoundForRemoval(product));

            Result canRemoveTrait = await CanRemoveTraitAsync(pawn, product, targetTrait);

            if (!canRemoveTrait.IsSuccess) return canRemoveTrait;

            Result<Transaction> transaction = context.Invoker.ReserveCoins(product.PriceToRemove);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(product.PriceToRemove, context.Invoker.Coins));

            await TakePawnTrait(pawn, targetTrait);
            transaction.Value.PostTransaction();

            await context.SendReplyAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Trait.Remove").Format(
                    new
                    {
                        TraitName = targetTrait.Label,
                    }
                )
            );
            await letterService.SendNeutralLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Trait.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Trait.Remove.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name, TraitName = targetTrait.Label,
                    }
                ),
                pawn
            );

            return Result.Ok();
        }

        [Command("set")]
        public async Task<Result> SetTraitsAsync(params TraitProduct[] products)
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

            // TODO: Ideally we would reserve the full amount up front.
            Result<Transaction> transaction = context.Invoker.ReserveCoins(products.Sum(p => p.PriceToAdd));

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(transaction.Value.Amount, context.Invoker.Coins));

            for (var i = 0; i < pawn.story.traits.allTraits.Count; i++)
            {
                Trait trait = pawn.story.traits.allTraits[i];
                TraitProduct? product = Registries.Traits.AllRegistrants.FirstOrDefault(t => t.Def == trait.def && t.Degree == trait.Degree);

                if (product == null)
                {
                    // Since there's no product for the given trait, we can't properly charge the viewer for this,
                    // so we'll abort and let the framework refund the viewer.

                    return Result.Fail(TranslationService.Instance.FormatInvalidQuery(trait.Label));
                }

                Result removalCheckResult = await CanRemoveTraitAsync(pawn, product, trait);

                if (!removalCheckResult.IsSuccess)
                {
                    transaction.Value.ChangeAmount(-product.PriceToRemove);

                    continue;
                }

                await MainThreadExtensions.OnMainAsync(TakePawnTrait, pawn, trait);
            }

            for (var i = 0; i < products.Length; i++)
            {
                TraitProduct product = products[i];
                Trait? trait = pawn.story.traits.allTraits.Find(t => t.def == product.Def && t.Degree == product.Degree);

                Result additionVerificationResult = await CanAddTraitAsync(pawn, product);

                if (trait == null && additionVerificationResult.IsSuccess)
                    await MainThreadExtensions.OnMainAsync(GivePawnTrait, pawn, trait!);
                else
                    transaction.Value.ChangeAmount(-product.PriceToAdd);
            }

            transaction.Value.PostTransaction();

            await context.SendReplyAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Responses.Trait.Set").Format(
                    new
                    {
                        ProductCount = products.Length,
                    }
                )
            );

            return Result.Ok(); // TODO
        }

        [Command("clear")]
        public async Task<Result> ClearTraitsAsync()
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());
            if (pawn.story.traits.allTraits.Count <= 0) return Result.Fail(TranslationService.Instance.FormatNoTraitsForRemoval());

            List<Trait> traits = pawn.story.traits.allTraits;
            int totalTraits = traits.Count;
            var candidates = new Trait?[totalTraits];
            var insertionPoint = 0;

            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.RemoveTrait.cost * totalTraits);

            if (!transaction.IsSuccess)
                return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(StoreIncidentDefOfs.RemoveTrait.cost * totalTraits, context.Invoker.Coins));

            for (var i = 0; i < traits.Count; i++)
            {
                Trait? trait = traits[i];
                TraitProduct? product = Registries.Traits.AllRegistrants.FirstOrDefault(t => t.Def == trait.def && t.Degree == trait.Degree);

                if (product == null) continue;

                Result result = await CanRemoveTraitAsync(pawn, product, trait);

                if (!result.IsSuccess) continue;

                candidates[insertionPoint++] = trait;
            }

            int candidateCount = insertionPoint + 1;
            if (candidateCount < totalTraits) transaction.Value.ChangeAmount(StoreIncidentDefOfs.RemoveTrait.cost * candidateCount);

            foreach (Trait? trait in candidates)
            {
                if (trait == null) continue;

                await TakePawnTrait(pawn, trait);
            }

            transaction.Value.PostTransaction();

            await letterService.SendNeutralLetterAsync(
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Trait.Title"),
                TranslationService.Instance.GetTranslation("TKUtils.Letters.Trait.Clear.Description").Format(
                    new
                    {
                        ViewerName = context.Invoker.Name,
                    }
                ),
                pawn
            );

            return Result.Ok(TranslationService.Instance.GetTranslation("TKUtils.Responses.Trait.Clear"));
        }

        [Command("replace")]
        public async Task<Result> ReplaceTraitAsync(TraitProduct original, TraitProduct replacement)
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

            Trait? currentTrait = pawn.story.traits.allTraits.Find(t => t.def == original.Def && t.Degree == original.Degree);

            if (currentTrait != null) {}
        }

        private static async Task<bool> TakePawnTrait(Pawn pawn, Trait trait)
        {
            pawn.story.traits.allTraits.Remove(trait);

            if (trait.CurrentData.skillGains is { Count: >= 0, } skillGains) await MainThreadExtensions.OnMainAsync(ApplySkillChanges, pawn, skillGains, arg3: true);
            if (trait.GetDisabledWorkTypes() is { Count: >= 0, } workTypes) await MainThreadExtensions.OnMainAsync(ApplyWorkTypeDisableChanges, pawn, workTypes);

            return true;
        }

        private static async Task<bool> GivePawnTrait(Pawn pawn, Trait trait)
        {
            if (pawn.story.traits.HasTrait(trait.def)) return false;

            pawn.story.traits.allTraits.Add(trait);
            pawn.Notify_DisabledWorkTypesChanged();
            pawn.skills.Notify_SkillDisablesChanged();

            if (!pawn.Dead && pawn.RaceProps.Humanlike) pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();

            MeditationFocusTypeAvailabilityCache.ClearFor(pawn);

            if (trait.CurrentData.skillGains is { Count: >= 0, } gains) await MainThreadExtensions.OnMainAsync(ApplySkillChanges, pawn, gains, arg3: true);
            if (trait.GetDisabledWorkTypes() is { Count: >= 0, } disabledWorkTypes) await MainThreadExtensions.OnMainAsync(ApplyWorkTypeDisableChanges, pawn, disabledWorkTypes);

            return true;
        }

        private static ValueTask<bool> ApplyWorkTypeEnableChanges(Pawn pawn, IReadOnlyList<WorkTypeDef> workTypes)
        {
            pawn.Notify_DisabledWorkTypesChanged();

            for (var i = 0; i < workTypes.Count; i++)
            {
                WorkTypeDef? workType = workTypes[i];

                pawn.workSettings.SetPriority(workType, priority: 3);
            }

            return new ValueTask<bool>(true);
        }

        private static ValueTask<bool> ApplyWorkTypeDisableChanges(Pawn pawn, IReadOnlyList<WorkTypeDef> workTypes)
        {
            pawn.Notify_DisabledWorkTypesChanged();

            for (var i = 0; i < workTypes.Count; i++)
            {
                WorkTypeDef? workType = workTypes[i];

                if (pawn.WorkTypeIsDisabled(workType)) continue;

                pawn.workSettings.Disable(workType);
            }

            return new ValueTask<bool>(true);
        }

        private static ValueTask<bool> ApplySkillChanges(Pawn pawn, IReadOnlyList<SkillGain> gains, bool decrement = false)
        {
            for (var i = 0; i < gains.Count; i++)
            {
                SkillGain gain = gains[i];
                SkillRecord? skillRecord = pawn.skills.GetSkill(gain.skill);

                if (skillRecord.TotallyDisabled) continue;

                int modifier = decrement ? -1 : 1;
                skillRecord.Level += gain.amount * modifier;
            }

            return new ValueTask<bool>(true);
        }

        private async Task<Result> CanRemoveTraitAsync(Pawn pawn, TraitProduct product, Trait trait)
        {
            if (!product.CanRemove) return Result.Fail(TranslationService.Instance.FormatProductDisabled(product));
            if (pawn.story.traits.allTraits.Count <= 0) return Result.Fail(TranslationService.Instance.FormatNoTraitsForRemoval());
            if (context.Invoker.Coins <= product.PriceToRemove)
                return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(product.PriceToRemove, context.Invoker.Coins));
            if (pawn.TryGetTraitGeneConflictForRemoval(trait.def, out Gene? conflictingGene, trait.Degree))
                return Result.Fail(TranslationService.Instance.FormatTraitGeneLocked(product, conflictingGene.def));

            foreach (ITraitProvider? provider in Registries.Compatibilities.AllRegistrants.OfType<ITraitProvider>())
            {
                Result succeeded = await provider.CanPurchaseTraitRemovalAsync(pawn, trait.def, trait.Degree);

                if (!succeeded.IsSuccess) return succeeded;
            }

            return Result.Ok();
        }

        private async Task<Result> CanAddTraitAsync(Pawn pawn, TraitProduct product)
        {
            if (!product.CanAdd) return Result.Fail(TranslationService.Instance.FormatProductDisabled(product));

            TraitDef? traitDef = DefDatabase<TraitDef>.GetNamedSilentFail(product.Id);

            if (traitDef == null) return Result.Fail(TranslationService.Instance.FormatInvalidQuery(product.Id));
            if (context.Invoker.Coins <= product.PriceToAdd) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(product.PriceToAdd, context.Invoker.Coins));
            if (!product.BypassesTraitLimit && pawn.CalculateTraits() >= AddTraitSettings.maxTraits)
                return Result.Fail(new Translation("Adding would bypass limit".MarkNotTranslated()));
            if (pawn.TryGetBackstoryViolation(traitDef, out BackstoryDef? backstory, product.Degree))
                return Result.Fail(TranslationService.Instance.FormatBackstoryRestrictedTrait(product, backstory));
            if (pawn.TryGetConflictingTrait(traitDef, out Trait? conflictingTrait))
                return Result.Fail(TranslationService.Instance.FormatTraitConflict(traitDef, conflictingTrait.def));
            if (pawn.TryGetDuplicateTrait(traitDef, out Trait? duplicateTrait))
                return Result.Fail(TranslationService.Instance.FormatTraitSpectrumConflict(traitDef, duplicateTrait.def));
            if (pawn.TryGetTraitGeneConflictForAddition(traitDef, out Gene? conflictingGene, product.Degree))
                return Result.Fail(TranslationService.Instance.FormatTraitGeneSuppressed(product, conflictingGene.def));

            foreach (ITraitProvider provider in Registries.Compatibilities.AllRegistrants.OfType<ITraitProvider>())
            {
                Result succeeded = await provider.CanPurchaseTraitAsync(pawn, traitDef, product.Degree);

                if (!succeeded.IsSuccess) return succeeded;
            }

            return Result.Ok();
        }
    }
}
