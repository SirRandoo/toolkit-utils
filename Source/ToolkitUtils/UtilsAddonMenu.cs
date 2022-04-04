// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Interfaces;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class UtilsAddonMenu : IAddonMenu
    {
        private static readonly List<FloatMenuOption> Options = new List<FloatMenuOption>
        {
            new FloatMenuOption("TKUtils.AddonMenu.Settings".TranslateSimple(), () => Find.WindowStack.Add(new UtilsSettingsWindow())),
            new FloatMenuOption("TKUtils.AddonMenu.Editor".TranslateSimple(), () => Find.WindowStack.Add(new Editor())),
            new FloatMenuOption("TKUtils.AddonMenu.PawnKind".TranslateSimple(), () => Find.WindowStack.Add(new PawnKindConfigDialog())),
            new FloatMenuOption("TKUtils.AddonMenu.Trait".TranslateSimple(), () => Find.WindowStack.Add(new TraitConfigDialog())),
            new FloatMenuOption("TKUtils.AddonMenu.Purge".TranslateSimple(), () => Find.WindowStack.Add(new PurgeViewersDialog())),
            new FloatMenuOption("Wiki".TranslateSimple(), () => Application.OpenURL("https://sirrandoo.github.io/toolkit-utils"))
        };

        public List<FloatMenuOption> MenuOptions() => Options;
    }
}
