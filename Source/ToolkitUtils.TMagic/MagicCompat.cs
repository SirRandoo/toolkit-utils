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

using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using TorannMagic;
using TorannMagic.ModOptions;
using Verse;

namespace SirRandoo.ToolkitUtils.TMagic
{
    [UsedImplicitly]
    public class MagicCompat : IMagicCompatibilityProvider
    {
        public bool HasClass(Pawn pawn) => pawn.TryGetComp<CompAbilityUserMagic>()?.IsMagicUser == true || pawn.TryGetComp<CompAbilityUserMight>()?.IsMightUser == true;

        public bool IsClassTrait(TraitDef trait)
        {
            return trait.Equals(TorannMagicDefOf.DeathKnight) || TM_Data.AllClassTraits.Any(t => t.Equals(trait));
        }

        public void ResetClass(Pawn pawn)
        {
            TM_DebugTools.RemoveClass(pawn);
        }

        public bool IsUndead(Pawn pawn) => TM_Calc.IsUndead(pawn);

        public string GetSkillDescription(string invoker, string query)
        {
            if (!PurchaseHelper.TryGetPawn(invoker, out Pawn pawn))
            {
                return "TKUtils.NoPawn".Localize();
            }

            var builder = new StringBuilder();
            var userMagic = pawn.TryGetComp<CompAbilityUserMagic>();

            if (userMagic is { IsMagicUser: true } && TryGetMagicDescription(userMagic, query.ToToolkit(), out string description))
            {
                builder.Append(ResponseHelper.MagicGlyph.AltText("TKUtils.PawnClass.Magic".Localize()));
                builder.Append(" ");
                builder.Append(description);
            }

            var userMight = pawn.TryGetComp<CompAbilityUserMight>();

            if (!(userMight is { IsMightUser: true }) || !TryGetMightDescription(userMight, query.ToToolkit(), out description))
            {
                return builder.ToString();
            }

            builder.Append(ResponseHelper.DaggerGlyph.AltText("TKUtils.PawnClass.Might".Localize()));
            builder.Append(" ");
            builder.Append(description);

            return builder.ToString();
        }

        [NotNull] public string ModId => "Torann.ARimworldOfMagic";

        [ContractAnnotation("=> false,s:null; => true,s:notnull")]
        private static bool TryGetMagicDescription(CompAbilityUserMagic userMagic, string query, out string s) =>
            TryGetGlobalMagicDescription(userMagic, query, out s) || TryGetMagicSkillDescription(userMagic, query, out s);

        [ContractAnnotation("=> false,s:null; => true,s:notnull")]
        private static bool TryGetMightDescription(CompAbilityUserMight userMight, string query, out string s) =>
            TryGetGlobalMightDescription(userMight, query, out s) || TryGetMightSkillDescription(userMight, query, out s);

        [ContractAnnotation("=> false,s:null; => true,s:notnull")]
        private static bool TryGetGlobalMagicDescription(CompAbilityUserMagic userMagic, string query, out string s)
        {
            string regen = "TM_global_regen_pwr".Localize("regen").ToToolkit();

            if (query.EqualsIgnoreCase(regen) || query.EqualsIgnoreCase("regen"))
            {
                s = userMagic.MagicData.MagicPowerSkill_global_regen.FirstOrDefault()?.desc.Localize(null);

                return s != null;
            }

            string efficiency = "TM_global_eff_pwr".Localize("efficiency").ToToolkit();

            if (query.EqualsIgnoreCase(efficiency) || query.EqualsIgnoreCase("efficiency"))
            {
                s = userMagic.MagicData.MagicPowerSkill_global_eff.FirstOrDefault()?.desc.Localize(null);

                return s != null;
            }

            string spirit = "TM_global_spirit_pwr".Localize("versatility").ToToolkit();

            if (query.EqualsIgnoreCase(spirit) || query.EqualsIgnoreCase("versatility"))
            {
                s = userMagic.MagicData.MagicPowerSkill_global_spirit.FirstOrDefault()?.desc.Localize(null);

                return s != null;
            }

            s = null;

            return false;
        }

