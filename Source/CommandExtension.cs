using System.Collections.Generic;
using JetBrains.Annotations;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum UserLevels
    {
        Anyone,
        Vip,
        Subscriber,
        Moderator
    }

    [UsedImplicitly]
    public class Parameter
    {
        [Description("The name of this parameter.")]
        public string Name;

        [Description("Whether or not this parameter can be omitted.")]
        [DefaultValue(false)]
        public bool Optional;

        public override string ToString()
        {
            return Optional ? $"[{Name}]" : $"<{Name}>";
        }
    }

    [UsedImplicitly]
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
