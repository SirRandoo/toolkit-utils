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
        public static readonly Texture2D SortingAscend;
        public static readonly Texture2D SortingDescend;
        public static readonly Texture2D Gear;
        public static readonly Texture2D Trash;
        public static readonly Texture2D Refresh;
        public static readonly Texture2D Edit;
        public static readonly Texture2D Visible;
        public static readonly Texture2D Hidden;
        public static readonly Texture2D Stack;
        public static readonly Texture2D Reset;
        public static readonly Texture2D QuestionMark;
        public static readonly Texture2D Filter;
        public static readonly Texture2D CollapsedArrow;
        public static readonly Texture2D ExpandedArrow;
        public static readonly Texture2D DiceSideOne;
        public static readonly Texture2D DiceSideTwo;
        public static readonly Texture2D DiceSideThree;
        public static readonly Texture2D DiceSideFour;
        public static readonly Texture2D DiceSideFive;
        public static readonly Texture2D DiceSideSix;
        public static readonly List<Texture2D> DiceSides;
        public static readonly Texture2D Hammer;
        public static readonly Texture2D DropdownArrow;
        public static readonly Texture2D CloseButton;
        public static readonly Texture2D MaximizeWindow;
        public static readonly Texture2D RestoreWindow;
        public static readonly Texture2D CopySettings;
        public static readonly Texture2D PasteSettings;
        public static readonly Texture2D CloseGateway;
        public static readonly Texture2D Snowman;
        public static readonly Texture2D HumanMeat;
        public static readonly Texture2D Info;
        public static readonly Texture2D Warning;
        public static readonly Texture2D Debug;
        public static readonly Texture2D UtilsEdition;
        public static readonly Texture2D StandardEdition;

        static Textures()
        {
            Gear = ContentFinder<Texture2D>.Get("UI/Icons/Gear");
            Trash = ContentFinder<Texture2D>.Get("UI/Icons/Trash");
            Refresh = ContentFinder<Texture2D>.Get("UI/Icons/Refresh");
            SortingAscend = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
            SortingDescend = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
            Edit = ContentFinder<Texture2D>.Get("UI/Icons/Edit");
            Visible = ContentFinder<Texture2D>.Get("UI/Icons/Visible");
            Hidden = ContentFinder<Texture2D>.Get("UI/Icons/Hidden");
            Reset = ContentFinder<Texture2D>.Get("UI/Icons/Reset");
            Filter = ContentFinder<Texture2D>.Get("UI/Icons/Filter");
            QuestionMark = ContentFinder<Texture2D>.Get("UI/Icons/QuestionMark");
            CollapsedArrow = ContentFinder<Texture2D>.Get("UI/Icons/CollapsedArrow");
            ExpandedArrow = ContentFinder<Texture2D>.Get("UI/Icons/ExpandedArrow");
            DiceSideOne = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideOne");
            DiceSideTwo = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideTwo");
            DiceSideThree = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideThree");
            DiceSideFour = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideFour");
            DiceSideFive = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideFive");
            DiceSideSix = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideSix");
            Hammer = ContentFinder<Texture2D>.Get("UI/Icons/Hammer");
            DropdownArrow = ContentFinder<Texture2D>.Get("UI/Icons/DropdownArrow");
            CloseButton = ContentFinder<Texture2D>.Get("UI/Widgets/CloseXSmall");
            MaximizeWindow = ContentFinder<Texture2D>.Get("UI/Icons/MaximizeWindow");
            RestoreWindow = ContentFinder<Texture2D>.Get("UI/Icons/RestoreWindow");
            Stack = ContentFinder<Texture2D>.Get("UI/Icons/Stack");
            CopySettings = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings");
            PasteSettings = ContentFinder<Texture2D>.Get("UI/Commands/PasteSettings");
            CloseGateway = ContentFinder<Texture2D>.Get("UI/Icons/CloseGateway");
            Snowman = ContentFinder<Texture2D>.Get("Things/Building/Art/Snowman/Snowman_D");
            Debug = ContentFinder<Texture2D>.Get("UI/Icons/Debug");
            Warning = ContentFinder<Texture2D>.Get("UI/Icons/Warning");
            Info = ContentFinder<Texture2D>.Get("UI/Icons/Info");
            UtilsEdition = ContentFinder<Texture2D>.Get("UI/Icons/UtilsEdition");
            StandardEdition = ContentFinder<Texture2D>.Get("UI/Icons/StandardEdition");

            DiceSides = new List<Texture2D>
            {
                DiceSideOne,
                DiceSideTwo,
                DiceSideThree,
                DiceSideFour,
                DiceSideFive,
                DiceSideSix
            };


            ThingDef humanMeat = DefDatabase<ThingDef>.GetNamed("Meat_Human");
            if (humanMeat.graphic is Graphic_Appearances graphic)
            {
                HumanMeat = graphic.SubGraphicFor(GenStuff.DefaultStuffFor(humanMeat))
                   .MatAt(humanMeat.defaultPlacingRot)
                   .GetMaskTexture();
            }
            else
            {
                HumanMeat = humanMeat.uiIcon;
            }
        }
    }
}
