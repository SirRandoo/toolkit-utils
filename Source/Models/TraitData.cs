using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TraitData
    {
        public bool CanBypassLimit;
        [DefaultValue(new string[] { })] public string[] Conflicts;
        public bool CustomName;

        [DefaultValue(null)] public KarmaType? KarmaTypeForAdding;

        [DefaultValue(null)] public KarmaType? KarmaTypeForRemoving;

        [DefaultValue(new string[] { })] public string[] Stats;
    }
}
