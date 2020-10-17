using JetBrains.Annotations;
using MoonSharp.Interpreter;
using SirRandoo.ToolkitUtils.Models;

namespace SirRandoo.ToolkitUtils.Proxies
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class PawnKindItemProxy
    {
        private PawnKindItem pawnKindItem;

        [MoonSharpHidden]
        public PawnKindItemProxy(PawnKindItem pawnKindItem)
        {
            this.pawnKindItem = pawnKindItem;
        }

        public string DefName => pawnKindItem.DefName;
        public string Name => pawnKindItem.Name;
        public bool Enabled => pawnKindItem.Enabled;
        public int Cost => pawnKindItem.Cost;
        public bool HasCustomName => pawnKindItem.Data.CustomName;
        public string KarmaType => pawnKindItem.Data.KarmaType.ToString();
    }
}
