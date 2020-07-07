using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    [UsedImplicitly]
    public class BuyPawnHelper : IncidentHelperVariables
    {
        private PawnKindDef kindDef = PawnKindDefOf.Colonist;
        private IntVec3 loc;
        private Map map;

        private XmlRace race =
            TkUtils.ShopExpansion.Races.FirstOrDefault(r => r.DefName.Equals(PawnKindDefOf.Colonist.race.defName));

        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (CommandBase.GetOrFindPawn(viewer.username) != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.HasPawn".TranslateSimple());
                return false;
            }

            Viewer = viewer;
            map = Helper.AnyPlayerMap;

            if (map == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.NoMap".TranslateSimple());
                return false;
            }

            if (!CellFinder.TryFindRandomEdgeCellWith(
                p => map.reachability.CanReachColony(p) && !p.Fogged(map),
                map,
                CellFinder.EdgeRoadChance_Neutral,
                out loc
            ))
            {
                TkLogger.Warn("No reachable location to spawn a viewer pawn!");
                return false;
            }

            GetDefaultRace();

            if (!TkSettings.Race)
            {
                return CanPurchaseRace(viewer, race);
            }

            string[] segments = CommandFilter.Parse(message).Skip(2).ToArray();
            string query = segments.FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return CanPurchaseRace(viewer, race);
            }

            PawnKindDef raceDef = DefDatabase<PawnKindDef>.AllDefsListForReading
                .FirstOrDefault(
                    r => r.race.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                         || r.race.label.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                );

            if (raceDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.NoRace".Translate(query));
                return false;
            }

            if (!raceDef.RaceProps.Humanlike)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.OnlyHuman".TranslateSimple());
                return false;
            }

            kindDef = raceDef;
            race = TkUtils.ShopExpansion.Races.FirstOrDefault(r => r.DefName.Equals(kindDef.race.defName));

            if (race != null)
            {
                return CanPurchaseRace(viewer, race);
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.Responses.Buy.NoRace".Translate(query));
            return false;
        }

        public override void TryExecute()
        {
            try
            {
                var request = new PawnGenerationRequest(
                    kindDef,
                    Faction.OfPlayer,
                    tile: map.Tile,
                    allowFood: false,
                    mustBeCapableOfViolence: true
                );
                Pawn pawn = PawnGenerator.GeneratePawn(request);

                if (!(pawn.Name is NameTriple name))
                {
                    TkLogger.Warn("Pawn name is not a name triple!");
                    return;
                }

                GenSpawn.Spawn(pawn, loc, map);
                pawn.Name = new NameTriple(name.First ?? string.Empty, Viewer.username, name.Last ?? string.Empty);
                TaggedString title = "TKUtils.Responses.Buy.Joined.Title".TranslateSimple();
                TaggedString text = "TKUtils.Responses.Buy.Joined.Text".Translate(Viewer.username);
                PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref title, pawn);

                Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.PositiveEvent, pawn);
                Current.Game.GetComponent<GameComponentPawns>().AssignUserToPawn(Viewer.username, pawn);

                if (!ToolkitSettings.UnlimitedCoins)
                {
                    Viewer.TakeViewerCoins(race?.Price ?? storeIncident.cost);
                }

                Viewer.CalculateNewKarma(storeIncident.karmaType, race?.Price ?? storeIncident.cost);

                if (ToolkitSettings.PurchaseConfirmations)
                {
                    MessageHelper.ReplyToUser(
                        Viewer.username,
                        "TKUtils.Responses.Buy.PurchaseMessage".TranslateSimple()
                    );
                }
            }
            catch (Exception e)
            {
                TkLogger.Error("Could not execute buy pawn", e);
            }
        }

        private bool CanPurchaseRace(Viewer viewer, XmlRace target)
        {
            if (!target.Enabled && TkSettings.Race)
            {
                MessageHelper.ReplyToUser(
                    viewer.username,
                    "TKUtils.Responses.DisabledItem.Informative".Translate(target.Name)
                );
                return false;
            }

            if (viewer.CanAfford(target.Price))
            {
                return true;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.Responses.NotEnoughCoins".Translate(
                    target.Price.ToString("N0"),
                    Viewer.GetViewerCoins().ToString("N0")
                )
            );
            return false;
        }

        private void GetDefaultRace()
        {
            XmlRace human = TkUtils.ShopExpansion.Races
                .FirstOrDefault(d => d.DefName.EqualsIgnoreCase(PawnKindDefOf.Colonist.race.defName));

            if (human?.Enabled ?? false)
            {
                kindDef = PawnKindDefOf.Colonist;
                race = human;
                return;
            }

            XmlRace randomKind = TkUtils.ShopExpansion.Races
                .FirstOrDefault(k => k.Enabled);

            if (randomKind == null)
            {
                TkLogger.Warn("Could not get next enabled race!");
                return;
            }

            kindDef = DefDatabase<PawnKindDef>.AllDefsListForReading
                .FirstOrDefault(k => k.race.defName.EqualsIgnoreCase(randomKind.DefName));
            race = randomKind;
        }
    }
}
