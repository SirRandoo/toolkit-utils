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

using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public enum FieldTypes { Script, Calculation, Setter }

    public class Editor : Window
    {
        private readonly ItemWorker itemWorker;
        private readonly PawnWorker pawnWorker;
        private readonly TraitWorker traitWorker;
        private bool dirty = true;
        private ScriptEngine engine;

        private bool maximized;
        private TabWorker tabWorker;

        public Editor()
        {
            doCloseButton = false;
            forcePause = true;
            draggable = true;

            itemWorker = new ItemWorker();
            traitWorker = new TraitWorker();
            pawnWorker = new PawnWorker();
        }

        protected override float Margin => 0f;

        public override Vector2 InitialSize =>
            new Vector2(
                maximized ? UI.screenWidth : Mathf.Min(UI.screenWidth, 800f),
                maximized ? UI.screenHeight : Mathf.FloorToInt(UI.screenHeight * 0.8f)
            );

        public override void PreOpen()
        {
            base.PreOpen();

            tabWorker = TabWorker.CreateInstance();
            // ReSharper disable once StringLiteralTypo
            engine = ScriptEngine.CreateInstance("tkutils.editor");
            InitializeTabs();
            itemWorker.Prepare();
            traitWorker.Prepare();
            pawnWorker.Prepare();
        }

        private void InitializeTabs()
        {
            tabWorker.SelectedTab =
                new TabItem {Label = "TKUtils.EditorTabs.Items".Localize(), ContentDrawer = DrawItemsTab};

            tabWorker.AddTab(tabWorker.SelectedTab);
            tabWorker.AddTab(
                new TabItem {Label = "TKUtils.EditorTabs.Traits".Localize(), ContentDrawer = DrawTraitsTab}
            );
            tabWorker.AddTab(
                new TabItem {Label = "TKUtils.EditorTabs.PawnKinds".Localize(), ContentDrawer = DrawPawnKindsTab}
            );
        }

        private void DrawTraitsTab(Rect obj) { }

        private void DrawPawnKindsTab(Rect obj) { }

        private void DrawItemsTab(Rect obj)
        {
            itemWorker.Draw();
        }

        public override void DoWindowContents(Rect canvas)
        {
            Rect tabRect = new Rect(0f, 0f, canvas.width, Text.LineHeight * 2f).Rounded();
            var contentRect = new Rect(0f, tabRect.height, canvas.width, canvas.height - tabRect.height);

            GUI.BeginGroup(canvas);
            tabWorker.Draw(tabRect);
            DrawWindowDecorations(tabRect);


            GUI.BeginGroup(contentRect);
            var innerRect = new Rect(0f, 0f, contentRect.width, contentRect.height);

            if (dirty)
            {
                Rect innerResolution = innerRect.ContractedBy(16f);
                itemWorker.NotifyResolutionChanged(innerResolution);
                traitWorker.NotifyResolutionChanged(innerResolution);
                pawnWorker.NotifyResolutionChanged(innerResolution);
                dirty = false;
            }

            tabWorker.SelectedTab?.Draw(innerRect);
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawWindowDecorations(Rect tabRect)
        {
            var butRect = new Rect(
                tabRect.x + tabRect.width - tabRect.height + 14f,
                14f,
                tabRect.height - 28f,
                tabRect.height - 28f
            );

            if (Widgets.ButtonImage(butRect, Textures.CloseButton))
            {
                Close();
            }

            butRect = butRect.ShiftLeft();
            if (Widgets.ButtonImage(butRect, maximized ? Textures.RestoreWindow : Textures.MaximizeWindow))
            {
                maximized = !maximized;
                SetInitialSizeAndPosition();
            }

            if (Widgets.ButtonImage(butRect.ShiftLeft(), Textures.QuestionMark))
            {
                Application.OpenURL("https://sirrandoo.github.io/toolkit-utils/editor");
            }
        }

        public override void PreClose()
        {
            base.PreClose();

            Store_ItemEditor.UpdateStoreItemList();
        }

        public override void Notify_ResolutionChanged()
        {
            base.Notify_ResolutionChanged();
            dirty = true;
        }
    }
}
