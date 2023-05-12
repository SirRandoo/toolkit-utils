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
using RimWorld;
using ToolkitCore.Utilities;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using ToolkitUtils.Workers;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class Database : CommandBase
    {
        private static readonly Dictionary<string, Category> Index = new Dictionary<string, Category>
        {
            { "weapon", Category.Weapon },
            { "weapons", Category.Weapon },
            { "gun", Category.Weapon },
            { "sword", Category.Weapon },
            { "melee", Category.Weapon },
            { "ranged", Category.Weapon },
            { "club", Category.Weapon },
            { "clubs", Category.Weapon },
            { "knife", Category.Weapon },
            { "knives", Category.Weapon },
            { "spell", Category.TMagic },
            { "spells", Category.TMagic },
            { "ability", Category.TMagic },
            { "abilities", Category.TMagic }
        };

        private static readonly List<StatDef> WeaponStats = new List<StatDef> { StatDefOf.AccuracyLong, StatDefOf.AccuracyMedium, StatDefOf.AccuracyShort };

        private static readonly List<StatDef> RangedWeaponStats = new List<StatDef> { StatDefOf.RangedWeapon_Cooldown, StatDefOf.RangedWeapon_DamageMultiplier };

        private static readonly List<StatDef> MeleeWeaponStats = new List<StatDef>
        {
            StatDefOf.MeleeWeapon_AverageArmorPenetration,
            StatDefOf.MeleeWeapon_AverageDPS,
            StatDefOf.MeleeWeapon_CooldownMultiplier,
            StatDefOf.MeleeWeapon_DamageMultiplier
        };

        private string _invoker;

        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            _invoker = twitchMessage.Username;
            string[] segments = CommandFilter.Parse(twitchMessage.Message).Skip(1).ToArray();
            string category = segments.FirstOrFallback("");
            string query = segments.Skip(1).FirstOrFallback("");

            if (!Index.TryGetValue(category.ToLowerInvariant(), out Category result))
            {
                query = category;
            }

            PerformLookup(result, query);
        }

        private void NotifyLookupComplete(string result)
        {
            if (result.NullOrEmpty())
            {
                return;
            }

            MessageHelper.ReplyToUser(_invoker, result);
        }

        private void PerformWeaponLookup([NotNull] string query)
        {
            var worker = ArgWorker.CreateInstance(query);

            if (!worker.TryGetNextAsItem(out ArgWorker.ItemProxy item) || !item.IsValid() || !item.Thing.Thing.IsWeapon)
            {
                MessageHelper.ReplyToUser(_invoker, "TKUtils.InvalidItemQuery".Translate(worker.GetLast()));

                return;
            }

            if (item.TryGetError(out string error))
            {
                MessageHelper.ReplyToUser(_invoker, error);

                return;
            }

            var result = new List<string>();

            Thing thing = PurchaseHelper.MakeThing(item.Thing.Thing, item.Stuff.Thing, item.Quality);

            CommandRouter.MainThreadCommands.Enqueue(
                () =>
                {
                    result.AddRange(WeaponStats.Select(s => s.ValueToString(thing.GetStatValue(s))));

                    if (item.Thing.Thing.IsMeleeWeapon)
                    {
                        result.AddRange(MeleeWeaponStats.Select(s => s.ValueToString(thing.GetStatValue(s))));
                    }

                    if (item.Thing.Thing.IsRangedWeapon)
                    {
                        result.AddRange(RangedWeaponStats.Select(s => s.ValueToString(thing.GetStatValue(s))));
                    }

                    NotifyLookupComplete(ResponseHelper.JoinPair(item.Thing.ToString(), result.GroupedJoin()));
                }
            );
        }

        private void PerformTMagicLookup(string query)
        {
            CommandRouter.MainThreadCommands.Enqueue(() => NotifyLookupComplete(CompatRegistry.Magic?.GetSkillDescription(_invoker, query)));
        }

        private void PerformLookup(Category category, string query)
        {
            switch (category)
            {
                case Category.Weapon:
                    PerformWeaponLookup(query);

                    return;
                case Category.TMagic when CompatRegistry.Magic != null:
                    PerformTMagicLookup(query);

                    return;
            }
        }

        // ReSharper disable once InconsistentNaming
        private enum Category { Weapon, TMagic }
    }
}
