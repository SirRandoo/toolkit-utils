// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;
using Verse.Grammar;

namespace SirRandoo.ToolkitUtils.Incidents
{
    [UsedImplicitly]
    public class RescueMe : IncidentVariablesBase
    {
        private KidnapReport report;

        public override bool CanHappen(string msg, [NotNull] Viewer viewer)
        {
            Pawn pawn = CommandBase.GetOrFindPawn(viewer.username, true);
            bool? isKidnapped = pawn?.IsKidnapped();

            if (!isKidnapped ?? false)
            {
                return false;
            }

            if (pawn?.IsBorrowedByAnyFaction() == true)
            {
                return false;
            }

            if (isKidnapped ?? false)
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
                LogHelper.Error(
                    $"An error was thrown while trying to find {viewer.username}'s pawn in the kidnapped pawn list! Try again later.",
                    e
                );
                return false;
            }

            return !report?.PawnIds.NullOrEmpty() ?? false;
        }

        public override void Execute()
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
            Viewer.Charge(storeIncident);
        }
    }

    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class ViewerRescue : SitePartWorker
    {
        internal static readonly ConcurrentQueue<KidnapReport> QueuedViewers = new ConcurrentQueue<KidnapReport>();

        public override void PostDestroy([NotNull] SitePart sitePart)
        {
            (sitePart.things.FirstOrFallback() as Pawn)?.mindState.JoinColonyBecauseRescuedBy(
                sitePart.site.Map.PlayerPawnsForStoryteller.RandomElementWithFallback()
            );
        }

        public override void Notify_GeneratedByQuestGen(
            [NotNull] SitePart part,
            [NotNull] Slate slate,
            [NotNull] List<Rule> outExtraDescriptionRules,
            Dictionary<string, string> outExtraDescriptionConstants
        )
        {
            base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);

            Pawn pawn = null;
            var isNew = false;
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

            if (pawn == null)
            {
                pawn = PrisonerWillingToJoinQuestUtility.GeneratePrisoner(part.site.Tile, part.site.Faction);
                isNew = true;
            }

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

            part.things = new ThingOwner<Pawn>(part, true, isNew ? LookMode.Deep : LookMode.Reference);
            part.things.TryAdd(pawn);

            PawnRelationUtility.Notify_PawnsSeenByPlayer(
                Gen.YieldSingle(pawn),
                out string pawnRelationsInfo,
                true,
                false
            );

            string output = pawnRelationsInfo.NullOrEmpty()
                ? ""
                : $"\n\n{"PawnHasRelationshipsWithColonists".LocalizeKeyed(pawn?.LabelShort, pawn)}\n\n{pawnRelationsInfo}";
            slate.Set("prisoner", pawn);

            outExtraDescriptionRules.Add(new Rule_String("prisonerFullRelationInfo", output));
        }

        public override string GetPostProcessedThreatLabel([NotNull] Site site, [NotNull] SitePart sitePart)
        {
            string str = base.GetPostProcessedThreatLabel(site, sitePart);
            if (sitePart.things is {Any: true})
            {
                str = str + ": " + sitePart.things[0].LabelShortCap;
            }

            if (site.HasWorldObjectTimeout)
            {
                str +=
                    $" ({"DurationLeft".LocalizeKeyed((NamedArgument) site.WorldObjectTimeoutTicksLeft.ToStringTicksToPeriod())})";
            }

            return str;
        }
    }
}
