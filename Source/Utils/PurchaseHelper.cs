using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class PurchaseHelper
    {
        public static bool CanAfford(this Viewer v, int price)
        {
            return v.GetViewerCoins() >= price || ToolkitSettings.UnlimitedCoins;
        }
    }
}
