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

using System.Text;
using JetBrains.Annotations;
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
        private object CompData { get; set; }
        private object Data { get; set; }

        [NotNull] public string ExperienceString => $"{CurrentExp:N0} / {ExpToNextLevel:N0}";

        [NotNull]
        public static CharacterData CreateInstance(Pawn pawn)
        {
            return new CharacterData {Parent = pawn};
        }

        [NotNull]
        internal CharacterData FindClass()
        {
            FindClassType();
            FindClassName();

            return this;
        }

        [NotNull]
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
            ExpForCurrentLevel = MagicComp.GetCurrentLevelExpFrom(CompData, cType);
            ExpToNextLevel = MagicComp.GetNextLevelExpFrom(CompData, cType);
            ResourceRegenRate = MagicComp.GetResourceRegenRateFrom(CompData, cType);
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
                CompData = magicData;
                Data = MagicComp.GetDataFromComp(magicData, ClassTypes.Magic);
                return;
            }

            object mightData = Parent.GetMightDataComp();

            if (mightData != null && MagicComp.IsMightUser(mightData))
            {
                Type = ClassTypes.Might;
                CompData = mightData;
                Data = MagicComp.GetDataFromComp(mightData, ClassTypes.Might);
            }
        }

        private void FindClassName()
        {
            switch (Type)
            {
                case ClassTypes.Might:
                    ClassName = MagicComp.GetMightClassName(CompData, Parent);
                    break;
                case ClassTypes.Magic:
                    ClassName = MagicComp.GetMagicClassName(CompData, Parent);
                    break;
            }
        }

        public void Reset()
        {
            if (Type == null)
            {
                return;
            }

            MagicComp.ResetCharacterData(CompData, (ClassTypes) Type);
        }

        public override string ToString()
        {
            return new StringBuilder("CharacterData {").AppendLine($"  ClassType={Type?.ToString() ?? "None"}")
               .AppendLine($"  ClassName={ClassName ?? "None"}")
               .Append("}")
               .ToString();
        }
    }
}
