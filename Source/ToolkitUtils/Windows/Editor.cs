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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using TwitchToolkit;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class Editor : Window
    {
        private readonly EventWorker eventWorker;
        private readonly ItemWorker itemWorker;
        private readonly PawnWorker pawnWorker;
        private readonly TraitWorker traitWorker;
        private bool dirty = true;

        private TabItem eventTab;
        private string exportPartialTooltip;
        private string helpTooltip;

        private string importPartialTooltip;
        private TabItem itemTab;

        private bool maximized;
        private TabItem pawnTab;
        private TabWorker tabWorker;
        private TabItem traitTab;

        public Editor()
        {
            draggable = true;
            forcePause = true;
            doCloseButton = false;

            itemWorker = new ItemWorker();
            pawnWorker = new PawnWorker();
            eventWorker = new EventWorker();
            traitWorker = new TraitWorker();
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
            InitializeTabs();
            itemWorker.Prepare();
            pawnWorker.Prepare();
            eventWorker.Prepare();
            traitWorker.Prepare();

            FetchTranslations();
        }

        private void FetchTranslations()
        {
            importPartialTooltip = "TKUtils.EditorTooltips.ImportPartial".Localize();
            exportPartialTooltip = "TKUtils.EditorTooltips.ExportPartial".Localize();
            helpTooltip = "TKUtils.EditorTooltips.GetHelp".Localize();
        }

        private void InitializeTabs()
        {
            itemTab ??= new TabItem
            {
                Label = "TKUtils.EditorTabs.Items".Localize(), ContentDrawer = rect => itemWorker.Draw()
            };

            pawnTab ??= new TabItem
            {
                Label = "TKUtils.EditorTabs.PawnKinds".Localize(), ContentDrawer = rect => pawnWorker.Draw()
            };

            eventTab ??= new TabItem
            {
                Label = "TKUtils.EditorTabs.Events".Localize(), ContentDrawer = rect => eventWorker.Draw()
            };

            traitTab ??= new TabItem
            {
                Label = "TKUtils.EditorTabs.Traits".Localize(), ContentDrawer = rect => traitWorker.Draw()
            };


            tabWorker.AddTab(itemTab);
            tabWorker.AddTab(eventTab);
            tabWorker.AddTab(traitTab);
            tabWorker.AddTab(pawnTab);
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
                eventWorker.NotifyResolutionChanged(innerResolution);
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

            butRect = butRect.ShiftLeft();
            if (Widgets.ButtonImage(butRect, Textures.QuestionMark))
            {
                Application.OpenURL("https://sirrandoo.github.io/toolkit-utils/editor");
            }

            butRect.TipRegion(helpTooltip);

            butRect = butRect.ShiftLeft();
            if (Widgets.ButtonImage(butRect, Textures.CopySettings))
            {
                SavePartial();
            }

            butRect.TipRegion(exportPartialTooltip);

            butRect = butRect.ShiftLeft();
            if (Widgets.ButtonImage(butRect, Textures.PasteSettings))
            {
                LoadPartial();
            }

            butRect.TipRegion(importPartialTooltip);
        }

        private void LoadPartial()
        {
            if (tabWorker.SelectedTab == itemTab)
            {
                Find.WindowStack.Add(PartialManager<ItemPartial>.CreateLoadInstance(LoadPartialItemData));
            }
            else if (tabWorker.SelectedTab == eventTab)
            {
                Find.WindowStack.Add(PartialManager<EventPartial>.CreateLoadInstance(LoadPartialEventData));
            }
            else if (tabWorker.SelectedTab == pawnTab)
            {
                Find.WindowStack.Add(PartialManager<PawnKindItem>.CreateLoadInstance(LoadPartialPawnData));
            }
            else if (tabWorker.SelectedTab == traitTab)
            {
                Find.WindowStack.Add(PartialManager<TraitItem>.CreateLoadInstance(LoadPartialTraitData));
            }
        }

        private void LoadPartialItemData([NotNull] PartialData<ItemPartial> data)
        {
            Data.LoadItemPartial(data.Data);
            itemWorker.NotifyGlobalDataChanged();
        }

        private void LoadPartialEventData([NotNull] PartialData<EventPartial> data)
        {
            Data.LoadEventPartial(data.Data);
            eventWorker.NotifyGlobalDataChanged();
        }

        private void LoadPartialTraitData([NotNull] PartialData<TraitItem> data)
        {
            Data.LoadTraitPartial(data.Data);
            traitWorker.NotifyGlobalDataChanged();
        }

        private void LoadPartialPawnData([NotNull] PartialData<PawnKindItem> data)
        {
            Data.LoadPawnPartial(data.Data);
            pawnWorker.NotifyGlobalDataChanged();
        }

        private void SavePartial()
        {
            if (tabWorker.SelectedTab == itemTab)
            {
                Find.WindowStack.Add(PartialManager<ItemPartial>.CreateSaveInstance(SaveItemPartial));
            }
            else if (tabWorker.SelectedTab == eventTab)
            {
                Find.WindowStack.Add(PartialManager<EventPartial>.CreateSaveInstance(SaveEventPartial));
            }
            else if (tabWorker.SelectedTab == traitTab)
            {
                Find.WindowStack.Add(PartialManager<TraitItem>.CreateSaveInstance(SaveTraitPartial));
            }
            else if (tabWorker.SelectedTab == pawnTab)
            {
                Find.WindowStack.Add(PartialManager<PawnKindItem>.CreateSaveInstance(SavePawnPartial));
            }
        }

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
            Rect contentRect = new Rect(0f, tabRect.height, windowRect.width, windowRect.height - tabRect.height)
               .ContractedBy(16f);

            itemWorker.NotifyResolutionChanged(contentRect);
            traitWorker.NotifyResolutionChanged(contentRect);
            pawnWorker.NotifyResolutionChanged(contentRect);
            eventWorker.NotifyResolutionChanged(contentRect);
        }

        public override void Notify_ResolutionChanged()
        {
            base.Notify_ResolutionChanged();
            dirty = true;
        }

        private void SaveItemPartial(PartialManager<ItemPartial>.PartialUgc data)
        {
            Task.Run(
                async () =>
                {
                    await Data.SaveJsonAsync(
                        new PartialData<ItemPartial>
                        {
                            Data = itemWorker.Data.Where(i => !i.IsHidden)
                               .Select(i => i.Data)
                               .Select(
                                    i => new ItemPartial
                                    {
                                        DefName = i.DefName,
                                        Cost = i.Cost,
                                        Enabled = i.Enabled,
                                        Name = i.Name,
                                        ItemData = i.ItemData
                                    }
                                )
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
                            Data = eventWorker.Data.Where(i => !i.IsHidden)
                               .Select(i => i.Data)
                               .Select(EventPartial.FromIncident)
                               .ToList(),
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
                            Data = traitWorker.Data.Where(i => !i.IsHidden).Select(i => i.Data).ToList(),
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
                            Data = pawnWorker.Data.Where(i => !i.IsHidden).Select(i => i.Data).ToList(),
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
