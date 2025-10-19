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
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.TMagic.Models;
using TorannMagic;
using TorannMagic.TMDefs;
using Verse;

namespace ToolkitUtils.TMagic;

[UsedImplicitly]
[StaticConstructorOnStartup]
public static class ClassData
{
    private static readonly Dictionary<TraitDef, IReadOnlyList<ITorannPawnPower>> BaseClassPowers = GetBaseClassPowerList().ToDictionary(
        keySelector: p => p.Item1,
        elementSelector: p => p.Item2
    );

    [UsedImplicitly] private static readonly List<Class> Classes = GenerateClassTrees().ToList();

    private static Tuple<TraitDef, IReadOnlyList<ITorannPawnPower>> PowerPair(TraitDef trait, IReadOnlyList<ITorannPawnPower> abilities) => new(trait, abilities);

    private static IEnumerable<Tuple<TraitDef, IReadOnlyList<ITorannPawnPower>>> GetBaseClassPowerList()
    {
        yield return PowerPair(TorannMagicDefOf.TM_Wanderer, GetWandererPowers());
        yield return PowerPair(TorannMagicDefOf.TM_Wayfarer, GetWandererPowers());
        yield return PowerPair(TorannMagicDefOf.InnerFire, GetInnerFirePowers());
        yield return PowerPair(TorannMagicDefOf.HeartOfFrost, GetHeartOfFrostPowers());
        yield return PowerPair(TorannMagicDefOf.StormBorn, GetStormBornPowers());
        yield return PowerPair(TorannMagicDefOf.Arcanist, GetArcanistPowers());
        yield return PowerPair(TorannMagicDefOf.Paladin, GetPaladinPowers());
        yield return PowerPair(TorannMagicDefOf.Summoner, GetSummonerPowers());
        yield return PowerPair(TorannMagicDefOf.Druid, GetDruidPowers());
        yield return PowerPair(TorannMagicDefOf.Necromancer, GetNecromancerPowers());
        yield return PowerPair(TorannMagicDefOf.Lich, GetNecromancerPowers());
        yield return PowerPair(TorannMagicDefOf.Priest, GetPriestPowers());
        yield return PowerPair(TorannMagicDefOf.TM_Bard, GetBardPowers());
        yield return PowerPair(TorannMagicDefOf.Succubus, GetSuccubusPowers());
        yield return PowerPair(TorannMagicDefOf.Warlock, GetWarlockPowers());
        yield return PowerPair(TorannMagicDefOf.Geomancer, GetGeomancerPowers());
        yield return PowerPair(TorannMagicDefOf.Technomancer, GetTechnomancerPowers());
        yield return PowerPair(TorannMagicDefOf.BloodMage, GetBloodMagePowers());
        yield return PowerPair(TorannMagicDefOf.Enchanter, GetEnchanterPowers());
        yield return PowerPair(TorannMagicDefOf.Chronomancer, GetChronomancerPowers());
        yield return PowerPair(TorannMagicDefOf.ChaosMage, GetChaosMagePowers());
    }

