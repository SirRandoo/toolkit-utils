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

using System.Linq;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class Unrichify
    {
        private static readonly string[] SupportedTags;

        static Unrichify()
        {
            SupportedTags = new[] {"b", "i", "size", "color", "material", "quad"};
        }

        public static bool IsRichText(string input)
        {
            string lowered = input.ToLowerInvariant();

            return SupportedTags.Any(tag => lowered.Contains($"<{tag}") && lowered.Contains($"</{tag}>"));
        }

        public static string StripTags(string input)
        {
            string container = input;
            var i = 0;
            int expectedTags = Mathf.Min(input.Count(ch => ch.Equals('<')), input.Count(ch => ch.Equals('>')));

            while (IsRichText(container))
            {
                var nameEnd = false;
                var inTag = false;
                var tagContent = "";
                var tag = "";

                foreach (char c in container)
                {
                    switch (c)
                    {
                        case '<' when tagContent == "":
                            inTag = true;
                            break;
                        case '=' when inTag:
                            nameEnd = true;
                            tagContent += c.ToString();
                            break;
                        case '>':
                            inTag = false;
                            break;
                        default:
                        {
                            if (inTag)
                            {
                                tagContent += c.ToString();

                                if (!nameEnd)
                                {
                                    tag += c.ToString();
                                }
                            }

                            break;
                        }
                    }
                }

                if (!tagContent.NullOrEmpty())
                {
                    container = container.ReplaceFirst($"<{tagContent}>", "");
                    container = container.ReplaceFirst($"</{tag}>", "");
                }

                i++;

                // While this may not catch everything, this should help prevent infinite loops. For
                // the types of content this method will be used for, any strings that contain more
                // than 10 tags is questionable.
                if ((container.Contains("<") || container.Contains(">")) && i > expectedTags)
                {
                    return container;
                }
            }

            return container;
        }
    }
}
