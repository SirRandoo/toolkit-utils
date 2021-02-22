﻿using System;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models.Tables;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Workers
{
    public class ItemTableWorker : TableWorker<ItemTableItem>
    {
        private Rect categoryHeaderRect = Rect.zero;
        private string categoryHeaderText;
        private Rect expandedHeaderRect = Rect.zero;
        private Rect nameHeaderRect = Rect.zero;
        private string nameHeaderText;
        private Rect priceHeaderRect = Rect.zero;
        private string priceHeaderText;
        private Vector2 scrollPos = Vector2.zero;
        private SortKey sortKey = SortKey.Name;
        private SortOrder sortOrder = SortOrder.Descending;

        private Rect stateHeaderRect = Rect.zero;

        private StateKey stateKey = StateKey.Enable;

        public override void DrawHeaders(Rect canvas)
        {
            if (Widgets.ButtonImage(
                stateHeaderRect,
                stateKey == StateKey.Enable ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                NotifyGlobalStateChanged(stateKey);
                stateKey = stateKey == StateKey.Enable ? StateKey.Disable : StateKey.Enable;
            }

            if (Widgets.ButtonImage(expandedHeaderRect, Textures.Gear))
            {
                NotifyGlobalSettingsCollapse();
            }

            DrawSortableHeaders();
            DrawSortableHeaderIcon();
        }

        private void DrawSortableHeaderIcon()
        {
            switch (sortKey)
            {
                case SortKey.Name:
                    SettingsHelper.DrawSortIndicator(nameHeaderRect, sortOrder);
                    return;
                case SortKey.Price:
                    SettingsHelper.DrawSortIndicator(priceHeaderRect, sortOrder);
                    return;
                case SortKey.Category:
                    SettingsHelper.DrawSortIndicator(categoryHeaderRect, sortOrder);
                    return;
                default:
                    return;
            }
        }

        private void DrawSortableHeaders()
        {
            SortKey previousKey = sortKey;
            if (SettingsHelper.DrawTabButton(nameHeaderRect, nameHeaderText))
            {
                sortKey = SortKey.Name;
            }

            if (SettingsHelper.DrawTabButton(priceHeaderRect, priceHeaderText))
            {
                sortKey = SortKey.Price;
            }

            if (SettingsHelper.DrawTabButton(categoryHeaderRect, categoryHeaderText))
            {
                sortKey = SortKey.Category;
            }

            if (sortKey != previousKey)
            {
                sortOrder = SortOrder.Descending;
                NotifySortRequested();
            }
            else
            {
                InvertSortOrder();
                NotifySortRequested();
            }
        }

        private void InvertSortOrder()
        {
            sortOrder = sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }

        private void NotifyGlobalSettingsCollapse()
        {
            foreach (ItemTableItem item in Data)
            {
                item.SettingsVisible = false;
            }
        }

        private void NotifyGlobalStateChanged(StateKey newState)
        {
            foreach (ItemTableItem item in Data)
            {
                item.Data.IsEnabled = newState == StateKey.Enable;
                item.Data.Update();
            }
        }

        public override void DrawTableContents(Rect canvas)
        {
            var viewPort = new Rect(
                0f,
                0f,
                canvas.width - 16f,
                RowLineHeight * Data.Where(i => !i.IsHidden).Sum(i => i.SettingsVisible ? 5f : 1f)
            );

            var listing = new Listing_Standard();
            listing.BeginScrollView(canvas, ref scrollPos, ref viewPort);

            foreach (ItemTableItem item in Data.Where(i => !i.IsHidden))
            {
                Rect lineRect = listing.GetRect(RowLineHeight * (item.SettingsVisible ? 5f : 1f));

                if (!lineRect.IsRegionVisible(viewPort, scrollPos))
                {
                    return;
                }

                DrawItem(lineRect, item);
            }

            listing.EndScrollView(ref viewPort);
        }

        private void DrawItem(Rect canvas, ItemTableItem item)
        {
            var checkboxRect = new Rect(stateHeaderRect.x, canvas.y, stateHeaderRect.width, canvas.height);
        }

        public override void Prepare()
        {
            LoadTranslations();
        }

        private void LoadTranslations()
        {
            nameHeaderText = "TKUtils.Headers.Name".Localize();
            priceHeaderText = "TKUtils.Headers.Price".Localize();
            categoryHeaderText = "TKUtils.ItemStore.Category".Localize();
        }

        public override void NotifySortRequested()
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    NotifyAscendingSortRequested();
                    return;
                case SortOrder.Descending:
                    NotifyDescendingSortRequested();
                    return;
                default:
                    return;
            }
        }

        private void NotifyDescendingSortRequested()
        {
            switch (sortKey)
            {
                case SortKey.Price:
                    Data = Data.OrderByDescending(i => i.Data.Price).ThenByDescending(i => i.Data.Name).ToList();
                    return;
                case SortKey.Category:
                    Data = Data.OrderByDescending(i => i.Data.Category).ThenByDescending(i => i.Data.Name).ToList();
                    return;
                default:
                    Data = Data.OrderByDescending(i => i.Data.Name).ToList();
                    return;
            }
        }

        private void NotifyAscendingSortRequested()
        {
            switch (sortKey)
            {
                case SortKey.Price:
                    Data = Data.OrderBy(i => i.Data.Price).ThenBy(i => i.Data.Name).ToList();
                    return;
                case SortKey.Category:
                    Data = Data.OrderBy(i => i.Data.Category).ThenBy(i => i.Data.Name).ToList();
                    return;
                default:
                    Data = Data.OrderBy(i => i.Data.Name).ToList();
                    return;
            }
        }

        public override void NotifySearchRequested(string query)
        {
            FilterDataBySearch(query);
        }

        public override void NotifyResolutionChanged(Rect canvas)
        {
            float distributedWidth = Mathf.FloorToInt((canvas.width - LineHeight * 2f) * 0.333f);
            stateHeaderRect = new Rect(0f, 0f, LineHeight, LineHeight);
            nameHeaderRect = new Rect(LineHeight, 0f, distributedWidth, LineHeight);
            priceHeaderRect = nameHeaderRect.ShiftRight(0f);
            categoryHeaderRect = priceHeaderRect.ShiftRight(0f);
            expandedHeaderRect = new Rect(categoryHeaderRect.x + categoryHeaderRect.width, 0f, LineHeight, LineHeight);
        }

        protected override void FilterDataBySearch(string query)
        {
            foreach (ItemTableItem item in Data)
            {
                item.IsHidden = !item.Data.Name.Equals(query, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private enum StateKey { Enable, Disable }

        private enum SortKey { Name, Price, Category }
    }
}