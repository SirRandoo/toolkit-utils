using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class BuySurgery : IncidentHelperVariables
    {
        private Appointment appointment;
        private Map map;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            string[] segments = CommandFilter.Parse(message).Skip(2).ToArray();
            string partQuery = segments.FirstOrFallback();

            if (!PurchaseHelper.TryGetPawn(viewer.username, out Pawn pawn))
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            appointment = Appointment.ParseInput(pawn, segments);

            if (appointment.ThingDef == null || appointment.Item == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidItemQuery".Localize(partQuery));
                return false;
            }

            if (appointment.ThingDef.IsMedicine || appointment.Surgery == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Surgery.HasNoSurgery".Localize(partQuery));
                return false;
            }

            if (appointment.ThingDef.GetUnfinishedPrerequisites() is { } projects && projects.Count > 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ResearchRequired".Localize(
                        appointment.ThingDef.LabelCap.RawText,
                        projects.Select(p => p.LabelCap.RawText).SectionJoin()
                    )
                );
                return false;
            }

            if (!viewer.CanAfford(appointment.Item.Price))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        appointment.Item.Price.ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            if (appointment.BodyParts.NullOrEmpty())
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Surgery.NoSlotAvailable".Localize());
                return false;
            }

            map = Current.Game.AnyPlayerHomeMap;

            if (map != null)
            {
                return true;
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoMap".Localize());
            return false;
        }

        public override void TryExecute()
        {
            if (map == null || appointment == null || Viewer == null)
            {
                return;
            }

            Thing thing = ThingMaker.MakeThing(appointment.ThingDef);
            IntVec3 spot = DropCellFinder.TradeDropSpot(map);

            if (appointment.ThingDef.Minifiable)
            {
                ThingDef minifiedDef = appointment.ThingDef.minifiedDef;
                var minifiedThing = (MinifiedThing) ThingMaker.MakeThing(minifiedDef);
                minifiedThing.InnerThing = thing;
                minifiedThing.stackCount = appointment.Quantity;
                TradeUtility.SpawnDropPod(spot, map, minifiedThing);
            }
            else
            {
                thing.stackCount = appointment.Quantity;
                TradeUtility.SpawnDropPod(spot, map, thing);
            }

            appointment.BookSurgeries();

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(appointment.Item.Price * appointment.Quantity);
            }

            Viewer.CalculateNewKarma(
                appointment.Item.Data?.KarmaType ?? storeIncident.karmaType,
                Mathf.CeilToInt(appointment.Item.Price * appointment.Quantity * (appointment.Item.Data?.Weight ?? 1f))
            );

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.Surgery.Complete".Localize(appointment.ThingDef.LabelCap)
                );
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.SurgeryLetter.Title".Localize(),
                "TKUtils.SurgeryLetter.Description".Localize(
                    Viewer.username,
                    Find.ActiveLanguageWorker.WithDefiniteArticle(thing.LabelCap)
                ),
                LetterDefOf.NeutralEvent,
                new LookTargets(thing)
            );
        }

        private class Appointment
        {
            public Pawn Patient { get; set; }
            public RecipeDef Surgery { get; set; }
            public List<BodyPartRecord> BodyParts { get; set; }
            public ThingItem Item { get; set; }
            public ThingDef ThingDef { get; set; }
            public int Quantity { get; set; }

            public static Appointment ParseInput(Pawn patient, string[] segments)
            {
                var appointment = new Appointment {Patient = patient};
                appointment.ParseThingDef(segments.FirstOrFallback());

                string multi = segments.Skip(1).FirstOrFallback();

                if (multi.NullOrEmpty())
                {
                    multi = "1";
                }

                if (int.TryParse(multi, out int quantity))
                {
                    appointment.Quantity = multi.Equals("*") ? 100 : Mathf.Clamp(quantity, 0, 100);
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
                Item = Data.Items.Where(i => i.Price > 0)
                   .FirstOrDefault(
                        t => t.Name.ToToolkit().EqualsIgnoreCase(input.ToToolkit())
                             || t.DefName.ToToolkit().EqualsIgnoreCase(input.ToToolkit())
                    );

                ThingDef = Item?.Thing;
            }

            private void ParseBodyPart(string input)
            {
                BodyParts = new List<BodyPartRecord>
                {
                    Patient.RaceProps.body.AllParts.FirstOrDefault(
                        t => t.Label.ToToolkit().EqualsIgnoreCase(input.ToToolkit())
                             || t.def.defName.ToToolkit().EqualsIgnoreCase(input.ToToolkit())
                    )
                };
            }

            private void LocateSurgery()
            {
                Surgery = DefDatabase<RecipeDef>.AllDefs.Where(r => r.IsSurgery)
                   .Where(r => r.Worker != null)
                   .Where(r => r.IsIngredient(ThingDef))
                   .Where(r => r.AvailableOnNow(Patient))
                   .Where(
                        r => r.Worker.GetPartsToApplyOn(Patient, r)
                           .Any(p => BodyParts?.Any(b => b.def.defName.Equals(p.def.defName)) ?? true)
                    )
                   .FirstOrFallback();
            }

            private void TryFillQuota()
            {
                BodyParts = new List<BodyPartRecord>();

                if (Surgery?.Worker == null || Surgery.addsHediff == null)
                {
                    return;
                }

                foreach (BodyPartRecord part in Surgery.Worker.GetPartsToApplyOn(Patient, Surgery))
                {
                    if (Patient.health.surgeryBills.Bills.Any(b => b is Bill_Medical bill && bill.Part == part))
                    {
                        continue;
                    }

                    if (Patient.health.hediffSet.hediffs.Where(h => h.def.defName == Surgery.addsHediff.defName)
                       .Any(h => h.Part == part))
                    {
                        continue;
                    }

                    BodyParts.Add(part);
                }

                BodyParts.SortBy(part => HealHelper.GetAverageHealthOfPart(Patient, part));
                BodyParts = BodyParts.Take(Quantity).ToList();
            }

            private IEnumerable<Bill_Medical> GenerateSurgeries()
            {
                return BodyParts.Select(p => new Bill_Medical(Surgery) {Part = p});
            }

            public void BookSurgeries()
            {
                foreach (Bill_Medical surgery in GenerateSurgeries())
                {
                    Patient.health.surgeryBills.AddBill(surgery);
                }
            }
        }
    }
}
