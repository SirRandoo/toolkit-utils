using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    [UsedImplicitly]
    public class BuySurgeryHelper : IncidentHelperVariables
    {
        private Map map;
        private ThingDef part;
        private Pawn pawn;
        private Item product;
        private RecipeDef surgery;
        private BodyPartRecord toPart;

        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

            string[] segments = CommandFilter.Parse(message).Skip(2).ToArray();
            string partQuery = segments.FirstOrDefault();

            if (partQuery.NullOrEmpty())
            {
                return false;
            }

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoPawn".Localize());
                return false;
            }

            Appointment appointment = Appointment.ParseInput(pawn, segments);
            part = appointment.ThingDef;

            if (part == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InvalidItemQuery".Localize(partQuery)
                );
                return false;
            }

            List<ResearchProjectDef> projects = part.GetUnfinishedPrerequisites();

            if (projects.Count > 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.ResearchRequired".Localize(
                        part.LabelCap.RawText,
                        projects.Select(p => p.LabelCap.RawText).SectionJoin()
                    )
                );
                return false;
            }

            if (part.category != ThingCategory.Item
                || Androids.Active && !part.thingCategories.Any(c => c.defName.EqualsIgnoreCase("BodyPartsAndroid")))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Surgery.HasNoSurgery".Localize(partQuery)
                );
                return false;
            }

            if (part.IsMedicine)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Surgery.HasNoSurgery".Localize(partQuery)
                );
                return false;
            }

            product = StoreInventory.items.FirstOrDefault(i => i.defname.EqualsIgnoreCase(part.defName));

            if (product == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InvalidItemQuery".Localize(partQuery)
                );
                return false;
            }

            if (product.price < 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Surgery.ItemDisabled".Localize(partQuery)
                );
                return false;
            }

            if (!viewer.CanAfford(product.price))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.InsufficientBalance".Localize(
                        product.price.ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            List<RecipeDef> recipes = DefDatabase<RecipeDef>.AllDefsListForReading.Where(r => r.IsSurgery).ToList();
            List<RecipeDef> partRecipes = recipes
                .Where(r => (r.Worker is Recipe_Surgery || Androids.IsSurgeryUsable(pawn, r)) && r.IsIngredient(part))
                .ToList();

            if (!partRecipes.Any())
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Surgery.HasNoSurgery".Localize(partQuery)
                );
                return false;
            }

            surgery = partRecipes.FirstOrDefault(r => r.AvailableOnNow(pawn));

            if (surgery == null)
            {
                return false;
            }

            List<BodyPartRecord> surgeryParts =
                surgery.Worker?.GetPartsToApplyOn(pawn, surgery).ToList() ?? new List<BodyPartRecord>();
            BodyPartRecord shouldAdd = appointment.BodyPart;
            var lastHealth = 99999f;

            if (shouldAdd == null)
                foreach (BodyPartRecord applied in surgeryParts)
                {
                    if (pawn.health.surgeryBills.Bills.Count > 0
                        && pawn.health.surgeryBills.Bills.Any(
                            b => b is Bill_Medical bill && bill.Part == applied
                        ))
                    {
                        continue;
                    }

                    float partHealth = HealHelper.GetAverageHealthOfPart(pawn, applied);

                    if (partHealth > lastHealth)
                    {
                        continue;
                    }

                    shouldAdd = applied;
                    lastHealth = partHealth;
                }

            if (shouldAdd == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Surgery.NoSlotAvailable".Localize()
                );
                return false;
            }

            if (!surgery.AvailableOnNow(pawn))
            {
                return false;
            }

            map = Current.Game.AnyPlayerHomeMap;

            if (map == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoMap".Localize());
                return false;
            }

            toPart = shouldAdd;

            return true;
        }

        public override void TryExecute()
        {
            if (product == null || part == null || map == null || surgery == null)
            {
                return;
            }

            Thing thing = ThingMaker.MakeThing(part);
            IntVec3 spot = DropCellFinder.TradeDropSpot(map);
            var bill = new Bill_Medical(surgery);

            TradeUtility.SpawnDropPod(spot, map, thing);

            pawn.health.surgeryBills.AddBill(bill);
            bill.Part = toPart;

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(product.price);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, product.price);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.Surgery.Complete".Localize(part.LabelCap)
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
            public RecipeDef Recipe { get; set; }
            public BodyPartRecord BodyPart { get; set; }
            public Item Item { get; set; }
            public ThingDef ThingDef { get; set; }
            public int Quantity { get; set; }

            public static Appointment ParseInput(Pawn patient, string[] segments)
            {
                var appointment = new Appointment
                {
                    Patient = patient, ThingDef = ParseThingDef(segments.FirstOrFallback())
                };

                string multi = segments.Skip(1).FirstOrFallback();
                if (int.TryParse(multi, out int quantity))
                {
                    appointment.Quantity = Mathf.Clamp(quantity, 0, 100);
                }
                else
                {
                    appointment.BodyPart = ParseBodyPart(patient, multi);
                    appointment.Quantity = 1;
                }

                return appointment;
            }

            private static ThingDef ParseThingDef(string input)
            {
                return DefDatabase<ThingDef>.AllDefsListForReading
                    .FirstOrDefault(
                        t => t.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(input.ToToolkit())
                             || t.defName.ToToolkit().EqualsIgnoreCase(input.ToToolkit())
                    );
            }

            private static BodyPartRecord ParseBodyPart(Pawn patient, string input)
            {
                return patient.RaceProps.body.AllParts
                    .FirstOrDefault(
                        t => t.Label.ToToolkit().EqualsIgnoreCase(input.ToToolkit())
                             || t.def.defName.ToToolkit().EqualsIgnoreCase(input.ToToolkit())
                    );
            }
        }
    }
}
