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
