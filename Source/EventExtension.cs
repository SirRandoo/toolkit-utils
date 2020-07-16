using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public enum EventTypes
    {
        None,

        /// <summary>
        /// Refers to events whose cost depends on the trait a user supplies
        /// to the event.
        /// </summary>
        /// <example>
        /// Utils' trait events are a good example of what this type refers to.
        /// The cost a viewer will pay for the event depends on what item the
        /// user supplies to the event. If a user requests the trait "brawler"
        /// be added to their pawn, and brawler costs 3,500 coins to add, the
        /// viewer would be charged 3,500 coins at some point before the event
        /// completes. The same can be said about removing the trait "brawler".
        /// </example>
        [UsedImplicitly]
        Trait,

        /// <summary>
        /// Refers to events whose cost depends on the pawn kind a user
        /// supplies to the event.
        /// </summary>
        /// <example>
        /// Utils' pawn event is a good example of what this type refers to.
        /// The cost a viewer will pay for the event depends on what item the
        /// user supplies to the event. If a user requests a tier 5 android,
        /// and the android costs 13,333 coins, the viewer would be charged
        /// 13,333 coins at some point before the event completes.
        /// </example>
        [UsedImplicitly]
        PawnKind,

        /// <summary>
        /// Refers to events whose cost depends on the item a user supplies
        /// to the event.
        /// </summary>
        /// <example>
        /// Utils' surgery event is a good example of what this type refers
        /// to. The cost a viewer will pay for the event depends on what item
        /// the user supplies to the command. If a user requests an archotech
        /// arm to be installed with said event, and the archotech arm costs
        /// 5,000 coins, the viewer would be charged 5,000 coins at some point
        /// before the event completes.
        /// </example>
        [UsedImplicitly]
        Item,

        /// <summary>
        /// Refers to events whose cost is managed by an external system.
        /// External in this context refers to a system that isn't Twitch
        /// Toolkit's.
        /// </summary>
        /// <remarks>
        /// Any misc events will not have their price displayed on ToolkitUtils'
        /// default item list. Developers will have to convey this information
        /// in another form.
        /// </remarks>
        [UsedImplicitly]
        Misc,

        /// <summary>
        /// Refers to events whose cost scales depending on some criteria.
        /// </summary>
        /// <example>
        /// Utils' full heal event is a good example of what this type
        /// refers to. The total cost a viewer will pay depends on how
        /// many injuries a viewer would heal. If the command heals 5
        /// injuries, and the event costs 500 coins, the viewer would
        /// be charged 2,500 coins at some point before the event completes.
        /// </example>
        [UsedImplicitly]
        Variable
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
