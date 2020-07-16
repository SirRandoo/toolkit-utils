using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum EventTypes
    {
        None,
        [UsedImplicitly] Trait,
        [UsedImplicitly] PawnKind,
        [UsedImplicitly] Item,
        [UsedImplicitly] Misc,
        [UsedImplicitly] Variable
    }

    [UsedImplicitly]
    public class EventExtension : DefModExtension
    {
        [Description("A brief explanation of what the event does.")]
        [DefaultValue(null)]
        [UsedImplicitly]
        public string Description;

        [Description("The product category this event's final price will be.")]
        [DefaultValue(EventTypes.None)]
        [UsedImplicitly]
        public EventTypes EventType;

        [Description("The parameters this event can take.")]
        [DefaultValue(null)]
        [UsedImplicitly]
        public List<Parameter> Parameters;
    }
}
