using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore;
using ToolkitCore.Models;
using TwitchLib.Client.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    public class FactionsCommand : CommandBase
    {
        private List<Faction> factions;

        public override bool CanExecute(ITwitchCommand twitchCommand)
        {
            if (!base.CanExecute(twitchCommand))
            {
                return false;
            }

            factions = Current.Game.World.factionManager.AllFactionsVisibleInViewOrder.ToList();

            return factions.Any();
        }

        public override void Execute(ITwitchCommand twitchCommand)
        {
            twitchCommand.Reply(
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

        public FactionsCommand(ToolkitChatCommand command) : base(command)
        {
        }
    }
}
