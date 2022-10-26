﻿// ToolkitUtils
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
using SirRandoo.ToolkitUtils.Utils.ModComp;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class SetTraits : IncidentVariablesBase
    {
        private List<TraitEvent> _events;
        private Pawn _pawn;

        public override bool CanHappen(string msg, Viewer viewer)
        {
            string[] traitQueries = CommandFilter.Parse(msg).Skip(2).ToArray();

            if (traitQueries.Length <= 0)
            {
                return false;
            }

            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));
            List<TraitItem> items = worker.GetAllAsTrait().ToList();

            if (worker.HasNext() && !worker.GetLast().NullOrEmpty())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(worker.GetLast()));

                return false;
            }


            if (!TryProcessTraits(_pawn!, items, out _events))
            {
                TraitEvent errored = _events.FirstOrDefault(e => !e.Error.NullOrEmpty());

                if (errored != null)
                {
                    MessageHelper.ReplyToUser(viewer.username, errored.Error);
                }

                return false;
            }

            if (_events.Count(e => e.ContributesToLimit) > AddTraitSettings.maxTraits)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.LimitReached".LocalizeKeyed(AddTraitSettings.maxTraits));

                return false;
            }

            int total = _events.Sum(
                i =>
                {
                    switch (i.Type)
                    {
                        case EventType.Add:
                            return i.Item.CostToAdd;
                        case EventType.Remove:
                            return i.Item.CostToRemove;
                        default:
                            return 0;
                    }
                }
            );

            if (!viewer.CanAfford(total))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".LocalizeKeyed(total.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

                return false;
            }

            return true;
        }

        [ContractAnnotation("=> true,traitEvents:notnull; => false,traitEvents:notnull")]
        private bool TryProcessTraits([NotNull] Pawn subject, [NotNull] IEnumerable<TraitItem> traits, out List<TraitEvent> traitEvents)
        {
            var container = new List<TraitEvent>();
            List<TraitItem> traitItems = traits.ToList();
            List<string> defNames = traitItems.Select(t => t.DefName).ToList();

            container.AddRange(
                subject.story.traits.allTraits.Where(t => !defNames.Contains(t.def.defName))
                   .Select(
                        t =>
                        {
                            TraitItem trait = Data.Traits.Find(i => i.DefName.Equals(t.def.defName) && i.Degree == t.Degree);

                            return !IsTraitRemovable(subject, trait, out string error)
                                ? new TraitEvent { Type = EventType.Noop, Error = error, Item = trait }
                                : new TraitEvent { Type = EventType.Remove, Trait = t, Item = trait };
                        }
                    )
            );

            container.AddRange(
                traitItems.Where(t => !subject.story.traits.HasTrait(t.TraitDef))
                   .Select(
                        t =>
                        {
                            if (!t.CanAdd)
                            {
                                return new TraitEvent { Type = EventType.Noop, Error = "TKUtils.Trait.Disabled".LocalizeKeyed(t.Name) };
                            }

                            return !IsTraitAddable(t, out string error)
                                ? new TraitEvent { Type = EventType.Noop, Error = error, Item = t }
                                : new TraitEvent { Type = EventType.Add, Item = t };
                        }
                    )
            );

            container.AddRange(
                traitItems.Where(t => subject.story.traits.allTraits.Find(i => i.def.defName.Equals(t.DefName) && i.Degree == t.Degree) != null)
                   .Select(t => new TraitEvent { Type = EventType.Noop, Item = t })
            );

            var final = new List<TraitEvent>(container.Where(e => e.Type == EventType.Remove));
            final.AddRange(container.Where(t => t.Type != EventType.Remove).GroupBy(t => t.Item.DefName).Select(e => e.First()));

            foreach (TraitEvent traitEvent in final.Where(f => f.Type == EventType.Add || f.Type == EventType.Noop))
            {
                TraitEvent conflictsWith = final.Where(f => f != traitEvent && (f.Type == EventType.Add || f.Type == EventType.Noop))
                   .FirstOrDefault(f => f.Item.TraitDef!.ConflictsWith(traitEvent.Item.TraitDef));

                if (conflictsWith != null && string.IsNullOrEmpty(traitEvent.Error))
                {
                    traitEvent.Error = "TKUtils.Trait.Conflict".LocalizeKeyed(traitEvent.Item.Name, conflictsWith.Item.Name);

                    traitEvents = final;

                    return false;
                }
            }

            traitEvents = final;

            return true;
        }

        public override void Execute()
        {
            TkUtils.Logger.Info("Executing...");

            foreach (TraitEvent ev in _events)
            {
                ev.Execute(_pawn);

                switch (ev.Type)
                {
                    case EventType.Add:
                        Viewer.Charge(ev.Item.CostToAdd, ev.Item.TraitData?.KarmaType ?? storeIncident.karmaType);

                        break;
                    case EventType.Remove:
                        Viewer.Charge(ev.Item.CostToRemove, ev.Item.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType);

                        break;
                }
            }

            string traitString = _pawn.story.traits.allTraits.Select(t => t.Label ?? t.def.defName).ToCommaList(true);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.SetTraits.Complete".LocalizeKeyed(traitString));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.SetDescription".LocalizeKeyed(Viewer.username, traitString),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private bool IsTraitRemovable(Pawn pawn, TraitItem trait, out string error)
        {
            if (IsTraitGeneLocked(pawn, trait))
            {
                error = "TKUtils.RemoveTrait.GeneLocked".LocalizeKeyed(trait.Name);

                return false;
            }

            if (CompatRegistry.Alien != null && CompatRegistry.Alien.IsTraitForced(_pawn, trait.DefName, trait.Degree))
            {
                error = "TKUtils.RemoveTrait.Kind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, trait.Name);

                return false;
            }

            if (RationalRomance.Active && RationalRomance.IsTraitDisabled(trait.TraitDef!))
            {
                error = "TKUtils.RemoveTrait.RationalRomance".LocalizeKeyed(trait.Name.CapitalizeFirst());

                return false;
            }

            if (CompatRegistry.Magic?.IsClassTrait(trait.TraitDef!) == true && !TkSettings.ClassChanges)
            {
                error = "TKUtils.RemoveTrait.Class".LocalizeKeyed(trait.Name);

                return false;
            }

            if (!trait.CanRemove)
            {
                error = "TKUtils.RemoveTrait.Disabled".LocalizeKeyed(trait.Name);

                return false;
            }

            error = null;

            return true;
        }

        [ContractAnnotation("=> true,error:null; => false,error:notnull")]
        private bool IsTraitAddable([NotNull] TraitItem trait, out string error)
        {
            if (trait.TraitDef.IsDisallowedByBackstory(_pawn, trait.Degree, out BackstoryDef backstory))
            {
                error = "TKUtils.Trait.RestrictedByBackstory".LocalizeKeyed(backstory.identifier, trait.Name);

                return false;
            }

            if (IsTraitGeneSuppressed(_pawn, trait))
            {
                error = "TKUtils.Trait.GeneSuppressed".LocalizeKeyed(trait.Name);

                return false;
            }

            if (_pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(trait.DefName)) == true)
            {
                error = "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, trait.Name);

                return false;
            }

            if (trait.TraitDef.IsDisallowedByKind(_pawn, trait.Degree))
            {
                error = "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, trait.Name);

                return false;
            }

            if (!trait.CanAdd)
            {
                error = "TKUtils.Trait.Disabled".LocalizeKeyed(trait.Name);

                return false;
            }

            error = null;

            return true;
        }

        private static bool IsTraitGeneLocked(Pawn pawn, TraitItem trait)
        {
            if (!ModLister.BiotechInstalled)
            {
                return false;
            }

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (!gene.Active)
                {
                    continue;
                }

                GeneticTraitData data = gene.def.forcedTraits.Find(t => t.def == trait.TraitDef && t.degree == trait.Degree);

                if (data != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsTraitGeneSuppressed(Pawn pawn, TraitItem trait)
        {
            if (!ModLister.BiotechInstalled)
            {
                return false;
            }

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (!gene.Active)
                {
                    continue;
                }

                GeneticTraitData data = gene.def.suppressedTraits.Find(t => t.def == trait.TraitDef && t.degree == trait.Degree);

                if (data != null)
                {
                    return true;
                }
            }

            return false;
        }

        private enum EventType { Remove, Add, Noop }

        private sealed class TraitEvent
        {
            public string Error { get; set; }
            public EventType Type { get; set; }
            public TraitItem Item { get; set; }
            public Trait Trait { get; set; }

            public bool ContributesToLimit => (Type == EventType.Noop || Type == EventType.Add) && Item.TraitData?.CanBypassLimit != true;

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

                if (CompatRegistry.Magic?.IsClassTrait(Trait.def) == true && TkSettings.ResetClass)
                {
                    CompatRegistry.Magic.ResetClass(pawn);
                }

                TraitHelper.RemoveTraitFromPawn(pawn, Trait);
            }
        }
    }
}
