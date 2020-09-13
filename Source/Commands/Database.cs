﻿using System.Collections.Generic;
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
        private static readonly Dictionary<string, string> Index = new Dictionary<string, string>
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
            {"knives", "weapons"},
            {"pawn", "kinds"},
            {"pawns", "kinds"},
            {"race", "kinds"},
            {"races", "kinds"},
            {"kinds", "kinds"},
            {"kind", "kinds"},
            {"trait", "traits"},
            {"traits", "traits"}
        };

        private ITwitchMessage msg;

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
                case "traits":
                    PerformTraitLookup(query);
                    return;
                case "kinds":
                    PerformKindLookup(query);
                    return;
            }
        }

        private void PerformKindLookup(string query)
        {
            PawnKindDef kind = DefDatabase<PawnKindDef>.AllDefsListForReading.FirstOrDefault(
                t => t.race.label.ToToolkit().EqualsIgnoreCase(query.ToToolkit())
                     || t.race.defName.ToToolkit().EqualsIgnoreCase(query)
            );

            if (kind == null)
            {
                return;
            }

            var result = new List<string>();

            if (!kind.disallowedTraits.NullOrEmpty())
            {
                var container = new List<string>();
            }

            string[] results = Data.PawnKinds.Where(
                    i =>
                    {
                        string label = i.Name.ToToolkit();
                        string q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.DefName.ToToolkit().Contains(query.ToToolkit())
                               || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
               .Where(r => r.Enabled)
               .Select(i => i.Name.ToToolkit().CapitalizeFirst())
               .ToArray();

            Notify__LookupComplete(result.GroupedJoin());
        }

        private void PerformTraitLookup(string query)
        {
            string[] results = Data.Traits.Where(
                    i =>
                    {
                        string label = i.Name.ToToolkit();
                        string q = query.ToToolkit();

                        if (label.Contains(q) || label.EqualsIgnoreCase(q))
                        {
                            return true;
                        }

                        return i.DefName.ToToolkit().Contains(query.ToToolkit())
                               || i.DefName.ToToolkit().EqualsIgnoreCase(query.ToToolkit());
                    }
                )
               .Where(t => t.CanAdd || t.CanRemove)
               .Select(i => i.Name.ToToolkit().CapitalizeFirst())
               .ToArray();

            var container = new List<string>();

            Notify__LookupComplete(container.GroupedJoin());
        }
    }
}
