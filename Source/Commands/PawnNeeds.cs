using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnNeeds : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            if (pawn.needs?.AllNeeds == null)
            {
                twitchMessage.Reply("TKUtils.PawnNeeds.None".Localize().WithHeader("TabNeeds".Localize()));
                return;
            }

            twitchMessage.Reply(
                pawn.needs.AllNeeds.Select(
                        n => ResponseHelper.JoinPair(n.LabelCap, n.CurLevelPercentage.ToStringPercent())
                    )
                   .SectionJoin()
                   .WithHeader("TabNeeds".Localize())
            );
        }
    }
}
