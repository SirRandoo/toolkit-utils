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
