using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PawnKindConfigDialog : Window
    {
        private const float LineScale = 1.25f;
        private readonly List<PawnKindItem> cache = Data.PawnKinds;
        private readonly KarmaType pawnKarmaType;
        private string applyText;

        private bool control;
        private string currentQuery = "";
        private string disableText;
        private string enableText;
        private PawnKindItem expanded;
        private string expandedName;
        private int globalCost;
        private string karmaTypeText;
        private string lastQuery = "";
        private string nameHeaderText;
        private string nameText;
        private string noCustomKarmaText;
        private string priceHeaderText;
        private string priceText;
        private string resetText;
        private List<PawnKindItem> results;
        private Vector2 scrollPos = Vector2.zero;
        private string searchText;
        private bool shift;

        private Sorter sorter = Sorter.Name;
        private SortMode sortMode = SortMode.Ascending;

        private string titleText;


        public PawnKindConfigDialog()
        {
            GetTranslations();

            doCloseX = true;
            forcePause = true;

            optionalTitle = titleText;
            cache?.SortBy(r => r.Name);

            pawnKarmaType = DefDatabase<StoreIncidentVariables>.GetNamedSilentFail("BuyPawn").karmaType;
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
            noCustomKarmaText = "TKUtils.TraitStore.NoCustomKarmaType".Localize();
            disableText = "TKUtils.Buttons.DisableAll".Localize();
            nameText = "TKUtils.Inputs.Name".Localize();
            karmaTypeText = "TKUtils.IncidentEditor.Karma".Localize();
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

            DrawGlobalButtons(resetRect, enableRect, disableRect);


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

        private void DrawGlobalButtons(Rect resetRect, Rect enableRect, Rect disableRect)
        {
            DrawGlobalResetButton(resetRect);
            DrawGlobalEnableButton(enableRect);
            DrawGlobalEnableButtons(disableRect);
        }

        private void DrawGlobalEnableButtons(Rect disableRect)
        {
            if (!Widgets.ButtonText(disableRect, disableText))
            {
                return;
            }

            foreach (PawnKindItem item in Data.PawnKinds)
            {
                item.Enabled = false;
            }
        }

        private void DrawGlobalEnableButton(Rect enableRect)
        {
            if (!Widgets.ButtonText(enableRect, globalCost > 0 ? applyText : enableText))
            {
                return;
            }

            foreach (PawnKindItem item in Data.PawnKinds)
            {
                if (globalCost > 0)
                {
                    item.Cost = globalCost;
                }
                else
                {
                    item.Enabled = true;
                }
            }

            if (globalCost > 0)
            {
                globalCost = 0;
            }
        }

        private void DrawGlobalResetButton(Rect resetRect)
        {
            if (!Widgets.ButtonText(resetRect, resetText))
            {
                return;
            }

            foreach (PawnKindItem item in results ?? cache)
            {
                PawnKindDef kind =
                    DefDatabase<PawnKindDef>.AllDefs.FirstOrDefault(k => k.race?.defName.Equals(item.DefName) ?? false);

                if (kind == null)
                {
                    continue;
                }

                item.Cost = kind.race.CalculateStorePrice();
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            var listing = new Listing_Standard {maxOneColumn = true};
            var headerRect = new Rect(0f, 0f, inRect.width, Text.LineHeight * 2f);
            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 5f,
                inRect.width,
                inRect.height - Text.LineHeight * 5f
            );

            DrawHeader(headerRect);

            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 3f, inRect.width);

            if (expanded != null)
            {
                DoExpandedDialog(contentArea);
                GUI.EndGroup();
                return;
            }

            var headingRect = new Rect(0f, Text.LineHeight * 4f, inRect.width, Text.LineHeight);
            var nameHeadingRect = new Rect(Widgets.CheckboxSize + 5f, 0f, inRect.width * 0.45f, Text.LineHeight);
            var priceHeadingRect = new Rect(
                nameHeadingRect.x + nameHeadingRect.width + 5f,
                0f,
                inRect.width - nameHeadingRect.width - Widgets.CheckboxSize * 2 - 20f,
                Text.LineHeight
            );

            GUI.BeginGroup(headingRect);
            Widgets.DrawHighlightIfMouseover(nameHeadingRect);
            Widgets.DrawHighlightIfMouseover(priceHeadingRect);

            DrawKindHeaders(nameHeadingRect, priceHeadingRect);
            GUI.EndGroup();

            List<PawnKindItem> effectiveList = results ?? cache;
            int total = effectiveList.Count;
            var viewPort = new Rect(0f, 0f, contentArea.width - 16f, Text.LineHeight * LineScale * total);
            var races = new Rect(0f, 0f, contentArea.width, contentArea.height);

            GUI.BeginGroup(contentArea);
            listing.BeginScrollView(races, ref scrollPos, ref viewPort);
            for (var index = 0; index < effectiveList.Count; index++)
            {
                PawnKindItem item = effectiveList[index];
                Rect lineRect = listing.GetRect(Text.LineHeight * LineScale);
                var stateRect = new Rect(0f, lineRect.y, Widgets.CheckboxSize, Text.LineHeight);
                var nameRect = new Rect(Widgets.CheckboxSize + 5f, lineRect.y, nameHeadingRect.width, lineRect.height);
                var priceRect = new Rect(priceHeadingRect.x, lineRect.y, priceHeadingRect.width - 16f, lineRect.height);
                var settingsRect = new Rect(
                    priceRect.x + priceRect.width + 5f,
                    priceRect.y,
                    Widgets.CheckboxSize,
                    lineRect.height
                );

                if (!lineRect.IsRegionVisible(races, scrollPos))
                {
                    continue;
                }

                if (index % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                DrawKindItem(nameRect, item, stateRect, priceRect, settingsRect);
            }

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);

            GUI.EndGroup();
        }

        private void DrawKindHeaders(Rect nameHeadingRect, Rect priceHeadingRect)
        {
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
                if (sorter != Sorter.Cost)
                {
                    sortMode = SortMode.Descending;
                }

                sorter = Sorter.Cost;
                sortMode = sortMode == SortMode.Ascending ? SortMode.Descending : SortMode.Ascending;

                SortCurrentWorkingList();
            }

            var position = new Rect(0f, nameHeadingRect.y + Text.LineHeight / 2f - 4f, 8f, 8f);
            switch (sorter)
            {
                case Sorter.Name:
                    position.x = nameHeadingRect.x;
                    break;
                case Sorter.Cost:
                    position.x = priceHeadingRect.x;
                    break;
            }

            GUI.DrawTexture(
                position,
                sortMode != SortMode.Descending ? Textures.SortingAscend : Textures.SortingDescend
            );
        }

        private void DrawKindItem(Rect nameRect, PawnKindItem item, Rect stateRect, Rect priceRect, Rect settingsRect)
        {
            SettingsHelper.DrawLabelAnchored(
                nameRect,
                item.Name.ToLowerInvariant().Equals(item.Name) ? item.Name.CapitalizeFirst() : item.Name,
                TextAnchor.MiddleLeft
            );

            Widgets.Checkbox(stateRect.x, stateRect.y, ref item.Enabled, paintable: true);

            if (!item.Enabled)
            {
                return;
            }

            SettingsHelper.DrawPriceField(priceRect, ref item.Cost, ref control, ref shift);

            GUI.DrawTexture(settingsRect, Textures.Gear);

            if (Widgets.ButtonInvisible(settingsRect))
            {
                expanded = item;
            }
        }

        private void DoExpandedDialog(Rect contentArea)
        {
            float expandedWidth = contentArea.width * 0.45f;
            Vector2 center = contentArea.center;

            Rect expandedDialog = new Rect(
                center.x - expandedWidth / 2f,
                center.y - Text.LineHeight * LineScale * 4f,
                expandedWidth,
                Text.LineHeight * LineScale * 8f
            ).ExpandedBy(StandardMargin * 2f);

            Widgets.DrawBoxSolid(expandedDialog, new Color(0.13f, 0.16f, 0.17f));
            Widgets.Label(
                new Rect(
                    expandedDialog.x + 8f,
                    expandedDialog.y + 5f,
                    expandedDialog.width - 30f,
                    Text.LineHeight * LineScale
                ),
                "TKUtils.Headers.DataDialog".Localize(expanded.Name)
            );

            Widgets.DrawHighlight(
                new Rect(expandedDialog.position, new Vector2(expandedDialog.width, Text.LineHeight * LineScale))
            );

            GUI.BeginGroup(expandedDialog.ContractedBy(StandardMargin * 2f));
            DrawKindSettings(
                new Rect(
                    0f,
                    0f,
                    expandedDialog.width - StandardMargin * 4f,
                    expandedDialog.height - StandardMargin * 4f
                )
            );
            GUI.EndGroup();

            if (Widgets.CloseButtonFor(expandedDialog))
            {
                CloseExpandedMenu();
            }
        }

        private void DrawKindSettings(Rect inRect)
        {
            expanded.Data ??= new PawnKindData {KarmaType = pawnKarmaType};
            string removeKarma = expanded.Data.KarmaType == null
                ? noCustomKarmaText
                : Enum.GetName(typeof(KarmaType), expanded.Data.KarmaType);


            var listing = new Listing_Standard();
            listing.Begin(inRect);

            (Rect nameLabel, Rect nameField) = listing.GetRect(Text.LineHeight * LineScale).ToForm(0.45f);
            expandedName = Widgets.TextField(nameField, expandedName).ToToolkit();

            if (expandedName.Length > 0 && SettingsHelper.DrawClearButton(nameField))
            {
                expandedName = "";
                expanded.Name = expanded.GetDefaultName();
                expanded.Data!.CustomName = false;
            }

            SettingsHelper.DrawLabelAnchored(nameLabel, nameText, TextAnchor.MiddleLeft);

            listing.Gap(4f);
            (Rect karmaTypeLabel, Rect karmaTypeField) = listing.GetRect(Text.LineHeight * LineScale).ToForm(0.45f);
            SettingsHelper.DrawLabelAnchored(karmaTypeLabel, karmaTypeText, TextAnchor.MiddleLeft);
            if (Widgets.ButtonText(karmaTypeField, removeKarma))
            {
                Find.WindowStack.Add(
                    new FloatMenu(
                        StoreDialog.KarmaTypes.Select(
                                k => new FloatMenuOption(
                                    Enum.GetName(typeof(KarmaType), k),
                                    () => expanded.Data.KarmaType = k
                                )
                            )
                           .ToList()
                    )
                );
            }

            var lastLine = new Rect(
                0f,
                inRect.height - Text.LineHeight * LineScale,
                inRect.width,
                Text.LineHeight * LineScale
            );

            if (Widgets.ButtonText(
                new Rect(lastLine.x + 10f, lastLine.y, lastLine.width - 20f, lastLine.height),
                resetText
            ))
            {
                expanded.Name = expanded.GetDefaultName();
                expanded.Data!.CustomName = false;
                expanded.Data!.KarmaType = null;
            }

            listing.End();
        }

        private void Notify__SearchRequested()
        {
            lastQuery = currentQuery;

            results = GetSearchResults();
            SortCurrentWorkingList();
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
            if (expanded.Name.NullOrEmpty())
            {
                expanded.Name = expanded.GetDefaultName();

                if (expanded.Data != null)
                {
                    expanded.Data.CustomName = false;
                }
            }

            expanded = null;
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

        private List<PawnKindItem> GetSearchResults()
        {
            string serialized = currentQuery?.ToToolkit();

            if (serialized == null)
            {
                return null;
            }

            List<PawnKindItem> searchResults = cache.Where(
                    t => t.DefName.ToToolkit().EqualsIgnoreCase(currentQuery.ToToolkit())
                         || t.DefName.ToToolkit().Contains(currentQuery.ToToolkit())
                )
               .ToList();

            return searchResults;
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
                                Data.SavePawnKinds(Paths.PawnKindFilePath);
                                return;
                            case "SingleFile":
                                Data.SaveLegacyShop(Paths.LegacyShopDumpFilePath);
                                return;
                        }
                    }
                );
            }
            else
            {
                switch (TkSettings.DumpStyle)
                {
                    case "MultiFile":
                        Data.SavePawnKinds(Paths.PawnKindFilePath);
                        return;
                    case "SingleFile":
                        Data.SaveLegacyShop(Paths.LegacyShopDumpFilePath);
                        return;
                }
            }
        }

        private void SortCurrentWorkingList()
        {
            List<PawnKindItem> workingList = results ?? cache;

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
                case Sorter.Cost:
                    switch (sortMode)
                    {
                        case SortMode.Ascending:
                            workingList.SortBy(i => i.Cost);
                            results = workingList;
                            return;
                        case SortMode.Descending:
                            workingList.SortByDescending(i => i.Cost);
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
