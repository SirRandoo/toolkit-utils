using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnRelations : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if (component == null)
            {
                return;
            }

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

        private static string GetSocialString(Pawn pawn, Pawn otherPawn, int opinion)
        {
            PawnRelationDef relations = pawn.GetMostImportantRelation(otherPawn);

            if (relations != null)
            {
                return relations.GetGenderSpecificLabelCap(otherPawn);
            }

            if (Mathf.Abs(opinion) >= TkSettings.OpinionMinimum)
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
