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
using System.Threading.Tasks;
using JetBrains.Annotations;
using NetEscapades.EnumGenerators;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;
using Result = ToolkitUtils.Mod.Result;

[assembly: EnumExtensions<QualityCategory>]

namespace ToolkitUtils.Interactions.Commands;

[Group("rwdata")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class Database : CommandGroup
{
    private static readonly QualityCategory[] Qualities = QualityCategoryExtensions.GetValues();
    private static readonly StatDef[] WeaponStats = [StatDefOf.AccuracyLong, StatDefOf.AccuracyMedium, StatDefOf.AccuracyShort,];
    private static readonly StatDef[] RangedWeaponStats = [StatDefOf.RangedWeapon_Cooldown, StatDefOf.RangedWeapon_DamageMultiplier,];
    private static readonly StatDef[] MeleeWeaponStats = [StatDefOf.MeleeWeapon_AverageDPS, StatDefOf.MeleeWeapon_CooldownMultiplier, StatDefOf.MeleeWeapon_DamageMultiplier,];

    [Command("weapon")]
    public async Task<Result> GetWeaponDataAsync(string query)
    {
        ExtendedMetadataParser parser = ExtendedMetadataParser.Parse(query);
        ItemProduct? weapon = parser.GetSubject(Registries.Items.AllRegistrants);

        if (weapon is not { Metadata: {} weaponMetadata, } || !weaponMetadata.Properties.HasFlagFast(ItemProperties.Weapon))
            return Result.Fail(TranslationService.Instance.FormatInvalidQuery(query));

        ItemProduct? material = parser.GetMetadata(Registries.Items.AllRegistrants);
        QualityCategory quality = parser.GetMetadata(Qualities, nameGetter: category => category.ToStringFast(), defaultGetter: () => QualityCategory.Normal);

        Thing thing = ThingMaker.MakeThing(weapon.Def, material?.Def);
        thing.TryGetComp<CompQuality>().SetQuality(quality, ArtGenerationContext.Outsider);

        StatDef[] categoryStats = weapon.Def.IsRangedWeapon ? RangedWeaponStats : MeleeWeaponStats;

        int length = WeaponStats.Length + categoryStats.Length;
        var container = new List<string>(length);

        for (var i = 0; i < WeaponStats.Length; i++) container.Add(await GetStatValueAsync(thing, WeaponStats[i]));
        for (var i = 0; i < categoryStats.Length; i++) container.Add(await GetStatValueAsync(thing, categoryStats[i]));

        return Result.Ok($"{weapon.Name}: {string.Join(separator: ", ", container)}");
    }

    private async Task<string> GetStatValueAsync(Thing thing, StatDef stat)
    {
        float value = await RouterService.Instance.RouteToMainAsync(GetStatValue, thing, stat);

        return await RouterService.Instance.RouteToMainAsync(ValueToString, stat, value);
    }

    private static string ValueToString(StatDef stat, float value) => stat.ValueToString(value);

    private static float GetStatValue(Thing thing, StatDef stat) => thing.GetStatValue(stat);
}
