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
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using FormatWith;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Extensions;
using Verse;

namespace ToolkitUtils.Mod.Localization;

/// <summary>Provides extension methods for formatting specific error messages using an <see cref="TranslationService" />.</summary>
[PublicAPI]
public static class TranslationServiceErrorMessageExtensions
{
    /// <summary>Formats the translation for an insufficient balance error message with the given balance and required amount.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="balance">The current balance available to the user.</param>
    /// <param name="requiredAmount">The amount required to complete the operation.</param>
    /// <returns>
    ///     A formatted string containing the insufficient balance error message with the specified balance and required
    ///     amount.
    /// </returns>
    public static Translation FormatInsufficientBalance(this TranslationService service, int balance, long requiredAmount) =>
        service.GetTranslation("TKUtils.Errors.InsufficientBalance").Format(
            new
            {
                Balance = balance.ToString("N0"), RequiredAmount = requiredAmount.ToString("N0"),
            },
            MissingKeyBehaviour.Ignore
        );

    /// <summary>Formats the translation for a pawn required a self-related error message.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A formatted string containing the pawn required a self-related error message.</returns>
    public static Translation FormatPawnRequiredSelf(this TranslationService service) => service.GetTranslation("TKUtils.Errors.PawnRequired.Self");

    /// <summary>
    ///     Retrieves a formatted error message indicating that a specific pawn is required, optionally replacing
    ///     placeholders in the translation string with the relevant viewer's name.
    /// </summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="viewer">The viewer object whose name will be used to replace placeholders in the formatted string.</param>
    /// <returns>The formatted string indicating the pawn requirement.</returns>
    public static Translation FormatPawnRequiredOther(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Errors.PawnRequired.Other").Format(
            new
            {
                ViewerName = viewer.Name,
            },
            MissingKeyBehaviour.Ignore
        );

    /// <summary>Formats the translation for an invalid query error message with the given query string.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="query">The invalid query string to include in the error message.</param>
    /// <returns>A formatted string containing the invalid query error message with the specified query.</returns>
    public static Translation FormatInvalidQuery(this TranslationService service, string query) =>
        service.GetTranslation("TKUtils.Errors.InvalidQuery").Format(
            new
            {
                Query = query,
            },
            MissingKeyBehaviour.Ignore
        );

    /// <summary>Formats the translation for a disabled product error message using the specified product.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="product">The product for which the disabled error message should be formatted.</param>
    /// <returns>A formatted string containing the disabled product error message with the product's name.</returns>
    public static Translation FormatProductDisabled(this TranslationService service, IIdentifiable product) =>
        service.GetTranslation("TKUtils.Errors.ProductDisabled").Format(
            new
            {
                ProductName = product.Name,
            }
        );

    /// <summary>
    ///     Formats the translation for an error message indicating unmet research prerequisites for the specified project
    ///     and product.
    /// </summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="project">The research project that has unmet prerequisites.</param>
    /// <param name="product">The product associated with the research prerequisites.</param>
    /// <returns>A formatted string containing the error message with the specified research project and product information.</returns>
    public static Translation FormatUnfulfilledResearchPrerequisites(this TranslationService service, ResearchProjectDef project, IIdentifiable product) =>
        service.GetTranslation("TKUtils.Errors.UnfulfilledResearchPrerequisite").Format(
            new
            {
                ResearchName = project.LabelCap, ProductName = product.Name,
            }
        );

    /// <summary>Formats the translation for a cart total error message when the minimum cart total is not reached.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="minimumTotal">The minimum cart total required to proceed with the operation.</param>
    /// <returns>A formatted string containing the minimum cart total error message with the specified minimum total.</returns>
    public static Translation FormatMinimumCartTotalNotReached(this TranslationService service, int minimumTotal) =>
        service.GetTranslation("TKUtils.Errors.MinimumCartTotalNotReached").Format(
            new
            {
                MinimumCartTotal = minimumTotal.ToString("N0"),
            }
        );

