using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnHealthCommand : CommandBase
    {
        private Pawn pawn;
        private PawnCapacityDef capacity;
        
        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            pawn = GetOrFindPawn(twitchCommand.Message);

            if (pawn == null)
            {
                twitchCommand.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabHealth".Translate()));
                return false;
            }

            var segment = CommandParser.Parse(twitchCommand.Message, TkSettings.Prefix).Skip(1).FirstOrFallback("");

            if (segment.NullOrEmpty())
            {
                return true;
            }

            capacity = DefDatabase<PawnCapacityDef>.AllDefsListForReading
                .FirstOrDefault(
                    d => d.defName.EqualsIgnoreCase(segment)
                         || d.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(segment.ToToolkit())
                );

            if (capacity != null)
            {
                return true;
            }

            twitchCommand.Reply("TKUtils.Responses.PawnHealth.Capacity.None".Translate(segment).ToString());
            return false;

        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            twitchCommand.Reply(
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

            var thresholdExtreme = subject.mindState.mentalBreaker.BreakThresholdExtreme;
            var moodLevel = subject.needs.mood.CurLevel;

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

                foreach (var i in impactors)
                {
                    if (i is PawnCapacityUtility.CapacityImpactorHediff)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach (var i in impactors)
                {
                    if (i is PawnCapacityUtility.CapacityImpactorBodyPartHealth)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach (var i in impactors)
                {
                    if (i is PawnCapacityUtility.CapacityImpactorCapacity)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach (var i in impactors)
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
                var ticks = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);

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

                var capacities = source
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

            var surgeries = pawn.health.surgeryBills;

            if (surgeries == null || surgeries.Count <= 0)
            {
                return string.Join("⎮", segments.ToArray());
            }

            var queued = surgeries.Bills.Select(item => item.LabelCap).ToArray();

            segments.Add("TKUtils.Formats.PawnHealth.Surgeries".Translate(string.Join(", ", queued)));

            return string.Join("⎮", segments.ToArray());
        }

        public PawnHealthCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}
