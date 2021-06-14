// ToolkitUtils
// Copyright (C) 2021  SirRandoo
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.IncidentSettings.Windows;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.Incidents;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings
{
    [UsedImplicitly]
    public class Item : IncidentHelperVariablesSettings, IEventSettings
    {
        public static bool Stuff = true;
        public static bool Quality = true;

        public static bool AwfulQuality = true;
        public static float AwfulMultiplier = 0.5f;
        public static bool PoorQuality = true;
        public static float PoorMultiplier = 0.75f;
        public static bool NormalQuality = true;
        public static float NormalMultiplier = 1f;
        public static bool GoodQuality = true;
        public static float GoodMultiplier = 1.25f;
        public static bool ExcellentQuality = true;
        public static float ExcellentMultiplier = 1.5f;
        public static bool MasterworkQuality = true;
        public static float MasterworkMultiplier = 2.5f;
        public static bool LegendaryQuality = true;
        public static float LegendaryMultiplier = 5f;

        internal static string AwfulMultiplierBuffer;
        internal static string PoorMultiplierBuffer;
        internal static string NormalMultiplierBuffer;
        internal static string GoodMultiplierBuffer;
        internal static string ExcellentMultiplierBuffer;
        internal static string MasterworkMultiplierBuffer;
        internal static string LegendaryMultiplierBuffer;

        public int LineSpan => 9;

        public void Draw(Rect canvas, float preferredHeight)
        {
            var listing = new Listing_Standard();
            listing.Begin(canvas);

            (Rect awfulLabel, Rect awfulField) = listing.GetRect(preferredHeight).ToForm(0.7f);
            SettingsHelper.DrawLabel(awfulLabel, "TKUtils.Item.AwfulMultiplier".Localize());
            Widgets.TextFieldNumeric(awfulField, ref AwfulMultiplier, ref AwfulMultiplierBuffer);

            if (SettingsHelper.DrawFieldButton(
                awfulLabel,
                AwfulQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                AwfulQuality = !AwfulQuality;
            }

            (Rect poorLabel, Rect poorField) = listing.GetRect(preferredHeight).ToForm(0.7f);
            SettingsHelper.DrawLabel(poorLabel, "TKUtils.Item.PoorMultiplier".Localize());
            Widgets.TextFieldNumeric(poorField, ref PoorMultiplier, ref PoorMultiplierBuffer);

            if (SettingsHelper.DrawFieldButton(poorLabel, PoorQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                PoorQuality = !PoorQuality;
            }

            (Rect normalLabel, Rect normalField) = listing.GetRect(preferredHeight).ToForm(0.7f);
            SettingsHelper.DrawLabel(normalLabel, "TKUtils.Item.NormalMultiplier".Localize());
            Widgets.TextFieldNumeric(normalField, ref NormalMultiplier, ref NormalMultiplierBuffer);

            if (SettingsHelper.DrawFieldButton(
                normalLabel,
                NormalQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                NormalQuality = !NormalQuality;
            }

            (Rect goodLabel, Rect goodField) = listing.GetRect(preferredHeight).ToForm(0.7f);
            SettingsHelper.DrawLabel(goodLabel, "TKUtils.Item.GoodMultiplier".Localize());
            Widgets.TextFieldNumeric(goodField, ref GoodMultiplier, ref GoodMultiplierBuffer);

            if (SettingsHelper.DrawFieldButton(goodLabel, GoodQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex))
            {
                GoodQuality = !GoodQuality;
            }

            (Rect excLabel, Rect excField) = listing.GetRect(preferredHeight).ToForm(0.7f);
            SettingsHelper.DrawLabel(excLabel, "TKUtils.Item.ExcellentMultiplier".Localize());
            Widgets.TextFieldNumeric(excField, ref ExcellentMultiplier, ref ExcellentMultiplierBuffer);


            if (SettingsHelper.DrawFieldButton(
                excLabel,
                ExcellentQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                ExcellentQuality = !ExcellentQuality;
            }

            (Rect mWorkLabel, Rect mWorkField) = listing.GetRect(preferredHeight).ToForm(0.7f);
            SettingsHelper.DrawLabel(mWorkLabel, "TKUtils.Item.MasterworkMultiplier".Localize());
            Widgets.TextFieldNumeric(mWorkField, ref MasterworkMultiplier, ref MasterworkMultiplierBuffer);


            if (SettingsHelper.DrawFieldButton(
                mWorkLabel,
                MasterworkQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                MasterworkQuality = !MasterworkQuality;
            }

            (Rect legLabel, Rect legField) = listing.GetRect(preferredHeight).ToForm(0.7f);
            SettingsHelper.DrawLabel(legLabel, "TKUtils.Item.LegendaryMultiplier".Localize());
            Widgets.TextFieldNumeric(legField, ref LegendaryMultiplier, ref LegendaryMultiplierBuffer);


            if (SettingsHelper.DrawFieldButton(
                legLabel,
                LegendaryQuality ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex
            ))
            {
                LegendaryQuality = !LegendaryQuality;
            }

            listing.CheckboxLabeled(
                "TKUtils.Item.Stuff.Label".Localize(),
                ref Stuff,
                "TKUtils.Item.Stuff.Description".Localize()
            );
            listing.CheckboxLabeled(
                "TKUtils.Item.Quality.Label".Localize(),
                ref Quality,
                "TKUtils.Item.Quality.Description".Localize()
            );
            listing.CheckboxLabeled(
                "TKUtils.Item.Research.Label".Localize(),
                ref BuyItemSettings.mustResearchFirst,
                "TKUtils.Item.Research.Description".Localize()
            );

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref Stuff, "buyItemMaterial", true);
            Scribe_Values.Look(ref Quality, "buyItemQuality");
            Scribe_Values.Look(ref AwfulQuality, "awfulQuality", true);
            Scribe_Values.Look(ref AwfulMultiplier, "awfulQualityMultiplier", 0.5f);
            Scribe_Values.Look(ref PoorQuality, "poorQuality", true);
            Scribe_Values.Look(ref PoorMultiplier, "poorQualityMultiplier", 0.75f);
            Scribe_Values.Look(ref NormalQuality, "normalQuality", true);
            Scribe_Values.Look(ref NormalMultiplier, "normalQualityMultiplier", 1f);
            Scribe_Values.Look(ref GoodQuality, "goodQuality", true);
            Scribe_Values.Look(ref GoodMultiplier, "goodQualityMultiplier", 1.25f);
            Scribe_Values.Look(ref ExcellentQuality, "excellentQuality");
            Scribe_Values.Look(ref ExcellentMultiplier, "excellentQualityMultiplier", 1.5f);
            Scribe_Values.Look(ref MasterworkQuality, "masterworkQuality");
            Scribe_Values.Look(ref MasterworkMultiplier, "masterworkQualityMultiplier", 2.5f);
            Scribe_Values.Look(ref LegendaryQuality, "legendaryQuality");
            Scribe_Values.Look(ref LegendaryMultiplier, "legendaryQualityMultiplier", 5f);
            Scribe_Values.Look(ref BuyItemSettings.mustResearchFirst, "BuyItemSettings.mustResearchFirst", true);
        }

        public override void EditSettings()
        {
            Find.WindowStack.Add(new BuyItemDialog());
        }
    }
}
