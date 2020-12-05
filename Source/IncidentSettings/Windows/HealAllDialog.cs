using SirRandoo.ToolkitUtils.Helpers;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings.Windows
{
    public class HealAllDialog : Window
    {
        private string fairFightsDescription;
        private string fairFightsLabel;

        public HealAllDialog()
        {
            doCloseButton = true;
        }

        public override void PreOpen()
        {
            base.PreOpen();

            fairFightsLabel = "TKUtils.Heal.FairFights.Label".Localize();
            fairFightsDescription = "TKUtils.Heal.FairFights.Description".Localize();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled(fairFightsLabel, ref HealMe.FairFights);
            listing.DrawDescription(fairFightsDescription);

            listing.End();
        }
    }
}
