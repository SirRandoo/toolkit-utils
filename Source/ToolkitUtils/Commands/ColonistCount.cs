// ToolkitUtils
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

using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class ColonistCount : CommandBase
    {
        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            if (Find.ColonistBar == null)
            {
                return;
            }

            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder();

            if (colonists.Count <= 0)
            {
                twitchMessage.Reply("TKUtils.ColonistCount.None".Localize());
                return;
            }

            twitchMessage.Reply("TKUtils.ColonistCount.Any".LocalizeKeyed(colonists.Count.ToString("N0")));
        }
    }
}
