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
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Incidents.Extensions;

public static class TranslationServiceIncidentLetterExtensions
{
    public static Translation FormatSelfHealLetterDescription(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Letters.Heal.Self.Description").Format(
            new
            {
                ViewerName = viewer.Name,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatRandomHealLetterDescription(this TranslationService service, Viewer viewer, Pawn pawn) =>
        service.GetTranslation("TKUtils.Letters.Heal.Random.Description").Format(
            new
            {
                ViewerName = viewer.Name, RandomColonistName = pawn.LabelShortCap,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatColonyHealLetterDescription(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Letters.Heal.Colony.Description").Format(
            new
            {
                ViewerName = viewer.Name,
            },
            MissingKeyBehaviour.Ignore
        );

    public static Translation FormatFullHealLetterDescription(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Letters.Heal.Full.Description").Format(
            new
            {
                ViewerName = viewer.Name,
            },
            MissingKeyBehaviour.Ignore
        );
}
