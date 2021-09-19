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

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Models;
using TorannMagic;
using TorannMagic.TMDefs;
using Verse;
using Ability = SirRandoo.ToolkitUtils.Models.Ability;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class ClassData
    {
        private static readonly List<Class> Classes;
        private static readonly Dictionary<TraitDef, List<TMAbilityDef>> BaseClassPowers;

        static ClassData()
        {
            BaseClassPowers = new Dictionary<TraitDef, List<TMAbilityDef>>();
            foreach ((TraitDef @class, List<TMAbilityDef> powers) in GetBaseClassPowerList())
            {
                BaseClassPowers.Add(@class, powers);
            }

            Classes = new List<Class>(GenerateClassTrees());
        }

        [NotNull]
        private static Tuple<TraitDef, List<TMAbilityDef>> PowerPair(TraitDef trait, List<TMAbilityDef> abilities)
        {
            return new Tuple<TraitDef, List<TMAbilityDef>>(trait, abilities);
        }

        [NotNull]
        [ItemNotNull]
        private static IEnumerable<Tuple<TraitDef, List<TMAbilityDef>>> GetBaseClassPowerList()
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

        [NotNull]
        private static List<TMAbilityDef> GetChaosMagePowers()
        {
            List<TMAbilityDef> defs = GetRawPowersForBaseClass(TorannMagicDefOf.ChaosMage);
            if (defs != null)
            {
                return defs;
            }

            var list = new List<TMAbilityDef>
            {
                TorannMagicDefOf.TM_ChaosTradition, TorannMagicDefOf.TM_WandererCraft, TorannMagicDefOf.TM_Cantrips
            };

            list.AddRange(GetInnerFirePowers().Where(p => p != TorannMagicDefOf.TM_Firestorm));
            list.AddRange(
                GetHeartOfFrostPowers()
                   .Where(p => p != TorannMagicDefOf.TM_Rainmaker || p != TorannMagicDefOf.TM_Blizzard)
            );
            list.AddRange(GetStormBornPowers().Where(p => p != TorannMagicDefOf.TM_EyeOfTheStorm));
            list.AddRange(GetArcanistPowers().Where(p => p != TorannMagicDefOf.TM_FoldReality));
            list.AddRange(GetPaladinPowers().Where(p => p != TorannMagicDefOf.TM_HolyWrath));
            list.AddRange(GetSummonerPowers().Where(p => p != TorannMagicDefOf.TM_SummonPoppi));
            list.AddRange(GetDruidPowers().Where(p => p != TorannMagicDefOf.TM_RegrowLimb));
            list.AddRange(
                GetNecromancerPowers()
                   .Where(
                        p => p != TorannMagicDefOf.TM_RaiseUndead
                             || p != TorannMagicDefOf.TM_LichForm
                             || p != TorannMagicDefOf.TM_DeathBolt
                             || p != TorannMagicDefOf.TM_DeathBolt_I
                             || p != TorannMagicDefOf.TM_DeathBolt_II
                             || p != TorannMagicDefOf.TM_DeathBolt_III
                    )
            );
            list.AddRange(GetPriestPowers().Where(p => p != TorannMagicDefOf.TM_Resurrection));
            list.AddRange(
                GetBardPowers()
                   .Where(
                        p => p != TorannMagicDefOf.TM_BardTraining
                             || p != TorannMagicDefOf.TM_Inspire
                             || p != TorannMagicDefOf.TM_Entertain
                             || p != TorannMagicDefOf.TM_BattleHymn
                    )
            );
            list.AddRange(
                GetWarlockPowers()
                   .Where(
                        p => p != TorannMagicDefOf.TM_SoulBond
                             || p != TorannMagicDefOf.TM_ShadowBolt
                             || p != TorannMagicDefOf.TM_Dominate
                             || p != TorannMagicDefOf.TM_Scorn
                    )
            );
            list.AddRange(GetGeomancerPowers().Where(p => p != TorannMagicDefOf.TM_Meteor));
            list.AddRange(
                GetTechnomancerPowers()
                   .Where(
                        p => p != TorannMagicDefOf.TM_OrbitalStrike
                             || p != TorannMagicDefOf.TM_OrbitalStrike_I
                             || p != TorannMagicDefOf.TM_OrbitalStrike_II
                             || p != TorannMagicDefOf.TM_OrbitalStrike_III
                    )
            );
            list.AddRange(GetEnchanterPowers().Where(p => p != TorannMagicDefOf.TM_Shapeshift));
            list.AddRange(GetChronomancerPowers().Where(p => p != TorannMagicDefOf.TM_Recall));

            return list;
        }

        [NotNull]
        private static List<TMAbilityDef> GetChronomancerPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Chronomancer)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_Prediction,
                       TorannMagicDefOf.TM_AlterFate,
                       TorannMagicDefOf.TM_AccelerateTime,
                       TorannMagicDefOf.TM_ReverseTime,
                       TorannMagicDefOf.TM_ChronostaticField,
                       TorannMagicDefOf.TM_ChronostaticField_I,
                       TorannMagicDefOf.TM_ChronostaticField_II,
                       TorannMagicDefOf.TM_ChronostaticField_III,
                       TorannMagicDefOf.TM_Recall
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetEnchanterPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Enchanter)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_EnchantedAura,
                       TorannMagicDefOf.TM_EnchantedBody,
                       TorannMagicDefOf.TM_Transmutate,
                       TorannMagicDefOf.TM_EnchanterStone,
                       TorannMagicDefOf.TM_EnchantWeapon,
                       TorannMagicDefOf.TM_Polymorph,
                       TorannMagicDefOf.TM_Polymorph_I,
                       TorannMagicDefOf.TM_Polymorph_II,
                       TorannMagicDefOf.TM_Polymorph_III,
                       TorannMagicDefOf.TM_Shapeshift
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetBloodMagePowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.BloodMage)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_BloodGift,
                       TorannMagicDefOf.TM_IgniteBlood,
                       TorannMagicDefOf.TM_BloodForBlood,
                       TorannMagicDefOf.TM_BloodShield,
                       TorannMagicDefOf.TM_Rend,
                       TorannMagicDefOf.TM_Rend_I,
                       TorannMagicDefOf.TM_Rend_II,
                       TorannMagicDefOf.TM_Rend_III,
                       TorannMagicDefOf.TM_BloodMoon,
                       TorannMagicDefOf.TM_BloodMoon_I,
                       TorannMagicDefOf.TM_BloodMoon_II,
                       TorannMagicDefOf.TM_BloodMoon_III
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetTechnomancerPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Technomancer)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_TechnoShield,
                       TorannMagicDefOf.TM_Sabotage,
                       TorannMagicDefOf.TM_Overdrive,
                       TorannMagicDefOf.TM_OrbitalStrike,
                       TorannMagicDefOf.TM_OrbitalStrike_I,
                       TorannMagicDefOf.TM_OrbitalStrike_II,
                       TorannMagicDefOf.TM_OrbitalStrike_III,
                       TorannMagicDefOf.TM_TechnoBit,
                       TorannMagicDefOf.TM_TechnoTurret,
                       TorannMagicDefOf.TM_TechnoWeapon
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetGeomancerPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Geomancer)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_Stoneskin,
                       TorannMagicDefOf.TM_Encase,
                       TorannMagicDefOf.TM_Encase_I,
                       TorannMagicDefOf.TM_Encase_II,
                       TorannMagicDefOf.TM_Encase_III,
                       TorannMagicDefOf.TM_EarthSprites,
                       TorannMagicDefOf.TM_EarthernHammer,
                       TorannMagicDefOf.TM_Sentinel,
                       TorannMagicDefOf.TM_Meteor,
                       TorannMagicDefOf.TM_Meteor_I,
                       TorannMagicDefOf.TM_Meteor_II,
                       TorannMagicDefOf.TM_Meteor_III
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetWarlockPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Warlock)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_SoulBond,
                       TorannMagicDefOf.TM_ShadowBolt,
                       TorannMagicDefOf.TM_ShadowBolt_I,
                       TorannMagicDefOf.TM_ShadowBolt_II,
                       TorannMagicDefOf.TM_ShadowBolt_III,
                       TorannMagicDefOf.TM_Dominate,
                       TorannMagicDefOf.TM_Repulsion,
                       TorannMagicDefOf.TM_Repulsion_I,
                       TorannMagicDefOf.TM_Repulsion_II,
                       TorannMagicDefOf.TM_Repulsion_III,
                       TorannMagicDefOf.TM_PsychicShock
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetSuccubusPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Succubus)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_SoulBond,
                       TorannMagicDefOf.TM_ShadowBolt,
                       TorannMagicDefOf.TM_ShadowBolt_I,
                       TorannMagicDefOf.TM_ShadowBolt_II,
                       TorannMagicDefOf.TM_ShadowBolt_III,
                       TorannMagicDefOf.TM_Dominate,
                       TorannMagicDefOf.TM_Attraction,
                       TorannMagicDefOf.TM_Attraction_I,
                       TorannMagicDefOf.TM_Attraction_II,
                       TorannMagicDefOf.TM_Attraction_III,
                       TorannMagicDefOf.TM_Scorn
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetBardPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.TM_Bard)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_BardTraining,
                       TorannMagicDefOf.TM_Inspire,
                       TorannMagicDefOf.TM_Entertain,
                       TorannMagicDefOf.TM_Lullaby,
                       TorannMagicDefOf.TM_Lullaby_I,
                       TorannMagicDefOf.TM_Lullaby_II,
                       TorannMagicDefOf.TM_Lullaby_III,
                       TorannMagicDefOf.TM_BattleHymn
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetPriestPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Priest)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_AdvancedHeal,
                       TorannMagicDefOf.TM_Purify,
                       TorannMagicDefOf.TM_HealingCircle,
                       TorannMagicDefOf.TM_HealingCircle_I,
                       TorannMagicDefOf.TM_HealingCircle_II,
                       TorannMagicDefOf.TM_HealingCircle_III,
                       TorannMagicDefOf.TM_BestowMight,
                       TorannMagicDefOf.TM_BestowMight_I,
                       TorannMagicDefOf.TM_BestowMight_II,
                       TorannMagicDefOf.TM_BestowMight_III,
                       TorannMagicDefOf.TM_Resurrection
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetNecromancerPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Necromancer)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_RaiseUndead,
                       TorannMagicDefOf.TM_DeathMark,
                       TorannMagicDefOf.TM_DeathMark_I,
                       TorannMagicDefOf.TM_DeathMark_II,
                       TorannMagicDefOf.TM_DeathMark_III,
                       TorannMagicDefOf.TM_FogOfTorment,
                       TorannMagicDefOf.TM_ConsumeCorpse,
                       TorannMagicDefOf.TM_ConsumeCorpse_I,
                       TorannMagicDefOf.TM_ConsumeCorpse_II,
                       TorannMagicDefOf.TM_ConsumeCorpse_III,
                       TorannMagicDefOf.TM_CorpseExplosion,
                       TorannMagicDefOf.TM_CorpseExplosion_I,
                       TorannMagicDefOf.TM_CorpseExplosion_II,
                       TorannMagicDefOf.TM_CorpseExplosion_III,
                       TorannMagicDefOf.TM_LichForm,
                       TorannMagicDefOf.TM_DeathBolt,
                       TorannMagicDefOf.TM_DeathBolt_I,
                       TorannMagicDefOf.TM_DeathBolt_II,
                       TorannMagicDefOf.TM_DeathBolt_III
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetDruidPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Druid)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_Poison,
                       TorannMagicDefOf.TM_SootheAnimal,
                       TorannMagicDefOf.TM_SootheAnimal_I,
                       TorannMagicDefOf.TM_SootheAnimal_II,
                       TorannMagicDefOf.TM_SootheAnimal_III,
                       TorannMagicDefOf.TM_Regenerate,
                       TorannMagicDefOf.TM_CureDisease,
                       TorannMagicDefOf.TM_RegrowLimb
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetSummonerPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Summoner)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_SummonMinion,
                       TorannMagicDefOf.TM_SummonPylon,
                       TorannMagicDefOf.TM_SummonExplosive,
                       TorannMagicDefOf.TM_SummonElemental,
                       TorannMagicDefOf.TM_SummonPoppi
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetPaladinPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Paladin)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_P_RayofHope,
                       TorannMagicDefOf.TM_P_RayofHope_I,
                       TorannMagicDefOf.TM_P_RayofHope_II,
                       TorannMagicDefOf.TM_P_RayofHope_III,
                       TorannMagicDefOf.TM_Heal,
                       TorannMagicDefOf.TM_Shield,
                       TorannMagicDefOf.TM_Shield_I,
                       TorannMagicDefOf.TM_Shield_II,
                       TorannMagicDefOf.TM_Shield_III,
                       TorannMagicDefOf.TM_ValiantCharge,
                       TorannMagicDefOf.TM_Overwhelm,
                       TorannMagicDefOf.TM_HolyWrath
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetArcanistPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.Arcanist)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_Shadow,
                       TorannMagicDefOf.TM_Shadow_I,
                       TorannMagicDefOf.TM_Shadow_II,
                       TorannMagicDefOf.TM_Shadow_III,
                       TorannMagicDefOf.TM_MagicMissile,
                       TorannMagicDefOf.TM_MagicMissile_I,
                       TorannMagicDefOf.TM_MagicMissile_II,
                       TorannMagicDefOf.TM_MagicMissile_III,
                       TorannMagicDefOf.TM_Blink,
                       TorannMagicDefOf.TM_Blink_I,
                       TorannMagicDefOf.TM_Blink_II,
                       TorannMagicDefOf.TM_Blink_III,
                       TorannMagicDefOf.TM_Summon,
                       TorannMagicDefOf.TM_Summon_I,
                       TorannMagicDefOf.TM_Summon_II,
                       TorannMagicDefOf.TM_Summon_III,
                       TorannMagicDefOf.TM_Teleport,
                       TorannMagicDefOf.TM_FoldReality
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetStormBornPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.StormBorn)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_AMP,
                       TorannMagicDefOf.TM_AMP_I,
                       TorannMagicDefOf.TM_AMP_II,
                       TorannMagicDefOf.TM_AMP_III,
                       TorannMagicDefOf.TM_LightningBolt,
                       TorannMagicDefOf.TM_LightningCloud,
                       TorannMagicDefOf.TM_LightningStorm,
                       TorannMagicDefOf.TM_EyeOfTheStorm
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetHeartOfFrostPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.HeartOfFrost)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_Soothe,
                       TorannMagicDefOf.TM_Soothe_I,
                       TorannMagicDefOf.TM_Soothe_II,
                       TorannMagicDefOf.TM_Soothe_III,
                       TorannMagicDefOf.TM_Icebolt,
                       TorannMagicDefOf.TM_Snowball,
                       TorannMagicDefOf.TM_FrostRay,
                       TorannMagicDefOf.TM_FrostRay_I,
                       TorannMagicDefOf.TM_FrostRay_II,
                       TorannMagicDefOf.TM_FrostRay_III,
                       TorannMagicDefOf.TM_Rainmaker,
                       TorannMagicDefOf.TM_Blizzard
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetInnerFirePowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.InnerFire)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_RayofHope,
                       TorannMagicDefOf.TM_RayofHope_I,
                       TorannMagicDefOf.TM_RayofHope_II,
                       TorannMagicDefOf.TM_RayofHope_III,
                       TorannMagicDefOf.TM_Firebolt,
                       TorannMagicDefOf.TM_Fireclaw,
                       TorannMagicDefOf.TM_Fireball,
                       TorannMagicDefOf.TM_Firestorm
                   };
        }

        [NotNull]
        private static List<TMAbilityDef> GetWandererPowers()
        {
            return GetRawPowersForBaseClass(TorannMagicDefOf.TM_Wanderer)
                   ?? new List<TMAbilityDef>
                   {
                       TorannMagicDefOf.TM_TransferMana,
                       TorannMagicDefOf.TM_SiphonMana,
                       TorannMagicDefOf.TM_SpellMending,
                       TorannMagicDefOf.TM_DirtDevil,
                       TorannMagicDefOf.TM_Heater,
                       TorannMagicDefOf.TM_Cooler,
                       TorannMagicDefOf.TM_PowerNode,
                       TorannMagicDefOf.TM_Sunlight,
                       TorannMagicDefOf.TM_SmokeCloud,
                       TorannMagicDefOf.TM_Extinguish,
                       TorannMagicDefOf.TM_EMP,
                       TorannMagicDefOf.TM_ManaShield,
                       TorannMagicDefOf.TM_Blur,
                       TorannMagicDefOf.TM_ArcaneBolt,
                       TorannMagicDefOf.TM_LightningTrap,
                       TorannMagicDefOf.TM_Invisibility,
                       TorannMagicDefOf.TM_MageLight,
                       TorannMagicDefOf.TM_Ignite,
                       TorannMagicDefOf.TM_SnapFreeze,
                       TorannMagicDefOf.TM_Heal,
                       TorannMagicDefOf.TM_Blink,
                       TorannMagicDefOf.TM_Rainmaker,
                       TorannMagicDefOf.TM_SummonMinion,
                       TorannMagicDefOf.TM_Teleport
                   };
        }

        [ItemNotNull]
        private static IEnumerable<Class> GenerateClassTrees()
        {
            foreach (TraitDef trait in TM_Data.AllClassTraits)
            {
                yield return GenerateClassTree(trait);
            }

            yield return GenerateClassTree(TorannMagicDefOf.DeathKnight);
        }

        [NotNull]
        private static Class GenerateClassTree([NotNull] TraitDef trait)
        {
            var @class = new Class { TraitDef = trait, Abilities = new List<Ability>() };

            foreach (TMAbilityDef ability in GetAbilitiesFor(trait))
            {
                @class.Abilities.Add(new Ability { Def = ability });
            }

            return @class;
        }

        private static IEnumerable<TMAbilityDef> GetAbilitiesFor([NotNull] TraitDef trait)
        {
            if (BaseClassPowers.TryGetValue(trait, out List<TMAbilityDef> powers))
            {
                foreach (TMAbilityDef ability in powers)
                {
                    yield return ability;
                }
            }

            TM_CustomClass @class = TM_ClassUtility.CustomClasses().Find(c => c.classTrait == trait);

            if (@class == null)
            {
                yield break;
            }

            foreach (TMAbilityDef ability in @class.classAbilities)
            {
                yield return ability;
            }
        }

        [CanBeNull]
        private static List<TMAbilityDef> GetRawPowersForBaseClass([NotNull] TraitDef trait)
        {
            return !BaseClassPowers.TryGetValue(trait, out List<TMAbilityDef> powers) ? null : powers;
        }
    }
}
