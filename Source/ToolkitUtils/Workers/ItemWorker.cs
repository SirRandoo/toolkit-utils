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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Models.Tables;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class ItemWorker : ItemWorkerBase
    {
        private string addMutatorText;
        private Vector2 addMutatorTextSize;
        private string addSelectorText;
        private Vector2 addSelectorTextSize;
        private string applyText;
        private Vector2 applyTextSize;
        private List<FloatMenuOption> mutateAdders;
        private List<IMutatorBase<ThingItem>> mutators;
        private Vector2 mutatorScrollPos = Vector2.zero;
        private List<FloatMenuOption> selectorAdders;
        private List<ISelectorBase<ThingItem>> selectors;
        private Vector2 selectorScrollPos = Vector2.zero;

        public override void Prepare()
        {
            Worker.Prepare();

            selectors = new List<ISelectorBase<ThingItem>>();
            mutators = new List<IMutatorBase<ThingItem>>();

            selectorAdders = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.Fields.Price".Localize(),
                    () => AddSelector(new PriceSelector<ThingItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Name".Localize(),
                    () => AddSelector(new NameSelector<ThingItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.DefName".Localize(),
                    () => AddSelector(new DefNameSelector<ThingItem>())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Mod".Localize(),
                    () => AddSelector(new ModSelector<ThingItem>())
                ),
                new FloatMenuOption("TKUtils.Fields.IsWeapon".Localize(), () => AddSelector(new WeaponSelector())),
                new FloatMenuOption(
                    "TKUtils.Fields.IsMeleeWeapon".Localize(),
                    () => AddSelector(new MeleeWeaponSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.IsRangedWeapon".Localize(),
                    () => AddSelector(new RangedWeaponSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.HasQuantityLimit".Localize(),
                    () => AddSelector(new HasQuantityLimitSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.QuantityLimit".Localize(),
                    () => AddSelector(new QuantityLimitSelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.Category".Localize(),
                    () => AddSelector(new CategorySelector())
                ),
                new FloatMenuOption(
                    "TKUtils.Fields.State".Localize(),
                    () => AddSelector(new StateSelector<ThingItem>())
                ),
                new FloatMenuOption("TKUtils.Fields.CanBeStuff".Localize(), () => AddSelector(new StuffSelector())),
                new FloatMenuOption("TKUtils.Fields.Weight".Localize(), () => AddSelector(new WeightSelector())),
                new FloatMenuOption(
                    "TKUtils.Fields.CanStack".Localize(),
                    () => AddSelector(new StackabilitySelector())
                )
            };

            mutateAdders = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.Fields.HasQuantityLimit".Localize(),
                    () => AddMutator(new HasQuantityLimitMutator())
                ),
                new FloatMenuOption("TKUtils.Fields.Name".Localize(), () => AddMutator(new ItemNameMutator())),
                new FloatMenuOption("TKUtils.Fields.Price".Localize(), () => AddMutator(new ItemPriceMutator())),
                new FloatMenuOption(
                    "TKUtils.Fields.QuantityLimit".Localize(),
                    () => AddMutator(new QuantityLimitMutator())
                ),
                new FloatMenuOption("TKUtils.Fields.State".Localize(), () => AddMutator(new ItemStateMutator())),
                new FloatMenuOption("TKUtils.Fields.CanBeStuff".Localize(), () => AddMutator(new StuffMutator())),
                new FloatMenuOption("TKUtils.Fields.Weight".Localize(), () => AddMutator(new WeightMutator()))
            };

            LoadTranslations();
        }

        private void LoadTranslations()
        {
            addMutatorText = "TKUtils.Buttons.AddMutator".Localize();
            addSelectorText = "TKUtils.Buttons.AddSelector".Localize();
            applyText = "TKUtils.Buttons.Apply".Localize();

            addSelectorTextSize = Text.CalcSize(addSelectorText);
            addMutatorTextSize = Text.CalcSize(addMutatorText);
            applyTextSize = Text.CalcSize(applyText);
        }

        public override void DrawSelectorMenu(Rect canvas)
        {
            var addRect = new Rect(0f, 0f, addSelectorTextSize.x + 16f, Text.LineHeight);
            var selectorsRect = new Rect(0f, addRect.height, canvas.width, canvas.height - addRect.height);
            var viewPort = new Rect(0f, 0f, selectorsRect.width - 16f, selectors.Count * Text.LineHeight);

            if (Widgets.ButtonText(addRect, addSelectorText))
            {
                Find.WindowStack.Add(new FloatMenu(selectorAdders));
            }

            selectorScrollPos = GUI.BeginScrollView(selectorsRect, selectorScrollPos, viewPort);
            ISelectorBase<ThingItem> toRemove = null;
            for (var i = 0; i < selectors.Count; i++)
            {
                ISelectorBase<ThingItem> selector = selectors[i];
                var lineRect = new Rect(0f, Text.LineHeight * i, selectorsRect.width, Text.LineHeight);

                if (!lineRect.IsRegionVisible(selectorsRect, selectorScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                Rect selectorRect = lineRect.WithWidth(lineRect.width - Text.LineHeight - 2f);
                selector.Draw(selectorRect);

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

        public override void DrawMutatorMenu(Rect canvas)
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
                Find.WindowStack.Add(new FloatMenu(mutateAdders));
            }

            if (Widgets.ButtonText(applyRect, applyText))
            {
                ExecuteMutators();
            }

            mutatorScrollPos = GUI.BeginScrollView(mutatorsRect, mutatorScrollPos, viewPort);
            IMutatorBase<ThingItem> toRemove = null;
            for (var i = 0; i < mutators.Count; i++)
            {
                IMutatorBase<ThingItem> mutator = mutators[i];
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
            foreach (ItemTableItem item in Worker.Data)
            {
                foreach (IMutatorBase<ThingItem> mutator in mutators)
                {
                    mutator.Mutate(item);
                }
            }
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            base.NotifyResolutionChanged(canvas);
            Worker.NotifyResolutionChanged(TableRect);
        }

        public override void NotifyWindowUpdate() { }

        protected void UpdateView(bool state)
        {
            if (state == false)
            {
                return;
            }

            Worker.NotifyCustomSearchRequested(item => selectors.All(i => i.IsVisible(item)));

            foreach (ISelectorBase<ThingItem> selector in selectors)
            {
                selector.Dirty.Set(false);
            }
        }

        public void AddSelector(ISelectorBase<ThingItem> selector)
        {
            selector.Prepare();
            selector.Dirty ??= new ObservableProperty<bool>(false);
            selector.Dirty.Changed += UpdateView;
            selectors.Add(selector);
            UpdateView(true);
        }

        public void AddMutator(IMutatorBase<ThingItem> mutator)
        {
            mutator.Prepare();
            mutators.Add(mutator);
        }
    }
}
