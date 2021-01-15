using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
