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
using TwitchToolkit.Utilities;

namespace SirRandoo.ToolkitUtils
{
    public static class Paths
    {
        public static readonly string TraitFilePath;
        public static readonly string ModListFilePath;
        public static readonly string LegacyShopFilePath;
        public static readonly string ItemDataFilePath;
        public static readonly string PawnKindFilePath;
        public static readonly string CommandListFilePath;
        public static readonly string LegacyShopDumpFilePath;
        public static readonly string ToolkitItemFilePath;

        static Paths()
        {
            ToolkitItemFilePath = Path.Combine(SaveHelper.dataPath, "StoreItems.json");
            LegacyShopDumpFilePath = Path.Combine(SaveHelper.dataPath, "ShopExt.json");
            CommandListFilePath = Path.Combine(SaveHelper.dataPath, "commands.json");
            PawnKindFilePath = Path.Combine(SaveHelper.dataPath, "pawnkinds.json");
            ItemDataFilePath = Path.Combine(SaveHelper.dataPath, "itemdata.json");
            LegacyShopFilePath = Path.Combine(SaveHelper.dataPath, "ShopExt_1.xml");
            ModListFilePath = Path.Combine(SaveHelper.dataPath, "modlist.json");
            TraitFilePath = Path.Combine(SaveHelper.dataPath, "traits.json");
        }
    }
}
