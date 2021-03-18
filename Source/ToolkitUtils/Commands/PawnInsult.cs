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
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using Verse;
using Verse.AI;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnInsult : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            Viewer data = Viewers.GetViewer(twitchMessage.Username);

            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            string query = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrFallback("");
            Pawn target = null;

            if (!query.NullOrEmpty())
            {
                if (query.StartsWith("@"))
                {
                    query = query.Substring(1);
                }

                Viewer viewer = Viewers.All.FirstOrDefault(v => v.username.EqualsIgnoreCase(query));

                if (viewer == null)
                {
                    return;
                }

                target = GetOrFindPawn(viewer.username);

                if (target == null)
                {
                    twitchMessage.Reply("TKUtils.PawnNotFound".Localize(query));
                    return;
                }
            }

            target ??= Find.ColonistBar.Entries.RandomElement().pawn;
            Job job = JobMaker.MakeJob(JobDefOf.Insult, target);

            if (job.CanBeginNow(pawn))
            {
                data.SetViewerKarma(Math.Max(data.karma - 15, ToolkitSettings.KarmaMinimum));

                pawn.jobs.StartJob(job, JobCondition.InterruptForced);
            }
        }
    }
}
