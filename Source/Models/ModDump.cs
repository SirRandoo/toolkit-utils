using System.Diagnostics.CodeAnalysis;

namespace SirRandoo.ToolkitUtils.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ModDump
    {
        public string author { get; set; }
        public string name { get; set; }
        public string steamId { get; set; }
        public string version { get; set; }
    }
}
