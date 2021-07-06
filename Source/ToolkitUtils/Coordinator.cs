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
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using TwitchToolkit;
using TwitchToolkit.Store;
using TwitchToolkit.Votes;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class Coordinator : GameComponent
    {
        private readonly ConcurrentQueue<IncidentProxy> incidentQueue = new ConcurrentQueue<IncidentProxy>();
        private readonly List<ToolkitGateway> portals = new List<ToolkitGateway>();

        private int lastMinute;
        private int lastRefreshMinute;
        private int rewardPeriodTracker;

        public Coordinator(Game game) { }

        internal bool HasActivePortals => portals.Count > 0;

        [ContractAnnotation("map:notnull => true,portal:notnull; map:notnull => false,portal:null")]
        internal bool TryGetRandomPortal(Map map, out ToolkitGateway portal)
        {
            portals.Where(p => p.Map.Equals(map)).TryRandomElement(out portal);
            return portal != null;
        }

        [ContractAnnotation("map:notnull => true,portal:notnull; map:notnull => false,portal:null")]
        internal bool TryGetRandomItemPortal(Map map, out ToolkitGateway portal)
        {
            portals.Where(p => p.ForItems).Where(p => p.Map.Equals(map)).TryRandomElement(out portal);
            return portal != null;
        }

        [ContractAnnotation("map:notnull => true,portal:notnull; map:notnull => false,portal:null")]
        internal bool TryGetRandomPawnPortal(Map map, out ToolkitGateway portal)
        {
            portals.Where(p => p.ForPawns).Where(p => p.Map.Equals(map)).TryRandomElement(out portal);
            return portal != null;
        }

        internal bool TrySpawnItem(Map map, Thing item)
        {
            if (!TryGetRandomItemPortal(map, out ToolkitGateway portal))
            {
                return false;
            }

            GenPlace.TryPlaceThing(
                item,
                portal.Position,
                portal.Map,
                ThingPlaceMode.Near,
                (thing, count) =>
                {
                    if (!TkSettings.GatewayPuff)
                    {
                        return;
                    }

                #if RW12
                    MoteMaker.ThrowSmoke(thing.Position.ToVector3(), thing.Map, thing.Graphic.drawSize.magnitude);
                #else
                    FleckMaker.ThrowSmoke(thing.Position.ToVector3(), thing.Map, thing.Graphic.drawSize.magnitude);
                #endif
                }
            );
            return true;
        }

        internal bool TrySpawnPawn(Map map, Pawn pawn)
        {
            if (!TryGetRandomPawnPortal(map, out ToolkitGateway portal))
            {
                return false;
            }

            GenSpawn.Spawn(pawn, portal.Position, portal.Map, WipeMode.VanishOrMoveAside);
            return true;
        }

        internal void RemovePortal(ToolkitGateway thing)
        {
            portals.Remove(thing);
        }

        internal void RegisterPortal(ToolkitGateway thing)
        {
            portals.Add(thing);
        }

        internal void QueueIncident(IncidentProxy incident)
        {
            incidentQueue.Enqueue(incident);
        }

        public override void GameComponentUpdate()
        {
            VoteHandler.CheckForQueuedVotes();

            if (Viewers.jsonallviewers.NullOrEmpty())
            {
                Viewers.RefreshViewers();
            }

            if (!Viewers.jsonallviewers.NullOrEmpty() && ToolkitSettings.EarningCoins)
            {
                ProcessCoinReward();
            }

            if (TkSettings.AsapPurchases)
            {
                ProcessNextEvent();
            }
        }

        private void ProcessCoinReward()
        {
            int currentMinute = GetCurrentMinute();

            if (currentMinute > 0 && currentMinute % 5 == 0 && lastRefreshMinute != currentMinute)
            {
                Viewers.RefreshViewers();
                lastRefreshMinute = currentMinute;
                LogHelper.Debug($"Refreshed viewers @ {DateTime.Now:T}");
            }

            if (currentMinute <= lastMinute || currentMinute < 1)
            {
                return;
            }

            rewardPeriodTracker += 1;
            lastMinute = currentMinute;

            if (rewardPeriodTracker < ToolkitSettings.CoinInterval)
            {
                return;
            }

            Viewers.AwardViewersCoins();
            rewardPeriodTracker = 0;
            LogHelper.Debug($"Awarded viewers coins @ {DateTime.Now:T}");
        }

        public override void GameComponentTick()
        {
            if (!TryProcessNextVoteIncident())
            {
                ProcessNextIncident();
            }

            Toolkit.JobManager.CheckAllJobs();

            if (TkSettings.AsapPurchases)
            {
                return;
            }

            ProcessNextEvent();
        }

        private static bool TryProcessNextVoteIncident()
        {
            if (!Ticker.IncidentHelpers.TryDequeue(out IncidentHelper incident))
            {
                return false;
            }

            try
            {
                incident.TryExecute();
                return true;
            }
            catch (Exception)
            {
                // unused
                return false;
            }
        }

        private static void ProcessNextIncident()
        {
            if (!Ticker.FiringIncidents.TryDequeue(out FiringIncident incident))
            {
                return;
            }

            try
            {
                incident.def.Worker.TryExecute(incident.parms);
            }
            catch (Exception e)
            {
                LogHelper.Error($@"The incident ""{incident.def.defName}"" raised an exception", e);
            }
        }

        private void ProcessNextEvent()
        {
            if (!incidentQueue.TryDequeue(out IncidentProxy incident))
            {
                return;
            }

            incident.TryExecute();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref rewardPeriodTracker, "rewardPeriod", GetCurrentMinute());
        }

        private static int GetCurrentMinute()
        {
            return Mathf.FloorToInt(Time.unscaledTime / 60.0f);
        }
    }
}
