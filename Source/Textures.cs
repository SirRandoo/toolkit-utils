using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public class Textures
    {
        internal static readonly Texture2D SortingAscend;
        internal static readonly Texture2D SortingDescend;

        static Textures()
        {
            SortingAscend = ContentFinder<Texture2D>.Get("UI/Icons/Sorting");
            SortingDescend = ContentFinder<Texture2D>.Get("UI/Icons/SortingDescending");
        }
    }
}
