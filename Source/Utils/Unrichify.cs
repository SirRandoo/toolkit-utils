using System.Linq;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class Unrichify
    {
        private static readonly string[] SupportedTags = {"b", "i", "size", "color", "material", "quad"};

        public static bool IsRichText(string input)
        {
            string lowered = input.ToLowerInvariant();

            return SupportedTags.Any(tag => lowered.Contains($"<{tag}") && lowered.Contains($"</{tag}>"));
        }

        public static string StripTags(string input)
        {
            string container = input;
            var i = 0;
            int expectedTags = Mathf.Min(
                input.Count(ch => ch.Equals('<')),
                input.Count(ch => ch.Equals('>'))
            );

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
