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

using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    public class RandomInspire : IncidentVariablesBase
    {
        public override bool CanHappen(string msg, Viewer viewer)
        {
            return true;
        }

        public override void Execute()
        {
            foreach (Pawn pawn in Find.ColonistBar.GetColonistsInOrder().InRandomOrder())
            {
                if (pawn == null || pawn.Inspired)
                {
                    continue;
                }

                if (!DefDatabase<InspirationDef>.AllDefs.TryRandomElementByWeight(
                    i => i.Worker.CommonalityFor(pawn),
                    out InspirationDef def
                ))
                {
                    continue;
                }

                if (!pawn.mindState.inspirationHandler.TryStartInspiration(def))
                {
                    continue;
                }

                MessageHelper.SendConfirmation(
                    Viewer.username,
                    "TKUtils.RandomInspire.Complete".LocalizeKeyed(
                        pawn.LabelShort?.CapitalizeFirst() ?? pawn.LabelCap,
                        def.label
                    )
                );
                Viewer.Charge(storeIncident);
                return;
            }

            MessageHelper.ReplyToUser(Viewer.username, "TKUtils.RandomInspire.None".Localize());
        }
    }
}
