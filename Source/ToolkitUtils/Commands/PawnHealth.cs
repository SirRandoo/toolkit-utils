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
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnHealth : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize().WithHeader("TabHealth".Localize()));
                return;
            }

            string segment = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrFallback("");

            if (segment.NullOrEmpty())
            {
                twitchMessage.Reply(HealthReport(pawn!).WithHeader("TabHealth".Localize()));
                return;
            }

            PawnCapacityDef capacity = DefDatabase<PawnCapacityDef>.AllDefs.FirstOrDefault(
                d => d.defName.EqualsIgnoreCase(segment)
                     || d.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(segment.ToToolkit())
            );

            twitchMessage.Reply(
                (capacity == null ? HealthReport(pawn!) : HealthCapacityReport(pawn!, capacity)).WithHeader(
                    "TabHealth".Localize()
                )
            );
        }

        private static string GetHealthStateFriendly(PawnHealthState state)
        {
            switch (state)
            {
                case PawnHealthState.Down:
                    return ResponseHelper.DazedGlyph.AltText("DownedLower".Localize().CapitalizeFirst());
                case PawnHealthState.Dead:
                    return ResponseHelper.GhostGlyph.AltText("Dead".Localize());
                default:
                    return string.Empty;
            }
        }

        private static string GetMoodFriendly([NotNull] Pawn subject)
        {
            if (subject.MentalStateDef != null)
            {
                return ResponseHelper.LightningGlyph.AltText($"({subject.MentalStateDef.label.CapitalizeFirst()})");
            }

            float thresholdExtreme = subject.mindState.mentalBreaker.BreakThresholdExtreme;
            float moodLevel = subject.needs.mood.CurLevel;

            if (moodLevel < thresholdExtreme)
            {
                return ResponseHelper.AboutToBreakGlyph.AltText($"({"Mood_AboutToBreak".Localize()})");
            }

            if (moodLevel < thresholdExtreme + 0.0500000007450581)
            {
                return ResponseHelper.OnEdgeGlyph.AltText($"({"Mood_OnEdge".Localize()})");
            }

            if (moodLevel < subject.mindState.mentalBreaker.BreakThresholdMinor)
            {
                return ResponseHelper.StressedGlyph.AltText($"({"Mood_Stressed".Localize()})");
            }

            if (moodLevel < 0.649999976158142)
            {
                return ResponseHelper.NeutralGlyph.AltText($"({"Mood_Neutral".Localize()})");
            }

            return moodLevel < 0.899999976158142
                ? ResponseHelper.ContentGlyph.AltText($"({"Mood_Content".Localize()})")
                : ResponseHelper.HappyGlyph.AltText($"({"Mood_Happy".Localize()})");
        }

        private static string HealthCapacityReport([NotNull] Pawn pawn, [NotNull] PawnCapacityDef capacity)
        {
            if (!PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity))
            {
                return "TKUtils.PawnHealth.IncapableOfCapacity".LocalizeKeyed(capacity.GetLabelFor(pawn));
            }

            var impactors = new List<PawnCapacityUtility.CapacityImpactor>();
            var segments = new List<string>
            {
                ResponseHelper.JoinPair(
                    capacity.LabelCap,
                    PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, capacity, impactors)
                       .ToStringPercent()
                ),
                impactors.Any()
                    ? ResponseHelper.JoinPair(
                        "TKUtils.PawnHealth.AffectedBy".Localize(),
                        GetImpactorsForPawn(pawn, impactors).SectionJoin()
                    )
                    : "NoHealthConditions".Localize().CapitalizeFirst()
            };


            return segments.GroupedJoin();
        }

        [NotNull]
        private static IEnumerable<string> GetImpactorsForPawn(
            Pawn pawn,
            [NotNull] IReadOnlyCollection<PawnCapacityUtility.CapacityImpactor> impactors
        )
        {
            List<string> parts = impactors.OfType<PawnCapacityUtility.CapacityImpactorHediff>()
               .Select(i => i.Readable(pawn))
               .ToList();

            parts.AddRange(
                impactors.OfType<PawnCapacityUtility.CapacityImpactorBodyPartHealth>().Select(i => i.Readable(pawn))
            );
            parts.AddRange(
                impactors.OfType<PawnCapacityUtility.CapacityImpactorCapacity>().Select(i => i.Readable(pawn))
            );
            parts.AddRange(impactors.OfType<PawnCapacityUtility.CapacityImpactorPain>().Select(i => i.Readable(pawn)));

            return parts;
        }

        [NotNull]
        private static string HealthReport([NotNull] Pawn pawn)
        {
            var segments = new List<string>
            {
                ResponseHelper.JoinPair(
                    "TKUtils.PawnHealth.OverallHealth".Localize(),
                    pawn.health.summaryHealth.SummaryHealthPercent.ToStringPercent()
                )
            };

            if (pawn.health.State != PawnHealthState.Mobile)
            {
                segments[0] += $" {GetHealthStateFriendly(pawn.health.State)}";
            }
            else
            {
                segments[0] += $" {GetMoodFriendly(pawn)}";
            }

            if (pawn.health.hediffSet.BleedRateTotal > 0.01f)
            {
                int ticks = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);

                segments.Add(
                    ticks >= 60000
                        ? ResponseHelper.BleedingSafeGlyphs.AltText("WontBleedOutSoon".Localize().CapitalizeFirst())
                        : $"{ResponseHelper.BleedingBadGlyphs.AltText("BleedingRate".Localize())} ({ticks.ToStringTicksToPeriod(shortForm: true)})"
                );
            }

            List<PawnCapacityDef> source = GetCapacitiesForPawn(pawn).ToList();

            if (source.Count > 0)
            {
                source = source.OrderBy(d => d.listOrder).ToList();

                string[] capacities = source
                   .Where(capacity => PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity))
                   .Select(
                        capacity => ResponseHelper.JoinPair(
                            capacity.GetLabelFor(pawn).CapitalizeFirst(),
                            HealthCardUtility.GetEfficiencyLabel(pawn, capacity).First
                        )
                    )
                   .ToArray();

                segments.Add(capacities.SectionJoin());
            }
            else
            {
                segments.Add("TKUtils.Responses.UnsupportedRace".LocalizeKeyed(pawn.kindDef.race.defName));
            }

            if (!TkSettings.ShowSurgeries)
            {
                return segments.GroupedJoin();
            }

            BillStack surgeries = pawn.health.surgeryBills;

            if (surgeries?.Count <= 0)
            {
                return segments.GroupedJoin();
            }

            string[] queued = surgeries!.Bills.Select(item => item.LabelCap).ToArray();

            segments.Add(
                ResponseHelper.JoinPair("TKUtils.PawnHealth.QueuedSurgeries".Localize(), queued.SectionJoin())
            );

            return segments.GroupedJoin();
        }

        [NotNull]
        private static IEnumerable<PawnCapacityDef> GetCapacitiesForPawn([NotNull] Thing pawn)
        {
            IEnumerable<PawnCapacityDef> capacityDefs = DefDatabase<PawnCapacityDef>.AllDefs;


            if (pawn.def.race.Humanlike)
            {
                return capacityDefs.Where(d => d.showOnHumanlikes);
            }

            if (pawn.def.race.Animal)
            {
                return capacityDefs.Where(d => d.showOnAnimals);
            }

            return pawn.def.race.IsMechanoid
                ? capacityDefs.Where(d => d.showOnMechanoids)
                : new List<PawnCapacityDef>();
        }
    }
}
