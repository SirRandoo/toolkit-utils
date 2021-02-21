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
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class Database : CommandBase
    {
        private static readonly Dictionary<string, string> Index;
        private ITwitchMessage msg;

        static Database()
        {
            Index = new Dictionary<string, string>
            {
                {"weapon", "weapons"},
                {"weapons", "weapons"},
                {"gun", "weapons"},
                {"sword", "weapons"},
                {"melee", "weapons"},
                {"ranged", "weapons"},
                {"club", "weapons"},
                {"clubs", "weapons"},
                {"knife", "weapons"},
                {"knives", "weapons"}
            };
        }

        public override void RunCommand(ITwitchMessage twitchMessage)
        {
            msg = twitchMessage;
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

        private void Notify__LookupComplete(string result)
        {
            if (result.NullOrEmpty())
            {
                return;
            }

            msg.Reply(result);
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

            Notify__LookupComplete(ResponseHelper.JoinPair(weapon.Name, result.GroupedJoin()));
        }

        private void PerformLookup(string category, string query)
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
