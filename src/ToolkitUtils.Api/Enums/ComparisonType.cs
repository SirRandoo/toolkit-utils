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

using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace ToolkitUtils.Api
{
    /// <summary>
    ///     An enum representing the various types of comparison operations
    ///     that can take place within certain sections of the mod.
    /// </summary>
    [EnumExtensions]
    public enum ComparisonType
    {
        /// <summary>
        ///     Describes an operation that determines if one object is greater
        ///     than another object.
        /// </summary>
        [Description(">")]
        Greater,

        /// <summary>
        ///     Describes an operation that determines if one object is less than
        ///     another object.
        /// </summary>
        [Description("<")]
        Less,

        /// <summary>
        ///     Describes an operation that determines if two objects are
        ///     considered equals.
        /// </summary>
        [Description("=")]
        Equal,

        /// <summary>
        ///     Describes an operation that determines if two objects are
        ///     considered equals, or one is greater than the other.
        /// </summary>
        [Description(">=")]
        GreaterEqual,

        /// <summary>
        ///     Describes an operation that determines if two objects are
        ///     considered equals, or one object is lesser than the other.
        /// </summary>
        [Description("<=")]
        LesserEqual,

        /// <summary>
        ///     Describes an operation that determines if two objects shouldn't
        ///     be considered equals.
        /// </summary>
        [Description("!=")]
        NotEqual
    }
}
