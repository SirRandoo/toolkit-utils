﻿using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using TwitchToolkit.Windows;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class StoreIncidentEditor : TwitchToolkit.Windows.StoreIncidentEditor
    {
        protected List<FloatMenuOption> KarmaTypeOptions;
        protected string[] KarmaTypeStrings = Enum.GetNames(typeof(KarmaType));

        public StoreIncidentEditor(StoreIncident storeIncident) : base(storeIncident)
        {
            KarmaTypeOptions = KarmaTypeStrings
                .Select(
                    t => new FloatMenuOption(
                        t,
                        () => storeIncident.karmaType = (KarmaType) Enum.Parse(typeof(KarmaType), t)
                    )
                )
                .ToList();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (!checkedForBackup || !haveBackup)
            {
                MakeSureSaveExists();
                return;
            }

            var offset = 0f;
            var listing = new Listing_Standard {maxOneColumn = true};
            listing.Begin(inRect);

            var buttonGroup = listing.GetRect(Text.LineHeight);

            if (storeIncident.cost > 0)
            {
                var disableText = "TKUtils.Windows.IncidentEditor.Disable".Translate();
                var disableWidth = Text.CalcSize(disableText).x * 1.5f;
                var disableRect = new Rect(buttonGroup.width - disableWidth, 0, disableWidth, Text.LineHeight);
                offset += disableWidth + 5f;

                if (Widgets.ButtonText(disableRect, disableText))
                {
                    storeIncident.cost = -10;
                }
            }

            if (!storeIncident.defName.Equals("Item"))
            {
                var resetText = "TKUtils.Windows.IncidentEditor.Reset".Translate();
                var resetWidth = Text.CalcSize(resetText).x * 1.5f;
                var resetRect = new Rect(
                    buttonGroup.width - offset - resetWidth,
                    0,
                    resetWidth,
                    Text.LineHeight
                );

                offset += resetWidth + 5f;

                if (Widgets.ButtonText(resetRect, resetText))
                {
                    Store_IncidentEditor.LoadBackup(storeIncident);

                    if (storeIncident.cost < 1)
                    {
                        storeIncident.cost = 50;
                    }

                    MakeSureSaveExists();
                }
            }

            if (variableIncident
                && storeIncidentVariables.customSettings)
            {
                var settingsText = "TKUtils.Windows.IncidentEditor.Settings".Translate();
                var settingsWidth = Text.CalcSize(settingsText).x * 1.5f;
                var settingsRect = new Rect(
                    buttonGroup.width - offset - settingsWidth,
                    0,
                    settingsWidth,
                    Text.LineHeight
                );

                offset += settingsWidth + 5f;

                if (Widgets.ButtonText(settingsRect, settingsText))
                {
                    storeIncidentVariables.settings.EditSettings();
                }
            }

            var titleRect = new Rect(0f, buttonGroup.y, buttonGroup.width - offset, buttonGroup.height);
            Widgets.Label(titleRect, storeIncident.LabelCap);

            listing.GapLine(Text.LineHeight * 3);

            if (storeIncident.cost > 0)
            {
                var (abbrLabel, abbrField) = listing.GetRect(Text.LineHeight).ToForm();

                Widgets.Label(abbrLabel, "TKUtils.Windows.IncidentEditor.Code".Translate());
                storeIncident.abbreviation = Widgets.TextField(abbrField, storeIncident.abbreviation);

                if (!(storeIncident.defName.Equals("AddTrait")
                      || storeIncident.defName.Equals("RemoveTrait")
                      || storeIncident.defName.Equals("BuyPawn")))
                {
                    var (costLabel, costField) = listing.GetRect(Text.LineHeight).ToForm();
                    var costBuffer = storeIncident.cost.ToString();

                    listing.Gap();
                    Widgets.Label(costLabel, "TKUtils.Windows.IncidentEditor.Cost".Translate());
                    Widgets.TextFieldNumeric(costField, ref storeIncident.cost, ref costBuffer, 1f, 100000f);
                }

                var (timesLabel, timesField) = listing.GetRect(Text.LineHeight).ToForm();
                var timesBuffer = storeIncident.eventCap.ToString();

                listing.Gap();
                Widgets.Label(
                    timesLabel,
                    "TKUtils.Windows.IncidentEditor.Times".Translate(ToolkitSettings.EventCooldownInterval)
                );
                Widgets.TextFieldNumeric(timesField, ref storeIncident.eventCap, ref timesBuffer, max: 15f);

                if (variableIncident && storeIncidentVariables.maxWager > 0)
                {
                    listing.Gap();

                    var (wagerLabel, wagerField) = listing.GetRect(Text.LineHeight).ToForm();
                    var wagerBuffer = storeIncidentVariables.maxWager.ToString();

                    Widgets.Label(wagerLabel, "TKUtils.Windows.IncidentEditor.Wager".Translate());
                    Widgets.TextFieldNumeric(
                        wagerField,
                        ref storeIncidentVariables.maxWager,
                        ref wagerBuffer,
                        storeIncidentVariables.cost,
                        20000f
                    );

                    if (storeIncidentVariables.maxWager < storeIncidentVariables.cost)
                    {
                        storeIncidentVariables.maxWager = storeIncidentVariables.cost * 2;
                    }
                }

                listing.Gap();
                var (karmaLabel, karmaField) = listing.GetRect(Text.LineHeight).ToForm();
                var karmaType = storeIncident.karmaType.ToString();

                Widgets.Label(karmaLabel, "TKUtils.Windows.IncidentEditor.Karma".Translate());

                if (Widgets.ButtonText(karmaField, karmaType))
                {
                    Find.WindowStack.Add(new FloatMenu(KarmaTypeOptions));
                }
            }

            listing.Gap();

            if (storeIncident.defName.Equals("Item"))
            {
                var (timesLabel, timesField) = listing.GetRect(Text.LineHeight).ToForm();
                var timesBuffer = storeIncident.eventCap.ToString();

                listing.Gap(6f);
                Widgets.Label(
                    timesLabel,
                    "TKUtils.Windows.IncidentEditor.Times".Translate(ToolkitSettings.EventCooldownInterval)
                );
                Widgets.TextFieldNumeric(timesField, ref storeIncident.eventCap, ref timesBuffer, max: 15f);

                listing.Gap();

                var current = listing.GetRect(Text.LineHeight);
                var itemText = "TKUtils.Windows.IncidentEditor.Items".Translate();
                var itemWidth = Text.CalcSize(itemText).x * 1.5f;
                var itemRect = new Rect(current.x + current.width - itemWidth, current.y, itemWidth, current.height);

                if (Widgets.ButtonText(itemRect, itemText))
                {
                    Find.WindowStack.TryRemove(typeof(StoreItemsWindow));
                    Find.WindowStack.Add(new StoreItemsWindow());
                }
            }

            if (storeIncident.defName.Equals("AddTrait") || storeIncident.defName.Equals("RemoveTrait"))
            {
                var current = listing.GetRect(Text.LineHeight);
                var traitText = "TKUtils.Windows.IncidentEditor.Traits".Translate();
                var traitWidth = Text.CalcSize(traitText).x * 1.5f;
                var traitRect = new Rect(current.x + current.width - traitWidth, current.y, traitWidth, current.height);

                if (Widgets.ButtonText(traitRect, traitText))
                {
                    Find.WindowStack.TryRemove(typeof(TraitConfigDialog));
                    Find.WindowStack.Add(new TraitConfigDialog());
                }
            }

            if (storeIncident.defName.Equals("BuyPawn"))
            {
                var current = listing.GetRect(Text.LineHeight);
                var raceText = "TKUtils.Windows.IncidentEditor.Races".Translate();
                var raceWidth = Text.CalcSize(raceText).x * 1.5f;
                var raceRect = new Rect(current.x + current.width - raceWidth, current.y, raceWidth, current.height);

                if (Widgets.ButtonText(raceRect, raceText))
                {
                    Find.WindowStack.TryRemove(typeof(RaceConfigDialog));
                    Find.WindowStack.Add(new RaceConfigDialog());
                }
            }

            listing.End();
        }
    }
}