﻿// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Entities;
using SirRandoo.CommonLib.Interfaces;
using ToolkitCore.Utilities;
using ToolkitUtils.Helpers;
using ToolkitUtils.Interfaces;
using ToolkitUtils.Models.Serialization;
using ToolkitUtils.Utils;
using ToolkitUtils.Workers;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;

namespace ToolkitUtils.Incidents
{
    public class BuyPawn : IncidentVariablesBase
    {
        private static readonly IRimLogger Logger = new RimThreadedLogger("ToolkitUtils.Incidents.BuyPawn");

        private PawnKindDef _kindDef = PawnKindDefOf.Colonist;
        private IntVec3 _loc;
        private Map _map;
        private PawnKindItem _pawnKindItem;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (CommandBase.GetOrFindPawn(viewer.username) != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.HasPawn".TranslateSimple());

                return false;
            }

            _map = Helper.AnyPlayerMap;

            if (_map == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoMap".TranslateSimple());

                return false;
            }

            if (!CellFinder.TryFindRandomEdgeCellWith(p => _map.reachability.CanReachColony(p) && !p.Fogged(_map), _map, CellFinder.EdgeRoadChance_Neutral, out _loc))
            {
                Logger.Warn("No reachable location to spawn a viewer pawn!");

                return false;
            }

            GetDefaultKind();

            if (!TkSettings.PurchasePawnKinds)
            {
                return CanPurchaseRace(viewer, _pawnKindItem);
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsPawn(out PawnKindItem temp) || _pawnKindItem?.ColonistKindDef == null)
            {
                if (worker.GetLast().NullOrEmpty())
                {
                    return CanPurchaseRace(viewer, _pawnKindItem!);
                }

                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidKindQuery".Translate(worker.GetLast()));

                return false;
            }

            _pawnKindItem = temp;
            _kindDef = _pawnKindItem.ColonistKindDef;

            if (_kindDef.RaceProps.Humanlike)
            {
                return CanPurchaseRace(viewer, _pawnKindItem);
            }

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

                if (!(pawn.Name is NameTriple name))
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
                {
                    egg.Execute(Viewer, pawn);
                }

                Viewer.Charge(_pawnKindItem.Cost, _pawnKindItem.Data?.KarmaType ?? storeIncident.karmaType);
                MessageHelper.SendConfirmation(Viewer.username, "TKUtils.BuyPawn.Confirmation".TranslateSimple());
            }
            catch (Exception e)
            {
                Logger.Error("Could not execute buy pawn", e);
            }
        }

        private static bool CanPurchaseRace([NotNull] Viewer viewer, [NotNull] IShopItemBase target)
        {
            if (!target.Enabled && TkSettings.PurchasePawnKinds)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InformativeDisabledItem".Translate(target.Name));

                return false;
            }

            if (viewer.CanAfford(target.Cost))
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.InsufficientBalance".Translate(target.Cost.ToString("N0"), viewer.GetViewerCoins().ToString("N0")));

            return false;
        }

        private void GetDefaultKind()
        {
            if (Data.TryGetPawnKind($"${PawnKindDefOf.Colonist.race.defName}", out PawnKindItem human) && (human!.Enabled || !TkSettings.PurchasePawnKinds))
            {
                _kindDef = PawnKindDefOf.Colonist;
                _pawnKindItem = human;

                return;
            }

            PawnKindItem randomKind = Data.PawnKinds.FirstOrDefault(k => k.Enabled);

            if (randomKind == null)
            {
                Logger.Warn("Could not get next enabled race!");

                return;
            }

            _kindDef = randomKind.ColonistKindDef;
            _pawnKindItem = randomKind;
        }
    }
}
