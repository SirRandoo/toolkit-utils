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
    [UsedImplicitly]
    public class Lookup : CommandBase
    {
        public enum Category
        {
            Item,
            Event,
            Kind,
            Disease,
            Animal,
            Skill,
            Trait,
            Mod
        }

        internal static readonly Dictionary<string, Category> Index = new Dictionary<string, Category>
        {
            { "item", Category.Item },
            { "items", Category.Item },
            { "incident", Category.Event },
            { "incidents", Category.Event },
            { "event", Category.Event },
            { "events", Category.Event },
            { "pawn", Category.Kind },
            { "pawns", Category.Kind },
            { "race", Category.Kind },
            { "races", Category.Kind },
            { "kinds", Category.Kind },
            { "kind", Category.Kind },
            { "pawnkinds", Category.Kind },
            { "disease", Category.Disease },
            { "diseases", Category.Disease },
            { "animal", Category.Animal },
            { "animals", Category.Animal },
            { "skill", Category.Skill },
            { "skills", Category.Skill },
            { "trait", Category.Trait },
            { "traits", Category.Trait },
            { "mods", Category.Mod },
            { "mod", Category.Mod }
        };

        private ITwitchMessage _msg;

        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            _msg = twitchMessage;
            string[] segments = CommandFilter.Parse(twitchMessage.Message).Skip(1).ToArray();
            string category = segments.FirstOrFallback("");
            string query = segments.Skip(1).FirstOrFallback("");

            if (!Index.TryGetValue(category.ToLowerInvariant(), out Category c))
            {
                query = category;
                c = Category.Item;
            }

            PerformLookup(c, query);
        }

        private void NotifyLookupComplete(string query, [NotNull] IReadOnlyCollection<string> results)
        {
            if (results.Count <= 0)
            {
                return;
            }

            _msg.Reply("TKUtils.Lookup".LocalizeKeyed(query, results.Take(TkSettings.LookupLimit).SectionJoin()));
        }

        private void PerformAnimalLookup(string query)
        {
            string[] results = Data.Items.Where(i => i.Thing.race.Animal)
               .Where(i => i.Cost > 0)
               .Where(
                    i =>
                    {
                        if (i.DefName == null || i.Name == null)
                        {
                            return false;
                        }

                        string q = query.ToToolkit();

                        return i.Name.ToToolkit().Contains(q) || i.DefName.ToToolkit().Contains(q);
                    }
                )
               .Select(i => i.Name.CapitalizeFirst())
               .ToArray();

            NotifyLookupComplete(query, results);
        }

        private void PerformDiseaseLookup(string query)
        {
            string[] results = DefDatabase<IncidentDef>.AllDefs.Where(i => i.category == IncidentCategoryDefOf.DiseaseHuman)
               .Where(
                    i =>
                    {
                        string q = query.ToToolkit();

                        return i.label.ToToolkit().Contains(q) || i.defName.ToToolkit().Contains(q);
                    }
                )
               .Select(i => i.label.ToToolkit().CapitalizeFirst())
               .ToArray();

            NotifyLookupComplete(query, results);
        }

        private void PerformEventLookup(string query)
        {
            string[] results = DefDatabase<StoreIncident>.AllDefs.Where(i => i.cost > 0)
               .Where(
                    i =>
                    {
                        string q = query.ToToolkit();

                        return i.abbreviation.ToToolkit().Contains(q) || i.defName.ToToolkit().Contains(q);
                    }
                )
               .Select(i => i.abbreviation.ToToolkit().CapitalizeFirst())
               .ToArray();

            NotifyLookupComplete(query, results);
        }

        private void PerformItemLookup(string query)
        {
            string[] results = Data.Items.Where(i => i.Cost > 0)
               .Where(
                    i =>
                    {
                        if (i.Name == null || i.DefName == null)
                        {
                            return false;
                        }

                        string q = query.ToToolkit();

                        return i.Name.ToToolkit().Contains(q) || i.DefName!.ToToolkit().Contains(q);
                    }
                )
               .Select(i => i.Name.ToToolkit().CapitalizeFirst())
               .ToArray();

            NotifyLookupComplete(query, results);
        }

        private void PerformLookup(Category category, string query)
        {
            switch (category)
            {
                case Category.Disease:
                    PerformDiseaseLookup(query);

                    return;
                case Category.Skill:
                    PerformSkillLookup(query);

                    return;
                case Category.Event:
                    PerformEventLookup(query);

                    return;
                case Category.Item:
                    PerformItemLookup(query);

                    return;
                case Category.Animal:
                    PerformAnimalLookup(query);

                    return;
                case Category.Trait:
                    PerformTraitLookup(query);

                    return;
                case Category.Kind:
                    PerformKindLookup(query);

                    return;
                case Category.Mod:
                    PerformModLookup(query);

                    return;
            }
        }

        private void PerformModLookup(string query)
        {
            string[] results = Data.Mods.Where(m => m.Name.ToToolkit().Contains(query.ToToolkit())).Select(m => m.Name).ToArray();

            NotifyLookupComplete(query, results);
        }

        private void PerformKindLookup(string query)
        {
            string[] results = Data.PawnKinds.Where(i => i.Enabled)
               .Where(
                    i =>
                    {
                        string q = query.ToToolkit();

                        return i.Name.ToToolkit().Contains(q) || i.DefName.ToToolkit().Contains(q);
                    }
                )
               .Select(i => i.Name.ToToolkit().CapitalizeFirst())
               .ToArray();

            NotifyLookupComplete(query, results);
        }

        private void PerformSkillLookup(string query)
        {
            string[] results = DefDatabase<SkillDef>.AllDefs.Where(
                    i =>
                    {
                        string q = query.ToToolkit();

                        return i.label.ToToolkit().Contains(q) || i.defName.ToToolkit().Contains(q);
                    }
                )
               .Select(i => i.defName.ToLower().CapitalizeFirst())
               .ToArray();

            NotifyLookupComplete(query, results);
        }

        private void PerformTraitLookup(string query)
        {
            string[] results = Data.Traits.Where(t => t.CanAdd || t.CanRemove)
               .Where(
                    i =>
                    {
                        string q = query.ToToolkit();

                        return i.Name.ToToolkit().Contains(q) || i.DefName.ToToolkit().Contains(q);
                    }
                )
               .Select(i => i.Name.ToToolkit().CapitalizeFirst())
               .ToArray();

            NotifyLookupComplete(query, results);
        }
    }
}
