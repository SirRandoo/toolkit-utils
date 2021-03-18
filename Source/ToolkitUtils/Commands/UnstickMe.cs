﻿// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class UnstickMe : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!Purchase_Handler.viewerNamesDoingVariableCommands.Contains(twitchMessage.Username.ToLowerInvariant()))
            {
                return;
            }

            if (Find.TickManager?.Paused ?? true)
            {
                twitchMessage.Reply("TKUtils.Paused".Localize());
                return;
            }

            if (Purchase_Handler.viewerNamesDoingVariableCommands.Remove(twitchMessage.Username.ToLowerInvariant()))
            {
                twitchMessage.Reply("TKUtils.UnstickMe".Localize());
            }
        }
    }
}
