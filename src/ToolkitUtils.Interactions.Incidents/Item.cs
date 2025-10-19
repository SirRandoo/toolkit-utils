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
using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Entities;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;
using Verse.AI;
using QualitySettings = ToolkitUtils.Mod.Domain.Settings.QualitySettings;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class Item(ExecutionContext context, ILetterService letterService, QualitySettings qualitySettings, ISpawnService spawnService) : CommandGroup
{
    [Command("item")]
    public async Task<Result> BuyItemAsync(CompositeItemProduct item, int amount = 1)
    {
        Result passesStoreRequirements = PassesStoreRequirements(item, amount);

        if (!passesStoreRequirements.IsSuccess) return passesStoreRequirements;

        Result result = await HasUnfinishedResearchAsync(item);

        if (!result.IsSuccess) return result;

        Map? map = await RouterService.Instance.GetRandomPlayerHomeMapAsync();

        if (map == null) return Result.Fail(TranslationService.Instance.GetNoPlayerMapError());

        IntVec3 position = await RouterService.Instance.GetTradeDropSpotAsync(map);

        if (position == IntVec3.Invalid) return Result.Fail(new Translation("No drop cell could be found".MarkNotTranslated()));

        Result<Transaction> transaction = context.Invoker.ReserveCoins(item.Cost * amount);

        if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(item.Cost * amount, context.Invoker.Coins));

        Thing thing = await CreateThing(item);
        thing.stackCount = amount;
        await RouterService.Instance.RouteToMainAsync(TradeUtility.SpawnDropPod, position, map, thing);
        transaction.Value.PostTransaction();

        await letterService.SendNeutralLetterAsync(
            thing.LabelCap,
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.ItemPurchase.Body").Format(
                new
                {
                    ViewerName = context.Invoker.Name, Amount = amount.ToString("N0"), ItemName = thing.Label,
                }
            ),
            thing
        );

        return Result.Ok(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.ItemPurchase").Format(
                new
                {
                    Amount = amount.ToString("N0"), ItemName = thing.Label, TotalCost = (item.Cost * amount).ToString("N0"),
                }
            )
        );
    }

    [Command("wear")]
    public async Task<Result> WearItemAsync(CompositeItemProduct item)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.GetPawnRequiredError(context.Invoker));

        Result passesStoreRequirements = PassesStoreRequirements(item, amount: 1);

        if (!passesStoreRequirements.IsSuccess) return passesStoreRequirements;

        Result result = await HasUnfinishedResearchAsync(item);

        if (!result.IsSuccess) return result;

        Result<Transaction> transaction = context.Invoker.ReserveCoins(item.Cost);

        if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(item.Cost, context.Invoker.Coins));

        Thing thing = await CreateThing(item);
        await WearInternalAsync(pawn, (Apparel)thing);
        transaction.Value.PostTransaction();

        await letterService.SendNeutralLetterAsync(
            thing.LabelCap,
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.Complete").Format(
                new
                {
                    ViewerName = context.Invoker.Name, ApparelName = thing.Label,
                }
            ),
            thing
        );

        return Result.Ok(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.Wear").Format(
                new
                {
                    ApparelName = thing.Label,
                }
            )
        );
    }

    [Command("equip")] public async Task<Result> EquipItemAsync(CompositeItemProduct item)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.GetPawnRequiredError(context.Invoker));

        Result passesStoreRequirements = PassesStoreRequirements(item, amount: 1);

        if (!passesStoreRequirements.IsSuccess) return passesStoreRequirements;

        Result result = await HasUnfinishedResearchAsync(item);

        if (!result.IsSuccess) return result;

        Result<Transaction> transaction = context.Invoker.ReserveCoins(item.Cost);

        if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(item.Cost, context.Invoker.Coins));

        Thing thing = await CreateThing(item);
        await EquipInternalAsync(pawn, (Apparel)thing);
        transaction.Value.PostTransaction();

        await letterService.SendNeutralLetterAsync(
            thing.LabelCap,
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Equip.Complete").Format(
                new
                {
                    ViewerName = context.Invoker.Name, ApparelName = thing.Label,
                }
            ),
            thing
        );

        return Result.Ok(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.Equip").Format(
                new
                {
                    ApparelName = thing.Label,
                }
            )
        );
    }

    [Command("use")] public async Task<Result> UseItemAsync(ItemProduct item, int amount = 1) => throw new NotImplementedException();

    [Command("backpack")] public async Task<Result> BackpackAsync(CompositeItemProduct item, int amount = 1)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.GetPawnRequiredError(context.Invoker));

        Result passesStoreRequirements = PassesStoreRequirements(item, amount: 1);

        if (!passesStoreRequirements.IsSuccess) return passesStoreRequirements;

        Result result = await HasUnfinishedResearchAsync(item);

        if (!result.IsSuccess) return result;

        Result<Transaction> transaction = context.Invoker.ReserveCoins(item.Cost);

        if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(item.Cost, context.Invoker.Coins));

        Thing thing = await CreateThing(item);
        await BackpackInternalAsync(pawn, thing);
        transaction.Value.PostTransaction();

        await letterService.SendNeutralLetterAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Backpack.Title"),
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Backpack.Body").Format(
                new
                {
                    ViewerName = context.Invoker.Name, Amount = amount.ToString("N0"), ItemName = thing.Label,
                }
            ),
            thing
        );

        return Result.Ok(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.Backpack").Format(
                new
                {
                    Amount = amount.ToString("N0"), ItemName = thing.Label, TotalCost = item.Cost.ToString("N0"),
                }
            )
        );
    }

    private async Task<Result> HasUnfinishedResearchAsync(CompositeItemProduct item)
    {
        if (!BuyItemSettings.mustResearchFirst || item.Item.ResearchPrerequisites.Count <= 0) return Result.Ok();

        foreach (ResearchProjectDef def in item.Item.ResearchPrerequisites)
        {
            if (await RouterService.Instance.GetResearchProgressAsync(def) < 1.0f)
                return Result.Fail(TranslationService.Instance.FormatUnfulfilledResearchPrerequisites(def, item.Item));
        }

        if (item.Material is not { ResearchPrerequisites.Count: > 0, }) return Result.Ok();

        foreach (ResearchProjectDef def in item.Material.ResearchPrerequisites)
        {
            if (await RouterService.Instance.GetResearchProgressAsync(def) < 1.0f)
                return Result.Fail(TranslationService.Instance.FormatUnfulfilledResearchPrerequisites(def, item.Material));
        }

        return Result.Ok();
    }

    private Result PassesStoreRequirements(CompositeItemProduct item, int amount)
    {
        if (Current.Game == null) return Result.Fail(TranslationService.Instance.GetNoGameError());
        if (Find.Maps == null || Find.Maps.Count <= 0) return Result.Fail(TranslationService.Instance.FormatNoMapFound());
        if (!item.Item.Purchasable) return Result.Fail(TranslationService.Instance.FormatProductDisabled(item.Item));

        return item.Material is not { Purchasable: true, } ? Result.Fail(TranslationService.Instance.FormatProductDisabled(item.Material!)) : IsQualityEnabled(item.Quality);
    }

    private Result IsQualityEnabled(ItemQuality quality)
    {
        if (quality == ItemQuality.Awful && !qualitySettings.Awful.IsEnabled) return Result.Fail(new Translation("Cannot purchase 'awful' quality items".MarkNotTranslated()));
        if (quality == ItemQuality.Poor && !qualitySettings.Poor.IsEnabled) return Result.Fail(new Translation("Cannot purchase 'poor' quality items".MarkNotTranslated()));
        if (quality == ItemQuality.Normal && !qualitySettings.Normal.IsEnabled) return Result.Fail(new Translation("Cannot purchase 'normal' quality items".MarkNotTranslated()));
        if (quality == ItemQuality.Good && !qualitySettings.Good.IsEnabled) return Result.Fail(new Translation("Cannot purchase 'good' quality items".MarkNotTranslated()));
        if (quality == ItemQuality.Excellent && !qualitySettings.Excellent.IsEnabled)
            return Result.Fail(new Translation("Cannot purchase 'excellent' quality items".MarkNotTranslated()));
        if (quality == ItemQuality.Masterwork && !qualitySettings.Masterwork.IsEnabled)
            return Result.Fail(new Translation("Cannot purchase 'masterwork' quality items".MarkNotTranslated()));

        return Result.Ok();
    }

    private async ValueTask<Thing> CreateThing(CompositeItemProduct item)
    {
        Thing thing = await RouterService.Instance.MakeThingAsync(item.Item.Def, item.Material!.Def);

        thing.TryGetComp<CompQuality>()?.SetQuality(item.Quality, ArtGenerationContext.Outsider);

        return thing;
    }

    private async Task<Result> BackpackInternalAsync(Pawn pawn, Thing thing)
    {
        bool wouldOverencumber = await RouterService.Instance.RouteToMainAsync(MassUtility.WillBeOverEncumberedAfterPickingUp, pawn, thing, arg3: 1);

        if (wouldOverencumber) return await spawnService.SpawnItemAsync(thing, pawn.Map, pawn.Position);

        bool canEverCarryAnything = await RouterService.Instance.RouteToMainAsync(MassUtility.CanEverCarryAnything, pawn);

        if (!canEverCarryAnything) return await spawnService.SpawnItemAsync(thing, pawn.Map, pawn.Position);

        bool wasAddedToInventory = await RouterService.Instance.RouteToMainAsync(pawn.inventory.innerContainer.TryAdd, thing, arg2: true);

        if (wasAddedToInventory) return Result.Ok();

        return await spawnService.SpawnItemAsync(thing, pawn.Map, pawn.Position);
    }

    private async Task<Result> EquipWeaponDirectAsync(Pawn pawn, ThingWithComps weapon)
    {
        await RouterService.Instance.RouteToMainAsync(pawn.equipment.AddEquipment, weapon);

        return Result.Ok();
    }

    private async Task<Result> EquipWeaponWithJobAsync(Pawn pawn, ThingWithComps weapon)
    {
        await spawnService.SpawnItemAsync(weapon, pawn.Map, pawn.Position);

        // TODO: This "equip weapon" job may or may not work. Be sure to test it.
        Job equipJob = await RouterService.Instance.RouteToMainAsync(JobMaker.MakeJob, JobDefOf.Equip, (LocalTargetInfo)weapon);
        await RouterService.Instance.RouteToMainAsync(pawn.jobs.jobQueue.EnqueueFirst, equipJob, (JobTag?)JobTag.UnspecifiedLordDuty);

        return Result.Ok();
    }

    private async Task<Result> EquipWeaponInternalAsync(Pawn pawn, ThingWithComps weapon)
    {
        if (pawn.equipment.Primary == null) return await EquipWeaponDirectAsync(pawn, weapon);

        bool transferred = await RouterService.Instance.RouteToMainAsync(pawn.equipment.TryTransferEquipmentToContainer, pawn.equipment.Primary, pawn.inventory.innerContainer);

        if (transferred) return await EquipWeaponDirectAsync(pawn, weapon);

        bool dropped = await RouterService.Instance.RouteToMainAsync(func: p => p.equipment.TryDropEquipment(p.equipment.Primary, out ThingWithComps _, p.Position), pawn);

        if (dropped) return await EquipWeaponDirectAsync(pawn, weapon);

        return await EquipWeaponWithJobAsync(pawn, weapon);
    }

    private async Task<Result> EquipInternalAsync(Pawn pawn, Thing weapon)
    {
        bool canEquip = await RouterService.Instance.RouteToMainAsync(EquipmentUtility.CanEquip, weapon, pawn);

        if (!canEquip) return await EquipInternalWithUnequippableAsync(pawn, weapon);

        return Result.Ok();
    }

    private async Task<Result> EquipInternalWithUnequippableAsync(Pawn pawn, Thing weapon)
    {
        await spawnService.SpawnItemAsync(weapon, pawn.Map, pawn.Position);
        await context.SendReplyAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.Equip.Unequippable").Format(
                new
                {
                    WeaponName = weapon.Label,
                }
            )
        );

        await letterService.SendNeutralLetterAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Equip.Title"),
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Equip.Unequippable").Format(
                new
                {
                    ViewerName = context.Invoker.Name, WeaponName = weapon.Label,
                }
            ),
            weapon
        );

        return Result.Ok();
    }

    private async Task<Result> WearInternalAsync(Pawn pawn, Apparel apparel)
    {
        bool hasPartsToWear = await RouterService.Instance.RouteToMainAsync(ApparelUtility.HasPartsToWear, pawn, apparel.def);

        if (!hasPartsToWear) return await WearInternalWithNoPartsAsync(pawn, apparel);

        bool wouldReplaceLockedApparel = await RouterService.Instance.RouteToMainAsync(pawn.apparel.WouldReplaceLockedApparel, apparel);

        if (wouldReplaceLockedApparel) return await WearInternalWithLockedApparel(pawn, apparel);

        bool canEquip = await RouterService.Instance.RouteToMainAsync(EquipmentUtility.CanEquip, apparel, pawn);

        if (!canEquip) return await WearInternalWithUnequippableApparel(pawn, apparel);

        await RouterService.Instance.RouteToMainAsync(pawn.apparel.Wear, apparel, arg2: true, arg3: true);
        await RouterService.Instance.RouteToMainAsync(pawn.outfits.forcedHandler.SetForced, apparel, arg2: true);
        await context.SendReplyAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.Wear").Format(
                new
                {
                    ApparelName = apparel.Label,
                }
            )
        );
        await letterService.SendNeutralLetterAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.Title"),
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.Complete").Format(
                new
                {
                    ApparelName = apparel.Label, ViewerName = context.Invoker.Name,
                }
            ),
            pawn
        );

        return Result.Ok();
    }

    private async Task<Result> WearInternalWithNoPartsAsync(Pawn pawn, Apparel apparel)
    {
        Result result = await BackpackInternalAsync(pawn, apparel);

        if (!result.IsSuccess) return result;

        await context.SendReplyAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.Wear.NoParts").Format(
                new
                {
                    ApparelName = apparel.Label,
                }
            )
        );

        await letterService.SendNeutralLetterAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.Title"),
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.NoParts").Format(
                new
                {
                    ViewerName = context.Invoker.Name, ApparelName = apparel.Label,
                }
            ),
            pawn
        );

        return Result.Ok();
    }

    private async Task<Result> WearInternalWithLockedApparel(Pawn pawn, Apparel apparel)
    {
        Result result = await BackpackInternalAsync(pawn, apparel);

        if (!result.IsSuccess) return result;

        await context.SendReplyAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.Wear.Locked").Format(
                new
                {
                    ApparelName = apparel.Label,
                }
            )
        );

        await letterService.SendNeutralLetterAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.Title"),
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.Locked").Format(
                new
                {
                    ViewerName = context.Invoker.Name, ApparelName = apparel.Label,
                }
            ),
            pawn
        );

        return Result.Ok();
    }

    private async Task<Result> WearInternalWithUnequippableApparel(Pawn pawn, Apparel apparel)
    {
        Result result = await BackpackInternalAsync(pawn, apparel);

        if (!result.IsSuccess) return result;

        await context.SendReplyAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Responses.Item.Wear.Unequippable").Format(
                new
                {
                    ApparelName = apparel.Label,
                }
            )
        );

        await letterService.SendNeutralLetterAsync(
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.Title"),
            TranslationService.Instance.GetTranslation("TKUtils.Letters.Item.Wear.Unequippable").Format(
                new
                {
                    ViewerName = context.Invoker.Name, ApparelName = apparel.Label,
                }
            ),
            pawn
        );

        return Result.Ok();
    }
}
