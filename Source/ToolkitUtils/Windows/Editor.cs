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

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using TwitchToolkit;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    /// <summary>
    ///     A window for editing Twitch Toolkit's store in bulk.
    /// </summary>
    public class Editor : Window
    {
        private readonly EventWorker _eventWorker;
        private readonly ItemWorker _itemWorker;
        private readonly PawnWorker _pawnWorker;
        private readonly TraitWorker _traitWorker;
        private bool _dirty = true;

        private TabItem _eventTab;
        private string _exportPartialTooltip;
        private string _helpTooltip;

        private string _importPartialTooltip;
        private TabItem _itemTab;

        private bool _maximized;
        private TabItem _pawnTab;
        private TabWorker _tabWorker;
        private TabItem _traitTab;

        public Editor()
        {
            forcePause = true;
            doCloseButton = false;

            _itemWorker = new ItemWorker();
            _pawnWorker = new PawnWorker();
            _eventWorker = new EventWorker();
            _traitWorker = new TraitWorker();
        }

        protected override float Margin => 0f;

        /// <inheritdoc cref="Window.InitialSize"/>
        public override Vector2 InitialSize => new Vector2(
            _maximized ? UI.screenWidth : Mathf.Min(UI.screenWidth, 800f),
            _maximized ? UI.screenHeight : Mathf.FloorToInt(UI.screenHeight * 0.8f)
        );

        /// <inheritdoc cref="Window.PreOpen"/>
        public override void PreOpen()
        {
            base.PreOpen();

            _tabWorker = TabWorker.CreateInstance();
            InitializeTabs();
            _itemWorker.Prepare();
            _pawnWorker.Prepare();
            _eventWorker.Prepare();
            _traitWorker.Prepare();

            FetchTranslations();
        }

        private void FetchTranslations()
        {
            _importPartialTooltip = "TKUtils.EditorTooltips.ImportPartial".TranslateSimple();
            _exportPartialTooltip = "TKUtils.EditorTooltips.ExportPartial".TranslateSimple();
            _helpTooltip = "TKUtils.EditorTooltips.GetHelp".TranslateSimple();
        }

        private void InitializeTabs()
        {
            _itemTab ??= new TabItem { Label = "TKUtils.EditorTabs.Items".TranslateSimple(), ContentDrawer = rect => _itemWorker.Draw() };

            _pawnTab ??= new TabItem { Label = "TKUtils.EditorTabs.PawnKinds".TranslateSimple(), ContentDrawer = rect => _pawnWorker.Draw() };

            _eventTab ??= new TabItem { Label = "TKUtils.EditorTabs.Events".TranslateSimple(), ContentDrawer = rect => _eventWorker.Draw() };

            _traitTab ??= new TabItem { Label = "TKUtils.EditorTabs.Traits".TranslateSimple(), ContentDrawer = rect => _traitWorker.Draw() };


            _tabWorker.AddTab(_itemTab);
            _tabWorker.AddTab(_eventTab);
            _tabWorker.AddTab(_traitTab);
            _tabWorker.AddTab(_pawnTab);
        }

        /// <inheritdoc cref="Window.DoWindowContents"/>
        public override void DoWindowContents(Rect canvas)
        {
            Rect tabRect = new Rect(0f, 0f, canvas.width, Text.LineHeight * 2f).Rounded();
            var contentRect = new Rect(0f, tabRect.height, canvas.width, canvas.height - tabRect.height);

            GUI.BeginGroup(canvas);
            _tabWorker.Draw(tabRect);
            DrawWindowDecorations(tabRect);


            GUI.BeginGroup(contentRect);
            var innerRect = new Rect(0f, 0f, contentRect.width, contentRect.height);

            if (_dirty)
            {
                Rect innerResolution = innerRect.ContractedBy(16f);
                _itemWorker.NotifyResolutionChanged(innerResolution);
                _traitWorker.NotifyResolutionChanged(innerResolution);
                _pawnWorker.NotifyResolutionChanged(innerResolution);
                _eventWorker.NotifyResolutionChanged(innerResolution);
                _dirty = false;
            }

            _tabWorker.SelectedTab?.Draw(innerRect);
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawWindowDecorations(Rect tabRect)
        {
            var butRect = new Rect(tabRect.x + tabRect.width - tabRect.height + 14f, 14f, tabRect.height - 28f, tabRect.height - 28f);

            if (Widgets.ButtonImage(butRect, Textures.CloseButton))
            {
                Close();
            }

            butRect = butRect.Shift(Direction8Way.West);

            if (Widgets.ButtonImage(butRect, _maximized ? Textures.RestoreWindow : Textures.MaximizeWindow))
            {
                _maximized = !_maximized;
                SetInitialSizeAndPosition();
            }

            butRect = butRect.Shift(Direction8Way.West);

            if (Widgets.ButtonImage(butRect, Textures.QuestionMark))
            {
                Application.OpenURL("https://sirrandoo.github.io/toolkit-utils/editor");
            }

            butRect.TipRegion(_helpTooltip);

            butRect = butRect.Shift(Direction8Way.West);

            if (Widgets.ButtonImage(butRect, Textures.CopySettings))
            {
                SavePartial();
            }

            butRect.TipRegion(_exportPartialTooltip);

            butRect = butRect.Shift(Direction8Way.West);

            if (Widgets.ButtonImage(butRect, Textures.PasteSettings))
            {
                LoadPartial();
            }

            butRect.TipRegion(_importPartialTooltip);
        }

        private void LoadPartial()
        {
            if (_tabWorker.SelectedTab == _itemTab)
            {
                Find.WindowStack.Add(PartialManager<ItemPartial>.CreateLoadInstance(LoadPartialItemData));
            }
            else if (_tabWorker.SelectedTab == _eventTab)
            {
                Find.WindowStack.Add(PartialManager<EventPartial>.CreateLoadInstance(LoadPartialEventData));
            }
            else if (_tabWorker.SelectedTab == _pawnTab)
            {
                Find.WindowStack.Add(PartialManager<PawnKindItem>.CreateLoadInstance(LoadPartialPawnData));
            }
            else if (_tabWorker.SelectedTab == _traitTab)
            {
                Find.WindowStack.Add(PartialManager<TraitItem>.CreateLoadInstance(LoadPartialTraitData));
            }
        }

        private void LoadPartialItemData([NotNull] PartialData<ItemPartial> data)
        {
            Data.LoadItemPartial(data.Data);
            _itemWorker.NotifyGlobalDataChanged();
        }

        private void LoadPartialEventData([NotNull] PartialData<EventPartial> data)
        {
            Data.LoadEventPartial(data.Data);
            _eventWorker.NotifyGlobalDataChanged();
        }

        private void LoadPartialTraitData([NotNull] PartialData<TraitItem> data)
        {
            Data.LoadTraitPartial(data.Data);
            _traitWorker.NotifyGlobalDataChanged();
        }

        private void LoadPartialPawnData([NotNull] PartialData<PawnKindItem> data)
        {
            Data.LoadPawnPartial(data.Data);
            _pawnWorker.NotifyGlobalDataChanged();
        }

        private void SavePartial()
        {
            if (_tabWorker.SelectedTab == _itemTab)
            {
                Find.WindowStack.Add(PartialManager<ItemPartial>.CreateSaveInstance(SaveItemPartial));
            }
            else if (_tabWorker.SelectedTab == _eventTab)
            {
                Find.WindowStack.Add(PartialManager<EventPartial>.CreateSaveInstance(SaveEventPartial));
            }
            else if (_tabWorker.SelectedTab == _traitTab)
            {
                Find.WindowStack.Add(PartialManager<TraitItem>.CreateSaveInstance(SaveTraitPartial));
            }
            else if (_tabWorker.SelectedTab == _pawnTab)
            {
                Find.WindowStack.Add(PartialManager<PawnKindItem>.CreateSaveInstance(SavePawnPartial));
            }
        }

        /// <inheritdoc cref="Window.PreClose"/>
        public override void PreClose()
        {
            base.PreClose();

            Store_ItemEditor.UpdateStoreItemList();
            Store_IncidentEditor.UpdatePriceSheet();
            Toolkit.Mod.WriteSettings();

            Task.Run(
                    async () =>
                    {
                        switch (TkSettings.DumpStyle)
                        {
                            case "SingleFile":
                                await Data.SaveLegacyShopAsync(Paths.LegacyShopDumpFilePath);

                                return;
                            case "MultiFile":
                                await Data.SaveTraitsAsync(Paths.TraitFilePath);
                                await Data.SavePawnKindsAsync(Paths.PawnKindFilePath);

                                return;
                        }

                        await Data.SaveItemDataAsync(Paths.ItemDataFilePath);
                        await Data.SaveEventDataAsync(Paths.EventDataFilePath);
                    }
                )
               .ConfigureAwait(false);
        }

        protected override void SetInitialSizeAndPosition()
        {
            base.SetInitialSizeAndPosition();

            Rect tabRect = new Rect(0f, 0f, windowRect.width, Text.LineHeight * 2f).Rounded();
            Rect contentRect = new Rect(0f, tabRect.height, windowRect.width, windowRect.height - tabRect.height).ContractedBy(16f);

            _itemWorker.NotifyResolutionChanged(contentRect);
            _traitWorker.NotifyResolutionChanged(contentRect);
            _pawnWorker.NotifyResolutionChanged(contentRect);
            _eventWorker.NotifyResolutionChanged(contentRect);
        }

        /// <inheritdoc cref="Window.Notify_ResolutionChanged"/>
        public override void Notify_ResolutionChanged()
        {
            base.Notify_ResolutionChanged();
            _dirty = true;
        }

        private void SaveItemPartial(PartialManager<ItemPartial>.PartialUgc data)
        {
            Task.Run(
                async () =>
                {
                    await Data.SaveJsonAsync(
                        new PartialData<ItemPartial>
                        {
                            Data = _itemWorker.Data.Where(i => !i.IsHidden)
                               .Select(i => i.Data)
                               .Select(i => new ItemPartial { DefName = i.DefName, Cost = i.Cost, Enabled = i.Enabled, Name = i.Name, ItemData = i.ItemData })
                               .ToList(),
                            PartialType = PartialType.Items,
                            Description = data.Description
                        },
                        Path.Combine(Paths.PartialPath, data.Name)
                    );
                }
            );
        }

        private void SaveEventPartial(PartialManager<EventPartial>.PartialUgc data)
        {
            Task.Run(
                async () =>
                {
                    await Data.SaveJsonAsync(
                        new PartialData<EventPartial>
                        {
                            Data = _eventWorker.Data.Where(i => !i.IsHidden).Select(i => i.Data).Select(EventPartial.FromIncident).ToList(),
                            PartialType = PartialType.Events,
                            Description = data.Description
                        },
                        Path.Combine(Paths.PartialPath, data.Name)
                    );
                }
            );
        }

        private void SaveTraitPartial(PartialManager<TraitItem>.PartialUgc data)
        {
            Task.Run(
                async () =>
                {
                    await Data.SaveJsonAsync(
                        new PartialData<TraitItem>
                        {
                            Data = _traitWorker.Data.Where(i => !i.IsHidden).Select(i => i.Data).ToList(),
                            PartialType = PartialType.Traits,
                            Description = data.Description
                        },
                        Path.Combine(Paths.PartialPath, data.Name)
                    );
                }
            );
        }

        private void SavePawnPartial(PartialManager<PawnKindItem>.PartialUgc data)
        {
            Task.Run(
                async () =>
                {
                    await Data.SaveJsonAsync(
                        new PartialData<PawnKindItem>
                        {
                            Data = _pawnWorker.Data.Where(i => !i.IsHidden).Select(i => i.Data).ToList(),
                            PartialType = PartialType.Pawns,
                            Description = data.Description
                        },
                        Path.Combine(Paths.PartialPath, data.Name)
                    );
                }
            );
        }
    }
}
