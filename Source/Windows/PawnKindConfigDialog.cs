using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PawnKindConfigDialog : Window
    {
        private readonly List<XmlRace> cache = TkUtils.ShopExpansion.Races;
        private TaggedString applyText;

        private bool control;
        private string currentQuery = "";
        private TaggedString disableText;
        private TaggedString enableText;
        private int globalCost;
        private string lastQuery = "";
        private TaggedString nameHeaderText;
        private TaggedString priceHeaderText;
        private TaggedString priceText;
        private TaggedString resetText;
        private List<XmlRace> results;
        private Vector2 scrollPos = Vector2.zero;
        private TaggedString searchText;
        private bool shift;

        private Sorter sorter = Sorter.Name;
        private SortMode sortMode = SortMode.Ascending;

        private string titleText;


        public PawnKindConfigDialog()
        {
            GetTranslations();

            onlyOneOfTypeAllowed = true;
            doCloseX = true;
            forcePause = true;

            optionalTitle = titleText;
            cache?.SortBy(r => r.Name.NullOrEmpty() ? r.DefName : r.Name);

            if (cache == null)
            {
                TkLogger.Warn("The race shop is null! You should report this.");
            }
        }

        public override Vector2 InitialSize => new Vector2(640f, 740f);

        private void GetTranslations()
        {
            titleText = "TKUtils.PawnKindStore.Title".Localize();
            nameHeaderText = "TKUtils.Headers.Name".Localize();
            priceText = "TKUtils.Inputs.Price".Localize();
            priceHeaderText = "TKUtils.Headers.Price".Localize();
            applyText = "TKUtils.Buttons.Apply".Localize();
            searchText = "TKUtils.Buttons.Search".Localize();
            resetText = "TKUtils.Buttons.ResetAll".Localize();
            enableText = "TKUtils.Buttons.EnableAll".Localize();
            disableText = "TKUtils.Buttons.DisableAll".Localize();
        }

        private void DrawHeader(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            float midpoint = inRect.width / 2f;
            var searchRect = new Rect(inRect.x, inRect.y, inRect.width * 0.3f, Text.LineHeight);
            Rect searchLabel = searchRect.LeftHalf();
            Rect searchField = searchRect.RightHalf();

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


            float maxWidth = Mathf.Max(
                                 Text.CalcSize(enableText).x,
                                 Text.CalcSize(disableText).x,
                                 Text.CalcSize(resetText).x
                             )
                             * 1.5f;
            var disableRect = new Rect(inRect.width - maxWidth, inRect.y + Text.LineHeight, maxWidth, Text.LineHeight);
            var enableRect = new Rect(inRect.width - maxWidth, inRect.y, maxWidth, Text.LineHeight);
            var resetRect = new Rect(inRect.width - maxWidth * 2f, inRect.y, maxWidth, Text.LineHeight);

            if (Widgets.ButtonText(resetRect, resetText))
            {
                List<PawnKindDef> kinds = DefDatabase<PawnKindDef>.AllDefsListForReading;

                foreach (XmlRace race in results ?? cache)
                {
                    PawnKindDef kind = kinds.FirstOrDefault(k => k.race?.defName.Equals(race.DefName) ?? false);

                    if (kind == null)
                    {
                        continue;
                    }

                    race.Price = StoreDialog.CalculateToolkitPrice(kind.race?.BaseMarketValue ?? 0f);
                }
            }

            if (Widgets.ButtonText(enableRect, globalCost > 0 ? applyText : enableText))
            {
                foreach (XmlRace race in TkUtils.ShopExpansion.Races)
                {
                    if (globalCost > 0)
                    {
                        race.Price = globalCost;
                    }
                    else
                    {
                        race.Enabled = true;
                    }
                }

                if (globalCost > 0)
                {
                    globalCost = 0;
                }
            }

            if (Widgets.ButtonText(disableRect, disableText))
            {
                foreach (XmlRace race in TkUtils.ShopExpansion.Races)
                {
                    race.Enabled = false;
                }
            }


            var globalCostRect = new Rect(
                enableRect.x + enableRect.width + 5f,
                enableRect.y,
                midpoint - enableRect.width - 5f,
                Text.LineHeight
            );

            var globalAddBuffer = globalCost.ToString();
            Widgets.Label(globalCostRect.LeftHalf(), priceText);
            Widgets.TextFieldNumeric(globalCostRect.RightHalf(), ref globalCost, ref globalAddBuffer);

            if (globalCost > 0 && SettingsHelper.DrawClearButton(globalCostRect.RightHalf()))
            {
                globalCost = 0;
            }

            GUI.EndGroup();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            var listing = new Listing_Standard {maxOneColumn = true};

            DrawHeader(new Rect(0f, 0f, inRect.width, Text.LineHeight * 2f));

            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 3f, inRect.width);

            var headingRect = new Rect(0f, Text.LineHeight * 4f, inRect.width, Text.LineHeight);
            var nameHeadingRect = new Rect(Widgets.CheckboxSize + 5f, 0f, inRect.width * 0.5f, Text.LineHeight);
            var priceHeadingRect = new Rect(
                nameHeadingRect.x + nameHeadingRect.width + 5f,
                0f,
                inRect.width - nameHeadingRect.width - Widgets.CheckboxSize - 10f,
                Text.LineHeight
            );

            GUI.BeginGroup(headingRect);
            Widgets.DrawHighlightIfMouseover(nameHeadingRect);
            Widgets.DrawHighlightIfMouseover(priceHeadingRect);

            if (Widgets.ButtonText(nameHeadingRect, "  " + nameHeaderText, false))
            {
                if (sorter != Sorter.Name)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.Name;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;
                SortCurrentWorkingList();
            }

            if (Widgets.ButtonText(priceHeadingRect, "  " + priceHeaderText, false))
            {
                if (sorter != Sorter.Price)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.Price;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                SortCurrentWorkingList();
            }

            var position = new Rect(0f, nameHeadingRect.y + Text.LineHeight / 2f - 4f, 8f, 8f);
            switch (sorter)
            {
                case Sorter.Name:
                    position.x = nameHeadingRect.x;
                    break;
                case Sorter.Price:
                    position.x = priceHeadingRect.x;
                    break;
            }

            GUI.DrawTexture(
                position,
                sortMode != SortMode.Descending ? Textures.SortingAscend : Textures.SortingDescend
            );
            GUI.EndGroup();

            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 5f,
                inRect.width,
                inRect.height - Text.LineHeight * 5f
            );

            List<XmlRace> effectiveList = results ?? cache;
            int total = effectiveList.Count;
            const float scale = 1.1f;
            var viewPort = new Rect(0f, 0f, contentArea.width - 16f, Text.LineHeight * scale * total);
            var races = new Rect(0f, 0f, contentArea.width, contentArea.height);

            GUI.BeginGroup(contentArea);
            listing.BeginScrollView(races, ref scrollPos, ref viewPort);
            for (var index = 0; index < effectiveList.Count; index++)
            {
                XmlRace race = effectiveList[index];
                TextAnchor anchor = Text.Anchor;
                Rect lineRect = listing.GetRect(Text.LineHeight * scale);
                var stateRect = new Rect(
                    0f,
                    lineRect.y,
                    Widgets.CheckboxSize,
                    Text.LineHeight
                );
                var nameRect = new Rect(Widgets.CheckboxSize + 5f, lineRect.y, nameHeadingRect.width, lineRect.height);
                var priceRect = new Rect(priceHeadingRect.x, lineRect.y, priceHeadingRect.width - 16f, lineRect.height);

                if (!lineRect.IsRegionVisible(races, scrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(
                    nameRect,
                    race.Name.ToLowerInvariant().Equals(race.Name) ? race.Name.CapitalizeFirst() : race.Name
                );
                Text.Anchor = anchor;

                Widgets.Checkbox(stateRect.x, stateRect.y, ref race.Enabled, paintable: true);

                if (!race.Enabled)
                {
                    continue;
                }

                SettingsHelper.DrawPriceField(priceRect, ref race.Price, ref control, ref shift);
            }

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);

            GUI.EndGroup();
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

        private List<XmlRace> GetSearchResults()
        {
            string serialized = currentQuery?.ToToolkit();

            if (serialized == null)
            {
                return null;
            }

            List<XmlRace> searchResults = cache.Where(
                    t => t.DefName.ToToolkit().EqualsIgnoreCase(currentQuery.ToToolkit())
                         || t.DefName.ToToolkit().Contains(currentQuery.ToToolkit())
                )
                .ToList();

            return searchResults;
        }

        public override void PreClose()
        {
            ShopExpansionHelper.SaveData(TkUtils.ShopExpansion, ShopExpansionHelper.ExpansionFile);

            if (TkSettings.JsonShop)
            {
                ShopExpansionHelper.DumpShopExpansion();
            }
        }

        private void SortCurrentWorkingList()
        {
            List<XmlRace> workingList = results ?? cache;

            switch (sorter)
            {
                case Sorter.Name:
                    switch (sortMode)
                    {
                        case SortMode.Ascending:
                            workingList.SortBy(i => i.Name);
                            results = workingList;
                            return;
                        case SortMode.Descending:
                            workingList.SortByDescending(i => i.Name);
                            results = workingList;
                            return;
                        default:
                            return;
                    }
                case Sorter.Price:
                    switch (sortMode)
                    {
                        case SortMode.Ascending:
                            workingList.SortBy(i => i.Price);
                            results = workingList;
                            return;
                        case SortMode.Descending:
                            workingList.SortByDescending(i => i.Price);
                            results = workingList;
                            return;
                        default:
                            return;
                    }
                default:
                    return;
            }
        }
    }
}
