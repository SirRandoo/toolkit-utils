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
using System.Linq;
using System.Threading.Tasks;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using SirRandoo.ToolkitUtils.Workers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class TraitConfigDialog : Window
    {
        private readonly TraitTableWorker worker;
        private string addCostText;
        private int globalAddCost;
        private int globalRemoveCost;
        private float lastSearchTick;

        private string query = "";
        private string removeCostText;
        private string resetAllText;
        private Vector2 resetAllTextSize;
        private string searchText;
        private bool shouldResizeTable = true;
        private string titleText;

        public TraitConfigDialog()
        {
            doCloseX = true;
            worker = new TraitTableWorker();
        }

        public override Vector2 InitialSize => new Vector2(1024f, UI.screenHeight * 0.9f);
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
            titleText = "TKUtils.TraitStore.Title".Localize();
            searchText = "TKUtils.Buttons.Search".Localize();
            addCostText = "TKUtils.Fields.AddPrice".Localize();
            removeCostText = "TKUtils.Fields.RemovePrice".Localize();
            resetAllText = "TKUtils.Buttons.ResetAll".Localize();

            resetAllTextSize = Text.CalcSize(resetAllText);
        }

        private void DrawHeader(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            (Rect searchLabel, Rect searchField) =
                new Rect(canvas.x, canvas.y, canvas.width * 0.19f, Text.LineHeight).ToForm(0.3f);

            Widgets.Label(searchLabel, searchText);

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


            float resetBtnWidth = resetAllTextSize.x + 16f;
            var gResetRect = new Rect(
                canvas.x + canvas.width - resetBtnWidth,
                canvas.y,
                resetBtnWidth,
                Text.LineHeight
            );
            var gAddPriceRect = new Rect(canvas.width - 5f - 200f - resetBtnWidth - 10f, 0f, 200f, Text.LineHeight);
            var gRemovePriceRect = new Rect(
                canvas.width - 5f - 200f - resetBtnWidth - 10f,
                Text.LineHeight,
                200f,
                Text.LineHeight
            );

            DrawGlobalAddPriceField(gAddPriceRect);
            DrawGlobalRemovePriceField(gRemovePriceRect);
            DrawGlobalResetButton(gResetRect);
            GUI.EndGroup();
        }

        private void DrawGlobalResetButton(Rect canvas)
        {
            if (!Widgets.ButtonText(canvas, resetAllText))
            {
                return;
            }

            foreach (TraitTableItem trait in worker.Data.Where(i => !i.IsHidden))
            {
                trait.Data.CanAdd = true;
                trait.Data.CostToAdd = 3500;
                trait.Data.CanRemove = true;
                trait.Data.CostToRemove = 5500;
                trait.Data.Name = trait.Data.GetDefaultName();
                trait.Data.Data.CustomName = false;
                trait.Data.Data.CanBypassLimit = false;
                trait.Data.Data.KarmaTypeForAdding = null;
                trait.Data.Data.KarmaTypeForRemoving = null;
            }
        }

        private void DrawGlobalRemovePriceField(Rect canvas)
        {
            var buffer = globalRemoveCost.ToString();
            Widgets.Label(canvas.LeftHalf(), removeCostText);
            Widgets.TextFieldNumeric(canvas.RightHalf(), ref globalRemoveCost, ref buffer);

            if (!SettingsHelper.DrawDoneButton(canvas.RightHalf()))
            {
                return;
            }

            foreach (TraitTableItem trait in worker.Data.Where(i => !i.IsHidden))
            {
                trait.Data.CostToRemove = globalRemoveCost;
            }
        }

        private void DrawGlobalAddPriceField(Rect canvas)
        {
            var buffer = globalAddCost.ToString();
            Widgets.Label(canvas.LeftHalf(), addCostText);
            Widgets.TextFieldNumeric(canvas.RightHalf(), ref globalAddCost, ref buffer);

            if (!SettingsHelper.DrawDoneButton(canvas.RightHalf()))
            {
                return;
            }

            foreach (TraitTableItem trait in worker.Data.Where(i => !i.IsHidden))
            {
                trait.Data.CostToAdd = globalAddCost;
            }
        }

        public override void DoWindowContents(Rect canvas)
        {
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            GUI.BeginGroup(canvas);

            var headerRect = new Rect(0f, 0f, canvas.width, Text.LineHeight * 2f);
            var contentArea = new Rect(
                canvas.x,
                Text.LineHeight * 4f,
                canvas.width,
                canvas.height - Text.LineHeight * 4f
            );

            DrawHeader(headerRect);
            Widgets.DrawLineHorizontal(canvas.x, Text.LineHeight * 3f, canvas.width);

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
                        () =>
                        {
                            switch (TkSettings.DumpStyle)
                            {
                                case "MultiFile":
                                    Data.SaveTraits(Paths.TraitFilePath);
                                    return;
                                case "SingleFile":
                                    Data.SaveLegacyShop(Paths.LegacyShopDumpFilePath);
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
                        Data.SaveTraits(Paths.TraitFilePath);
                        return;
                    case "SingleFile":
                        Data.SaveLegacyShop(Paths.LegacyShopDumpFilePath);
                        return;
                }
            }
        }

        public override void Notify_ResolutionChanged()
        {
            base.Notify_ResolutionChanged();
            shouldResizeTable = true;
        }
    }
}
