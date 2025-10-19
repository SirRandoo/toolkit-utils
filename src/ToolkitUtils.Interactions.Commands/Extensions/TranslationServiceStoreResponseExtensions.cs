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
using JetBrains.Annotations;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Commands.Extensions;

/// <summary>
///     Provides extension methods for formatting localized messages in the context of translation service responses,
///     specifically for operations involving coin distribution to viewers.
/// </summary>
[PublicAPI]
public static class TranslationServiceStoreResponseExtensions
{
    /// <summary>
    ///     Formats a message indicating that coins have been given to all viewers, including the total amount distributed
    ///     and the number of viewers who received the coins.
    /// </summary>
    /// <param name="service">The translation service used to retrieve and format the message.</param>
    /// <param name="amount">The number of coins given to each viewer.</param>
    /// <param name="viewerCount">The total number of viewers who received the coins.</param>
    /// <returns>A formatted string containing the message for distributing coins to all viewers.</returns>
    public static string FormatGiveAllCoins(this TranslationService service, int amount, int viewerCount) =>
        service.GetTranslation("TKUtils.Response.GiveAllCoins").Format(
            new
            {
                Amount = amount.ToString("N0"), NumberOfViewers = viewerCount.ToString("N0"),
            }
        );

    /// <summary>
    ///     Formats a message indicating that coins have been given to a viewer, including the amount, the viewer's name,
    ///     and their updated coin balance.
    /// </summary>
    /// <param name="service">The translation service used to retrieve and format the message.</param>
    /// <param name="viewer">The viewer who is receiving the coins.</param>
    /// <param name="amount">The number of coins being given to the viewer.</param>
    /// <returns>A formatted string containing the give coins message.</returns>
    public static string FormatGiveCoins(this TranslationService service, Viewer viewer, int amount) =>
        service.GetTranslation("TKUtils.Response.GiveCoins").Format(
            new
            {
                Amount = amount.ToString("N0"), ViewerName = viewer.Name, NewBalance = viewer.Coins.ToString("N0"),
            }
        );

    public static string FormatItemPriceCheck(this TranslationService service, ItemProduct product, int quantity, int price) =>
        service.GetTranslation("TKUtils.Response.ItemPriceCheck").Format(
            new
            {
                ItemName = product.Name, Quantity = quantity.ToString("N0"), Price = price.ToString("N0"),
            }
        );

    public static string FormatStaticPriceCheck(this TranslationService service, IIdentifiable product, int price) =>
        service.GetTranslation("TKUtils.Response.StaticPriceCheck").Format(
            new
            {
                ProductName = product.Name, Price = price.ToString("N0"),
            }
        );

    public static string FormatMetaPriceCheck(this TranslationService service) => service.GetTranslation("TKUtils.Response.MetaPriceCheck");

    public static string FormatFullTraitPriceCheck(this TranslationService service, TraitProduct product) =>
        service.GetTranslation("TKUtils.Response.TraitPriceCheck.Full").Format(
            new
            {
                TraitName = product.Name, AcquisitionPrice = product.PriceToAdd.ToString("N0"), RelinquishmentCost = product.PriceToRemove.ToString("N0"),
            }
        );

    public static string FormatAcquirePriceCheck(this TranslationService service, TraitProduct product) =>
        service.GetTranslation("TKUtils.Response.TraitPriceCheck.OnlyAcquire").Format(
            new
            {
                TraitName = product.Name, AcquisitionPrice = product.PriceToAdd.ToString("N0"),
            }
        );

    public static string FormatRelinquishPriceCheck(this TranslationService service, TraitProduct product) =>
        service.GetTranslation("TKUtils.Response.TraitPriceCheck.OnlyRelinquish").Format(
            new
            {
                TraitName = product.Name, RelinquishmentCost = product.PriceToRemove.ToString("N0"),
            }
        );

    public static string FormatColonistCount(this TranslationService service, int total)
    {
        if (total <= 0) return service.GetTranslation("TKUtils.Responses.ColonistCount.None");

        return total <= 1
            ? service.GetTranslation("TKUtils.Response.ColonistCount.Single")
            : service.GetTranslation("TKUtils.Response.ColonistCount.Multiple").Format(
                new
                {
                    Count = total.ToString("N0"),
                }
            );
    }

    public static string FormatMapWealth(this TranslationService service, float mapWealth) => $"{service.GetTranslation("ThisMapColonyWealthTotal")} {mapWealth:F2}";

    public static string FormatMapWealthWithGlobal(this TranslationService service, float mapWealth, float globalWealth) =>
        service.GetTranslation("TKUtils.Responses.Wealth.MultiMap").Format(
            new
            {
                CurrentMapWealth = mapWealth.ToString("F2"), GlobalWealth = globalWealth.ToString("F2"),
            }
        );

    public static string FormatPawnFixSingular(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Responses.PawnFix.Single").Format(
            new
            {
                ViewerName = viewer.Name,
            }
        );

    public static string FormatPawnFixPlural(this TranslationService service, int relinkedCount) =>
        service.GetTranslation("TKUtils.Responses.PawnFix.Multiple").Format(
            new
            {
                Count = relinkedCount.ToString("N0"),
            }
        );

    public static string FormatPawnThanosSnapped(this TranslationService service) => service.GetTranslation("TKUtils.Responses.PawnLeave.Thanos");
    public static string FormatPawnGenericLeave(this TranslationService service) => service.GetTranslation("TKUtils.Responses.PawnLeave.Generic");
    public static string FormatPawnAge(this TranslationService service, Pawn pawn) => string.Format(service.GetTranslation("AgeIndicator"), pawn.ageTracker.AgeNumberString);

    public static string FormatWedded(this TranslationService service, Viewer spouse) =>
        service.GetTranslation("TKUtils.Responses.Marriage.Wedded").Format(
            new
            {
                SpouseName = spouse.Name,
            }
        );

    public static string FormatProposal(this TranslationService service, Viewer proposer, string commandPrefix, string marriageCommand) =>
        service.GetTranslation("TKUtils.Responses.Marriage.Proposal").Format(
            new
            {
                ProposerName = proposer.Name, CommandPrefix = commandPrefix, MarriageCommand = marriageCommand,
            }
        );

    public static string FormatDivorced(this TranslationService service, Viewer exSpouse) =>
        service.GetTranslation("TKUtils.Responses.Divorce.Finalized").Format(
            new
            {
                ExName = exSpouse.Name,
            }
        );
}
