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
            var lowered = input.ToLowerInvariant();

            return SupportedTags.Any(tag => lowered.Contains($"<{tag}") && lowered.Contains($"</{tag}>"));
        }

        public static string StripTags(string input)
        {
            var container = input;
            var i = 0;
            var expectedTags = Mathf.Min(
                input.Count(ch => ch.Equals('<')),
                input.Count(ch => ch.Equals('>'))
            );

            while (IsRichText(container))
            {
                var nameEnd = false;
                var inTag = false;
                var tagContent = "";
                var tag = "";

                foreach (var c in container)
                {
                    if(c.Equals('<') && tagContent == "")
                    {
                        inTag = true;
                    }
                    else if(c.Equals(' ') && inTag)
                    {
                        nameEnd = true;
                        tagContent += c;
                    }
                    else if(c.Equals('>'))
                    {
                        inTag = false;
                    }
                    else if(inTag)
                    {
                        tagContent += c;

                        if(!nameEnd)
                        {
                            tag += c;
                        }
                    }
                }

                if (!tagContent.NullOrEmpty())
                {
                    container = container.Replace($"<{tagContent}>", "");
                    container = container.Replace($"</{tag}>", "");
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
