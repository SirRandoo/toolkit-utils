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

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class BuyItem : IncidentVariablesBase
    {
        private PurchaseRequest purchaseRequest;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsItem(out ArgWorker.ItemProxy product) || !product.IsValid())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".LocalizeKeyed(worker.GetLast()));
                return false;
            }

            if (product.TryGetError(out string error))
            {
                MessageHelper.ReplyToUser(viewer.username, error);
                return false;
            }

            int amount = product.Thing.ItemData?.HasQuantityLimit == true
                ? worker.GetNextAsInt(1, product.Thing.ItemData.QuantityLimit)
                : worker.GetNextAsInt(1);

            List<ResearchProjectDef> projects = product!.Thing.Thing.GetUnfinishedPrerequisites();
            if (BuyItemSettings.mustResearchFirst && projects.Count > 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ResearchRequired".LocalizeKeyed(
                        product.Thing.Thing.LabelCap.RawText,
                        projects.Select(p => p.LabelCap.RawText).SectionJoin()
                    )
                );
                return false;
            }

            if (worker.GetLast()?.Equals("*") ?? false)
            {
                amount = viewer.GetMaximumPurchaseAmount(product.Thing.Cost);
            }

            purchaseRequest = new PurchaseRequest
            {
                Proxy = product, Quantity = amount, Purchaser = viewer, Map = Helper.AnyPlayerMap
            };

            if (purchaseRequest.Price < ToolkitSettings.MinimumPurchasePrice)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Item.MinimumViolation".LocalizeKeyed(
                        purchaseRequest.Price.ToString("N0"),
                        ToolkitSettings.MinimumPurchasePrice.ToString("N0")
                    )
                );
                return false;
            }

            if (viewer.CanAfford(purchaseRequest.Price))
            {
                return purchaseRequest.Map != null;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.InsufficientBalance".LocalizeKeyed(
                    purchaseRequest.Price.ToString("N0"),
                    viewer.GetViewerCoins().ToString("N0")
                )
            );
            return false;
        }

        public override void Execute()
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
        public int Price =>
            (Proxy.Quality.HasValue
                ? Proxy.Thing.GetItemPrice(Proxy.Stuff, Proxy.Quality.Value)
                : Proxy.Thing.GetItemPrice(Proxy.Stuff))
            * Quantity;

        public int Quantity { get; set; }
        public ArgWorker.ItemProxy Proxy { get; set; }
        public Viewer Purchaser { get; set; }
        public Map Map { get; set; }

        public void Spawn()
        {
            if (Proxy.Thing.Thing.race != null)
            {
                SpawnAnimal();
                return;
            }

            SpawnItem();
        }

        private void SpawnAnimal()
        {
            if (Proxy.Thing.Thing.race.Humanlike)
            {
                // ReSharper disable once StringLiteralTypo
                LogHelper.Warn("Tried to spawn a humanlike -- Humanlikes should be spawned via !buy pawn");
                return;
            }

            string animal = Proxy.Thing.Thing.label.CapitalizeFirst();

            if (Quantity > 1)
            {
                animal = animal.Pluralize();
            }

            var coordinator = Current.Game.GetComponent<Coordinator>();

            if (coordinator?.HasActivePortals == true)
            {
                for (var _ = 0; _ < Quantity; _++)
                {
                    var request = PawnGenerationRequest.MakeDefault();
                    request.KindDef = PawnKindDef.Named(Proxy.Thing.Thing.defName);
                    request.FixedGender = Proxy.Gender;
                    request.Faction = Faction.OfPlayer;
                    coordinator.TrySpawnAnimal(Map, PawnGenerator.GeneratePawn(request));
                }

                return;
            }

            var worker = new AnimalSpawnWorker
            {
                Label = "TKUtils.ItemLetter.Animal".LocalizeKeyed(animal),
                AnimalDef = PawnKindDef.Named(Proxy.Thing.Thing.defName),
                Gender = Proxy.Gender,
                def = IncidentDef.Named("FarmAnimalsWanderIn"),
                Quantity = Quantity,
                SpawnTamed = true
            };

            worker.TryExecute(StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, Map));
        }

        private void SpawnItem()
        {
            ThingDef result = Proxy.Stuff?.Thing;
            if (Proxy.Thing.Thing.CanBeStuff(Proxy.Stuff?.Thing) != true)
            {
                result = GenStuff.RandomStuffByCommonalityFor(Proxy.Thing.Thing);
            }

            Thing thing = PurchaseHelper.MakeThing(Proxy.Thing.Thing, result, Proxy.Quality);
            IntVec3 position = DropCellFinder.TradeDropSpot(Map);

            if (Proxy.Thing.Thing.Minifiable)
            {
                ThingDef minifiedDef = Proxy.Thing.Thing.minifiedDef;
                var minifiedThing = (MinifiedThing) ThingMaker.MakeThing(minifiedDef);
                minifiedThing.InnerThing = thing;
                minifiedThing.stackCount = Quantity;
                PurchaseHelper.SpawnItem(position, Map, minifiedThing);
            }
            else
            {
                thing.stackCount = Quantity;
                PurchaseHelper.SpawnItem(position, Map, thing);
            }

            Find.LetterStack.ReceiveLetter(
                (Quantity > 1 ? Proxy.Thing.Name.Pluralize() : Proxy.Thing.Name).Truncate(15, true).CapitalizeFirst(),
                "TKUtils.ItemLetter.ItemDescription".LocalizeKeyed(
                    Quantity.ToString("N0"),
                    Proxy.AsString(Quantity > 1),
                    Purchaser.username
                ),
                ItemHelper.GetLetterFromValue(Price),
                thing
            );
        }

        public void CompletePurchase(StoreIncident incident)
        {
            Purchaser.Charge(
                Price,
                Proxy.Thing.ItemData?.Weight ?? 1f,
                Proxy.Thing.Data?.KarmaType ?? incident.karmaType
            );

            if (Proxy.Thing.Thing.race != null)
            {
                NotifyAnimalPurchaseComplete();
            }
            else
            {
                NotifyItemPurchaseComplete();
            }
        }

        private void NotifyAnimalPurchaseComplete()
        {
            if (TkSettings.BuyItemBalance)
            {
                MessageHelper.SendConfirmation(
                    Purchaser.username,
                    "TKUtils.Item.Complete".LocalizeKeyed(
                        Quantity.ToString("N0"),
                        Quantity > 1 ? Proxy.Thing.Thing.label.Pluralize() : Proxy.Thing.Thing.label,
                        Price.ToString("N0"),
                        Purchaser.GetViewerCoins().ToString("N0")
                    )
                );
            }
            else
            {
                MessageHelper.SendConfirmation(
                    Purchaser.username,
                    "TKUtils.Item.CompleteMinimal".LocalizeKeyed(
                        Quantity.ToString("N0"),
                        Quantity > 1 ? Proxy.Thing.Thing.label.Pluralize() : Proxy.Thing.Thing.label,
                        Price.ToString("N0")
                    )
                );
            }
        }

        private void NotifyItemPurchaseComplete()
        {
            if (TkSettings.BuyItemBalance)
            {
                MessageHelper.SendConfirmation(
                    Purchaser.username,
                    "TKUtils.Item.Complete".LocalizeKeyed(
                        Quantity.ToString("N0"),
                        Proxy.AsString(Quantity > 1),
                        Price.ToString("N0"),
                        Purchaser.GetViewerCoins().ToString("N0")
                    )
                );
            }
            else
            {
                MessageHelper.SendConfirmation(
                    Purchaser.username,
                    "TKUtils.Item.CompleteMinimal".LocalizeKeyed(
                        Quantity.ToString("N0"),
                        Proxy.AsString(Quantity > 1),
                        Price.ToString("N0")
                    )
                );
            }
        }
    }
}
