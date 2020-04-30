using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class StoreIncidentEditor : TwitchToolkit.Windows.StoreIncidentEditor
    {
        public StoreIncidentEditor(StoreIncident storeIncident) : base(storeIncident)
        {
        }

        public override void PostOpen()
        {
            base.PostOpen();

            if (storeIncidentVariables == null)
            {
                return;
            }

            if (storeIncidentVariables.defName.Equals("AddTrait") && TkSettings.UtilsNoticeAdd)
            {
                Find.WindowStack.Add(new NoticeWindow(storeIncidentVariables.defName));
            }

            if (storeIncidentVariables.defName.Equals("RemoveTrait") && TkSettings.UtilsNoticeRemove)
            {
                Find.WindowStack.Add(new NoticeWindow(storeIncidentVariables.defName));
            }

            if (storeIncidentVariables.defName.Equals("BuyPawn") && TkSettings.UtilsNoticePawn)
            {
                Find.WindowStack.Add(new NoticeWindow(storeIncidentVariables.defName));
            }
        }

        public override void PreClose()
        {
            base.PreClose();

            if (storeIncidentVariables == null)
            {
                return;
            }

            if (storeIncidentVariables.cost <= 1)
            {
                return;
            }

            switch (storeIncidentVariables.defName)
            {
                case "AddTrait":
                    foreach (var trait in TkUtils.ShopExpansion.Traits)
                    {
                        trait.AddPrice = storeIncidentVariables.cost;
                    }

                    storeIncidentVariables.cost = 1;
                    return;
                case "RemoveTrait":
                    foreach (var trait in TkUtils.ShopExpansion.Traits)
                    {
                        trait.RemovePrice = storeIncidentVariables.cost;
                    }

                    storeIncidentVariables.cost = 1;
                    return;
                case "BuyPawn":
                    foreach (var race in TkUtils.ShopExpansion.Races)
                    {
                        race.Price = storeIncidentVariables.cost;
                    }

                    storeIncidentVariables.cost = 1;
                    return;
            }
        }
    }
}
