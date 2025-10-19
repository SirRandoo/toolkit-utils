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
using FormatWith;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using TorannMagic;

namespace ToolkitUtils.TMagic.Extensions;

[PublicAPI]
public static class TranslationServiceTMagicResponseExtensions
{
    public static Translation FormatAbilityPowerGrew(this TranslationService service, MagicPower power) =>
        service.GetTranslation("TKUtils.Responses.AbilityPowerGrew").Format(
            new
            {
                AbilityName = power.abilityDef.label,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatAbilityPowerGrew(this TranslationService service, MightPower power) =>
        service.GetTranslation("TKUtils.Responses.AbilityPowerGrew").Format(
            new
            {
                AbilityName = power.abilityDef.label,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatAbilityPowerGrew(this TranslationService service, MightPowerSkill power) =>
        service.GetTranslation("TKUtils.Responses.AbilityPowerGrew").Format(
            new
            {
                AbilityName = power.label,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatAbilityPowerGrew(this TranslationService service, MagicPowerSkill power) =>
        service.GetTranslation("TKUtils.Responses.AbilityPowerGrew").Format(
            new
            {
                AbilityName = power.label,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatClassLevel(this TranslationService service, int level) =>
        string.Format(format: "{0}{1:N0}", service.GetTranslation("TM_MCU_CurrentLevel"), level);

    public static Translation FormatClassExperience(this TranslationService service, int current, int cap) =>
        service.GetTranslation("TKUtils.Responses.TorannClassExperience").Format(
            new
            {
                CurrentLevelExperience = current.ToString("N0"), LevelExperienceCap = cap.ToString("N0"),
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatClassAbilityPoints(this TranslationService service, int points) =>
        string.Format(format: "{0:N0} {1}", points, service.GetTranslation("TM_PointsAvail"));
}
