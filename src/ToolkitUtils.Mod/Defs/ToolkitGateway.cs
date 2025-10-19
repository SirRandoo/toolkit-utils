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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Mod.Domain;
using ToolkitUtils.Mod.Domain.Settings;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Services;
using Verse;

namespace ToolkitUtils.Mod.Defs;

/// <inheritdoc cref="IToolkitGateway" />
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public sealed class ToolkitGateway : Building, IToolkitGateway
{
    private const float RatSpawnChance = 0.01f;
    private const float BoomRatMutationChance = 0.1f;
    private ClientSettings? _clientSettings;
    private IReadOnlyList<Gizmo>? _gizmoCache;
    private MiscSettings? _miscSettings;
    private SpawnFlags _spawnFlags = SpawnFlags.None;

    /// <inheritdoc />
    public SpawnFlags SpawnFlags => _spawnFlags;

    /// <inheritdoc />
    public string Id
    {
        get
        {
            return field ??= $"{def.defName}_{Position.ToString()}";
        }
    } = null!;

    /// <inheritdoc />
    public bool TrySpawn(Thing thing) => GenPlace.TryPlaceThing(thing, Position, Map, ThingPlaceMode.Near, SpawnGatewayPuff);

    /// <inheritdoc />
    public bool TrySpawn(Pawn pawn) => GenSpawn.Spawn(pawn, Position, Map, WipeMode.VanishOrMoveAside) != null;

    /// <inheritdoc />
    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref _spawnFlags, label: "spawnFlags");
    }

    /// <inheritdoc />
    public override IEnumerable<Gizmo> GetGizmos()
    {
        if (_gizmoCache != null) return _gizmoCache;

        _gizmoCache =
        [
            new Command_Action
            {
                action = DestroyInternal,
                icon = Widgets.PlaceholderIconTex, // TODO: Replace icon with proper icon.
                defaultLabel = TranslationService.Instance.GetTranslation("TKUtils.Gizmos.Gateway.Destroy.Label"),
                defaultDesc = TranslationService.Instance.GetTranslation("TKUtils.Gizmos.Gateway.Destroy.Description"),
            },
            new Command_Toggle
            {
                icon = Widgets.PlaceholderIconTex, // TODO: Replace icon with proper icon.
                defaultLabel = TranslationService.Instance.GetTranslation("TKUtils.Gizmos.Gateway.AllowsAnimals.Label"),
                defaultDesc = TranslationService.Instance.GetTranslation("TKUtils.Gizmos.Gateway.AllowsAnimals.Description"),
                isActive = AllowsAnimals,
                toggleAction = ToggleAllowsAnimals, },
            new Command_Toggle
            {
                icon = Widgets.PlaceholderIconTex, // TODO: Replace icon with proper icon.
                defaultLabel = TranslationService.Instance.GetTranslation("TKUtils.Gizmos.Gateway.AllowsItems.Label"),
                defaultDesc = TranslationService.Instance.GetTranslation("TKUtils.Gizmos.Gateway.AllowsItems.Description"),
                isActive = AllowsItems,
                toggleAction = ToggleAllowsItems, },
            new Command_Toggle
            {
                icon = Widgets.PlaceholderIconTex, // TODO: Replace icon with proper icon.
                defaultLabel = TranslationService.Instance.GetTranslation("TKUtils.Gizmos.Gateway.AllowsPawns.Label"),
                defaultDesc = TranslationService.Instance.GetTranslation("TKUtils.Gizmos.Gateway.AllowsPawns.Description"),
                isActive = AllowsPawns,
                toggleAction = ToggleAllowsPawns, },
            ..base.GetGizmos(),
        ];

        return _gizmoCache;
    }

    /// <inheritdoc />
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        _clientSettings = Orchestration.ServiceProvider.GetService<ClientSettings>();
        _miscSettings = Orchestration.ServiceProvider.GetService<MiscSettings>();

        GatewayService.Instance.TryRegisterGateway(this);
    }

    /// <inheritdoc />
    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        GatewayService.Instance.TryUnregisterGateway(this);

        base.Destroy(mode);
    }

    /// <inheritdoc />
    public override void PreApplyDamage(ref DamageInfo dinfo, [UnscopedRef] out bool absorbed)
    {
        absorbed = true;
    }

    /// <inheritdoc />
    public override void TickLong()
    {
        if (_miscSettings is not { UseGatewayEasterEggs: true, }) return;
        if (!Rand.Chance(RatSpawnChance)) return;

        PawnKindDef ratKind = DefDatabase<PawnKindDef>.GetNamed("Rat");

        if (Rand.Chance(BoomRatMutationChance)) ratKind = DefDatabase<PawnKindDef>.GetNamed("Boomrat");

        GenSpawn.Spawn(PawnGenerator.GeneratePawn(ratKind), Position, Map, WipeMode.VanishOrMoveAside);
    }

    private bool AllowsAnimals() => _spawnFlags.HasFlagFast(SpawnFlags.AllowsAnimals);
    private void ToggleAllowsAnimals() => _spawnFlags ^= SpawnFlags.AllowsAnimals;

    private bool AllowsItems() => _spawnFlags.HasFlagFast(SpawnFlags.AllowsItems);
    private void ToggleAllowsItems() => _spawnFlags ^= SpawnFlags.AllowsItems;

    private bool AllowsPawns() => _spawnFlags.HasFlagFast(SpawnFlags.AllowsPawns);
    private void ToggleAllowsPawns() => _spawnFlags ^= SpawnFlags.AllowsPawns;

    private void DestroyInternal() => Destroy();

    private void SpawnGatewayPuff(Thing thing, int count)
    {
        if (_clientSettings is not { UseGatewayPuff: true, }) return;

        FleckMaker.ThrowSmoke(Position.ToVector3(), Map, thing.Graphic.drawSize.magnitude);
    }
}
