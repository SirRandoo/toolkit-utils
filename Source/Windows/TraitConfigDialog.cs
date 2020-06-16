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

        private bool control;
        private string currentQuery = "";
        private int globalAddCost;
        private int globalRemoveCost;
        private string lastQuery = "";
        private List<XmlTrait> results;
        private Vector2 scrollPos = Vector2.zero;
        private bool shift;

        private Sorter sorter = Sorter.Name;
        private SortMode sortMode = SortMode.Ascending;

        private TaggedString titleText;
        // private TaggedString nameHeaderText;
        // private TaggedString priceText;
        // private TaggedString priceHeaderText;

        public TraitConfigDialog()
        {
            GetTranslations();

            doCloseX = true;
            globalRemoveCost = 0;
            forcePause = true;
            onlyOneOfTypeAllowed = true;

            optionalTitle = titleText;
            cache?.SortBy(t => t.Name);

            if (cache == null)
            {
                TkLogger.Warn("The trait shop is null! You should report this.");
            }
        }

        public override Vector2 InitialSize => new Vector2(640f, Screen.height * 0.85f);

        private void GetTranslations()
        {
            titleText = "TKUtils.Windows.Races.Title".Translate();
            // nameHeaderText = "TKUtils.Windows.Store.Headers.Name".Translate();
            // priceText = "TKUtils.Windows.Config.Input.Price.Label".Translate();
            // priceHeaderText = "TKUtils.Windows.Store.Headers.Price".Translate();
            // applyText = "TKUtils.Windows.Config.Buttons.Apply.Label".Translate();
            // searchText = "TKUtils.Windows.Config.Buttons.Search.Label".Translate();
            // resetText = "TKUtils.Windows.Config.Buttons.ResetAll.Label".Translate();
            // enableText = "TKUtils.Windows.Config.Buttons.EnableAll.Label".Translate();
            // disableText = "TKUtils.Windows.Config.Buttons.DisableAll.Label".Translate();
        }

        private void DrawHeader(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            float midpoint = inRect.width / 2f;
            var searchRect = new Rect(inRect.x, inRect.y, inRect.width * 0.3f, Text.LineHeight);
            Rect searchLabel = searchRect.LeftHalf();
            Rect searchField = searchRect.RightHalf();

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


            TaggedString enableText = "TKUtils.Windows.Config.Buttons.EnableAll.Label".Translate();
            TaggedString disableText = "TKUtils.Windows.Config.Buttons.DisableAll.Label".Translate();
            TaggedString applyText = "TKUtils.Windows.Config.Buttons.Apply.Label".Translate();
            Vector2 enableSize = Text.CalcSize(enableText) * 1.5f;
            Vector2 disableSize = Text.CalcSize(disableText) * 1.5f;
            float maxWidth = Mathf.Max(enableSize.x, disableSize.x);
            var disableRect = new Rect(midpoint, inRect.y + Text.LineHeight, maxWidth, Text.LineHeight);
            var enableRect = new Rect(midpoint, inRect.y, maxWidth, Text.LineHeight);

            if (Widgets.ButtonText(enableRect, globalAddCost > 0 ? applyText : enableText))
            {
                foreach (XmlTrait trait in TkUtils.ShopExpansion.Traits)
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
                foreach (XmlTrait trait in TkUtils.ShopExpansion.Traits)
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

            GUI.EndGroup();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            var listing = new Listing_Standard {maxOneColumn = true};
            var headerRect = new Rect(0f, 0f, inRect.width, Text.LineHeight * 2f);

            DrawHeader(headerRect);
            Widgets.DrawLineHorizontal(inRect.x, Text.LineHeight * 3f, inRect.width);

            var contentArea = new Rect(
                inRect.x,
                Text.LineHeight * 4f,
                inRect.width,
                inRect.height - Text.LineHeight * 4f
            );

            TextAnchor old = Text.Anchor;
            List<XmlTrait> effectiveList = results ?? cache;
            int total = effectiveList.Count;
            float maxHeight = (Text.LineHeight * 3f + 2f) * total;
            var viewPort = new Rect(0f, 0f, contentArea.width - 16f, maxHeight);
            var traits = new Rect(0f, 0f, contentArea.width, contentArea.height);

            GUI.BeginGroup(contentArea);
            listing.BeginScrollView(traits, ref scrollPos, ref viewPort);

            for (var index = 0; index < effectiveList.Count; index++)
            {
                XmlTrait trait = effectiveList[index];
                Rect lineRect = listing.GetRect(Text.LineHeight * 2f + 2f);
                Rect spacerRect = listing.GetRect(Text.LineHeight);
                var labelRect = new Rect(0f, lineRect.y, lineRect.width * 0.6f, lineRect.height);

                if (!lineRect.IsRegionVisible(traits, scrollPos))
                {
                    continue;
                }

                if (index % 2 == 1)
                {
                    Widgets.DrawLightHighlight(
                        new Rect(
                            lineRect.x,
                            lineRect.y - Text.LineHeight / 2f,
                            lineRect.width,
                            lineRect.height + Text.LineHeight
                        )
                    );
                }

                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, trait.Name.CapitalizeFirst());
                Text.Anchor = old;

                var inputRect = new Rect(
                    labelRect.width + 5f,
                    lineRect.y,
                    lineRect.width - labelRect.width - 35f,
                    labelRect.height
                );

                Widgets.Checkbox(inputRect.x, inputRect.y, ref trait.CanAdd);

                var addRect = new Rect(
                    inputRect.x + 28f,
                    inputRect.y,
                    inputRect.width,
                    Text.LineHeight - 1f
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
                    inputRect.BottomHalf().y + 1f,
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

                Color colorCache = GUI.color;
                GUI.color = Color.gray;
                Widgets.DrawLineHorizontal(spacerRect.x, spacerRect.y + spacerRect.height / 2f, spacerRect.width);
                GUI.color = colorCache;
            }

            GUI.EndGroup();
            listing.EndScrollView(ref viewPort);
            GUI.EndGroup();
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

        private List<XmlTrait> GetSearchResults()
        {
            string serialized = currentQuery?.ToToolkit();

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
