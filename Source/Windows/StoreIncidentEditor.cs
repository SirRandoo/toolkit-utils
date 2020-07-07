using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class StoreIncidentEditor : TwitchToolkit.Windows.StoreIncidentEditor
    {
        private readonly List<FloatMenuOption> karmaTypeOptions;
        private readonly string[] karmaTypeStrings = Enum.GetNames(typeof(KarmaType));
        private bool ctrl;
        private bool shft;

        public StoreIncidentEditor(StoreIncident storeIncident) : base(storeIncident)
        {
            onlyOneOfTypeAllowed = true;

            karmaTypeOptions = karmaTypeStrings
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

            Rect buttonGroup = listing.GetRect(Text.LineHeight);

            if (storeIncident.cost > 0)
            {
                TaggedString disableText = "TKUtils.Windows.IncidentEditor.Disable".Translate();
                float disableWidth = Text.CalcSize(disableText).x * 1.5f;
                var disableRect = new Rect(buttonGroup.width - disableWidth, 0, disableWidth, Text.LineHeight);
                offset += disableWidth + 5f;

                if (Widgets.ButtonText(disableRect, disableText))
                {
                    storeIncident.cost = -10;
                }
            }

            if (!storeIncident.defName.Equals("Item"))
            {
                TaggedString resetText = "TKUtils.Windows.IncidentEditor.Reset".Translate();
                float resetWidth = Text.CalcSize(resetText).x * 1.5f;
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
                TaggedString settingsText = "TKUtils.Windows.IncidentEditor.Settings".Translate();
                float settingsWidth = Text.CalcSize(settingsText).x * 1.5f;
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
                (Rect abbrLabel, Rect abbrField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);

                Widgets.Label(abbrLabel, "TKUtils.Windows.IncidentEditor.Code".Translate());
                storeIncident.abbreviation = Widgets.TextField(abbrField, storeIncident.abbreviation);

                if ((storeIncident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.None) == EventTypes.None)
                {
                    (Rect costLabel, Rect costField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);

                    listing.Gap();
                    Widgets.Label(costLabel, "TKUtils.Windows.IncidentEditor.Cost".Translate());
                    SettingsHelper.DrawPriceField(costField, ref storeIncident.cost, ref ctrl, ref shft);

                    if (storeIncident.cost == 0)
                    {
                        storeIncident.cost = 1;
                    }
                }

                (Rect timesLabel, Rect timesField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
                var timesBuffer = storeIncident.eventCap.ToString();

                listing.Gap();
                Widgets.Label(
                    timesLabel,
                    "TKUtils.Windows.IncidentEditor.Times".Translate(ToolkitSettings.EventCooldownInterval)
                );
                Widgets.TextFieldNumeric(timesField, ref storeIncident.eventCap, ref timesBuffer, max: 60f);

                if (variableIncident && storeIncidentVariables.maxWager > 0)
                {
                    listing.Gap();

                    (Rect wagerLabel, Rect wagerField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
                    Widgets.Label(wagerLabel, "TKUtils.Windows.IncidentEditor.Wager".Translate());
                    SettingsHelper.DrawPriceField(
                        wagerField,
                        ref storeIncidentVariables.maxWager,
                        ref ctrl,
                        ref shft
                    );

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

                Widgets.Label(karmaLabel, "TKUtils.Windows.IncidentEditor.Karma".Translate());

                if (Widgets.ButtonText(karmaField, karmaType))
                {
                    Find.WindowStack.Add(new FloatMenu(karmaTypeOptions));
                }
            }

            listing.Gap();

            if (storeIncident.GetModExtension<EventExtension>()?.EventType == EventTypes.Item)
            {
                (Rect timesLabel, Rect timesField) = listing.GetRect(Text.LineHeight).ToForm(0.6f);
                var timesBuffer = storeIncident.eventCap.ToString();

                listing.Gap(6f);
                Widgets.Label(
                    timesLabel,
                    "TKUtils.Windows.IncidentEditor.Times".Translate(ToolkitSettings.EventCooldownInterval)
                );
                Widgets.TextFieldNumeric(timesField, ref storeIncident.eventCap, ref timesBuffer, max: 15f);

                listing.Gap();

                Rect current = listing.GetRect(Text.LineHeight);
                TaggedString itemText = "TKUtils.Windows.IncidentEditor.Items".Translate();
                float itemWidth = Text.CalcSize(itemText).x * 1.5f;
                var itemRect = new Rect(current.x + current.width - itemWidth, current.y, itemWidth, current.height);

                if (Widgets.ButtonText(itemRect, itemText))
                {
                    Find.WindowStack.Add(new StoreDialog());
                }
            }

            if (storeIncident.GetModExtension<EventExtension>()?.EventType == EventTypes.Trait)
            {
                Rect current = listing.GetRect(Text.LineHeight);
                TaggedString traitText = "TKUtils.Windows.IncidentEditor.Traits".Translate();
                float traitWidth = Text.CalcSize(traitText).x * 1.5f;
                var traitRect = new Rect(current.x + current.width - traitWidth, current.y, traitWidth, current.height);

                if (Widgets.ButtonText(traitRect, traitText))
                {
                    Find.WindowStack.Add(new TraitConfigDialog());
                }
            }

            if (storeIncident.GetModExtension<EventExtension>()?.EventType == EventTypes.PawnKind)
            {
                Rect current = listing.GetRect(Text.LineHeight);
                TaggedString raceText = "TKUtils.Windows.IncidentEditor.PawnKind".Translate();
                float raceWidth = Text.CalcSize(raceText).x * 1.5f;
                var raceRect = new Rect(current.x + current.width - raceWidth, current.y, raceWidth, current.height);

                if (Widgets.ButtonText(raceRect, raceText))
                {
                    Find.WindowStack.Add(new RaceConfigDialog());
                }
            }

            listing.End();
        }
    }
}