    private static IReadOnlyList<ITorannPawnPower> GetChaosMagePowers()
    {
        IReadOnlyList<ITorannPawnPower>? defs = GetRawPowersForBaseClass(TorannMagicDefOf.ChaosMage);

        if (defs != null) return defs;

        var list = new List<ITorannPawnPower>
        {
            new ClassPower.Builder(TorannMagicDefOf.TM_ChaosTradition).WithTier(tier: 0, TorannMagicDefOf.TM_ChaosTradition).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_WandererCraft).WithTier(tier: 0, TorannMagicDefOf.TM_WandererCraft).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Cantrips).WithTier(tier: 0, TorannMagicDefOf.TM_Cantrips).Build(),
        };

        list.AddRange(GetInnerFirePowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Firestorm)));
        list.AddRange(GetHeartOfFrostPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Rainmaker) || !IsAbility(p, TorannMagicDefOf.TM_Blizzard)));
        list.AddRange(GetStormBornPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_EyeOfTheStorm)));
        list.AddRange(GetArcanistPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_FoldReality)));
        list.AddRange(GetPaladinPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_HolyWrath)));
        list.AddRange(GetSummonerPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_SummonPoppi)));
        list.AddRange(GetDruidPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_RegrowLimb)));
        list.AddRange(
            GetNecromancerPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_RaiseUndead)
                                           || !IsAbility(p, TorannMagicDefOf.TM_LichForm)
                                           || !IsAbility(p, TorannMagicDefOf.TM_DeathBolt)
            )
        );
        list.AddRange(GetPriestPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Resurrection)));
        list.AddRange(
            GetBardPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_BardTraining)
                                    || !IsAbility(p, TorannMagicDefOf.TM_Inspire)
                                    || !IsAbility(p, TorannMagicDefOf.TM_Entertain)
                                    || !IsAbility(p, TorannMagicDefOf.TM_BattleHymn)
            )
        );
        list.AddRange(
            GetWarlockPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_SoulBond)
                                       || !IsAbility(p, TorannMagicDefOf.TM_ShadowBolt)
                                       || !IsAbility(p, TorannMagicDefOf.TM_Dominate)
                                       || !IsAbility(p, TorannMagicDefOf.TM_Scorn)
            )
        );
        list.AddRange(GetGeomancerPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Meteor)));
        list.AddRange(GetTechnomancerPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_OrbitalStrike)));
        list.AddRange(GetEnchanterPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Shapeshift)));
        list.AddRange(GetChronomancerPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Recall)));

        return list;
    }

    private static IReadOnlyList<ITorannPawnPower> GetChronomancerPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Chronomancer);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_Prediction).WithTier(tier: 0, TorannMagicDefOf.TM_Prediction).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_AlterFate).WithTier(tier: 0, TorannMagicDefOf.TM_AlterFate).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_AccelerateTime).WithTier(tier: 0, TorannMagicDefOf.TM_AccelerateTime).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_ReverseTime).WithTier(tier: 0, TorannMagicDefOf.TM_ReverseTime).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_ChronostaticField).WithTier(tier: 0, TorannMagicDefOf.TM_ChronostaticField)
                                                                         .WithTier(tier: 1, TorannMagicDefOf.TM_ChronostaticField_I)
                                                                         .WithTier(tier: 2, TorannMagicDefOf.TM_ChronostaticField_II)
                                                                         .WithTier(tier: 3, TorannMagicDefOf.TM_ChronostaticField_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Recall).WithTier(tier: 0, TorannMagicDefOf.TM_Recall).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetEnchanterPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Enchanter);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_EnchantedAura).WithTier(tier: 0, TorannMagicDefOf.TM_EnchantedAura).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_EnchantedBody).WithTier(tier: 0, TorannMagicDefOf.TM_EnchantedBody).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Transmutate).WithTier(tier: 0, TorannMagicDefOf.TM_Transmutate).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_EnchanterStone).WithTier(tier: 0, TorannMagicDefOf.TM_EnchanterStone).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_EnchantWeapon).WithTier(tier: 0, TorannMagicDefOf.TM_EnchantWeapon).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Polymorph).WithTier(tier: 0, TorannMagicDefOf.TM_Polymorph).WithTier(tier: 1, TorannMagicDefOf.TM_Polymorph_I)
                                                                 .WithTier(tier: 2, TorannMagicDefOf.TM_Polymorph_II).WithTier(tier: 3, TorannMagicDefOf.TM_Polymorph_III)
                                                                 .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Shapeshift).WithTier(tier: 0, TorannMagicDefOf.TM_Shapeshift).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetBloodMagePowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.BloodMage);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_BloodGift).WithTier(tier: 0, TorannMagicDefOf.TM_BloodGift).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_IgniteBlood).WithTier(tier: 0, TorannMagicDefOf.TM_IgniteBlood).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_BloodForBlood).WithTier(tier: 0, TorannMagicDefOf.TM_BloodForBlood).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_BloodShield).WithTier(tier: 0, TorannMagicDefOf.TM_BloodShield).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Rend).WithTier(tier: 0, TorannMagicDefOf.TM_Rend).WithTier(tier: 1, TorannMagicDefOf.TM_Rend_I)
                                                            .WithTier(tier: 2, TorannMagicDefOf.TM_Rend_II).WithTier(tier: 3, TorannMagicDefOf.TM_Rend_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_BloodMoon).WithTier(tier: 0, TorannMagicDefOf.TM_BloodMoon).WithTier(tier: 1, TorannMagicDefOf.TM_BloodMoon_I)
                                                                 .WithTier(tier: 2, TorannMagicDefOf.TM_BloodMoon_II).WithTier(tier: 3, TorannMagicDefOf.TM_BloodMoon_III)
                                                                 .Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetTechnomancerPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Technomancer);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_TechnoShield).WithTier(tier: 0, TorannMagicDefOf.TM_TechnoShield).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Sabotage).WithTier(tier: 0, TorannMagicDefOf.TM_Sabotage).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Overdrive).WithTier(tier: 0, TorannMagicDefOf.TM_Overdrive).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_OrbitalStrike).WithTier(tier: 0, TorannMagicDefOf.TM_OrbitalStrike)
                                                                     .WithTier(tier: 1, TorannMagicDefOf.TM_OrbitalStrike_I)
                                                                     .WithTier(tier: 2, TorannMagicDefOf.TM_OrbitalStrike_II)
                                                                     .WithTier(tier: 3, TorannMagicDefOf.TM_OrbitalStrike_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_TechnoBit).WithTier(tier: 0, TorannMagicDefOf.TM_TechnoBit).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_TechnoTurret).WithTier(tier: 0, TorannMagicDefOf.TM_TechnoTurret).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_TechnoWeapon).WithTier(tier: 0, TorannMagicDefOf.TM_TechnoWeapon).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetGeomancerPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Geomancer);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_Stoneskin).WithTier(tier: 0, TorannMagicDefOf.TM_Stoneskin).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Encase).WithTier(tier: 0, TorannMagicDefOf.TM_Encase).WithTier(tier: 1, TorannMagicDefOf.TM_Encase_I)
                                                              .WithTier(tier: 2, TorannMagicDefOf.TM_Encase_II).WithTier(tier: 3, TorannMagicDefOf.TM_Encase_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_EarthSprites).WithTier(tier: 0, TorannMagicDefOf.TM_EarthSprites).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_EarthernHammer).WithTier(tier: 0, TorannMagicDefOf.TM_EarthernHammer).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Sentinel).WithTier(tier: 0, TorannMagicDefOf.TM_Sentinel).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Meteor).WithTier(tier: 0, TorannMagicDefOf.TM_Meteor).WithTier(tier: 1, TorannMagicDefOf.TM_Meteor_I)
                                                              .WithTier(tier: 2, TorannMagicDefOf.TM_Meteor_II).WithTier(tier: 3, TorannMagicDefOf.TM_Meteor_III).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetWarlockPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Warlock);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_SoulBond).WithTier(tier: 0, TorannMagicDefOf.TM_SoulBond).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_ShadowBolt).WithTier(tier: 0, TorannMagicDefOf.TM_ShadowBolt).WithTier(tier: 1, TorannMagicDefOf.TM_ShadowBolt_I)
                                                                  .WithTier(tier: 2, TorannMagicDefOf.TM_ShadowBolt_II).WithTier(tier: 3, TorannMagicDefOf.TM_ShadowBolt_III)
                                                                  .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Dominate).WithTier(tier: 0, TorannMagicDefOf.TM_Dominate).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Repulsion).WithTier(tier: 0, TorannMagicDefOf.TM_Repulsion).WithTier(tier: 1, TorannMagicDefOf.TM_Repulsion_I)
                                                                 .WithTier(tier: 2, TorannMagicDefOf.TM_Repulsion_II).WithTier(tier: 3, TorannMagicDefOf.TM_Repulsion_III)
                                                                 .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_PsychicShock).WithTier(tier: 0, TorannMagicDefOf.TM_PsychicShock).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetSuccubusPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Succubus);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_SoulBond).WithTier(tier: 0, TorannMagicDefOf.TM_SoulBond).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_ShadowBolt).WithTier(tier: 0, TorannMagicDefOf.TM_ShadowBolt).WithTier(tier: 1, TorannMagicDefOf.TM_ShadowBolt_I)
                                                                  .WithTier(tier: 2, TorannMagicDefOf.TM_ShadowBolt_II).WithTier(tier: 3, TorannMagicDefOf.TM_ShadowBolt_III)
                                                                  .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Dominate).WithTier(tier: 0, TorannMagicDefOf.TM_Dominate).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Attraction).WithTier(tier: 0, TorannMagicDefOf.TM_Attraction).WithTier(tier: 1, TorannMagicDefOf.TM_Attraction_I)
                                                                  .WithTier(tier: 2, TorannMagicDefOf.TM_Attraction_II).WithTier(tier: 3, TorannMagicDefOf.TM_Attraction_III)
                                                                  .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Scorn).WithTier(tier: 0, TorannMagicDefOf.TM_Scorn).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetBardPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.TM_Bard);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_BardTraining).WithTier(tier: 0, TorannMagicDefOf.TM_BardTraining).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Inspire).WithTier(tier: 0, TorannMagicDefOf.TM_Inspire).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Entertain).WithTier(tier: 0, TorannMagicDefOf.TM_Entertain).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Lullaby).WithTier(tier: 0, TorannMagicDefOf.TM_Lullaby).WithTier(tier: 1, TorannMagicDefOf.TM_Lullaby_I)
                                                               .WithTier(tier: 2, TorannMagicDefOf.TM_Lullaby_II).WithTier(tier: 3, TorannMagicDefOf.TM_Lullaby_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_BattleHymn).WithTier(tier: 0, TorannMagicDefOf.TM_BattleHymn).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetPriestPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Priest);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_AdvancedHeal).WithTier(tier: 0, TorannMagicDefOf.TM_AdvancedHeal).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Purify).WithTier(tier: 0, TorannMagicDefOf.TM_Purify).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_HealingCircle).WithTier(tier: 0, TorannMagicDefOf.TM_HealingCircle)
                                                                     .WithTier(tier: 1, TorannMagicDefOf.TM_HealingCircle_I)
                                                                     .WithTier(tier: 2, TorannMagicDefOf.TM_HealingCircle_II)
                                                                     .WithTier(tier: 3, TorannMagicDefOf.TM_HealingCircle_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_BestowMight).WithTier(tier: 0, TorannMagicDefOf.TM_BestowMight).WithTier(tier: 1, TorannMagicDefOf.TM_BestowMight_I)
                                                                   .WithTier(tier: 2, TorannMagicDefOf.TM_BestowMight_II).WithTier(tier: 3, TorannMagicDefOf.TM_BestowMight_III)
                                                                   .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Resurrection).WithTier(tier: 0, TorannMagicDefOf.TM_Resurrection).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetNecromancerPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Necromancer);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_RaiseUndead).WithTier(tier: 0, TorannMagicDefOf.TM_RaiseUndead).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_DeathMark).WithTier(tier: 0, TorannMagicDefOf.TM_DeathMark).WithTier(tier: 1, TorannMagicDefOf.TM_DeathMark_I)
                                                                 .WithTier(tier: 2, TorannMagicDefOf.TM_DeathMark_II).WithTier(tier: 3, TorannMagicDefOf.TM_DeathMark_III)
                                                                 .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_FogOfTorment).WithTier(tier: 0, TorannMagicDefOf.TM_FogOfTorment).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_ConsumeCorpse).WithTier(tier: 0, TorannMagicDefOf.TM_ConsumeCorpse)
                                                                     .WithTier(tier: 1, TorannMagicDefOf.TM_ConsumeCorpse_I)
                                                                     .WithTier(tier: 2, TorannMagicDefOf.TM_ConsumeCorpse_II)
                                                                     .WithTier(tier: 3, TorannMagicDefOf.TM_ConsumeCorpse_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_CorpseExplosion).WithTier(tier: 0, TorannMagicDefOf.TM_CorpseExplosion)
                                                                       .WithTier(tier: 1, TorannMagicDefOf.TM_CorpseExplosion_I)
                                                                       .WithTier(tier: 2, TorannMagicDefOf.TM_CorpseExplosion_II)
                                                                       .WithTier(tier: 3, TorannMagicDefOf.TM_CorpseExplosion_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_LichForm).WithTier(tier: 0, TorannMagicDefOf.TM_LichForm).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_DeathBolt).WithTier(tier: 0, TorannMagicDefOf.TM_DeathBolt).WithTier(tier: 1, TorannMagicDefOf.TM_DeathBolt_I)
                                                                 .WithTier(tier: 2, TorannMagicDefOf.TM_DeathBolt_II).WithTier(tier: 3, TorannMagicDefOf.TM_DeathBolt_III)
                                                                 .Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetDruidPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Druid);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_Poison).WithTier(tier: 0, TorannMagicDefOf.TM_Poison).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SootheAnimal).WithTier(tier: 0, TorannMagicDefOf.TM_SootheAnimal).WithTier(tier: 1, TorannMagicDefOf.TM_SootheAnimal_I)
                                                                    .WithTier(tier: 2, TorannMagicDefOf.TM_SootheAnimal_II)
                                                                    .WithTier(tier: 3, TorannMagicDefOf.TM_SootheAnimal_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Regenerate).WithTier(tier: 0, TorannMagicDefOf.TM_Regenerate).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_CureDisease).WithTier(tier: 0, TorannMagicDefOf.TM_CureDisease).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_RegrowLimb).WithTier(tier: 0, TorannMagicDefOf.TM_RegrowLimb).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetSummonerPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Summoner);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_SummonMinion).WithTier(tier: 0, TorannMagicDefOf.TM_SummonMinion).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SummonPylon).WithTier(tier: 0, TorannMagicDefOf.TM_SummonPylon).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SummonExplosive).WithTier(tier: 0, TorannMagicDefOf.TM_SummonExplosive).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SummonElemental).WithTier(tier: 0, TorannMagicDefOf.TM_SummonElemental).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SummonPoppi).WithTier(tier: 0, TorannMagicDefOf.TM_SummonPoppi).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetPaladinPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Paladin);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_P_RayofHope).WithTier(tier: 0, TorannMagicDefOf.TM_P_RayofHope).WithTier(tier: 1, TorannMagicDefOf.TM_P_RayofHope_I)
                                                                   .WithTier(tier: 2, TorannMagicDefOf.TM_P_RayofHope_II).WithTier(tier: 3, TorannMagicDefOf.TM_P_RayofHope_III)
                                                                   .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Heal).WithTier(tier: 0, TorannMagicDefOf.TM_Heal).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Shield).WithTier(tier: 0, TorannMagicDefOf.TM_Shield).WithTier(tier: 1, TorannMagicDefOf.TM_Shield_I)
                                                              .WithTier(tier: 2, TorannMagicDefOf.TM_Shield_II).WithTier(tier: 3, TorannMagicDefOf.TM_Shield_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_ValiantCharge).WithTier(tier: 0, TorannMagicDefOf.TM_ValiantCharge).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Overwhelm).WithTier(tier: 0, TorannMagicDefOf.TM_Overwhelm).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_HolyWrath).WithTier(tier: 0, TorannMagicDefOf.TM_HolyWrath).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetArcanistPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.Arcanist);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_Shadow).WithTier(tier: 0, TorannMagicDefOf.TM_Shadow).WithTier(tier: 1, TorannMagicDefOf.TM_Shadow_I)
                                                              .WithTier(tier: 2, TorannMagicDefOf.TM_Shadow_II).WithTier(tier: 3, TorannMagicDefOf.TM_Shadow_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_MagicMissile).WithTier(tier: 0, TorannMagicDefOf.TM_MagicMissile).WithTier(tier: 1, TorannMagicDefOf.TM_MagicMissile_I)
                                                                    .WithTier(tier: 2, TorannMagicDefOf.TM_MagicMissile_II)
                                                                    .WithTier(tier: 3, TorannMagicDefOf.TM_MagicMissile_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Blink).WithTier(tier: 0, TorannMagicDefOf.TM_Blink).WithTier(tier: 1, TorannMagicDefOf.TM_Blink_I)
                                                             .WithTier(tier: 2, TorannMagicDefOf.TM_Blink_II).WithTier(tier: 3, TorannMagicDefOf.TM_Blink_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Summon).WithTier(tier: 0, TorannMagicDefOf.TM_Summon).WithTier(tier: 1, TorannMagicDefOf.TM_Summon_I)
                                                              .WithTier(tier: 2, TorannMagicDefOf.TM_Summon_II).WithTier(tier: 3, TorannMagicDefOf.TM_Summon_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Teleport).WithTier(tier: 0, TorannMagicDefOf.TM_Teleport).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_FoldReality).WithTier(tier: 0, TorannMagicDefOf.TM_FoldReality).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetStormBornPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.StormBorn);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_AMP).WithTier(tier: 0, TorannMagicDefOf.TM_AMP).WithTier(tier: 1, TorannMagicDefOf.TM_AMP_I)
                                                           .WithTier(tier: 2, TorannMagicDefOf.TM_AMP_II).WithTier(tier: 3, TorannMagicDefOf.TM_AMP_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_LightningBolt).WithTier(tier: 0, TorannMagicDefOf.TM_LightningBolt).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_LightningCloud).WithTier(tier: 0, TorannMagicDefOf.TM_LightningCloud).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_LightningStorm).WithTier(tier: 0, TorannMagicDefOf.TM_LightningStorm).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_EyeOfTheStorm).WithTier(tier: 0, TorannMagicDefOf.TM_EyeOfTheStorm).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetHeartOfFrostPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.HeartOfFrost);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_Soothe).WithTier(tier: 0, TorannMagicDefOf.TM_Soothe).WithTier(tier: 1, TorannMagicDefOf.TM_Soothe_I)
                                                              .WithTier(tier: 2, TorannMagicDefOf.TM_Soothe_II).WithTier(tier: 3, TorannMagicDefOf.TM_Soothe_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Icebolt).WithTier(tier: 0, TorannMagicDefOf.TM_Icebolt).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Snowball).WithTier(tier: 0, TorannMagicDefOf.TM_Snowball).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_FrostRay).WithTier(tier: 0, TorannMagicDefOf.TM_FrostRay).WithTier(tier: 1, TorannMagicDefOf.TM_FrostRay_I)
                                                                .WithTier(tier: 2, TorannMagicDefOf.TM_FrostRay_II).WithTier(tier: 3, TorannMagicDefOf.TM_FrostRay_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Rainmaker).WithTier(tier: 0, TorannMagicDefOf.TM_Rainmaker).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Blizzard).WithTier(tier: 0, TorannMagicDefOf.TM_Blizzard).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetInnerFirePowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.InnerFire);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_RayofHope).WithTier(tier: 0, TorannMagicDefOf.TM_RayofHope).WithTier(tier: 1, TorannMagicDefOf.TM_RayofHope_I)
                                                                 .WithTier(tier: 2, TorannMagicDefOf.TM_RayofHope_II).WithTier(tier: 3, TorannMagicDefOf.TM_RayofHope_III)
                                                                 .Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Firebolt).WithTier(tier: 0, TorannMagicDefOf.TM_Firebolt).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Fireclaw).WithTier(tier: 0, TorannMagicDefOf.TM_Fireclaw).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Fireball).WithTier(tier: 0, TorannMagicDefOf.TM_Fireball).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Firestorm).WithTier(tier: 0, TorannMagicDefOf.TM_Firestorm).Build(),
        ];
    }

    private static IReadOnlyList<ITorannPawnPower> GetWandererPowers()
    {
        IReadOnlyList<ITorannPawnPower>? list = GetRawPowersForBaseClass(TorannMagicDefOf.TM_Wanderer);

        if (list != null) return list;

        return
        [
            new ClassPower.Builder(TorannMagicDefOf.TM_TransferMana).WithTier(tier: 0, TorannMagicDefOf.TM_TransferMana).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SiphonMana).WithTier(tier: 0, TorannMagicDefOf.TM_SiphonMana).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_DirtDevil).WithTier(tier: 0, TorannMagicDefOf.TM_DirtDevil).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Heater).WithTier(tier: 0, TorannMagicDefOf.TM_Heater).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Cooler).WithTier(tier: 0, TorannMagicDefOf.TM_Cooler).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_PowerNode).WithTier(tier: 0, TorannMagicDefOf.TM_PowerNode).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Sunlight).WithTier(tier: 0, TorannMagicDefOf.TM_Sunlight).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SmokeCloud).WithTier(tier: 0, TorannMagicDefOf.TM_SmokeCloud).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Extinguish).WithTier(tier: 0, TorannMagicDefOf.TM_Extinguish).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_EMP).WithTier(tier: 0, TorannMagicDefOf.TM_EMP).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_ManaShield).WithTier(tier: 0, TorannMagicDefOf.TM_ManaShield).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Blur).WithTier(tier: 0, TorannMagicDefOf.TM_Blur).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_ArcaneBolt).WithTier(tier: 0, TorannMagicDefOf.TM_ArcaneBolt).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_LightningTrap).WithTier(tier: 0, TorannMagicDefOf.TM_LightningTrap).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Invisibility).WithTier(tier: 0, TorannMagicDefOf.TM_Invisibility).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_MageLight).WithTier(tier: 0, TorannMagicDefOf.TM_MageLight).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Ignite).WithTier(tier: 0, TorannMagicDefOf.TM_Ignite).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SnapFreeze).WithTier(tier: 0, TorannMagicDefOf.TM_SnapFreeze).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Heal).WithTier(tier: 0, TorannMagicDefOf.TM_Heal).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Blink).WithTier(tier: 0, TorannMagicDefOf.TM_Blink).WithTier(tier: 1, TorannMagicDefOf.TM_Blink_I)
                                                             .WithTier(tier: 2, TorannMagicDefOf.TM_Blink_II).WithTier(tier: 3, TorannMagicDefOf.TM_Blink_III).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Rainmaker).WithTier(tier: 0, TorannMagicDefOf.TM_Rainmaker).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_SummonMinion).WithTier(tier: 0, TorannMagicDefOf.TM_SummonMinion).Build(),
            new ClassPower.Builder(TorannMagicDefOf.TM_Teleport).WithTier(tier: 0, TorannMagicDefOf.TM_Teleport).Build(),
        ];
    }

    private static IEnumerable<Class> GenerateClassTrees()
    {
        foreach (TraitDef trait in TM_Data.AllClassTraits) yield return GenerateClassTree(trait);

        yield return GenerateClassTree(TorannMagicDefOf.DeathKnight);
    }

    private static Class GenerateClassTree(TraitDef trait) => new(trait, GetAbilitiesFor(trait).ToList());

    private static IEnumerable<ITorannPawnPower> GetAbilitiesFor(TraitDef trait)
    {
        if (BaseClassPowers.TryGetValue(trait, out IReadOnlyList<ITorannPawnPower> powers))
        {
            foreach (ITorannPawnPower ability in powers) yield return ability;
        }

        TM_CustomClass @class = TM_ClassUtility.CustomClasses.Find(c => c.classTrait == trait);

        if (@class == null) yield break;

        var lastDefName = "";
        ClassPower.Builder? builder = null;

        foreach (TMAbilityDef ability in @class.classAbilities.OrderBy(i => i.defName))
        {
            int tier = ability.defName.LastIndexOf('_') > 0 ? ability.defName[(ability.defName.LastIndexOf('_') + 1)..].Count(c => c == 'I') : 0;

            if (lastDefName.NullOrEmpty() || builder == null)
            {
                builder = new ClassPower.Builder(ability).WithTier(tier, ability);
                lastDefName = ability.defName;
            }
            else if (ability.defName.StartsWith(lastDefName))
            {
                builder.WithTier(tier, ability);
                lastDefName = ability.defName;
            }
            else
            {
                builder.WithTier(tier, ability);
                lastDefName = ability.defName;
            }
        }

        if (builder == null) yield break;

        yield return builder.Build();
    }

    private static IReadOnlyList<ITorannPawnPower>? GetRawPowersForBaseClass(TraitDef trait) => BaseClassPowers.GetValueOrDefault(trait);

    private static bool IsAbility(ITorannPawnPower ability, Def def) => ability.Name == (def.label ?? def.defName);
}
