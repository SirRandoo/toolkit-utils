// MIT License
// 
// Copyright (c) 2023 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using JetBrains.Annotations;
using ToolkitUtils.Data.Models;
using UnityEngine;
using Verse;

namespace ToolkitUtils.Api
{
    [StaticConstructorOnStartup]
    public static class ColorRegistry
    {
        private static readonly List<NamedColor> Registry = new List<NamedColor>();
        private static readonly Dictionary<string, NamedColor> RegistryKeyed = new Dictionary<string, NamedColor>();
        private static readonly Dictionary<string, NamedColor> RegistryNameKeyed = new Dictionary<string, NamedColor>();

        static ColorRegistry()
        {
            Register(new NamedColor("Red", Color.red));
            Register(new NamedColor("Yellow", Color.yellow));
            Register(new NamedColor("Green", Color.green));
            Register(new NamedColor("Blue", Color.blue));
            Register(new NamedColor("Black", Color.black));
            Register(new NamedColor("Cyan", Color.cyan));
            Register(new NamedColor("Magenta", Color.magenta));
            Register(new NamedColor("Gray", Color.gray));
            Register(new NamedColor("Grey", Color.grey));

            Register(new NamedColor("Aqua", ColorLibrary.Aqua));
            Register(new NamedColor("Aquamarine", ColorLibrary.Aquamarine));
            Register(new NamedColor("Baby Blue", ColorLibrary.BabyBlue));
            Register(new NamedColor("Beige", ColorLibrary.Beige));
            Register(new NamedColor("Blue Green", ColorLibrary.BlueGreen));
            Register(new NamedColor("Brick Red", ColorLibrary.BrickRed));
            Register(new NamedColor("Bright Blue", ColorLibrary.BrightBlue));
            Register(new NamedColor("Bright Green", ColorLibrary.BrightGreen));
            Register(new NamedColor("Bright Pink", ColorLibrary.BrightPink));
            Register(new NamedColor("Bright Purple", ColorLibrary.BrightPurple));
            Register(new NamedColor("Brown", ColorLibrary.Brown));
            Register(new NamedColor("Burgundy", ColorLibrary.Burgundy));
            Register(new NamedColor("Burnt Orange", ColorLibrary.BurntOrange));
            Register(new NamedColor("Dark Blue", ColorLibrary.DarkBlue));
            Register(new NamedColor("Dark Brown", ColorLibrary.DarkBrown));
            Register(new NamedColor("Dark Green", ColorLibrary.DarkGreen));
            Register(new NamedColor("Dark Orange", ColorLibrary.DarkOrange));
            Register(new NamedColor("Dark Pink", ColorLibrary.DarkPink));
            Register(new NamedColor("Dark Purple", ColorLibrary.DarkPurple));
            Register(new NamedColor("Dark Red", ColorLibrary.DarkRed));
            Register(new NamedColor("Dark Teal", ColorLibrary.DarkTeal));
            Register(new NamedColor("Deep Purple", ColorLibrary.DeepPurple));
            Register(new NamedColor("Forest Green", ColorLibrary.ForestGreen));
            Register(new NamedColor("Gold", ColorLibrary.Gold));
            Register(new NamedColor("Grass Green", ColorLibrary.GrassGreen));
            Register(new NamedColor("Hot Pink", ColorLibrary.HotPink));
            Register(new NamedColor("Indigo", ColorLibrary.Indigo));
            Register(new NamedColor("Khaki", ColorLibrary.Khaki));
            Register(new NamedColor("Lavender", ColorLibrary.Lavender));
            Register(new NamedColor("Leather", ColorLibrary.Leather));
            Register(new NamedColor("Light Blue", ColorLibrary.LightBlue));
            Register(new NamedColor("Light Brown", ColorLibrary.LightBrown));
            Register(new NamedColor("Light Green", ColorLibrary.LightGreen));
            Register(new NamedColor("Light Orange", ColorLibrary.LightOrange));
            Register(new NamedColor("Light Pink", ColorLibrary.LightPink));
            Register(new NamedColor("Light Purple", ColorLibrary.LightPurple));
            Register(new NamedColor("Light Lilac", ColorLibrary.Lilac));
            Register(new NamedColor("Lime", ColorLibrary.Lime));
            Register(new NamedColor("Lime Green", ColorLibrary.LimeGreen));
            Register(new NamedColor("Maroon", ColorLibrary.Maroon));
            Register(new NamedColor("Mauve", ColorLibrary.Mauve));
            Register(new NamedColor("Mint", ColorLibrary.Mint));
            Register(new NamedColor("Mustard", ColorLibrary.Mustard));
            Register(new NamedColor("Navy", ColorLibrary.Navy));
            Register(new NamedColor("Navy Blue", ColorLibrary.NavyBlue));
            Register(new NamedColor("Neon Green", ColorLibrary.NeonGreen));
            Register(new NamedColor("Olive", ColorLibrary.Olive));
            Register(new NamedColor("Olive Green", ColorLibrary.OliveGreen));
            Register(new NamedColor("Orange", ColorLibrary.Orange));
            Register(new NamedColor("Pale Blue", ColorLibrary.PaleBlue));
            Register(new NamedColor("Pale Green", ColorLibrary.PaleGreen));
            Register(new NamedColor("Pastel Green", ColorLibrary.PastelGreen));
            Register(new NamedColor("Peach", ColorLibrary.Peach));
            Register(new NamedColor("Pea Green", ColorLibrary.PeaGreen));
            Register(new NamedColor("Pink", ColorLibrary.Pink));
            Register(new NamedColor("Plum", ColorLibrary.Plum));
            Register(new NamedColor("Puke Green", ColorLibrary.PukeGreen));
            Register(new NamedColor("Purple", ColorLibrary.Purple));
            Register(new NamedColor("Rose", ColorLibrary.Rose));
            Register(new NamedColor("Royal Blue", ColorLibrary.RoyalBlue));
            Register(new NamedColor("Royal Purple", ColorLibrary.RoyalPurple));
            Register(new NamedColor("Salmon", ColorLibrary.Salmon));
            Register(new NamedColor("Sand", ColorLibrary.Sand));
            Register(new NamedColor("Sky Blue", ColorLibrary.SkyBlue));
            Register(new NamedColor("Tan", ColorLibrary.Tan));
            Register(new NamedColor("Taupe", ColorLibrary.Taupe));
            Register(new NamedColor("Teal", ColorLibrary.Teal));
            Register(new NamedColor("Turquoise", ColorLibrary.Turquoise));
            Register(new NamedColor("Violet", ColorLibrary.Violet));
            Register(new NamedColor("Yellow Green", ColorLibrary.YellowGreen));
        }

