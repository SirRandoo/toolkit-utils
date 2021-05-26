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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public abstract class ItemWorkerBase<T, TU>
        where T : TableWorker<TableSettingsItem<TU>> where TU : class, IShopItemBase
    {
        private string addMutatorText;
        private Vector2 addMutatorTextSize;
        private string addSelectorText;
        private Vector2 addSelectorTextSize;
        private string applyText;
        private Vector2 applyTextSize;
        private Rect modifierRect = Rect.zero;
        protected List<FloatMenuOption> MutateAdders;
        private List<IMutatorBase<TU>> mutators;
        private Vector2 mutatorScrollPos = Vector2.zero;
        protected List<FloatMenuOption> SelectorAdders;
        private Rect selectorRect = Rect.zero;
        private List<ISelectorBase<TU>> selectors;
        private Vector2 selectorScrollPos = Vector2.zero;

        private Rect tableRect = Rect.zero;
        private protected T Worker;

        public IEnumerable<TableSettingsItem<TU>> Data => Worker.Data;

        public virtual void Prepare()
        {
            addMutatorText = "TKUtils.Buttons.AddMutator".Localize();
            addSelectorText = "TKUtils.Buttons.AddSelector".Localize();
            applyText = "TKUtils.Buttons.Apply".Localize();

            addSelectorTextSize = Text.CalcSize(addSelectorText);
            addMutatorTextSize = Text.CalcSize(addMutatorText);
            applyTextSize = Text.CalcSize(applyText);

            selectors = new List<ISelectorBase<TU>>();
            mutators = new List<IMutatorBase<TU>>();
        }

        public void Draw()
        {
            GUI.color = new Color(1f, 1f, 1f, 0.15f);
            Widgets.DrawLineVertical(modifierRect.x - 8f, modifierRect.y + 10f, modifierRect.height - 20f);
            GUI.color = Color.white;

            GUI.BeginGroup(selectorRect);
            DrawSelectorMenu(selectorRect.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(modifierRect);
            DrawMutatorMenu(modifierRect.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(tableRect);
            Worker?.Draw(tableRect.AtZero());
            GUI.EndGroup();
        }

        public void NotifyResolutionChanged(Rect canvas)
        {
            var userRect = new Rect(0f, 0f, canvas.width, Mathf.FloorToInt(canvas.height / 2f) - 4f);
            selectorRect = new Rect(0f, 0f, Mathf.FloorToInt(userRect.width / 2f) - 8f, userRect.height);
            modifierRect = new Rect(selectorRect.x + selectorRect.width + 16f, 0f, selectorRect.width, userRect.height);

            tableRect = new Rect(0f, userRect.height + 8f, canvas.width, canvas.height - userRect.height - 8f);
            Worker?.NotifyResolutionChanged(tableRect);
        }

        internal void NotifyGlobalDataChanged()
        {
            Worker.NotifyGlobalDataChanged();
        }

        private void DrawSelectorMenu(Rect canvas)
        {
            var addRect = new Rect(0f, 0f, addSelectorTextSize.x + 16f, Text.LineHeight);
            var selectorsRect = new Rect(0f, addRect.height, canvas.width, canvas.height - addRect.height);
            var viewPort = new Rect(0f, 0f, selectorsRect.width - 16f, selectors.Count * Text.LineHeight);

            if (Widgets.ButtonText(addRect, addSelectorText))
            {
                Find.WindowStack.Add(new FloatMenu(SelectorAdders));
            }

            selectorScrollPos = GUI.BeginScrollView(selectorsRect, selectorScrollPos, viewPort);
            ISelectorBase<TU> toRemove = null;
            for (var i = 0; i < selectors.Count; i++)
            {
                ISelectorBase<TU> selector = selectors[i];
                var lineRect = new Rect(0f, Text.LineHeight * i, selectorsRect.width, Text.LineHeight);

                if (!lineRect.IsRegionVisible(selectorsRect, selectorScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                Rect sRect = lineRect.WithWidth(lineRect.width - Text.LineHeight - 2f);
                selector.Draw(sRect);

                if (SettingsHelper.DrawFieldButton(lineRect, Widgets.CheckboxOffTex))
                {
                    toRemove = selector;
                }
            }

            GUI.EndScrollView();

            if (toRemove == null)
            {
                return;
            }

            selectors.Remove(toRemove);
            UpdateView(true);
        }

        private void DrawMutatorMenu(Rect canvas)
        {
            var addRect = new Rect(0f, 0f, addMutatorTextSize.x + 16f, Text.LineHeight);
            var applyRect = new Rect(
                canvas.width - (applyTextSize.x + 16f),
                0f,
                applyTextSize.x + 16f,
                Text.LineHeight
            );
            var mutatorsRect = new Rect(0f, addRect.height, canvas.width, canvas.height - addRect.height);
            var viewPort = new Rect(0f, 0f, mutatorsRect.width - 16f, mutators.Count * Text.LineHeight);

            if (Widgets.ButtonText(addRect, addMutatorText))
            {
                Find.WindowStack.Add(new FloatMenu(MutateAdders));
            }

            if (Widgets.ButtonText(applyRect, applyText))
            {
                ExecuteMutators();
            }

            mutatorScrollPos = GUI.BeginScrollView(mutatorsRect, mutatorScrollPos, viewPort);
            IMutatorBase<TU> toRemove = null;
            for (var i = 0; i < mutators.Count; i++)
            {
                IMutatorBase<TU> mutator = mutators[i];
                var lineRect = new Rect(0f, Text.LineHeight * i, mutatorsRect.width, Text.LineHeight);

                if (!lineRect.IsRegionVisible(mutatorsRect, mutatorScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                Rect mutatorRect = lineRect.WithWidth(lineRect.width - Text.LineHeight - 2f);
                mutator.Draw(mutatorRect);

                if (SettingsHelper.DrawFieldButton(lineRect, Widgets.CheckboxOffTex))
                {
                    toRemove = mutator;
                }
            }

            GUI.EndScrollView();

            if (toRemove == null)
            {
                return;
            }

            mutators.Remove(toRemove);
            UpdateView(true);
        }

        private void ExecuteMutators()
        {
            if (Worker == null)
            {
                return;
            }

            foreach (TableSettingsItem<TU> item in Worker.Data.Where(i => !i.IsHidden))
            {
                foreach (IMutatorBase<TU> mutator in mutators.OrderByDescending(m => m.Priority))
                {
                    mutator.Mutate(item);
                }
            }
        }

        protected void AddSelector([NotNull] ISelectorBase<TU> selector)
        {
            selector.Prepare();
            selector.Dirty ??= new ObservableProperty<bool>(false);
            selector.Dirty.Changed += UpdateView;
            selectors.Add(selector);
            UpdateView(true);
        }

        protected void AddMutator([NotNull] IMutatorBase<TU> mutator)
        {
            mutator.Prepare();
            mutators.Add(mutator);
        }

        private void UpdateView(bool state)
        {
            if (state == false)
            {
                return;
            }

            Worker?.NotifyCustomSearchRequested(item => selectors.All(i => i.IsVisible(item)));

            foreach (ISelectorBase<TU> selector in selectors)
            {
                selector.Dirty.Set(false);
            }
        }
    }
}