    /// <summary>
    ///     Formats the translation for an invalid item material error message using the provided product and material
    ///     information.
    /// </summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="product">The product associated with the invalid material error.</param>
    /// <param name="productMaterial">The material related to the product that is invalid.</param>
    /// <returns>
    ///     A formatted string containing the invalid item material error message with the specified product and material
    ///     names.
    /// </returns>
    public static Translation FormatItemMaterialInvalid(this TranslationService service, ItemProduct product, ItemProduct productMaterial) =>
        service.GetTranslation("TKUtils.Errors.ItemMaterialInvalid").Format(
            new
            {
                ItemName = product.Name, MaterialName = productMaterial.Name,
            }
        );

    /// <summary>Formats the translation for an invalid item quality error message with the specified product.</summary>
    /// <param name="service">The translation service used to fetch and format the translation.</param>
    /// <param name="product">The product for which the invalid item quality error message needs to be formatted.</param>
    /// <returns>A formatted string containing the invalid item quality error message for the provided product.</returns>
    public static Translation FormatItemQualityInvalid(this TranslationService service, ItemProduct product) =>
        service.GetTranslation("TKUtils.Errors.ItemQualityInvalid").Format(
            new
            {
                ItemName = product.Name,
            }
        );

    /// <summary>Formats the translation for an invalid animal gender error message using the provided product information.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="product">The product associated with the invalid animal gender.</param>
    /// <returns>A formatted string containing the invalid animal gender error message with the product's name.</returns>
    public static Translation FormatAnimalGenderInvalid(this TranslationService service, ItemProduct product) =>
        service.GetTranslation("TKUtils.Errors.AnimalGenderInvalid").Format(
            new
            {
                AnimalName = product.Name,
            }
        );

    /// <summary>Formats the translation for a genderless swap error message using the specified product details.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="product">The product representing the pawn for which the genderless swap occurred.</param>
    /// <returns>A formatted string containing the genderless swap error message with the specified product name.</returns>
    public static Translation FormatGenderlessSwap(this TranslationService service, PawnProduct product) =>
        service.GetTranslation("TKUtils.Errors.GenderlessSwap").Format(
            new
            {
                PawnKindName = product.Name,
            }
        );

    /// <summary>Formats the translation for an unusable item error message with the specified product.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="product">The item product that is unusable and requires error formatting.</param>
    /// <returns>A formatted string containing the unusable item error message with the product's name.</returns>
    public static Translation FormatUnusable(this TranslationService service, ItemProduct product) =>
        service.GetTranslation("TKUtils.Errors.Unusable").Format(
            new
            {
                ItemName = product.Name,
            }
        );

