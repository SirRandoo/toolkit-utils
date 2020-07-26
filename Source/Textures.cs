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
        }
    }
}
