using System.IO;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Windows;

namespace SirRandoo.ToolkitUtils.Models
{
    public enum ItemTargets
    {
        Global,
        Name,
        State,
        Price,
        Karma,
        Quantity,
        Stuff,
        Research
    }

    public class ItemEntry
    {
        public string FieldContents { get; set; }
        public FieldTypes FieldType { get; set; }
        public ItemTargets Target { get; set; }


        public static ItemEntry LoadScriptFromFile(string file)
        {
            if (!File.Exists(file))
            {
                LogHelper.Warn($"The specified script @ {file} does not exist.");
                return null;
            }

            using (FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    return new ItemEntry
                    {
                        FieldContents = reader.ReadToEnd(),
                        FieldType = FieldTypes.Script,
                        Target = ItemTargets.Global
                    };
                }
            }
        }
    }
}
