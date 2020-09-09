using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Store;
using Verse;
using Verse.Grammar;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class RescueMe : IncidentHelperVariables
    {
        private KidnapReport report;
        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            Viewer = viewer;
            Pawn pawn = CommandBase.GetOrFindPawn(viewer.username, true);

            if (pawn?.IsKidnapped() ?? false)
            {
                report = new KidnapReport {Viewer = viewer.username, PawnIds = new List<string> {pawn.ThingID}};
                return true;
            }

            try
            {
                report = KidnapReport.KidnapReportFor(viewer.username);
            }
            catch (Exception e)
            {
                TkLogger.Error(
                    $"An error was thrown while trying to find {viewer.username}'s pawn in the kidnapped pawn list! Try again later.",
                    e
                );
                return false;
            }

            return !report?.PawnIds.NullOrEmpty() ?? false;
        }

        public override void TryExecute()
        {
            QuestScriptDef scriptDef = DefDatabase<QuestScriptDef>.GetNamed("TKUtilsViewerRescue");
            float threatPoints = StorytellerUtility.DefaultSiteThreatPointsNow();

            var component = Current.Game.GetComponent<GameComponentPawns>();

            if (component != null && component.pawnHistory.ContainsKey(Viewer.username.ToLower()))
            {
                component.pawnHistory.Remove(Viewer.username.ToLower());
            }

            ViewerRescue.QueuedViewers.Enqueue(report);
            QuestUtility.SendLetterQuestAvailable(QuestUtility.GenerateQuestAndMakeAvailable(scriptDef, threatPoints));
        }
    }

    [UsedImplicitly]
    public class ViewerRescue : SitePartWorker
    {
        internal static readonly ConcurrentQueue<KidnapReport> QueuedViewers = new ConcurrentQueue<KidnapReport>();

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

            Pawn pawn = null;
            if (!QueuedViewers.TryDequeue(out KidnapReport report))
            {
                report = null;
            }
            else
            {
                pawn = CommandBase.GetOrFindPawn(report.Viewer, true);

                if (!pawn.IsKidnapped())
                {
                    pawn = null;
                }
            }

            pawn ??= report?.GetMostRecentKidnapping();
            pawn ??= report?.GetPawns().RandomElementWithFallback();
            pawn ??= PawnGenerator.GeneratePawn(
                new PawnGenerationRequest(PawnKindDefOf.Colonist, Faction.OfPlayer, forceAddFreeWarmLayerIfNeeded: true)
            );

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
