using RimWorld;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public enum ClassTypes { Might, Magic }

    public class CharacterData
    {
        public ClassTypes? Type;

        public string ClassName { get; private set; }
        public bool IsGifted => Type != null && ClassName != null;
        public int Level { get; private set; }
        public int MaxLevel { get; private set; }
        public int SkillPoints { get; private set; }

        public float CurrentExp { get; set; }
        public float ExpToNextLevel { get; set; }
        public float ExpForCurrentLevel { get; set; }
        public float CurrentResource { get; private set; }
        public float MaxResource { get; private set; }
        public float ResourceRegenRate { get; private set; }
        private Pawn Parent { get; set; }
        private object Data { get; set; }

        public string ExperienceString => $"{CurrentExp:N0} / {ExpToNextLevel:N0}";

        public static CharacterData CreateInstance(Pawn pawn)
        {
            return new CharacterData {Parent = pawn};
        }

        internal CharacterData FindClass()
        {
            FindClassType();
            FindClassName();

            return this;
        }

        internal CharacterData PullData()
        {
            PullFromData();
            PullFromResource();

            return this;
        }

        private void PullFromData()
        {
            if (Type == null)
            {
                return;
            }

            var cType = (ClassTypes) Type;
            SkillPoints = MagicComp.GetAbilityPointsFrom(Data, cType);
            Level = MagicComp.GetLevelFrom(Data, cType);
            CurrentExp = MagicComp.GetCurrentExpFrom(Data, cType);
            ExpForCurrentLevel = MagicComp.GetCurrentLevelExpFrom(Data, cType);
            ExpToNextLevel = MagicComp.GetNextLevelExpFrom(Data, cType);
            ResourceRegenRate = MagicComp.GetResourceRegenRateFrom(Data, cType);
        }

        private void PullFromResource()
        {
            if (Type == null)
            {
                return;
            }

            Need resource = MagicComp.GetResourceFor(Parent, (ClassTypes) Type);

            if (resource == null)
            {
                CurrentResource = 0;
                MaxResource = 0;
                return;
            }

            CurrentResource = Mathf.CeilToInt(resource.CurLevel * 100f);
            MaxResource = Mathf.CeilToInt(resource.MaxLevel * 100f);
        }

        private void FindClassType()
        {
            object magicData = Parent.GetMagicDataComp();

            if (magicData != null && MagicComp.IsMagicUser(magicData))
            {
                Type = ClassTypes.Magic;
                Data = magicData;
                return;
            }

            object mightData = Parent.GetMightDataComp();

            if (mightData != null && MagicComp.IsMightUser(mightData))
            {
                Type = ClassTypes.Might;
                Data = mightData;
            }
        }

        private void FindClassName()
        {
            switch (Type)
            {
                case ClassTypes.Might:
                    ClassName = MagicComp.GetMightClassName(Data, Parent);
                    break;
                case ClassTypes.Magic:
                    ClassName = MagicComp.GetMagicClassName(Data, Parent);
                    break;
            }
        }

        public void Reset()
        {
            if (Type == null)
            {
                return;
            }

            MagicComp.ResetCharacterData(Data, (ClassTypes) Type);
        }
    }
}
