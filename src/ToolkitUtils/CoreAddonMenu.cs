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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ToolkitCore;
using ToolkitCore.Interfaces;
using ToolkitCore.Windows;
using ToolkitUtils.Api;
using ToolkitUtils.Windows;
using UnityEngine;
using Verse;

namespace ToolkitUtils
{
    /// <summary>
    ///     An <see cref="IAddonMenu"/> used by ToolkitCore to display a set
    ///     of "quick menu options" for users.
    /// </summary>
    [UsedImplicitly]
    public class CoreAddonMenu : IAddonMenu
    {
        private static readonly List<FloatMenuOption> Options = new List<FloatMenuOption>
        {
            new FloatMenuOption("TKUtils.AddonMenu.Settings".TranslateSimple(), () => Find.WindowStack.Add(new CoreSettingsWindow())),
            new FloatMenuOption("Message Log", () => Find.WindowStack.Add(new Window_MessageLog())),
            new FloatMenuOption("Help", () => Application.OpenURL("https://github.com/hodldeeznuts/ToolkitCore/wiki")),
            new FloatMenuOption(
                "TKUtils.AddonMenu.Reconnect".TranslateSimple(),
                () => Task.Run(
                    () =>
                    {
                        if (TwitchWrapper.Client == null || !TwitchWrapper.Client.IsConnected)
                        {
                            TwitchWrapper.StartAsync();

                            return;
                        }

                        try
                        {
                            TwitchWrapper.Client.Disconnect();
                        }
                        catch (Exception e)
                        {
                            ExceptionHandler.HandleException(e);
                        }

                        TwitchWrapper.StartAsync();
                    }
                )
            )
        };

        /// <inheritdoc cref="IAddonMenu.MenuOptions"/>
        public List<FloatMenuOption> MenuOptions() => Options;
    }
}
