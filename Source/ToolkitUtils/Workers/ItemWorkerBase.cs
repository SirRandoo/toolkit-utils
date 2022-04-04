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
using System.Collections.Generic;
using System.Linq;
using CommonLib.Helpers;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public abstract class ItemWorkerBase<T, TU> where T : TableWorker<TableSettingsItem<TU>> where TU : class, IShopItemBase
    {
        private string _addMutatorText;
        private Vector2 _addMutatorTextSize;
        private string _addSelectorText;
        private Vector2 _addSelectorTextSize;
        private string _applyText;
        private Vector2 _applyTextSize;
        private Rect _modifierRect = Rect.zero;
        private List<FloatMenuOption> _mutateAdders;
        private List<IMutatorBase<TU>> _mutators;
        private Vector2 _mutatorScrollPos = Vector2.zero;
        private List<FloatMenuOption> _selectorAdders;
        private Rect _selectorRect = Rect.zero;
        private List<ISelectorBase<TU>> _selectors;
        private Vector2 _selectorScrollPos = Vector2.zero;

        private Rect _tableRect = Rect.zero;
        private protected T Worker;

        public IEnumerable<TableSettingsItem<TU>> Data => Worker.Data;

        public virtual void Prepare()
        {
            _addMutatorText = "TKUtils.Buttons.AddMutator".Localize();
            _addSelectorText = "TKUtils.Buttons.AddSelector".Localize();
            _applyText = "TKUtils.Buttons.Apply".Localize();

            _addSelectorTextSize = Text.CalcSize(_addSelectorText);
            _addMutatorTextSize = Text.CalcSize(_addMutatorText);
            _applyTextSize = Text.CalcSize(_applyText);

            _selectors = new List<ISelectorBase<TU>>();
            _mutators = new List<IMutatorBase<TU>>();
            _mutateAdders = new List<FloatMenuOption>();
            _selectorAdders = new List<FloatMenuOption>();
        }

        public void Draw()
        {
            GUI.color = new Color(1f, 1f, 1f, 0.15f);
            Widgets.DrawLineVertical(_modifierRect.x - 8f, _modifierRect.y + 10f, _modifierRect.height - 20f);
            GUI.color = Color.white;

            GUI.BeginGroup(_selectorRect);
            DrawSelectorMenu(_selectorRect.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(_modifierRect);
            DrawMutatorMenu(_modifierRect.AtZero());
            GUI.EndGroup();

            GUI.BeginGroup(_tableRect);
            Worker?.Draw(_tableRect.AtZero());
            GUI.EndGroup();
        }

        public void NotifyResolutionChanged(Rect canvas)
        {
            var userRect = new Rect(0f, 0f, canvas.width, Mathf.FloorToInt(canvas.height / 2f) - 4f);
            _selectorRect = new Rect(0f, 0f, Mathf.FloorToInt(userRect.width / 2f) - 8f, userRect.height);
            _modifierRect = new Rect(_selectorRect.x + _selectorRect.width + 16f, 0f, _selectorRect.width, userRect.height);

            _tableRect = new Rect(0f, userRect.height + 8f, canvas.width, canvas.height - userRect.height - 8f);
            Worker?.NotifyResolutionChanged(_tableRect);
        }

        internal void NotifyGlobalDataChanged()
        {
            Worker.NotifyGlobalDataChanged();
        }

        private void DrawSelectorMenu(Rect canvas)
        {
            var addRect = new Rect(0f, 0f, _addSelectorTextSize.x + 16f, Text.LineHeight);
            var selectorsRect = new Rect(0f, addRect.height, canvas.width, canvas.height - addRect.height);
            var viewPort = new Rect(0f, 0f, selectorsRect.width - 16f, _selectors.Count * Text.LineHeight);

            if (Widgets.ButtonText(addRect, _addSelectorText))
            {
                Find.WindowStack.Add(new FloatMenu(_selectorAdders));
            }

            _selectorScrollPos = GUI.BeginScrollView(selectorsRect, _selectorScrollPos, viewPort);
            ISelectorBase<TU> toRemove = null;

            for (var i = 0; i < _selectors.Count; i++)
            {
                ISelectorBase<TU> selector = _selectors[i];
                var lineRect = new Rect(0f, Text.LineHeight * i, selectorsRect.width, Text.LineHeight);

                if (!lineRect.IsVisible(selectorsRect, _selectorScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                Rect sRect = lineRect.Trim(Direction8Way.East, lineRect.width - Text.LineHeight - 2f);
                selector.Draw(sRect);

                if (UiHelper.FieldButton(lineRect, Widgets.CheckboxOffTex))
                {
                    toRemove = selector;
                }
            }

            GUI.EndScrollView();

            if (toRemove == null)
            {
                return;
            }

            _selectors.Remove(toRemove);
            UpdateView(true);
        }

        private void DrawMutatorMenu(Rect canvas)
        {
            var addRect = new Rect(0f, 0f, _addMutatorTextSize.x + 16f, Text.LineHeight);
            var applyRect = new Rect(canvas.width - (_applyTextSize.x + 16f), 0f, _applyTextSize.x + 16f, Text.LineHeight);
            var mutatorsRect = new Rect(0f, addRect.height, canvas.width, canvas.height - addRect.height);
            var viewPort = new Rect(0f, 0f, mutatorsRect.width - 16f, _mutators.Count * Text.LineHeight);

            if (Widgets.ButtonText(addRect, _addMutatorText))
            {
                Find.WindowStack.Add(new FloatMenu(_mutateAdders));
            }

            if (Widgets.ButtonText(applyRect, _applyText))
            {
                ExecuteMutators();
            }

            _mutatorScrollPos = GUI.BeginScrollView(mutatorsRect, _mutatorScrollPos, viewPort);
            IMutatorBase<TU> toRemove = null;

            for (var i = 0; i < _mutators.Count; i++)
            {
                IMutatorBase<TU> mutator = _mutators[i];
                var lineRect = new Rect(0f, Text.LineHeight * i, mutatorsRect.width, Text.LineHeight);

                if (!lineRect.IsVisible(mutatorsRect, _mutatorScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                Rect mutatorRect = lineRect.Trim(Direction8Way.East, lineRect.width - Text.LineHeight - 2f);
                mutator.Draw(mutatorRect);

                if (UiHelper.FieldButton(lineRect, Widgets.CheckboxOffTex))
                {
                    toRemove = mutator;
                }
            }

            GUI.EndScrollView();

            if (toRemove == null)
            {
                return;
            }

            _mutators.Remove(toRemove);
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
                foreach (IMutatorBase<TU> mutator in _mutators.OrderByDescending(m => m.Priority))
                {
                    mutator.Mutate(item);
                }
            }
        }

        private void AddSelector([NotNull] ISelectorBase<TU> selector)
        {
            selector.Prepare();
            selector.Dirty ??= new ObservableProperty<bool>(false);
            selector.Dirty.Changed += UpdateView;
            _selectors.Add(selector);
            UpdateView(true);
        }

        private void AddMutator([NotNull] IMutatorBase<TU> mutator)
        {
            mutator.Prepare();
            _mutators.Add(mutator);
        }

        private protected void DiscoverMutators(DomainIndexer.EditorTarget target)
        {
            foreach (DomainIndexer.MutatorEntry entry in DomainIndexer.Mutators)
            {
                if (entry.Target != target)
                {
                    continue;
                }

                IMutatorBase<TU> mutator = CreateMutator(entry);

                if (mutator == null)
                {
                    continue;
                }

                _mutateAdders.Add(new FloatMenuOption(mutator.Label, () => AddMutator(mutator)));
            }
        }

        private protected void DiscoverSelectors(DomainIndexer.EditorTarget target)
        {
            foreach (DomainIndexer.SelectorEntry entry in DomainIndexer.Selectors)
            {
                if (entry.Target != target)
                {
                    continue;
                }

                ISelectorBase<TU> selector = CreateSelector(entry);

                if (selector == null)
                {
                    continue;
                }

                _selectorAdders.Add(new FloatMenuOption(selector.Label, () => AddSelector(selector)));
            }
        }

        [CanBeNull]
        private static IMutatorBase<TU> CreateMutator([NotNull] DomainIndexer.MutatorEntry entry) =>
            Activator.CreateInstance(entry.Type.GetGenericArguments().Length > 0 ? entry.Type.MakeGenericType(typeof(TU)) : entry.Type) as IMutatorBase<TU>;

        [CanBeNull]
        private static ISelectorBase<TU> CreateSelector([NotNull] DomainIndexer.SelectorEntry entry) =>
            Activator.CreateInstance(entry.Type.GetGenericArguments().Length > 0 ? entry.Type.MakeGenericType(typeof(TU)) : entry.Type) as ISelectorBase<TU>;

        private void UpdateView(bool state)
        {
            if (!state)
            {
                return;
            }

            Worker?.NotifyCustomSearchRequested(item => _selectors.All(i => i.IsVisible(item)));

            foreach (ISelectorBase<TU> selector in _selectors)
            {
                selector.Dirty.Set(false);
            }
        }
    }
}
