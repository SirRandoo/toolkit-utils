using System.ComponentModel;
using JetBrains.Annotations;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class PawnKindData
    {
        public bool CustomName;

        [DefaultValue(null)] public KarmaType? KarmaType;
    }
}
