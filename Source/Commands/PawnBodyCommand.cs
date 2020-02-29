using System.Collections.Generic;
using System.Linq;
using System.Text;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnBodyCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if(!CommandsHandler.AllowCommand(message)) return;

            var pawn = GetPawn(message.User);

            if(pawn == null)
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named("TKUtils.Responses.NoPawn".Translate(), "MESSAGE")
                    ),
                    message
                );
                return;
            }

            var builder = new StringBuilder($"{"TKUtils.Responses.BodyWord".Translate()}: ");
            var hediffs = pawn.health.hediffSet.hediffs;

            if(hediffs != null && hediffs.Any())
            {
                var e = GetVisibleHediffGroupsInOrder(pawn);
                var segments = new List<string>();

                foreach(var item in e)
                {
                    var t = (item.Key != null ? item.Key.LabelCap : "WholeBody".Translate().ToString()) + ": ";
                    var chunks = new List<string>();

                    foreach(var group in item.GroupBy(h => h.UIGroupKey))
                    {
                        var count = group.Where(i => i.Bleeding).Count();
                        var total = group.Count();

                        if(total != 1) t += $" x{total.ToString()}";
                        if(count > 0) t += $" {"TKUtils.Responses.BleedingWord".Translate()}x{count}";

                        chunks.Add(group.First().LabelCap);
                    }

                    segments.Add(t + string.Join(", ", chunks));
                }

                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named(string.Join(" | ", segments), "MESSAGE")
                    ),
                    message
                );
            }
        }

        private float GetListPriority(BodyPartRecord record) => record == null ? 9999999f : ((float) record.height * 10000) + record.coverageAbsWithChildren;

        private IEnumerable<IGrouping<BodyPartRecord, Hediff>> GetVisibleHediffGroupsInOrder(Pawn pawn)
        {
            return GetVisibleHediffs(pawn)
                .GroupBy(x => x.Part)
                .OrderByDescending(x => GetListPriority(x.First().Part));
        }

        private IEnumerable<Hediff> GetVisibleHediffs(Pawn pawn)
        {
            var missing = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            foreach(var part in missing)
            {
                yield return part;
            }

            var e = pawn.health.hediffSet.hediffs.Where(d => !(d is Hediff_MissingPart) && d.Visible);

            foreach(var item in e)
            {
                yield return item;
            }
        }
    }
}
