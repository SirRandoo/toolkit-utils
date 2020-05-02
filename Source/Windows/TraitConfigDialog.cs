using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class TraitConfigDialog : Window
    {
        private readonly List<XmlTrait> cache = TkUtils.ShopExpansion.Traits;
        private string currentQuery = "";
        private int globalAddCost;
        private int globalRemoveCost;
        private string lastQuery = "";
        private IReadOnlyCollection<XmlTrait> results;
        private Vector2 scrollPos = Vector2.zero;

        public TraitConfigDialog()
        {
            doCloseX = true;
            globalRemoveCost = 0;
            forcePause = true;

            optionalTitle = "TKUtils.Windows.Traits.Title".Translate();
            cache?.SortBy(t => t.DefName);

            if (cache == null)
            {
                TkLogger.Warn("The trait shop is null! You should report this.");
            }
        }

        public override Vector2 InitialSize => new Vector2(640f, Screen.height * 0.85f);

        private void DrawHeader(Rect inRect)
        {
            var midpoint = inRect.width / 2f;
            var searchRect = new Rect(inRect.x, inRect.y, inRect.width * 0.3f, Text.LineHeight);
            var searchLabel = searchRect.LeftHalf();
            var searchField = searchRect.RightHalf();

            Widgets.Label(searchLabel, "TKUtils.Windows.Config.Buttons.Search.Label".Translate());
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


            var enableText = "TKUtils.Windows.Config.Buttons.EnableAll.Label".Translate();
            var disableText = "TKUtils.Windows.Config.Buttons.DisableAll.Label".Translate();
            var applyText = "TKUtils.Windows.Config.Buttons.Apply.Label".Translate();
            var enableSize = Text.CalcSize(enableText) * 1.5f;
            var disableSize = Text.CalcSize(disableText) * 1.5f;
            var maxWidth = Mathf.Max(enableSize.x, disableSize.x);
            var disableRect = new Rect(midpoint, inRect.y + Text.LineHeight, maxWidth, Text.LineHeight);
            var enableRect = new Rect(midpoint, inRect.y, maxWidth, Text.LineHeight);

            if (Widgets.ButtonText(enableRect, globalAddCost > 0 ? applyText : enableText))
            {
                foreach (var trait in TkUtils.ShopExpansion.Traits)
                {
                    if (globalAddCost > 0)
                    {
                        trait.AddPrice = globalAddCost;
                    }
                    else
                    {
                        trait.CanAdd = true;
                        trait.CanRemove = true;
                    }
                }

                if (globalAddCost > 0)
                {
                    globalAddCost = 0;
                }
            }

            if (Widgets.ButtonText(disableRect, globalRemoveCost > 0 ? applyText : disableText))
            {
                foreach (var trait in TkUtils.ShopExpansion.Traits)
                {
                    if (globalRemoveCost > 0)
                    {
                        trait.RemovePrice = globalRemoveCost;
                    }
                    else
                    {
                        trait.CanAdd = false;
                        trait.CanRemove = false;
                    }
                }

                if (globalRemoveCost > 0)
                {
                    globalRemoveCost = 0;
                }
            }


            var globalAddRect = new Rect(
                enableRect.x + enableRect.width + 5f,
                enableRect.y,
                midpoint - enableRect.width - 5f,
                Text.LineHeight
            );
            var globalRemoveRect = new Rect(
                disableRect.x + disableRect.width + 5f,
                disableRect.y,
                midpoint - disableRect.width - 5f,
                Text.LineHeight
            );

            var globalAddBuffer = globalAddCost.ToString();
            Widgets.Label(globalAddRect.LeftHalf(), "TKUtils.Windows.Traits.AddPrice.Label".Translate());
            Widgets.TextFieldNumeric(globalAddRect.RightHalf(), ref globalAddCost, ref globalAddBuffer);

            if (globalAddCost > 0 && SettingsHelper.DrawClearButton(globalAddRect.RightHalf()))
            {
                globalAddCost = 0;
            }

            var globalRemoveBuffer = globalRemoveCost.ToString();
            Widgets.Label(globalRemoveRect.LeftHalf(), "TKUtils.Windows.Traits.RemovePrice.Label".Translate());
            Widgets.TextFieldNumeric(globalRemoveRect.RightHalf(), ref globalRemoveCost, ref globalRemoveBuffer);

            if (globalRemoveCost > 0 && SettingsHelper.DrawClearButton(globalRemoveRect.RightHalf()))
            {
                globalRemoveCost = 0;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard {maxOneColumn = true};

            DrawHeader(inRect);
            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 3f, inRect.width);

            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 4f,
                inRect.width,
                inRect.height - Text.LineHeight * 3f
            );

            var old = Text.Anchor;
            var total = results?.Count ?? cache.Count;
            var maxHeight = Text.LineHeight * total * 3 - 1;
            var viewPort = new Rect(contentArea.x, 0f, contentArea.width * 0.9f, maxHeight);

            listing.BeginScrollView(contentArea, ref scrollPos, ref viewPort);
            foreach (var trait in results ?? cache)
            {
                var lineRect = listing.GetRect(Text.LineHeight * 2);
                var labelRect = new Rect(lineRect.x, lineRect.y, lineRect.width * 0.6f, lineRect.height);

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, trait.Name.CapitalizeFirst());
                Text.Anchor = old;

                var inputRect = new Rect(
                    labelRect.x + labelRect.width + 5f,
                    lineRect.y,
                    lineRect.width - labelRect.width - 35f,
                    labelRect.height
                );

                Widgets.Checkbox(inputRect.x, inputRect.y, ref trait.CanAdd);

                var addRect = new Rect(
                    inputRect.x + 28f,
                    inputRect.y,
                    inputRect.width,
                    Text.LineHeight
                );

                if (trait.CanAdd)
                {
                    var addBuffer = trait.AddPrice.ToString();
                    Widgets.TextFieldNumericLabeled(
                        addRect,
                        "TKUtils.Windows.Traits.AddPrice.Label".Translate(),
                        ref trait.AddPrice,
                        ref addBuffer
                    );
                }
                else
                {
                    Widgets.Label(addRect.LeftHalf(), "TKUtils.Windows.Traits.AddPrice.Label".Translate());

                    Text.Anchor = TextAnchor.MiddleRight;
                    Widgets.Label(addRect.RightHalf(), trait.AddPrice.ToString());

                    Text.Anchor = old;
                }

                var removeRect = new Rect(
                    inputRect.x + 28f,
                    inputRect.BottomHalf().y,
                    inputRect.width,
                    Text.LineHeight
                );

                Widgets.Checkbox(removeRect.x - 28f, removeRect.y, ref trait.CanRemove);

                if (trait.CanRemove)
                {
                    var removeBuffer = trait.RemovePrice.ToString();
                    Widgets.TextFieldNumericLabeled(
                        removeRect,
                        "TKUtils.Windows.Traits.RemovePrice.Label".Translate(),
                        ref trait.RemovePrice,
                        ref removeBuffer
                    );
                }
                else
                {
                    Widgets.Label(removeRect.LeftHalf(), "TKUtils.Windows.Traits.RemovePrice.Label".Translate());

                    Text.Anchor = TextAnchor.MiddleRight;
                    Widgets.Label(removeRect.RightHalf(), trait.RemovePrice.ToString());

                    Text.Anchor = old;
                }

                listing.GapLine();
            }

            listing.EndScrollView(ref viewPort);
        }

        private void Notify__SearchRequested()
        {
            lastQuery = currentQuery;

            results = GetSearchResults();
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

        private IReadOnlyCollection<XmlTrait> GetSearchResults()
        {
            var serialized = currentQuery?.ToToolkit();

            if (serialized == null)
            {
                return null;
            }

            return cache
                .Where(
                    t => t.DefName.ToToolkit().EqualsIgnoreCase(currentQuery.ToToolkit())
                         || t.DefName.ToToolkit().Contains(currentQuery.ToToolkit())
                )
                .ToList();
        }

        public override void PreClose()
        {
            ShopExpansionHelper.SaveData(TkUtils.ShopExpansion, ShopExpansionHelper.ExpansionFile);

            if (TkSettings.JsonShop)
            {
                ShopExpansionHelper.DumpShopExpansion();
            }
        }
    }
}
