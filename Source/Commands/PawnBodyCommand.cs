using System.Collections.Generic;
using System.Linq;
using RimWorld;
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
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetOrFindPawn(message.User);

            if (pawn == null)
            {
                message.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("HealthOverview".Translate()));
                return;
            }

            message.Reply(GetPawnBody(pawn).WithHeader("HealthOverview".Translate()));
        }

        private static float GetListPriority(BodyPartRecord record)
        {
            return record == null ? 9999999f : (float) record.height * 10000 + record.coverageAbsWithChildren;
        }

        private static string GetPawnBody(Pawn target)
        {
            var hediffs = target.health.hediffSet.hediffs;

            if (hediffs == null || !hediffs.Any())
            {
                return "NoHealthConditions".Translate().CapitalizeFirst();
            }

            var hediffsGrouped = GetVisibleHediffGroupsInOrder(target);
            var parts = new List<string>();

            if (!TkSettings.TempInGear)
            {
                var tempMin = target.GetStatValue(StatDefOf.ComfyTemperatureMin).ToStringTemperature();
                var tempMax = target.GetStatValue(StatDefOf.ComfyTemperatureMax).ToStringTemperature();

                parts.Add($"{"🌡".AltText("ComfyTemperatureRange".Translate().RawText)}{tempMin}~{tempMax}");
            }

            foreach (var item in hediffsGrouped)
            {
                var bodyPart = item.Key?.LabelCap ?? "WholeBody".Translate();
                var bits = new List<string>();

                foreach (var group in item.GroupBy(h => h.UIGroupKey))
                {
                    var display = group.First().LabelCap;
                    var count = group.Count(i => i.Bleeding);
                    var total = group.Count();

                    if (total != 1)
                    {
                        display += $" x{total}";
                    }

                    if (count > 0)
                    {
                        display = "🩸".AltText().Translate("BleedingRate".Translate().RawText) + display;
                    }

                    bits.Add(display);
                }

                parts.Add(
                    "TKUtils.Formats.KeyValue".Translate(
                        bodyPart,
                        string.Join(", ", bits.ToArray())
                    )
                );
            }

            return string.Join("⎮", parts.ToArray());
        }

        private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> GetVisibleHediffGroupsInOrder(Pawn pawn)
        {
            return GetVisibleHediffs(pawn)
                .GroupBy(x => x.Part)
                .OrderByDescending(x => GetListPriority(x.First().Part));
        }

        private static IEnumerable<Hediff> GetVisibleHediffs(Pawn pawn)
        {
            var missing = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            foreach (var part in missing)
            {
                yield return part;
            }

            var e = pawn.health.hediffSet.hediffs.Where(d => !(d is Hediff_MissingPart) && d.Visible);

            foreach (var item in e)
            {
                yield return item;
            }
        }
    }
}
