using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class BuyItem : IncidentHelperVariables
    {
        private PurchaseRequest purchaseRequest;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            string[] segments = CommandFilter.Parse(message).Skip(2).ToArray();
            string item = segments.FirstOrFallback();
            string quantity = segments.Skip(1).FirstOrFallback("1");

            if (item.NullOrEmpty())
            {
                return false;
            }

            if (!int.TryParse(quantity, out int amount) || amount < 0)
            {
                amount = 1;
            }

            ThingItem product = Data.Items.Where(i => i.Price > 0).FirstOrDefault(i => i.Name.EqualsIgnoreCase(item));

            if (product?.Thing == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".Localize(item));
                return false;
            }

            List<ResearchProjectDef> projects = product.Thing.GetUnfinishedPrerequisites();
            if (BuyItemSettings.mustResearchFirst && projects.Count > 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ResearchRequired".Localize(
                        product.Thing.LabelCap.RawText,
                        projects.Select(p => p.LabelCap.RawText).SectionJoin()
                    )
                );
                return false;
            }

            if (quantity.Equals("*"))
            {
                amount = viewer.GetMaximumPurchaseAmount(product.Price);
            }

            if (product.Data != null && product.Data.HasQuantityLimit)
            {
                amount = Mathf.Clamp(amount, 1, product.Data.QuantityLimit);
            }

            purchaseRequest = new PurchaseRequest
            {
                ItemData = product,
                ThingDef = product.Thing,
                Quantity = amount,
                Purchaser = viewer,
                Map = Helper.AnyPlayerMap
            };

            if (purchaseRequest.Price < ToolkitSettings.MinimumPurchasePrice)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Item.MinimumViolation".Localize(
                        purchaseRequest.Price.ToString("N0"),
                        ToolkitSettings.MinimumPurchasePrice.ToString("N0")
                    )
                );
                return false;
            }

            if (!viewer.CanAfford(purchaseRequest.Price))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        purchaseRequest.Price.ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            return purchaseRequest.Map != null;
        }

        public override void TryExecute()
        {
            try
            {
                purchaseRequest.Spawn();
                purchaseRequest.CompletePurchase(storeIncident);
            }
            catch (Exception e)
            {
                LogHelper.Warn($"Buy item failed to execute with error message: {e.Message}");
            }
        }
    }

    internal class PurchaseRequest
    {
        public int Price => Mathf.Clamp(ItemData.Price * Quantity, 0, int.MaxValue);
        public int Quantity { get; set; }
        public ThingItem ItemData { get; set; }
        public ThingDef ThingDef { get; set; }
        public Viewer Purchaser { get; set; }
        public Map Map { get; set; }

        public void Spawn()
        {
            if (ThingDef.race != null)
            {
                SpawnAnimal();
                return;
            }

            SpawnItem();
        }

        private void SpawnAnimal()
        {
            if (ThingDef.race.Humanlike)
            {
                LogHelper.Warn("Tried to spawn a humanlike -- Humanlikes should be spawned via !buy pawn");
                return;
            }

            string animal = ThingDef.label.CapitalizeFirst();

            if (Quantity > 1)
            {
                animal = animal.Pluralize();
            }

            var worker = new IncidentWorker_SpecificAnimalsWanderIn(
                "TKUtils.ItemLetter.Animal".Localize(Quantity > 1 ? animal.Pluralize() : animal),
                PawnKindDef.Named(ThingDef.defName),
                true,
                Quantity,
                false,
                true
            ) {def = IncidentDef.Named("FarmAnimalsWanderIn")};

            worker.TryExecute(StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, Map));
        }

        private void SpawnItem()
        {
            ThingDef result = null;
            if (ThingDef.MadeFromStuff
                && !GenStuff.AllowedStuffsFor(ThingDef)
                   .Where(t => Data.ItemData.TryGetValue(t.defName, out ItemData data) && data.IsStuffAllowed)
                   .Where(t => !PawnWeaponGenerator.IsDerpWeapon(ThingDef, t))
                   .TryRandomElementByWeight(t => t.stuffProps.commonality, out result))
            {
                LogHelper.Warn("Could not generate stuff for item! Falling back to a random stuff...");
                result = GenStuff.RandomStuffByCommonalityFor(ThingDef);
            }

            Thing thing = ThingMaker.MakeThing(ThingDef, result);

            if (thing.TryGetQuality(out QualityCategory _))
            {
                ItemHelper.setItemQualityRandom(thing);
            }

            IntVec3 position = DropCellFinder.TradeDropSpot(Map);

            if (ThingDef.Minifiable)
            {
                ThingDef minifiedDef = ThingDef.minifiedDef;
                var minifiedThing = (MinifiedThing) ThingMaker.MakeThing(minifiedDef);
                minifiedThing.InnerThing = thing;
                minifiedThing.stackCount = Quantity;
                TradeUtility.SpawnDropPod(position, Map, minifiedThing);
            }
            else
            {
                thing.stackCount = Quantity;
                TradeUtility.SpawnDropPod(position, Map, thing);
            }

            Find.LetterStack.ReceiveLetter(
                (Quantity > 1 ? ItemData.Name.Pluralize() : ItemData.Name).Truncate(15, true).CapitalizeFirst(),
                "TKUtils.ItemLetter.ItemDescription".Localize(
                    Quantity.ToString("N0"),
                    Quantity > 1 ? ItemData.Name.Pluralize() : ItemData.Name,
                    Purchaser.username
                ),
                ItemHelper.GetLetterFromValue(Price),
                thing
            );
        }

        public void CompletePurchase(StoreIncident incident)
        {
            Purchaser.Charge(Price, ItemData.Data?.Weight ?? 1f, ItemData.Data?.KarmaType ?? incident.karmaType);

            if (ThingDef.race != null)
            {
                Notify_AnimalPurchaseComplete();
            }
            else
            {
                Notify_ItemPurchaseComplete();
            }
        }

        private void Notify_AnimalPurchaseComplete()
        {
            if (TkSettings.BuyItemBalance)
            {
                MessageHelper.SendConfirmation(
                    Purchaser.username,
                    "TKUtils.Item.Complete".Localize(
                        Quantity.ToString("N0"),
                        Quantity > 1 ? ThingDef.label.Pluralize() : ThingDef.label,
                        Price.ToString("N0"),
                        Purchaser.GetViewerCoins().ToString("N0")
                    )
                );
            }
            else
            {
                MessageHelper.SendConfirmation(
                    Purchaser.username,
                    "TKUtils.Item.CompleteMinimal".Localize(
                        Quantity.ToString("N0"),
                        Quantity > 1 ? ThingDef.label.Pluralize() : ThingDef.label,
                        Price.ToString("N0")
                    )
                );
            }
        }

        private void Notify_ItemPurchaseComplete()
        {
            if (TkSettings.BuyItemBalance)
            {
                MessageHelper.SendConfirmation(
                    Purchaser.username,
                    "TKUtils.Item.Complete".Localize(
                        Quantity.ToString("N0"),
                        Quantity > 1 ? ItemData.Name.Pluralize() : ItemData.Name,
                        Price.ToString("N0"),
                        Purchaser.GetViewerCoins().ToString("N0")
                    )
                );
            }
            else
            {
                MessageHelper.SendConfirmation(
                    Purchaser.username,
                    "TKUtils.Item.CompleteMinimal".Localize(
                        Quantity.ToString("N0"),
                        Quantity > 1 ? ItemData.Name.Pluralize() : ItemData.Name,
                        Price.ToString("N0")
                    )
                );
            }
        }
    }
}
