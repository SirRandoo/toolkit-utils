using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.PawnQueue;
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
                    pair => ResponseHelper.JoinPair(
                        pair.Key.CapitalizeFirst(),
                        GetSocialString(pawn, pair.Value, pawn.relations.OpinionOf(pair.Value))
                    )
                )
               .ToList();

            twitchMessage.Reply(
                container.Count <= 0 ? "TKUtils.PawnRelations.None".Localize() : container.SectionJoin()
            );
        }

        private static string GetSocialString(Pawn pawn, Pawn otherPawn, int opinion)
        {
            List<PawnRelationDef> relations = pawn.GetRelations(otherPawn).ToList();

            if (relations.Count != 0)
            {
                return relations.Aggregate(
                    "",
                    (current, relation) => current.NullOrEmpty()
                        ? relation.GetGenderSpecificLabelCap(otherPawn)
                        : current + ", " + relation.GetGenderSpecificLabel(otherPawn)
                );
            }

            if (opinion < -20)
            {
                return "Rival".Localize();
            }

            return opinion > 20 ? "Friend".Localize() : "Acquaintance".Localize();
        }
    }
}
