using System.Diagnostics.CodeAnalysis;

namespace SirRandoo.ToolkitUtils.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class RaceDump
    {
        public string defName { get; set; }
        public bool enabled { get; set; }
        public string name { get; set; }
        public int price { get; set; }
    }
}
