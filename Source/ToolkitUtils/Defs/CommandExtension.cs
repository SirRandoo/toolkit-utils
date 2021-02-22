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
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum UserLevels { Anyone, Vip, Subscriber, Moderator }

    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class Parameter
    {
        [Description("The name of this parameter.")]
        [DefaultValue(null)]
        public string Name;

        [Description("A list of names this parameter can be")]
        [DefaultValue(null)]
        public List<string> Names;

        [Description("Whether or not this parameter can be omitted.")]
        [DefaultValue(false)]
        public bool Optional;

        public override string ToString()
        {
            string name = Names != null ? string.Join("/", Names) : Name.ToStringSafe();
            return Optional ? $"[{name}]" : $"<{name}>";
        }
    }

    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class CommandExtension : DefModExtension
    {
        [Description("A brief overview of what the command does.")]
        public string Description;

        [Description("The arguments this command can handle.")]
        public List<Parameter> Parameters;

        [Description("The permission level a user must have before they can use the command.")]
        [DefaultValue(UserLevels.Anyone)]
        public UserLevels UserLevel;

        public bool HasPermission(Viewer viewer)
        {
            switch (UserLevel)
            {
                case UserLevels.Anyone:
                    return true;
                case UserLevels.Moderator:
                    return viewer.mod || viewer.username.EqualsIgnoreCase(ToolkitSettings.Channel);
                case UserLevels.Subscriber:
                    return viewer.subscriber;
                case UserLevels.Vip:
                    return viewer.vip;
                default:
                    return false;
            }
        }
    }
}
