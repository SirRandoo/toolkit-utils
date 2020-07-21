using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ShopLegacy
    {
        public List<TraitItem> Traits { get; set; }
        public List<PawnKindItem> Races { get; set; }
    }
}
