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
    /// <summary>
    ///     A <see cref="RimWorld.MainButtonWorker"/> responsible for
    ///     displaying a status icon over <see cref="CoreMainTab"/> to inform
    ///     users of any errors that may appear.
    /// </summary>
    [UsedImplicitly]
    public class CoreWorkerTab : MainButtonWorker_ToggleTab
    {
        public override void DoButton(Rect rect)
        {
            base.DoButton(rect);

            Rect iconRect;

            if (def.Icon == null)
            {
                iconRect = new Rect(rect.x + rect.width - 20f, rect.y + 3f, 16f, 16f);
            }
            else
            {
                Vector2 center = rect.center;

                bool isOver = Mouse.IsOver(rect);
                iconRect = new Rect(center.x + (isOver ? 6f : 4f), center.y - (isOver ? 14f : 12f), 16f, 16f);
            }

            HealthReport mostSevere = Data.AllHealthReports.OrderByDescending(r => r.Type).FirstOrDefault();

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
