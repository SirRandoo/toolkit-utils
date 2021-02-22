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
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class PawnKindItem
    {
        [JsonProperty("price")] public int Cost;
        [JsonIgnore] private PawnKindData data;

        public string DefName;
        public bool Enabled;
        [JsonIgnore] private List<PawnKindDef> kinds;
        public string Name;

        [JsonIgnore]
        public IEnumerable<PawnKindDef> Kinds =>
            kinds ??= DefDatabase<PawnKindDef>.AllDefs.Where(k => k.race.defName.Equals(DefName)).ToList();

        [JsonIgnore]
        public PawnKindDef ColonistKindDef =>
            Kinds.FirstOrDefault(k => k.defaultFactionType.isPlayer) ?? Kinds.FirstOrFallback();

        public PawnKindData Data => data ??= new PawnKindData();


        public static PawnKindItem MigrateFrom(XmlRace race)
        {
            return new PawnKindItem
            {
                DefName = race.DefName, Enabled = race.Enabled, Name = race.Name, Cost = race.Price
            };
        }

        public string GetDefaultName()
        {
            return ColonistKindDef?.race.label.ToToolkit() ?? DefName;
        }
    }
}
