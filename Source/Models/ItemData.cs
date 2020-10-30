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
        public bool IsStuffAllowed;

        public bool IsWeapon;

        [System.ComponentModel.DefaultValue(null)]
        public KarmaType? KarmaType;

        public string Mod;

        [System.ComponentModel.DefaultValue(-1)]
        public int QuantityLimit;

        [JsonIgnore] private List<ResearchProjectDef> researchProjectDefs;

        [System.ComponentModel.DefaultValue(null)]
        public List<string> ResearchProjects;

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
