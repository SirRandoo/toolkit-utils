using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings.Windows
{
    public class BackpackDialog : Window
    {
        private string autoEquipDescription;
        private string autoEquipLabel;

        public BackpackDialog()
        {
            doCloseButton = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();

            autoEquipLabel = "TKUtils.Backpack.AutoEquip.Label".Localize();
            autoEquipDescription = "TKUtils.Backpack.AutoEquip.Description".Localize();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();

            GUI.BeginGroup(inRect);

            listing.CheckboxLabeled(autoEquipLabel, ref Backpack.AutoEquip);
            listing.DrawDescription(autoEquipDescription);

            GUI.EndGroup();
        }
    }
}
