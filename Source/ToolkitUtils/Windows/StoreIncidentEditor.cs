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
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    /// <summary>
    ///     An augmented version of Twitch Toolkit's.
    /// </summary>
    public class StoreIncidentEditor : TwitchToolkit.Windows.StoreIncidentEditor
    {
        private readonly EventTypes _eventType;
        private readonly List<FloatMenuOption> _karmaTypeOptions;
        private string _capBuffer;
        private bool _capBufferValid = true;
        private string _codeText;

        private string _costBuffer;
        private bool _costBufferValid = true;

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
        private string _wagerBuffer;
        private bool _wagerBufferValid = true;
        private string _wagerText;
        private readonly EventItem _item;
        private string _globalCooldownText;
        private string _localCooldownText;
        private string _globalCooldownBuffer;
        private string _localCooldownBuffer;
        private bool _globalCooldownValid;
        private bool _localCooldownValid;

        public StoreIncidentEditor([NotNull] StoreIncident storeIncident) : base(storeIncident)
        {
            onlyOneOfTypeAllowed = true;
            _eventType = storeIncident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.Default;
            _costBuffer = storeIncident.cost.ToString();
            _capBuffer = storeIncident.eventCap.ToString();
            _wagerBuffer = (storeIncidentVariables?.maxWager ?? 0).ToString();
            _karmaTypeOptions = Data.KarmaTypes.Values.Select(t => new FloatMenuOption(t.ToString(), () => storeIncident.karmaType = t)).ToList();
            _item = Data.Events.Find(e => string.Equals(e.DefName, storeIncident.defName));
            
            TkUtils.Logger.Info($"Event data null? {(_item.Data == null).ToStringYesNo()}");

            if (_item.Data == null)
            {
                return;
            }

            _localCooldownValid = true;
            _globalCooldownValid = true;
            _localCooldownBuffer = _item.EventData.LocalCooldown.ToString();
            _globalCooldownBuffer = _item.EventData.GlobalCooldown.ToString();
        }

        /// <inheritdoc cref="Window.PreOpen"/>
        public override void PreOpen()
        {
            base.PreOpen();

            if (!checkedForBackup || !haveBackup)
            {
                MakeSureSaveExists();
            }

            _disableText = "TKUtils.Buttons.Disable".TranslateSimple();
            _resetText = "TKUtils.Buttons.Reset".TranslateSimple();
            _settingsText = "TKUtils.Buttons.Settings".TranslateSimple();
            _codeText = "TKUtils.Fields.PurchaseCode".TranslateSimple();
            _priceText = "TKUtils.Fields.Price".TranslateSimple();
            _wagerText = "TKUtils.Fields.Wager".TranslateSimple();
            _karmaText = "TKUtils.Fields.KarmaType".TranslateSimple();
            _timesText = "TKUtils.Fields.IncidentTimes".Translate(ToolkitSettings.EventCooldownInterval);
            _editItemsText = "TKUtils.Buttons.EditItems".TranslateSimple();
            _editTraitsText = "TKUtils.Buttons.EditTraits".TranslateSimple();
            _editPawnsText = "TKUtils.Buttons.EditPawns".TranslateSimple();
            _globalCooldownText = "TKUtils.Fields.GlobalCooldown".TranslateSimple();
            _localCooldownText = "TKUtils.Fields.LocalCooldown".TranslateSimple();

            _headerButtonWidth = Mathf.Max(Text.CalcSize(_disableText).x, Text.CalcSize(_resetText).x, Text.CalcSize(_settingsText).x) + 16f;

            _titleWidth = Text.CalcSize(storeIncident.LabelCap).x + 16f;
        }

        /// <inheritdoc cref="Window.DoWindowContents"/>
        public override void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard { maxOneColumn = true };
            listing.Begin(inRect);

            (Rect titleRect, Rect buttonHeaderRect) = listing.Split(_titleWidth / inRect.width);

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
                    (Rect _, Rect buttonRect) = listing.Split(0.6f);
                    DrawEditButtonFor(_eventType, buttonRect);

                    break;
            }

            listing.End();
        }

        private void DrawGeneralSettings([NotNull] Listing listing)
        {
            (Rect abbrLabel, Rect abbrField) = listing.GetRect(Text.LineHeight).Split(0.6f);

            UiHelper.Label(abbrLabel, _codeText);

            if (UiHelper.TextField(abbrField, storeIncident.abbreviation, out string newAbbr))
            {
                storeIncident.abbreviation = newAbbr;
            }

            if (_eventType == EventTypes.Default || _eventType == EventTypes.Variable)
            {
                (Rect costLabel, Rect costField) = listing.GetRect(Text.LineHeight).Split(0.6f);

                listing.Gap();
                Widgets.Label(costLabel, _priceText);
                
                if (UiHelper.NumberField(costField, out int value, ref _costBuffer, ref _costBufferValid, 1, int.MaxValue))
                {
                    storeIncident.cost = value;
                }
            }

            if (!storeIncident.defName.Equals("Sanctuary"))
            {
                (Rect capLabel, Rect capField) = listing.Split(0.6f);

                listing.Gap();
                Widgets.Label(capLabel, _timesText);
                
                if (UiHelper.NumberField(capField, out int value, ref _capBuffer, ref _capBufferValid, 1, 200))
                {
                    storeIncident.eventCap = value;
                }
            }

            if (storeIncidentVariables?.maxWager > 0)
            {
                listing.Gap();

                (Rect wagerLabel, Rect wagerField) = listing.Split(0.6f);
                Widgets.Label(wagerLabel, _wagerText);

                if (UiHelper.NumberField(wagerField, out int wager, ref _wagerBuffer, ref _wagerBufferValid, storeIncidentVariables.minPointsToFire, 20000))
                {
                    storeIncidentVariables.maxWager = wager;
                }
            }
            
            DrawCooldownFields(listing);

            listing.Gap();
            (Rect karmaLabel, Rect karmaField) = listing.Split(0.6f);
            Widgets.Label(karmaLabel, _karmaText);

            if (Widgets.ButtonText(karmaField, storeIncident.karmaType.ToString()))
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

                buttonRect = buttonRect.Shift(Direction8Way.West, 0f);
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

                buttonRect = buttonRect.Shift(Direction8Way.West, 0f);
            }

            if (storeIncidentVariables?.customSettings == true && Widgets.ButtonText(buttonRect, _settingsText))
            {
                storeIncidentVariables.settings.EditSettings();
            }

            GUI.EndGroup();
        }
        
        private void DrawCooldownFields([NotNull] Listing listing)
        {
            if (_item.Data == null)
            {
                return;
            }

            (Rect globalLabel, Rect globalField) = listing.Split(0.6f);
            var globalLine = new Rect(globalLabel.x, globalLabel.y, globalLabel.width + globalField.width, globalLabel.height);
            bool hasGlobalCooldown = _item.EventData.HasGlobalCooldown;
            
            Widgets.CheckboxLabeled(hasGlobalCooldown ? globalLabel : globalLine, _globalCooldownText, ref hasGlobalCooldown);

            if (hasGlobalCooldown != _item.EventData.HasGlobalCooldown)
            {
                _item.EventData.HasGlobalCooldown = hasGlobalCooldown;
            }

            if (hasGlobalCooldown && UiHelper.FieldButton(globalLabel, Widgets.CheckboxOnTex))
            {
                _item.EventData.HasGlobalCooldown = !_item.EventData.HasGlobalCooldown;
            }

            if (hasGlobalCooldown && UiHelper.NumberField(globalField, out int globalCooldown, ref _globalCooldownBuffer, ref _globalCooldownValid))
            {
                _item.EventData.GlobalCooldown = globalCooldown;
            }


            (Rect localLabel, Rect localField) = listing.Split(0.6f);
            var localLine = new Rect(localLabel.x, localLabel.y, localLabel.width + localField.width, localLabel.height);
            bool hasLocalCooldown = _item.EventData.HasLocalCooldown;

            Widgets.CheckboxLabeled(hasLocalCooldown ? localLabel : localLine, _localCooldownText, ref hasLocalCooldown);

            if (hasLocalCooldown != _item.EventData.HasLocalCooldown)
            {
                _item.EventData.HasLocalCooldown = hasLocalCooldown;
            }

            if (hasLocalCooldown && UiHelper.NumberField(localField, out int localCooldown, ref _localCooldownBuffer, ref _localCooldownValid))
            {
                _item.EventData.LocalCooldown = localCooldown;
            }
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