        [ContractAnnotation("=> false,s:null; => true,s:notnull")]
        private static bool TryGetGlobalMightDescription(CompAbilityUserMight userMight, string query, out string s)
        {
            string refresh = "TM_global_refresh_pwr".Localize("refresh").ToToolkit();

            if (query.EqualsIgnoreCase(refresh) || query.EqualsIgnoreCase("refresh"))
            {
                s = userMight.MightData.MightPowerSkill_global_refresh.FirstOrDefault()?.desc.Localize(null);

                return s != null;
            }

            string efficiency = "TM_global_seff_pwr".Localize("efficiency").ToToolkit();

            if (query.EqualsIgnoreCase(efficiency) || query.EqualsIgnoreCase("efficiency"))
            {
                s = userMight.MightData.MightPowerSkill_global_seff.FirstOrDefault()?.desc.Localize(null);

                return s != null;
            }

            string strength = "TM_global_strength_pwr".Localize("strength").ToToolkit();

            if (query.EqualsIgnoreCase(strength) || query.EqualsIgnoreCase("strength"))
            {
                s = userMight.MightData.MightPowerSkill_global_strength.FirstOrDefault()?.desc.Localize(null);

                return s != null;
            }

            string endurance = "TM_global_endurance_pwr".Localize("endurance").ToToolkit();

            if (query.EqualsIgnoreCase(endurance) || query.EqualsIgnoreCase("endurance"))
            {
                s = userMight.MightData.MightPowerSkill_global_endurance.FirstOrDefault()?.desc.Localize(null);

                return s != null;
            }

            s = null;

            return false;
        }

        [ContractAnnotation("=> false,s:null; => true,s:notnull")]
        private static bool TryGetMagicSkillDescription([NotNull] CompAbilityUserMagic userMagic, string query, out string s)
        {
            foreach (MagicPower magicPower in userMagic.MagicData.AllMagicPowers)
            {
                if (!magicPower.learned || !(magicPower.abilityDef is TMAbilityDef def))
                {
                    continue;
                }

                if (query.EqualsIgnoreCase(def.label.ToToolkit()) || query.Equals(def.defName))
                {
                    s = def.description;

                    return s != null;
                }

                MagicPowerSkill power = userMagic.MagicData.GetSkill_Power(def);

                if (query.EqualsIgnoreCase(power.label.ToToolkit()) || query.Equals($"{def.defName}_power"))
                {
                    s = power.desc.Localize(null);

                    return s != null;
                }

                MagicPowerSkill efficiency = userMagic.MagicData.GetSkill_Efficiency(def);

                if (query.EqualsIgnoreCase(efficiency.label.ToToolkit()) || query.Equals($"{def.defName}_efficiency"))
                {
                    s = efficiency.desc.Localize(null);

                    return s != null;
                }

                MagicPowerSkill versatility = userMagic.MagicData.GetSkill_Versatility(def);

                if (query.EqualsIgnoreCase(versatility.label.ToToolkit()) || query.Equals($"{def.defName}_versatility"))
                {
                    s = versatility.desc.Localize(null);

                    return s != null;
                }
            }

            s = null;

            return false;
        }

        [ContractAnnotation("=> false,s:null; => true,s:notnull")]
        private static bool TryGetMightSkillDescription([NotNull] CompAbilityUserMight userMight, string query, out string s)
        {
            foreach (MightPower mightPower in userMight.MightData.AllMightPowers)
            {
                if (!mightPower.learned || !(mightPower.abilityDef is TMAbilityDef def))
                {
                    continue;
                }

                if (query.EqualsIgnoreCase(def.label.ToToolkit()) || query.Equals(def.defName))
                {
                    s = def.description;

                    return s != null;
                }

                MightPowerSkill power = userMight.MightData.GetSkill_Power(def);

                if (query.EqualsIgnoreCase(power.label.ToToolkit()) || query.Equals($"{def.defName}_power"))
                {
                    s = power.desc.Localize(null);

                    return s != null;
                }

                MightPowerSkill efficiency = userMight.MightData.GetSkill_Efficiency(def);

                if (query.EqualsIgnoreCase(efficiency.label.ToToolkit()) || query.Equals($"{def.defName}_efficiency"))
                {
                    s = efficiency.desc.Localize(null);

                    return s != null;
                }

                MightPowerSkill versatility = userMight.MightData.GetSkill_Versatility(def);

                if (query.EqualsIgnoreCase(versatility.label.ToToolkit()) || query.Equals($"{def.defName}_versatility"))
                {
                    s = versatility.desc.Localize(null);

                    return s != null;
                }
            }

            s = null;

            return false;
        }
    }
}
