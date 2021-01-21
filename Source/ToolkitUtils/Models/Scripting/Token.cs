namespace SirRandoo.ToolkitUtils.Models.Scripting
{
    public sealed class Token
    {
        public enum Types
        {
            Global,
            Function,
            Variable,
            BuiltIn,
            Comment,
            String,
            Symbol,
            Number,
            Keyword
        }

        public Types Type { get; set; }
        public string Pattern { get; set; }
    }
}
