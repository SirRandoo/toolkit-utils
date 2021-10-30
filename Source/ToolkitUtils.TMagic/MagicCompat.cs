// MIT License
//
// Copyright (c) 2021 SirRandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using TorannMagic;
using TorannMagic.ModOptions;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public class MagicCompat : Compat.MagicCompat
    {
        public override bool HasClass(Pawn pawn)
        {
            if (pawn.TryGetComp<CompAbilityUserMagic>()?.IsMagicUser ?? false)
            {
                return true;
            }

            return pawn.TryGetComp<CompAbilityUserMight>()?.IsMightUser ?? false;
        }

        public override bool IsClassTrait(TraitDef trait)
        {
            return trait.Equals(TorannMagicDefOf.DeathKnight) || TM_Data.AllClassTraits.Any(t => t.Equals(trait));
        }

        public override void ResetClass(Pawn pawn)
        {
            TM_DebugTools.RemoveClass(pawn);
        }

        public override string GetSkillDescription(string invoker, string query)
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

        [ContractAnnotation("=> false,s:null; => true,s:notnull")]
        private static bool TryGetMagicDescription(CompAbilityUserMagic userMagic, string query, out string s)
        {
            return TryGetGlobalMagicDescription(userMagic, query, out s) || TryGetMagicSkillDescription(userMagic, query, out s);
        }

        [ContractAnnotation("=> false,s:null; => true,s:notnull")]
        private static bool TryGetMightDescription(CompAbilityUserMight userMight, string query, out string s)
        {
            return TryGetGlobalMightDescription(userMight, query, out s) || TryGetMightSkillDescription(userMight, query, out s);
        }

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
