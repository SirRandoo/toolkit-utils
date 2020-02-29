using System.Linq;

using UnityEngine;

using Verse;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class RichTextUnparser
    {
        private static readonly string[] supportedTags = new[] { "b", "i", "size", "color", "material", "quad" };

        public static bool IsRichText(string input)
        {
            var lowered = input.ToLowerInvariant();

            foreach(var tag in supportedTags)
            {
                if(lowered.Contains($"<{tag}") && lowered.Contains($"</{tag}>"))
                {
                    return true;
                }
            }

            return false;
        }

        public static string StripTags(string input)
        {
            var container = input;
            var i = 0;
            var expectedTags = Mathf.Min(
                input.Count(ch => ch.Equals('<')),
                input.Count(ch => ch.Equals('>'))
            );

            while(IsRichText(container))
            {
                var inTag = false;
                var tagContent = "";

                foreach(var c in container)
                {
                    if(c.Equals('<'))
                    {
                        inTag = true;
                    }
                    else if(inTag)
                    {
                        tagContent += c;
                    }
                    else if(c.Equals('/'))
                    {
                        inTag = false;
                    }
                }

                if(!tagContent.NullOrEmpty())
                {
                    container = container.Replace($"<{tagContent}>", "");
                    container = container.Replace($"</{tagContent}>", "");
                }

                i++;

                // While this may not catch everything, this should help prevent infinite loops. For
                // the types of content this method will be used for, any strings that contain more
                // than 10 tags is questionable.
                if((container.Contains("<") || container.Contains(">")) && i > expectedTags)
                {
                    Log.Message($"<color=red>WARN {TKUtils.ID} :: Attempted to unrichify string \"{input}\", but exceeded {expectedTags} attempts.</color>");
                    return container;
                }
            }

            return container;
        }
    }
}
