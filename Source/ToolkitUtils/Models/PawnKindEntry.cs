using SirRandoo.ToolkitUtils.Windows;

namespace SirRandoo.ToolkitUtils.Models
{
    public enum PawnKindTargets { Global, Name, State, Price, Karma }

    public class PawnKindEntry
    {
        public string FieldContents { get; set; }
        public FieldTypes FieldType { get; set; }
        public PawnKindTargets Target { get; set; }
    }
}
