using System.Diagnostics.CodeAnalysis;

namespace SirRandoo.ToolkitUtils.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class TraitDump
    {
        public int addPrice { get; set; }
        public bool bypassLimit { get; set; }
        public bool canAdd { get; set; }
        public bool canRemove { get; set; }
        public string[] conflicts { get; set; }
        public string defName { get; set; }
        public int degree { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public int removePrice { get; set; }
        public string[] stats { get; set; }
    }
}
