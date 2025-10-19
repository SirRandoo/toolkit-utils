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
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using RimWorld;
using RimWorld.Planet;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Logging;
using Verse;
using Verse.AI.Group;

namespace ToolkitUtils.Mod.Services;

/// <inheritdoc />
internal sealed class ResurrectionService(TranslationService translationService) : IResurrectionService
{
    private const int InvisibleStunDuration = 5 * GenTicks.TicksPerRealSecond;
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();
    private static readonly SimpleCurve DementiaChancePerRotDaysCurve = [new CurvePoint(x: 0.1f, y: 0.02f), new CurvePoint(x: 5f, y: 0.8f),];
    private static readonly SimpleCurve BlindnessChancePerRotDaysCurve = [new CurvePoint(x: 0.1f, y: 0.02f), new CurvePoint(x: 5f, y: 0.8f),];
    private static readonly SimpleCurve ResurrectionPsychosisChancePerRotDaysCurve = [new CurvePoint(x: 0.1f, y: 0.02f), new CurvePoint(x: 5f, y: 0.8f),];

    /// <inheritdoc />
    public async Task<Result> ResurrectAsync(ResurrectionOptions options, Pawn pawn)
    {
        Logger.Info(message: "Attempting to revive the pawn '{PawnName}'", pawn.Label);

        if (!pawn.Dead)
        {
            Logger.Warn("Tried to resurrect a living pawn");

            return Result.Fail(translationService.GetNotDeadError(pawn));
        }

        if (pawn.Discarded)
        {
            Logger.Warn(message: "Tried to resurrect a discarded pawn (Pawn {PawnString})", pawn.ToStringSafe());

            return Result.Fail(translationService.GetDiscardedError(pawn));
        }

        Corpse corpse = pawn.Corpse;
        var spawned = false;
        IntVec3 location = IntVec3.Invalid;
        Map? map = null;

        if (ModsConfig.AnomalyActive && corpse is UnnaturalCorpse)
        {
            Logger.Warn("Tried to resurrect an unnatural corpse");

            return Result.Fail(translationService.GetUnnaturalCorpseResurrectionError(pawn));
        }

        bool isSelected = await RouterService.Instance.RouteToMainAsync(Find.Selector.IsSelected, corpse);

        if (corpse != null)
        {
            spawned = corpse.SpawnedOrAnyParentSpawned;
            location = corpse.PositionHeld;
            map = corpse.MapHeld;
            corpse.InnerPawn = null;

            await RouterService.Instance.RouteToMainAsync(corpse.Destroy, DestroyMode.Vanish);
        }

        if (spawned && await RouterService.Instance.RouteToMainAsync(pawn.IsWorldPawn)) await RouterService.Instance.RouteToMainAsync(Find.WorldPawns.RemovePawn, pawn);

        await RouterService.Instance.RouteToMainAsync(pawn.ForceSetStateToUnspawned);
        await RouterService.Instance.RouteToMainAsync(PawnComponentsUtility.CreateInitialComponents, pawn);
        await RouterService.Instance.RouteToMainAsync(pawn.health.Notify_Resurrected, options.RestoreMissingParts, options.ScarChance);

        if (pawn.Faction is { IsPlayer: true, })
        {
            pawn.workSettings?.EnableAndInitialize();

            await RouterService.Instance.RouteToMainAsync(Find.StoryWatcher.watcherPopAdaptation.Notify_PawnEvent, pawn, PopAdaptationEvent.GainedColonist);
        }

        if (pawn.RaceProps.IsMechanoid && MechRepairUtility.IsMissingWeapon(pawn)) await RouterService.Instance.RouteToMainAsync(MechRepairUtility.GenerateWeapon, pawn);

        if (spawned && !options.DontSpawn)
        {
            await RouterService.Instance.RouteToMainAsync(GenSpawn.Spawn, pawn, location, map, WipeMode.Vanish);

            Lord lord = pawn.GetLord();

            if (lord != null)
                await RouterService.Instance.RouteToMainAsync(lord.Notify_PawnUndowned, pawn);
            else if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer && pawn.HostileTo(Faction.OfPlayer) && !options.NoLord)
            {
                var jobAssaultColony = new LordJob_AssaultColony(
                    pawn.Faction,
                    options.CanKidnap,
                    options.CanTimeoutOrFlee,
                    options.Sappers,
                    options.UseAvoidGridSmart,
                    options.CanSteal,
                    options.Breachers,
                    options.CanPickupOpportunisticWeapons
                );

                LordMaker.MakeNewLord(pawn.Faction, jobAssaultColony, pawn.Map, Gen.YieldSingle(pawn));
            }

            if (pawn.apparel != null)
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;

                for (var i = 0; i < wornApparel.Count; i++) await RouterService.Instance.RouteToMainAsync(wornApparel[i].Notify_PawnResurrected, pawn);
            }
        }

        if (options.RemoveDiedThoughts) await RouterService.Instance.RouteToMainAsync(PawnDiedOrDownedThoughtsUtility.RemoveDiedThoughts, pawn);

        pawn.royalty?.Notify_Resurrected();

        if (pawn.guest != null && pawn.guest.IsInteractionEnabled(PrisonerInteractionModeDefOf.Execution)) pawn.guest.SetNoInteraction();

        if (isSelected) Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);

        pawn.Drawer.renderer.SetAllGraphicsDirty();

        if (options.InvisibleStun) pawn.stances.stunner.StunFor(InvisibleStunDuration, pawn, addBattleLog: false, showMote: false);

        await RouterService.Instance.RouteToMainAsync(pawn.needs.AddOrRemoveNeedsAsAppropriate);

        return Result.Ok();
    }

    /// <inheritdoc />
    public async Task<Result> ResurrectWithSideEffectsAsync(Pawn pawn)
    {
        Corpse corpse = pawn.Corpse;

        float rotProgress = corpse == null ? 0.0f : (await RouterService.Instance.RouteToMainAsync(corpse.GetComp<CompRottable>)).RotProgress / 60_000f;

        Result resurrectionResult = await ResurrectAsync(new ResurrectionOptions(), pawn);

        if (!resurrectionResult.IsSuccess) return resurrectionResult;

        BodyPartRecord brain = pawn.health.hediffSet.GetBrain();
        Hediff resurrectionSicknessHediff = HediffMaker.MakeHediff(HediffDefOf.ResurrectionSickness, pawn, brain);

        if (!await RouterService.Instance.RouteToMainAsync(pawn.health.WouldDieAfterAddingHediff, resurrectionSicknessHediff))
        {
            await RouterService.Instance.RouteToMainAsync(
                pawn.health.AddHediff,
                resurrectionSicknessHediff,
                (BodyPartRecord?)null,
                (DamageInfo?)null,
                (DamageWorker.DamageResult?)null
            );
        }

        bool revivingWithDementia = await RouterService.Instance.RouteToMainAsync(Rand.Chance, DementiaChancePerRotDaysCurve.Evaluate(rotProgress));

        if (revivingWithDementia)
        {
            Hediff dementiaHediff = HediffMaker.MakeHediff(HediffDefOf.Dementia, pawn, brain);
            bool wouldDie = await RouterService.Instance.RouteToMainAsync(pawn.health.WouldDieAfterAddingHediff, dementiaHediff);

            if (!wouldDie)
                await RouterService.Instance.RouteToMainAsync(pawn.health.AddHediff, dementiaHediff, (BodyPartRecord?)null, (DamageInfo?)null, (DamageWorker.DamageResult?)null);
        }

        bool revivingWithBlindness = await RouterService.Instance.RouteToMainAsync(Rand.Chance, BlindnessChancePerRotDaysCurve.Evaluate(rotProgress));

        if (revivingWithBlindness)
        {
            foreach (BodyPartRecord record in pawn.health.hediffSet.GetNotMissingParts().Where(r => r.def == BodyPartDefOf.Eye))
            {
                if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record)) continue;

                Hediff blindnessHediff = HediffMaker.MakeHediff(HediffDefOf.Blindness, pawn, record);

                await RouterService.Instance.RouteToMainAsync(pawn.health.AddHediff, blindnessHediff, (BodyPartRecord?)null, (DamageInfo?)null, (DamageWorker.DamageResult?)null);
            }
        }

        if (brain != null && await RouterService.Instance.RouteToMainAsync(Rand.Chance, ResurrectionPsychosisChancePerRotDaysCurve.Evaluate(rotProgress)))
        {
            Hediff resurrectionPsychosisHediff = HediffMaker.MakeHediff(HediffDefOf.ResurrectionPsychosis, pawn, brain);

            bool wouldDie = await RouterService.Instance.RouteToMainAsync(pawn.health.WouldDieAfterAddingHediff, resurrectionPsychosisHediff);

            if (wouldDie)
            {
                await RouterService.Instance.RouteToMainAsync(
                    pawn.health.AddHediff,
                    resurrectionPsychosisHediff,
                    (BodyPartRecord?)null,
                    (DamageInfo?)null,
                    (DamageWorker.DamageResult?)null
                );
            }
        }

        if (await RouterService.Instance.RouteToMainAsync(func: p => p.Dead, pawn))
        {
            Logger.Warn(message: "The pawn '{PawnString}' died while being resurrected", pawn.ToStringSafe());

            await ResurrectAsync(new ResurrectionOptions(), pawn);
        }

        return Result.Ok();
    }
}
