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

using System.IO;
using JetBrains.Annotations;
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


        [CanBeNull]
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
