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
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class KidnapReport
    {
        public string Viewer { get; set; }
        public List<string> PawnIds { get; set; }

        [CanBeNull]
        public static KidnapReport KidnapReportFor(string username)
        {
            if (PurchaseHelper.TryGetPawn(username, out Pawn linkedPawn))
            {
                return new KidnapReport { Viewer = username, PawnIds = new List<string> { linkedPawn.ThingID } };
            }

            if (!Find.FactionManager?.AllFactions.Any() != true)
            {
                return null;
            }

            var report = new KidnapReport { Viewer = username, PawnIds = new List<string>() };

            foreach (Pawn pawn in Find.FactionManager.AllFactions.SelectMany(f => f.kidnapped.KidnappedPawnsListForReading))
            {
                if (!(pawn.Name is NameTriple name) || !name.Nick.EqualsIgnoreCase(username))
                {
                    continue;
                }

                report.PawnIds.Add(pawn.ThingID);
            }

            foreach (Pawn pawn in Find.WorldPawns.AllPawnsAlive)
            {
                if (pawn.GuestStatus == GuestStatus.Prisoner && pawn.HomeFaction == Faction.OfPlayer)
                {
                    report.PawnIds.Add(pawn.ThingID);
                }
            }

            return report;
        }

        [NotNull]
        public IEnumerable<Pawn> GetPawns()
        {
            return Find.FactionManager.AllFactions.SelectMany(f => f.kidnapped.KidnappedPawnsListForReading).Where(kidnapped => PawnIds.Contains(kidnapped.ThingID));
        }

        [CanBeNull]
        public Pawn GetMostRecentKidnapping()
        {
            Tale_DoublePawn currentTale = null;

            foreach (Tale_DoublePawn kidnapping in Find.TaleManager.AllTalesListForReading.Where(t => t.def == TaleDefOf.KidnappedColonist)
               .Select(t => (Tale_DoublePawn)t)
               .Where(t => PawnIds.Contains(t.secondPawnData.pawn.ThingID)))
            {
                if (currentTale == null)
                {
                    currentTale = kidnapping;

                    continue;
                }

                if (currentTale.AgeTicks > kidnapping.AgeTicks)
                {
                    currentTale = kidnapping;
                }
            }

            if (currentTale != null)
            {
                return currentTale.secondPawnData.pawn;
            }

            foreach (Pawn pawn in Find.WorldPawns.AllPawnsAlive)
            {
                if (!PawnIds.Contains(pawn.ThingID))
                {
                    continue;
                }

                return pawn;
            }

            return null;
        }
    }
}
