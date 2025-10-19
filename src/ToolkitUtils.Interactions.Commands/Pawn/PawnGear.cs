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
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Presentation;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[Group("pawn")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnGear(GearSettings settings, ExecutionContext context) : CommandGroup
{
    [Command("gear")]
    public async Task<Result> GetPawnGearInformationAsync()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf());

        var builder = new StringBuilder();

        if (settings.ShouldShowTemperatureRange) builder.Append(await GetPawnTemperatureRatingAsync(pawn));
        if (settings.ShouldShowArmor) builder.Append(await GetPawnArmorRatingAsync(pawn));
        if (settings.ShouldShowWeapon) builder.Append(await GetPawnWeaponAsync(pawn));

        List<Apparel> apparel = pawn.apparel.WornApparel ?? [];

        if (!settings.ShouldShowApparel) return Result.Ok(builder.ToString());

        builder.Append(" | ");
        builder.Append(TranslationService.Instance.GetTranslation("Apparel"));
        builder.Append(": ");

        if (apparel.Count <= 0)
        {
            builder.Append(TranslationService.Instance.GetTranslation("None"));

            return Result.Ok(builder.ToString());
        }

        for (var i = 0; i < apparel.Count; i++)
        {
            builder.Append(RichTextHelper.StripTags(apparel[i].LabelCap));
            builder.Append(", ");
        }

        return Result.Ok(builder.ToString(startIndex: 0, builder.Length - 2));
    }

    private Task<string> GetPawnWeaponAsync(Pawn pawn)
    {
        var builder = new StringBuilder();
        List<ThingWithComps> equipment = pawn.equipment.AllEquipmentListForReading ?? [];

        if (equipment.Count <= 0) return Task.FromResult(string.Empty);

        for (var i = 0; i < equipment.Count; i++)
        {
            builder.Append(RichTextHelper.StripTags(equipment[i].LabelCap));
            builder.Append(", ");
        }

        builder.Insert(index: 0, LanguageDatabase.activeLanguage.Worker.Pluralize(TranslationService.Instance.GetTranslation("Stat_Weapon_Name")));

        return Task.FromResult(builder.ToString(startIndex: 0, builder.Length - 2));
    }

    private static async Task<string> GetPawnTemperatureRatingAsync(Pawn pawn)
    {
        float minimumTemperature = await pawn.GetStatValueAsync(StatDefOf.ComfyTemperatureMin);
        float maximumTemperature = await pawn.GetStatValueAsync(StatDefOf.ComfyTemperatureMax);

        string stringifiedMinimumTemperature = minimumTemperature.ToStringTemperature();
        string stringifiedMaximumTemperature = maximumTemperature.ToStringTemperature();

        return $"{UnicodeCharacter.Thermometer.Codepoint}{stringifiedMinimumTemperature}~{stringifiedMaximumTemperature}";
    }

    private async Task<string> GetPawnArmorRatingAsync(Pawn pawn)
    {
        float sharpArmorRating = await pawn.CalculateArmorRatingAsync(StatDefOf.ArmorRating_Sharp);
        float bluntArmorRating = await pawn.CalculateArmorRatingAsync(StatDefOf.ArmorRating_Blunt);
        float heatArmorRating = await pawn.CalculateArmorRatingAsync(StatDefOf.ArmorRating_Heat);

        var builder = new StringBuilder();

        builder.Append(TranslationService.Instance.GetTranslation("OverallArmor"));
        builder.Append(" ");

        builder.Append(UnicodeCharacter.Dagger.Codepoint);
        builder.Append(TranslationService.Instance.GetTranslation("ArmorSharp"));
        builder.Append(" ");
        builder.Append(sharpArmorRating.ToString("P2"));

        builder.Append(UnicodeCharacter.Pan.Codepoint);
        builder.Append(TranslationService.Instance.GetTranslation("ArmorBlunt"));
        builder.Append(" ");
        builder.Append(bluntArmorRating.ToString("P2"));

        builder.Append(UnicodeCharacter.Flame.Codepoint);
        builder.Append(TranslationService.Instance.GetTranslation("ArmorHeat"));
        builder.Append(" ");
        builder.Append(heatArmorRating.ToString("P2"));

        return builder.ToString();
    }
}
