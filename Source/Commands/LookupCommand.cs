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
        private bool separateChannel;
        private string viewer;

        public override void RunCommand(IRCMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            viewer = message.User;
            separateChannel = CommandsHandler.SendToChatroom(message);

            var segments = CommandParser.Parse(message.Message, prefix: TKSettings.Prefix).Skip(1).ToArray();

            PerformLookup(segments.FirstOrDefault(), segments.Skip(1).FirstOrDefault());
        }

        private void Notify__LookupComplete(string query, string[] results, int limit = 10)
        {
            if(limit < 0 || results.Length <= 0)
            {
                return;
            }

            var formatted = string.Join("TKUtils.Misc.Separators.Inner".Translate(), results.Take(limit));

            SendCommandMessage(
                viewer,
                "TKUtils.Formats.Lookup.Base".Translate(
                    query.Named("QUERY"),
                    formatted.Named("RESULTS")
                ),
                separateChannel
            );
        }

        private void PerformAnimalLookup(string query)
        {
            var results = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(i => i.RaceProps.Animal)
                .Where(i =>
                {
                    if(i.LabelCap.RawText.Contains(query))
                    {
                        return true;
                    }

                    if(i.LabelCap.RawText.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    var t = i.LabelCap.RawText.ToLower().Replace(" ", "");

                    if(t.Contains(query))
                    {
                        return true;
                    }

                    if(t.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    if(i.defName.ToLower().Contains(query))
                    {
                        return true;
                    }

                    if(i.defName.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    return false;
                })
                .Select(i => GenText.CapitalizeFirst(i.defName))
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformDiseaseLookup(string query)
        {
            var results = DefDatabase<IncidentDef>.AllDefsListForReading
                .Where(i => i.category == IncidentCategoryDefOf.DiseaseHuman)
                .Where(i =>
                {
                    if(i.LabelCap.RawText.Contains(query))
                    {
                        return true;
                    }

                    if(i.LabelCap.RawText.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    var t = i.LabelCap.RawText.Replace(" ", "").ToLower();

                    if(t.Contains(query))
                    {
                        return true;
                    }

                    if(t.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    return false;
                }).Select(i => GenText.CapitalizeFirst(i.LabelCap.RawText.Replace(" ", "").ToLower()))
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformEventLookup(string query)
        {
            var results = DefDatabase<StoreIncident>.AllDefsListForReading
                .Where(i => i.cost > 0)
                .Where(i =>
                {
                    if(i.abbreviation.Contains(query))
                    {
                        return true;
                    }

                    if(i.abbreviation.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    var t = i.abbreviation.Replace(" ", "").ToLower();

                    if(t.Contains(query))
                    {
                        return true;
                    }

                    if(t.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    if(i.defName.ToLower().Contains(query))
                    {
                        return true;
                    }

                    if(i.defName.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    return false;
                }).Select(i => GenText.CapitalizeFirst(i.abbreviation.Replace(" ", "").ToLower()))
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformItemLookup(string query)
        {
            var results = StoreInventory.items
                .Where(i => i.price > 0)
                .Where(i =>
                {
                    if(i.abr.Contains(query))
                    {
                        return true;
                    }

                    if(i.abr.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    var t = i.abr.Replace(" ", "").ToLower();

                    if(t.Contains(query))
                    {
                        return true;
                    }

                    if(t.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    if(i.defname.ToLower().Contains(query))
                    {
                        return true;
                    }

                    if(i.defname.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    return false;
                }).Select(i => GenText.CapitalizeFirst(i.abr.Replace(" ", "").ToLower()))
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformLookup(string category, string query)
        {
            if(category.EqualsIgnoreCase("disease") || category.EqualsIgnoreCase("diseases"))
            {
                PerformDiseaseLookup(query);
            }
            else if(category.EqualsIgnoreCase("skill") || category.EqualsIgnoreCase("skills"))
            {
                PerformSkillLookup(query);
            }
            else if(category.EqualsIgnoreCase("event") || category.EqualsIgnoreCase("events"))
            {
                PerformEventLookup(query);
            }
            else if(category.EqualsIgnoreCase("item") || category.EqualsIgnoreCase("items"))
            {
                PerformItemLookup(query);
            }
            else if(category.EqualsIgnoreCase("animal") || category.EqualsIgnoreCase("animals"))
            {
                PerformAnimalLookup(query);
            }
            else if(category.EqualsIgnoreCase("trait") || category.EqualsIgnoreCase("traits"))
            {
                PerformTraitLookup(query);
            }
            else if(category.EqualsIgnoreCase("race") || category.EqualsIgnoreCase("races"))
            {
                PerformRaceLookup(query);
            }
        }

        private void PerformRaceLookup(string query)
        {
            var results = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(i => i.RaceProps.Humanlike)
                .Where(i =>
                {
                    if(i.LabelCap.RawText.Contains(query))
                    {
                        return true;
                    }

                    if(i.LabelCap.RawText.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    var t = i.LabelCap.RawText.ToLower().Replace(" ", "");

                    if(t.Contains(query))
                    {
                        return true;
                    }

                    if(t.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    if(i.defName.ToLower().Contains(query))
                    {
                        return true;
                    }

                    if(i.defName.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    return false;
                })
                .GroupBy(i => i.race.defName)
                .Select(i => GenText.CapitalizeFirst(i.Key))
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformSkillLookup(string query)
        {
            var results = DefDatabase<SkillDef>.AllDefsListForReading
                .Where(i =>
                {
                    if(i.LabelCap.RawText.Contains(query))
                    {
                        return true;
                    }

                    if(i.LabelCap.RawText.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    var t = i.LabelCap.RawText.Replace(" ", "").ToLower();

                    if(t.Contains(query))
                    {
                        return true;
                    }

                    if(t.EqualsIgnoreCase(query))
                    {
                        return true;
                    }

                    return false;
                }).Select(i => GenText.CapitalizeFirst(i.defName.ToLower()))
                .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformTraitLookup(string query)
        {
            var results = AllTraits.buyableTraits
                .Where(i => i.label.ToLower().Contains(query) || i.label.EqualsIgnoreCase(query) || i.def.defName.ToLower().Contains(query) || i.def.defName.EqualsIgnoreCase(query))
                .Select(i => GenText.CapitalizeFirst(i.label))
                .ToArray();

            Notify__LookupComplete(query, results);
        }
    }
}
