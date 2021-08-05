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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnRelations : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage msg)
        {
            if (!PurchaseHelper.TryGetPawn(msg.Username, out Pawn pawn))
            {
                msg.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if (component == null)
            {
                return;
            }

            string viewer = CommandFilter.Parse(msg.Message).Skip(1).FirstOrFallback();

            if (!viewer.NullOrEmpty()
                && component.pawnHistory.TryGetValue(viewer.ToLowerInvariant(), out Pawn viewerPawn))
            {
                int theirOpinion = viewerPawn.relations.OpinionOf(pawn);
                int myOpinion = pawn!.relations.OpinionOf(viewerPawn);
                string relationship = GetSocialString(pawn, viewerPawn, myOpinion, true);

                if (relationship.NullOrEmpty())
                {
                    return;
                }

                var container = new List<string>
                {
                    $"{myOpinion.ToStringWithSign()} ({msg.Username!.ToLowerInvariant()})",
                    $"{theirOpinion.ToStringWithSign()} ({viewer.ToLowerInvariant()})"
                };

                msg.Reply(new[] { relationship, container.SectionJoin() }.GroupedJoin());
                return;
            }

            ShowRelationshipOverview(msg, component, pawn);
        }

        private static void ShowRelationshipOverview(
            [NotNull] ITwitchMessage twitchMessage,
            [NotNull] GameComponentPawns component,
            Pawn pawn
        )
        {
            List<string> container = component.pawnHistory.Where(p => p.Value != pawn)
               .Select(
                    pair =>
                    {
                        string relationString = GetSocialString(pawn, pair.Value, pawn.relations.OpinionOf(pair.Value));
                        return relationString == null
                            ? null
                            : ResponseHelper.JoinPair(pair.Key.CapitalizeFirst(), relationString);
                    }
                )
               .Where(s => s != null)
               .ToList();

            twitchMessage.Reply(
                container.Count <= 0 ? "TKUtils.PawnRelations.None".Localize() : container.SectionJoin()
            );
        }

        private static string GetSocialString(Pawn pawn, Pawn otherPawn, int opinion, bool overrideSettings = false)
        {
            PawnRelationDef relations = pawn.GetMostImportantRelation(otherPawn);

            if (relations != null)
            {
                return relations.GetGenderSpecificLabelCap(otherPawn);
            }

            if (!overrideSettings && (TkSettings.MinimalRelations || Mathf.Abs(opinion) < TkSettings.OpinionMinimum))
            {
                return null;
            }

            if (opinion < -20)
            {
                return "Rival".Localize();
            }

            return opinion > 20 ? "Friend".Localize() : "Acquaintance".Localize();
        }
    }
}
