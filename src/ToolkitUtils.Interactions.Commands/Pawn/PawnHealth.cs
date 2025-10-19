// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Interactions.Commands. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Presentation;
using Verse;

namespace ToolkitUtils.Interactions.Commands;

[Group("pawn")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnHealth(HealthSettings settings, ExecutionContext context) : CommandGroup
{
    [Command("health")]
    public Task<Result> RunCommand(string? capacityQuery = null)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Task.FromResult(Result.Fail(TranslationService.Instance.FormatPawnRequiredSelf()));
        if (capacityQuery == null) return Task.FromResult<Result>(Result.Ok($"{TranslationService.Instance.GetTranslation("TabHealth")} {HealthReport(pawn)}"));

        PawnCapacityDef? capacity = DefDatabase<PawnCapacityDef>.AllDefs.FirstOrDefault(d => d.defName.EqualsIgnoreCase(capacityQuery)
                                                                                          || d.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(capacityQuery.ToToolkit())
        );

        return Task.FromResult<Result>(
            Result.Ok(
                string.Format(
                    format: "[{0}] {1}",
                    TranslationService.Instance.GetTranslation("TabHealth"),
                    capacity == null ? HealthReport(pawn) : HealthCapacityReport(pawn, capacity)
                )
            )
        );
    }

    private static string GetHealthStateFriendly(PawnHealthState state)
    {
        return state switch
        {
            PawnHealthState.Down => UnicodeCharacter.SwirlingStar.Codepoint,
            PawnHealthState.Dead => UnicodeCharacter.Ghost.Codepoint,
            var _                => string.Empty,
        };
    }

    private static string GetMoodFriendly(Pawn subject)
    {
        if (subject.MentalStateDef != null) return UnicodeCharacter.Lighting.Codepoint;

        float thresholdExtreme = subject.mindState.mentalBreaker.BreakThresholdExtreme;
        float moodLevel = subject.needs.mood.CurLevel;

        if (moodLevel < thresholdExtreme) return UnicodeCharacter.FaceWithSymbolsOnMouth.Codepoint;
        if (moodLevel < thresholdExtreme + 0.0500000007450581) return UnicodeCharacter.AngryFace.Codepoint;
        if (moodLevel < subject.mindState.mentalBreaker.BreakThresholdMinor) return UnicodeCharacter.PerseveringFace.Codepoint;
        if (moodLevel < 0.649999976158142) return UnicodeCharacter.NeutralFace.Codepoint;

        return moodLevel < 0.899999976158142 ? UnicodeCharacter.SlightlySmilingFace.Codepoint : UnicodeCharacter.SmilingFace.Codepoint;
    }

    private string HealthCapacityReport(Pawn pawn, PawnCapacityDef capacity)
    {
        if (!PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity)) return TranslationService.Instance.FormatIncapableOfCapacity(capacity, pawn);

        var impactors = new List<PawnCapacityUtility.CapacityImpactor>();

        var segments = new List<string>
        {
            string.Format(
                format: "{0}: {1}",
                RichTextHelper.StripTags(capacity.LabelCap),
                PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, capacity, impactors).ToStringPercent()
            ),
            impactors.Any()
                ? string.Format(
                    format: "{0}: {1}",
                    TranslationService.Instance.GetTranslation("AffectedBy").Value,
                    string.Join(separator: ", ", GetImpactorsForPawn(pawn, impactors))
                )
                : TranslationService.Instance.GetTranslation("NoHealthConditions").CapitalizeFirst().Value,
        };

        return string.Join(separator: " | ", segments);
    }

    private static IEnumerable<string> GetImpactorsForPawn(Pawn pawn, IReadOnlyCollection<PawnCapacityUtility.CapacityImpactor> impactors)
    {
        List<string> parts = impactors.OfType<PawnCapacityUtility.CapacityImpactorHediff>().Select(i => i.Readable(pawn)).ToList();

        parts.AddRange(impactors.OfType<PawnCapacityUtility.CapacityImpactorBodyPartHealth>().Select(i => i.Readable(pawn)));
        parts.AddRange(impactors.OfType<PawnCapacityUtility.CapacityImpactorCapacity>().Select(i => i.Readable(pawn)));
        parts.AddRange(impactors.OfType<PawnCapacityUtility.CapacityImpactorPain>().Select(i => i.Readable(pawn)));

        return parts;
    }

    private string HealthReport(Pawn pawn)
    {
        var segments = new List<string>
        {
            string.Join(
                separator: "{0}: {1}",
                TranslationService.Instance.GetTranslation("TKUtils.Responses.PawnHealth.OverallHealth").Value,
                pawn.health.summaryHealth.SummaryHealthPercent.ToStringPercent()
            ),
        };

        if (pawn.health.State != PawnHealthState.Mobile)
            segments[0] += $" {GetHealthStateFriendly(pawn.health.State)}";
        else
            segments[0] += $" {GetMoodFriendly(pawn)}";

        if (pawn.health.hediffSet.BleedRateTotal > 0.01f)
        {
            int ticks = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);

            segments.Add(
                ticks >= 60000
                    ? UnicodeCharacter.BloodWithHourglassDone.Codepoint
                    : $"{UnicodeCharacter.BloodWithHourglassNotDone.Codepoint} ({ticks.ToStringTicksToPeriod(shortForm: true)})"
            );
        }

        IReadOnlyList<PawnCapacityDef> source = GetCapacitiesForPawn(pawn);

        if (source.Count > 0)
        {
            source = source.OrderBy(d => d.listOrder).ToList();

            string[] capacities = source.Where(capacity => PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, capacity)).Select(capacity => string.Format(
                    format: "{0}: {1}",
                    RichTextHelper.StripTags(capacity.GetLabelFor(pawn)).CapitalizeFirst(),
                    HealthCardUtility.GetEfficiencyLabel(pawn, capacity).First
                )
            ).ToArray();

            segments.Add(string.Join(separator: ", ", capacities));
        }
        else
            segments.Add(TranslationService.Instance.FormatKindIncapableOfCapacity(pawn.kindDef));

        if (!settings.ShouldShowSurgeries) return string.Join(separator: " | ", segments);

        BillStack surgeries = pawn.health.surgeryBills;

        if (surgeries?.Count <= 0) return string.Join(separator: " | ", segments);

        string[] queued = surgeries!.Bills.Select(item => RichTextHelper.StripTags(item.LabelCap)).ToArray();

        segments.Add($"{TranslationService.Instance.GetTranslation("TKUtils.Responses.PawnHealth.QueuedSurgeries")}: {string.Join(separator: ", ", queued)}");

        return string.Join(separator: " | ", segments);
    }

    private static IReadOnlyList<PawnCapacityDef> GetCapacitiesForPawn(Pawn pawn)
    {
        return DefDatabase<PawnCapacityDef>.AllDefs.Where(c => pawn.health.capacities.CapableOf(c)).ToList();
    }
}
