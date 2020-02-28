using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class PawnHealthCommand : CommandBase
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

            var segment = message.Message.Split(' ').Skip(1).FirstOrDefault();

            if(!segment.NullOrEmpty())
            {
                var cap = DefDatabase<PawnCapacityDef>.AllDefsListForReading.Where(d => d.defName.EqualsIgnoreCase(segment));

                if(cap.Any())
                {
                    SendMessage(
                        "TKUtils.Responses.Format".Translate(
                            NamedArgumentUtility.Named(message.User, "VIEWER"),
                            NamedArgumentUtility.Named(HealthCapacityReport(pawn, cap.First()), "MESSAGE")
                        ),
                        message
                    );
                }
                else
                {
                    SendMessage(
                        "TKUtils.Responses.Format".Translate(
                            NamedArgumentUtility.Named(message.User, "VIEWER"),
                            NamedArgumentUtility.Named(
                                "TKUtils.Responses.NoCapacity".Translate(
                                    NamedArgumentUtility.Named(segment, "CAPACITY")
                                ),
                                "MESSAGE"
                            )
                        ),
                        message
                    );
                }
            }
            else
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named(HealthReport(pawn), "MESSAGE")
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
                    return "💫";

                case PawnHealthState.Dead:
                    return "👻";

                default:
                    return $"[{Gen.ToStringSafe(state)}]";
            }
        }

        private string GetFriendlyMoodState(Pawn pawn)
        {
            if(pawn.MentalStateDef != null) return "😐";

            var breakThresholdExtreme = pawn.mindState.mentalBreaker.BreakThresholdExtreme;

            if(pawn.needs.mood.CurLevel < breakThresholdExtreme) return "🤬";
            if(pawn.needs.mood.CurLevel < breakThresholdExtreme + 0.05f) return "😡";
            if(pawn.needs.mood.CurLevel < pawn.mindState.mentalBreaker.BreakThresholdMinor) return "😤";
            if(pawn.needs.mood.CurLevel < 0.65f) return "😐";
            if(pawn.needs.mood.CurLevel < 0.9f) return "🙁";

            return "🙂";
        }

        private string HealthCapacityReport(Pawn pawn, PawnCapacityDef capacity)
        {
            if(!PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity))
            {
                return "TKUtils.Responses.RaceCapacityError".Translate(pawn.kindDef.race.defName.Named("RACE"), capacity.GetLabelFor(pawn).Named("CAPACITY"));
            }

            var effLabel = HealthCardUtility.GetEfficiencyLabel(pawn, capacity).First;
            var impactors = new List<PawnCapacityUtility.CapacityImpactor>();
            var builder = new StringBuilder();

            builder.Append($"{capacity.LabelCap} (");
            builder.Append($"{(PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, capacity, impactors) * 100f).ToString("F0")}%");
            builder.Append($") | {"TKUtils.Responses.CapacityAffectedBy".Translate()} ");

            if(impactors.Any())
            {
                var segments = new List<string>();

                foreach(var i in impactors)
                {
                    if(i is PawnCapacityUtility.CapacityImpactorHediff)
                    {
                        segments.Add(i.Readable(pawn));
                    }
                }

                foreach(var i in impactors)
                {
                    if(i is PawnCapacityUtility.CapacityImpactorBodyPartHealth)
                    {
                        segments.Add(i.Readable(pawn));
                    }
                }

                foreach(var i in impactors)
                {
                    if(i is PawnCapacityUtility.CapacityImpactorCapacity)
                    {
                        segments.Add(i.Readable(pawn));
                    }
                }

                foreach(var i in impactors)
                {
                    if(i is PawnCapacityUtility.CapacityImpactorPain)
                    {
                        segments.Add(i.Readable(pawn));
                    }
                }

                return string.Join(", ", segments);
            }
            else
            {
                return "TKUtils.Responses.CapacityUnaffected".Translate();
            }
        }

        private string HealthReport(Pawn pawn)
        {
            var builder = new StringBuilder($"{(pawn.health.summaryHealth.SummaryHealthPercent * 100f).ToString("n1")}%");

            if(pawn.health.State != PawnHealthState.Mobile)
            {
                builder.Append(GetFriendlyHealthState(pawn.health.State));
            }
            else
            {
                builder.Append(GetFriendlyMoodState(pawn));
            }

            if(pawn.health.hediffSet.BleedRateTotal > 0.01f)
            {
                builder.Append(" Bleeding: ");

                var ticks = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);

                if(ticks >= 60000)
                {
                    builder.Append("⌛");
                }
                else
                {
                    builder.Append($"⏳ ({GenDate.ToStringTicksToPeriod(ticks, shortForm: true)})");
                }
            }

            builder.Append(" | ");

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
                builder.Append("TKUtils.Responses.UnsupportedRace".Translate(pawn.kindDef.race.defName.Named("RACE")));
            }

            var segments = new List<string>();
            foreach(var def in source.OrderBy(d => d.listOrder))
            {
                if(PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, def))
                {
                    segments.Add($"{def.GetLabelFor(pawn).CapitalizeFirst()}: {HealthCardUtility.GetEfficiencyLabel(pawn, def).First}");
                }
            }

            if(source.Any()) builder.Append(string.Join(", ", segments));

            if(Settings.ShowSurgeries)
            {
                var surgeries = pawn.health.surgeryBills;

                if(surgeries != null && surgeries.Count > 0)
                {
                    builder.Append(" | Queued Surgeries: ");
                    segments.Clear();
                    foreach(var surgery in surgeries)
                    {
                        segments.Add(surgery.LabelCap);
                    }

                    builder.Append(string.Join(", ", segments));
                }
            }

            return builder.ToString();
        }
    }
}
