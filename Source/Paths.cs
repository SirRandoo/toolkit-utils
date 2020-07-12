using System.IO;
using TwitchToolkit.Utilities;

namespace SirRandoo.ToolkitUtils
{
    public static class Paths
    {
        public static readonly string TraitFilePath = Path.Combine(SaveHelper.dataPath, "traits.json");
        public static readonly string ModListFilePath = Path.Combine(SaveHelper.dataPath, "modlist.json");
        public static readonly string LegacyShopFilePath = Path.Combine(SaveHelper.dataPath, "ShopExt.xml");
        public static readonly string ItemDataFilePath = Path.Combine(SaveHelper.dataPath, "itemdata.json");
        public static readonly string PawnKindFilePath = Path.Combine(SaveHelper.dataPath, "pawnkinds.json");
        public static readonly string CommandListFilePath = Path.Combine(SaveHelper.dataPath, "commands.json");
        public static readonly string LegacyShopDumpFilePath = Path.Combine(SaveHelper.dataPath, "ShopExt.json");
        public static readonly string ToolkitItemFilePath = Path.Combine(SaveHelper.dataPath, "StoreItems.json");
    }
}
