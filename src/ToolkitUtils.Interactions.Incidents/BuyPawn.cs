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
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using RimWorld.Planet;
using ToolkitCore;
using ToolkitCore.Utilities;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Core.Extensions;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Localization;
using TwitchToolkit.PawnQueue;
using Verse;
using IResult = Remora.Results.IResult;
using Viewer = TwitchToolkit.Viewer;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
public sealed class BuyPawnGroup(ILogger logger) : CommandGroup
{
    private PawnKindDef _kindDef = PawnKindDefOf.Colonist;
    private IntVec3 _loc;
    private Map _map;
    private PawnKindItem _pawnKindItem;

    public async Task<IResult> BuyPawnAsync(Mod.Data.Viewer invoker, PawnProduct product)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(invoker.Id);

        if (pawn != null)
        {
            TwitchWrapper.Client?.SendMessageTo(invoker, Translation.Of("TKUtils.Errors.HasPawn"));

            return Result.FromError(new InvalidOperationError("Viewers may not purchase multiple pawns"));
        }

        if (Find.Maps.Count <= 0)
        {
            TwitchWrapper.Client?.SendMessageTo(invoker, Translation.Of("TKUtils.Errors.NoMap"));

            return GlobalErrors.NoMapCandidate;
        }

        Map[] playerMaps = await MainThreadExtensions.OnMainAsync(() => Current.Game.Maps.Where(m => m.IsPlayerHome).ToArray());
        Map playerMap = playerMaps[ThreadSafeRandom.Next(minimum: 0, playerMaps.Length - 1)];
        Area_Home homeArea = playerMap.areaManager.Home;
        IntVec3 spawnCell;

        foreach (IntVec3 cell in homeArea.ActiveCells)
        {
            bool isWalkable = await MainThreadExtensions.OnMainAsync(GenGrid.Walkable, cell, playerMap);

            if (!isWalkable) continue;

            spawnCell = cell;

            break;
        }
    }

    public override bool CanHappen(string msg, Viewer viewer)
    {
        GetDefaultKind();

        if (!TkSettings.PurchasePawnKinds) return CanPurchaseRace(viewer, _pawnKindItem);

        var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

        if (!worker.TryGetNextAsPawn(out PawnKindItem temp) || _pawnKindItem?.ColonistKindDef == null)
        {
            if (worker.GetLast().NullOrEmpty()) return CanPurchaseRace(viewer, _pawnKindItem!);

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidKindQuery".Translate(worker.GetLast()));

            return false;
        }

        _pawnKindItem = temp;
        _kindDef = _pawnKindItem.ColonistKindDef;

        if (_kindDef.RaceProps.Humanlike) return CanPurchaseRace(viewer, _pawnKindItem);

        MessageHelper.ReplyToUser(viewer.username, "TKUtils.BuyPawn.Humanlike".TranslateSimple());

        return false;
    }

    public override void Execute()
    {
        try
        {
            var request = new PawnGenerationRequest(
                _kindDef,
                Faction.OfPlayer,
                allowFood: false,
                mustBeCapableOfViolence: true,
                fixedIdeo: Find.FactionManager.OfPlayer.ideos.GetRandomIdeoForNewPawn()
            );

            Pawn pawn = PawnGenerator.GeneratePawn(request);

            if (pawn.Name is not NameTriple name)
            {
                Logger.Warn("Pawn name is not a name triple!");

                return;
            }

            PurchaseHelper.SpawnPawn(pawn, _loc, _map);
            pawn.Name = new NameTriple(name.First ?? string.Empty, Viewer.username, name.Last ?? string.Empty);
            TaggedString title = "TKUtils.PawnLetter.Title".TranslateSimple();
            TaggedString text = "TKUtils.PawnLetter.Description".Translate(Viewer.username);
            PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref title, pawn);

            Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.PositiveEvent, pawn);
            Current.Game.GetComponent<GameComponentPawns>().AssignUserToPawn(Viewer.username, pawn);

            if (TkSettings.EasterEggs && Basket.TryGetEggFor(Viewer.username, out IEasterEgg egg) && Rand.Chance(egg.Chance) && egg.IsPossible(storeIncident, Viewer))
                egg.Execute(Viewer, pawn);

            Viewer.Charge(_pawnKindItem.Cost, _pawnKindItem.Data?.KarmaType ?? storeIncident.karmaType);
            MessageHelper.SendConfirmation(Viewer.username, "TKUtils.BuyPawn.Confirmation".TranslateSimple());
        }
        catch (Exception e) { Logger.Error(message: "Could not execute buy pawn", e); }
    }

    private static bool CanPurchaseRace(Viewer viewer, [NotNull] IShopItemBase target)
    {
        if (!target.Enabled && TkSettings.PurchasePawnKinds)
        {
            MessageHelper.ReplyToUser(viewer.username, "TKUtils.InformativeDisabledItem".Translate(target.Name));

            return false;
        }

        if (viewer.CanAfford(target.Cost)) return true;

        MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".Translate(target.Cost.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

        return false;
    }

    private void GetDefaultKind()
    {
        if (PositionData.Data.TryGetPawnKind($"${PawnKindDefOf.Colonist.race.defName}", out PawnKindItem human) && (human!.Enabled || !TkSettings.PurchasePawnKinds))
        {
            _kindDef = PawnKindDefOf.Colonist;
            _pawnKindItem = human;

            return;
        }

        PawnKindItem randomKind = PositionData.Data.PawnKinds.FirstOrDefault(k => k.Enabled);

        if (randomKind == null)
        {
            Logger.Warn("Could not get next enabled race!");

            return;
        }

        _kindDef = randomKind.ColonistKindDef;
        _pawnKindItem = randomKind;
    }
}
