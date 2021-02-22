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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class FullHeal : IncidentHelperVariables
    {
        private Pawn pawn;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            if (IncidentSettings.FullHeal.FairFights
                && pawn.mindState.lastAttackTargetTick > 0
                && Find.TickManager.TicksGame < pawn.mindState.lastAttackTargetTick + 1800)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InCombat".Localize());
                return false;
            }

            if (HealHelper.GetPawnHealable(pawn) != null)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.NotInjured".Localize());
            return false;
        }

        public override void TryExecute()
        {
            var healed = 0;
            var iterations = 0;
            while (true)
            {
                if (!Viewer.CanAfford(storeIncident.cost))
                {
                    break;
                }

                object healable = HealHelper.GetPawnHealable(pawn);

                if (healable == null)
                {
                    break;
                }

                healed = Heal(healable, healed);
                iterations += 1;

                if (iterations < 500)
                {
                    continue;
                }

                LogHelper.Warn("Exceeded the maximum number of iterations during full heal.");
                break;
            }

            MessageHelper.SendConfirmation(
                Viewer.username,
                healed > 1
                    ? "TKUtils.FullHeal.CompletePlural".Localize(healed.ToString("N0"))
                    : "TKUtils.FullHeal.Complete".Localize()
            );

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.FullHealLetter.Title".Localize(),
                "TKUtils.FullHealLetter.Description".Localize(Viewer.username),
                LetterDefOf.PositiveEvent,
                pawn
            );
        }

        private int Heal(object injury, int healed)
        {
            switch (injury)
            {
                case Hediff hediff:
                    HealHelper.Cure(hediff);
                    healed += 1;

                    Viewer.Charge(storeIncident, healed);
                    break;
                case BodyPartRecord record:
                    pawn.health.RestorePart(record);
                    healed += 1;

                    Viewer.Charge(storeIncident, healed);
                    break;
            }

            return healed;
        }
    }
}
