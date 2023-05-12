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

using System.Threading.Tasks;
using JetBrains.Annotations;
using Verse;

namespace ToolkitUtils.Wrappers.Async
{
    public static class TranslatorExtensions
    {
        /// <inheritdoc cref="Translator.TranslateSimple"/>
        public static async Task<string> TranslateSimpleAsync(this string key)
        {
            return await TaskExtensions.OnMainAsync(Translator.TranslateSimple, key);
        }

        /// <inheritdoc cref="TranslatorFormattedStringExtensions.Translate(string, NamedArgument)"/>
        [ItemNotNull]
        public static async Task<string> TranslateAsync(this string key, NamedArgument arg1)
        {
            return await TaskExtensions.OnMainAsync(TranslatorFormattedStringExtensions.Translate, key, arg1);
        }

        /// <inheritdoc cref="TranslatorFormattedStringExtensions.Translate(string, NamedArgument, NamedArgument)"/>
        [ItemNotNull]
        public static async Task<string> TranslateAsync(this string key, NamedArgument arg1, NamedArgument arg2)
        {
            return await TaskExtensions.OnMainAsync(TranslatorFormattedStringExtensions.Translate, key, arg1, arg2);
        }

        /// <inheritdoc cref="TranslatorFormattedStringExtensions.Translate(string, NamedArgument, NamedArgument, NamedArgument)"/>
        [ItemNotNull]
        public static async Task<string> TranslateAsync(this string key, NamedArgument arg1, NamedArgument arg2, NamedArgument arg3)
        {
            return await TaskExtensions.OnMainAsync(TranslatorFormattedStringExtensions.Translate, key, arg1, arg2, arg3);
        }

        /// <inheritdoc cref="Translator.CanTranslate"/>
        public static async Task<bool> CanTranslate(this string key)
        {
            return await TaskExtensions.OnMainAsync(Translator.CanTranslate, key);
        }

        /// <inheritdoc cref="Translator.TranslateWithBackup"/>
        [ItemNotNull]
        public static async Task<string> TranslateWithBackupAsync(this string key, TaggedString backupKey)
        {
            return await TaskExtensions.OnMainAsync(Translator.TranslateWithBackup, key, backupKey);
        }
    }
}
