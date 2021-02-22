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
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class KidnapReport
    {
        public string Viewer { get; set; }
        public List<string> PawnIds { get; set; }

        public static KidnapReport KidnapReportFor(string username)
        {
            if (CommandBase.GetOrFindPawn(username) is { } linkedPawn)
            {
                return new KidnapReport {Viewer = username, PawnIds = new List<string> {linkedPawn.ThingID}};
            }

            if (Find.FactionManager == null || Find.FactionManager.AllFactionsListForReading.NullOrEmpty())
            {
                return null;
            }

            var report = new KidnapReport {Viewer = username, PawnIds = new List<string>()};
            foreach (List<Pawn> kidnapped in Find.FactionManager.AllFactions.Select(
                f => f.kidnapped?.KidnappedPawnsListForReading
            ))
            {
                if (kidnapped == null)
                {
                    continue;
                }

                report.PawnIds.AddRange(
                    kidnapped.Where(p => !p.Dead)
                       .Where(p => p.Name is NameTriple name && name.Nick.EqualsIgnoreCase(username))
                       .Select(p => p.ThingID)
                );
            }

            return report;
        }

        public IEnumerable<Pawn> GetPawns()
        {
            return Find.FactionManager.AllFactions.SelectMany(f => f.kidnapped.KidnappedPawnsListForReading)
               .Where(kidnapped => PawnIds.Contains(kidnapped.ThingID));
        }

        public Pawn GetMostRecentKidnapping()
        {
            Tale_DoublePawn currentTale = null;
            foreach (Tale_DoublePawn kidnapping in Find.TaleManager.AllTalesListForReading
               .Where(t => t.def == TaleDefOf.KidnappedColonist)
               .Select(t => (Tale_DoublePawn) t)
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

            return currentTale?.secondPawnData.pawn;
        }
    }
}
