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
using SirRandoo.ToolkitUtils.Utils.ModComp;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class ClearTraits : IncidentVariablesBase
    {
        private Pawn _pawn;
        private List<(Trait trait, TraitItem item)> _traits;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            Viewer = viewer;

            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            _traits = new List<(Trait trait, TraitItem item)>();

            foreach (Trait trait in _pawn!.story.traits.allTraits)
            {
                TraitItem item = Data.Traits.Find(t => t.DefName.Equals(trait.def.defName) && t.Degree == trait.Degree);

                if (!(item is { CanRemove: true }) || !CanRemove(_pawn, item))
                {
                    continue;
                }

                _traits.Add((trait, item));
            }

            int total = _traits.Sum(t => t.item.CostToRemove);

            if (viewer.CanAfford(total))
            {
                return _traits.Count > 0;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".LocalizeKeyed(total.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

            return false;
        }

        public override void Execute()
        {
            foreach ((Trait trait, TraitItem item) in _traits)
            {
                if (CompatRegistry.Magic?.IsClassTrait(trait.def) == true && TkSettings.ResetClass)
                {
                    CompatRegistry.Magic?.ResetClass(_pawn);
                }

                TraitHelper.RemoveTraitFromPawn(_pawn, trait);

                if (!(item is null))
                {
                    Viewer.Charge(item.CostToRemove, item.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType);
                }
            }

            MessageHelper.SendConfirmation(message, "TKUtils.ClearTraits.Complete".LocalizeKeyed(_traits.Count.ToString("N0")));

            Find.LetterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.ClearDescription".LocalizeKeyed(Viewer.username),
                LetterDefOf.NeutralEvent,
                _pawn
            );
        }

        private bool CanRemove(Pawn pawn, TraitItem trait)
        {
            if (IsTraitGeneLocked(pawn, trait))
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RemoveTrait.GeneLocked".LocalizeKeyed(trait.Name));
                
                return false;
            }
            
            if (RationalRomance.Active && RationalRomance.IsTraitDisabled(trait.TraitDef!))
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RemoveTrait.RationalRomance".LocalizeKeyed(trait.Name.CapitalizeFirst()));

                return false;
            }

            if (CompatRegistry.Alien != null && CompatRegistry.Alien.IsTraitForced(_pawn, trait.DefName, trait.Degree))
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RemoveTrait.Kind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, trait.Name));

                return false;
            }

            if (CompatRegistry.Magic?.IsClassTrait(trait.TraitDef!) != true || TkSettings.ClassChanges)
            {
                return true;
            }

            MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RemoveTrait.Class".LocalizeKeyed(trait.Name));

            return false;
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
    }
}
