using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class CommandItem
    {
        [CanBeNull] public CommandData Data;
        public string Description;
        public string Name;
        public string Usage;
        public UserLevels UserLevel;

        public bool Shortcut => !Data?.IsShortcut ?? false;
    }
}
