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
using FormatWith;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Incidents.Extensions;

[PublicAPI]
public static class TranslationServiceIncidentResponseExtensions
{
    public static Translation FormatSelfFullHeal(this TranslationService service, int totalAfflictions) =>
        service.GetTranslation("TKUtils.Responses.Heal.Full.Self").Format(
            new
            {
                TotalAfflictions = totalAfflictions.ToString("N0"),
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatOtherFullHeal(this TranslationService service, Viewer viewer, int totalAfflictions) =>
        service.GetTranslation("TKUtils.Responses.Heal.Full.Other").Format(
            new
            {
                ViewerName = viewer.Name, TotalAfflictions = totalAfflictions.ToString("N0"),
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatSelfHeal(this TranslationService service, Hediff affliction) =>
        service.GetTranslation("TKUtils.Responses.Heal.Self").Format(
            new
            {
                AfflictionName = affliction.Label,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatOtherHeal(this TranslationService service, Viewer viewer, Hediff affliction) =>
        service.GetTranslation("TKUtils.Responses.Heal.Other").Format(
            new
            {
                ViewerName = viewer.Name, AfflictionName = affliction.Label,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatColonyHeal(this TranslationService service, int totalHealedColonists) =>
        service.GetTranslation("TKUtils.Responses.Heal.Colony").Format(
            new
            {
                TotalColonists = totalHealedColonists.ToString("N0"),
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatColonyFullHeal(this TranslationService service, int totalHealedColonists, int totalHealedAfflictions) =>
        service.GetTranslation("TKUtils.Responses.Heal.Full.Colony").Format(
            new
            {
                TotalColonists = totalHealedColonists.ToString("N0"), TotalAfflictions = totalHealedAfflictions.ToString("N0"),
            },
            MissingKeyBehaviour.Ignore
        );
}
