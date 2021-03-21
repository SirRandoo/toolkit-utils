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
using System.Runtime.Serialization;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PawnKindItem : IShopItemBase
    {
        [IgnoreDataMember] private PawnKindData data;
        [IgnoreDataMember] private List<PawnKindDef> kinds;

        [NotNull]
        [IgnoreDataMember]
        public IEnumerable<PawnKindDef> Kinds =>
            kinds ??= DefDatabase<PawnKindDef>.AllDefs.Where(k => k.race.defName.Equals(DefName)).ToList();

        [IgnoreDataMember]
        public PawnKindDef ColonistKindDef =>
            Kinds.FirstOrDefault(k => k.defaultFactionType.isPlayer) ?? Kinds.FirstOrFallback();

        [DataMember(Name = "data")]
        public PawnKindData PawnData
        {
            get => data ??= (PawnKindData) Data;
            set => Data = data = value;
        }

        public string DefName { get; set; }

        public bool Enabled { get; set; }
        public string Name { get; set; }

        [DataMember(Name = "price")] public int Cost { get; set; }

        [IgnoreDataMember] public IShopDataBase Data { get; set; }


        [NotNull]
        public static PawnKindItem MigrateFrom([NotNull] XmlRace race)
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
