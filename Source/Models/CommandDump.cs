using System.Diagnostics.CodeAnalysis;

namespace SirRandoo.ToolkitUtils.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class CommandDump
    {
        public string description { get; set; }
        public string name { get; set; }
        public bool shortcut { get; set; }
        public string usage { get; set; }
        public string userLevel { get; set; }
    }
}
