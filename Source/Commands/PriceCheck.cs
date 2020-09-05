using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PriceCheck : CommandBase
    {
        private ITwitchMessage msg;

        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            msg = twitchMessage;
            string[] segments = CommandFilter.Parse(twitchMessage.Message).Skip(1).ToArray();
            string category = segments.FirstOrFallback("");
            string query = segments.Skip(1).FirstOrFallback("");
            string quantity = segments.Skip(2).FirstOrFallback("1");

            if (!Lookup.Index.TryGetValue(category.ToLowerInvariant(), out string result))
            {
                quantity = query;
                query = category;
                category = "items";
            }

            if (result != null && (result.Equals("diseases") || result.Equals("skills")))
            {
                quantity = query;
                query = category;
                category = "items";
            }

            PerformLookup(category, query, quantity);
        }

        private void Notify__LookupComplete(string query, string result)
        {
            msg.Reply("TKUtils.Lookup".Localize(query, result));
        }

        private void PerformAnimalLookup(string query, int quantity)
        {
            PawnKindDef result = DefDatabase<PawnKindDef>.AllDefsListForReading.FirstOrDefault(
                i => i.RaceProps.Animal
                     && (i.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                         || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit()))
            );

            if (result == null)
            {
                return;
            }

            Item item = StoreInventory.items.FirstOrDefault(i => i.defname.EqualsIgnoreCase(result.defName));

            if (item == null || item.price <= 0)
            {
                return;
            }

            Notify__LookupComplete(
                query,
                "TKUtils.Price.Quantity".Localize(
                    result.defName.CapitalizeFirst(),
                    item.price.ToString("N0"),
                    item.CalculatePrice(quantity).ToString("N0"),
                    quantity
                )
            );
        }

        private void PerformEventLookup(string query)
        {
            StoreIncident result = DefDatabase<StoreIncident>.AllDefsListForReading.FirstOrDefault(
                i => i.cost > 0
                     && (i.abbreviation.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                         || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit()))
            );

            if (result == null)
            {
                return;
            }

            EventTypes eventType = result.GetModExtension<EventExtension>()?.EventType ?? EventTypes.None;
            switch (eventType)
            {
                case EventTypes.None:
                    Notify__LookupComplete(
                        query,
                        "TKUtils.Price.Limited".Localize(
                            result.abbreviation.CapitalizeFirst(),
                            result.cost.ToString("N0")
                        )
                    );
                    return;
                default:
                    Notify__LookupComplete(query, "TKUtils.Price.Overridden".Localize(eventType.ToString()));
                    return;
            }
        }

        private void PerformItemLookup(string query, int quantity)
        {
            ThingItem result = Data.Items.FirstOrDefault(
                i => i.Price > 0
                     && (i.Name.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                         || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit()))
            );

            if (result == null)
            {
                return;
            }

            Notify__LookupComplete(
                query,
                "TKUtils.Price.Quantity".Localize(
                    result.Name.CapitalizeFirst(),
                    result.Price.ToString("N0"),
                    result.Item.CalculatePrice(quantity).ToString("N0"),
                    quantity
                )
            );
        }

        private void PerformLookup(string category, string query, string amount)
        {
            if (!int.TryParse(amount, out int quantity))
            {
                quantity = 1;
            }

            if (!Lookup.Index.TryGetValue(category.ToLowerInvariant(), out string result))
            {
                return;
            }

            switch (result)
            {
                case "events":
                    PerformEventLookup(query);
                    return;
                case "items":
                    PerformItemLookup(query, quantity);
                    return;
                case "animals":
                    PerformAnimalLookup(query, quantity);
                    return;
                case "traits":
                    PerformTraitLookup(query);
                    return;
                case "races":
                    PerformKindLookup(query);
                    return;
            }
        }

        private void PerformKindLookup(string query)
        {
            PawnKindItem result = Data.PawnKinds.FirstOrDefault(
                i => i.Name.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                     || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
            );

            if (result == null)
            {
                return;
            }

            Notify__LookupComplete(
                query,
                "TKUtils.Price.Limited".Localize(result.Name.ToToolkit().CapitalizeFirst(), result.Cost.ToString("N0"))
            );
        }

        private void PerformTraitLookup(string query)
        {
            TraitItem result = Data.Traits.FirstOrDefault(
                i => i.Name.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                     || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
            );

            if (result == null)
            {
                return;
            }

            if (!result.CanAdd && !result.CanRemove)
            {
                return;
            }

            var parts = new List<string>();

            if (result.CanAdd)
            {
                parts.Add("TKUtils.Price.AddTrait".Localize(result.CostToAdd.ToString("N0")));
            }

            if (result.CanRemove)
            {
                parts.Add("TKUtils.Price.RemoveTrait".Localize(result.CostToRemove.ToString("N0")));
            }

            Notify__LookupComplete(query, $"{result.Name.ToToolkit().CapitalizeFirst()} - {parts.Join(" / ")}");
        }
    }
}
