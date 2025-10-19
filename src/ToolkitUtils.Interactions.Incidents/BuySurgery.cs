// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Interactions.Incidents. If not, see <https://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using RimWorld;
using RimWorld.Planet;
using ToolkitCore.Utilities;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using UnityEngine;
using BodyPartDef = Verse.BodyPartDef;
using BodyPartRecord = Verse.BodyPartRecord;
using Current = Verse.Current;
using Find = Verse.Find;
using GenCollection = Verse.GenCollection;
using GenList = Verse.GenList;
using GenText = Verse.GenText;
using IntVec3 = Verse.IntVec3;
using LookTargets = Verse.LookTargets;
using Map = Verse.Map;
using Pawn = Verse.Pawn;
using Pawn_HealthTracker = Verse.Pawn_HealthTracker;
using RecipeDef = Verse.RecipeDef;
using Thing = Verse.Thing;
using ThingDef = Verse.ThingDef;
using ThingMaker = Verse.ThingMaker;
using Translator = Verse.Translator;
using TranslatorFormattedStringExtensions = Verse.TranslatorFormattedStringExtensions;

namespace ToolkitUtils.Interactions.Incidents;

[Group("buy")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class SurgeryBuyGroup(ExecutionContext context, TranslationService service)
{
    [Command("surgery")]
    public async Task<Result> PurchaseSurgeryAsync(ItemProduct part, int amount = 1) => await PurchaseSurgeryInternalAsync(part, bodyPart: null, amount);

    [Command("surgery")]
    public async Task<Result> PurchaseSurgeryAsync(ItemProduct part, BodyPartDef? bodyPart = null) => await PurchaseSurgeryInternalAsync(part, bodyPart, amount: 1);

    private async Task<Result> PurchaseSurgeryInternalAsync(ItemProduct part, BodyPartDef bodyPart, int amount = 1)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Ok(service.FormatPawnRequiredSelf());
    }

    private async Task<Result<RecipeDef>> GetSurgeryAsync(ItemProduct part) {}
}

public class BuySurgery : IncidentVariablesBase
{
    private Appointment _appointment;
    private Map _map;

    public override bool CanHappen(string msg, Viewer viewer)
    {
        string[] segments = CommandFilter.Parse(msg).Skip(2).ToArray();
        string partQuery = GenCollection.FirstOrFallback(segments);

        _appointment = Appointment.ParseInput(pawn, segments);

        if (_appointment.ThingDef == null || _appointment.Item == null)
        {
            MessageHelper.ReplyToUser(viewer.username, TranslatorFormattedStringExtensions.Translate("TKUtils.InvalidItemQuery", partQuery));

            return false;
        }

        if (_appointment.ThingDef.IsMedicine || _appointment.Surgery == null)
        {
            MessageHelper.ReplyToUser(viewer.username, TranslatorFormattedStringExtensions.Translate("TKUtils.Surgery.HasNoSurgery", partQuery));

            return false;
        }

        if (BuyItemSettings.mustResearchFirst && _appointment.ThingDef.GetUnfinishedPrerequisites() is {} projects && projects.Count > 0)
        {
            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.ResearchRequired".Translate(_appointment.ThingDef.LabelCap.RawText, projects.Select(p => p.LabelCap.RawText).SectionJoin())
            );

            return false;
        }

        if (!viewer.CanAfford(_appointment.Cost))
        {
            MessageHelper.ReplyToUser(
                viewer.username,
                TranslatorFormattedStringExtensions.Translate("TKUtils.InsufficientBalance", _appointment.Cost.ToString("N0"), viewer.GetViewerCoins().ToString("N0"))
            );

            return false;
        }

        if (_appointment.Overflowed)
        {
            MessageHelper.ReplyToUser(viewer.username, Translator.TranslateSimple("TKUtils.Overflowed"));

            return false;
        }

        if (GenList.NullOrEmpty(_appointment.BodyParts))
        {
            MessageHelper.ReplyToUser(viewer.username, Translator.TranslateSimple("TKUtils.Surgery.NoSlotAvailable"));

            return false;
        }

        _map = Current.Game.AnyPlayerHomeMap;

        if (_map != null) return true;

        MessageHelper.ReplyToUser(viewer.username, Translator.TranslateSimple("TKUtils.NoMap"));

