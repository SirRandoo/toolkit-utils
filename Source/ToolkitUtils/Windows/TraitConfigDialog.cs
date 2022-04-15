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
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class TraitConfigDialog : Window
    {
        private readonly TraitTableWorker _worker;
        private string _addCostText;
        private int _globalAddCost;
        private int _globalRemoveCost;
        private float _lastSearchTick;

        private string _query = "";
        private string _removeCostText;
        private string _resetAllText;
        private Vector2 _resetAllTextSize;
        private string _searchText;
        private bool _shouldResizeTable = true;
        private string _titleText;

        public TraitConfigDialog()
        {
            doCloseX = true;
            _worker = new TraitTableWorker();
        }

        public override Vector2 InitialSize => new Vector2(900f, UI.screenHeight * 0.9f);
        protected override float Margin => 22f;

        private void NotifySearchRequested()
        {
            _lastSearchTick = 10f;
        }

        public override void PreOpen()
        {
            base.PreOpen();
            GetTranslations();
            _worker.Prepare();
            optionalTitle = _titleText;
        }

        private void GetTranslations()
        {
            _titleText = "TKUtils.TraitStore.Title".TranslateSimple();
            _searchText = "TKUtils.Buttons.Search".TranslateSimple();
            _addCostText = "TKUtils.Fields.AddPrice".TranslateSimple();
            _removeCostText = "TKUtils.Fields.RemovePrice".TranslateSimple();
            _resetAllText = "TKUtils.Buttons.ResetAll".TranslateSimple();

            _resetAllTextSize = Text.CalcSize(_resetAllText);
        }

        private void DrawHeader(Rect canvas)
        {
            GUI.BeginGroup(canvas);
            (Rect searchLabel, Rect searchField) = new Rect(canvas.x, canvas.y, canvas.width * 0.19f, Text.LineHeight).Split(0.3f);

            Widgets.Label(searchLabel, _searchText);

            if (UiHelper.TextField(searchField, _query, out string input))
            {
                _query = input;
                NotifySearchRequested();
            }

            if (_query.Length > 0 && UiHelper.ClearButton(searchField))
            {
                _query = "";
                NotifySearchRequested();
            }


            float resetBtnWidth = _resetAllTextSize.x + 16f;
            var gResetRect = new Rect(canvas.x + canvas.width - resetBtnWidth, canvas.y, resetBtnWidth, Text.LineHeight);
            var gAddPriceRect = new Rect(canvas.width - 5f - 200f - resetBtnWidth - 10f, 0f, 200f, Text.LineHeight);
            var gRemovePriceRect = new Rect(canvas.width - 5f - 200f - resetBtnWidth - 10f, Text.LineHeight, 200f, Text.LineHeight);

            DrawGlobalAddPriceField(gAddPriceRect);
            DrawGlobalRemovePriceField(gRemovePriceRect);
            DrawGlobalResetButton(gResetRect);
            GUI.EndGroup();
        }

        private void DrawGlobalResetButton(Rect canvas)
        {
            if (Widgets.ButtonText(canvas, _resetAllText))
            {
                ConfirmationDialog.Open("TKUtils.TraitStore.ConfirmReset".TranslateSimple(), PerformGlobalReset);
            }
        }
        private void PerformGlobalReset()
        {
            foreach (TableSettingsItem<TraitItem> trait in _worker.Data.Where(i => !i.IsHidden))
            {
                trait.Data.CanAdd = true;
                trait.Data.CostToAdd = 3500;
                trait.Data.CanRemove = true;
                trait.Data.CostToRemove = 5500;
                trait.Data.Name = trait.Data.GetDefaultName();
                trait.Data.TraitData!.CustomName = false;
                trait.Data.TraitData.CanBypassLimit = false;
                trait.Data.Data.KarmaType = null;
                trait.Data.TraitData.KarmaTypeForRemoving = null;
            }
        }

        private void DrawGlobalRemovePriceField(Rect canvas)
        {
            var buffer = _globalRemoveCost.ToString();
            Widgets.Label(canvas.LeftHalf(), _removeCostText);
            Widgets.TextFieldNumeric(canvas.RightHalf(), ref _globalRemoveCost, ref buffer);

            if (!UiHelper.DoneButton(canvas.RightHalf()))
            {
                return;
            }

            foreach (TableSettingsItem<TraitItem> trait in _worker.Data.Where(i => !i.IsHidden))
            {
                trait.Data.CostToRemove = _globalRemoveCost;
            }
        }

        private void DrawGlobalAddPriceField(Rect canvas)
        {
            var buffer = _globalAddCost.ToString();
            Widgets.Label(canvas.LeftHalf(), _addCostText);
            Widgets.TextFieldNumeric(canvas.RightHalf(), ref _globalAddCost, ref buffer);

            if (!UiHelper.DoneButton(canvas.RightHalf()))
            {
                return;
            }

            foreach (TableSettingsItem<TraitItem> trait in _worker.Data.Where(i => !i.IsHidden))
            {
                trait.Data.CostToAdd = _globalAddCost;
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
            var contentArea = new Rect(canvas.x, Text.LineHeight * 4f, canvas.width, canvas.height - Text.LineHeight * 4f);

            DrawHeader(headerRect);
            Widgets.DrawLineHorizontal(canvas.x, Text.LineHeight * 3f, canvas.width);

            bool wrapped = Text.WordWrap;
            Text.WordWrap = false;

            GUI.BeginGroup(contentArea);

            if (_shouldResizeTable)
            {
                _worker.NotifyResolutionChanged(contentArea.AtZero());
                _shouldResizeTable = false;
            }

            _worker.Draw(contentArea.AtZero());
            GUI.EndGroup();
            GUI.EndGroup();

            Text.WordWrap = wrapped;
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();


            if (_lastSearchTick <= 0)
            {
                _worker.NotifySearchRequested(_query);
            }
            else
            {
                _lastSearchTick -= Time.unscaledTime - _lastSearchTick;
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
                                    await Data.SaveTraitsAsync(Paths.TraitFilePath);

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
            _shouldResizeTable = true;
        }
    }
}
