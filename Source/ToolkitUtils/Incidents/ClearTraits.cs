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
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ClearTraits : IncidentVariablesBase
    {
        private Pawn pawn;
        private List<(Trait trait, TraitItem item)> traits;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            traits = new List<(Trait trait, TraitItem item)>();

            foreach (Trait trait in pawn!.story.traits.allTraits)
            {
                TraitItem item = Data.Traits.Find(t => t.DefName.Equals(trait.def.defName) && t.Degree == trait.Degree);

                traits.Add((trait, item));
            }

            return traits.Count > 0;
        }

        public override void Execute()
        {
            foreach ((Trait trait, TraitItem item) in traits)
            {
                if ((CompatRegistry.Magic?.IsClassTrait(trait.def) ?? false) && TkSettings.ResetClass)
                {
                    CompatRegistry.Magic?.ResetClass(pawn);
                }

                TraitHelper.RemoveTraitFromPawn(pawn, trait);

                if (!(item is null))
                {
                    Viewer.Charge(item.CostToRemove, item.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType);
                }
            }

            MessageHelper.SendConfirmation(
                message,
                "TKUtils.ClearTraits.Complete".LocalizeKeyed(traits.Count.ToString("N0"))
            );

            Find.LetterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.ClearDescription".LocalizeKeyed(Viewer.username),
                LetterDefOf.NeutralEvent,
                pawn
            );
        }
    }
}
