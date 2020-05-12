using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.IncidentHelpers.IncidentHelper_Settings;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    public class BuySurgeryHelper : IncidentHelperVariables
    {
        private Map map;
        private ThingDef part;
        private Pawn pawn;
        private Item product;
        private RecipeDef surgery;
        private BodyPartRecord toPart;

        public override Viewer Viewer { get; set; }

        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (viewer == null)
            {
                return false;
            }

            Viewer = viewer;

            var partQuery = CommandFilter.Parse(message).Skip(2).FirstOrDefault();

            if (partQuery.NullOrEmpty())
            {
                return false;
            }

            pawn = CommandBase.GetOrFindPawn(viewer.username);

            if (pawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            part = DefDatabase<ThingDef>.AllDefsListForReading.FirstOrDefault(
                t => t.defName.ToToolkit().EqualsIgnoreCase(partQuery.ToToolkit())
                     || t.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(partQuery.ToToolkit())
            );

            if (part == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.InvalidQuery".Translate(partQuery)
                );
                return false;
            }

            var researched = true;
            var projects = new List<ResearchProjectDef>();

            if (!part.recipeMaker?.researchPrerequisite?.IsFinished ?? false)
            {
                projects.Add(part.recipeMaker.researchPrerequisite);
                researched = false;
            }

            if (part.recipeMaker?.researchPrerequisites?.All(p => !p.IsFinished) ?? false)
            {
                projects = part.recipeMaker.researchPrerequisites;
                researched = false;
            }

            if (!part.IsResearchFinished)
            {
                projects = part.researchPrerequisites;
                researched = false;
            }

            if (BuyItemSettings.mustResearchFirst && !researched)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.Buy.MissingResearch".Translate(
                        part.LabelCap.RawText,
                        string.Join(", ", projects.Select(p => p.LabelCap).ToArray())
                    )
                );
                return false;
            }

            if (part.category != ThingCategory.Item
                || (Androids.Active && !part.thingCategories.Any(c => c.defName.EqualsIgnoreCase("BodyPartsAndroid"))))
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.OnlySurgeries".Translate(partQuery)
                );
                return false;
            }

            if (part.IsMedicine)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.OnlySurgeries".Translate(partQuery)
                );
                return false;
            }

            product = StoreInventory.items.FirstOrDefault(i => i.defname.EqualsIgnoreCase(part.defName));

            if (product == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.InvalidQuery".Translate(partQuery)
                );
                return false;
            }

            if (product.price < 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.DisabledItem".Translate(partQuery)
                );
                return false;
            }

            if (viewer.GetViewerCoins() < product.price + storeIncident.cost)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.NotEnoughCoins".Translate(
                        (product.price + storeIncident.cost).ToString("N0"),
                        viewer.GetViewerCoins().ToString("N0")
                    )
                );
                return false;
            }

            var recipes = DefDatabase<RecipeDef>.AllDefsListForReading.Where(r => r.IsSurgery).ToList();
            var partRecipes = recipes
                .Where(r => (r.Worker is Recipe_Surgery || Androids.IsSurgeryUsable(pawn, r)) && r.IsIngredient(part))
                .ToList();

            if (!partRecipes.Any())
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.OnlySurgeries".Translate(partQuery)
                );
                return false;
            }

            surgery = partRecipes.FirstOrDefault(
                r => r.Worker is Recipe_InstallImplant
                     || r.Worker is Recipe_InstallNaturalBodyPart
                     || r.Worker is Recipe_InstallArtificialBodyPart
                     || Androids.IsSurgeryUsable(pawn, r)
            );

            if (surgery == null)
            {
                return false;
            }

            var surgeryParts = surgery.Worker?.GetPartsToApplyOn(pawn, surgery).ToList() ?? new List<BodyPartRecord>();
            BodyPartRecord shouldAdd = null;
            var lastHealth = 99999f;

            foreach (var applied in surgeryParts)
            {
                if (pawn.health.surgeryBills.Bills.Count > 0
                    && pawn.health.surgeryBills.Bills.Any(
                        b => b is Bill_Medical bill && bill.Part == applied
                    ))
                {
                    continue;
                }

                var partHealth = HealHelper.GetAverageHealthOfPart(pawn, applied);

                if (partHealth > lastHealth)
                {
                    continue;
                }

                shouldAdd = applied;
                lastHealth = partHealth;
            }

            if (shouldAdd == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.BuySurgery.NoAvailableSlots".Translate());
                return false;
            }

            map = Current.Game.AnyPlayerHomeMap;

            if (map == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.NoMap".Translate());
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

            var thing = ThingMaker.MakeThing(part);
            var spot = DropCellFinder.TradeDropSpot(map);
            var bill = new Bill_Medical(surgery);

            TradeUtility.SpawnDropPod(spot, map, thing);

            pawn.health.surgeryBills.AddBill(bill);
            bill.Part = toPart;

            if (!ToolkitSettings.UnlimitedCoins)
            {
                Viewer.TakeViewerCoins(storeIncident.cost + product.price);
            }

            Viewer.CalculateNewKarma(storeIncident.karmaType, storeIncident.cost + product.price);

            if (ToolkitSettings.PurchaseConfirmations)
            {
                MessageHelper.ReplyToUser(
                    Viewer.username,
                    "TKUtils.Responses.BuySurgery.Queued".Translate(part.LabelCap)
                );
            }

            Find.LetterStack.ReceiveLetter(
                "TKUtils.Letters.BuySurgery.Title".Translate(),
                "TKUtils.Letters.BuySurgery.Description".Translate(
                    Viewer.username,
                    Find.ActiveLanguageWorker.WithDefiniteArticle(thing.LabelCap)
                ),
                LetterDefOf.NeutralEvent,
                new LookTargets(thing)
            );
        }
    }
}
