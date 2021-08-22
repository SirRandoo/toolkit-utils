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

using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class CoreWorkerTab : MainButtonWorker_ToggleTab
    {
        public override void DoButton(Rect region)
        {
            base.DoButton(region);

            Vector2 center = region.center;

            if (Mouse.IsOver(region))
            {
                center += new Vector2(2f, -2f);
            }

            var iconRect = new Rect(region.x + region.width - 20f, region.y + 3f, 16f, 16f);
            HealthReport mostSevere = Data.HealthReports.OrderByDescending(r => r.Type).FirstOrDefault();

            if (mostSevere == null)
            {
                return;
            }

            Texture2D texture = null;
            switch (mostSevere.Type)
            {
                case HealthReport.ReportType.Info:
                    texture = Textures.Info;
                    GUI.color = ColorLibrary.PaleGreen;
                    break;
                case HealthReport.ReportType.Warning:
                    texture = Textures.Warning;
                    GUI.color = ColorLibrary.Yellow;
                    break;
                case HealthReport.ReportType.Error:
                    texture = Textures.Warning;
                    GUI.color = ColorLibrary.Salmon;
                    break;
                case HealthReport.ReportType.Debug:
                    texture = Textures.Debug;
                    GUI.color = ColorLibrary.LightPink;
                    break;
            }

            if (texture != null)
            {
                GUI.DrawTexture(iconRect, texture);
            }

            GUI.color = Color.white;
        }
    }
}
