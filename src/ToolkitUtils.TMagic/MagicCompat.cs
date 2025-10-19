// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.TMagic. If not, see <https://www.gnu.org/licenses/>.
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Core;
using ToolkitUtils.Mod.Data;
using ToolkitUtils.Mod.Extensions;
using TorannMagic;
using TorannMagic.ModOptions;
using Verse;

namespace ToolkitUtils.TMagic;

[UsedImplicitly]
public class MagicCompat : ICompatibilityProvider
{
    public const string ModId = "Torann.ARimworldOfMagic";

    /// <inheritdoc />
    public string Id { get; init; } = "sirrandoo.tku:compatibility.magic";

    /// <inheritdoc />
    public string Name { get; init; } = "A RimWorld of Magic Compatibility Provider";

    /// <inheritdoc />
    public int Priority => 2;

    /// <inheritdoc />
    public string[] RequiredMods { get; } = [ModId,];

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

    public string GetSkillDescription(Viewer invoker, string query)
    {
        Pawn? pawn = ViewerPawnRegistry.Get(invoker.Id);

        if (pawn == null) return "TKUtils.NoPawn".TranslateSimple();

        var builder = new StringBuilder();
        var userMagic = pawn.TryGetComp<CompAbilityUserMagic>();

        if (userMagic is { IsMagicUser: true, } && TryGetMagicDescription(userMagic, query.ToToolkit(), out string? description))
        {
            builder.Append(UnicodeCharacter.CrystalBall.Codepoint);
            builder.Append(" ");
            builder.Append(description);
        }

        var userMight = pawn.TryGetComp<CompAbilityUserMight>();

        if (userMight is not { IsMightUser: true, } || !TryGetMightDescription(userMight, query.ToToolkit(), out description)) return builder.ToString();

        builder.Append(UnicodeCharacter.Dagger.Codepoint);
        builder.Append(" ");
        builder.Append(description);

        return builder.ToString();
    }

    private static bool TryGetMagicDescription(CompAbilityUserMagic userMagic, string query, [NotNullWhen(true)] out string? s) =>
        TryGetGlobalMagicDescription(userMagic, query, out s) || TryGetMagicSkillDescription(userMagic, query, out s);

    private static bool TryGetMightDescription(CompAbilityUserMight userMight, string query, [NotNullWhen(true)] out string? s) =>
        TryGetGlobalMightDescription(userMight, query, out s) || TryGetMightSkillDescription(userMight, query, out s);

    private static bool TryGetGlobalMagicDescription(CompAbilityUserMagic userMagic, string query, [NotNullWhen(true)] out string? s)
    {
        string regen = ((string)"TM_global_regen_pwr".TranslateWithBackup("regen")).ToToolkit();

        if (query.EqualsIgnoreCase(regen) || query.EqualsIgnoreCase("regen"))
        {
            s = userMagic.MagicData.MagicPowerSkill_global_regen.FirstOrDefault()?.desc.TranslateSimple();

            return s != null;
        }

        string efficiency = ((string)"TM_global_eff_pwr".TranslateWithBackup("efficiency")).ToToolkit();

        if (query.EqualsIgnoreCase(efficiency) || query.EqualsIgnoreCase("efficiency"))
        {
            s = userMagic.MagicData.MagicPowerSkill_global_eff.FirstOrDefault()?.desc.TranslateSimple();

            return s != null;
        }

        string spirit = ((string)"TM_global_spirit_pwr".TranslateWithBackup("versatility")).ToToolkit();

        if (query.EqualsIgnoreCase(spirit) || query.EqualsIgnoreCase("versatility"))
        {
            s = userMagic.MagicData.MagicPowerSkill_global_spirit.FirstOrDefault()?.desc.TranslateSimple();

            return s != null;
        }

        s = null;

        return false;
    }

