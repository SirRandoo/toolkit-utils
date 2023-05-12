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
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ToolkitUtils.Wrappers.Async
{
    public static class LetterStackExtensions
    {
        public static async Task RemoveLetterAsync([NotNull] this LetterStack stack, Letter letter)
        {
            await TaskExtensions.OnMainAsync(stack.RemoveLetter, letter);
        }

        public static async Task ReceiveLetterAsync([NotNull] this LetterStack stack, Letter letter)
        {
            await TaskExtensions.OnMainAsync(stack.ReceiveLetter, letter, (string)null);
        }

        public static async Task ReceiveLetterAsync([NotNull] this LetterStack stack, Letter letter, string debugInfo)
        {
            await TaskExtensions.OnMainAsync(stack.ReceiveLetter, letter, debugInfo);
        }

        public static async Task ReceiveLetterAsync([NotNull] this LetterStack stack, TaggedString label, TaggedString text, LetterDef def)
        {
            await TaskExtensions.OnMainAsync(stack.ReceiveLetter, label, text, def, (string)null);
        }

        public static async Task ReceiveLetterAsync([NotNull] this LetterStack stack, TaggedString label, TaggedString text, LetterDef def, string debugInfo)
        {
            await TaskExtensions.OnMainAsync(stack.ReceiveLetter, label, text, def, debugInfo);
        }

        public static async Task ReceiveLetterAsync([NotNull] this LetterStack stack, TaggedString label, TaggedString text, LetterDef def, LookTargets targets)
        {
            await TaskExtensions.OnMainAsync(stack.ReceiveLetter, label, text, def, targets, (Faction)null, (Quest)null, (List<ThingDef>)null, (string)null);
        }
    }
}
