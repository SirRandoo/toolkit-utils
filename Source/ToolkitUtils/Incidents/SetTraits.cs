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
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class SetTraits : IncidentVariablesBase
    {
        private List<TraitEvent> events;
        private Pawn pawn;

        public override bool CanHappen(string msg, Viewer viewer)
        {
            string[] traitQueries = CommandFilter.Parse(msg).Skip(2).ToArray();

            if (traitQueries.Length <= 0)
            {
                return false;
            }

            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));
            List<TraitItem> items = worker.GetAllAsTrait().ToList();

            if (worker.HasNext() && !worker.GetLast().NullOrEmpty())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(worker.GetLast()));
                return false;
            }

            if (TryProcessTraits(pawn!, items, out events))
            {
                return events != null;
            }

            TraitEvent errored = events.FirstOrDefault(e => !e.Error.NullOrEmpty());

            if (errored != null)
            {
                MessageHelper.ReplyToUser(viewer.username, errored.Error);
            }

            return events != null;
        }

        private IEnumerable<TraitItem> FindTraits(Viewer viewer, [NotNull] params string[] traits)
        {
            foreach (string query in traits)
            {
                if (!Data.TryGetTrait(query, out TraitItem trait))
                {
                    MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(query));
                    yield break;
                }

                yield return trait;
            }
        }

        private bool TryProcessTraits(
            [NotNull] Pawn subject,
            [NotNull] IEnumerable<TraitItem> traits,
            [NotNull] out List<TraitEvent> traitEvents
        )
        {
            var container = new List<TraitEvent>();
            List<TraitItem> traitItems = traits.ToList();
            List<string> defNames = traitItems.Select(t => t.DefName).ToList();

            container.AddRange(
                subject.story.traits.allTraits.Where(t => !defNames.Contains(t.def.defName))
                   .Select(
                        t =>
                        {
                            TraitItem trait =
                                Data.Traits.Find(i => i.DefName.Equals(t.def.defName) && i.Degree == t.Degree);

                            if (trait.CanRemove)
                            {
                                return new TraitEvent {Type = EventType.Remove, Trait = t, Item = trait};
                            }

                            return new TraitEvent
                            {
                                Type = EventType.Noop, Error = "TKUtils.RemoveTrait.Disabled".LocalizeKeyed(trait.Name)
                            };
                        }
                    )
            );

            container.AddRange(
                traitItems.Where(t => !subject.story.traits.HasTrait(t.TraitDef))
                   .Select(
                        t =>
                        {
                            if (t.CanAdd)
                            {
                                return new TraitEvent {Type = EventType.Add, Item = t};
                            }

                            return new TraitEvent
                            {
                                Type = EventType.Noop, Error = "TKUtils.Trait.Disabled".LocalizeKeyed(t.Name)
                            };
                        }
                    )
            );

            traitEvents = container;
            return true;
        }

        public override void Execute()
        {
            foreach (TraitEvent ev in events)
            {
                ev.Execute(pawn);

                switch (ev.Type)
                {
                    case EventType.Add:
                        Viewer.Charge(ev.Item.CostToAdd, ev.Item.TraitData?.KarmaType ?? storeIncident.karmaType);
                        break;
                    case EventType.Remove:
                        Viewer.Charge(
                            ev.Item.CostToRemove,
                            ev.Item.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType
                        );
                        break;
                }
            }
        }

        private enum EventType { Remove, Add, Noop }

        private class TraitEvent
        {
            public string Error { get; set; }
            public EventType Type { get; set; }
            public TraitItem Item { get; set; }
            public Trait Trait { get; set; }

            public void Execute(Pawn pawn)
            {
                switch (Type)
                {
                    case EventType.Add:
                        AddTrait(pawn);
                        break;
                    case EventType.Remove:
                        RemoveTrait(pawn);
                        break;
                }
            }

            private void AddTrait(Pawn pawn)
            {
                if (Item == null)
                {
                    return;
                }

                TraitHelper.GivePawnTrait(pawn, new Trait(Item.TraitDef, Item.Degree));
            }

            private void RemoveTrait(Pawn pawn)
            {
                if (Trait == null)
                {
                    return;
                }

                if ((CompatRegistry.Magic?.IsClassTrait(Trait.def) ?? false) && TkSettings.ResetClass)
                {
                    CompatRegistry.Magic.ResetClass(pawn);
                }

                TraitHelper.RemoveTraitFromPawn(pawn, Trait);
            }
        }
    }
}
