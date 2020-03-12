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

                SendCommandMessage(
                    string.Join("TKUtils.Misc.Separators.Inner".Translate(), segments),
                    message
                );
            }
            else
            {
                SendCommandMessage("TKUtils.Responses.NoFactions".Translate(), message);
            }
        }
    }
}
