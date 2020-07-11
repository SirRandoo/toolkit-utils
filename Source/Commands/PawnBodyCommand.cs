using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnBodyCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply(
                    "TKUtils.NoPawn".Localize().WithHeader("HealthOverview".Localize())
                );
                return;
            }

            twitchMessage.Reply(GetPawnBody(pawn).WithHeader("HealthOverview".Localize()));
        }

        private static float GetListPriority(BodyPartRecord record)
        {
            return record == null ? 9999999f : (float) record.height * 10000 + record.coverageAbsWithChildren;
        }

        private static string GetPawnBody(Pawn target)
        {
            List<Hediff> hediffs = target.health.hediffSet.hediffs;

            if (hediffs == null || !hediffs.Any())
            {
                return "NoHealthConditions".Localize().CapitalizeFirst();
            }

            IEnumerable<IGrouping<BodyPartRecord, Hediff>> hediffsGrouped = GetVisibleHediffGroupsInOrder(target);
            var parts = new List<string>();

            if (!TkSettings.TempInGear)
            {
                string tempMin = target.GetStatValue(StatDefOf.ComfyTemperatureMin).ToStringTemperature();
                string tempMax = target.GetStatValue(StatDefOf.ComfyTemperatureMax).ToStringTemperature();

                parts.Add(
                    $"{ResponseHelper.TemperatureGlyph.AltText("ComfyTemperatureRange".Localize())}{tempMin}~{tempMax}"
                );
            }

            foreach (IGrouping<BodyPartRecord, Hediff> item in hediffsGrouped)
            {
                string bodyPart = item.Key?.LabelCap ?? "WholeBody".Localize();
                var bits = new List<string>();

                foreach (IGrouping<int, Hediff> group in item.GroupBy(h => h.UIGroupKey))
                {
                    Hediff affliction = group.First();
                    string display = affliction.LabelCap;
                    int total = group.Count();

                    if (total != 1)
                    {
                        display += $" x{total.ToString()}";
                    }

                    if (group.Count(i => i.Bleeding) > 0)
                    {
                        display = ResponseHelper.BleedingGlyph.AltText("BleedingRate".Localize()) + display;
                    }

                    if (group.All(i => i.IsTended()))
                    {
                        display = ResponseHelper.BandageGlyph.AltText("") + display;
                    }

                    bits.Add(display);
                }

                parts.Add(ResponseHelper.JoinPair(bodyPart, bits.SectionJoin()));
            }

            return parts.GroupedJoin();
        }

        private static IEnumerable<IGrouping<BodyPartRecord, Hediff>> GetVisibleHediffGroupsInOrder(Pawn pawn)
        {
            return GetVisibleHediffs(pawn)
                .GroupBy(x => x.Part)
                .OrderByDescending(x => GetListPriority(x.First().Part));
        }

        private static IEnumerable<Hediff> GetVisibleHediffs(Pawn pawn)
        {
            List<Hediff_MissingPart> missing = pawn.health.hediffSet.GetMissingPartsCommonAncestors();

            foreach (Hediff_MissingPart part in missing)
            {
                yield return part;
            }

            IEnumerable<Hediff> e = pawn.health.hediffSet.hediffs.Where(d => !(d is Hediff_MissingPart) && d.Visible);

            foreach (Hediff item in e)
            {
                yield return item;
            }
        }
    }
}
