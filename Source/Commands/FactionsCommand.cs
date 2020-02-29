using System.Linq;

using SirRandoo.ToolkitUtils.Utils;

using TwitchToolkit;
using TwitchToolkit.IRC;

using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class FactionsCommand : CommandBase
    {
        public override void RunCommand(IRCMessage message)
        {
            if(!CommandsHandler.AllowCommand(message)) return;

            var filteredFactions = Current.Game.World.factionManager.AllFactionsVisible
                .Where(f => !Current.Game.World.factionManager.OfAncients.Equals(f))
                .Where(f => !Current.Game.World.factionManager.OfMechanoids.Equals(f))
                .Where(f => !Current.Game.World.factionManager.OfInsects.Equals(f))
                .Where(f => !Current.Game.World.factionManager.OfAncientsHostile.Equals(f))
                .Where(f => !Current.Game.World.factionManager.OfPlayer.Equals(f));

            if(filteredFactions.Any())
            {
                var segments = filteredFactions.Select(f => $"{f.GetCallLabel()}: {f.PlayerGoodwill.ToStringWithSign()}");

                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named(string.Join(", ", segments), "MESSAGE")
                    ),
                    message
                );
            }
            else
            {
                SendMessage(
                    "TKUtils.Responses.Format".Translate(
                        NamedArgumentUtility.Named(message.User, "VIEWER"),
                        NamedArgumentUtility.Named("TKUtils.Responses.NoFactions".Translate(), "MESSAGE")
                    ),
                    message
                );
            }
        }
    }
}
