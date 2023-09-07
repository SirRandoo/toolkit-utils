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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using TwitchToolkit;
using Verse;

namespace SirRandoo.ToolkitUtils.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ItemData : IShopDataBase
{
    internal const int CurrentVersion = 2;
    private readonly List<ResearchProjectDef> _researchProjects = new List<ResearchProjectDef>();

    [JsonProperty("CustomName")] public string CustomName { get; set; }
    [JsonProperty("HasQuantityLimit")] public bool HasQuantityLimit { get; set; }
    [JsonProperty("IsMelee")] public bool IsMelee { get; set; }
    [JsonProperty("IsRanged")] public bool IsRanged { get; set; }
    [JsonProperty("IsStuffAllowed")] public bool IsStuffAllowed { get; set; }
    [JsonProperty("IsWeapon")] public bool IsWeapon { get; set; }
    [JsonProperty("QuantityLimit")] public int QuantityLimit { get; set; } = 1;
    [JsonProperty("Weight")] public float Weight { get; set; } = 1f;
    [JsonProperty("ResearchOverrides")] public List<string> ResearchOverrides { get; set; }
    [JsonProperty("IsUsable")] public bool IsUsable { get; set; }
    [JsonProperty("IsEquippable")] public bool IsEquippable { get; set; }
    [JsonProperty("IsWearable")] public bool IsWearable { get; set; }
    [JsonProperty("KarmaTypeForUsing")] public KarmaType? KarmaTypeForUsing { get; set; }
    [JsonProperty("KarmaTypeForWearing")] public KarmaType? KarmaTypeForWearing { get; set; }
    [JsonProperty("KarmaTypeForEquipping")] public KarmaType? KarmaTypeForEquipping { get; set; }
    [JsonProperty("version")] public int Version { get; set; }
    [JsonProperty("Mod")] public string Mod { get; set; }
    [JsonProperty("KarmaType")] public KarmaType? KarmaType { get; set; }

    public void Reset()
    {
        KarmaType = null;
        KarmaTypeForEquipping = null;
        KarmaTypeForUsing = null;
        KarmaTypeForWearing = null;
        ResearchOverrides = null;
        Weight = 1f;
        QuantityLimit = 1;
        IsStuffAllowed = true;
        CustomName = null;
        HasQuantityLimit = false;
        IsWearable = true;
        IsUsable = true;
        IsEquippable = true;

        ThingItem parent = Data.Items.Find(i => i.ItemData == this);

        if (parent?.Thing == null)
        {
            return;
        }

        IsUsable = GameHelper.GetDefaultUsability(parent.Thing);
        IsStuffAllowed = GameHelper.GetDefaultMaterialState(parent.Thing);
        IsEquippable = parent.Thing.IsWeapon;
        IsWearable = parent.Thing.IsApparel;
    }

    [CanBeNull]
    public List<ResearchProjectDef> GetResearchOverrides()
    {
        if (ResearchOverrides.NullOrEmpty())
        {
            return new List<ResearchProjectDef>(0);
        }

        if (_researchProjects.Count == ResearchOverrides.Count)
        {
            return _researchProjects;
        }

        var toCull = new List<string>();

        foreach (string defName in ResearchOverrides.Where(o => _researchProjects.Find(i => i.defName.Equals(o)) == null))
        {
            ResearchProjectDef def = DefDatabase<ResearchProjectDef>.GetNamed(defName, false);

            if (def == null)
            {
                TkUtils.Logger.Warn($@"The research project ""{defName}"" could not be found!");
                toCull.Add(defName);
            }

            _researchProjects.Add(def);
        }

        if (toCull.Count <= 0)
        {
            return _researchProjects;
        }

        foreach (string defName in toCull)
        {
            ResearchOverrides.Remove(defName);
        }

        return _researchProjects;
    }
}