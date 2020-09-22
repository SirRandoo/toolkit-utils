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
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnRelations : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if (component == null)
            {
                return;
            }

            string viewer = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrFallback();

            if (!viewer.NullOrEmpty()
                && component.pawnHistory.TryGetValue(viewer.ToLowerInvariant(), out Pawn viewerPawn))
            {
                int theirOpinion = viewerPawn.relations.OpinionOf(pawn);
                int myOpinion = pawn.relations.OpinionOf(viewerPawn);

                string relationship = GetSocialString(pawn, viewerPawn, myOpinion);

                if (relationship.NullOrEmpty())
                {
                    return;
                }

                var container = new List<string>
                {
                    $"{myOpinion.ToStringWithSign()} ({twitchMessage.Username.ToLowerInvariant()})",
                    $"{theirOpinion.ToStringWithSign()} ({viewer.ToLowerInvariant()})"
                };

                twitchMessage.Reply(new[] {relationship, container.SectionJoin()}.GroupedJoin());
                return;
            }

            ShowRelationshipOverview(twitchMessage, component, pawn);
        }

        private static void ShowRelationshipOverview(
            ITwitchMessage twitchMessage,
            GameComponentPawns component,
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

        private static string GetSocialString(Pawn pawn, Pawn otherPawn, int opinion)
        {
            PawnRelationDef relations = pawn.GetMostImportantRelation(otherPawn);

            if (relations != null)
            {
                return relations.GetGenderSpecificLabelCap(otherPawn);
            }

            if (TkSettings.MinimalRelations || Mathf.Abs(opinion) >= TkSettings.OpinionMinimum)
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
