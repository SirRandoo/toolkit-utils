using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
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
            {"stat", "stats"},
            {"stats", "stats"}
        };

        private ITwitchMessage msg;

        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            msg = twitchMessage;
            string[] segments = CommandFilter.Parse(twitchMessage.Message).Skip(1).ToArray();
            string category = segments.FirstOrFallback("");
            string query = segments.Skip(1).FirstOrFallback("");

            if (!Index.TryGetValue(category.ToLowerInvariant(), out string _))
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

            string formatted = results.Take(TkSettings.LookupLimit).SectionJoin();

            msg.Reply("TKUtils.Formats.Lookup".Translate(query, formatted));
        }

        private void PerformAnimalLookup(string query)
        {
            string[] results = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(i => i.RaceProps.Animal)
                .Where(
                    i =>
                    {
                        string label = i.LabelCap.RawText.ToToolkit();
                        string q = query.ToToolkit();

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
            string[] results = DefDatabase<IncidentDef>.AllDefsListForReading
                .Where(i => i.category == IncidentCategoryDefOf.DiseaseHuman)
                .Where(
                    i =>
                    {
                        string label = i.LabelCap.RawText.ToToolkit();
                        string q = query.ToToolkit();

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
            string[] results = DefDatabase<StoreIncident>.AllDefsListForReading
                .Where(i => i.cost > 0)
                .Where(
                    i =>
                    {
                        string label = i.abbreviation.ToToolkit();
                        string q = query.ToToolkit();

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
            string[] results = StoreInventory.items
                .Where(i => i.price > 0)
                .Where(
                    i =>
                    {
                        string label = i.abr.ToToolkit();
                        string q = query.ToToolkit();

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
            if (!Index.TryGetValue(category.ToLowerInvariant(), out string result))
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
                case "stats":
                    PerformStatLookup(query);
                    return;
            }
        }

        private void PerformRaceLookup(string query)
        {
            string[] results = TkUtils.ShopExpansion.Races
                .Where(
                    i =>
                    {
                        string label = i.Name.ToToolkit();
                        string q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.DefName.ToToolkit().Contains(query.ToToolkit())
                               || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Where(r => r.Enabled)
                .Select(i => i.Name.ToToolkit().CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformSkillLookup(string query)
        {
            string[] results = DefDatabase<SkillDef>.AllDefsListForReading
                .Where(
                    i =>
                    {
                        string label = i.LabelCap.RawText.ToToolkit();
                        string q = query.ToToolkit();

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
            string[] results = TkUtils.ShopExpansion.Traits
                .Where(
                    i =>
                    {
                        string label = i.Name.ToToolkit();
                        string q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.DefName.ToToolkit().Contains(query.ToToolkit())
                               || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
                .Where(t => t.CanAdd || t.CanRemove)
                .Select(i => i.Name.ToToolkit().CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformStatLookup(string query)
        {
            List<string> results = PawnStatsCommand.StatRegistry.Keys
                .Where(i => i.EqualsIgnoreCase(query.ToToolkit()) || i.Contains(query.ToToolkit()))
                .Select(i => PawnStatsCommand.StatRegistry[i].ToToolkit().CapitalizeFirst())
                .ToList();

            for (int index = results.Count - 1; index >= 0; index--)
            {
                string result = results[index];

                if (results.Count(i => i.EqualsIgnoreCase(result)) <= 1)
                {
                    continue;
                }

                try
                {
                    results.RemoveAt(index);
                }
                catch (IndexOutOfRangeException)
                {
                }
            }

            Notify__LookupComplete(query, results.ToArray());
        }
    }
}
