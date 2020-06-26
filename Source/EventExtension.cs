using System.Collections.Generic;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum EventTypes
    {
        Void, Trait, Race,
        Item
    }

    public class EventExtension : DefModExtension
    {
        [Description("A brief explanation of what the event does.")]
        [DefaultValue(null)]
        public string Description;

        [Description("The product category this event's final price will be.")]
        [DefaultValue(EventTypes.Void)]
        public EventTypes EventType;

        [Description("The parameters this event can take.")]
        [DefaultValue(null)]
        public List<Parameter> Parameters;
    }
}
