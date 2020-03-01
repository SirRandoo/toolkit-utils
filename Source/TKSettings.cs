using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils
{
    public class TKSettings : ModSettings
    {
        public static bool Commands = true;
        public static string Prefix = "!";
        public static bool RichText = true;
        public static bool ShowApparel = false;
        public static bool ShowArmor = true;
        public static bool ShowSurgeries = true;
        public static bool ShowWeapon = true;
        public static bool TempInGear = false;
        public static bool VersionedModList = false;
        public static bool ViewerRelations = true;
        private static Vector2 ScrollPos = Vector2.zero;

        public void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            var view = new Rect(inRect.x, inRect.y, inRect.width * 0.9f, 26f * 36f);

            listing.BeginScrollView(inRect, ref ScrollPos, ref view);

            listing.Label("TKUtils.Groups.CommandTweaks.Label".Translate());
            listing.GapLine();

            listing.CheckboxLabeled("TKUtils.CommandTweaks.VersionedModList.Label".Translate(), ref VersionedModList);
            listing.CheckboxLabeled("TKUtils.CommandTweaks.RichText.Label".Translate(), ref RichText);
            listing.CheckboxLabeled("TKUtils.CommandTweaks.Label".Translate(), ref Commands);

            if(Commands)
            {
                Prefix = listing.TextEntryLabeled("TKUtils.CommandTweaks.CommandPrefix.Label".Translate(), Prefix);

                if(Prefix.StartsWith(" ") || Prefix.StartsWith("/") || Prefix.StartsWith("."))
                {
                    Prefix = "!";
                    Log.Message($"WARN {TKUtils.ID} :: User attempted to use a reserved character in their command prefix!  Reseting...");
                }
            }
            listing.Gap(24);

            listing.Label("TKUtils.Groups.PawnCommands.Label".Translate());
            listing.GapLine();

            listing.CheckboxLabeled("TKUtils.PawnCommands.Body.TempInGear.Label".Translate(), ref TempInGear);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Gear.ShowApparel.Label".Translate(), ref ShowApparel);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Gear.ShowArmor.Label".Translate(), ref ShowArmor);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Gear.ShowWeapon.Label".Translate(), ref ShowWeapon);
            listing.CheckboxLabeled("TKUtils.PawnCommands.Health.ShowSurgeries.Label".Translate(), ref ShowSurgeries);
            listing.Gap(24);

            listing.EndScrollView(ref view);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Prefix, "prefix", "!");
            Scribe_Values.Look(ref Commands, "commands", true);
            Scribe_Values.Look(ref RichText, "richText", true);

            Scribe_Values.Look(ref ShowSurgeries, "surgeries", true);
            Scribe_Values.Look(ref ShowArmor, "armor", true);
            Scribe_Values.Look(ref ShowApparel, "apparel", false);
            Scribe_Values.Look(ref TempInGear, "tempInGear", false);
        }
    }
}
