using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.IRC;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnHealthCommand : CommandBase
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
                message.Reply("TKUtils.Responses.NoPawn".Translate().WithHeader("TabHealth".Translate()));
                return;
            }

            var segment = message.Message.Split(' ').Skip(1).FirstOrDefault();

            if (segment.NullOrEmpty())
            {
                message.Reply(HealthReport(pawn).WithHeader("TabHealth".Translate()));
                return;
            }

            var cap = DefDatabase<PawnCapacityDef>.AllDefsListForReading.Where(
                    d => d.defName.EqualsIgnoreCase(segment)
                )
                .ToArray();


            message.Reply(
                (cap.Any()
                    ? HealthCapacityReport(pawn, cap.First())
                    : "TKUtils.Responses.PawnHealth.Capacity.None".Translate(segment).ToString()
                ).WithHeader("TabHealth".Translate())
            );
        }

        private static string GetHealthStateFriendly(PawnHealthState state)
        {
            switch (state)
            {
                case PawnHealthState.Down:
                    return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Downed").Translate();

                case PawnHealthState.Dead:
                    return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Dead").Translate();

                default:
                    return string.Empty;
            }
        }

        private static string GetMoodFriendly(Pawn subject)
        {
            if (subject.MentalStateDef != null)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Mood.None").Translate();
            }

            var thresholdExtreme = subject.mindState.mentalBreaker.BreakThresholdExtreme;
            var moodLevel = subject.needs.mood.CurLevel;

            if (moodLevel < thresholdExtreme)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Mood.Extreme").Translate();
            }

            if (moodLevel < thresholdExtreme + 0.0500000007450581)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Mood.Upset").Translate();
            }

            if (moodLevel < subject.mindState.mentalBreaker.BreakThresholdMinor)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Mood.Stressed").Translate();
            }

            if (moodLevel < 0.649999976158142)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Mood.Bad").Translate();
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
                    Find.ActiveLanguageWorker.Pluralize(pawn.kindDef.race.defName).Named("RACE"),
                    capacity.GetLabelFor(pawn).Named("CAPACITY")
                );
            }

            var impactors = new List<PawnCapacityUtility.CapacityImpactor>();
            var segments = new List<string>
            {
                "TKUtils.Formats.PawnHealth.Capacity".Translate(
                    capacity.LabelCap.Named("CAPACITY"),
                    GenText.ToStringPercent(
                        PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, capacity, impactors)
                    ).Named("PERCENT")
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
                    "TKUtils.Formats.PawnHealth.Impactors".Translate(
                        string.Join(
                            "TKUtils.Misc.Separators.Inner".Translate(),
                            parts
                        ).Named("IMPACTORS")
                    )
                );
            }
            else
            {
                segments.Add("TKUtils.Responses.Healthy".Translate());
            }

            return string.Join(
                "TKUtils.Misc.Separators.Upper".Translate(),
                segments
            );
        }

        private static string HealthReport(Pawn pawn)
        {
            var segments = new List<string>()
            {
                "TKUtils.Formats.PawnHealth.Summary".Translate(
                    GenText.ToStringPercent(pawn.health.summaryHealth.SummaryHealthPercent).Named("PERCENT")
                )
            };

            if (pawn.health.State != PawnHealthState.Mobile)
            {
                segments[0] += " " + GetFriendlyHealthState(pawn.health.State);
            }
            else
            {
                segments[0] += " " + GetFriendlyMoodState(pawn);
            }

            if (pawn.health.hediffSet.BleedRateTotal > 0.01f)
            {
                var ticks = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);

                if(ticks >= 60000)
                {
                    segments.Add(
                        GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Bleeding.NoDanger").Translate()
                    );
                }
                else
                {
                    segments.Add(
                        GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Bleeding.Danger")
                            .Translate(
                                GenDate.ToStringTicksToPeriod(ticks, shortForm: true).Named("TIME")
                            )
                    );
                }
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
                        pawn.kindDef.race.defName.Named("RACE")
                    )
                );
            }

            if (source.Any())
            {
                source = source.OrderBy(d => d.listOrder).ToList();

                foreach(var capacity in source)
                {
                    if(!PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity))
                    {
                        continue;
                    }

                    capacities.Add(
                        "TKUtils.Formats.PawnHealth.Capacity".Translate(
                            capacity.GetLabelFor(pawn).CapitalizeFirst().Named("CAPACITY"),
                            HealthCardUtility.GetEfficiencyLabel(pawn, capacity).First.Named("PERCENT")
                        )
                    );
                }

                segments.Add(
                    string.Join(
                        "TKUtils.Misc.Separators.Inner".Translate(),
                        capacities
                    )
                );
            }

            if (!TkSettings.ShowSurgeries)
            {
                var surgeries = pawn.health.surgeryBills;

                if(surgeries != null && surgeries.Count > 0)
                {
                    var queued = new List<string>();

                    foreach(var item in surgeries.Bills)
                    {
                        queued.Add(item.LabelCap);
                    }

                    segments.Add(
                        "TKUtils.Formats.PawnHealth.Surgeries".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                queued
                            ).Named("SURGERIES")
                        )
                    );
                }
            }

            return string.Join(
                "TKUtils.Misc.Separators.Upper".Translate(),
                segments
            );
        }
    }
}
