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
// with ToolkitUtils.Patches. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Api;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;
using Logger = NLog.Logger;

namespace ToolkitUtils.Patches;

/// <summary>
///     A harmony patch for adjusting how the "Colonist need names" in-game alert determines candidates. By default, the
///     alert selects all pawns that don't have a viewer assigned to them, including borrowed pawns. This patch changes
///     that behavior to only select pawns that meet the following criteria:
///     <ul>
///         <li>The pawn isn't borrowed from any faction.</li>
///         <li>The pawn isn't reanimated, assuming A RimWorld of Magic is active.</li>
///         <li>The pawn isn't currently assigned to someone else.</li>
///     </ul>
/// </summary>
/// <inheritdoc cref="DisablerPatch" path="/remarks[@id='patch']" />
[HarmonyPatch]
[SuppressMessage(category: "csharpsquid", checkId: "S1144")]
[SuppressMessage(category: "csharpsquid", checkId: "S3400")]
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global")]
[SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming")]
internal static class ColonistUnnamedAlertPatch
{
    private static readonly List<Pawn> PawnContainer = [];
    private static readonly HashSet<string> InvalidPawnCandidates = [];
    private static readonly Logger Logger = ToolkitLogManager.GetLogger(typeof(ColonistUnnamedAlertPatch));

    private static IPawnProvider? _pawnProvider;

    [UsedImplicitly]
    private static bool Prepare()
    {
        _pawnProvider = Registries.Compatibilities.Get(id: "Torann.ARimWorldOfMagic") as IPawnProvider;

        if (_pawnProvider != null) return true;

        Logger.Warn(message: "Could not get compatibility provider for RimWorld of Magic. The 'Colonists need names' patch won't be enabled.");

        return false;
    }

    [UsedImplicitly]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Alert_UnnamedColonist), nameof(Alert_UnnamedColonist.GetReport));
    }

    [UsedImplicitly]
    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null) return null;

        Logger.Error(exception, message: "Could not patch {Method} :: Some colonists will be included in the 'Colonists need names' alert in-game", original.FullDescription());

        return null;
    }

    [UsedImplicitly]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantAssignment")]
    private static bool Prefix(ref AlertReport __result)
    {
        __result = false;

        if (!ToolkitSettings.ViewerNamedColonistQueue) return false;

        Map currentMap = Find.CurrentMap;

        if (currentMap == null) return false;

        var component = Current.Game.GetComponent<GameComponentPawns>();

        if (component == null) return false;

        PawnContainer.Clear();
        Dictionary<string, Pawn> pawnHistory = component.pawnHistory;
        List<Pawn> colonistsSpawned = Find.CurrentMap.mapPawns.FreeColonistsSpawned;

        if (colonistsSpawned is not { Count: > 0, }) return false;

        for (var index = 0; index < colonistsSpawned.Count; index++)
        {
            Pawn pawn = colonistsSpawned[index: index];
            string uniquePawnId = pawn.GetUniqueLoadID();

            if (InvalidPawnCandidates.Contains(item: uniquePawnId) || pawnHistory.ContainsKey(key: pawn.LabelShort)) continue;

            if (pawn.IsBorrowedByAnyFaction() || !_pawnProvider!.IsValidPawnCandidate(pawn: pawn).IsSuccess)
            {
                InvalidPawnCandidates.Add(pawn.GetUniqueLoadID());

                continue;
            }

            PawnContainer.Add(item: pawn);
        }

        if (PawnContainer.Count > 0) __result = AlertReport.CulpritsAre(culprits: PawnContainer);

        return false;
    }
}
