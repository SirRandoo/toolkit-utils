using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Factions : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            List<Faction> factions = Current.Game.World.factionManager.AllFactionsVisibleInViewOrder
               .Where(i => !i.IsPlayer)
               .ToList();

            if (!factions.Any())
            {
                twitchMessage.Reply("TKUtils.Factions.None".Localize().WithHeader("WorldFactionsTab".Localize()));
                return;
            }

            twitchMessage.Reply(
                factions.Select(f => ResponseHelper.JoinPair(f.GetCallLabel(), f.PlayerGoodwill.ToStringWithSign()))
                   .SectionJoin()
            );
        }
    }
}
