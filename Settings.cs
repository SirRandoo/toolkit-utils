using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils
{
    public class Settings : ModSettings
    {
        public static bool Linker = true;
        public static bool ShowApparel = false;
        public static bool ShowArmor = true;
        public static bool ShowSurgeries = true;
        public static bool ShowWeapon = true;
        public static bool TempInGear = false;
        public static bool ViewerRelations = true;
        private static Vector2 ScrollPos = Vector2.zero;

        public void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            var view = new Rect(inRect.x, inRect.y, inRect.width * 0.9f, 26f * 36f);

            listing.BeginScrollView(inRect, ref ScrollPos, ref view);

            listing.Label("TKUtils.Groups.PawnCommands.Label".Translate());
            listing.GapLine();

            listing.CheckboxLabeled("TKUtils.PawnCommands.Health.ShowSurgeries.Label".Translate(), ref ShowSurgeries);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Gear.ShowArmor.Label".Translate(), ref ShowArmor);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Gear.ShowWeapon.Label".Translate(), ref ShowWeapon);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Gear.ShowApparel.Label".Translate(), ref ShowApparel);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Body.TempInGear.Label".Translate(), ref TempInGear);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Relations.OnlyViewers.Label".Translate(), ref ViewerRelations);
            listing.Gap(24);

            listing.Label("TKUtils.Groups.Experimental.Label".Translate());
            listing.GapLine();

            listing.CheckboxLabeled("TKUtils.Experimental.Linker.Label".Translate(), ref Linker);

            listing.EndScrollView(ref view);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref ShowSurgeries, "surgeries", true);
            Scribe_Values.Look(ref ShowArmor, "armor", true);
            Scribe_Values.Look(ref ShowApparel, "apparel", false);
            Scribe_Values.Look(ref TempInGear, "tempInGear", false);
            Scribe_Values.Look(ref ViewerRelations, "viewerRelations", true);

            Scribe_Values.Look(ref Linker, "linker", true);
        }
    }
}
