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
using Newtonsoft.Json;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class ItemData
    {
        [System.ComponentModel.DefaultValue(null)]
        public string CustomName;

        public bool HasQuantityLimit;
        public bool IsMelee;
        public bool IsRanged;

        [System.ComponentModel.DefaultValue(true)]
        public bool IsStuffAllowed = true;

        public bool IsWeapon;

        [System.ComponentModel.DefaultValue(null)]
        public KarmaType? KarmaType;

        public string Mod;

        [System.ComponentModel.DefaultValue(-1)]
        public int QuantityLimit = 1;

        [JsonIgnore] private List<ResearchProjectDef> researchProjectDefs;

        [System.ComponentModel.DefaultValue(null)]
        public List<string> ResearchProjects;

        [System.ComponentModel.DefaultValue(1f)]
        public float Weight = 1f;

        public IEnumerable<ResearchProjectDef> GetResearchProjects()
        {
            if (ResearchProjects.NullOrEmpty() || researchProjectDefs.NullOrEmpty())
            {
                yield break;
            }

            if (researchProjectDefs.Count <= 0)
            {
                researchProjectDefs ??= new List<ResearchProjectDef>();
                foreach (ResearchProjectDef projectDef in DefDatabase<ResearchProjectDef>.AllDefs.Where(
                    r => ResearchProjects.Contains(r.defName)
                ))
                {
                    researchProjectDefs.Add(projectDef);
                }
            }

            foreach (ResearchProjectDef projectDef in researchProjectDefs)
            {
                yield return projectDef;
            }
        }
    }
}
