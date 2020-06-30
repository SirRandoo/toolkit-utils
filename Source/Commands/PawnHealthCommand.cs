using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnHealthCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabHealth".Translate()));
                return;
            }

            string segment = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrFallback("");

            if (segment.NullOrEmpty())
            {
                twitchMessage.Reply(HealthReport(pawn).WithHeader("TabHealth".Translate()));
                return;
            }

            PawnCapacityDef capacity = DefDatabase<PawnCapacityDef>.AllDefsListForReading
                .FirstOrDefault(
                    d => d.defName.EqualsIgnoreCase(segment)
                         || d.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(segment.ToToolkit())
                );

            twitchMessage.Reply(
                (
                    capacity == null
                        ? HealthReport(pawn)
                        : HealthCapacityReport(pawn, capacity)
                ).WithHeader("TabHealth".Translate())
            );
        }

        private static string GetHealthStateFriendly(PawnHealthState state)
        {
            switch (state)
            {
                case PawnHealthState.Down:
                    return "💫".AltText("DownedLower".Translate().CapitalizeFirst());
                case PawnHealthState.Dead:
                    return "👻".AltText("Dead".Translate());
                default:
                    return string.Empty;
            }
        }

        private static string GetMoodFriendly(Pawn subject)
        {
            if (subject.MentalStateDef != null)
            {
                return "⚡".AltText(subject.MentalStateDef.LabelCap);
            }

            float thresholdExtreme = subject.mindState.mentalBreaker.BreakThresholdExtreme;
            float moodLevel = subject.needs.mood.CurLevel;

            if (moodLevel < thresholdExtreme)
            {
                return "🤬".AltText("Mood_AboutToBreak");
            }

            if (moodLevel < thresholdExtreme + 0.0500000007450581)
            {
                return "😠".AltText("Mood_OnEdge");
            }

            if (moodLevel < subject.mindState.mentalBreaker.BreakThresholdMinor)
            {
                return "😣".AltText("Mood_Stressed");
            }

            if (moodLevel < 0.649999976158142)
            {
                return "😐".AltText("Mood_Neutral");
            }

            return moodLevel < 0.899999976158142
                ? "🙂".AltText("Mood_Content".Translate())
                : "😊".AltText("Mood_Happy".Translate());
        }

        private static string HealthCapacityReport(Pawn pawn, PawnCapacityDef capacity)
        {
            if (!PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity))
            {
                return "TKUtils.Responses.PawnHealth.Capacity.Race".Translate(
                    Find.ActiveLanguageWorker.Pluralize(pawn.kindDef.race.defName),
                    capacity.GetLabelFor(pawn)
                );
            }

            var impactors = new List<PawnCapacityUtility.CapacityImpactor>();
            var segments = new List<string>
            {
                "TKUtils.Formats.KeyValue".Translate(
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

                segments.Add(
                    "TKUtils.Formats.PawnHealth.Impactors".Translate(string.Join(", ", parts))
                );
            }
            else
            {
                segments.Add("NoHealthConditions".Translate().CapitalizeFirst());
            }

            return string.Join("⎮", segments.ToArray());
        }

        private static string HealthReport(Pawn pawn)
        {
            var segments = new List<string>
            {
                "TKUtils.Formats.PawnHealth.Summary".Translate(
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
                        ? "🩸⌛".AltText("WontBleedOutSoon".Translate().CapitalizeFirst())
                        : $"{"🩸⏳".AltText("BleedingRate".Translate().RawText)} ({ticks.ToStringTicksToPeriod(shortForm: true)})"
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

                segments.Add(
                    "TKUtils.Responses.UnsupportedRace".Translate(
                        pawn.kindDef.race.defName
                    )
                );
            }

            if (source.Any())
            {
                source = source.OrderBy(d => d.listOrder).ToList();

                string[] capacities = source
                    .Where(capacity => PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity))
                    .Select(
                        capacity => "TKUtils.Formats.KeyValue".Translate(
                            capacity.GetLabelFor(pawn).CapitalizeFirst(),
                            HealthCardUtility.GetEfficiencyLabel(pawn, capacity).First
                        )
                    )
                    .Select(dummy => (string) dummy)
                    .ToArray();

                segments.Add(string.Join(", ", capacities));
            }

            if (!TkSettings.ShowSurgeries)
            {
                return string.Join("⎮", segments.ToArray());
            }

            BillStack surgeries = pawn.health.surgeryBills;

            if (surgeries == null || surgeries.Count <= 0)
            {
                return string.Join("⎮", segments.ToArray());
            }

            string[] queued = surgeries.Bills.Select(item => item.LabelCap).ToArray();

            segments.Add("TKUtils.Formats.PawnHealth.Surgeries".Translate(string.Join(", ", queued)));

            return string.Join("⎮", segments.ToArray());
        }
    }
}
