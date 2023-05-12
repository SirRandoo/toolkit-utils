﻿// ToolkitUtils
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
using SirRandoo.CommonLib.Helpers;
using ToolkitUtils.Interfaces;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using UnityEngine;
using Verse;

namespace ToolkitUtils.IncidentSettings.Embeds
{
    public class LevelPawnSettingEmbed : IEventSettings
    {
        private string _buffer;
        private bool _bufferValid = true;
        public int LineSpan => 1;

        public void Draw(Rect canvas, float preferredHeight)
        {
            (Rect label, Rect field) = new Rect(canvas.x, canvas.y, canvas.width, preferredHeight).Split(0.65f);
            UiHelper.Label(label, "TKUtils.Fields.LevelSkillMultiplier".TranslateSimple());

            if (UiHelper.NumberField(field, out float value, ref _buffer, ref _bufferValid, 0.5f, 5f))
            {
                LevelPawnSettings.xpMultiplier = value;
            }
        }
    }
}
