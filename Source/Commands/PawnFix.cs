using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnFix : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            Pawn pawn = GetOrFindPawn(twitchMessage.Username);

            if (pawn == null)
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            var name = pawn.Name as NameTriple;

            if (name?.Nick != twitchMessage.Username)
            {
                pawn.Name = new NameTriple(name?.First ?? "", twitchMessage.Username, name?.Last ?? "");
            }

            twitchMessage.Reply("TKUtils.PawnFix".Localize());
        }
    }
}
