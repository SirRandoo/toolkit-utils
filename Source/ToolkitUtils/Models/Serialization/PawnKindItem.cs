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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PawnKindItem : IShopItemBase
    {
        [IgnoreDataMember] private KindDefData _colonistDef;
        [IgnoreDataMember] private PawnKindData _data;
        [IgnoreDataMember] private KindDefData[] _kinds;

        [NotNull] [IgnoreDataMember] public IEnumerable<PawnKindDef> Kinds => _kinds.Select(d => d.Def);

        [IgnoreDataMember] public PawnKindDef ColonistKindDef => _colonistDef.Def;

        [DataMember(Name = "data")]
        public PawnKindData PawnData
        {
            get => _data ??= (PawnKindData)Data;
            set => Data = _data = value;
        }

        [CanBeNull] [DataMember(Name = "description")] public string Description { get; private set; }

        [DataMember(Name = "defName")] public string DefName { get; set; }
        [DataMember(Name = "enabled")] public bool Enabled { get; set; }
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "price")] public int Cost { get; set; }

        [IgnoreDataMember] public IShopDataBase Data { get; set; }

        public void ResetName()
        {
            if (ColonistKindDef != null)
            {
                Name = ColonistKindDef.label.ToToolkit();
            }
        }

        public void ResetPrice()
        {
            PawnKindDef def = ColonistKindDef;

            if (def?.race != null)
            {
                Cost = def.race.CalculateStorePrice();
            }
        }

        public void ResetData()
        {
            PawnData.Reset();
        }

        public void UpdateStats()
        {
            PawnKindDef def = ColonistKindDef;

            if (def?.race?.statBases == null)
            {
                return;
            }

            var builder = new StringBuilder();
            var container = new List<string>();

            foreach (StatModifier stat in def.race.statBases)
            {
                try
                {
                    container.Add($"{stat.ValueToStringAsOffset} {stat.stat.label?.CapitalizeFirst() ?? stat.stat.defName}");
                }
                catch (Exception)
                {
                    builder.AppendLine($"- {stat?.stat?.label ?? stat?.stat?.defName ?? "UNPROCESSABLE"}");
                }
            }

            if (builder.Length > 0)
            {
                builder.Insert(0, $@"The following stats could not be processed for ""{def.label ?? def.defName}"":\n");
                TkUtils.Logger.Warn(builder.ToString());
            }

            PawnData.Stats = container.ToArray();
        }

        [OnDeserialized]
        private void LoadGameData(StreamingContext context)
        {
            KindDefData? colonist = null;
            var container = new List<KindDefData>();

            foreach (PawnKindDef kindDef in DefDatabase<PawnKindDef>.AllDefs)
            {
                if (!kindDef.race.defName.Equals(DefName))
                {
                    continue;
                }

                var data = new KindDefData { Name = kindDef.race.label.ToToolkit().ToLower(), Def = kindDef };
                container.Add(data);

                if (kindDef.defaultFactionType == FactionDefOf.PlayerColony)
                {
                    colonist = data;
                }
            }

            colonist ??= container.First();
            _colonistDef = colonist.Value;
            Description = _colonistDef.Def.race.description;
            _kinds = container.ToArray();
        }

        public string GetDefaultName() => _colonistDef.Name ?? DefName;

        private struct KindDefData
        {
            public string Name { get; set; }
            public PawnKindDef Def { get; set; }
        }
    }
}
