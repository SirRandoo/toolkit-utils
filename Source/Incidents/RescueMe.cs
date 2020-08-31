using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;
using Verse.Grammar;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class RescueMe : IncidentHelperVariables
    {
        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Pawn pawn = CommandBase.GetOrFindPawn(viewer.username, true);

            if (pawn?.IsKidnapped() ?? false)
            {
                return true;
            }

            TkLogger.Warn("Rescue request received after disconnect! Scanning through kidnapped pawns...");
            try
            {
                pawn = Faction.OfPlayer.kidnapped.KidnappedPawnsListForReading
                   .Where(p => ((NameTriple) p.Name)?.Nick?.EqualsIgnoreCase(viewer.username) ?? false)
                   .RandomElementWithFallback();
            }
            catch (Exception e)
            {
                TkLogger.Error(
                    $"An error was thrown while trying to find {viewer.username}'s pawn in the kidnapped pawn list! Try again later.",
                    e
                );
                return false;
            }

            return pawn != null && !pawn.Dead && pawn.IsKidnapped();
        }

        public override void TryExecute()
        {
            QuestScriptDef scriptDef = DefDatabase<QuestScriptDef>.GetNamed("TKUtilsViewerRescue");
            float threatPoints = StorytellerUtility.DefaultSiteThreatPointsNow();

            ViewerRescue.QueuedViewers.Enqueue(Viewer.username);
            QuestUtility.SendLetterQuestAvailable(QuestUtility.GenerateQuestAndMakeAvailable(scriptDef, threatPoints));
        }
    }

    [UsedImplicitly]
    public class ViewerRescue : SitePartWorker
    {
        internal static readonly ConcurrentQueue<string> QueuedViewers = new ConcurrentQueue<string>();

        public override void PostDestroy(SitePart sitePart)
        {
            (sitePart.things.FirstOrFallback() as Pawn)?.mindState.JoinColonyBecauseRescuedBy(
                sitePart.site.Map.PlayerPawnsForStoryteller.RandomElementWithFallback()
            );
        }

        public override void Notify_GeneratedByQuestGen(
            SitePart part,
            Slate slate,
            List<Rule> outExtraDescriptionRules,
            Dictionary<string, string> outExtraDescriptionConstants
        )
        {
            base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);

            if (!QueuedViewers.TryDequeue(out string nextViewer))
            {
                nextViewer = "";
            }

            Pawn pawn = CommandBase.GetOrFindPawn(nextViewer, true);
            pawn ??= Faction.OfPlayer.kidnapped.KidnappedPawnsListForReading.FirstOrFallback(
                p => ((NameTriple) p.Name)?.Nick.EqualsIgnoreCase(nextViewer) ?? false
            );
            pawn ??= Faction.OfPlayer.kidnapped.KidnappedPawnsListForReading.RandomElementWithFallback();

            if (pawn != null)
            {
                pawn.SetFaction(part.site.Faction);
                pawn.guest.SetGuestStatus(part.site.Faction, true);
                pawn.mindState.WillJoinColonyIfRescued = true;
                PawnApparelGenerator.GenerateStartingApparelFor(
                    pawn,
                    new PawnGenerationRequest(
                        pawn.kindDef,
                        pawn.Faction,
                        PawnGenerationContext.NonPlayer,
                        part.site.Tile,
                        forceAddFreeWarmLayerIfNeeded: true
                    )
                );
            }

            part.things = new ThingOwner<Pawn>(part, true, LookMode.Reference);
            part.things.TryAdd(pawn);

            PawnRelationUtility.Notify_PawnsSeenByPlayer(
                Gen.YieldSingle(pawn),
                out string pawnRelationsInfo,
                true,
                false
            );

            string output = pawnRelationsInfo.NullOrEmpty()
                ? ""
                : $"\n\n{"PawnHasRelationshipsWithColonists".Localize(pawn?.LabelShort, pawn)}\n\n{pawnRelationsInfo}";
            slate.Set("prisoner", pawn);

            outExtraDescriptionRules.Add(new Rule_String("prisonerFullRelationInfo", output));
        }

        public override string GetPostProcessedThreatLabel(Site site, SitePart sitePart)
        {
            string str = base.GetPostProcessedThreatLabel(site, sitePart);
            if (sitePart.things != null && sitePart.things.Any)
            {
                str = str + ": " + sitePart.things[0].LabelShortCap;
            }

            if (site.HasWorldObjectTimeout)
            {
                str +=
                    $" ({"DurationLeft".Localize((NamedArgument) site.WorldObjectTimeoutTicksLeft.ToStringTicksToPeriod())})";
            }

            return str;
        }
    }
}
