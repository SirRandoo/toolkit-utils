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

internal sealed class NoOpResurrectionService(TranslationService translationService) : IResurrectionService
{
    private const int InvisibleStunDuration = 5 * GenTicks.TicksPerRealSecond;
    private static readonly SimpleCurve DementiaChancePerRotDaysCurve = [new CurvePoint(x: 0.1f, y: 0.02f), new CurvePoint(x: 5f, y: 0.8f),];
    private static readonly SimpleCurve BlindnessChancePerRotDaysCurve = [new CurvePoint(x: 0.1f, y: 0.02f), new CurvePoint(x: 5f, y: 0.8f),];
    private static readonly SimpleCurve ResurrectionPsychosisChancePerRotDaysCurve = [new CurvePoint(x: 0.1f, y: 0.02f), new CurvePoint(x: 5f, y: 0.8f),];
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

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
            Logger.Trace("Found a corpse; simulating destroying it...");
            spawned = corpse.SpawnedOrAnyParentSpawned;
            location = corpse.PositionHeld;
            map = corpse.MapHeld;
            Logger.Trace("Nullifying corpse's inner pawn reference...");

            Logger.Trace("Destroying corpse...");
            // await router.RouteToMainAsync(corpse.Destroy, DestroyMode.Vanish);
        }

        if (spawned && await RouterService.Instance.RouteToMainAsync(pawn.IsWorldPawn))
        {
            Logger.Trace("The corpse is spawned in the world as world pawn; removing it from the world pawn list...");
            // await router.RouteToMainAsync(Find.WorldPawns.RemovePawn, pawn);
        }

        Logger.Trace("Forcibly setting the pawn's state to unspawned.");
        // await router.RouteToMainAsync(pawn.ForceSetStateToUnspawned);

        Logger.Trace("Resetting initial components for the pawn...");
        // await router.RouteToMainAsync(PawnComponentsUtility.CreateInitialComponents, pawn);

        Logger.Trace("Notifying the pawn they've been resurrected...");
        // await router.RouteToMainAsync(pawn.health.Notify_Resurrected, options.RestoreMissingParts, options.ScarChance);

        if (pawn.Faction is { IsPlayer: true, })
        {
            Logger.Trace("Revived pawn is a player-controlled pawn; re-enabling and initializing their work settings...");
            // pawn.workSettings?.EnableAndInitialize();

            Logger.Trace("Notifying player their colonist was revived...");
            // await router.RouteToMainAsync(Find.StoryWatcher.watcherPopAdaptation.Notify_PawnEvent, pawn, PopAdaptationEvent.GainedColonist);
        }

        if (pawn.RaceProps.IsMechanoid && MechRepairUtility.IsMissingWeapon(pawn))
        {
            Logger.Trace("The pawn is a mechanoid without a weapon; generating a new one...");
            // await router.RouteToMainAsync(MechRepairUtility.GenerateWeapon, pawn);
        }

        if (spawned && !options.DontSpawn)
        {
            Logger.Trace(message: "Spawning the pawn on the map {MapId} at position {XPos}, {YPos},  {ZPos}...", map!.uniqueID, location.x, location.y, location.z);
            // await router.RouteToMainAsync(GenSpawn.Spawn, pawn, location, map, WipeMode.Vanish);

            Lord lord = pawn.GetLord();

            if (lord != null)
            {
                Logger.Trace("Notifying the pawn's lord that the pawn's no longer downed");
                // lord.Notify_PawnUndowned(pawn);
            }
            else if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer && pawn.HostileTo(Faction.OfPlayer) && !options.NoLord)
            {
                Logger.Trace("Resurrected pawn is hostile to the colony; preparing an one person assault on the colony...");
                // var jobAssaultColony = new LordJob_AssaultColony(
                //     pawn.Faction,
                //     options.CanKidnap,
                //     options.CanTimeoutOrFlee,
                //     options.Sappers,
                //     options.UseAvoidGridSmart,
                //     options.CanSteal,
                //     options.Breachers,
                //     options.CanPickupOpportunisticWeapons
                // );
                //
                // LordMaker.MakeNewLord(pawn.Faction, jobAssaultColony, pawn.Map, Gen.YieldSingle(pawn));
            }

            if (pawn.apparel != null)
            {
                Logger.Trace("The resurrected pawn had apparel; notifying their apparel that they're no longer on a dead person...");
                List<Apparel> wornApparel = pawn.apparel.WornApparel;

                for (var i = 0; i < wornApparel.Count; i++) Logger.Trace(message: "Notifying apparel {ApparelName} that the wearer is resurrected...", wornApparel[i].Label);
                // await router.RouteToMainAsync(wornApparel[i].Notify_PawnResurrected, pawn);
            }
        }

        if (options.RemoveDiedThoughts)
        {
            Logger.Trace("Removing died thoughts from pawn...");
            // await router.RouteToMainAsync(PawnDiedOrDownedThoughtsUtility.RemoveDiedThoughts, pawn);
        }

        Logger.Trace("Notifying pawn's royalty that they've been resurrected...");
        // pawn.royalty?.Notify_Resurrected();

        if (pawn.guest != null && pawn.guest.IsInteractionEnabled(PrisonerInteractionModeDefOf.Execution))
        {
            Logger.Trace("Notifying death-row prisoner that they've fulfilled their death penalty...");
            // pawn.guest.SetNoInteraction();
        }

        if (isSelected)
        {
            Logger.Trace("Re-selecting pawn...");
            // Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);
        }

        Logger.Trace("Marking pawn's graphics as dirty...");
        // pawn.Drawer.renderer.SetAllGraphicsDirty();

        if (options.InvisibleStun)
        {
            Logger.Trace("Stunning resurrected pawn...");
            // pawn.stances.stunner.StunFor(InvisibleStunDuration, pawn, addBattleLog: false, showMote: false);
        }

        Logger.Trace("Re-adjusting the pawn's needs...");
        // await router.RouteToMainAsync(pawn.needs.AddOrRemoveNeedsAsAppropriate);

        return Result.Ok();
    }

    /// <inheritdoc />
    public async Task<Result> ResurrectWithSideEffectsAsync(Pawn pawn)
    {
        Corpse corpse = pawn.Corpse;

        float rotProgress = corpse == null ? 0.0f : (await RouterService.Instance.RouteToMainAsync(corpse.GetComp<CompRottable>)).RotProgress / 60_000f;

        Result resurrectionResult = await ResurrectAsync(ResurrectionOptions.Default, pawn);

        if (!resurrectionResult.IsSuccess)
        {
            Logger.Warn("Could not resurrect pawn; aborting side effects...");

            return resurrectionResult;
        }

        BodyPartRecord brain = pawn.health.hediffSet.GetBrain();
        Hediff? resurrectionSicknessHediff = HediffMaker.MakeHediff(HediffDefOf.ResurrectionSickness, pawn, brain);

        if (!await RouterService.Instance.RouteToMainAsync(pawn.health.WouldDieAfterAddingHediff, resurrectionSicknessHediff))
            Logger.Trace("Resurrection sickness wouldn't kill pawn; adding resurrection sickness...");
        else
            Logger.Trace("Resurrection sickness would kill pawn; omitting...");

        bool revivingWithDementia = await RouterService.Instance.RouteToMainAsync(Rand.Chance, DementiaChancePerRotDaysCurve.Evaluate(rotProgress));

        if (revivingWithDementia)
        {
            Logger.Trace("Reviving pawn with dementia...");
            Hediff? dementiaHediff = HediffMaker.MakeHediff(HediffDefOf.Dementia, pawn, brain);
            bool wouldDie = await RouterService.Instance.RouteToMainAsync(pawn.health.WouldDieAfterAddingHediff, dementiaHediff);

            if (!wouldDie)
            {
                Logger.Trace("Dementia wouldn't kill pawn; adding dementia...");
                // await router.RouteToMainAsync(pawn.health.AddHediff, dementiaHediff, (BodyPartRecord?)null, (DamageInfo?)null, (DamageWorker.DamageResult?)null);
            }
            else
                Logger.Trace("Dementia would kill pawn; omitting dementia...");
        }
        else
            Logger.Trace("Pawn didn't roll dementia; skipping...");

        bool revivingWithBlindness = await RouterService.Instance.RouteToMainAsync(Rand.Chance, BlindnessChancePerRotDaysCurve.Evaluate(rotProgress));

        if (revivingWithBlindness)
        {
            Logger.Trace("Reviving pawn with blindness...");

            foreach (BodyPartRecord record in pawn.health.hediffSet.GetNotMissingParts().Where(r => r.def == BodyPartDefOf.Eye))
            {
                if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record)) continue;

                string location = record.woundAnchorTag == "RightEye" ? "right" : "left";
                Logger.Trace(message: "Blinding pawn in the {Location} eye", location);
            }
        }
        else
            Logger.Trace("Pawn didn't roll blindness; skipping...");

        if (brain != null && await RouterService.Instance.RouteToMainAsync(Rand.Chance, ResurrectionPsychosisChancePerRotDaysCurve.Evaluate(rotProgress)))
        {
            Hediff? resurrectionPsychosisHediff = HediffMaker.MakeHediff(HediffDefOf.ResurrectionPsychosis, pawn, brain);

            bool wouldDie = await RouterService.Instance.RouteToMainAsync(pawn.health.WouldDieAfterAddingHediff, resurrectionPsychosisHediff);

            if (wouldDie)
            {
                Logger.Trace("Pawn wouldn't die by adding resurrection psychosis; adding...");
                // await router.RouteToMainAsync(pawn.health.AddHediff, resurrectionPsychosisHediff, (BodyPartRecord?)null, (DamageInfo?)null, (DamageWorker.DamageResult?)null);
            }
            else
                Logger.Trace("Pawn would die by adding resurrection psychosis; skipping...");
        }
        else
            Logger.Trace("Pawn didn't roll resurrection psychosis; skipping...");

        if (await RouterService.Instance.RouteToMainAsync(func: p => p.Dead, pawn))
        {
            Logger.Warn(message: "The pawn '{PawnString}' died while being resurrected; attempting to resurrect again...", pawn.ToStringSafe());

            await ResurrectAsync(ResurrectionOptions.Default, pawn);
        }

        return Result.Ok();
    }
}
