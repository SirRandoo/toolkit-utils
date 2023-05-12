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

using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Entities;
using SirRandoo.CommonLib.Interfaces;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace ToolkitUtils.Incidents
{
    public class FullHeal : IncidentVariablesBase
    {
        private static readonly IRimLogger Logger = new RimThreadedLogger("ToolkitUtils.Incidents.FullHeal");
        
        private Pawn _pawn;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (!PurchaseHelper.TryGetPawn(viewer.username, out _pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".TranslateSimple());

                return false;
            }

            if (global::ToolkitUtils.IncidentSettings.FullHeal.FairFights && _pawn!.mindState.lastAttackTargetTick > 0
                && Find.TickManager.TicksGame < _pawn.mindState.lastAttackTargetTick + 1800)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InCombat".TranslateSimple());

                return false;
            }

            if (HealHelper.GetPawnHealable(_pawn!) != null)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.NotInjured".TranslateSimple());

            return false;
        }

        public override void Execute()
        {
            var healed = 0;
            var iterations = 0;

            while (true)
            {
                if (!Viewer.CanAfford(storeIncident.cost))
                {
                    break;
                }

                object healable = HealHelper.GetPawnHealable(_pawn);

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

                Logger.Warn("Exceeded the maximum number of iterations during full heal.");

                break;
            }

            MessageHelper.SendConfirmation(
                Viewer.username,
                healed > 1 ? (string) "TKUtils.FullHeal.CompletePlural".Translate(healed.ToString("N0")) : "TKUtils.FullHeal.Complete".TranslateSimple()
            );

            Current.Game.letterStack.ReceiveLetter(
                "TKUtils.FullHealLetter.Title".TranslateSimple(),
                "TKUtils.FullHealLetter.Description".Translate(Viewer.username),
                LetterDefOf.PositiveEvent,
                _pawn
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
                    _pawn.health.RestorePart(record);
                    healed += 1;

                    Viewer.Charge(storeIncident, healed);

                    break;
            }

            return healed;
        }
    }
}
