﻿// ToolkitUtils
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
using System.Threading.Tasks;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class TraitConfigDialog : Window
    {
        private const float LineScale = 1.25f;
        private readonly List<TraitItem> cache = Data.Traits;
        private string addCostText;
        private string addKarmaTypeText;
        private string bypassLimitText;

        private string currentQuery = "";
        private string defaultKarmaText;
        private string disableAllText;
        private string enableAllText;
        private TraitItem expanded;
        private string expandedName = "";
        private int globalAddCost;
        private int globalRemoveCost;
        private string lastQuery = "";
        private string nameHeaderText;
        private string nameText;
        private string removeCostText;
        private string removeKarmaTypeText;
        private string resetText;
        private List<TraitItem> results;
        private Vector2 scrollPos = Vector2.zero;
        private string searchText;
        private Sorter sorter = Sorter.Name;
        private SortMode sortMode = SortMode.Ascending;
        private string titleText;

        public TraitConfigDialog()
        {
            GetTranslations();

            doCloseX = true;
            forcePause = true;

            optionalTitle = titleText;
            cache?.SortBy(t => t.Name);
        }

        public override Vector2 InitialSize => new Vector2(1024f, UI.screenHeight * 0.9f);

        private void GetTranslations()
        {
            titleText = "TKUtils.TraitStore.Title".Localize();
            nameHeaderText = "TKUtils.Headers.Name".Localize();
            searchText = "TKUtils.Buttons.Search".Localize();
            addCostText = "TKUtils.Fields.AddPrice".Localize();
            removeCostText = "TKUtils.Fields.RemovePrice".Localize();
            addKarmaTypeText = "TKUtils.Fields.AddKarmaType".Localize();
            removeKarmaTypeText = "TKUtils.Fields.RemoveKarmaType".Localize();
            bypassLimitText = "TKUtils.Fields.BypassTraitLimit".Localize();
            resetText = "TKUtils.Buttons.Reset".Localize();
            defaultKarmaText = "TKUtils.Fields.DefaultKarmaType".Localize();
            nameText = "TKUtils.Inputs.Name".Localize();
        }

        private void DrawHeader(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            (Rect searchLabel, Rect searchField) =
                new Rect(inRect.x, inRect.y, inRect.width * 0.19f, Text.LineHeight).ToForm(0.3f);

            Widgets.Label(searchLabel, searchText);
            currentQuery = Widgets.TextField(searchField, currentQuery);

            if (currentQuery.Length > 0 && SettingsHelper.DrawClearButton(searchField))
            {
                currentQuery = "";
            }

            if (currentQuery.NullOrEmpty())
            {
                results = null;
                lastQuery = "";
            }


            var globalAddRect = new Rect(inRect.width - 5f - 200f, 0f, 200f, Text.LineHeight);
            var globalRemoveRect = new Rect(inRect.width - 5f - 200f, Text.LineHeight, 200f, Text.LineHeight);

            var globalAddBuffer = globalAddCost.ToString();
            Widgets.Label(globalAddRect.LeftHalf(), addCostText);
            Widgets.TextFieldNumeric(globalAddRect.RightHalf(), ref globalAddCost, ref globalAddBuffer);

            if (SettingsHelper.DrawDoneButton(globalAddRect.RightHalf()))
            {
                foreach (TraitItem trait in Data.Traits)
                {
                    trait.CostToAdd = globalAddCost;
                }
            }

            var globalRemoveBuffer = globalRemoveCost.ToString();
            Widgets.Label(globalRemoveRect.LeftHalf(), removeCostText);
            Widgets.TextFieldNumeric(globalRemoveRect.RightHalf(), ref globalRemoveCost, ref globalRemoveBuffer);

            if (SettingsHelper.DrawDoneButton(globalRemoveRect.RightHalf()))
            {
                foreach (TraitItem trait in Data.Traits)
                {
                    trait.CostToRemove = globalRemoveCost;
                }
            }

            GUI.EndGroup();
        }

        private void DrawGlobalStateButtons(Rect enableRect, Rect disableRect)
        {
            if (Widgets.ButtonText(enableRect, enableAllText))
            {
                foreach (TraitItem trait in results ?? Data.Traits)
                {
                    trait.CanAdd = true;
                    trait.CanRemove = true;
                }
            }

            if (Widgets.ButtonText(disableRect, disableAllText))
            {
                foreach (TraitItem trait in results ?? Data.Traits)
                {
                    trait.CanAdd = false;
                    trait.CanRemove = false;
                }
            }
        }

        private void DrawTraitSettings(Rect inRect)
        {
            string addKarma = expanded.Data.KarmaTypeForAdding == null
                ? defaultKarmaText
                : Enum.GetName(typeof(KarmaType), expanded.Data.KarmaTypeForAdding);
            string removeKarma = expanded.Data.KarmaTypeForRemoving == null
                ? defaultKarmaText
                : Enum.GetName(typeof(KarmaType), expanded.Data.KarmaTypeForRemoving);


            var listing = new Listing_Standard();
            listing.Begin(inRect);

            Widgets.CheckboxLabeled(
                listing.GetRect(Text.LineHeight * LineScale),
                bypassLimitText,
                ref expanded.Data.CanBypassLimit
            );
            listing.Gap(Text.LineHeight * LineScale * 0.5f);

            (Rect nameLabel, Rect nameField) = listing.GetRect(Text.LineHeight * LineScale).ToForm(0.52f);
            expandedName = Widgets.TextField(nameField, expandedName).ToToolkit();

            if (expandedName.Length > 0 && SettingsHelper.DrawClearButton(nameField))
            {
                expandedName = "";
                expanded.Name = expanded.GetDefaultName();
                expanded.Data!.CustomName = false;
            }

            SettingsHelper.DrawLabel(nameLabel, nameText);

            listing.GapLine(Text.LineHeight * LineScale);
            (Rect addKarmaLabel, Rect addKarmaField) = listing.GetRect(Text.LineHeight * LineScale).ToForm(0.52f);
            SettingsHelper.DrawLabel(addKarmaLabel, addKarmaTypeText);
            if (Widgets.ButtonText(addKarmaField, addKarma))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        Data.KarmaTypes.Select(
                                k => new FloatMenuOption(
                                    Enum.GetName(typeof(KarmaType), k),
                                    () => expanded.Data.KarmaTypeForAdding = k
                                )
                            )
                           .ToList()
                    )
                );
            }

            listing.Gap(1f);
            (Rect removeKarmaLabel, Rect removeKarmaField) = listing.GetRect(Text.LineHeight * LineScale).ToForm(0.52f);
            SettingsHelper.DrawLabel(removeKarmaLabel, removeKarmaTypeText);
            if (Widgets.ButtonText(removeKarmaField, removeKarma))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        Data.KarmaTypes.Select(
                                k => new FloatMenuOption(
                                    Enum.GetName(typeof(KarmaType), k),
                                    () => expanded.Data.KarmaTypeForRemoving = k
                                )
                            )
                           .ToList()
                    )
                );
            }

            Rect lastLine = new Rect(
                0f,
                inRect.height - Text.LineHeight * LineScale,
                inRect.width,
                Text.LineHeight * LineScale
            ).Rounded();

            if (Widgets.ButtonText(
                new Rect(lastLine.x + 10f, lastLine.y, lastLine.width - 20f, lastLine.height),
                resetText
            ))
            {
                expanded.Name = expanded.GetDefaultName();
                expanded.CanAdd = true;
                expanded.CanRemove = true;
                expanded.Data!.CustomName = false;
                expanded.Data.KarmaTypeForAdding = null;
                expanded.Data.KarmaTypeForRemoving = null;
            }

            listing.End();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            var listing = new Listing_Standard {maxOneColumn = true};
            var headerRect = new Rect(0f, 0f, inRect.width, Text.LineHeight * 2f);
            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 4f,
                inRect.width,
                inRect.height - Text.LineHeight * 4f
            );

            DrawHeader(headerRect);
            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 3f, inRect.width);

            if (expanded != null)
            {
                DoExpandedDialog(contentArea);
                GUI.EndGroup();
                return;
            }

            float availableWidth = inRect.width - 30f * 2f - Text.LineHeight * LineScale;
            bool wrapped = Text.WordWrap;
            Text.WordWrap = false;
            Rect nameHeaderRect = new Rect(0f, 0f, availableWidth * 0.43f - 5f, Text.LineHeight).Rounded();

            float remainingWidth = availableWidth - nameHeaderRect.width;

            Rect addHeaderRect = new Rect(
                nameHeaderRect.width + 5f,
                nameHeaderRect.y,
                Text.LineHeight * LineScale,
                nameHeaderRect.height
            ).Rounded();
            Rect addCostHeaderRect = new Rect(
                addHeaderRect.x + addHeaderRect.width + 5f,
                nameHeaderRect.y,
                remainingWidth * 0.413f,
                nameHeaderRect.height
            ).Rounded();
            Rect removeHeaderRect = new Rect(
                addCostHeaderRect.x + addCostHeaderRect.width + 60f,
                nameHeaderRect.y,
                Text.LineHeight * LineScale,
                nameHeaderRect.height
            ).Rounded();
            var removeCostHeaderRect = new Rect(
                removeHeaderRect.x + removeHeaderRect.width + 5f,
                nameHeaderRect.y,
                addCostHeaderRect.width,
                nameHeaderRect.height
            );
            Rect expandHeaderRect = new Rect(
                removeCostHeaderRect.x + removeCostHeaderRect.width + 5f,
                nameHeaderRect.y,
                Text.LineHeight * LineScale,
                nameHeaderRect.height
            ).Rounded();

            GUI.BeginGroup(new Rect(0f, Text.LineHeight * 3f, inRect.width, Text.LineHeight));
            Widgets.DrawHighlightIfMouseover(nameHeaderRect);
            Widgets.DrawHighlightIfMouseover(addCostHeaderRect);
            Widgets.DrawHighlightIfMouseover(removeCostHeaderRect);


            DrawTraitHeaders(nameHeaderRect, addCostHeaderRect, removeCostHeaderRect);
            GUI.EndGroup();


            float lineHeight = Mathf.RoundToInt(Text.LineHeight * LineScale);
            List<TraitItem> effectiveList = results ?? cache;
            int total = effectiveList.Count;
            var viewPort = new Rect(0f, 0f, contentArea.width - 16f, lineHeight * total);
            var traits = new Rect(0f, 0f, contentArea.width, contentArea.height);

            GUI.BeginGroup(contentArea);
            listing.BeginScrollView(traits, ref scrollPos, ref viewPort);
            for (var index = 0; index < effectiveList.Count; index++)
            {
                TraitItem trait = effectiveList[index];
                Rect lineRect = listing.GetRect(lineHeight);

                if (!lineRect.IsRegionVisible(traits, scrollPos))
                {
                    continue;
                }

                if (index % 2 == 1)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                var nameRect = new Rect(nameHeaderRect.x, lineRect.y, nameHeaderRect.width, lineRect.height);
                var addRect = new Rect(addHeaderRect.x, lineRect.y, addHeaderRect.width, lineRect.height);
                var addCostRect = new Rect(addCostHeaderRect.x, lineRect.y, addCostHeaderRect.width, lineRect.height);
                var removeRect = new Rect(removeHeaderRect.x, lineRect.y, removeHeaderRect.width, lineRect.height);
                var removeCostRect = new Rect(
                    removeCostHeaderRect.x,
                    lineRect.y,
                    removeCostHeaderRect.width,
                    lineRect.height
                );
                var expandRect = new Rect(expandHeaderRect.x, lineRect.y, expandHeaderRect.width, lineRect.height);

                GUI.DrawTexture(expandRect, Textures.Gear);

                if (Widgets.ButtonInvisible(expandRect))
                {
                    expanded = trait;

                    if (expanded.Data?.CustomName == true)
                    {
                        expandedName = expanded.Name;
                    }
                }

                SettingsHelper.DrawLabel(nameRect, trait.Name);
                Widgets.Checkbox(addRect.position, ref trait.CanAdd, addRect.height, paintable: true);
                SettingsHelper.DrawPriceField(addCostRect, ref trait.CostToAdd);
                Widgets.Checkbox(removeRect.position, ref trait.CanRemove, removeRect.height, paintable: true);
                SettingsHelper.DrawPriceField(removeCostRect, ref trait.CostToRemove);
            }

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);
            GUI.EndGroup();

            Text.WordWrap = wrapped;
        }

        private void DrawTraitHeaders(Rect nameHeaderRect, Rect addCostHeaderRect, Rect removeCostHeaderRect)
        {
            if (Widgets.ButtonText(nameHeaderRect, $"   {nameHeaderText}", false))
            {
                if (sorter != Sorter.Name)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.Name;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                SortCurrentWorkingList();
            }

            if (Widgets.ButtonText(addCostHeaderRect, $"   {addCostText}", false))
            {
                if (sorter != Sorter.AddCost)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.AddCost;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                SortCurrentWorkingList();
            }

            if (Widgets.ButtonText(removeCostHeaderRect, $"   {removeCostText}", false))
            {
                if (sorter != Sorter.RemoveCost)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.RemoveCost;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                SortCurrentWorkingList();
            }

            DrawHeaderSortingIcon(nameHeaderRect, addCostHeaderRect, removeCostHeaderRect);
        }

        private void DrawHeaderSortingIcon(Rect nameHeaderRect, Rect addCostHeaderRect, Rect removeCostHeaderRect)
        {
            Rect position = new Rect(0f, nameHeaderRect.y + Text.LineHeight / 2f - 4f, 8f, 8f).Rounded();
            switch (sorter)
            {
                case Sorter.Name:
                    position.x = nameHeaderRect.x;
                    break;
                case Sorter.AddCost:
                    position.x = addCostHeaderRect.x;
                    break;
                case Sorter.RemoveCost:
                    position.x = removeCostHeaderRect.x;
                    break;
            }

            GUI.DrawTexture(
                position,
                sortMode != SortMode.Descending ? Textures.SortingAscend : Textures.SortingDescend
            );
        }

        private void DoExpandedDialog(Rect inRect)
        {
            float expandedWidth = inRect.width * 0.333f;
            Vector2 center = inRect.center;

            Rect expandedDialog = new Rect(
                    center.x - expandedWidth / 2f,
                    center.y - Text.LineHeight * LineScale * 4f,
                    expandedWidth,
                    Text.LineHeight * LineScale * 8f
                ).ExpandedBy(StandardMargin * 2f)
               .Rounded();

            Widgets.DrawBoxSolid(expandedDialog, new Color(0.13f, 0.16f, 0.17f));
            Widgets.Label(
                new Rect(
                    expandedDialog.x + 8f,
                    expandedDialog.y + 5f,
                    expandedDialog.width - 30f,
                    Text.LineHeight * LineScale
                ).Rounded(),
                "TKUtils.Headers.DataDialog".Localize(expanded.Name)
            );

            Widgets.DrawHighlight(
                new Rect(expandedDialog.position, new Vector2(expandedDialog.width, Text.LineHeight * LineScale))
                   .Rounded()
            );

            GUI.BeginGroup(expandedDialog.ContractedBy(StandardMargin * 2f));
            DrawTraitSettings(
                new Rect(
                    0f,
                    0f,
                    expandedDialog.width - StandardMargin * 4f - 1f,
                    expandedDialog.height - StandardMargin * 4f
                )
            );
            GUI.EndGroup();

            if (Widgets.CloseButtonFor(expandedDialog))
            {
                CloseExpandedMenu();
            }
        }

        private void SortCurrentWorkingList()
        {
            List<TraitItem> workingList = results ?? cache;

            switch (sorter)
            {
                case Sorter.Name when sortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.Name);
                    results = workingList;
                    return;
                case Sorter.Name when sortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.Name);
                    results = workingList;
                    return;
                case Sorter.AddCost when sortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.CostToAdd);
                    results = workingList;
                    return;
                case Sorter.AddCost when sortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.CostToAdd);
                    results = workingList;
                    return;
                case Sorter.RemoveCost when sortMode == SortMode.Ascending:
                    workingList.SortBy(i => i.CostToRemove);
                    results = workingList;
                    return;
                case Sorter.RemoveCost when sortMode == SortMode.Descending:
                    workingList.SortByDescending(i => i.CostToRemove);
                    results = workingList;
                    return;
            }
        }

        private void Notify__SearchRequested()
        {
            lastQuery = currentQuery;

            results = GetSearchResults();
            SortCurrentWorkingList();
        }

        public override void WindowUpdate()
        {
            base.WindowUpdate();

            if (lastQuery.Equals(currentQuery))
            {
                return;
            }

            if (Time.time % 2 < 1)
            {
                Notify__SearchRequested();
            }
        }

        private List<TraitItem> GetSearchResults()
        {
            string serialized = currentQuery?.ToToolkit();

            if (serialized == null)
            {
                return null;
            }

            return cache.Where(
                    t => t.Name.ToToolkit().EqualsIgnoreCase(currentQuery.ToToolkit())
                         || t.Name.ToToolkit().Contains(currentQuery.ToToolkit())
                )
               .ToList();
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

        public override void OnCancelKeyPressed()
        {
            if (expanded == null)
            {
                base.OnCancelKeyPressed();
            }
            else
            {
                CloseExpandedMenu();
                Event.current.Use();
            }
        }

        public override void OnAcceptKeyPressed()
        {
            if (expanded == null)
            {
                base.OnAcceptKeyPressed();
            }
            else
            {
                CloseExpandedMenu();
                Event.current.Use();
            }
        }

        private void CloseExpandedMenu()
        {
            if (expanded.Data != null && !expandedName.NullOrEmpty())
            {
                expanded.Name = expandedName;
                expanded.Data.CustomName = true;
            }

            if (!expandedName.NullOrEmpty())
            {
                SortCurrentWorkingList();
            }

            expanded = null;
            expandedName = null;
        }
    }
}
