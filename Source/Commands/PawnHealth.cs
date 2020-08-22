﻿using System.Collections.Generic;
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
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".TranslateSimple().WithHeader("TabHealth".Localize()));
                return;
            }

            string segment = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrFallback("");

            if (segment.NullOrEmpty())
            {
                twitchMessage.Reply(HealthReport(pawn).WithHeader("TabHealth".Localize()));
                return;
            }

            PawnCapacityDef capacity = DefDatabase<PawnCapacityDef>.AllDefsListForReading.FirstOrDefault(
                d => d.defName.EqualsIgnoreCase(segment)
                     || d.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(segment.ToToolkit())
            );

            twitchMessage.Reply(
                (capacity == null ? HealthReport(pawn) : HealthCapacityReport(pawn, capacity)).WithHeader(
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

        private static string GetMoodFriendly(Pawn subject)
        {
            if (subject.MentalStateDef != null)
            {
                return ResponseHelper.LightningGlyph.AltText(subject.MentalStateDef.LabelCap);
            }

            float thresholdExtreme = subject.mindState.mentalBreaker.BreakThresholdExtreme;
            float moodLevel = subject.needs.mood.CurLevel;

            if (moodLevel < thresholdExtreme)
            {
                return ResponseHelper.AboutToBreakGlyph.AltText("Mood_AboutToBreak".Localize());
            }

            if (moodLevel < thresholdExtreme + 0.0500000007450581)
            {
                return ResponseHelper.OnEdgeGlyph.AltText("Mood_OnEdge".Localize());
            }

            if (moodLevel < subject.mindState.mentalBreaker.BreakThresholdMinor)
            {
                return ResponseHelper.StressedGlyph.AltText("Mood_Stressed".Localize());
            }

            if (moodLevel < 0.649999976158142)
            {
                return ResponseHelper.NeutralGlyph.AltText("Mood_Neutral".Localize());
            }

            return moodLevel < 0.899999976158142
                ? ResponseHelper.ContentGlyph.AltText("Mood_Content".Localize())
                : ResponseHelper.HappyGlyph.AltText("Mood_Happy".Localize());
        }

        private static string HealthCapacityReport(Pawn pawn, PawnCapacityDef capacity)
        {
            if (!PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity))
            {
                return "TKUtils.PawnHealth.IncapableOfCapacity".Localize(capacity.GetLabelFor(pawn));
            }

            var impactors = new List<PawnCapacityUtility.CapacityImpactor>();
            var segments = new List<string>
            {
                ResponseHelper.JoinPair(
                    capacity.LabelCap,
                    PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, capacity, impactors)
                       .ToStringPercent()
                )
            };

            if (impactors.Any())
            {
                var parts = new List<string>();

                foreach (PawnCapacityUtility.CapacityImpactor i in impactors)
                {
                    if (i is PawnCapacityUtility.CapacityImpactorHediff)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach (PawnCapacityUtility.CapacityImpactor i in impactors)
                {
                    if (i is PawnCapacityUtility.CapacityImpactorBodyPartHealth)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach (PawnCapacityUtility.CapacityImpactor i in impactors)
                {
                    if (i is PawnCapacityUtility.CapacityImpactorCapacity)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach (PawnCapacityUtility.CapacityImpactor i in impactors)
                {
                    if (i is PawnCapacityUtility.CapacityImpactorPain)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                segments.Add(ResponseHelper.JoinPair("TKUtils.PawnHealth.AffectedBy".Localize(), parts.SectionJoin()));
            }
            else
            {
                segments.Add("NoHealthConditions".Localize().CapitalizeFirst());
            }

            return segments.GroupedJoin();
        }

        private static string HealthReport(Pawn pawn)
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

            List<PawnCapacityDef> source;
            if (pawn.def.race.Humanlike)
            {
                source = DefDatabase<PawnCapacityDef>.AllDefs.Where(d => d.showOnHumanlikes).ToList();
            }
            else if (pawn.def.race.Animal)
            {
                source = DefDatabase<PawnCapacityDef>.AllDefs.Where(d => d.showOnAnimals).ToList();
            }
            else if (pawn.def.race.IsMechanoid)
            {
                source = DefDatabase<PawnCapacityDef>.AllDefs.Where(d => d.showOnMechanoids).ToList();
            }
            else
            {
                source = new List<PawnCapacityDef>().ToList();

                segments.Add("TKUtils.Responses.UnsupportedRace".Localize(pawn.kindDef.race.defName));
            }

            if (source.Any())
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

            if (!TkSettings.ShowSurgeries)
            {
                return segments.GroupedJoin();
            }

            BillStack surgeries = pawn.health.surgeryBills;

            if (surgeries == null || surgeries.Count <= 0)
            {
                return segments.GroupedJoin();
            }

            string[] queued = surgeries.Bills.Select(item => item.LabelCap).ToArray();

            segments.Add(
                ResponseHelper.JoinPair("TKUtils.PawnHealth.QueuedSurgeries".Localize(), queued.SectionJoin())
            );

            return segments.GroupedJoin();
        }
    }
}