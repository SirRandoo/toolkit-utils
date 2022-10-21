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
    /// <summary>
    ///     A class for housing the various textures used throughout the
    ///     mod's UI.
    /// </summary>
    [StaticConstructorOnStartup]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class Textures
    {
        internal static readonly Texture2D SortingAscend = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
        internal static readonly Texture2D SortingDescend = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
        internal static readonly Texture2D Gear = ContentFinder<Texture2D>.Get("UI/Icons/Gear");
        internal static readonly Texture2D Trash = ContentFinder<Texture2D>.Get("UI/Icons/Trash");
        internal static readonly Texture2D Refresh = ContentFinder<Texture2D>.Get("UI/Icons/Refresh");
        internal static readonly Texture2D Edit = ContentFinder<Texture2D>.Get("UI/Icons/Edit");
        internal static readonly Texture2D Visible = ContentFinder<Texture2D>.Get("UI/Icons/Visible");
        internal static readonly Texture2D Hidden = ContentFinder<Texture2D>.Get("UI/Icons/Hidden");
        internal static readonly Texture2D Stack = ContentFinder<Texture2D>.Get("UI/Icons/Stack");
        internal static readonly Texture2D Reset = ContentFinder<Texture2D>.Get("UI/Icons/Reset");
        internal static readonly Texture2D QuestionMark = ContentFinder<Texture2D>.Get("UI/Icons/QuestionMark");
        internal static readonly Texture2D Group = ContentFinder<Texture2D>.Get("UI/Icons/Group");
        internal static readonly Texture2D Filter = ContentFinder<Texture2D>.Get("UI/Icons/Filter");
        internal static readonly Texture2D CollapsedArrow = ContentFinder<Texture2D>.Get("UI/Icons/CollapsedArrow");
        internal static readonly Texture2D ExpandedArrow = ContentFinder<Texture2D>.Get("UI/Icons/ExpandedArrow");
        internal static readonly Texture2D DiceSideOne = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideOne");
        internal static readonly Texture2D DiceSideTwo = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideTwo");
        internal static readonly Texture2D DiceSideThree = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideThree");
        internal static readonly Texture2D DiceSideFour = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideFour");
        internal static readonly Texture2D DiceSideFive = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideFive");
        internal static readonly Texture2D DiceSideSix = ContentFinder<Texture2D>.Get("UI/Icons/DiceSideSix");
        internal static readonly List<Texture2D> DiceSides = new List<Texture2D>
        {
            DiceSideOne,
            DiceSideTwo,
            DiceSideThree,
            DiceSideFour,
            DiceSideFive,
            DiceSideSix
        };
        internal static readonly Texture2D Hammer = ContentFinder<Texture2D>.Get("UI/Icons/Hammer");
        internal static readonly Texture2D DropdownArrow = ContentFinder<Texture2D>.Get("UI/Icons/DropdownArrow");
        internal static readonly Texture2D CloseButton = ContentFinder<Texture2D>.Get("UI/Widgets/CloseXSmall");
        internal static readonly Texture2D MaximizeWindow = ContentFinder<Texture2D>.Get("UI/Icons/MaximizeWindow");
        internal static readonly Texture2D RestoreWindow = ContentFinder<Texture2D>.Get("UI/Icons/RestoreWindow");
        internal static readonly Texture2D CopySettings = ContentFinder<Texture2D>.Get("UI/Commands/CopySettings");
        internal static readonly Texture2D PasteSettings = ContentFinder<Texture2D>.Get("UI/Commands/PasteSettings");
        internal static readonly Texture2D CloseGateway = ContentFinder<Texture2D>.Get("UI/Icons/CloseGateway");
        internal static readonly Texture2D Snowman = ContentFinder<Texture2D>.Get("Things/Building/Art/Snowman/Snowman_D");
        internal static readonly Texture2D HumanMeat = GetHumanMeatTexture();
        internal static readonly Texture2D Info = ContentFinder<Texture2D>.Get("UI/Icons/Info");
        internal static readonly Texture2D Warning = ContentFinder<Texture2D>.Get("UI/Icons/Warning");
        internal static readonly Texture2D Debug = ContentFinder<Texture2D>.Get("UI/Icons/Debug");
        internal static readonly Texture2D Sword = ContentFinder<Texture2D>.Get("UI/Icons/Sword");
        internal static readonly Texture2D SlashedCircle = ContentFinder<Texture2D>.Get("UI/Icons/SlashedCircle");
        internal static readonly Texture2D ArrowGhost = ContentFinder<Texture2D>.Get("UI/Overlays/ArrowGhost");

        private static Texture2D GetHumanMeatTexture()
        {
            ThingDef humanMeat = DefDatabase<ThingDef>.GetNamed("Meat_Human");

            if (humanMeat.graphic is Graphic_Appearances graphic)
            {
                return graphic.SubGraphicFor(GenStuff.DefaultStuffFor(humanMeat)).MatAt(humanMeat.defaultPlacingRot).GetMaskTexture();
            }

            return humanMeat.uiIcon;
        }
    }
}
