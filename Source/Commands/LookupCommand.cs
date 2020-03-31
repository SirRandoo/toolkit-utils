using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.IRC;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class LookupCommand : CommandBase
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

            PerformLookup(segments.FirstOrFallback(""), segments.Skip(1).FirstOrFallback(""));
        }

        private void Notify__LookupComplete(string query, IReadOnlyCollection<string> results)
        {
            if (results.Count <= 0)
            {
                return;
            }

            var formatted = string.Join(", ", results.Take(TkSettings.LookupLimit));

            msg.Reply("TKUtils.Formats.Lookup".Translate(query, formatted));
        }

        private void PerformAnimalLookup(string query)
        {
            var results = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(i => i.RaceProps.Animal)
                .Where(
                    i =>
                    {
                        var label = i.LabelCap.RawText.ToToolkit();
                        var q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.defName.ToToolkit().Contains(query.ToToolkit())
                               || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Select(i => i.defName.CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformDiseaseLookup(string query)
        {
            var results = DefDatabase<IncidentDef>.AllDefsListForReading
                .Where(i => i.category == IncidentCategoryDefOf.DiseaseHuman)
                .Where(
                    i =>
                    {
                        var label = i.LabelCap.RawText.ToToolkit();
                        var q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.defName.ToToolkit().Contains(query.ToToolkit())
                               || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Select(i => i.LabelCap.RawText.ToToolkit().CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformEventLookup(string query)
        {
            var results = DefDatabase<StoreIncident>.AllDefsListForReading
                .Where(i => i.cost > 0)
                .Where(
                    i =>
                    {
                        var label = i.abbreviation.ToToolkit();
                        var q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.defName.ToToolkit().Contains(query.ToToolkit())
                               || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Select(i => i.abbreviation.ToToolkit().CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformItemLookup(string query)
        {
            var results = StoreInventory.items
                .Where(i => i.price > 0)
                .Where(
                    i =>
                    {
                        var label = i.abr.ToToolkit();
                        var q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.defname.ToToolkit().Contains(query.ToToolkit())
                               || i.defname.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Select(i => i.abr.ToToolkit().CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformLookup(string category, string query)
        {
            if (category.EqualsIgnoreCase("disease") || category.EqualsIgnoreCase("diseases"))
            {
                PerformDiseaseLookup(query);
            }
            else if (category.EqualsIgnoreCase("skill") || category.EqualsIgnoreCase("skills"))
            {
                PerformSkillLookup(query);
            }
            else if (category.EqualsIgnoreCase("event") || category.EqualsIgnoreCase("events"))
            {
                PerformEventLookup(query);
            }
            else if (category.EqualsIgnoreCase("item") || category.EqualsIgnoreCase("items"))
            {
                PerformItemLookup(query);
            }
            else if (category.EqualsIgnoreCase("animal") || category.EqualsIgnoreCase("animals"))
            {
                PerformAnimalLookup(query);
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
            var results = TkUtils.ShopExpansion.Races
                .Where(
                    i =>
                    {
                        var label = i.Name.ToToolkit();
                        var q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.DefName.ToToolkit().Contains(query.ToToolkit())
                               || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Select(i => i.Name.CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformSkillLookup(string query)
        {
            var results = DefDatabase<SkillDef>.AllDefsListForReading
                .Where(
                    i =>
                    {
                        var label = i.LabelCap.RawText.ToToolkit();
                        var q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.defName.ToToolkit().Contains(query.ToToolkit())
                               || i.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Select(i => i.defName.ToLower().CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformTraitLookup(string query)
        {
            var results = TkUtils.ShopExpansion.Traits
                .Where(
                    i =>
                    {
                        var label = i.Name.ToToolkit();
                        var q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.DefName.ToToolkit().Contains(query.ToToolkit())
                               || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Select(i => i.Name.CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }
    }
}
