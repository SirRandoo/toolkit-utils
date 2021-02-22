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

using System.Collections.Generic;

namespace SirRandoo.ToolkitUtils.Models.Scripting
{
    internal static class Definitions
    {
        public static readonly List<string> Keywords = new List<string>
        {
            "and",
            "break",
            "do",
            "else",
            "elseif",
            "end",
            "false",
            "for",
            "function",
            "goto",
            "if",
            "in",
            "local",
            "nil",
            "not",
            "or",
            "repeat",
            "return",
            "then",
            "true",
            "until",
            "while"
        };

        public static readonly List<string> ArithmeticOperators = new List<string>
        {
            "+",
            "-",
            "*",
            "/",
            "//",
            "%",
            "^"
        };

        public static readonly List<string> BitwiseOperators = new List<string>
        {
            "&",
            "|",
            "~",
            ">>",
            "<<",
            "~"
        };

        public static readonly List<string> RelationalOperators = new List<string>
        {
            "==",
            "~=",
            "<",
            ">",
            "<=",
            ">="
        };
    }
}
