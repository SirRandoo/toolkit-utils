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
