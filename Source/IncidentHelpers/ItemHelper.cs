using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Utils;
using ToolkitCore.Utilities;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentHelpers
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public class ItemHelper : IncidentHelperVariables
    {
        private static readonly ThingDef DropPod;
        private static readonly IncidentDef FarmAnimals;
        private PurchaseRequest purchaseRequest;

        static ItemHelper()
        {
            DropPod = ThingDef.Named("DropPodIncoming");
            FarmAnimals = IncidentDef.Named("FarmAnimalsWanderIn");
        }

        public override Viewer Viewer { get; set; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        public override bool IsPossible(string message, Viewer viewer, bool separateChannel = false)
        {
            string[] segments = CommandFilter.Parse(message).Skip(2).ToArray();
            string item = segments.FirstOrFallback();
            string quantity = segments.Skip(1).FirstOrFallback();

            if (item.NullOrEmpty())
            {
                return false;
            }

            if (!int.TryParse(quantity, out int amount))
            {
                amount = 1;
            }

            Item product = StoreInventory.items
                .Where(i => i.price > 0)
                .FirstOrDefault(i => i.defname.EqualsIgnoreCase(item) || i.abr.EqualsIgnoreCase(item));

            if (product == null)
            {
                MessageHelper.ReplyToUser(viewer.username, "TKUtils.");
                return false;
            }

            return false;
        }

        public override void TryExecute()
        {
        }
    }

    internal class PurchaseRequest
    {
        public int Price { get; set; }
        public int Quantity { get; set; }
        public Item ItemData { get; set; }
        public ThingDef ThingDef { get; set; }
    }
}
