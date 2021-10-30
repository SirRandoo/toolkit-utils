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
        private static readonly Dictionary<TraitDef, List<Ability>> BaseClassPowers;

        static ClassData()
        {
            BaseClassPowers = new Dictionary<TraitDef, List<Ability>>();
            foreach ((TraitDef @class, List<Ability> powers) in GetBaseClassPowerList())
            {
                BaseClassPowers.Add(@class, powers);
            }

            Classes = new List<Class>(GenerateClassTrees());
        }

        [NotNull]
        private static Tuple<TraitDef, List<Ability>> PowerPair(TraitDef trait, List<Ability> abilities)
        {
            return new Tuple<TraitDef, List<Ability>>(trait, abilities);
        }

        [NotNull]
        [ItemNotNull]
        private static IEnumerable<Tuple<TraitDef, List<Ability>>> GetBaseClassPowerList()
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
        private static List<Ability> GetChaosMagePowers()
        {
            List<Ability> defs = GetRawPowersForBaseClass(TorannMagicDefOf.ChaosMage);
            if (defs != null)
            {
                return defs;
            }

            var list = new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_ChaosTradition, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_WandererCraft, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Cantrips, 0))
            };

            list.AddRange(GetInnerFirePowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Firestorm)));
            list.AddRange(GetHeartOfFrostPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Rainmaker) || !IsAbility(p, TorannMagicDefOf.TM_Blizzard)));
            list.AddRange(GetStormBornPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_EyeOfTheStorm)));
            list.AddRange(GetArcanistPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_FoldReality)));
            list.AddRange(GetPaladinPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_HolyWrath)));
            list.AddRange(GetSummonerPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_SummonPoppi)));
            list.AddRange(GetDruidPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_RegrowLimb)));
            list.AddRange(
                GetNecromancerPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_RaiseUndead) || !IsAbility(p, TorannMagicDefOf.TM_LichForm) || !IsAbility(p, TorannMagicDefOf.TM_DeathBolt))
            );
            list.AddRange(GetPriestPowers().Where(p => !IsAbility(p, TorannMagicDefOf.TM_Resurrection)));
            list.AddRange(
                GetBardPowers()
                   .Where(
                        p => !IsAbility(p, TorannMagicDefOf.TM_BardTraining)
                             || !IsAbility(p, TorannMagicDefOf.TM_Inspire)
                             || !IsAbility(p, TorannMagicDefOf.TM_Entertain)
                             || !IsAbility(p, TorannMagicDefOf.TM_BattleHymn)
                    )
            );
            list.AddRange(
                GetWarlockPowers()
                   .Where(
                        p => !IsAbility(p, TorannMagicDefOf.TM_SoulBond)
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

        [NotNull]
        private static List<Ability> GetChronomancerPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Chronomancer);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Prediction, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_AlterFate, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_AccelerateTime, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_ReverseTime, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_ChronostaticField, 0),
                    ClassPower.From(TorannMagicDefOf.TM_ChronostaticField_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_ChronostaticField_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_ChronostaticField_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Recall, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetEnchanterPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Enchanter);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_EnchantedAura, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_EnchantedBody, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Transmutate, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_EnchanterStone, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_EnchantWeapon, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Polymorph, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Polymorph_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Polymorph_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Polymorph_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Shapeshift, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetBloodMagePowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.BloodMage);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_BloodGift, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_IgniteBlood, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_BloodForBlood, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_BloodShield, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Rend, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Rend_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Rend_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Rend_III, 3)
                ),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_BloodMoon, 0),
                    ClassPower.From(TorannMagicDefOf.TM_BloodMoon_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_BloodMoon_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_BloodMoon_III, 3)
                )
            };
        }

        [NotNull]
        private static List<Ability> GetTechnomancerPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Technomancer);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_TechnoShield, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Sabotage, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Overdrive, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_OrbitalStrike, 0),
                    ClassPower.From(TorannMagicDefOf.TM_OrbitalStrike_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_OrbitalStrike_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_OrbitalStrike_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_TechnoBit, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_TechnoTurret, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_TechnoWeapon, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetGeomancerPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Geomancer);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Stoneskin, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Encase, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Encase_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Encase_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Encase_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_EarthSprites, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_EarthernHammer, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Sentinel, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Meteor, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Meteor_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Meteor_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Meteor_III, 3)
                )
            };
        }

        [NotNull]
        private static List<Ability> GetWarlockPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Warlock);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SoulBond, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_ShadowBolt, 0),
                    ClassPower.From(TorannMagicDefOf.TM_ShadowBolt_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_ShadowBolt_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_ShadowBolt_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Dominate, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Repulsion, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Repulsion_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Repulsion_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Repulsion_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_PsychicShock, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetSuccubusPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Succubus);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SoulBond, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_ShadowBolt, 0),
                    ClassPower.From(TorannMagicDefOf.TM_ShadowBolt_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_ShadowBolt_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_ShadowBolt_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Dominate, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Attraction, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Attraction_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Attraction_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Attraction_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Scorn, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetBardPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.TM_Bard);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_BardTraining, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Inspire, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Entertain, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Lullaby, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Lullaby_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Lullaby_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Lullaby_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_BattleHymn, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetPriestPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Priest);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_AdvancedHeal, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Purify, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_HealingCircle, 0),
                    ClassPower.From(TorannMagicDefOf.TM_HealingCircle_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_HealingCircle_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_HealingCircle_III, 3)
                ),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_BestowMight, 0),
                    ClassPower.From(TorannMagicDefOf.TM_BestowMight_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_BestowMight_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_BestowMight_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Resurrection, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetNecromancerPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Necromancer);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_RaiseUndead, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_DeathMark, 0),
                    ClassPower.From(TorannMagicDefOf.TM_DeathMark_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_DeathMark_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_DeathMark_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_FogOfTorment, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_ConsumeCorpse, 0),
                    ClassPower.From(TorannMagicDefOf.TM_ConsumeCorpse_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_ConsumeCorpse_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_ConsumeCorpse_III, 3)
                ),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_CorpseExplosion, 0),
                    ClassPower.From(TorannMagicDefOf.TM_CorpseExplosion_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_CorpseExplosion_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_CorpseExplosion_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_LichForm, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_DeathBolt, 0),
                    ClassPower.From(TorannMagicDefOf.TM_DeathBolt_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_DeathMark_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_DeathMark_III, 3)
                )
            };
        }

        [NotNull]
        private static List<Ability> GetDruidPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Druid);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Poison, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_SootheAnimal, 0),
                    ClassPower.From(TorannMagicDefOf.TM_SootheAnimal_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_SootheAnimal_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_SootheAnimal_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Regenerate, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_CureDisease, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_RegrowLimb, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetSummonerPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Summoner);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SummonMinion, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SummonPylon, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SummonExplosive, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SummonElemental, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SummonPoppi, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetPaladinPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Paladin);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_P_RayofHope, 0),
                    ClassPower.From(TorannMagicDefOf.TM_P_RayofHope_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_P_RayofHope_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_P_RayofHope_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Heal, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Shield, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Shield_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Shield_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Shield_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_ValiantCharge, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Overwhelm, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_HolyWrath, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetArcanistPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.Arcanist);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Shadow, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Shadow_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Shadow_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Shadow_III, 3)
                ),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_MagicMissile, 0),
                    ClassPower.From(TorannMagicDefOf.TM_MagicMissile_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_MagicMissile_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_MagicMissile_III, 3)
                ),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Blink, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Blink_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Blink_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Blink_III, 3)
                ),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Summon, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Summon_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Summon_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Summon_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Teleport, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_FoldReality, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetStormBornPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.StormBorn);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_AMP, 0),
                    ClassPower.From(TorannMagicDefOf.TM_AMP_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_AMP_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_AMP_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_LightningBolt, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_LightningCloud, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_LightningStorm, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_EyeOfTheStorm, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetHeartOfFrostPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.HeartOfFrost);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Soothe, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Soothe_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Soothe_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Soothe_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Icebolt, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Snowball, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_FrostRay, 0),
                    ClassPower.From(TorannMagicDefOf.TM_FrostRay_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_FrostRay_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_FrostRay_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Rainmaker, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Blizzard, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetInnerFirePowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.InnerFire);
            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_RayofHope, 0),
                    ClassPower.From(TorannMagicDefOf.TM_RayofHope_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_RayofHope_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_RayofHope_III, 3)
                ),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Firebolt, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Fireclaw, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Fireball, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Firestorm, 0))
            };
        }

        [NotNull]
        private static List<Ability> GetWandererPowers()
        {
            List<Ability> list = GetRawPowersForBaseClass(TorannMagicDefOf.TM_Wanderer);

            if (list != null)
            {
                return list;
            }

            return new List<Ability>
            {
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_TransferMana, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SiphonMana, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_DirtDevil, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Heater, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Cooler, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_PowerNode, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Sunlight, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SmokeCloud, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Extinguish, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_EMP, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_ManaShield, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Blur, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_ArcaneBolt, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_LightningTrap, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Invisibility, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_MageLight, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Ignite, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_SnapFreeze, 0)),
                Ability.From(ClassPower.From(TorannMagicDefOf.TM_Heal, 0)),
                Ability.From(
                    ClassPower.From(TorannMagicDefOf.TM_Blink, 0),
                    ClassPower.From(TorannMagicDefOf.TM_Blink_I, 1),
                    ClassPower.From(TorannMagicDefOf.TM_Blink_II, 2),
                    ClassPower.From(TorannMagicDefOf.TM_Blink_III, 3)
                ),
                Ability.From(new ClassPower(TorannMagicDefOf.TM_Rainmaker, 0)),
                Ability.From(new ClassPower(TorannMagicDefOf.TM_SummonMinion, 0)),
                Ability.From(new ClassPower(TorannMagicDefOf.TM_Teleport, 0))
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

            foreach (Ability ability in GetAbilitiesFor(trait))
            {
                @class.Abilities.Add(ability);
            }

            return @class;
        }

        private static IEnumerable<Ability> GetAbilitiesFor([NotNull] TraitDef trait)
        {
            if (BaseClassPowers.TryGetValue(trait, out List<Ability> powers))
            {
                foreach (Ability ability in powers)
                {
                    yield return ability;
                }
            }

            TM_CustomClass @class = TM_ClassUtility.CustomClasses().Find(c => c.classTrait == trait);

            if (@class == null)
            {
                yield break;
            }

            Ability a = null;
            var lastDefName = "";
            foreach (TMAbilityDef ability in @class.classAbilities.OrderBy(i => i.defName))
            {
                if (lastDefName.NullOrEmpty() || a == null)
                {
                    a = Ability.From(ClassPower.From(ability, a?.Tiers?.Count ?? 0));
                    lastDefName = ability.defName;
                }
                else if (ability.defName.StartsWith(lastDefName))
                {
                    a.Tiers.Add(ClassPower.From(ability, a.Tiers.Count));
                    lastDefName = ability.defName;
                }
                else
                {
                    a = Ability.From(ClassPower.From(ability, a.Tiers?.Count ?? 0));
                    lastDefName = ability.defName;
                }
            }
        }

        [CanBeNull]
        private static List<Ability> GetRawPowersForBaseClass([NotNull] TraitDef trait)
        {
            return !BaseClassPowers.TryGetValue(trait, out List<Ability> powers) ? null : powers;
        }

        private static bool IsAbility([NotNull] Ability ability, [NotNull] Def def)
        {
            return ability.Name == (def.label ?? def.defName);
        }
    }
}
