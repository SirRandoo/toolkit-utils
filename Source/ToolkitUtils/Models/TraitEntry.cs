using SirRandoo.ToolkitUtils.Windows;

namespace SirRandoo.ToolkitUtils.Models
{
    public enum TraitTargets
    {
        Global,
        Name,
        State,
        AddPrice,
        RemovePrice,
        CanAdd,
        CanRemove,
        BypassLimit,
        AddKarma,
        RemoveKarma
    }

    public class TraitEntry
    {
        public string FieldContents { get; set; }
        public FieldTypes FieldType { get; set; }
        public TraitTargets Target { get; set; }
    }
}
