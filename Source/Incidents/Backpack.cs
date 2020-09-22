using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
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
    public class Backpack : IncidentHelperVariables
    {
        private PurchaseBackpackRequest purchaseRequest;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            string[] segments = CommandFilter.Parse(message).Skip(2).ToArray();
            string item = segments.FirstOrFallback();
            string quantity = segments.Skip(1).FirstOrFallback("1");

            if (item.NullOrEmpty() || (!CommandBase.GetOrFindPawn(viewer.username)?.Spawned ?? true))
            {
                return false;
            }

            if (!int.TryParse(quantity, out int amount))
            {
                amount = 1;
            }

            ThingItem product = Data.Items.FirstOrDefault(i => i.Name.EqualsIgnoreCase(item));

            if (product == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".Localize(item));
                return false;
            }

            if (product.Price <= 0)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Item.Disabled".Localize(product.Name));
                return false;
            }

            if (product.Thing == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".Localize(item));
                return false;
            }

            if (product.Thing.race != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoAnimals".Localize());
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

            if (product.Data != null && product.Data.QuantityLimit > 0 && product.Data.QuantityLimit < amount)
            {
                amount = product.Data.QuantityLimit;
            }

            purchaseRequest = new PurchaseBackpackRequest
            {
                ItemData = product, ThingDef = product.Thing, Quantity = amount, Purchaser = viewer
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

            if (purchaseRequest.Price > viewer.GetViewerCoins())
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

            return true;
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
                TkLogger.Warn($"Backpack failed to execute with error message: {e.Message}");
            }
        }
    }

    internal class PurchaseBackpackRequest
    {
        private Pawn pawn;

        public int Price => Mathf.Clamp(ItemData.Price * Quantity, 0, int.MaxValue);
        public int Quantity { get; set; }
        public ThingItem ItemData { get; set; }
        public ThingDef ThingDef { get; set; }
        public Viewer Purchaser { get; set; }
        public Pawn Pawn => pawn ??= CommandBase.GetOrFindPawn(Purchaser.username);

        public void Spawn()
        {
            ThingDef result = null;
            if (ThingDef.MadeFromStuff
                && !GenStuff.AllowedStuffsFor(ThingDef)
                   .Where(t => Data.ItemData.TryGetValue(t.defName, out ItemData data) && data.IsStuffAllowed)
                   .Where(t => !PawnWeaponGenerator.IsDerpWeapon(ThingDef, t))
                   .TryRandomElementByWeight(t => t.stuffProps.commonality, out result))
            {
                TkLogger.Warn("Could not generate stuff for item! Falling back to a random stuff...");
                result = GenStuff.RandomStuffByCommonalityFor(ThingDef);
            }

            Thing thing = ThingMaker.MakeThing(ThingDef, result);

            if (thing.TryGetQuality(out QualityCategory _))
            {
                ItemHelper.setItemQualityRandom(thing);
            }

            if (ThingDef.Minifiable)
            {
                ThingDef minifiedDef = ThingDef.minifiedDef;
                var minifiedThing = (MinifiedThing) ThingMaker.MakeThing(minifiedDef);
                minifiedThing.InnerThing = thing;
                minifiedThing.stackCount = Quantity;
                CarryOrSpawn(minifiedThing);
            }
            else
            {
                thing.stackCount = Quantity;
                CarryOrSpawn(thing);
            }

            Find.LetterStack.ReceiveLetter(
                (Quantity > 1 ? ItemData.Name.Pluralize() : ItemData.Name).Truncate(15, true).CapitalizeFirst(),
                "TKUtils.BackpackLetter.Description".Localize(
                    Quantity.ToString("N0"),
                    Quantity > 1 ? ItemData.Name.Pluralize() : ItemData.Name,
                    Purchaser.username
                ),
                ItemHelper.GetLetterFromValue(Price),
                thing
            );
        }

        private void CarryOrSpawn(Thing thing)
        {
            if (!Pawn.inventory.innerContainer.TryAdd(thing))
            {
                TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(Pawn.Map), Pawn.Map, thing);
            }
        }

        public void CompletePurchase(StoreIncident incident)
        {
            if (!ToolkitSettings.UnlimitedCoins)
            {
                Purchaser.TakeViewerCoins(Price);
            }

            Purchaser.CalculateNewKarma(ItemData.Data?.KarmaType ?? incident.karmaType, Price);


            if (!ToolkitSettings.PurchaseConfirmations)
            {
                return;
            }

            Notify_ItemPurchaseComplete();
        }

        private void Notify_ItemPurchaseComplete()
        {
            if (TkSettings.BuyItemBalance)
            {
                MessageHelper.ReplyToUser(
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
                MessageHelper.ReplyToUser(
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
