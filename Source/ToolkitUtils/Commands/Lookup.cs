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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class Lookup : CommandBase
    {
        internal static readonly Dictionary<string, string> Index;
        private ITwitchMessage msg;

        static Lookup()
        {
            Index = new Dictionary<string, string>
            {
                {"item", "items"},
                {"items", "items"},
                {"incident", "events"},
                {"incidents", "events"},
                {"event", "events"},
                {"events", "events"},
                {"pawn", "kinds"},
                {"pawns", "kinds"},
                {"race", "kinds"},
                {"races", "kinds"},
                {"kinds", "kinds"},
                {"kind", "kinds"},
                {"pawnkinds", "kinds"},
                {"disease", "diseases"},
                {"diseases", "diseases"},
                {"animal", "animals"},
                {"animals", "animals"},
                {"skill", "skills"},
                {"skills", "skills"},
                {"trait", "traits"},
                {"traits", "traits"}
            };
        }

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

            msg.Reply("TKUtils.Lookup".Localize(query, formatted));
        }

        private void PerformAnimalLookup(string query)
        {
            string[] results = Data.Items.Where(i => i.Thing.race.Animal)
               .Where(i => i.Price > 0)
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
               .Select(i => i.Name.CapitalizeFirst())
               .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformDiseaseLookup(string query)
        {
            string[] results = DefDatabase<IncidentDef>.AllDefs
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
            string[] results = DefDatabase<StoreIncident>.AllDefs.Where(i => i.cost > 0)
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
            string[] results = Data.Items.Where(i => i.Price > 0)
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
               .Select(i => i.Name.ToToolkit().CapitalizeFirst())
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
                case "kinds":
                    PerformKindLookup(query);
                    return;
            }
        }

        private void PerformKindLookup(string query)
        {
            string[] results = Data.PawnKinds.Where(i => i.Enabled)
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
               .Select(i => i.Name.ToToolkit().CapitalizeFirst())
               .ToArray();

            Notify__LookupComplete(query, results);
        }

        private void PerformSkillLookup(string query)
        {
            string[] results = DefDatabase<SkillDef>.AllDefs.Where(
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
            string[] results = Data.Traits.Where(t => t.CanAdd || t.CanRemove)
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
               .Select(i => i.Name.ToToolkit().CapitalizeFirst())
               .ToArray();

            Notify__LookupComplete(query, results);
        }
    }
}
