﻿using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class Wealth : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (Current.Game?.CurrentMap == null)
            {
                return;
            }

            twitchMessage.Reply(
                ResponseHelper.JoinPair(
                    "ThisMapColonyWealthTotal".Localize(),
                    Current.Game.CurrentMap.wealthWatcher.WealthTotal.ToString("N0")
                )
            );
        }
    }
}