        return false;
    }

    public override void Execute()
    {
        if (_map == null || _appointment == null || Viewer == null) return;

        Thing thing = ThingMaker.MakeThing(_appointment.ThingDef);
        IntVec3 spot = DropCellFinder.TradeDropSpot(_map);

        if (_appointment.ThingDef.Minifiable)
        {
            ThingDef minifiedDef = _appointment.ThingDef.minifiedDef;
            var minifiedThing = (MinifiedThing)ThingMaker.MakeThing(minifiedDef);
            minifiedThing.InnerThing = thing;
            minifiedThing.stackCount = _appointment.Quantity;
            PurchaseHelper.SpawnItem(spot, _map, minifiedThing);
        }
        else
        {
            thing.stackCount = _appointment.Quantity;
            PurchaseHelper.SpawnItem(spot, _map, thing);
        }

        _appointment.BookSurgeries();
        Viewer.Charge(_appointment.Cost, _appointment.ItemData?.Weight ?? 1f, _appointment.ItemData?.KarmaType ?? storeIncident.karmaType);

        MessageHelper.SendConfirmation(Viewer.username, TranslatorFormattedStringExtensions.Translate("TKUtils.Surgery.Complete", _appointment.ThingDef.LabelCap));

        Find.LetterStack.ReceiveLetter(
            Translator.TranslateSimple("TKUtils.SurgeryLetter.Title"),
            TranslatorFormattedStringExtensions.Translate("TKUtils.SurgeryLetter.Description", Viewer.username, Find.ActiveLanguageWorker.WithDefiniteArticle(thing.LabelCap)),
            LetterDefOf.NeutralEvent,
            new LookTargets(thing)
        );
    }

    private sealed class Appointment
    {
        private Pawn Patient { get; set; }
        public RecipeDef Surgery { get; private set; }
        public List<BodyPartRecord> BodyParts { get; private set; }
        public ThingItem Item { get; private set; }
        [CanBeNull] public ItemData ItemData => Item.ItemData;
        public ThingDef ThingDef { get; private set; }
        public int Quantity { get; private set; }

        public int Cost
        {
            get
            {
                if (!Overflowed && PurchaseHelper.TryMultiply(Item.Cost, Quantity, out int result)) return result;

                Overflowed = true;

                return int.MaxValue;
            }
        }

        public bool Overflowed { get; private set; }

        public static Appointment ParseInput(Pawn patient, string[] segments)
        {
            var appointment = new Appointment
            {
                Patient = patient,
            };
            appointment.ParseThingDef(GenCollection.FirstOrFallback(segments));

            string multi = GenCollection.FirstOrFallback(segments.Skip(1));

            if (GenText.NullOrEmpty(multi)) multi = "1";

            if (int.TryParse(multi, out int quantity))
            {
                appointment.Quantity = multi.Equals("*") ? 100 : Mathf.Clamp(quantity, min: 1, max: 100);
                appointment.LocateSurgery();
                appointment.TryFillQuota();
                appointment.Quantity = appointment.BodyParts.Count;
            }
            else
            {
                appointment.ParseBodyPart(multi);
                appointment.LocateSurgery();
                appointment.Quantity = 1;
            }

            return appointment;
        }

        private void ParseThingDef(string input)
        {
            Item = PositionData.Data.Items.Where(i => i.Cost > 0)
                               .FirstOrDefault(t => t.Name.ToToolkit().EqualsIgnoreCase(input.ToToolkit()) || t.DefName!.ToToolkit().EqualsIgnoreCase(input.ToToolkit()));

            ThingDef = Item?.Thing;
        }

        private void ParseBodyPart(string input)
        {
            BodyParts = new List<BodyPartRecord>();

            BodyPartRecord record = GenCollection.FirstOrDefault(
                Patient.RaceProps.body.AllParts,
                t => GenText.EqualsIgnoreCase(t.Label.ToToolkit(), input.ToToolkit()) || GenText.EqualsIgnoreCase(t.def.defName.ToToolkit(), input.ToToolkit())
            );

            if (record == null) return;

            BodyParts.Add(record);
        }

        private void LocateSurgery()
        {
            Surgery = PositionData.Data.Surgeries.Where(r => r.Surgery.IsIngredient(ThingDef)).Where(r => r.CanScheduleFor(Patient))
                                  .Where(r => r.Surgery.Worker.GetPartsToApplyOn(Patient, r.Surgery).Any(p => BodyParts?.Any(b => b.def.defName.Equals(p.def.defName)) ?? true))
                                  .Select(s => s.Surgery).FirstOrFallback();
        }

        private void TryFillQuota()
        {
            BodyParts = new List<BodyPartRecord>();

            if (Surgery?.Worker == null || Surgery.addsHediff == null) return;

            Pawn_HealthTracker pHealth = Patient.health;

            foreach (BodyPartRecord part in Surgery.Worker.GetPartsToApplyOn(Patient, Surgery))
            {
                if (GenCollection.Any(pHealth.surgeryBills.Bills, predicate: b => b is Bill_Medical bill && bill.Part == part && bill.recipe == Surgery)) continue;

                if (pHealth.hediffSet.hediffs.Where(h => h.def.defName == Surgery.addsHediff.defName).Any(h => h.Part == part)) continue;

                BodyParts.Add(part);
            }

            BodyParts.SortBy(part => HealHelper.GetAverageHealthOfPart(Patient, part));
            BodyParts = BodyParts.Take(Quantity).ToList();
        }

        private IEnumerable<Bill_Medical> GenerateSurgeries()
        {
            return BodyParts.Select(p => new Bill_Medical(Surgery, uniqueIngredients: null)
                {
                    Part = p,
                }
            );
        }

        public void BookSurgeries()
        {
            foreach (Bill_Medical surgery in GenerateSurgeries()) Patient.health.surgeryBills.AddBill(surgery);
        }
    }
}
