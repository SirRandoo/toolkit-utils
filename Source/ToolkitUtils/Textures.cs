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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RimWorld;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class Textures
    {
        public static readonly Texture2D SortingAscend = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
        public static readonly Texture2D SortingDescend = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
        public static readonly Texture2D Gear = ContentFinder<Texture2D>.Get("UI/Icons/Gear");
        public static readonly Texture2D Trash = ContentFinder<Texture2D>.Get("UI/Icons/Trash");
        public static readonly Texture2D Refresh = ContentFinder<Texture2D>.Get("UI/Icons/Refresh");
        public static readonly Texture2D Edit = ContentFinder<Texture2D>.Get("UI/Icons/Edit");
        public static readonly Texture2D Visible = ContentFinder<Texture2D>.Get("UI/Icons/Visible");
        public static readonly Texture2D Hidden = ContentFinder<Texture2D>.Get("UI/Icons/Hidden");
        public static readonly Texture2D Stack = ContentFinder<Texture2D>.Get("UI/Icons/Stack");
        public static readonly Texture2D Reset = ContentFinder<Texture2D>.Get("UI/Icons/Reset");
        public static readonly Texture2D QuestionMark = ContentFinder<Texture2D>.Get("UI/Icons/QuestionMark");
        public static readonly Texture2D Filter = ContentFinder<Texture2D>.Get("UI/Icons/Filter");
        public static readonly Texture2D CollapsedArrow = ContentFinder<Texture2D>.Get("UI/Icons/CollapsedArrow");
        public static readonly Texture2D ExpandedArrow = ContentFinder<Texture2D>.Get("UI/Icons/ExpandedArrow");
        public static readonly Texture2D DiceSideOne = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideOne");
        public static readonly Texture2D DiceSideTwo = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideTwo");
        public static readonly Texture2D DiceSideThree = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideThree");
        public static readonly Texture2D DiceSideFour = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideFour");
        public static readonly Texture2D DiceSideFive = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideFive");
        public static readonly Texture2D DiceSideSix = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideSix");
        public static readonly List<Texture2D> DiceSides = new List<Texture2D>
        {
            DiceSideOne,
            DiceSideTwo,
            DiceSideThree,
            DiceSideFour,
            DiceSideFive,
            DiceSideSix
        };
        public static readonly Texture2D Hammer = ContentFinder<Texture2D>.Get("UI/Icons/Hammer");
        public static readonly Texture2D DropdownArrow = ContentFinder<Texture2D>.Get("UI/Icons/DropdownArrow");
        public static readonly Texture2D CloseButton = ContentFinder<Texture2D>.Get("UI/Widgets/CloseXSmall");
        public static readonly Texture2D MaximizeWindow = ContentFinder<Texture2D>.Get("UI/Icons/MaximizeWindow");
        public static readonly Texture2D RestoreWindow = ContentFinder<Texture2D>.Get("UI/Icons/RestoreWindow");
        public static readonly Texture2D CopySettings = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings");
        public static readonly Texture2D PasteSettings = ContentFinder<Texture2D>.Get("UI/Commands/PasteSettings");
        public static readonly Texture2D CloseGateway = ContentFinder<Texture2D>.Get("UI/Icons/CloseGateway");
        public static readonly Texture2D Snowman = ContentFinder<Texture2D>.Get("Things/Building/Art/Snowman/Snowman_D");
        public static readonly Texture2D HumanMeat;
        public static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Icons/Info");
        public static readonly Texture2D Warning = ContentFinder<Texture2D>.Get("UI/Icons/Warning");
        public static readonly Texture2D Debug = ContentFinder<Texture2D>.Get("UI/Icons/Debug");
        public static readonly Texture2D UtilsEdition = ContentFinder<Texture2D>.Get("UI/Icons/UtilsEdition");
        public static readonly Texture2D StandardEdition = ContentFinder<Texture2D>.Get("UI/Icons/StandardEdition");

        static Textures()
        {
            ThingDef humanMeat = DefDatabase<ThingDef>.GetNamed("Meat_Human");

            if (humanMeat.graphic is Graphic_Appearances graphic)
            {
                HumanMeat = graphic.SubGraphicFor(GenStuff.DefaultStuffFor(humanMeat)).MatAt(humanMeat.defaultPlacingRot).GetMaskTexture();
            }
            else
            {
                HumanMeat = humanMeat.uiIcon;
            }
        }
    }
}
