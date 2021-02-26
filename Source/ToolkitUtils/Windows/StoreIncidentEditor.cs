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
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class StoreIncidentEditor : TwitchToolkit.Windows.StoreIncidentEditor
    {
        private readonly EventTypes eventType;
        private readonly List<FloatMenuOption> karmaTypeOptions;
        private readonly string[] karmaTypeStrings = Enum.GetNames(typeof(KarmaType));
        private string codeText;

        private string disableText;
        private string editItemsText;
        private string editPawnsText;
        private string editTraitsText;

        private float headerButtonWidth;
        private string karmaText;
        private string priceText;
        private string resetText;
        private string settingsText;
        private string timesText;
        private float titleWidth;
        private string wagerText;

        public StoreIncidentEditor(StoreIncident storeIncident) : base(storeIncident)
        {
            onlyOneOfTypeAllowed = true;
            eventType = storeIncident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.None;

            karmaTypeOptions = karmaTypeStrings.Select(
                    t => new FloatMenuOption(
                        t,
                        () => storeIncident.karmaType = (KarmaType) Enum.Parse(typeof(KarmaType), t)
                    )
                )
               .ToList();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (!checkedForBackup || !haveBackup)
            {
                MakeSureSaveExists();
            }

            disableText = "TKUtils.Buttons.Disable".Localize();
            resetText = "TKUtils.Buttons.Reset".Localize();
            settingsText = "TKUtils.Buttons.Settings".Localize();
            codeText = "TKUtils.Fields.PurchaseCode".Localize();
            priceText = "TKUtils.Fields.Price".Localize();
            wagerText = "TKUtils.Fields.Wager".Localize();
            karmaText = "TKUtils.Fields.KarmaType".Localize();
            timesText = "TKUtils.Fields.IncidentTimes".Localize(ToolkitSettings.EventCooldownInterval);
            editItemsText = "TKUtils.Buttons.EditItems".Localize();
            editTraitsText = "TKUtils.Buttons.EditTraits".Localize();
            editPawnsText = "TKUtils.Buttons.EditPawns".Localize();

            headerButtonWidth = Mathf.Max(
                                    Text.CalcSize(disableText).x,
                                    Text.CalcSize(resetText).x,
                                    Text.CalcSize(settingsText).x
                                )
                                + 16f;

            titleWidth = Text.CalcSize(storeIncident.LabelCap).x + 16f;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard {maxOneColumn = true};
            listing.Begin(inRect);

            (Rect titleRect, Rect buttonHeaderRect) =
                listing.GetRect(Text.LineHeight).ToForm(titleWidth / inRect.width);

            Widgets.Label(titleRect, storeIncident.LabelCap);
            DrawButtonHeader(buttonHeaderRect.Rounded());
            listing.GapLine(Text.LineHeight * 3);

            if (storeIncident.cost > 0)
            {
                DrawGeneralSettings(listing);
            }

            listing.Gap();

            switch (eventType)
            {
                case EventTypes.Item:
                case EventTypes.PawnKind:
                case EventTypes.Trait:
                    (Rect _, Rect buttonRect) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
                    DrawEditButtonFor(eventType, buttonRect);
                    break;
            }

            listing.End();
        }

        private void DrawGeneralSettings(Listing listing)
        {
            (Rect abbrLabel, Rect abbrField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);

            Widgets.Label(abbrLabel, codeText);
            storeIncident.abbreviation = Widgets.TextField(abbrField, storeIncident.abbreviation);

            if (eventType == EventTypes.None || eventType == EventTypes.Variable)
            {
                (Rect costLabel, Rect costField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);

                listing.Gap();
                Widgets.Label(costLabel, priceText);
                SettingsHelper.DrawPriceField(costField, ref storeIncident.cost);

                if (storeIncident.cost == 0)
                {
                    storeIncident.cost = 1;
                }
            }

            (Rect timesLabel, Rect timesField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
            var timesBuffer = storeIncident.eventCap.ToString();

            listing.Gap();
            Widgets.Label(timesLabel, timesText);
            Widgets.TextFieldNumeric(timesField, ref storeIncident.eventCap, ref timesBuffer, max: 200f);

            if (storeIncidentVariables?.maxWager > 0)
            {
                listing.Gap();

                (Rect wagerLabel, Rect wagerField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
                Widgets.Label(wagerLabel, wagerText);
                SettingsHelper.DrawPriceField(wagerField, ref storeIncidentVariables.maxWager);

                if (storeIncidentVariables.maxWager > 20000)
                {
                    storeIncidentVariables.maxWager = 20000;
                }

                if (storeIncidentVariables.maxWager < storeIncidentVariables.cost)
                {
                    storeIncidentVariables.maxWager = storeIncidentVariables.cost * 2;
                }
            }

            listing.Gap();
            (Rect karmaLabel, Rect karmaField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
            var karmaType = storeIncident.karmaType.ToString();

            Widgets.Label(karmaLabel, karmaText);

            if (Widgets.ButtonText(karmaField, karmaType))
            {
                Find.WindowStack.Add(new FloatMenu(karmaTypeOptions));
            }
        }

        private void DrawButtonHeader(Rect inRect)
        {
            var buttonRect = new Rect(inRect.width - headerButtonWidth, 0, headerButtonWidth, Text.LineHeight);

            GUI.BeginGroup(inRect);

            if (storeIncident.cost > 0)
            {
                if (Widgets.ButtonText(buttonRect, disableText))
                {
                    storeIncident.cost = -10;
                }

                buttonRect = buttonRect.ShiftLeft(0f);
            }

            if (!storeIncident.defName.Equals("Item"))
            {
                if (Widgets.ButtonText(buttonRect, resetText))
                {
                    Store_IncidentEditor.LoadBackup(storeIncident);

                    if (storeIncident.cost < 1)
                    {
                        storeIncident.cost = 50;
                    }

                    MakeSureSaveExists();
                }

                buttonRect = buttonRect.ShiftLeft(0f);
            }

            if (storeIncidentVariables?.customSettings == true)
            {
                if (Widgets.ButtonText(buttonRect, settingsText))
                {
                    storeIncidentVariables.settings.EditSettings();
                }
            }

            GUI.EndGroup();
        }

        private void DrawEditButtonFor(EventTypes type, Rect buttonRect)
        {
            switch (type)
            {
                case EventTypes.PawnKind:
                    if (Widgets.ButtonText(buttonRect, editPawnsText))
                    {
                        Find.WindowStack.Add(new PawnKindConfigDialog());
                    }

                    break;
                case EventTypes.Trait:
                    if (Widgets.ButtonText(buttonRect, editTraitsText))
                    {
                        Find.WindowStack.Add(new TraitConfigDialog());
                    }

                    break;
                case EventTypes.Item:
                    if (Widgets.ButtonText(buttonRect, editItemsText))
                    {
                        Find.WindowStack.Add(new StoreDialog());
                    }

                    break;
            }
        }
    }
}
