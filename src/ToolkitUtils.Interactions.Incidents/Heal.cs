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
// with ToolkitUtils.Interactions.Incidents. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Extensions;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Interactions.Incidents.Extensions;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class HealBuyGroup : CommandGroup
{
    [Group("heal")]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class Heal(ExecutionContext context) : CommandGroup
    {
        public enum HealTarget
        {
            Auto, Part, Hediff,
        }

        private const string HealLetterTitleKey = "TKUtils.Letters.Heal.Title";

        private static readonly float HandCoverageAbsWithChildren = ThingDefOf.Human.race.body.GetPartsWithDef(BodyPartDefOf.Hand)[0].coverageAbsWithChildren;

        [Command("self")]
        public async Task<Result> HealSelfAsync(HealTarget target = HealTarget.Auto, string? query = null)
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.GetPawnRequiredError(context.Invoker));

            Result<Hediff?> affliction = await ProcessHealTargetAsync(pawn, target, query);

            if (!affliction.IsSuccess) return affliction;

            Hediff healTarget = affliction.Value!;

            context.Invoker.Charge(StoreIncidentDefOfs.Heal);
            await MainThreadExtensions.OnMainAsync(HealthUtility.Cure, healTarget);
            await context.SendReplyAsync(TranslationService.Instance.FormatSelfHeal(healTarget));
            await Find.LetterStack.ReceiveLetterAsync(
                TranslationService.Instance.GetTranslation(HealLetterTitleKey),
                TranslationService.Instance.FormatSelfHealLetterDescription(context.Invoker),
                LetterDefOf.PositiveEvent,
                pawn
            );

            return Result.Ok();
        }

        [Command("all")]
        public async Task<IResult> HealAllAsync()
        {
            Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

            if (pawn == null) return Result.Fail(TranslationService.Instance.GetPawnRequiredError(context.Invoker));

            var healedAfflictionCount = 0;

            while (true)
            {
                Hediff? target = await GetCandidateAsync(pawn);

                if (target == null) break;

                Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.Heal.cost);

                if (!transaction.IsSuccess) break;

                await MainThreadExtensions.OnMainAsync(HealthUtility.Cure, target);

                healedAfflictionCount++;
                context.Invoker.FinalizeTransaction(transaction.Value);
            }

            if (healedAfflictionCount <= 0) return Result.Fail(TranslationService.Instance.GetNoOneInjuredError());

            await context.SendReplyAsync(TranslationService.Instance.FormatSelfFullHeal(healedAfflictionCount));
            await Find.LetterStack.ReceiveLetterAsync(
                TranslationService.Instance.GetTranslation(HealLetterTitleKey),
                TranslationService.Instance.FormatFullHealLetterDescription(context.Invoker),
                LetterDefOf.PositiveEvent,
                pawn
            );

            return Result.Ok();
        }

        [Command("colony")]
        public async Task<IResult> HealColonyAsync(HealTarget target = HealTarget.Auto, string? query = null)
        {
            IReadOnlyList<ViewerPawnRegistry.ViewerPawn> allRegisteredPawns = ViewerPawnRegistry.AllRegistrants;
            var afflictions = new Hediff[allRegisteredPawns.Count];
            var insertionPosition = 0;

            for (var i = 0; i < allRegisteredPawns.Count; i++)
            {
                Pawn pawn = allRegisteredPawns[i].Pawn;
                Result<Hediff?> affliction = await ProcessHealTargetAsync(pawn, target, query);

                if (!affliction.IsSuccess) continue;

                afflictions[insertionPosition++] = affliction.Value!;
            }

            if (insertionPosition <= 0) return Result.Fail(TranslationService.Instance.GetNoOneInjuredError());

            int healCost = StoreIncidentDefOfs.Heal.cost * (insertionPosition + 1);
            Result<Transaction> transaction = context.Invoker.ReserveCoins(healCost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(healCost, context.Invoker.Coins));

            var healedAfflictionCount = 0;

            for (var i = 0; i < afflictions.Length; i++)
            {
                Hediff hediff = afflictions[i];

                if (!hediff.pawn.health.hediffSet.hediffs.Contains(hediff)) continue;

                await MainThreadExtensions.OnMainAsync(HealthUtility.Cure, hediff);
                healedAfflictionCount++;
            }

            await context.SendReplyAsync(TranslationService.Instance.FormatColonyHeal(healedAfflictionCount));
            await Find.LetterStack.ReceiveLetterAsync(
                TranslationService.Instance.GetTranslation(HealLetterTitleKey),
                TranslationService.Instance.FormatColonyHealLetterDescription(context.Invoker),
                LetterDefOf.PositiveEvent
            );

            return Result.Ok();
        }

        [Command("random")]
        public async Task<IResult> HealRandomAsync(HealTarget target = HealTarget.Auto, string? query = null)
        {
            IReadOnlyList<ViewerPawnRegistry.ViewerPawn> allRegisteredPawns = ViewerPawnRegistry.AllRegistrants;
            var candidates = new (Pawn Pawn, Hediff Hediff)[Math.Min(allRegisteredPawns.Count, val2: 10)];
            var insertPosition = 0;
            var candidateCount = 0;

            Result<Transaction> transaction = context.Invoker.ReserveCoins(StoreIncidentDefOfs.Heal.cost);

            if (!transaction.IsSuccess) return Result.Fail(TranslationService.Instance.GetInsufficientBalanceError(StoreIncidentDefOfs.Heal.cost, context.Invoker.Coins));

            for (var i = 0; i < allRegisteredPawns.Count; i++)
            {
                ViewerPawnRegistry.ViewerPawn pawn = allRegisteredPawns[i];
                Result<Hediff?> affliction = await ProcessHealTargetAsync(pawn.Pawn, target, query);

                if (!affliction.IsSuccess) continue;

                candidates[insertPosition++] = new ValueTuple<Pawn, Hediff>(pawn.Pawn, affliction.Value!);
                candidateCount++;

                if (candidateCount == candidates.Length) break;
            }

            int randomCandidatePosition = ThreadSafeRandom.Next(minimum: 0, candidateCount);

            (Pawn Pawn, Hediff Hediff) candidate = candidates[randomCandidatePosition];

            if (candidate.Hediff is Hediff_MissingPart) await MainThreadExtensions.OnMainAsync(candidate.Pawn.health.RestorePart, candidate.Hediff.Part, (Hediff?)null, arg3: true);

            await MainThreadExtensions.OnMainAsync(HealthUtility.Cure, candidate.Hediff);
            transaction.Value.PostTransaction();

            await Find.LetterStack.ReceiveLetterAsync(
                TranslationService.Instance.GetTranslation(HealLetterTitleKey),
                TranslationService.Instance.FormatRandomHealLetterDescription(context.Invoker, candidate.Pawn),
                LetterDefOf.PositiveEvent,
                candidate.Pawn
            );

            return Result.Ok();
        }

        private async Task<Result<Hediff?>> ProcessHealTargetAsync(Pawn pawn, HealTarget target, string? query = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                return target switch
                {
                    HealTarget.Auto   => Result.Ok<Hediff?>(await GetCandidateAsync(pawn)),
                    HealTarget.Part   => Result.Ok<Hediff?>(await GetBodyPartRecordCandidateAsync(pawn)),
                    HealTarget.Hediff => Result.Ok<Hediff?>(await GetHediffCandidateAsync(pawn)),
                    var _             => throw new InvalidOperationException($"The heal target '{target}' is not supported."),
                };
            }

            Hediff? potentialHediff = await FindHediffNamedAsync(pawn, query!);

            if (potentialHediff != null) return Result.Ok<Hediff?>(potentialHediff);

            Hediff_MissingPart? potentialBodyPart = await FindBodyPartNamedAsync(pawn, query!);

            return potentialBodyPart != null ? Result.Ok<Hediff?>(potentialBodyPart) : Result.Fail<Hediff?>(TranslationService.Instance.FormatInvalidQuery(query!));
        }

        private static ValueTask<Hediff?> FindHediffNamedAsync(Pawn pawn, string query)
        {
            HediffDef? hediff = null;
            string? squashedQuery = null;

            foreach (HediffDef def in DefDatabase<HediffDef>.AllDefs)
            {
                if (string.Equals(def.defName, query, StringComparison.OrdinalIgnoreCase) || string.Equals(def.label, query, StringComparison.InvariantCultureIgnoreCase))
                {
                    hediff = def;

                    break;
                }

                squashedQuery ??= query.ToToolkit();

                if (!string.Equals(def.label.ToToolkit(), squashedQuery, StringComparison.InvariantCultureIgnoreCase)) continue;

                hediff = def;

                break;
            }

            if (hediff != null) return new ValueTask<Hediff?>(pawn.health.hediffSet.TryGetHediff(hediff, out Hediff hediffData) ? hediffData : null);

            return new ValueTask<Hediff?>((Hediff)null!);
        }

        private static ValueTask<Hediff_MissingPart?> FindBodyPartNamedAsync(Pawn pawn, string query)
        {
            BodyPartDef? bodyPartDef = null;
            string? squashedQuery = null;

            foreach (BodyPartDef def in DefDatabase<BodyPartDef>.AllDefs)
            {
                if (string.Equals(def.defName, query, StringComparison.OrdinalIgnoreCase) || string.Equals(def.label, query, StringComparison.InvariantCultureIgnoreCase))
                {
                    bodyPartDef = def;

                    break;
                }

                squashedQuery ??= query.ToToolkit();

                if (!string.Equals(def.label.ToToolkit(), squashedQuery, StringComparison.InvariantCultureIgnoreCase)) continue;

                bodyPartDef = def;

                break;
            }

            return new ValueTask<Hediff_MissingPart?>(
                (Hediff_MissingPart?)(bodyPartDef != null
                    ? pawn.health.hediffSet.hediffs.Find(h => h is Hediff_MissingPart missingPart && missingPart.Part.def == bodyPartDef)
                    : null)
            );
        }

        private static async Task<Hediff?> GetCandidateAsync(Pawn pawn)
        {
            IReadOnlyList<IHealProvider> healProviders = Registries.Compatibilities.AllRegistrants.OfType<IHealProvider>().ToList();
            Hediff[] pawnHediffsSnapshot = await MainThreadExtensions.OnMainAsync(pawn.health.hediffSet.hediffs.ToArray);
            Hediff? lifeThreateningHediff = await FindLifeThreateningHediffAsync(healProviders, pawnHediffsSnapshot);

            if (lifeThreateningHediff != null) return lifeThreateningHediff;

            if (await MainThreadExtensions.OnMainAsync(HealthUtility.TicksUntilDeathDueToBloodLoss, pawn) < 2_500)
            {
                Hediff? mostBleedingHediff = await FindMostBleedingHediffAsync(healProviders, pawnHediffsSnapshot);

                if (mostBleedingHediff != null) return mostBleedingHediff;
            }

            BodyPartRecord brainRecord = pawn.health.hediffSet.GetBrain();
            Hediff_Injury? permanentInjury;

            if (brainRecord != null)
            {
                permanentInjury = await FindPermanentInjuryAsync([brainRecord,], healProviders, pawnHediffsSnapshot);

                if (permanentInjury != null) return permanentInjury;
            }

            Hediff_MissingPart? biggestMissingBodyPart = await FindBiggestMissingBodyPartAsync(pawn, HandCoverageAbsWithChildren, healProviders);

            if (biggestMissingBodyPart != null) return biggestMissingBodyPart;

            Hediff_Injury? permanentEyeInjury = await FindPermanentInjuryAsync([pawn.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Eye),], healProviders, pawnHediffsSnapshot);

            if (permanentEyeInjury != null) return permanentEyeInjury;

            Hediff? immunizableHediffWhichCanKill = await FindImmunizableHediffWhichCanKillAsync(healProviders, pawnHediffsSnapshot);

            if (immunizableHediffWhichCanKill != null) return immunizableHediffWhichCanKill;

            Hediff? nonInjuryMiscBadHediff = await FindNonInjuryMiscBadHediffAsync(onlyIfCanKill: true, healProviders, pawnHediffsSnapshot);

            if (nonInjuryMiscBadHediff != null) return nonInjuryMiscBadHediff;

            nonInjuryMiscBadHediff = await FindNonInjuryMiscBadHediffAsync(onlyIfCanKill: false, healProviders, pawnHediffsSnapshot);

            if (nonInjuryMiscBadHediff != null) return nonInjuryMiscBadHediff;

            if (brainRecord != null)
            {
                Hediff_Injury? brainInjury = await FindInjuryAsync([brainRecord,], healProviders, pawnHediffsSnapshot);

                if (brainInjury != null) return brainInjury;
            }

            biggestMissingBodyPart = await FindBiggestMissingBodyPartAsync(pawn, minCoverage: 0f, healProviders);

            if (biggestMissingBodyPart != null) return biggestMissingBodyPart;

            Hediff_Addiction? addiction = await FindAddictionAsync(healProviders, pawnHediffsSnapshot);

            if (addiction != null) return addiction;

            permanentInjury = await FindPermanentInjuryAsync([], healProviders, pawnHediffsSnapshot);

            if (permanentInjury != null) return permanentInjury;

            Hediff? injury = await FindInjuryAsync([], healProviders, pawnHediffsSnapshot);

            return injury;
        }

        /// <summary>
        ///     Retrieves a candidate Hediff for further actions on the specified pawn, evaluating various factors and using
        ///     the provided healing providers for potential Hediff candidates.
        /// </summary>
        /// <param name="pawn">The pawn for which to find a Hediff candidate.</param>
        /// <returns>A task representing the asynchronous operation, containing the identified Hediff if found.</returns>
        private static async Task<Hediff?> GetHediffCandidateAsync(Pawn pawn)
        {
            IReadOnlyList<IHealProvider> healProviders = Registries.Compatibilities.AllRegistrants.OfType<IHealProvider>().ToList();
            Hediff[] pawnHediffsSnapshot = await MainThreadExtensions.OnMainAsync(pawn.health.hediffSet.hediffs.ToArray);
            Hediff? lifeThreateningHediff = await FindLifeThreateningHediffAsync(healProviders, pawnHediffsSnapshot);

            if (lifeThreateningHediff != null) return lifeThreateningHediff;

            if (await MainThreadExtensions.OnMainAsync(HealthUtility.TicksUntilDeathDueToBloodLoss, pawn) < 2_500)
            {
                Hediff? mostBleedingHediff = await FindMostBleedingHediffAsync(healProviders, pawnHediffsSnapshot);

                if (mostBleedingHediff != null) return mostBleedingHediff;
            }

            BodyPartRecord brainRecord = pawn.health.hediffSet.GetBrain();
            Hediff_Injury? permanentInjury;

            if (brainRecord != null)
            {
                permanentInjury = await FindPermanentInjuryAsync([brainRecord,], healProviders, pawnHediffsSnapshot);

                if (permanentInjury != null) return permanentInjury;
            }

            Hediff_Injury? permanentEyeInjury = await FindPermanentInjuryAsync([pawn.health.hediffSet.GetBodyPartRecord(BodyPartDefOf.Eye),], healProviders, pawnHediffsSnapshot);

            if (permanentEyeInjury != null) return permanentEyeInjury;

            Hediff? immunizableHediffWhichCanKill = await FindImmunizableHediffWhichCanKillAsync(healProviders, pawnHediffsSnapshot);

            if (immunizableHediffWhichCanKill != null) return immunizableHediffWhichCanKill;

            Hediff? nonInjuryMiscBadHediff = await FindNonInjuryMiscBadHediffAsync(onlyIfCanKill: true, healProviders, pawnHediffsSnapshot);

            if (nonInjuryMiscBadHediff != null) return nonInjuryMiscBadHediff;

            nonInjuryMiscBadHediff = await FindNonInjuryMiscBadHediffAsync(onlyIfCanKill: false, healProviders, pawnHediffsSnapshot);

            if (nonInjuryMiscBadHediff != null) return nonInjuryMiscBadHediff;

            if (brainRecord != null)
            {
                Hediff_Injury? brainInjury = await FindInjuryAsync([brainRecord,], healProviders, pawnHediffsSnapshot);

                if (brainInjury != null) return brainInjury;
            }

            Hediff_Addiction? addiction = await FindAddictionAsync(healProviders, pawnHediffsSnapshot);

            if (addiction != null) return addiction;

            permanentInjury = await FindPermanentInjuryAsync([], healProviders, pawnHediffsSnapshot);

            if (permanentInjury != null) return permanentInjury;

            Hediff_Injury? injury = await FindInjuryAsync([], healProviders, pawnHediffsSnapshot);

            return injury;
        }

        /// <summary>
        ///     Gets a candidate body part record for healing on the specified pawn, evaluating the provided healing providers
        ///     for suitable parts.
        /// </summary>
        /// <param name="pawn">The pawn for which to find a body part record candidate.</param>
        /// <returns>A task representing the asynchronous operation, containing the largest missing body part record if found.</returns>
        private static async Task<Hediff_MissingPart?> GetBodyPartRecordCandidateAsync(Pawn pawn)
        {
            IReadOnlyList<IHealProvider> healProviders = Registries.Compatibilities.AllRegistrants.OfType<IHealProvider>().ToList();
            Hediff_MissingPart? biggestMissingBodyPart = await FindBiggestMissingBodyPartAsync(pawn, HandCoverageAbsWithChildren, healProviders);

            if (biggestMissingBodyPart != null) return biggestMissingBodyPart;

            biggestMissingBodyPart = await FindBiggestMissingBodyPartAsync(pawn, minCoverage: 0f, healProviders);

            return biggestMissingBodyPart;
        }

        /// <summary>Finds an addiction on a given pawn, evaluating the provided health providers for healing capabilities.</summary>
        /// <param name="healProviders">A list of healing providers that can assess the heal-ability of an addiction.</param>
        /// <param name="pawnHediffSnapshot">A snapshot of the pawn's current hediffs.</param>
        /// <returns>A task representing the asynchronous operation, containing the visible, curable addiction if found.</returns>
        private static async Task<Hediff_Addiction?> FindAddictionAsync(IReadOnlyList<IHealProvider> healProviders, IReadOnlyList<Hediff> pawnHediffSnapshot)
        {
            for (var index = 0; index < pawnHediffSnapshot.Count; index++)
            {
                Hediff h = pawnHediffSnapshot[index];
                var skipping = false;

                for (var index2 = 0; index2 < healProviders.Count; index2++)
                {
                    IHealProvider provider = healProviders[index2];
                    Result canHeal = await provider.CanHealAsync(h);

                    if (canHeal.IsSuccess) continue;

                    skipping = true;

                    break;
                }

                if (skipping) continue;

                if (h is Hediff_Addiction { Visible: true, } addiction && addiction.def.everCurableByItem) return addiction;
            }

            return null;
        }

        /// <summary>Finds an injury on a given pawn, considering specific body parts and heal providers.</summary>
        /// <param name="records">A collection of specific body parts to check for injuries.</param>
        /// <param name="healProviders">A list of healing providers that can assess the heal-ability of the injury.</param>
        /// <param name="pawnHediffsSnapshot">An array containing a snapshot of the pawn's current hediffs.</param>
        /// <returns>A task representing the asynchronous operation, containing the visible, curable injury if found.</returns>
        private static async Task<Hediff_Injury?> FindInjuryAsync(
            IReadOnlyCollection<BodyPartRecord> records,
            IReadOnlyList<IHealProvider> healProviders,
            Hediff[] pawnHediffsSnapshot
        )
        {
            Hediff_Injury? hediff = null;

            for (var i = 0; i < pawnHediffsSnapshot.Length; i++)
            {
                Hediff h = pawnHediffsSnapshot[i];
                var skipping = false;

                for (var j = 0; j < healProviders.Count; j++)
                {
                    IHealProvider provider = healProviders[j];
                    Result canHeal = await provider.CanHealAsync(h);

                    if (canHeal.IsSuccess) continue;

                    skipping = true;

                    break;
                }

                if (skipping) continue;

                if (h is Hediff_Injury { Visible: true, } injury
                 && injury.def.everCurableByItem
                 && (records.Count <= 0 || records.Contains(injury.Part))
                 && (hediff == null || injury.Severity > hediff.Severity))
                    hediff = injury;
            }

            return hediff;
        }

        /// <summary>Finds a non-injury miscellaneous bad hediff for a given pawn.</summary>
        /// <param name="onlyIfCanKill">Specifies whether to include only hediffs that can potentially be lethal.</param>
        /// <param name="healProviders">A collection of healing providers that can assess the heal-ability of the hediff.</param>
        /// <param name="pawnHediffsSnapshot">An array containing a snapshot of the pawn's current hediffs.</param>
        /// <returns>A task representing the asynchronous operation, containing the non-injury miscellaneous bad hediff, if found.</returns>
        private static async Task<Hediff?> FindNonInjuryMiscBadHediffAsync(bool onlyIfCanKill, IReadOnlyList<IHealProvider> healProviders, Hediff[] pawnHediffsSnapshot)
        {
            Hediff? hediff = null;
            var num = 1f;

            for (var i = 0; i < pawnHediffsSnapshot.Length; i++)
            {
                Hediff h = pawnHediffsSnapshot[i];
                var skipping = false;

                for (var j = 0; j < healProviders.Count; j++)
                {
                    IHealProvider provider = healProviders[j];
                    Result canHeal = await provider.CanHealAsync(h);

                    if (canHeal.IsSuccess) continue;

                    skipping = true;

                    break;
                }

                if (skipping
                 || !h.Visible
                 || !h.def.isBad
                 || !h.def.everCurableByItem
                 || h is Hediff_Injury
                 || h is Hediff_MissingPart
                 || h is Hediff_Addiction
                 || h is Hediff_AddedPart
                 || onlyIfCanKill && !await CanEverKillAsync(h))
                    continue;

                float severity = h.Severity;

                if (hediff != null && severity <= num) continue;

                hediff = h;
                num = severity;
            }

            return hediff;
        }

        /// <summary>Determines whether a given hediff has the potential to be lethal.</summary>
        /// <param name="hediff">The hediff to evaluate for lethality.</param>
        /// <returns>
        ///     A task representing the asynchronous operation, containing a boolean that indicates if the hediff can ever be
        ///     lethal.
        /// </returns>
        private static ValueTask<bool> CanEverKillAsync(Hediff hediff)
        {
            return new ValueTask<bool>(hediff.def.stages?.Any(s => s.lifeThreatening) ?? hediff.def.lethalSeverity >= 0f);
        }

        /// <summary>
        ///     Finds an immunizable hediff that has the potential to kill the pawn from the given list of hediffs,
        ///     considering the healing capabilities of the provided healing providers.
        /// </summary>
        /// <param name="healProviders">A list of healing providers that determine which hediffs can be healed.</param>
        /// <param name="hediffSnapshot">A snapshot of the pawn's current hediffs.</param>
        /// <returns>
        ///     A task representing the asynchronous operation, containing the first found immunizable, potentially lethal
        ///     hediff if any, otherwise null.
        /// </returns>
        private static async Task<Hediff?> FindImmunizableHediffWhichCanKillAsync(IReadOnlyList<IHealProvider> healProviders, IReadOnlyList<Hediff> hediffSnapshot)
        {
            Hediff? hediff = null;
            float num = -1f;

            for (var i = 0; i < hediffSnapshot.Count; i++)
            {
                Hediff h = hediffSnapshot[i];
                var skipping = false;

                for (var j = 0; j < healProviders.Count; j++)
                {
                    IHealProvider provider = healProviders[j];
                    Result canHeal = await provider.CanHealAsync(h);

                    if (canHeal.IsSuccess) continue;

                    skipping = true;

                    break;
                }

                if (skipping || !h.Visible || !h.def.everCurableByItem || !h.def.HasComp(typeof(HediffComp_Immunizable)) || h.FullyImmune() || !await CanEverKillAsync(h)) continue;

                float severity = h.Severity;

                if (hediff != null && severity <= num) continue;

                hediff = h;
                num = severity;
            }

            return hediff;
        }

        /// <summary>
        ///     Finds the largest missing body part within the provided body parts that meets the specified coverage
        ///     requirements and can be healed by the available healing providers.
        /// </summary>
        /// <param name="pawn">The pawn to search for missing body parts.</param>
        /// <param name="minCoverage">The minimum coverage threshold for considering a body part.</param>
        /// <param name="healProviders">A list of healing providers that determine which missing parts can be healed.</param>
        /// <returns>
        ///     A task representing the asynchronous operation, containing the largest missing body part if found, otherwise
        ///     null.
        /// </returns>
        private static async Task<Hediff_MissingPart?> FindBiggestMissingBodyPartAsync(Pawn pawn, float minCoverage, IReadOnlyList<IHealProvider> healProviders)
        {
            Hediff_MissingPart? biggestMissingBodyPart = null;
            Hediff_MissingPart[] missingParts = await MainThreadExtensions.OnMainAsync(func: p => p.health.hediffSet.GetMissingPartsCommonAncestors().ToArray(), pawn);

            for (var i = 0; i < missingParts.Length; i++)
            {
                Hediff_MissingPart h = missingParts[i];
                var skipping = false;

                for (var j = 0; j < healProviders.Count; j++)
                {
                    IHealProvider provider = healProviders[j];
                    Result canHeal = await provider.CanHealAsync(h);

                    if (canHeal.IsSuccess) continue;

                    skipping = true;

                    break;
                }

                if (skipping) continue;

                if (h.Part.coverageAbsWithChildren >= minCoverage
                 && !await MainThreadExtensions.OnMainAsync(pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts, h.Part)
                 && (biggestMissingBodyPart == null || h.Part.coverageAbsWithChildren > biggestMissingBodyPart.Part.coverageAbsWithChildren))
                    biggestMissingBodyPart = h;
            }

            return biggestMissingBodyPart;
        }

        /// <summary>Finds a permanent injury within the provided body parts that can be healed by the specified healing providers.</summary>
        /// <param name="records">A collection of body part records to search for injuries.</param>
        /// <param name="healProviders">A list of healing providers that determine which injuries can be healed.</param>
        /// <param name="hediffSnapshot">A snapshot of the pawn's current hediffs.</param>
        /// <returns>
        ///     A task representing the asynchronous operation, containing the most severe permanent injury if found,
        ///     otherwise null.
        /// </returns>
        private static async Task<Hediff_Injury?> FindPermanentInjuryAsync(
            IReadOnlyCollection<BodyPartRecord> records,
            IReadOnlyList<IHealProvider> healProviders,
            Hediff[] hediffSnapshot
        )
        {
            Hediff_Injury? hediff = null;

            for (var i = 0; i < hediffSnapshot.Length; i++)
            {
                Hediff h = hediffSnapshot[i];
                var skipping = false;

                for (var j = 0; j < healProviders.Count; j++)
                {
                    IHealProvider provider = healProviders[j];
                    Result canHeal = await provider.CanHealAsync(h);

                    if (canHeal.IsSuccess) continue;

                    skipping = true;

                    break;
                }

                if (skipping) continue;

                if (h is Hediff_Injury { Visible: true, } injury
                 && injury.IsPermanent()
                 && injury.def.everCurableByItem
                 && (records.Count <= 0 || records.Contains(injury.Part))
                 && (hediff == null || injury.Severity > hediff.Severity))
                    hediff = injury;
            }

            return hediff;
        }

        /// <summary>Finds the hediff with the highest bleeding rate that can be healed by the provided healing providers.</summary>
        /// <param name="healProviders">A collection of healing providers that determine which hediffs can be healed.</param>
        /// <param name="hediffSnapshot">A snapshot of the pawn's current hediffs.</param>
        /// <returns>A task representing the asynchronous operation, containing the most bleeding hediff if found, otherwise null.</returns>
        private static async Task<Hediff?> FindMostBleedingHediffAsync(IReadOnlyList<IHealProvider> healProviders, Hediff[] hediffSnapshot)
        {
            var num = 0f;
            Hediff? hediff = null;

            for (var i = 0; i < hediffSnapshot.Length; i++)
            {
                Hediff h = hediffSnapshot[i];
                var skipping = false;

                for (var j = 0; j < healProviders.Count; j++)
                {
                    IHealProvider provider = healProviders[j];
                    Result canHeal = await provider.CanHealAsync(h);

                    if (canHeal.IsSuccess) continue;

                    skipping = true;

                    break;
                }

                if (skipping || !h.Visible || !h.def.everCurableByItem) continue;

                float bleedRate = h.BleedRate;

                if (bleedRate <= 0f || bleedRate <= num && hediff != null) continue;

                num = bleedRate;
                hediff = h;
            }

            return hediff;
        }

        /// <summary>
        ///     Finds a life-threatening hediff for a given pawn from a snapshot of hediffs that can be healed by the provided
        ///     healing providers.
        /// </summary>
        /// <param name="healProviders">A collection of healing providers that define what hediffs can be healed.</param>
        /// <param name="hediffSnapshot">A snapshot of the pawn's hediffs.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation, containing the found life-threatening hediff if any,
        ///     otherwise null.
        /// </returns>
        private static async Task<Hediff?> FindLifeThreateningHediffAsync(IReadOnlyList<IHealProvider> healProviders, Hediff[] hediffSnapshot)
        {
            Hediff? hediff = null;
            float num = -1f;

            for (var i = 0; i < hediffSnapshot.Length; i++)
            {
                Hediff h = hediffSnapshot[i];
                var skipping = false;

                for (var j = 0; j < healProviders.Count; j++)
                {
                    IHealProvider provider = healProviders[j];
                    Result canHeal = await provider.CanHealAsync(h);

                    if (canHeal.IsSuccess) continue;

                    skipping = true;

                    break;
                }

                if (skipping || !h.Visible || !h.def.everCurableByItem || await MainThreadExtensions.OnMainAsync(HediffUtility.FullyImmune, h)) continue;

                bool lifeThreatening = h.CurStage is { lifeThreatening: true, };
                bool lethal = h.def.lethalSeverity >= 0f && h.Severity / h.def.lethalSeverity >= 0.8f;

                if (!lifeThreatening || !lethal) continue;

                float coverage = h.Part?.coverageAbsWithChildren ?? 999f;

                if (coverage <= num) continue;

                num = coverage;
                hediff = h;
            }

            return hediff;
        }
    }
}