        /// <summary>
        ///     Returns an enumerable of the named colors registered within the
        ///     registry.
        /// </summary>
        [NotNull]
        public static IReadOnlyList<NamedColor> AllRegistrants => Registry;

        /// <summary>
        ///     Returns a color with the given id.
        /// </summary>
        /// <param name="id">The id of the color being requested.</param>
        [CanBeNull]
        public static NamedColor Get([NotNull] string id) => RegistryKeyed.TryGetValue(id, out NamedColor color) ? color : default;

        /// <summary>
        ///     Returns a color with a specific name.
        /// </summary>
        /// <param name="name">
        ///     The case-sensitive name of the color being
        ///     requested.
        /// </param>
        [CanBeNull]
        public static NamedColor GetNamed([NotNull] string name) => RegistryNameKeyed.TryGetValue(name, out NamedColor color) ? color : default;

        /// <summary>
        ///     Registers a named color within the registry.
        /// </summary>
        /// <param name="obj">The named color to register.</param>
        /// <remarks>
        ///     This method is a convenience method used internally to register
        ///     the color to all 3 internal collections. Developers should not
        ///     use this method as the color registry doesn't lock itself from
        ///     other callers accessing its internal collections, specifically
        ///     modifying them, and as a result you should only register new
        ///     colors at startup and with great care.
        /// </remarks>
        private static void Register([NotNull] NamedColor obj)
        {
            if (!RegistryKeyed.ContainsKey(obj.Id))
            {
                return;
            }

            Registry.Add(obj);
            RegistryKeyed.Add(obj.Id, obj);
            RegistryNameKeyed.Add(obj.Name, obj);
        }
    }
}
