using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class LookupCommand : CommandBase
    {
        internal static readonly Dictionary<string, string> Index = new Dictionary<string, string>
        {
            {"item", "items"},
            {"items", "items"},
            {"incident", "events"},
            {"incidents", "events"},
            {"event", "events"},
            {"events", "events"},
            {"pawn", "races"},
            {"pawns", "races"},
            {"race", "races"},
            {"races", "races"},
            {"disease", "diseases"},
            {"diseases", "diseases"},
            {"animal", "animals"},
            {"animals", "animals"},
            {"skill", "skills"},
            {"skills", "skills"},
            {"trait", "traits"},
            {"traits", "traits"},
        };

        private ITwitchMessage msg;

        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            msg = twitchMessage;
            var segments = CommandFilter.Parse(twitchMessage.Message).Skip(1).ToArray();
            var category = segments.FirstOrFallback("");
            var query = segments.Skip(1).FirstOrFallback("");

            if (!Index.TryGetValue(category.ToLowerInvariant(), out var _))
            {
                query = category;
                category = "items";
            }

            PerformLookup(category, query);
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
            if (!Index.TryGetValue(category.ToLowerInvariant(), out var result))
            {
                return;
            }

            switch (result)
            {
                case "diseases":
                    PerformDiseaseLookup(query);
                    return;
                case "skills":
                    PerformSkillLookup(query);
                    return;
                case "events":
                    PerformEventLookup(query);
                    return;
                case "items":
                    PerformItemLookup(query);
                    return;
                case "animals":
                    PerformAnimalLookup(query);
                    return;
                case "traits":
                    PerformTraitLookup(query);
                    return;
                case "races":
                    PerformRaceLookup(query);
                    return;
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
                .Select(i => i.Name.ToToolkit().CapitalizeFirst())
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
                .Select(i => i.Name.ToToolkit().CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }
    }
}
