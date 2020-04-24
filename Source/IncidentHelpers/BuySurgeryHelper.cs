using System.Collections.Generic;
using System.Linq;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
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

            var viewerPawn = CommandBase.GetOrFindPawn(viewer.username);

            if (viewerPawn == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.NoPawn".Translate());
                return false;
            }

            var buyable = DefDatabase<ThingDef>.AllDefsListForReading.FirstOrDefault(
                t => t.defName.ToToolkit().EqualsIgnoreCase(partQuery.ToToolkit())
                     || t.LabelCap.RawText.ToToolkit().EqualsIgnoreCase(partQuery.ToToolkit())
            );

            if (buyable == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.InvalidQuery".Translate(partQuery)
                );
                return false;
            }

            var researched = true;
            var projects = new List<ResearchProjectDef>();

            if (!buyable.recipeMaker?.researchPrerequisite?.IsFinished ?? false)
            {
                projects.Add(buyable.recipeMaker.researchPrerequisite);
                researched = false;
            }

            if (buyable.recipeMaker?.researchPrerequisites?.All(p => !p.IsFinished) ?? false)
            {
                projects = buyable.recipeMaker.researchPrerequisites;
                researched = false;
            }

            if (!buyable.IsResearchFinished)
            {
                projects = buyable.researchPrerequisites;
                researched = false;
            }

            if (BuyItemSettings.mustResearchFirst && !researched)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.Buy.MissingResearch".Translate(
                        buyable.LabelCap.RawText,
                        string.Join(", ", projects.Select(p => p.LabelCap).ToArray())
                    )
                );
                return false;
            }

            if (buyable.category != ThingCategory.Item)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.OnlySurgeries".Translate(partQuery)
                );
                return false;
            }

            if (buyable.IsMedicine)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.OnlySurgeries".Translate(partQuery)
                );
                return false;
            }

            var price = StoreInventory.items.FirstOrDefault(i => i.defname.EqualsIgnoreCase(buyable.defName));

            if (price == null)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.InvalidQuery".Translate(partQuery)
                );
                return false;
            }

            if (price.price < 0)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.BuySurgery.DisabledItem".Translate(partQuery)
                );
                return false;
            }

            var recipes = DefDatabase<RecipeDef>.AllDefsListForReading.Where(r => r.IsSurgery).ToList();
            var partRecipes = recipes
                .Where(r => r.Worker is Recipe_Surgery && r.IsIngredient(buyable))
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
            );

            if (!(surgery?.Worker is Recipe_Surgery worker))
            {
                return false;
            }

            var surgeryParts = worker.GetPartsToApplyOn(viewerPawn, surgery).ToList();
            BodyPartRecord shouldAdd = null;
            var lastHealth = 99999f;

            foreach (var applied in surgeryParts)
            {
                if (viewerPawn.health.surgeryBills.Bills.Count > 0
                    && viewerPawn.health.surgeryBills.Bills.Any(
                        b => b is Bill_Medical bill && bill.Part == applied
                    ))
                {
                    continue;
                }

                var partHealth = HealHelper.GetAverageHealthOfPart(viewerPawn, applied);

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

            var tMap = Current.Game.AnyPlayerHomeMap;

            if (tMap == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.NoMap".Translate());
                return false;
            }

            map = tMap;
            product = price;
            part = buyable;
            pawn = viewerPawn;
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
