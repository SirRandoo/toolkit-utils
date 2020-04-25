using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class FactionsCommand : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            var factions = Current.Game.World.factionManager.AllFactionsVisibleInViewOrder.ToList();

            if (!factions.Any())
            {
                twitchMessage.Reply("TKUtils.Responses.NoFactions".WithHeader("Factions"));
                return;
            }

            twitchMessage.Reply(
                string.Join(
                    ", ",
                    factions.Select(
                            f => "TKUtils.Formats.KeyValue".Translate(
                                    f.GetCallLabel(),
                                    f.PlayerGoodwill.ToStringWithSign()
                                )
                                .ToString()
                        )
                        .ToArray()
                )
            );
        }
    }
}
