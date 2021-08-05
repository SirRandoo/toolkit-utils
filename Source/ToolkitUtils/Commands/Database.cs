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
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Database : CommandBase
    {
        private static readonly Dictionary<string, string> Index;
        private string invoker;

        static Database()
        {
            Index = new Dictionary<string, string>
            {
                { "weapon", "weapons" },
                { "weapons", "weapons" },
                { "gun", "weapons" },
                { "sword", "weapons" },
                { "melee", "weapons" },
                { "ranged", "weapons" },
                { "club", "weapons" },
                { "clubs", "weapons" },
                { "knife", "weapons" },
                { "knives", "weapons" }
            };
        }

        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            invoker = twitchMessage.Username;
            string[] segments = CommandFilter.Parse(twitchMessage.Message).Skip(1).ToArray();
            string category = segments.FirstOrFallback("");
            string query = segments.Skip(1).FirstOrFallback("");

            if (!Index.TryGetValue(category.ToLowerInvariant(), out string _))
            {
                query = category;
                category = "weapons";
            }

            PerformLookup(category, query);
        }

        private void NotifyLookupComplete(string result)
        {
            if (result.NullOrEmpty())
            {
                return;
            }

            MessageHelper.ReplyToUser(invoker, result);
        }

        private void PerformWeaponLookup(string query)
        {
            ThingItem weapon = Data.Items.Where(t => t.Thing.IsWeapon)
               .FirstOrDefault(
                    t => t.Name.EqualsIgnoreCase(query.ToToolkit())
                         || t.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                );

            if (weapon == null)
            {
                return;
            }

            var result = new List<string>();

            if (!weapon.Thing.statBases.NullOrEmpty())
            {
                result.AddRange(weapon.Thing.statBases.Select(stat => $"{stat.value} {stat.stat.label}"));
            }

            if (!weapon.Thing.equippedStatOffsets.NullOrEmpty())
            {
                result.AddRange(weapon.Thing.equippedStatOffsets.Select(stat => $"{stat.ValueToStringAsOffset}"));
            }

            if (!weapon.Thing.damageMultipliers.NullOrEmpty())
            {
                result.AddRange(weapon.Thing.damageMultipliers.Select(m => $"{m.damageDef.LabelCap} x{m.multiplier}"));
            }

            NotifyLookupComplete(ResponseHelper.JoinPair(weapon.Name, result.GroupedJoin()));
        }

        private void PerformLookup([NotNull] string category, string query)
        {
            if (!Index.TryGetValue(category.ToLowerInvariant(), out string result))
            {
                return;
            }

            switch (result)
            {
                case "weapons":
                    PerformWeaponLookup(query);
                    return;
            }
        }
    }
}
