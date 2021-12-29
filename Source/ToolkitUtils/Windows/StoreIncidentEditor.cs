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
using JetBrains.Annotations;
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
        private readonly EventTypes _eventType;
        private readonly List<FloatMenuOption> _karmaTypeOptions;
        private string _codeText;

        private string _disableText;
        private string _editItemsText;
        private string _editPawnsText;
        private string _editTraitsText;

        private float _headerButtonWidth;
        private string _karmaText;
        private string _priceText;
        private string _resetText;
        private string _settingsText;
        private string _timesText;
        private float _titleWidth;
        private string _wagerText;

        public StoreIncidentEditor([NotNull] StoreIncident storeIncident) : base(storeIncident)
        {
            onlyOneOfTypeAllowed = true;
            _eventType = storeIncident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.Default;

            _karmaTypeOptions = Data.KarmaTypes.Select(t => new FloatMenuOption(t.ToString(), () => storeIncident.karmaType = t)).ToList();
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if (!checkedForBackup || !haveBackup)
            {
                MakeSureSaveExists();
            }

            _disableText = "TKUtils.Buttons.Disable".Localize();
            _resetText = "TKUtils.Buttons.Reset".Localize();
            _settingsText = "TKUtils.Buttons.Settings".Localize();
            _codeText = "TKUtils.Fields.PurchaseCode".Localize();
            _priceText = "TKUtils.Fields.Price".Localize();
            _wagerText = "TKUtils.Fields.Wager".Localize();
            _karmaText = "TKUtils.Fields.KarmaType".Localize();
            _timesText = "TKUtils.Fields.IncidentTimes".LocalizeKeyed(ToolkitSettings.EventCooldownInterval);
            _editItemsText = "TKUtils.Buttons.EditItems".Localize();
            _editTraitsText = "TKUtils.Buttons.EditTraits".Localize();
            _editPawnsText = "TKUtils.Buttons.EditPawns".Localize();

            _headerButtonWidth = Mathf.Max(Text.CalcSize(_disableText).x, Text.CalcSize(_resetText).x, Text.CalcSize(_settingsText).x) + 16f;

            _titleWidth = Text.CalcSize(storeIncident.LabelCap).x + 16f;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard { maxOneColumn = true };
            listing.Begin(inRect);

            (Rect titleRect, Rect buttonHeaderRect) = listing.GetRect(Text.LineHeight).ToForm(_titleWidth / inRect.width);

            Widgets.Label(titleRect, storeIncident.LabelCap);
            DrawButtonHeader(buttonHeaderRect.Rounded());
            listing.GapLine(Text.LineHeight * 3);

            if (storeIncident.cost > 0)
            {
                DrawGeneralSettings(listing);
            }

            listing.Gap();

            switch (_eventType)
            {
                case EventTypes.Item:
                case EventTypes.PawnKind:
                case EventTypes.Trait:
                    (Rect _, Rect buttonRect) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
                    DrawEditButtonFor(_eventType, buttonRect);

                    break;
            }

            listing.End();
        }

        private void DrawGeneralSettings([NotNull] Listing listing)
        {
            (Rect abbrLabel, Rect abbrField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);

            Widgets.Label(abbrLabel, _codeText);
            storeIncident.abbreviation = Widgets.TextField(abbrField, storeIncident.abbreviation);

            if (_eventType == EventTypes.Default || _eventType == EventTypes.Variable)
            {
                (Rect costLabel, Rect costField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);

                listing.Gap();
                Widgets.Label(costLabel, _priceText);
                SettingsHelper.DrawPriceField(costField, ref storeIncident.cost);

                if (storeIncident.cost == 0)
                {
                    storeIncident.cost = 1;
                }
            }

            if (!storeIncident.defName.Equals("Sanctuary"))
            {
                (Rect timesLabel, Rect timesField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
                var timesBuffer = storeIncident.eventCap.ToString();

                listing.Gap();
                Widgets.Label(timesLabel, _timesText);
                Widgets.TextFieldNumeric(timesField, ref storeIncident.eventCap, ref timesBuffer, max: 200f);
            }

            if (storeIncidentVariables?.maxWager > 0)
            {
                listing.Gap();

                (Rect wagerLabel, Rect wagerField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
                Widgets.Label(wagerLabel, _wagerText);
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

            Widgets.Label(karmaLabel, _karmaText);

            if (Widgets.ButtonText(karmaField, karmaType))
            {
                Find.WindowStack.Add(new FloatMenu(_karmaTypeOptions));
            }
        }

        private void DrawButtonHeader(Rect inRect)
        {
            var buttonRect = new Rect(inRect.width - _headerButtonWidth, 0, _headerButtonWidth, Text.LineHeight);

            GUI.BeginGroup(inRect);

            if (storeIncident.cost > 0)
            {
                if (Widgets.ButtonText(buttonRect, _disableText))
                {
                    storeIncident.cost = -10;
                }

                buttonRect = buttonRect.ShiftLeft(0f);
            }

            if (!storeIncident.defName.Equals("Item"))
            {
                if (Widgets.ButtonText(buttonRect, _resetText))
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
                if (Widgets.ButtonText(buttonRect, _settingsText))
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
                    if (Widgets.ButtonText(buttonRect, _editPawnsText))
                    {
                        Find.WindowStack.Add(new PawnKindConfigDialog());
                    }

                    break;
                case EventTypes.Trait:
                    if (Widgets.ButtonText(buttonRect, _editTraitsText))
                    {
                        Find.WindowStack.Add(new TraitConfigDialog());
                    }

                    break;
                case EventTypes.Item:
                    if (Widgets.ButtonText(buttonRect, _editItemsText))
                    {
                        Find.WindowStack.Add(new StoreDialog());
                    }

                    break;
            }
        }
    }
}
