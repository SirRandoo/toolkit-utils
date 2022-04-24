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

using TwitchToolkit.Settings;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    /// <summary>
    ///     A required class for being registered within Twitch Toolkit as an
    ///     addon.
    /// </summary>
    public class TkUtilsWindow : ToolkitWindow
    {
        public TkUtilsWindow(Mod mod) : base(mod)
        {
            Mod = mod;
        }

        /// <inheritdoc cref="SettingsWindow.DoWindowContents"/>
        public override void DoWindowContents(Rect inRect)
        {
            Mod.DoSettingsWindowContents(new Rect(inRect.x, inRect.y, inRect.width, inRect.height - 40f - CloseButSize.y));
        }
    }
}