    /// <summary>Formats the translation for a no passion error message.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A formatted string containing the no passion error message.</returns>
    public static Translation FormatNoPassions(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoPassions");

    /// <summary>Formats the translation for the message indicating that no trait is available for removal.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A string containing the formatted message for no trait available for removal.</returns>
    public static Translation FormatNoTraitForRemoval(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoTraitForRemoval");

    /// <summary>Formats the translation for the error message when no matching trait is found for removal.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="product">The trait product that was attempted to be removed.</param>
    /// <returns>
    ///     A formatted string containing the message indicating that no trait was found for removal with the specified
    ///     product.
    /// </returns>
    public static Translation FormatNoTraitFoundForRemoval(this TranslationService service, TraitProduct product) =>
        service.GetTranslation("TKUtils.Errors.NoTraitFoundForRemoval").Format(
            new
            {
                TraitName = product.Name,
            }
        );

    /// <summary>Formats the translation for a message indicating that replacing a trait would bypass the allowed trait limit.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="original">The original trait being replaced.</param>
    /// <param name="replacement">The replacement trait that would bypass the trait limit if applied.</param>
    /// <param name="limit">The applicable trait limit being exceeded.</param>
    /// <returns>
    ///     A formatted string containing the message about bypassing the trait limit with the specified original trait,
    ///     replacement trait, and the defined trait limit.
    /// </returns>
    public static Translation FormatTraitReplaceWouldBypassLimit(this TranslationService service, TraitProduct original, TraitProduct replacement, int limit) =>
        service.GetTranslation("TKUtils.Errors.TraitReplaceWouldBypassLimit").Format(
            new
            {
                TraitName = original.Name, ReplacementTraitName = replacement.Name, TraitLimit = limit.ToString("N0"),
            }
        );

    /// <summary>Formats the translation for an overflowed error message.</summary>
    /// <param name="service">The translation service to use for fetching the overflowed error message.</param>
    /// <returns>The formatted string containing the overflowed error message.</returns>
    public static Translation FormatOverflowed(this TranslationService service) => service.GetTranslation("TKUtils.Errors.Overflowed");

    /// <summary>Formats the translation for an error message indicating that the viewer already has an associated pawn.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A formatted string containing the viewer already has a pawn error message.</returns>
    public static Translation FormatViewerAlreadyHasPawn(this TranslationService service) => service.GetTranslation("TKUtils.Errors.ViewerAlreadyHasPawn");

    /// <summary>Formats the translation for a pawn being in a combat error message.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>The localized string indicating a pawn is in combat based on the translation service.</returns>
    public static Translation FormatPawnInCombat(this TranslationService service) => service.GetTranslation("TKUtils.Errors.PawnInCombat");

    /// <summary>Formats the translation for a wager required error message using the given wager range.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="range">The range representing the minimum and maximum wager amounts.</param>
    /// <returns>A formatted string containing the wager-required error message with the specified range.</returns>
    public static Translation FormatWagerRequired(this TranslationService service, IntRange range) =>
        service.GetTranslation("TKUtils.Errors.WagerRequired").Format(
            new
            {
                MinimumWagerAmount = range.min.ToString("N0"), MaximumWagerAmount = range.max.ToString("N0"),
            }
        );

    /// <summary>Formats the translation for a message indicating that the game is paused.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A string containing the translated message for a paused game.</returns>
    public static Translation FormatGamePaused(this TranslationService service) => service.GetTranslation("TKUtils.Errors.GamePaused");

    /// <summary>Formats the translation for a 'not injured' error message.</summary>
    /// <param name="service">The translation service to use for retrieving the 'not injured' error message translation.</param>
    /// <returns>A formatted string containing the 'not injured' error message.</returns>
    public static Translation FormatNotInjured(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NotInjured");

    /// <summary>
    ///     Formats the translation for a marriage forbidden error message due to beliefs, referencing the specified
    ///     viewer.
    /// </summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="viewer">The viewer for whom the error message is being formatted.</param>
    /// <returns>
    ///     A formatted string containing the error message indicating that the marriage is forbidden by beliefs,
    ///     including the viewer's name.
    /// </returns>
    public static Translation FormatMarriageForbiddenByBeliefs(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Errors.MarriageForbiddenByBeliefs").Format(
            new
            {
                ViewerName = viewer.Name,
            }
        );

    /// <summary>Formats the error message indicating that no active research is currently being conducted.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A formatted string containing the error message for the absence of active research.</returns>
    public static Translation FormatNotResearching(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoResearching");

    /// <summary>Formats the translation for an error message indicating no pawn requires any needs.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A formatted string containing the error message stating that no pawn has any needs.</returns>
    public static Translation FormatNoPawnNeeds(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoPawnNeeds");

    /// <summary>Formats the translation for a message indicating that a pawn is incapable of a specified capacity.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="capacity">The pawn capacity definition associated with the incapability.</param>
    /// <returns>A formatted string containing the message indicating the pawn's incapability with the specified capacity.</returns>
    public static Translation FormatIncapableOfCapacity(this TranslationService service, PawnCapacityDef capacity) =>
        service.GetTranslation("TKUtils.Errors.IncapableOfCapacity").Format(
            new
            {
                CapacityName = capacity.label,
            }
        );

    /// <summary>Formats the translation for a message indicating that there are no relationships available.</summary>
    /// <param name="service">The translation service to use for retrieving the message translation.</param>
    /// <returns>A string containing the translation for the "no relationships" message.</returns>
    public static Translation FormatNoRelationships(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoRelationships");

    /// <summary>
    ///     Formats the translation for an error message indicating that a user cannot leave while being part of a
    ///     caravan.
    /// </summary>
    /// <param name="service">The translation service to use for retrieving the localized error message.</param>
    /// <returns>A string containing the localized error message for the inability to leave while in a caravan.</returns>
    public static Translation FormatCannotLeaveWhileInCaravan(this TranslationService service) => service.GetTranslation("TKUtils.Errors.CannotLeaveWhileInCaravan");

    /// <summary>Formats the translation for a message indicating that no matching trait was found for removal.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="trait">The trait definition for which no matching removal was found.</param>
    /// <returns>A formatted string containing the message indicating that the specified trait was not found for removal.</returns>
    public static Translation FormatNoTraitFoundForRemoval(this TranslationService service, TraitDef trait) =>
        service.GetTranslation("TKUtils.Errors.NoTraitFoundForRemoval").Format(
            new
            {
                TraitName = trait.label,
            }
        );

    /// <summary>Formats the message indicating that there are no traits available for removal.</summary>
    /// <param name="service">The translation service to use for fetching the translation message.</param>
    /// <returns>A formatted string containing the message for no traits available for removal.</returns>
    public static Translation FormatNoTraitsForRemoval(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoTraitsForRemoval");

    /// <summary>Formats the translation for an invalid surgery candidate error message for the specified product.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="product">The product associated with the invalid surgery candidate error.</param>
    /// <returns>
    ///     A formatted string containing the invalid surgery candidate error message for the specified product, or null
    ///     if the translation was not found.
    /// </returns>
    public static Translation FormatInvalidSurgeryCandidate(this TranslationService service, ItemProduct product) =>
        service.GetTranslation("TKUtils.Errors.InvalidSurgeryCandidate").Format(
            new
            {
                ItemName = product.Name,
            }
        );

    /// <summary>Formats the error message for no available surgery slot for a specified product and body part.</summary>
    /// <param name="service">The translation service used to fetch the error message translation.</param>
    /// <param name="product">The item product for which the surgery slot error occurs.</param>
    /// <param name="bodyPart">The body part associated with the unavailable surgery slot.</param>
    /// <returns>A formatted string containing the no surgery slot error message.</returns>
    public static Translation FormatNoSurgerySlot(this TranslationService service, ItemProduct product, BodyPartDef bodyPart) =>
        service.GetTranslation("TKUtils.Errors.NoSurgerySlot").Format(
            new
            {
                ItemName = product.Name, BodyPartName = bodyPart.label,
            }
        );

    /// <summary>Formats the translation for a message indicating that an inspiration attempt yielded no results.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A string containing the formatted translation for the failed inspiration attempt message.</returns>
    public static Translation FormatInspirationYieldedNone(this TranslationService service) => service.GetTranslation("TKUtils.Errors.InspirationYieldedNone");

    /// <summary>Formats the translation for a conflict between two traits on the same spectrum.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="trait">The primary trait that is causing the conflict.</param>
    /// <param name="otherTrait">The other trait on the same spectrum that conflicts with the primary trait.</param>
    /// <returns>A formatted string containing the conflict error message for the specified traits.</returns>
    public static Translation FormatTraitSpectrumConflict(this TranslationService service, TraitDef trait, TraitDef otherTrait) =>
        service.GetTranslation("TKUtils.Errors.TraitSpectrumConflict").Format(
            new
            {
                TraitName = trait.label, OtherTraitName = otherTrait.label,
            }
        );

    /// <summary>Formats the translation for a trait conflict error message with the given trait and conflicting trait.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="trait">The trait that is causing the conflict.</param>
    /// <param name="conflictingTrait">The conflicting trait that is incompatible with the specified trait.</param>
    /// <returns>A formatted string containing the trait conflict error message with the specified traits.</returns>
    public static Translation FormatTraitConflict(this TranslationService service, TraitDef trait, TraitDef conflictingTrait) =>
        service.GetTranslation("TKUtils.Errors.TraitConflict").Format(
            new
            {
                TraitName = trait.label, ConflictingTraitName = conflictingTrait.label,
            }
        );

    /// <summary>
    ///     Formats the translation for a backstory-restricted trait error message, including the specified trait and
    ///     backstory.
    /// </summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="trait">The trait that is restricted by the backstory.</param>
    /// <param name="backstory">The backstory causing the restriction of the trait.</param>
    /// <returns>
    ///     A formatted string containing the error message that specifies the trait and the backstory causing the
    ///     restriction.
    /// </returns>
    public static Translation FormatBackstoryRestrictedTrait(this TranslationService service, TraitProduct trait, BackstoryDef backstory) =>
        service.GetTranslation("TKUtils.Errors.BackstoryRestrictedTrait").Format(
            new
            {
                TraitName = trait.Name, BackstoryName = backstory.label,
            }
        );

    /// <summary>Formats the translation for a trait that is locked due to being associated with a specific gene.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="trait">The trait that is locked.</param>
    /// <param name="gene">The gene associated with the locked trait.</param>
    /// <returns>A formatted string containing the trait and its associated gene lock message.</returns>
    public static Translation FormatTraitGeneLocked(this TranslationService service, TraitProduct trait, GeneDef gene) =>
        service.GetTranslation("TKUtils.Errors.TraitGeneLocked").Format(
            new
            {
                TraitName = trait.Name, GeneName = gene.label,
            }
        );

    /// <summary>Formats the translation for a suppressed trait due to a gene restriction.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="trait">The trait product that is suppressed due to the gene restriction.</param>
    /// <param name="gene">The gene definition causing the suppression of the trait.</param>
    /// <returns>A formatted string containing the error message for a suppressed trait caused by the specified gene.</returns>
    public static Translation FormatTraitGeneSuppressed(this TranslationService service, TraitProduct trait, GeneDef gene) =>
        service.GetTranslation("TKUtils.Errors.TraitGeneSuppressed").Format(
            new
            {
                TraitName = trait.Name, GeneName = gene.label,
            }
        );

    /// <summary>Formats the translation for an invalid color error message using the provided color value.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="color">The invalid color value that caused the error.</param>
    /// <returns>A formatted string containing the error message for the specified invalid color.</returns>
    public static Translation FormatInvalidColor(this TranslationService service, string color) =>
        service.GetTranslation("TKUtils.Errors.InvalidColor").Format(
            new
            {
                Color = color,
            }
        );

    /// <summary>Formats the translation for an error message indicating that no game is currently loaded.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A formatted string containing the error message indicating the absence of a loaded game.</returns>
    public static Translation FormatNoLoadedGame(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoLoadedGame");

    /// <summary>Formats the translation for an error message indicating that no map was found.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A formatted string containing the error message for no map found.</returns>
    public static Translation FormatNoMapFound(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoMapFound");

    /// <summary>Formats the translation for an error message indicating no player map was found.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A string containing the formatted error message indicating no player map was found.</returns>
    public static Translation FormatNoPlayerMapFound(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoPlayerMapFound");

    /// <summary>Formats the translation for an error message indicating that a pawn is incapable of work.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <returns>A formatted string containing the error message for being incapable of work.</returns>
    public static Translation FormatIncapableOfWork(this TranslationService service) => service.GetTranslation("TKUtils.Errors.IncapableOfWork");

    public static Translation FormatIncapableOfCapacity(this TranslationService service, PawnCapacityDef capacityDef, Pawn pawn) =>
        service.GetTranslation("IncapableOfCapacity").Format(capacityDef.GetLabelFor(pawn));

    public static Translation FormatKindIncapableOfCapacity(this TranslationService service, PawnKindDef kindDef) =>
        service.GetTranslation("TKUtils.Responses.PawnHealth.KindHasNoCapacities").Format(
            new
            {
                KindName = kindDef.LabelCap,
            }
        );

    public static Translation FormatNoFactions(this TranslationService service) => service.GetTranslation("TKUtils.Responses.Factions.None");

    public static Translation FormatNoHealthConditions(this TranslationService service) => service.GetTranslation("NoHealthConditions").CapitalizeFirst();

    public static Translation FormatNoSpouses(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoSpouses");

    public static Translation FormatNotMarriedToViewer(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Errors.NotMarried.Other").Format(
            new
            {
                ViewerName = viewer.Name,
            },
            MissingKeyBehaviour.Ignore
        );

    /// <summary>Retrieves and formats a localization error message indicating that the specified pawn is not dead.</summary>
    /// <param name="service">The translation service used to fetch the localized error message.</param>
    /// <param name="pawn">The pawn for whom the error message is being generated.</param>
    /// <returns>A formatted translation containing the error message that the specified pawn is not dead.</returns>
    public static Translation GetNotDeadError(this TranslationService service, Pawn pawn) =>
        service.GetTranslation("TKUtils.Errors.NotDead").Format(
            new
            {
                PawnName = pawn.Name,
            }
        );

    /// <summary>Fetches the localized error message indicating that the specified pawn has been discarded.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="pawn">The pawn that has been discarded.</param>
    /// <returns>A translation containing the localized discarded pawn error message.</returns>
    public static Translation GetDiscardedError(this TranslationService service, Pawn pawn) =>
        service.GetTranslation("TKUtils.Errors.Discarded").Format(
            new
            {
                PawnName = pawn.Name,
            }
        );

    /// <summary>
    ///     Fetches and formats the error message for attempting to resurrect an unnatural corpse, using the specified
    ///     corpse's details.
    /// </summary>
    /// <param name="service">The translation service to use for retrieving and formatting the error message.</param>
    /// <param name="pawn">
    ///     The pawn object representing the unnatural corpse whose information will be included in the error
    ///     message.
    /// </param>
    /// <returns>A formatted translation containing the error message for resurrecting an unnatural corpse.</returns>
    public static Translation GetUnnaturalCorpseResurrectionError(this TranslationService service, Pawn pawn) =>
        service.GetTranslation("TKUtils.Errors.UnnaturalCorpse").Format(
            new
            {
                PawnName = pawn.Name,
            }
        );

    /// <summary>Generates an insufficient balance error message with the given required and current balance amounts.</summary>
    /// <param name="service">The translation service to use for fetching the translation.</param>
    /// <param name="requiredAmount">The amount required to complete the operation.</param>
    /// <param name="currentAmount">The current balance available to the user.</param>
    /// <returns>
    ///     A formatted translation containing the insufficient balance error message with the specified required and
    ///     current balance amounts.
    /// </returns>
    public static Translation GetInsufficientBalanceError(this TranslationService service, long requiredAmount, long currentAmount) =>
        service.GetTranslation("TKUtils.Errors.InsufficientBalance").Format(
            new
            {
                RequiredAmount = requiredAmount, Balance = currentAmount,
            }
        );

    public static Translation GetNoPlayerMapError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoPlayerMap");
    public static Translation GetNoGameError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoGame");

    public static Translation GetNotInjuredError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NotInjured.Self");
    public static Translation GetNoOneInjuredError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NotInjured.Colony");

    public static Translation GetNotResearchingError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoResearchProject");

    public static Translation GetNoResultsError(this TranslationService service, string query) =>
        service.GetTranslation("TKUtils.Errors.QueryYieldedNone").Format(
            new
            {
                Query = query,
            }
        );

    public static Translation GetPawnRequiredError(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Errors.PawnRequired.Other").Format(
            new
            {
                ViewerName = viewer.Name,
            }
        );

    public static Translation GetAdulthoodConflictError(this TranslationService service, Pawn pawn) =>
        service.GetTranslation("TKUtils.Errors.AdulthoodConflict").Format(
            new
            {
                PawnName = pawn.Name,
            }
        );

    public static Translation GetChildhoodConflictError(this TranslationService service, Pawn pawn) =>
        service.GetTranslation("TKUtils.Errors.ChildhoodConflict").Format(
            new
            {
                PawnName = pawn.Name,
            }
        );
}
