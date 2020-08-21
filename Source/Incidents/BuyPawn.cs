using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class BuyPawn : IncidentHelperVariables
    {
        private PawnKindDef kindDef = PawnKindDefOf.Colonist;
        private IntVec3 loc;
        private Map map;

        private PawnKindItem pawnKindItem;

        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            if (CommandBase.GetOrFindPawn(viewer.username) != null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.HasPawn".Localize());
                return false;
            }

            Viewer = viewer;
            map = Helper.AnyPlayerMap;

            if (map == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.NoMap".Localize());
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

            GetDefaultKind();

            if (!TkSettings.Race)
            {
                return CanPurchaseRace(viewer, pawnKindItem);
            }

            string[] segments = CommandFilter.Parse(message).Skip(2).ToArray();
            string query = segments.FirstOrDefault();

            if (query.NullOrEmpty())
            {
                return CanPurchaseRace(viewer, pawnKindItem);
            }

            PawnKindDef[] raceDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(
                    r => r.race.defName.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                         || r.race.label.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                )
               .ToArray();

            PawnKindDef raceDef = raceDefs.FirstOrDefault(r => r.defaultFactionType.isPlayer)
                                  ?? raceDefs.RandomElementByWeightWithFallback(null);

            if (raceDef == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidKindQuery".Localize(query));
                return false;
            }

            if (!raceDef.RaceProps.Humanlike)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.BuyPawn.Humanlike".Localize());
                return false;
            }

            kindDef = raceDef;
            pawnKindItem = Data.PawnKinds.FirstOrDefault(r => r.DefName.Equals(kindDef.race.defName));

            if (pawnKindItem != null)
            {
                return CanPurchaseRace(viewer, pawnKindItem);
            }

            MessageHelper.ReplyToUser(viewer.username, "TKUtils.InvalidKindQuery".Localize(query));
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
                TaggedString title = "TKUtils.PawnLetter.Title".Localize();
                TaggedString text = "TKUtils.PawnLetter.Description".Localize(Viewer.username);
                PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref text, ref title, pawn);

                Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.PositiveEvent, pawn);
                Current.Game.GetComponent<GameComponentPawns>().AssignUserToPawn(Viewer.username, pawn);

                if (!ToolkitSettings.UnlimitedCoins)
                {
                    Viewer.TakeViewerCoins(pawnKindItem!.Cost);
                }

                Viewer.CalculateNewKarma(pawnKindItem!.Data?.KarmaType ?? storeIncident.karmaType, pawnKindItem!.Cost);

                if (ToolkitSettings.PurchaseConfirmations)
                {
                    MessageHelper.ReplyToUser(Viewer.username, "TKUtils.BuyPawn.Confirmation".Localize());
                }
            }
            catch (Exception e)
            {
                TkLogger.Error("Could not execute buy pawn", e);
            }
        }

        private bool CanPurchaseRace(Viewer viewer, PawnKindItem target)
        {
            if (!target.Enabled && TkSettings.Race)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.InformativeDisabledItem".Localize(target.Name));
                return false;
            }

            if (viewer.CanAfford(target.Cost))
            {
                return true;
            }

            MessageHelper.ReplyToUser(
                viewer.username,
                "TKUtils.InsufficientBalance".Localize(
                    target.Cost.ToString("N0"),
                    Viewer.GetViewerCoins().ToString("N0")
                )
            );
            return false;
        }

        private void GetDefaultKind()
        {
            PawnKindItem human = Data.PawnKinds.FirstOrDefault(
                d => d.DefName.EqualsIgnoreCase(PawnKindDefOf.Colonist.race.defName)
            );

            if (human?.Enabled ?? false)
            {
                kindDef = PawnKindDefOf.Colonist;
                pawnKindItem = human;
                return;
            }

            PawnKindItem randomKind = Data.PawnKinds.FirstOrDefault(k => k.Enabled);

            if (randomKind == null)
            {
                TkLogger.Warn("Could not get next enabled race!");
                return;
            }

            kindDef = DefDatabase<PawnKindDef>.AllDefsListForReading.FirstOrDefault(
                k => k.race.defName.EqualsIgnoreCase(randomKind.DefName)
            );
            pawnKindItem = randomKind;
        }
    }
}