    private static bool TryGetGlobalMightDescription(CompAbilityUserMight userMight, string query, [NotNullWhen(true)] out string? s)
    {
        string refresh = ((string)"TM_global_refresh_pwr".TranslateWithBackup("refresh")).ToToolkit();

        if (query.EqualsIgnoreCase(refresh) || query.EqualsIgnoreCase("refresh"))
        {
            s = userMight.MightData.MightPowerSkill_global_refresh.FirstOrDefault()?.desc.Translate();

            return s != null;
        }

        string efficiency = ((string)"TM_global_seff_pwr".TranslateWithBackup("efficiency")).ToToolkit();

        if (query.EqualsIgnoreCase(efficiency) || query.EqualsIgnoreCase("efficiency"))
        {
            s = userMight.MightData.MightPowerSkill_global_seff.FirstOrDefault()?.desc.TranslateSimple();

            return s != null;
        }

        string strength = ((string)"TM_global_strength_pwr".TranslateWithBackup("strength")).ToToolkit();

        if (query.EqualsIgnoreCase(strength) || query.EqualsIgnoreCase("strength"))
        {
            s = userMight.MightData.MightPowerSkill_global_strength.FirstOrDefault()?.desc.TranslateSimple();

            return s != null;
        }

        string endurance = ((string)"TM_global_endurance_pwr".TranslateWithBackup("endurance")).ToToolkit();

        if (query.EqualsIgnoreCase(endurance) || query.EqualsIgnoreCase("endurance"))
        {
            s = userMight.MightData.MightPowerSkill_global_endurance.FirstOrDefault()?.desc.TranslateSimple();

            return s != null;
        }

        s = null;

        return false;
    }

    private static bool TryGetMagicSkillDescription(CompAbilityUserMagic userMagic, string query, [NotNullWhen(true)] out string? s)
    {
        foreach (MagicPower magicPower in userMagic.MagicData.AllMagicPowers)
        {
            if (!magicPower.learned || magicPower.abilityDef is not TMAbilityDef def) continue;

            if (query.EqualsIgnoreCase(def.label.ToToolkit()) || query.Equals(def.defName))
            {
                s = def.description;

                return s != null;
            }

            MagicPowerSkill power = userMagic.MagicData.GetSkill_Power(def);

            if (query.EqualsIgnoreCase(power.label.ToToolkit()) || query.Equals($"{def.defName}_power"))
            {
                s = power.desc.TranslateSimple();

                return s != null;
            }

            MagicPowerSkill efficiency = userMagic.MagicData.GetSkill_Efficiency(def);

            if (query.EqualsIgnoreCase(efficiency.label.ToToolkit()) || query.Equals($"{def.defName}_efficiency"))
            {
                s = efficiency.desc.TranslateSimple();

                return s != null;
            }

            MagicPowerSkill versatility = userMagic.MagicData.GetSkill_Versatility(def);

            if (query.EqualsIgnoreCase(versatility.label.ToToolkit()) || query.Equals($"{def.defName}_versatility"))
            {
                s = versatility.desc.TranslateSimple();

                return s != null;
            }
        }

        s = null;

        return false;
    }

    private static bool TryGetMightSkillDescription(CompAbilityUserMight userMight, string query, [NotNullWhen(true)] out string? s)
    {
        foreach (MightPower mightPower in userMight.MightData.AllMightPowers)
        {
            if (!mightPower.learned || mightPower.abilityDef is not TMAbilityDef def) continue;

            if (query.EqualsIgnoreCase(def.label.ToToolkit()) || query.Equals(def.defName))
            {
                s = def.description;

                return s != null;
            }

            MightPowerSkill power = userMight.MightData.GetSkill_Power(def);

            if (query.EqualsIgnoreCase(power.label.ToToolkit()) || query.Equals($"{def.defName}_power"))
            {
                s = power.desc.TranslateSimple();

                return s != null;
            }

            MightPowerSkill efficiency = userMight.MightData.GetSkill_Efficiency(def);

            if (query.EqualsIgnoreCase(efficiency.label.ToToolkit()) || query.Equals($"{def.defName}_efficiency"))
            {
                s = efficiency.desc.TranslateSimple();

                return s != null;
            }

            MightPowerSkill versatility = userMight.MightData.GetSkill_Versatility(def);

            if (query.EqualsIgnoreCase(versatility.label.ToToolkit()) || query.Equals($"{def.defName}_versatility"))
            {
                s = versatility.desc.TranslateSimple();

                return s != null;
            }
        }

        s = null;

        return false;
    }
}
