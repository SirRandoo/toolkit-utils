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
using TwitchToolkit.IncidentHelpers.Special;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class Backpack : IncidentVariablesBase
    {
        private PurchaseBackpackRequest _purchaseRequest;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!PurchaseHelper.TryGetPawn(viewer.username, out Pawn pawn) || !pawn!.Spawned)
            {
                return false;
            }

            if (!worker.TryGetNextAsItem(out ArgWorker.ItemProxy proxy) || !proxy.IsValid() || proxy!.Thing.Thing.race != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".LocalizeKeyed(worker.GetLast()));

                return false;
            }

            if (proxy.TryGetError(out string error))
            {
                MessageHelper.ReplyToUser(viewer.username, error);

                return false;
            }

            if (!worker.TryGetNextAsInt(out int amount, 1, viewer.GetMaximumPurchaseAmount(proxy!.Thing.Cost)))
            {
                amount = 1;
            }

            if (PurchaseHelper.TryGetUnfinishedPrerequisites(proxy.Thing.Thing, out List<ResearchProjectDef> projects))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.ResearchRequired".LocalizeKeyed(proxy.Thing.Thing!.LabelCap.RawText, projects.Select(p => p.LabelCap.RawText).SectionJoin()));

                return false;
            }

            if (worker.GetLast().Equals("*"))
            {
                amount = viewer.GetMaximumPurchaseAmount(proxy.Thing.Cost);
            }

            if (proxy.Thing.ItemData?.HasQuantityLimit == true)
            {
                amount = Mathf.Clamp(amount, 1, proxy.Thing.ItemData.QuantityLimit);
            }

            _purchaseRequest = new PurchaseBackpackRequest
            {
                Proxy = proxy, Quantity = amount, Purchaser = viewer, Pawn = pawn
            };

            if (_purchaseRequest.Price < ToolkitSettings.MinimumPurchasePrice)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Item.MinimumViolation".LocalizeKeyed(_purchaseRequest.Price.ToString("N0"), ToolkitSettings.MinimumPurchasePrice.ToString("N0"))
                );

                return false;
            }

            if (_purchaseRequest.Overflowed)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Overflowed".Localize());

                return false;
            }

            if (viewer.CanAfford(_purchaseRequest.Price))
            {
                return _purchaseRequest.Map != null;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".LocalizeKeyed(_purchaseRequest.Price.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

            return false;
        }

        public override void Execute()
        {
            try
            {
                _purchaseRequest.Spawn();
                _purchaseRequest.CompletePurchase(storeIncident);
            }
            catch (Exception e)
            {
                LogHelper.Warn($"Backpack failed to execute with error message: {e.Message}");
            }
        }

        private class PurchaseBackpackRequest
        {
            private Pawn _pawn;

            public int Quantity { get; set; }
            public ArgWorker.ItemProxy Proxy { get; set; }

            public int Price
            {
                get
                {
                    int price = Proxy.Quality.HasValue ? Proxy.Thing.GetItemPrice(Proxy.Stuff, Proxy.Quality.Value) : Proxy.Thing.GetItemPrice(Proxy.Stuff);

                    if (!Overflowed && PurchaseHelper.TryMultiply(price, Quantity, out int result))
                    {
                        return result;
                    }

                    Overflowed = true;

                    return int.MaxValue;
                }
            }

            public bool Overflowed { get; private set; }

            public Viewer Purchaser { get; set; }
            [CanBeNull] public Map Map => Pawn?.Map;

            [CanBeNull]
            public Pawn Pawn
            {
                get => _pawn ??= CommandBase.GetOrFindPawn(Purchaser.username);
                set => _pawn = value;
            }

            public void Spawn()
            {
                ThingDef result = Proxy.Stuff?.Thing;

                if (Proxy.Thing.Thing.CanBeStuff(Proxy.Stuff?.Thing) != true)
                {
                    result = GenStuff.RandomStuffByCommonalityFor(Proxy.Thing.Thing);
                }

                Thing thing = PurchaseHelper.MakeThing(Proxy.Thing.Thing, result, Proxy.Quality);

                if (Proxy.Thing.Thing.Minifiable)
                {
                    ThingDef minifiedDef = Proxy.Thing.Thing.minifiedDef;
                    var minifiedThing = (MinifiedThing)ThingMaker.MakeThing(minifiedDef);
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
                    (Quantity > 1 ? Proxy.Thing.Name.Pluralize() : Proxy.Thing.Name).Truncate(15, true).CapitalizeFirst(),
                    "TKUtils.BackpackLetter.Description".LocalizeKeyed(Quantity.ToString("N0"), Proxy.AsString(Quantity > 1), Purchaser.username),
                    ItemHelper.GetLetterFromValue(Price),
                    thing
                );
            }

            private void CarryOrSpawn(Thing thing)
            {
                if (!MassUtility.CanEverCarryAnything(_pawn) || MassUtility.WillBeOverEncumberedAfterPickingUp(_pawn, thing, Quantity))
                {
                    PurchaseHelper.SpawnItem(DropCellFinder.TradeDropSpot(Map), Map, thing);

                    return;
                }

                _pawn.inventory.innerContainer.TryAdd(thing);
            }

            public void CompletePurchase(StoreIncident incident)
            {
                Purchaser.Charge(Price, Proxy.Thing.ItemData?.Weight ?? 1f, Proxy.Thing.Data?.KarmaType ?? incident.karmaType);
                NotifyItemPurchaseComplete();
            }

            private void NotifyItemPurchaseComplete()
            {
                if (TkSettings.BuyItemBalance)
                {
                    MessageHelper.SendConfirmation(
                        Purchaser.username,
                        "TKUtils.Item.Complete".LocalizeKeyed(
                            Quantity.ToString("N0"),
                            Quantity > 1 ? Proxy.Thing.Name.Pluralize() : Proxy.Thing.Name,
                            Price.ToString("N0"),
                            Purchaser.GetViewerCoins().ToString("N0")
                        )
                    );
                }
                else
                {
                    MessageHelper.SendConfirmation(
                        Purchaser.username,
                        "TKUtils.Item.CompleteMinimal".LocalizeKeyed(Quantity.ToString("N0"), Quantity > 1 ? Proxy.Thing.Name.Pluralize() : Proxy.Thing.Name, Price.ToString("N0"))
                    );
                }
            }
        }
    }
}
