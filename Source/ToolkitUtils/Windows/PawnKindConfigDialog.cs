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
using System.Threading.Tasks;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PawnKindConfigDialog : Window
    {
        private readonly PawnTableWorker worker;
        private float lastSearchTick;
        private string query = "";
        private string resetText;
        private string searchText;
        private Vector2 searchTextSize;
        private bool shouldResizeTable = true;

        private string titleText;


        public PawnKindConfigDialog()
        {
            doCloseX = true;
            forcePause = true;
            worker = new PawnTableWorker();
        }

        public override Vector2 InitialSize => new Vector2(640f, 740f);
        protected override float Margin => 22f;

        private void NotifySearchRequested()
        {
            lastSearchTick = 10f;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            GetTranslations();
            worker.Prepare();
            optionalTitle = titleText;
        }

        private void GetTranslations()
        {
            titleText = "TKUtils.PawnKindStore.Title".Localize();
            searchText = "TKUtils.Buttons.Search".Localize();
            resetText = "TKUtils.Buttons.ResetAll".Localize();

            searchTextSize = Text.CalcSize(searchText);
        }

        private void DrawHeader(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            var searchRect = new Rect(canvas.x, canvas.y, Mathf.FloorToInt(canvas.width * 0.3f), Text.LineHeight);
            var searchLabel = new Rect(searchRect.x, searchRect.y, searchTextSize.x, searchRect.height);
            var searchField = new Rect(
                searchLabel.x + searchLabel.width + 5f,
                searchRect.y,
                searchRect.width - searchLabel.width - 5f,
                searchRect.height
            );

            SettingsHelper.DrawLabel(searchLabel, searchText);

            if (SettingsHelper.DrawTextField(searchField, query, out string input))
            {
                query = input;
                NotifySearchRequested();
            }

            if (query.Length > 0 && SettingsHelper.DrawClearButton(searchField))
            {
                query = "";
                NotifySearchRequested();
            }


            float resetWidth = Text.CalcSize(resetText).x + 16f;
            var resetRect = new Rect(canvas.width - resetWidth, canvas.y, resetWidth, Text.LineHeight);

            DrawGlobalResetButton(resetRect);

            GUI.EndGroup();
        }

        private void DrawGlobalResetButton(Rect resetRect)
        {
            if (!Widgets.ButtonText(resetRect, resetText))
            {
                return;
            }

            foreach (TableSettingsItem<PawnKindItem> item in worker.Data.Where(i => !i.IsHidden)
               .Where(i => i.Data.ColonistKindDef != null))
            {
                item.Data.Cost = item.Data.ColonistKindDef.race.CalculateStorePrice();
                item.Data.Name = item.Data.GetDefaultName();
                item.Data.Enabled = true;
                item.Data.PawnData.CustomName = false;
                item.Data.Data.KarmaType = null;
            }
        }

        public override void DoWindowContents(Rect canvas)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GUI.BeginGroup(canvas);

            var headerRect = new Rect(0f, 0f, canvas.width, Text.LineHeight);
            var contentArea = new Rect(
                canvas.x,
                Text.LineHeight * 4f,
                canvas.width,
                canvas.height - Text.LineHeight * 4f
            );

            DrawHeader(headerRect);
            Widgets.DrawLineHorizontal(canvas.x, Text.LineHeight * 2f, canvas.width);

            bool wrapped = Text.WordWrap;
            Text.WordWrap = false;

            GUI.BeginGroup(contentArea);

            if (shouldResizeTable)
            {
                worker.NotifyResolutionChanged(contentArea.AtZero());
                shouldResizeTable = false;
            }

            worker.Draw(contentArea.AtZero());
            GUI.EndGroup();
            GUI.EndGroup();

            Text.WordWrap = wrapped;
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();


            if (lastSearchTick <= 0)
            {
                worker.NotifySearchRequested(query);
            }
            else
            {
                lastSearchTick -= Time.unscaledTime - lastSearchTick;
            }
        }

        public override void PreClose()
        {
            if (TkSettings.Offload)
            {
                Task.Run(
                        async () =>
                        {
                            switch (TkSettings.DumpStyle)
                            {
                                case "MultiFile":
                                    await Data.SavePawnKindsAsync(Paths.PawnKindFilePath);
                                    return;
                                case "SingleFile":
                                    await Data.SaveLegacyShopAsync(Paths.LegacyShopDumpFilePath);
                                    return;
                            }
                        }
                    )
                   .ConfigureAwait(false);
            }
            else
            {
                switch (TkSettings.DumpStyle)
                {
                    case "MultiFile":
                        Data.SavePawnKinds(Paths.PawnKindFilePath);
                        return;
                    case "SingleFile":
                        Data.SaveLegacyShop(Paths.LegacyShopDumpFilePath);
                        return;
                }
            }
        }
    }
}
