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
    [UsedImplicitly]
    public class ReplaceTrait : IncidentVariablesBase
    {
        private Pawn pawn;
        private TraitItem thatShop;
        private Trait thatTrait;
        private TraitItem thisShop;

        private Trait thisTrait;

        private int TotalPrice => thisShop.CostToRemove + thatShop.CostToAdd;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsTrait(out thisShop) || !worker.TryGetNextAsTrait(out thatShop))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(worker.GetLast()));
                return false;
            }

            if (!IsUsable(thisShop, thatShop))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    $"TKUtils.{(thisShop.CanRemove ? "" : "Remove")}Trait.Disabled".LocalizeKeyed(
                        (thisShop.CanRemove ? thatShop : thisShop).Name.CapitalizeFirst()
                    )
                );
                return false;
            }

            if (pawn!.story.traits.allTraits.Count > AddTraitSettings.maxTraits && WouldExceedLimit())
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ReplaceTrait.Violation".LocalizeKeyed(thisShop.Name, thatShop.Name)
                );
                return false;
            }

            if (!viewer.CanAfford(TotalPrice))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".LocalizeKeyed(
                        TotalPrice.ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            if (!PassesCharacterCheck(viewer))
            {
                return false;
            }

            if (!PassesModCheck(viewer))
            {
                return false;
            }

            if (!PassesValidationCheck(viewer))
            {
                return false;
            }

            thatTrait = new Trait(thatShop.TraitDef, thatShop.Degree);
            return true;
        }

        private bool PassesValidationCheck(Viewer viewer)
        {
            thisTrait = pawn.story.traits.allTraits.Find(t => t.def.defName.Equals(thisShop.DefName));

            if (thisTrait == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Missing".LocalizeKeyed(thisShop!.Name));
                return false;
            }

            Trait duplicate = pawn.story.traits.allTraits.Find(t => t.def.defName.Equals(thatShop.DefName));
            if (duplicate != null)
            {
                TraitItem item = Data.Traits.Find(
                    t => t.DefName.Equals(duplicate.def.defName) && t.Degree == duplicate.Degree
                );
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.Duplicate".LocalizeKeyed(item?.Name ?? duplicate.Label, thatShop.Name)
                );
                return false;
            }

            TraitDef thatTraitDef = thatShop.TraitDef;

            if (thatTraitDef == null)
            {
                return false;
            }

            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                if (trait.def.ConflictsWith(thatTraitDef))
                {
                    MessageHelper.ReplyToUser(
                        viewer.username,
                        "TKUtils.Trait.Conflict".LocalizeKeyed(trait.Label, thatTraitDef.label ?? thatTraitDef.defName)
                    );
                    return false;
                }
            }

            Trait @class =
                pawn.story.traits.allTraits.FirstOrDefault(t => CompatRegistry.Magic?.IsClassTrait(t.def) == true);

            if (@class != null && CompatRegistry.Magic?.IsClassTrait(thatTraitDef) == true)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Class".Localize());
                return false;
            }

            return true;
        }

        private bool PassesCharacterCheck(Viewer viewer)
        {
            if (thatShop!.TraitDef.IsDisallowedByBackstory(pawn, thatShop.Degree, out Backstory backstory))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByBackstory".LocalizeKeyed(backstory.identifier, thisShop!.Name)
                );
                return false;
            }

            if (pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(thatShop.TraitDef!.defName)) == true)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(pawn.kindDef.race.LabelCap, thatShop.Name)
                );
                return false;
            }

            if (thatShop.TraitDef.IsDisallowedByKind(pawn, thatShop.Degree))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(pawn.kindDef.race.LabelCap, thatShop.Name)
                );
                return false;
            }

            return true;
        }

        private bool PassesModCheck(Viewer viewer)
        {
            if (RationalRomance.Active
                && RationalRomance.IsTraitDisabled(thisShop!.TraitDef!)
                && !RationalRomance.IsTraitDisabled(thatShop.TraitDef!))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ReplaceTrait.RationalRomance".LocalizeKeyed(thisShop.Name.CapitalizeFirst())
                );
                return false;
            }

            if (AlienRace.Enabled && AlienRace.IsTraitForced(pawn, thisShop!.DefName, thisShop.Degree))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.RemoveTrait.Kind".LocalizeKeyed(pawn.kindDef.race.LabelCap, thisShop.Name)
                );
                return false;
            }

            if (CompatRegistry.Magic?.IsClassTrait(thisShop!.TraitDef!) == true && !TkSettings.ClassChanges)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Class".Localize());
                return false;
            }

            return true;
        }

        public override void Execute()
        {
            if ((CompatRegistry.Magic?.IsClassTrait(thisShop.TraitDef!) ?? false) && TkSettings.ResetClass)
            {
                CompatRegistry.Magic.ResetClass(pawn);
            }

            TraitHelper.RemoveTraitFromPawn(pawn, thisTrait);

            Viewer.Charge(thisShop.CostToRemove, thisShop.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType);


            TraitHelper.GivePawnTrait(pawn, thatTrait);

            Viewer.Charge(thatShop.CostToAdd, thatShop.Data?.KarmaType ?? storeIncident.karmaType);

            MessageHelper.SendConfirmation(
                Viewer.username,
                "TKUtils.ReplaceTrait.Complete".LocalizeKeyed(thisTrait.LabelCap, thatTrait.LabelCap)
            );

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.ReplaceDescription".LocalizeKeyed(
                    Viewer.username,
                    thisTrait.LabelCap,
                    thatTrait.LabelCap
                ),
                LetterDefOf.NeutralEvent,
                new LookTargets(pawn)
            );
        }

        private bool WouldExceedLimit()
        {
            if (thisShop.TraitData?.CanBypassLimit == true && thatShop.TraitData?.CanBypassLimit == false)
            {
                return true;
            }

            return thisShop.TraitData?.CanBypassLimit == false && !thatShop.TraitData?.CanBypassLimit == false;
        }

        private static bool IsUsable([CanBeNull] TraitItem t1, [CanBeNull] TraitItem t2)
        {
            if (t1?.TraitDef == null || t2?.TraitDef == null)
            {
                return false;
            }

            return t1.CanRemove && t2.CanAdd;
        }
    }
}
