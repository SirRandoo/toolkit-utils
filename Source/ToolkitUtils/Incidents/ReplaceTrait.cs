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
        private Pawn _pawn;
        private TraitItem _thatShop;
        private Trait _thatTrait;
        private TraitItem _thisShop;

        private Trait _thisTrait;

        private int TotalPrice => _thisShop.CostToRemove + _thatShop.CostToAdd;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());

                return false;
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsTrait(out _thisShop) || !worker.TryGetNextAsTrait(out _thatShop))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidTraitQuery".LocalizeKeyed(worker.GetLast()));

                return false;
            }

            if (!IsUsable(_thisShop, _thatShop))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    $"TKUtils.{(_thisShop.CanRemove ? "" : "Remove")}Trait.Disabled".LocalizeKeyed((_thisShop.CanRemove ? _thatShop : _thisShop).Name.CapitalizeFirst())
                );

                return false;
            }

            if (TraitHelper.GetTotalTraits(_pawn) >= AddTraitSettings.maxTraits && WouldExceedLimit())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.ReplaceTrait.Violation".LocalizeKeyed(_thisShop.Name, _thatShop.Name));

                return false;
            }

            if (!viewer.CanAfford(TotalPrice))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".LocalizeKeyed(TotalPrice.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

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

            return true;
        }

        private bool PassesValidationCheck(Viewer viewer)
        {
            _thisTrait = _pawn.story.traits.allTraits.Find(t => t.def.defName.Equals(_thisShop.DefName));
            _thatTrait = new Trait(_thatShop.TraitDef, _thatShop.Degree);

            if (_thisTrait == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Missing".LocalizeKeyed(_thisShop!.Name));

                return false;
            }

            TraitDef thatTraitDef = _thatShop.TraitDef;

            if (thatTraitDef == null)
            {
                return false;
            }

            foreach (Trait trait in _pawn.story.traits.allTraits)
            {
                if (trait != _thisTrait && trait.def.ConflictsWith(thatTraitDef))
                {
                    MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Conflict".LocalizeKeyed(trait.Label, _thatTrait.Label ?? _thatTrait.def.defName));

                    return false;
                }
            }

            Trait @class = _pawn.story.traits.allTraits.FirstOrDefault(t => CompatRegistry.Magic?.IsClassTrait(t.def) == true);

            if (@class != null && CompatRegistry.Magic?.IsClassTrait(thatTraitDef) == true)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.Class".Localize());

                return false;
            }

            return true;
        }

        private bool PassesCharacterCheck(Viewer viewer)
        {
            if (_thatShop!.TraitDef.IsDisallowedByBackstory(_pawn, _thatShop.Degree, out Backstory backstory))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.RestrictedByBackstory".LocalizeKeyed(backstory.identifier, _thisShop!.Name));

                return false;
            }

            if (_pawn.kindDef.disallowedTraits?.Any(t => t.defName.Equals(_thatShop.TraitDef!.defName)) == true)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, _thatShop.Name));

                return false;
            }

            if (_thatShop.TraitDef.IsDisallowedByKind(_pawn, _thatShop.Degree))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Trait.RestrictedByKind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, _thatShop.Name));

                return false;
            }

            return true;
        }

        private bool PassesModCheck(Viewer viewer)
        {
            if (RationalRomance.Active && RationalRomance.IsTraitDisabled(_thisShop!.TraitDef!) && !RationalRomance.IsTraitDisabled(_thatShop.TraitDef!))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.ReplaceTrait.RationalRomance".LocalizeKeyed(_thisShop.Name.CapitalizeFirst()));

                return false;
            }

            if (CompatRegistry.Alien != null && CompatRegistry.Alien.IsTraitForced(_pawn, _thisShop!.DefName, _thisShop.Degree))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Kind".LocalizeKeyed(_pawn.kindDef.race.LabelCap, _thisShop.Name));

                return false;
            }

            if (CompatRegistry.Magic?.IsClassTrait(_thisShop!.TraitDef!) == true && !TkSettings.ClassChanges)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.RemoveTrait.Class".Localize());

                return false;
            }

            return true;
        }

        public override void Execute()
        {
            if (CompatRegistry.Magic?.IsClassTrait(_thisShop.TraitDef!) == true && TkSettings.ResetClass)
            {
                CompatRegistry.Magic.ResetClass(_pawn);
            }

            TraitHelper.RemoveTraitFromPawn(_pawn, _thisTrait);

            Viewer.Charge(_thisShop.CostToRemove, _thisShop.TraitData?.KarmaTypeForRemoving ?? storeIncident.karmaType);


            TraitHelper.GivePawnTrait(_pawn, _thatTrait);

            Viewer.Charge(_thatShop.CostToAdd, _thatShop.Data?.KarmaType ?? storeIncident.karmaType);

            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.ReplaceTrait.Complete".LocalizeKeyed(_thisTrait.LabelCap, _thatTrait.LabelCap));

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.TraitLetter.Title".Localize(),
                "TKUtils.TraitLetter.ReplaceDescription".LocalizeKeyed(Viewer.username, _thisTrait.LabelCap, _thatTrait.LabelCap),
                LetterDefOf.NeutralEvent,
                new LookTargets(_pawn)
            );
        }

        private bool WouldExceedLimit()
        {
            if (_thisShop.TraitData?.CanBypassLimit == true && _thatShop.TraitData?.CanBypassLimit == false)
            {
                return true;
            }

            return _thisShop.TraitData?.CanBypassLimit == false && !_thatShop.TraitData?.CanBypassLimit == false;
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
