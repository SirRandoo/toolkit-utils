using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnFixAll : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            foreach (Viewer viewer in Viewers.All)
            {
                if (!PurchaseHelper.TryGetPawn(viewer.username, out Pawn pawn))
                {
                    continue;
                }

                var name = pawn.Name as NameTriple;

                if (name?.Nick != viewer.username)
                {
                    pawn.Name = new NameTriple(name?.First ?? "", viewer.username, name?.Last ?? "");
                }
            }

            twitchMessage.Reply("TKUtils.FixAll".Localize());
        }
    }
}
