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
// with ToolkitUtils.TMagic. If not, see <https://www.gnu.org/licenses/>.
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using TorannMagic;

namespace ToolkitUtils.TMagic.Extensions;

[PublicAPI]
public static class TranslationServiceTMagicErrorExtensions
{
    public static Translation GetNoMightAffinityError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoMightAffinity");
    public static Translation GetNoMagicAffinityError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoMagicAffinity");

    public static Translation FormatClassLimitReached(this TranslationService service, TraitDef classTrait) =>
        service.GetTranslation("TKUtils.Errors.ClassLimitReached").Format(
            new
            {
                ClassName = classTrait.label,
            }
        );

    public static Translation FormatClassTraitRemovalForbidden(this TranslationService service, TraitDef classTrait) =>
        service.GetTranslation("TKUtils.Errors.ClassTraitRemovalForbidden").Format(
            new
            {
                ClassName = classTrait.label,
            }
        );

    public static Translation GetNoClassError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.PawnHasNoClass");
    public static string FormatCannotLeaveNecromancer(this TranslationService service) => service.GetTranslation("TKUtils.Errors.CannotLeaveNecromancer");
    public static Translation GetNoAbilityPointsError(this TranslationService service) => service.GetTranslation("TKUtils.Errors.NoSkillPoints");

    public static Translation GetAbilityMaxedError(this TranslationService service, MightPower power) =>
        service.GetTranslation("TKUtils.Error.AbilityMaxed").Format(
            new
            {
                AbilityName = power.abilityDef.label,
            }
        );

    public static Translation GetAbilityMaxedError(this TranslationService service, MagicPower power) =>
        service.GetTranslation("TKUtils.Errors.AbilityMaxed").Format(
            new
            {
                AbilityName = power.abilityDef.label,
            }
        );

    public static Translation GetAbilityMaxedError(this TranslationService service, MightPowerSkill power) =>
        service.GetTranslation("TKUtils.Errors.AbilityMaxed").Format(
            new
            {
                AbilityName = power.label,
            }
        );

    public static Translation GetAbilityMaxedError(this TranslationService service, MagicPowerSkill power) =>
        service.GetTranslation("TKUtils.Errors.AbilityMaxed").Format(
            new
            {
                AbilityName = power.label,
            }
        );

    public static Translation GetInsufficientAbilityPointsError(this TranslationService service, MightPower power, int currentPoints) =>
        service.GetTranslation("TKUtils.Errors.InsufficientAbilityPoints").Format(
            new
            {
                AbilityName = power.abilityDef.label, PointsToNextLevel = power.costToLevel.ToString("N0"), CurrentAbilityPoints = currentPoints.ToString("N0"),
            }
        );

    public static Translation GetInsufficientAbilityPointsError(this TranslationService service, MagicPower power, int currentPoints) =>
        service.GetTranslation("TKUtils.Errors.InsufficientAbilityPoints").Format(
            new
            {
                AbilityName = power.abilityDef.label, PointsToNextLevel = power.costToLevel.ToString("N0"), CurrentAbilityPoints = currentPoints.ToString("N0"),
            }
        );

    public static Translation GetInsufficientAbilityPointsError(this TranslationService service, MightPowerSkill power, int currentPoints) =>
        service.GetTranslation("TKUtils.Errors.InsufficientAbilityPoints").Format(
            new
            {
                AbilityName = power.label, PointsToNextLevel = power.costToLevel.ToString("N0"), CurrentAbilityPoints = currentPoints.ToString("N0"),
            }
        );

    public static Translation GetInsufficientAbilityPointsError(this TranslationService service, MagicPowerSkill power, int currentPoints) =>
        service.GetTranslation("TKUtils.Errors.InsufficientAbilityPoints").Format(
            new
            {
                AbilityName = power.label, PointsToNextLevel = power.costToLevel.ToString("N0"), CurrentAbilityPoints = currentPoints.ToString("N0"),
            }
        );
}
