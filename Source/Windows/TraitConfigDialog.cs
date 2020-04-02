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
        private string lastQuery = "";
        private IReadOnlyCollection<XmlTrait> results;
        private Vector2 scrollPos = Vector2.zero;

        public TraitConfigDialog()
        {
            doCloseX = true;
            forcePause = true;

            optionalTitle = "TKUtils.Windows.Traits.Title".Translate();
            cache?.SortBy(t => t.DefName);

            if (cache == null)
            {
                Logger.Warn("The trait shop is null! You should report this.");
            }
        }

        public override Vector2 InitialSize => new Vector2(640f, 740f);

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard {maxOneColumn = true};
            var searchRect = new Rect(inRect.x, inRect.y, inRect.width * 0.3f - 28f, Text.LineHeight);
            var clearRect = new Rect(searchRect.x + searchRect.width + 2f, searchRect.y, 28f, Text.LineHeight);

            currentQuery = Widgets.TextEntryLabeled(
                searchRect,
                "TKUtils.Windows.Config.Buttons.Search.Label".Translate(),
                currentQuery
            );

            var old = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;

            if (Widgets.ButtonText(clearRect, "X"))
            {
                currentQuery = "";
            }

            if (currentQuery == "")
            {
                results = null;
                lastQuery = "";
            }

            Text.Anchor = old;
            var enableText = "TKUtils.Windows.Config.Buttons.EnableAll.Label".Translate();
            var disableText = "TKUtils.Windows.Config.Buttons.DisableAll.Label".Translate();
            var enableSize = Text.CalcSize(enableText);
            var disableSize = Text.CalcSize(disableText);
            var disableRect = new Rect(
                inRect.width - disableSize.x * 1.5f,
                inRect.y,
                disableSize.x * 1.5f,
                Text.LineHeight
            );
            var enableRect = new Rect(
                inRect.width - disableSize.x * 1.5f - enableSize.x * 1.5f,
                inRect.y,
                enableSize.x * 1.5f,
                Text.LineHeight
            );

            if (Widgets.ButtonText(enableRect, enableText))
            {
                foreach (var trait in TkUtils.ShopExpansion.Traits)
                {
                    trait.CanAdd = true;
                    trait.CanRemove = true;
                }
            }

            if (Widgets.ButtonText(disableRect, disableText))
            {
                foreach (var trait in TkUtils.ShopExpansion.Traits)
                {
                    trait.CanAdd = false;
                    trait.CanRemove = false;
                }
            }

            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 2f, inRect.width);

            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 3f,
                inRect.width,
                inRect.height - Text.LineHeight * 3f
            );

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

            var searchResults = cache.Where(
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
                ShopExpansionHelper.DumpShopExtension();
            }
        }
    }
}
