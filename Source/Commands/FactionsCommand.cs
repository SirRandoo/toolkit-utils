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
            if (!CommandsHandler.AllowCommand(message))
            {
                return;
            }

            var factions = Current.Game.World.factionManager.AllFactionsVisibleInViewOrder.ToList();

            if (!factions.Any())
            {
                message.Reply("TKUtils.Responses.NoFactions".WithHeader("Factions"));
                return;
            }

            message.Reply(
                string.Join(
                    ", ",
                    factions.Select(f => "TKUtils.Formats.KeyValue".Translate(
                            f.GetCallLabel(),
                            f.PlayerGoodwill.ToStringWithSign()
                        ).ToString()
                    ).ToArray()
                )
            );
        }
    }
}
