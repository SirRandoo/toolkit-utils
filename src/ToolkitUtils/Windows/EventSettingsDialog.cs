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

using ToolkitUtils.Interfaces;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Windows
{
    /// <summary>
    ///     A dialog for drawing <see cref="IEventSettings"/>s.
    /// </summary>
    public class EventSettingsDialog : Window
    {
        private readonly IEventSettings _settings;

        public EventSettingsDialog(IEventSettings settings)
        {
            _settings = settings;
            doCloseButton = true;
        }

        /// <inheritdoc cref="Window.DoWindowContents"/>
        public override void DoWindowContents(Rect region)
        {
            _settings.Draw(region, Text.LineHeight);
        }
    }
}
