using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SirRandoo.ToolkitUtils.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ShopDump
    {
        public List<TraitDump> traits { get; set; }
        public List<RaceDump> races { get; set; }
    }
}
