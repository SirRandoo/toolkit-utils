﻿using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public enum FieldTypes { Script, Calculation, Setter }

    public class Editor : Window
    {
        private ScriptEngine engine;

        private bool maximized;
        private TabWorker tabWorker;

        private string title;
        private float titleWidth;

        public Editor()
        {
            doCloseButton = false;
            forcePause = true;
            draggable = true;
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
            engine = ScriptEngine.CreateInstance("tkutils.editor");
            InitializeTabs();
            ProcessTranslations();
        }

        private void ProcessTranslations()
        {
            title = "TKUtils.Editor.Title".Localize();
            titleWidth = Text.CalcSize(title).x;
        }

        private void InitializeTabs()
        {
            tabWorker.SelectedTab =
                new TabItem {Label = "TKUtils.EditorTabs.Items".Localize(), ContentDrawer = DrawItemsTab};

            tabWorker.AddTab(tabWorker.SelectedTab);
            tabWorker.AddTab(
                new TabItem {Label = "TKUtils.EditorTabs.Events".Localize(), ContentDrawer = DrawEventsTab}
            );
            tabWorker.AddTab(
                new TabItem {Label = "TKUtils.EditorTabs.Traits".Localize(), ContentDrawer = DrawTraitsTab}
            );
            tabWorker.AddTab(
                new TabItem {Label = "TKUtils.EditorTabs.PawnKinds".Localize(), ContentDrawer = DrawPawnKindsTab}
            );
        }

        private void DrawTraitsTab(Rect obj) { }

        private void DrawPawnKindsTab(Rect obj) { }

        private void DrawEventsTab(Rect obj) { }

        private void DrawItemsTab(Rect obj) { }

        public override void DoWindowContents(Rect canvas)
        {
            Rect tabRect = new Rect(0f, 0f, canvas.width, Text.LineHeight * 2f).Rounded();
            var contentRect = new Rect(0f, tabRect.height, canvas.width, canvas.height - tabRect.height);

            GUI.BeginGroup(canvas);
            tabWorker.Draw(tabRect);
            DrawWindowDecorations(tabRect);


            GUI.BeginGroup(contentRect);
            tabWorker.SelectedTab?.Draw(new Rect(0f, 0f, contentRect.width, contentRect.height));
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

        private void DrawEditorMenu() { }
    }
}