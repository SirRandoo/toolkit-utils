using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class Textures
    {
        internal static readonly Texture2D SortingAscend;
        internal static readonly Texture2D SortingDescend;
        internal static readonly Texture2D Gear;
        internal static readonly Texture2D Trash;
        internal static readonly Texture2D Refresh;
        internal static readonly Texture2D Edit;
        internal static readonly Texture2D Visible;
        internal static readonly Texture2D Hidden;
        internal static readonly Texture2D Reset;
        internal static readonly Texture2D QuestionMark;
        internal static readonly Texture2D Filter;
        internal static readonly Texture2D CollapsedArrow;
        internal static readonly Texture2D ExpandedArrow;
        internal static readonly Texture2D DiceSideOne;
        internal static readonly Texture2D DiceSideTwo;
        internal static readonly Texture2D DiceSideThree;
        internal static readonly Texture2D DiceSideFour;
        internal static readonly Texture2D DiceSideFive;
        internal static readonly Texture2D DiceSideSix;
        internal static readonly List<Texture2D> DiceSides;
        internal static readonly Texture2D Hammer;
        internal static readonly Texture2D DropdownArrow;

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

            DiceSides = new List<Texture2D>
            {
                DiceSideOne,
                DiceSideTwo,
                DiceSideThree,
                DiceSideFour,
                DiceSideFive,
                DiceSideSix
            };
        }
    }
}
