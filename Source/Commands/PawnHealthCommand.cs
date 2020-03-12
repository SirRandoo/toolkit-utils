using System.Collections.Generic;
using System.Linq;

using RimWorld;

using SirRandoo.ToolkitUtils.Utils;

using TwitchLib.Client.Models;

using TwitchToolkit;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnHealthCommand : CommandBase
    {
        public override void RunCommand(ChatMessage message)
        {
            if(!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var pawn = GetPawnDestructive(message.Username);

            if(pawn == null)
            {
                SendCommandMessage(
                    "TKUtils.Responses.NoPawn".Translate(),
                    message
                );
                return;
            }

            var segment = message.Message.Split(' ').Skip(1).FirstOrDefault();

            if(segment.NullOrEmpty())
            {
                SendCommandMessage(HealthReport(pawn), message);
                return;
            }

            var cap = DefDatabase<PawnCapacityDef>.AllDefsListForReading.Where(d => d.defName.EqualsIgnoreCase(segment));

            if(cap.Any())
            {
                SendCommandMessage(
                    HealthCapacityReport(pawn, cap.First()),
                    message
                );
            }
            else
            {
                SendCommandMessage(
                    "TKUtils.Responses.PawnHealth.Capacity.None".Translate(
                        segment.Named("QUERY")
                    ),
                    message
                );
            }
        }

        private string GetFriendlyHealthState(PawnHealthState state)
        {
            switch(state)
            {
                case PawnHealthState.Down:
                    return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Downed").Translate();

                case PawnHealthState.Dead:
                    return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Dead").Translate();

                default:
                    return $"[{Gen.ToStringSafe(state)}]";
            }
        }

        private string GetFriendlyMoodState(Pawn pawn)
        {
            if(pawn.MentalStateDef != null)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.None").Translate();
            }

            var breakThresholdExtreme = pawn.mindState.mentalBreaker.BreakThresholdExtreme;

            if(pawn.needs.mood.CurLevel < breakThresholdExtreme)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Extreme").Translate();
            }

            if(pawn.needs.mood.CurLevel < breakThresholdExtreme + 0.05f)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Upset").Translate();
            }

            if(pawn.needs.mood.CurLevel < pawn.mindState.mentalBreaker.BreakThresholdMinor)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Stressed").Translate();
            }

            if(pawn.needs.mood.CurLevel < 0.65f)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Bad");
            }

            if(pawn.needs.mood.CurLevel < 0.9f)
            {
                return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Neutral").Translate();
            }

            return GetTranslatedEmoji("TKUtils.Responses.PawnHealth.Happy").Translate();
        }

        private string HealthCapacityReport(Pawn pawn, PawnCapacityDef capacity)
        {
            if(!PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity))
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
                    string.Format(
                        "{0:P2}",
                        PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, capacity, impactors)
                    ).Named("PERCENT")
                )
            };

            if(impactors.Any())
            {
                var parts = new List<string>();

                foreach(var i in impactors)
                {
                    if(i is PawnCapacityUtility.CapacityImpactorHediff)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach(var i in impactors)
                {
                    if(i is PawnCapacityUtility.CapacityImpactorBodyPartHealth)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach(var i in impactors)
                {
                    if(i is PawnCapacityUtility.CapacityImpactorCapacity)
                    {
                        parts.Add(i.Readable(pawn));
                    }
                }

                foreach(var i in impactors)
                {
                    if(i is PawnCapacityUtility.CapacityImpactorPain)
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

        private string HealthReport(Pawn pawn)
        {
            var segments = new List<string>()
            {
                "TKUtils.Formats.PawnHealth.Summary".Translate(
                    string.Format(
                        "{0:P2}",
                        pawn.health.summaryHealth.SummaryHealthPercent
                    ).Named("PERCENT")
                )
            };

            if(pawn.health.State != PawnHealthState.Mobile)
            {
                segments[0] += " " + GetFriendlyHealthState(pawn.health.State);
            }
            else
            {
                segments[0] += " " + GetFriendlyMoodState(pawn);
            }

            if(pawn.health.hediffSet.BleedRateTotal > 0.01f)
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

            IEnumerable<PawnCapacityDef> source;
            if(pawn.def.race.Humanlike)
            {
                source = DefDatabase<PawnCapacityDef>.AllDefs.Where(d => d.showOnHumanlikes);
            }
            else if(pawn.def.race.Animal)
            {
                source = DefDatabase<PawnCapacityDef>.AllDefs.Where(d => d.showOnAnimals);
            }
            else if(pawn.def.race.IsMechanoid)
            {
                source = DefDatabase<PawnCapacityDef>.AllDefs.Where(d => d.showOnMechanoids);
            }
            else
            {
                source = new List<PawnCapacityDef>();

                segments.Add(
                    "TKUtils.Responses.UnsupportedRace".Translate(
                        pawn.kindDef.race.defName.Named("RACE")
                    )
                );
            }

            if(source.Any())
            {
                segments.Add(
                    string.Join(
                        "TKUtils.Misc.Separators.Inner".Translate(),
                        source.OrderBy(d => d.listOrder)
                            .Where(d => PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, d))
                            .Select(d => "TKUtils.Formats.PawnHealth.Capacity".Translate(
                                d.GetLabelFor(pawn).CapitalizeFirst().Named("CAPACITY"),
                                HealthCardUtility.GetEfficiencyLabel(pawn, d).First.Named("PERCENT")
                            )
                        )
                    )
                );
            }

            if(TKSettings.ShowSurgeries)
            {
                var surgeries = pawn.health.surgeryBills;

                if(surgeries != null && surgeries.Count > 0)
                {
                    segments.Add(
                        "TKUtils.Formats.PawnHealth.Surgeries".Translate(
                            string.Join(
                                "TKUtils.Misc.Separators.Inner".Translate(),
                                surgeries.Bills.Select(s => s.LabelCap)
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
