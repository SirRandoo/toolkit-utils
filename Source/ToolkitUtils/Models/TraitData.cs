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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class TraitData : IShopDataBase
    {
        [DataMember(Name = "conflicts")] public string[] Conflicts = { };
        [DataMember(Name = "stats")] public string[] Stats = { };
        [DataMember(Name = "canBypassLimit")] public bool CanBypassLimit { get; set; }
        [DataMember(Name = "customName")] public bool CustomName { get; set; }

        [DataMember(Name = "karmaTypeForRemoving")]
        public KarmaType? KarmaTypeForRemoving { get; set; }

        [DataMember(Name = "mod")] public string Mod { get; set; }

        [DataMember(Name = "karmaTypeForAdding")]
        public KarmaType? KarmaType { get; set; }
    }
}
