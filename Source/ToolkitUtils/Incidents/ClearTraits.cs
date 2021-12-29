// MIT License
// 
// Copyright (c) 2021 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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

                if (item == null || !CanRemove(item))
                {
                    continue;
                }

                _traits.Add((trait, item));
            }

            int total = _traits.Sum(t => t.item.CostToRemove);

            if (!viewer.CanAfford(total))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".LocalizeKeyed(total.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

                return false;
            }

            return _traits.Count > 0;
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

            Find.LetterStack.ReceiveLetter("TKUtils.TraitLetter.Title".Localize(), "TKUtils.TraitLetter.ClearDescription".LocalizeKeyed(Viewer.username), LetterDefOf.NeutralEvent, _pawn);
        }

        private bool CanRemove(TraitItem trait)
        {
            if (RationalRomance.Active && RationalRomance.IsTraitDisabled(trait.TraitDef!))
            {
                MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RemoveTrait.RationalRomance".LocalizeKeyed(trait.Name.CapitalizeFirst()));

                return false;
            }

            if (AlienRace.Enabled && AlienRace.IsTraitForced(_pawn, trait.DefName, trait.Degree))
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
    }
}
