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
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;

namespace ToolkitUtils.Interactions.Commands.Extensions;

public static class TranslationServiceLetterExtensions
{
    public static string FormatThanosLeaveLetterDescription(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Letters.PawnLeave.Thanos.Description").Format(
            new
            {
                ViewerName = viewer.Name,
            }
        );

    public static string FormatGenericLeaveLetterDescription(this TranslationService service, Viewer viewer) =>
        service.GetTranslation("TKUtils.Letters.PawnLeave.Generic.Description").Format(
            new
            {
                ViewerName = viewer.Name,
            }
        );
}
