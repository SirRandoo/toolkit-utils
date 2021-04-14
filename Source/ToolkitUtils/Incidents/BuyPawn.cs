// ToolkitUtils
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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class BuyPawn : IncidentVariablesBase
    {
        private PawnKindDef kindDef = PawnKindDefOf.Colonist;
        private IntVec3 loc;
        private Map map;
        private PawnKindItem pawnKindItem;

        public override Viewer Viewer { get; set; }

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            if (CommandBase.GetOrFindPawn(viewer.username) != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.HasPawn".Localize());
                return false;
            }

            map = Helper.AnyPlayerMap;

            if (map == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoMap".Localize());
                return false;
            }

            if (!CellFinder.TryFindRandomEdgeCellWith(
                p => map.reachability.CanReachColony(p) && !p.Fogged(map),
                map,
                CellFinder.EdgeRoadChance_Neutral,
                out loc
            ))
            {
                LogHelper.Warn("No reachable location to spawn a viewer pawn!");
                return false;
            }

            GetDefaultKind();

            if (!TkSettings.PurchasePawnKinds)
            {
                return CanPurchaseRace(viewer, pawnKindItem);
            }

            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(msg).Skip(2));

            if (!worker.TryGetNextAsPawn(out pawnKindItem) || pawnKindItem?.ColonistKindDef == null)
            {
                if (worker.GetLast().NullOrEmpty())
                {
                    return CanPurchaseRace(viewer, pawnKindItem!);
                }

                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidKindQuery".LocalizeKeyed(worker.GetLast()));
                return false;
            }

            kindDef = pawnKindItem.ColonistKindDef;

            if (kindDef.RaceProps.Humanlike)
            {
                return CanPurchaseRace(viewer, pawnKindItem);
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.BuyPawn.Humanlike".Localize());
            return false;
        }

        public override void Execute()
        {
            try
            {
                var request = new PawnGenerationRequest(
                    kindDef,
                    Faction.OfPlayer,
                    tile: map.Tile,
                    allowFood: false,
                    mustBeCapableOfViolence: true
                );
                Pawn pawn = PawnGenerator.GeneratePawn(request);

                if (!(pawn.Name is NameTriple name))
                {
                    LogHelper.Warn("Pawn name is not a name triple!");
                    return;
                }

                GenSpawn.Spawn(pawn, loc, map);
                pawn.Name = new NameTriple(name.First ?? string.Empty, Viewer.username, name.Last ?? string.Empty);
                TaggedString title = "TKUtils.PawnLetter.Title".Localize();
                TaggedString text = "TKUtils.PawnLetter.Description".LocalizeKeyed(Viewer.username);
                PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref title, pawn);

                Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.PositiveEvent, pawn);
                Current.Game.GetComponent<GameComponentPawns>().AssignUserToPawn(Viewer.username, pawn);

                Viewer.Charge(pawnKindItem.Cost, pawnKindItem.Data?.KarmaType ?? storeIncident.karmaType);
                MessageHelper.SendConfirmation(Viewer.username, "TKUtils.BuyPawn.Confirmation".Localize());
            }
            catch (Exception e)
            {
                LogHelper.Error("Could not execute buy pawn", e);
            }
        }

        private static bool CanPurchaseRace([NotNull] Viewer viewer, [NotNull] IShopItemBase target)
        {
            if (!target.Enabled && TkSettings.PurchasePawnKinds)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InformativeDisabledItem".LocalizeKeyed(target.Name)
                );
                return false;
            }

            if (viewer.CanAfford(target.Cost))
            {
                return true;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.InsufficientBalance".LocalizeKeyed(
                    target.Cost.ToString("N0"),
                    viewer.GetViewerCoins().ToString("N0")
                )
            );
            return false;
        }

        private void GetDefaultKind()
        {
            if (Data.TryGetPawnKind($"${PawnKindDefOf.Colonist.race.defName}", out PawnKindItem human)
                && (human!.Enabled || !TkSettings.PurchasePawnKinds))
            {
                kindDef = PawnKindDefOf.Colonist;
                pawnKindItem = human;
                return;
            }

            PawnKindItem randomKind = Data.PawnKinds.FirstOrDefault(k => k.Enabled);

            if (randomKind == null)
            {
                LogHelper.Warn("Could not get next enabled race!");
                return;
            }

            kindDef = randomKind.ColonistKindDef;
            pawnKindItem = randomKind;
        }
    }
}
