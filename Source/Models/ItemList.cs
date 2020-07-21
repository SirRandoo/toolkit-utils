using System.Collections.Generic;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Models
{
    [UsedImplicitly]
    public class ItemList
    {
        public List<Item> Items;
        public int Total => Items.Count;
    }
}
