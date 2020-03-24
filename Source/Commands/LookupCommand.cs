using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.Traits;
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

            PerformLookup(segments.FirstOrDefault(), segments.Skip(1).FirstOrDefault());
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
                        if (i.LabelCap.RawText.Contains(query))
                        {
                            return true;
                        }

                        if (i.LabelCap.RawText.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        var t = i.LabelCap.RawText.ToToolkit();

                        if (t.Contains(query))
                        {
                            return true;
                        }

                        if (t.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        if (i.defName.ToLower().Contains(query))
                        {
                            return true;
                        }

                        return i.defName.EqualsIgnoreCase(query);
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
                        if (i.LabelCap.RawText.Contains(query))
                        {
                            return true;
                        }

                        if (i.LabelCap.RawText.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        var t = i.LabelCap.RawText.ToToolkit();

                        return t.Contains(query) || t.EqualsIgnoreCase(query);
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
                        if (i.abbreviation.Contains(query))
                        {
                            return true;
                        }

                        if (i.abbreviation.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        var t = i.abbreviation.ToToolkit();

                        if (t.Contains(query))
                        {
                            return true;
                        }

                        if (t.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        return i.defName.ToLower().Contains(query) || i.defName.EqualsIgnoreCase(query);
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
                        if (i.abr.Contains(query))
                        {
                            return true;
                        }

                        if (i.abr.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        var t = i.abr.ToToolkit();

                        if (t.Contains(query))
                        {
                            return true;
                        }

                        if (t.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        return i.defname.ToLower().Contains(query) || i.defname.EqualsIgnoreCase(query);
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
            var results = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(i => i.RaceProps.Humanlike)
                .Where(
                    i =>
                    {
                        if (i.LabelCap.RawText.Contains(query))
                        {
                            return true;
                        }

                        if (i.LabelCap.RawText.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        var t = i.LabelCap.RawText.ToToolkit();

                        if (t.Contains(query))
                        {
                            return true;
                        }

                        if (t.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        return i.defName.ToLower().Contains(query) || i.defName.EqualsIgnoreCase(query);
                    }
                )
                .GroupBy(i => i.race.defName)
                .Select(i => i.Key.CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformSkillLookup(string query)
        {
            var results = DefDatabase<SkillDef>.AllDefsListForReading
                .Where(
                    i =>
                    {
                        if (i.LabelCap.RawText.Contains(query))
                        {
                            return true;
                        }

                        if (i.LabelCap.RawText.EqualsIgnoreCase(query))
                        {
                            return true;
                        }

                        var t = i.LabelCap.RawText.ToToolkit();

                        return t.Contains(query) || t.EqualsIgnoreCase(query);
                    }
                )
                .Select(i => i.defName.ToLower().CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformTraitLookup(string query)
        {
            var results = AllTraits.buyableTraits
                .Where(
                    i => i.label.ToLower().Contains(query)
                         || i.label.EqualsIgnoreCase(query)
                         || i.def.defName.ToLower().Contains(query)
                         || i.def.defName.EqualsIgnoreCase(query)
                )
                .Select(i => i.label.CapitalizeFirst())
                .ToArray();

            Notify__LookupComplete(query, results);
        }
    }
}
