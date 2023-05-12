﻿// MIT License
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

using System;
using Verse;

namespace ToolkitUtils.Data.Models
{
    /// <summary>
    ///     A compatibility wrapper used by <see cref="Def"/> objects. This
    ///     wrapper exists as a way of porting RimWorld <see cref="Def"/>s
    ///     into the mod's systems in a compatible way, like for use in
    ///     registries.
    /// </summary>
    /// <typeparam name="T">The type of def being wrapped.</typeparam>
    public class DefWrapper<T> : IIdentifiable where T : Def
    {
        private readonly T _def;

        public DefWrapper(T def)
        {
            _def = def;
        }

        /// <inheritdoc/>
        public string Id => _def.defName;

        /// <inheritdoc/>
        public string Name
        {
            get => _def.label;
            set => throw new NotSupportedException("Cannot rename defs from a wrapper.");
        }
    }
}
