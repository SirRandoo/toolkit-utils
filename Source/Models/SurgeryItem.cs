using SirRandoo.ToolkitUtils.Utils.ModComp;
using Verse;

namespace SirRandoo.ToolkitUtils.Models
{
    public class SurgeryItem
    {
        public RecipeDef Surgery { get; set; }
        public bool IsForAndroids { get; set; }

        public bool IsAvailableOn(Pawn pawn)
        {
            if (IsForAndroids && Androids.IsAndroid(pawn))
            {
                return true;
            }

            return Surgery.AvailableOnNow(pawn);
        }
    }
}
