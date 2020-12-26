using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [StaticConstructorOnStartup]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static class MagicComp
    {
        public static readonly bool Active;

        // Might
        private static readonly FieldInfo MightCustomClass;
        private static readonly FieldInfo MightRegen;
        private static readonly PropertyInfo IsMightUserProperty;
        private static readonly PropertyInfo MightDataProperty;
        private static readonly PropertyInfo MightXpTillNextLevel;
        private static readonly PropertyInfo MightXpLastLevel;
        private static readonly PropertyInfo MightUserXp;
        private static readonly PropertyInfo MightUserLevel;
        private static readonly PropertyInfo MightAbilityPoints;
        private static readonly MethodInfo MightResetSkills;

        // Magic
        private static readonly FieldInfo MagicCustomClass;
        private static readonly FieldInfo MagicRegen;
        private static readonly PropertyInfo IsMagicUserProperty;
        private static readonly PropertyInfo MagicDataProperty;
        private static readonly PropertyInfo MagicXpLastLevel;
        private static readonly PropertyInfo MagicXpTillNextLevel;
        private static readonly PropertyInfo MagicUserLevel;
        private static readonly PropertyInfo MagicUserXp;
        private static readonly PropertyInfo MagicAbilityPoints;
        private static readonly MethodInfo MagicResetSkills;

        // TM_Calc
        private static readonly MethodInfo IsUndeadMethod;

        // TM_Data
        private static readonly PropertyInfo MagicTraits;
        private static readonly PropertyInfo MightTraits;

        // TM_ClassUtility
        private static readonly PropertyInfo CustomMagicClasses;
        private static readonly PropertyInfo CustomFighterClasses;

        // TM_CustomClass
        private static readonly FieldInfo CustomClassTraitField;

        // MagicUserUtility
        private static readonly MethodInfo GetMagicUserMethod;
        private static readonly MethodInfo GetManaNeedMethod;

        // MightUserUtility
        private static readonly MethodInfo GetMightUserMethod;
        private static readonly MethodInfo GetStaminaNeedMethod;

        static MagicComp()
        {
            foreach (Mod handle in LoadedModManager.ModHandles.Where(
                m => m.Content.PackageId.EqualsIgnoreCase("Torann.ARimworldOfMagic")
            ))
            {
                try
                {
                    Assembly assembly = handle.GetType().Assembly;

                    // Might
                    Type mightClass = assembly.GetType("TorannMagic.CompAbilityUserMight");
                    MightCustomClass = mightClass.GetField("customClass");
                    MightRegen = mightClass.GetField("spRegenRate");
                    MightXpLastLevel = mightClass.GetProperty("XPLastLevel");
                    MightXpTillNextLevel = mightClass.GetProperty("MightUserXPTillNextLevel");
                    IsMightUserProperty = mightClass.GetProperty("IsMightUser");
                    MightDataProperty = mightClass.GetProperty("MightData");
                    MightResetSkills = mightClass.GetMethod("ResetSkills");

                    Type mightData = assembly.GetType("TorannMagic.MightData");
                    MightUserLevel = mightData.GetProperty("MightUserLevel");
                    MightUserXp = mightData.GetProperty("MightUserXP");
                    MightAbilityPoints = mightData.GetProperty("MightAbilityPoints");


                    Type magicClass = assembly.GetType("TorannMagic.CompAbilityUserMagic");
                    MagicCustomClass = magicClass.GetField("customClass");
                    MagicRegen = magicClass.GetField("mpRegenRate");
                    MagicXpLastLevel = magicClass.GetProperty("XPLastLevel");
                    MagicXpTillNextLevel = magicClass.GetProperty("MagicUserXPTillNextLevel");
                    IsMagicUserProperty = magicClass.GetProperty("IsMagicUser");
                    MagicDataProperty = magicClass.GetProperty("MagicData");
                    MagicResetSkills = magicClass.GetMethod("ResetSkills");

                    Type magicData = assembly.GetType("TorannMagic.MagicData");

                    MagicUserLevel = magicData.GetProperty("MagicUserLevel");
                    MagicUserXp = magicData.GetProperty("MagicUserXP");
                    MagicAbilityPoints = magicData.GetProperty("MagicAbilityPoints");


                    Type tmCalcType = assembly.GetType("TorannMagic.TM_Calc");
                    IsUndeadMethod = tmCalcType.GetMethod("IsUndead");


                    Type tmDataType = assembly.GetType("TorannMagic.TM_Data");
                    MagicTraits = tmDataType.GetProperty("MagicTraits");
                    MightTraits = tmDataType.GetProperty("MightTraits");


                    Type tmClassUtilityType = assembly.GetType("TorannMagic.TM_ClassUtility");
                    CustomMagicClasses = tmClassUtilityType.GetProperty("CustomMageClasses");
                    CustomFighterClasses = tmClassUtilityType.GetProperty("CustomFighterClasses");


                    Type tmCustomClassType = assembly.GetType("TorannMagic.TMDefs.TM_CustomClass");
                    CustomClassTraitField = tmCustomClassType.GetField("classTrait");


                    Type magicUserUtility = assembly.GetType("TorannMagic.MagicUserUtility");
                    GetManaNeedMethod = magicUserUtility.GetMethod("GetMana");
                    GetMagicUserMethod = magicUserUtility.GetMethod("GetMagicUser");


                    Type mightUserUtility = assembly.GetType("TorannMagic.MightUserUtility");
                    GetMightUserMethod = mightUserUtility.GetMethod("GetMightUser");
                    GetStaminaNeedMethod = mightUserUtility.GetMethod("GetStamina");

                    Active = true;
                }
                catch (Exception e)
                {
                    LogHelper.Error("Compatiblity class for RimWorld of Magic failed!", e);
                }
            }
        }

        public static CharacterData GetCharacterData(Pawn pawn)
        {
            return !Active ? null : CharacterData.CreateInstance(pawn).FindClass().PullData();
        }

        internal static object GetMightDataComp(this Pawn pawn)
        {
            return GetMightUserMethod.Invoke(null, new object[] {pawn});
        }

        internal static object GetMagicDataComp(this Pawn pawn)
        {
            return GetMagicUserMethod.Invoke(null, new object[] {pawn});
        }

        internal static bool IsMagicUser(object magicData)
        {
            return (bool) IsMagicUserProperty.GetValue(magicData);
        }

        internal static bool IsMightUser(object mightData)
        {
            return (bool) IsMightUserProperty.GetValue(mightData);
        }

        internal static string GetMightClassName(object mightData, Pawn parent)
        {
            object result = MightCustomClass.GetValue(mightData);

            return result == null
                ? GetMightClassNameBase(parent)
                : parent.story.traits.GetTrait((TraitDef) CustomClassTraitField.GetValue(result))?.Label;
        }

        private static string GetMightClassNameBase(Pawn pawn)
        {
            foreach (TraitDef t in GetMightClasses())
            {
                Trait trait = pawn.story.traits.GetTrait(t);

                if (trait == null)
                {
                    continue;
                }

                return trait.Label;
            }

            return null;
        }

        internal static string GetMagicClassName(object magicData, Pawn parent)
        {
            object result = MagicCustomClass.GetValue(magicData);

            return result == null
                ? GetMagicClassNameBase(parent)
                : parent.story.traits.GetTrait((TraitDef) CustomClassTraitField.GetValue(result))?.Label;
        }

        private static string GetMagicClassNameBase(Pawn pawn)
        {
            foreach (TraitDef t in GetMagicClasses())
            {
                Trait trait = pawn.story.traits.GetTrait(t);

                if (trait == null)
                {
                    continue;
                }

                return trait.Label;
            }

            return null;
        }

        private static object GetMagicDataFromComp(object magicComp)
        {
            return MagicDataProperty.GetValue(magicComp);
        }

        private static object GetMightDataFromComp(object mightComp)
        {
            return MightDataProperty.GetValue(mightComp);
        }

        internal static object GetDataFromComp(object comp, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    return GetMagicDataFromComp(comp);
                case ClassTypes.Might:
                    return GetMightDataFromComp(comp);
            }

            return null;
        }

        internal static int GetAbilityPointsFrom(object data, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    return (int) MagicAbilityPoints.GetValue(data);
                case ClassTypes.Might:
                    return (int) MightAbilityPoints.GetValue(data);
            }

            return -1;
        }

        internal static int GetLevelFrom(object data, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    return (int) MagicUserLevel.GetValue(data);
                case ClassTypes.Might:
                    return (int) MightUserLevel.GetValue(data);
            }

            return -1;
        }

        internal static int GetCurrentExpFrom(object data, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    return (int) MagicUserXp.GetValue(data);
                case ClassTypes.Might:
                    return (int) MightUserXp.GetValue(data);
            }

            return -1;
        }

        internal static float GetCurrentLevelExpFrom(object data, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    return (float) MagicXpLastLevel.GetValue(data);
                case ClassTypes.Might:
                    return (float) MightXpLastLevel.GetValue(data);
            }

            return -1;
        }

        internal static int GetNextLevelExpFrom(object data, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    return (int) MagicXpTillNextLevel.GetValue(data);
                case ClassTypes.Might:
                    return (int) MightXpTillNextLevel.GetValue(data);
            }

            return -1;
        }

        internal static float GetResourceRegenRateFrom(object data, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    return (float) MagicRegen.GetValue(data);
                case ClassTypes.Might:
                    return (float) MightRegen.GetValue(data);
            }

            return -1f;
        }

        internal static Need GetResourceFor(Pawn pawn, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    return GetManaNeedMethod.Invoke(null, new object[] {pawn}) as Need;
                case ClassTypes.Might:
                    return GetStaminaNeedMethod.Invoke(null, new object[] {pawn}) as Need;
            }

            return null;
        }

        internal static bool IsUndead(this Pawn pawn)
        {
            return Active && (bool) IsUndeadMethod.Invoke(null, new object[] {pawn});
        }

        internal static IEnumerable<TraitDef> GetMagicClasses()
        {
            List<TraitDef> container = MagicTraits.GetValue(null) as List<TraitDef> ?? new List<TraitDef>();

            if (!(CustomMagicClasses.GetValue(null) is IList customMagic))
            {
                return container;
            }

            foreach (object trait in customMagic)
            {
                if (!(CustomClassTraitField.GetValue(trait) is TraitDef result))
                {
                    continue;
                }

                container.Add(result);
            }

            return container.Where(t => !t.defName.Equals("Gifted"));
        }

        internal static IEnumerable<TraitDef> GetMightClasses()
        {
            List<TraitDef> container = MightTraits.GetValue(null) as List<TraitDef> ?? new List<TraitDef>();

            if (!(CustomFighterClasses.GetValue(null) is IList customMight))
            {
                return container;
            }

            foreach (object trait in customMight)
            {
                if (!(CustomClassTraitField.GetValue(trait) is TraitDef result))
                {
                    continue;
                }

                container.Add(result);
            }

            return container.Where(t => !t.defName.Equals("PhysicalProdigy"));
        }

        internal static IEnumerable<TraitDef> GetAllClasses()
        {
            var container = new List<TraitDef>();

            container.AddRange(GetMagicClasses());
            container.AddRange(GetMightClasses());
            container.AddRange(GetMissingClasses(container));
            container = container.Where(c => c != null).ToList();
            container.SortBy(t => t.label ?? t.defName);

            return container;
        }

        private static IEnumerable<TraitDef> GetMissingClasses(List<TraitDef> container)
        {
            if (!container.Any(t => t.defName.Equals("DeathKnight")))
            {
                yield return TraitDef.Named("DeathKnight");
            }
        }

        internal static void ResetCharacterData(object data, ClassTypes classType)
        {
            switch (classType)
            {
                case ClassTypes.Magic:
                    MagicResetSkills.Invoke(data, new object[] { });
                    return;
                case ClassTypes.Might:
                    MightResetSkills.Invoke(data, new object[] { });
                    return;
            }
        }
    }
}
