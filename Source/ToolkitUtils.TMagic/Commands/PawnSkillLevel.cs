// ToolkitUtils.TMagic
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
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TorannMagic;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly]
    public class PawnSkillLevel : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage msg)
        {
            if (!PurchaseHelper.TryGetPawn(msg.Username, out Pawn pawn))
            {
                msg.Reply("TKUtils.NoPawn".Localize());
            }

            string skill = CommandFilter.Parse(msg.Message).Skip(1).FirstOrDefault();

            if (skill == null)
            {
                return;
            }

            CommandRouter.MainThreadCommands.Enqueue(
                () =>
                {
                    string error;
                    var magicUser = pawn.TryGetComp<CompAbilityUserMagic>();

                    if (magicUser is { IsMagicUser: true })
                    {
                        if (TryLevelMagic(magicUser, skill.ToToolkit(), out error))
                        {
                            msg.Reply("Done!");
                        }
                        else if (!error.NullOrEmpty())
                        {
                            msg.Reply(error);
                        }

                        return;
                    }

                    var mightUser = pawn.TryGetComp<CompAbilityUserMight>();

                    if (!(mightUser is { IsMightUser: true }))
                    {
                        return;
                    }

                    if (TryLevelMight(mightUser, skill.ToToolkit(), out error))
                    {
                        msg.Reply("Done!");
                    }
                    else if (!error.NullOrEmpty())
                    {
                        msg.Reply(error);
                    }
                }
            );
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private bool TryLevelMight([NotNull] CompAbilityUserMight mightUser, string query, out string error)
        {
            if (mightUser.MightData.MightAbilityPoints <= 0)
            {
                error = "No points to spend!";

                return false;
            }

            if (TryLevelGlobalMightSkill(mightUser, query, out error))
            {
                return true;
            }

            return error.NullOrEmpty() && TryLevelMightSkill(mightUser, query, out error);
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private static bool TryLevelGlobalMightSkill(CompAbilityUserMight mightUser, string query, out string error)
        {
            string refresh = "TM_global_refresh_pwr".Localize("refresh").ToToolkit();

            if (query.EqualsIgnoreCase(refresh) || query.EqualsIgnoreCase("refresh"))
            {
                return TryLevelSkill(mightUser.MightData.MightPowerSkill_global_refresh.FirstOrDefault(), mightUser, out error);
            }

            string efficiency = "TM_global_seff_pwr".Localize("efficiency").ToToolkit();

            if (query.EqualsIgnoreCase(efficiency) || query.EqualsIgnoreCase("efficiency"))
            {
                return TryLevelSkill(mightUser.MightData.MightPowerSkill_global_seff.FirstOrDefault(), mightUser, out error);
            }

            string strength = "TM_global_strength_pwr".Localize("strength").ToToolkit();

            if (query.EqualsIgnoreCase(strength) || query.EqualsIgnoreCase("strength"))
            {
                return TryLevelSkill(mightUser.MightData.MightPowerSkill_global_strength.FirstOrDefault(), mightUser, out error);
            }

            string endurance = "TM_global_endurance_pwr".Localize("endurance").ToToolkit();

            if (query.EqualsIgnoreCase(endurance) || query.EqualsIgnoreCase("endurance"))
            {
                return TryLevelSkill(mightUser.MightData.MightPowerSkill_global_endurance.FirstOrDefault(), mightUser, out error);
            }

            error = "";

            return false;
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private bool TryLevelMightSkill([NotNull] CompAbilityUserMight mightUser, string query, out string error)
        {
            foreach (MightPower magicPower in mightUser.MightData.AllMightPowers)
            {
                if (magicPower.learned || !(magicPower.abilityDef is TMAbilityDef def))
                {
                    continue;
                }

                if ((query.EqualsIgnoreCase(def.label.ToToolkit()) || query.Equals(def.defName)) && magicPower.level < magicPower.maxLevel
                    && mightUser.MightData.MightAbilityPoints >= magicPower.costToLevel)
                {
                    mightUser.MightData.MightAbilityPoints -= magicPower.costToLevel;
                    magicPower.level++;
                }

                MightPowerSkill power = mightUser.MightData.GetSkill_Power(def);

                if (query.EqualsIgnoreCase(power.label.ToToolkit()) || query.Equals($"{def.defName}_power"))
                {
                    return TryLevelSkill(power, mightUser, out error);
                }

                MightPowerSkill efficiency = mightUser.MightData.GetSkill_Efficiency(def);

                if (query.EqualsIgnoreCase(efficiency.label.ToToolkit()) || query.Equals($"{def.defName}_efficiency"))
                {
                    return TryLevelSkill(efficiency, mightUser, out error);
                }

                MightPowerSkill versatility = mightUser.MightData.GetSkill_Versatility(def);

                if (query.EqualsIgnoreCase(versatility.label.ToToolkit()) || query.Equals($"{def.defName}_versatility"))
                {
                    return TryLevelSkill(versatility, mightUser, out error);
                }
            }

            error = "";

            return false;
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private static bool TryLevelMagic([NotNull] CompAbilityUserMagic magicUser, string query, out string error)
        {
            if (magicUser.MagicData.MagicAbilityPoints <= 0)
            {
                error = "No points to spend!";

                return false;
            }

            if (TryLevelGlobalMagicSkill(magicUser, query, out error))
            {
                return true;
            }

            return error.NullOrEmpty() && TryLevelMagicSkill(magicUser, query, out error);
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private static bool TryLevelGlobalMagicSkill(CompAbilityUserMagic magicUser, string query, out string error)
        {
            string regen = "TM_global_regen_pwr".Localize("regen").ToToolkit();

            if (query.EqualsIgnoreCase(regen) || query.EqualsIgnoreCase("regen"))
            {
                return TryLevelSkill(magicUser.MagicData.MagicPowerSkill_global_regen.FirstOrDefault(), magicUser, out error);
            }

            string efficiency = "TM_global_eff_pwr".Localize("efficiency").ToToolkit();

            if (query.EqualsIgnoreCase(efficiency) || query.EqualsIgnoreCase("efficiency"))
            {
                return TryLevelSkill(magicUser.MagicData.MagicPowerSkill_global_eff.FirstOrDefault(), magicUser, out error);
            }

            string spirit = "TM_global_spirit_pwr".Localize("versatility").ToToolkit();

            if (query.EqualsIgnoreCase(spirit) || query.EqualsIgnoreCase("versatility"))
            {
                return TryLevelSkill(magicUser.MagicData.MagicPowerSkill_global_spirit.FirstOrDefault(), magicUser, out error);
            }

            error = "";

            return false;
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private static bool TryLevelMagicSkill([NotNull] CompAbilityUserMagic magicUser, string query, out string error)
        {
            foreach (MagicPower magicPower in magicUser.MagicData.AllMagicPowers)
            {
                if (magicPower.learned || !(magicPower.abilityDef is TMAbilityDef def))
                {
                    continue;
                }

                if ((query.EqualsIgnoreCase(def.label.ToToolkit()) || query.Equals(def.defName)) && magicPower.level < magicPower.maxLevel
                    && magicUser.MagicData.MagicAbilityPoints >= magicPower.costToLevel)
                {
                    magicUser.MagicData.MagicAbilityPoints -= magicPower.costToLevel;
                    magicPower.level++;
                }

                MagicPowerSkill power = magicUser.MagicData.GetSkill_Power(def);

                if (query.EqualsIgnoreCase(power.label.ToToolkit()) || query.Equals($"{def.defName}_power"))
                {
                    return TryLevelSkill(power, magicUser, out error);
                }

                MagicPowerSkill efficiency = magicUser.MagicData.GetSkill_Efficiency(def);

                if (query.EqualsIgnoreCase(efficiency.label.ToToolkit()) || query.Equals($"{def.defName}_efficiency"))
                {
                    return TryLevelSkill(efficiency, magicUser, out error);
                }

                MagicPowerSkill versatility = magicUser.MagicData.GetSkill_Versatility(def);

                if (query.EqualsIgnoreCase(versatility.label.ToToolkit()) || query.Equals($"{def.defName}_versatility"))
                {
                    return TryLevelSkill(versatility, magicUser, out error);
                }
            }

            error = "";

            return false;
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private static bool TryLevelSkill([CanBeNull] MagicPowerSkill skill, [CanBeNull] CompAbilityUserMagic magicUser, out string error)
        {
            if (skill == null || magicUser == null)
            {
                error = "You shouldn't be seeing this error.";

                return false;
            }

            if (skill.level >= skill.levelMax)
            {
                error = $"{skill.label} can't be leveled anymore.";

                return false;
            }

            int points = magicUser.MagicData.MagicAbilityPoints;

            if (skill.costToLevel > points)
            {
                error = $"{skill.label} requires {skill.costToLevel:N0} points, but you only have {points:N0}";

                return false;
            }

            magicUser.MagicData.MagicAbilityPoints -= skill.costToLevel;
            skill.level++;
            error = null;

            return true;
        }

        [ContractAnnotation("=> false,error:notnull; => true,error:null")]
        private static bool TryLevelSkill([CanBeNull] MightPowerSkill skill, [CanBeNull] CompAbilityUserMight mightUser, out string error)
        {
            if (skill == null || mightUser == null)
            {
                error = "You shouldn't be seeing this error.";

                return false;
            }

            if (skill.level >= skill.levelMax)
            {
                error = $"{skill.label} can't be leveled anymore.";

                return false;
            }

            int points = mightUser.MightData.MightAbilityPoints;

            if (skill.costToLevel > points)
            {
                error = $"{skill.label} requires {skill.costToLevel:N0} points, but you only have {points:N0}";

                return false;
            }

            mightUser.MightData.MightAbilityPoints -= skill.costToLevel;
            skill.level++;
            error = null;

            return true;
        }
    }
}
