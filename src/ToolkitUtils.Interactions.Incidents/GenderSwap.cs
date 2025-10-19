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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Application.Extensions;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class GenderSwap(ILetterService letterService, ExecutionContext context) : CommandGroup
{
    [Command("genderswap")]
    public async Task<Result> PerformGenderSwapAsync()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        PawnProduct? product = Registries.Pawns.Get(pawn.kindDef.defName);

        if (pawn.gender is Gender.None) return product != null ? Result.Fail(TranslationService.Instance.FormatGenderlessSwap(product)) : Result.Fail();

        Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.GenderSwap.cost);

        if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(StoreIncidentDefOfs.GenderSwap.cost, context.Invoker.Coins));

        Gender previousGender = pawn.gender;
        pawn.gender = pawn.gender is Gender.Female ? Gender.Male : Gender.Female;
        pawn.story.HairColor = await RouterService.Instance.RouteToMainAsync(PawnHairColors.RandomHairColor, pawn, pawn.story.SkinColor, pawn.ageTracker.AgeBiologicalYears);

        if (pawn.style.HasUnwantedBeard)
        {
            pawn.style.beardDef = BeardDefOf.NoBeard;

            await RouterService.Instance.RouteToMainAsync(pawn.style.Notify_StyleItemChanged);
        }

        pawn.story.hairDef = await RouterService.Instance.RouteToMainAsync(PawnStyleItemChooser.RandomHairFor, pawn);
        pawn.story.bodyType = pawn.story.Adulthood == null
            ? await RouterService.Instance.RouteToMainAsync(RandomBodyType, pawn)
            : await RouterService.Instance.RouteToMainAsync(pawn.story.Adulthood.BodyTypeFor, pawn.gender);

        transaction.Value.PostTransaction();

        await letterService.SendGenderSwapLetterAsync(pawn, previousGender);

        return Result.Ok(new Translation("Your gender has been swapped.".MarkNotTranslated()));
    }

    private static BodyTypeDef RandomBodyType(Pawn pawn) => Rand.Value >= 0.5 ? BodyTypeForGender(pawn.gender) : BodyTypeDefOf.Thin;

    private static BodyTypeDef BodyTypeForGender(Gender gender)
    {
        return gender switch
        {
            Gender.Female => BodyTypeDefOf.Female,
            Gender.Male   => BodyTypeDefOf.Male,
            var _         => BodyTypeDefOf.Thin,
        };
    }
}

file static class GenderSwapLetterExtensions
{
    public static Task SendGenderSwapLetterAsync(this ILetterService service, Pawn pawn, Gender previousGender) =>
        service.SendNeutralLetterAsync(
            TranslationExtensions.FromKey("TKUtils.Letters.GenderSwap.Title"),
            TranslationExtensions.FromKey("TKUtils.Letters.GenderSwap.Description").Format(
                new
                {
                    ViewerName = pawn.LabelShort,
                    PreviousGender = TranslationExtensions.FromKey(previousGender.ToStringFast()).Value,
                    NewGender = TranslationExtensions.FromKey(pawn.gender.ToStringFast()).Value,
                }
            ),
            pawn
        );
}
