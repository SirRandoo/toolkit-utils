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
using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Interactions.Commands.Extensions;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Logging;
using ToolkitUtils.Mod.Products;
using Result = ToolkitUtils.Mod.Result;

namespace ToolkitUtils.Interactions.Commands;

/// <summary>Represents a command group for checking prices of different product types in the application.</summary>
/// <remarks>
///     Each sub-command within this class is designed to handle price queries for specific product categories, such
///     as animal products, event-related items, or other defined types.
/// </remarks>
[Group("price")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PriceCheck : CommandGroup
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

    /// <summary>Looks up the price of an animal product based on a specified query and amount.</summary>
    /// <param name="query">The name of the animal product to look up.</param>
    /// <param name="amount">The amount of the product to calculate the price for. Must be between 1 and 1,000,000.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a success or error result
    ///     indicating the outcome of the operation.
    /// </returns>
    [Command("animal")]
    public ValueTask<Result> LookupAnimalPriceAsync(string query, int amount = 1)
    {
        if (amount is < 1 or > 5_000_00) return new ValueTask<Result>(Result.Fail(new Translation("Amount must be between 1 and 5,000,000".MarkNotTranslated())));

        ItemProduct? product = Registries.Items.GetByName(query);

        if (product == null) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)));
        if (!product.Purchasable) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatProductDisabled(product)));

        int price = Calculator.MultiplyChecked(product.Price, amount);

        return new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatItemPriceCheck(product, amount, price)));
    }

    /// <summary>Looks up the price of an event product based on a specified product and amount.</summary>
    /// <param name="event">The event product to look up.</param>
    /// <param name="amount">The amount of the product to calculate the price for. Must be between 1 and 1,000,000.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a success or error result
    ///     indicating the outcome of the operation.
    /// </returns>
    [Command("event")]
    public ValueTask<Result> LookupEventPriceAsync(EventProduct @event, int amount = 1)
    {
        if (amount is < 1 or > 5_000_00) return new ValueTask<Result>(Result.Fail(new Translation("Amount must be between 1 and 5,000,000".MarkNotTranslated())));
        if (@event.Metadata is not {} metadata) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatInvalidQuery(@event.Name)));
        if (metadata.EventType.HasFlagFast(EventTypes.Item) || metadata.EventType.HasFlagFast(EventTypes.Pawn) || metadata.EventType.HasFlagFast(EventTypes.Trait))
            return new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatMetaPriceCheck()));

        if (metadata.EventType.HasFlagFast(EventTypes.Variable))
        {
            // TODO: Perform a dry run on the event to obtain the current price.

            return new ValueTask<Result>(Result.Ok());
        }

        return new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatStaticPriceCheck(@event, @event.Price)));
    }

    [Command("item")]
    public ValueTask<Result> LookupItemPriceAsync(string query, int amount = 1)
    {
        if (amount is < 1 or > 5_000_00) return new ValueTask<Result>(Result.Fail(new Translation("Amount must be between 1 and 5,000,000".MarkNotTranslated())));

        ExtendedMetadataParser parser = ExtendedMetadataParser.Parse(query);
        ItemProduct? subject = parser.GetSubject(Registries.Items.AllRegistrants);

        if (subject is not { Price: > 0, }) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)));

        ItemProduct? material = parser.GetMetadata(Registries.Items.AllRegistrants);

        QualityCategory? quality = parser.GetMetadata(
            QualityCategoryExtensions.GetValues(),
            nameGetter: category => category.ToStringFast(),
            defaultGetter: () => QualityCategory.Normal
        );

        if (!subject.Def.HasComp(typeof(CompQuality)) && quality != null) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatItemQualityInvalid(subject)));
        if (material != null && !subject.Def.MadeFromStuff) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatItemMaterialInvalid(subject, material)));

        try
        {
            int price = Calculator.MultiplyChecked(subject.Price, amount);

            // TODO: Multiply the quality with the price.

            return new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatItemPriceCheck(subject, amount, price)));
        }
        catch (InvalidOperationException e)
        {
            Logger.Error(e, message: "An error occurred while calculating the price of an item.");

            return new ValueTask<Result>(Result.Fail());
        }
    }

    [Command("kind")]
    public ValueTask<Result> LookupKindPriceAsync(string query, int amount = 1)
    {
        if (amount is < 1 or > 5_000_00) return new ValueTask<Result>(Result.Fail(new Translation("Amount must be between 1 and 5,000,000".MarkNotTranslated())));

        PawnProduct? pawn = Registries.Pawns.GetByName(query);

        return pawn == null
            ? new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)))
            : new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatStaticPriceCheck(pawn, pawn.Price)));
    }

    [Command("trait")]
    public ValueTask<Result> LookupTraitPriceAsync(string query, int amount = 1)
    {
        if (amount is < 1 or > 5_000_00) return new ValueTask<Result>(Result.Fail(new Translation("Amount must be between 1 and 5,000,000".MarkNotTranslated())));

        TraitProduct? trait = Registries.Traits.GetByName(query);

        if (trait == null) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatInvalidQuery(query)));
        if (trait is { CanAdd: false, CanRemove: false, }) return new ValueTask<Result>(Result.Fail(TranslationService.Instance.FormatProductDisabled(trait)));

        return trait switch
        {
            { CanAdd: true, CanRemove: false, } => new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatAcquirePriceCheck(trait))),
            { CanAdd: false, CanRemove: true, } => new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatRelinquishPriceCheck(trait))),
            { CanAdd: true, CanRemove: true, }  => new ValueTask<Result>(Result.Ok(TranslationService.Instance.FormatFullTraitPriceCheck(trait))),
            var _                               => new ValueTask<Result>(Result.Fail()),
        };
    }
}
