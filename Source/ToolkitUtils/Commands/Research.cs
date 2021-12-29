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

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Research : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            string query = CommandFilter.Parse(twitchMessage.Message).Skip(1).FirstOrDefault();
            ResearchProjectDef project;

            if (query.NullOrEmpty())
            {
                project = Current.Game.researchManager.currentProj;
            }
            else
            {
                project = DefDatabase<ResearchProjectDef>.AllDefs.FirstOrDefault(p => p.defName.EqualsIgnoreCase(query) || p.label.ToToolkit().EqualsIgnoreCase(query!.ToToolkit()));


                if (project == null)
                {
                    ThingDef thing = DefDatabase<ThingDef>.AllDefs.FirstOrDefault(t => t.defName.EqualsIgnoreCase(query) || t.label?.ToToolkit()?.EqualsIgnoreCase(query!.ToToolkit()) == true);

                    project = thing?.recipeMaker?.researchPrerequisite;
                    project ??= thing?.recipeMaker?.researchPrerequisites?.FirstOrDefault(p => !p.IsFinished);
                }
            }

            if (project == null)
            {
                twitchMessage.Reply((!query.NullOrEmpty() ? "TKUtils.Research.InvalidQuery".LocalizeKeyed(query) : "TKUtils.Research.None".Localize()).WithHeader("Research".Localize()));

                return;
            }

            var segments = new List<string>
            {
                ResponseHelper.JoinPair(project.LabelCap, project.ProgressPercent.ToStringPercent())
            };

            if (project.prerequisites != null && !project.PrerequisitesCompleted)
            {
                List<ResearchProjectDef> prerequisites = project.prerequisites;

                string[] container = prerequisites.Where(prerequisite => !prerequisite.IsFinished)
                   .Select(prerequisite => ResponseHelper.JoinPair(prerequisite.LabelCap, prerequisite.ProgressPercent.ToStringPercent()))
                   .ToArray();

                segments.Add(ResponseHelper.JoinPair("ResearchPrerequisites".Localize(), container.SectionJoin()));
            }

            twitchMessage.Reply(segments.GroupedJoin().WithHeader("Research".Localize()));
        }
    }
}
