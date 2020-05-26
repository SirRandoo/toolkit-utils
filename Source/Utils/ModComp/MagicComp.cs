using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static class MagicComp
    {
        public enum ClassTypes { Unknown, Magic, Might }

        public static bool Active;

        private static readonly Type MightClass;
        private static readonly Type MagicClass;
        private static readonly Type StaminaNeed;
        private static readonly Type ManaNeed;
        private static readonly FieldInfo MightCustomClass;
        private static readonly FieldInfo MightRegen;
        private static readonly FieldInfo MagicCustomClass;
        private static readonly FieldInfo MagicRegen;
        private static readonly PropertyInfo IsMightUser;
        private static readonly PropertyInfo IsMagicUser;
        private static readonly PropertyInfo MightDataProperty;
        private static readonly PropertyInfo MagicDataProperty;
        private static readonly PropertyInfo MightXpLastLevel;
        private static readonly PropertyInfo MightXpTillNextLevel;
        private static readonly PropertyInfo MagicXpLastLevel;
        private static readonly PropertyInfo MagicXpTillNextLevel;
        private static readonly PropertyInfo MightUserLevel;
        private static readonly PropertyInfo MightUserXp;
        private static readonly PropertyInfo MightAbilityPoints;
        private static readonly PropertyInfo MagicUserLevel;
        private static readonly PropertyInfo MagicUserXp;
        private static readonly PropertyInfo MagicAbilityPoints;

        private static readonly List<string> KnownClasses = new List<string>
        {
            "Arcanist",
            "Summoner",
            "Paladin",
            "Gladiator",
            "TM_Sniper",
            "Druid",
            "Bladedancer",
            "Necromancer",
            "Priest",
            "Ranger",
            "Lich",
            "TM_Bard",
            "Faceless",
            "Succubus",
            "Warlock",
            "TM_Psionic",
            "Geomancer",
            "Technomancer",
            "DeathKnight",
            "Enchanter",
            "BloodMage",
            "Chronomancer",
            "TM_Monk",
            "TM_Wanderer",
            "TM_Wayfarer",
            "ChaosMage",
            "TM_SuperSoldier",
            "TM_Commander",
            "StormBorn",
            "InnerFire"
        };

        static MagicComp()
        {
            foreach (var handle in LoadedModManager.ModHandles.Where(
                m => m.Content.PackageId.EqualsIgnoreCase("Torann.ARimworldOfMagic")
            ))
            {
                try
                {
                    var assembly = handle.GetType().Assembly;

                    MightClass = assembly.GetType("TorannMagic.CompAbilityUserMight");
                    MagicClass = assembly.GetType("TorannMagic.CompAbilityUserMagic");
                    StaminaNeed = assembly.GetType("TorannMagic.Need_Stamina");
                    ManaNeed = assembly.GetType("TorannMagic.Need_Mana");

                    MightCustomClass = MightClass.GetField("customClass");
                    MightRegen = MightClass.GetField("spRegenRate");
                    MagicCustomClass = MagicClass.GetField("customClass");
                    MagicRegen = MagicClass.GetField("mpRegenRate");

                    MightXpLastLevel = MightClass.GetProperty("XPLastLevel");
                    MightXpTillNextLevel = MightClass.GetProperty("MightUserXPTillNextLevel");
                    MagicXpLastLevel = MagicClass.GetProperty("XPLastLevel");
                    MagicXpTillNextLevel = MagicClass.GetProperty("MagicUserXPTillNextLevel");

                    IsMightUser = MightClass.GetProperty("IsMightUser");
                    IsMagicUser = MagicClass.GetProperty("IsMagicUser");

                    MightDataProperty = MightClass.GetProperty("MightData");
                    MagicDataProperty = MagicClass.GetProperty("MagicData");

                    var mightData = assembly.GetType("TorannMagic.MightData");
                    var magicData = assembly.GetType("TorannMagic.MagicData");

                    MightUserLevel = mightData.GetProperty("MightUserLevel");
                    MightUserXp = mightData.GetProperty("MightUserXP");
                    MightAbilityPoints = mightData.GetProperty("MightAbilityPoints");
                    MagicUserLevel = magicData.GetProperty("MagicUserLevel");
                    MagicUserXp = magicData.GetProperty("MagicUserXP");
                    MagicAbilityPoints = magicData.GetProperty("MagicAbilityPoints");

                    Active = true;
                }
                catch (Exception e)
                {
                    TkLogger.Error("Compatiblity class for RimWorld of Magic failed!", e);
                }
            }
        }

        public static CharacterData GetCharacterData(Pawn pawn)
        {
            if (!Active)
            {
                return null;
            }

            var comps = pawn.AllComps
                .Where(c => c is CompUseEffect)
                .Where(c => c.GetType().Namespace.EqualsIgnoreCase("TorannMagic"))
                .ToList();

            if (comps.NullOrEmpty())
            {
                return null;
            }

            foreach (var comp in comps)
            {
                var compType = comp.GetType();

                if (compType != MagicClass && compType != MightClass)
                {
                    continue;
                }

                if (compType == MagicClass && !(bool) IsMagicUser.GetValue(comp))
                {
                    continue;
                }

                if (compType == MightClass && !(bool) IsMightUser.GetValue(comp))
                {
                    continue;
                }

                var data = CharacterData.FromComp(comp as CompUseEffect);
                data.LocateClass(pawn, comp as CompUseEffect);
                data.LocateResourceData(pawn);

                if (data.Class.NullOrEmpty())
                {
                    continue;
                }

                return data;
            }

            return null;
        }

        public class CharacterData
        {
            public string Class;
            public float CurrentXp;
            public bool Gifted;
            public int Level;
            public float LevelXp;
            public float NextLevelXp;
            public int Points;
            public float ResourceCurrent;
            public float ResourceMax;
            public float ResourceRegenRate;
            public ClassTypes Type = ClassTypes.Unknown;

            public string Experience => $"{CurrentXp:N0} / {NextLevelXp:N0}";

            public static CharacterData FromComp(CompUseEffect comp)
            {
                var data = new CharacterData();

                if (comp.GetType() == MightClass)
                {
                    data.Type = ClassTypes.Might;
                }
                else if (comp.GetType() == MagicClass)
                {
                    data.Type = ClassTypes.Magic;
                }

                data.Initialize(comp);

                return data;
            }

            private void Initialize(CompUseEffect comp)
            {
                if (Type == ClassTypes.Magic)
                {
                    var data = MagicDataProperty.GetValue(comp);

                    if (data == null)
                    {
                        return;
                    }

                    Gifted = true;
                    Points = (int) MagicAbilityPoints.GetValue(data);
                    Level = (int) MagicUserLevel.GetValue(data);
                    CurrentXp = (int) MagicUserXp.GetValue(data);
                    LevelXp = (float) MagicXpLastLevel.GetValue(comp);
                    NextLevelXp = (int) MagicXpTillNextLevel.GetValue(comp);
                    ResourceRegenRate = (float) MagicRegen.GetValue(comp);
                    return;
                }

                if (Type == ClassTypes.Might)
                {
                    var data = MightDataProperty.GetValue(comp);

                    if (data == null)
                    {
                        return;
                    }

                    Gifted = true;
                    Points = (int) MightAbilityPoints.GetValue(data);
                    Level = (int) MightUserLevel.GetValue(data);
                    CurrentXp = (int) MightUserXp.GetValue(data);
                    LevelXp = (float) MightXpLastLevel.GetValue(comp);
                    NextLevelXp = (int) MightXpTillNextLevel.GetValue(comp);
                    ResourceRegenRate = (float) MightRegen.GetValue(comp);
                }
            }

            internal void LocateClass(Pawn pawn, CompUseEffect comp)
            {
                Def customClass = null;

                switch (Type)
                {
                    case ClassTypes.Magic:
                        customClass = MagicCustomClass.GetValue(comp) as Def;
                        break;
                    case ClassTypes.Might:
                        customClass = MightCustomClass.GetValue(comp) as Def;
                        break;
                }

                if (customClass != null)
                {
                    Class = customClass.LabelCap;
                }

                Class = pawn.story.traits.allTraits
                    .FirstOrDefault(t => KnownClasses.Contains(t.def.defName))
                    ?.LabelCap;

                if (Class != null)
                {
                    Gifted = true;
                }
            }

            internal void LocateResourceData(Pawn pawn)
            {
                Need resourceNeed = null;

                switch (Type)
                {
                    case ClassTypes.Magic:
                    {
                        var need = pawn.needs.AllNeeds
                            .FirstOrDefault(n => n.GetType() == ManaNeed);

                        if (need == null)
                        {
                            return;
                        }

                        resourceNeed = need;
                        break;
                    }
                    case ClassTypes.Might:
                    {
                        var need = pawn.needs.AllNeeds
                            .FirstOrDefault(n => n.GetType() == StaminaNeed);

                        if (need == null)
                        {
                            return;
                        }

                        resourceNeed = need;
                        break;
                    }
                }


                if (resourceNeed == null)
                {
                    return;
                }

                ResourceCurrent = Mathf.CeilToInt(resourceNeed.CurLevel * 100f);
                ResourceMax = Mathf.CeilToInt(resourceNeed.MaxLevel * 100f);
            }
        }
    }
}
