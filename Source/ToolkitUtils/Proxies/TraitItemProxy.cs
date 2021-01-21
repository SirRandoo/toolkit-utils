using JetBrains.Annotations;
using MoonSharp.Interpreter;
using SirRandoo.ToolkitUtils.Models;

namespace SirRandoo.ToolkitUtils.Proxies
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class TraitItemProxy
    {
        private readonly TraitItem traitItem;

        [MoonSharpHidden]
        public TraitItemProxy(TraitItem traitItem)
        {
            this.traitItem = traitItem;
        }

        public bool CanAdd => traitItem.CanAdd;
        public bool CanRemove => traitItem.CanRemove;
        public int CostToAdd => traitItem.CostToAdd;
        public int CostToRemove => traitItem.CostToRemove;
        public string DefName => traitItem.DefName;
        public int Degree => traitItem.Degree;
        public string Name => traitItem.Name;
        public string[] Conflicts => traitItem.Data.Conflicts;
        public bool CanBypassLimit => traitItem.Data.CanBypassLimit;
        public string AddKarmaType => traitItem.Data.KarmaTypeForAdding.ToString();
        public string RemoveKarmaType => traitItem.Data.KarmaTypeForRemoving.ToString();
        public bool HasCustomName => traitItem.Data.CustomName;
    }
}
