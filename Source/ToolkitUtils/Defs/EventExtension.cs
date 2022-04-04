// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum EventTypes
    {
        Default,

        /// <summary>
        ///     Refers to events whose cost depends on the trait a user supplies
        ///     to the event.
        /// </summary>
        /// <example>
        ///     Utils' trait events are a good example of what this type refers
        ///     to.
        ///     The cost a viewer will pay for the event depends on what item the
        ///     user supplies to the event. If a user requests the trait
        ///     "brawler"
        ///     be added to their pawn, and brawler costs 3,500 coins to add, the
        ///     viewer would be charged 3,500 coins at some point before the
        ///     event
        ///     completes. The same can be said about removing the trait
        ///     "brawler".
        /// </example>
        Trait,

        /// <summary>
        ///     Refers to events whose cost depends on the pawn kind a user
        ///     supplies to the event.
        /// </summary>
        /// <example>
        ///     Utils' pawn event is a good example of what this type refers to.
        ///     The cost a viewer will pay for the event depends on what item the
        ///     user supplies to the event. If a user requests a tier 5 android,
        ///     and the android costs 13,333 coins, the viewer would be charged
        ///     13,333 coins at some point before the event completes.
        /// </example>
        PawnKind,

        /// <summary>
        ///     Refers to events whose cost depends on the item a user supplies
        ///     to the event.
        /// </summary>
        /// <example>
        ///     Utils' surgery event is a good example of what this type refers
        ///     to. The cost a viewer will pay for the event depends on what item
        ///     the user supplies to the command. If a user requests an archotech
        ///     arm to be installed with said event, and the archotech arm costs
        ///     5,000 coins, the viewer would be charged 5,000 coins at some
        ///     point
        ///     before the event completes.
        /// </example>
        Item,

        /// <summary>
        ///     Refers to events whose cost is managed by an external system.
        ///     External in this context refers to a system that isn't Twitch
        ///     Toolkit's.
        /// </summary>
        /// <remarks>
        ///     Any misc events will not have their price displayed on
        ///     ToolkitUtils'
        ///     default item list. Developers will have to convey this
        ///     information
        ///     in another form.
        /// </remarks>
        Misc,

        /// <summary>
        ///     Refers to events whose cost scales depending on some criteria.
        /// </summary>
        /// <example>
        ///     Utils' full heal event is a good example of what this type
        ///     refers to. The total cost a viewer will pay depends on how
        ///     many injuries a viewer would heal. If the command heals 5
        ///     injuries, and the event costs 500 coins, the viewer would
        ///     be charged 2,500 coins at some point before the event completes.
        /// </example>
        Variable
    }

    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class EventExtension : DefModExtension
    {
        [Description("The product category this event's final price will be.")]
        [DefaultValue(EventTypes.Default)]
        public EventTypes EventType;

        [Description("The parameters this event can take.")]
        [DefaultValue(null)]
        public List<Parameter> Parameters;

        [Description("A class used to embed event settings in the Editor.")]
        [DefaultValue(null)]
        public Type SettingsEmbed;
    }
}
