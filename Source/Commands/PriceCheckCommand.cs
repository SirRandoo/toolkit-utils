using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.IRC;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PriceCheckCommand : CommandBase
    {
        private IRCMessage msg;

        public override void RunCommand(IRCMessage message)
        {
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            msg = message;

            var segments = CommandParser.Parse(message.Message, TkSettings.Prefix).Skip(1).ToArray();

            PerformLookup(
                segments.FirstOrFallback(""),
                segments.Skip(1).FirstOrFallback(""),
                segments.Skip(2).FirstOrFallback("")
            );
        }

        private void Notify__LookupComplete(string query, string result)
        {
            msg.Reply("TKUtils.Formats.Lookup".Translate(query, result));
        }

        private void PerformAnimalLookup(string query, int quantity)
        {
            var result = DefDatabase<PawnKindDef>.AllDefsListForReading
                .FirstOrDefault(
                    i => i.RaceProps.Animal
                         && (i.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                             || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit()))
                );

            if (result == null)
            {
                return;
            }

            var item = StoreInventory.items.FirstOrDefault(i => i.defname.EqualsIgnoreCase(result.defName));

            if (item == null || item.price <= 0)
            {
                return;
            }

            Notify__LookupComplete(
                query,
                "TKUtils.Formats.PriceCheck.Quantity".Translate(
                    result.defName.CapitalizeFirst(),
                    item.price.ToString("N0"),
                    item.CalculatePrice(quantity).ToString("N0"),
                    quantity
                )
            );
        }

        private void PerformEventLookup(string query)
        {
            var result = DefDatabase<StoreIncident>.AllDefsListForReading
                .FirstOrDefault(
                    i => i.cost > 0
                         && (i.abbreviation.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                             || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit()))
                );

            if (result == null)
            {
                return;
            }

            Notify__LookupComplete(
                query,
                "TKUtils.Formats.PriceCheck.Limited".Translate(
                    result.abbreviation.CapitalizeFirst(),
                    result.cost.ToString("N0")
                )
            );
        }

        private void PerformItemLookup(string query, int quantity)
        {
            var result = StoreInventory.items
                .FirstOrDefault(
                    i => i.price > 0
                         && (i.abr.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                             || i.defname.ToToolkit().EqualsIgnoreCase(query.ToToolkit()))
                );

            if (result == null)
            {
                return;
            }

            Notify__LookupComplete(
                query,
                "TKUtils.Formats.PriceCheck.Quantity".Translate(
                    result.abr.CapitalizeFirst(),
                    result.price.ToString("N0"),
                    result.CalculatePrice(quantity).ToString("N0"),
                    quantity
                )
            );
        }

        private void PerformLookup(string category, string query, string amount)
        {
            if (!int.TryParse(amount, out var quantity))
            {
                quantity = 1;
            }

            if (category.EqualsIgnoreCase("event") || category.EqualsIgnoreCase("events"))
            {
                PerformEventLookup(query);
            }
            else if (category.EqualsIgnoreCase("item") || category.EqualsIgnoreCase("items"))
            {
                PerformItemLookup(query, quantity);
            }
            else if (category.EqualsIgnoreCase("animal") || category.EqualsIgnoreCase("animals"))
            {
                PerformAnimalLookup(query, quantity);
            }
            else if (category.EqualsIgnoreCase("trait") || category.EqualsIgnoreCase("traits"))
            {
                PerformTraitLookup(query);
            }
            else if (category.EqualsIgnoreCase("race") || category.EqualsIgnoreCase("races"))
            {
                PerformRaceLookup(query);
            }
        }

        private void PerformRaceLookup(string query)
        {
            var result = TkUtils.ShopExpansion.Races
                .FirstOrDefault(
                    i => i.Name.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                         || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                );

            if (result == null)
            {
                return;
            }

            Notify__LookupComplete(
                query,
                "TKUtils.Formats.PriceCheck.Limited".Translate(
                    result.Name.CapitalizeFirst(),
                    result.Price.ToString("N0")
                )
            );
        }

        private void PerformTraitLookup(string query)
        {
            var result = TkUtils.ShopExpansion.Traits.FirstOrDefault(
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
                parts.Add("TKUtils.Responses.PriceCheck.Trait.Add".Translate(result.AddPrice.ToString("N0")));
            }

            if (result.CanRemove)
            {
                parts.Add("TKUtils.Responses.PriceCheck.Trait.Remove".Translate(result.RemovePrice.ToString("N0")));
            }

            Notify__LookupComplete(
                query,
                $"{result.Name.CapitalizeFirst()} - {string.Join(" / ", parts.ToArray())}"
            );
        }
    }
}
