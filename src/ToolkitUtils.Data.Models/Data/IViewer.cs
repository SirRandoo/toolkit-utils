using System;

namespace ToolkitUtils.Data.Models
{
    /// <summary>
    ///     Represents a viewer within the mod.
    /// </summary>
    public interface IViewer : IIdentifiable
    {
        /// <summary>
        ///     The user's unlocalized name. This is the name viewers use to log
        ///     into their Twitch accounts.
        /// </summary>
        string Login { get; set; }

        /// <summary>
        ///     The date and time the viewer was last seen chatting in chat.
        /// </summary>
        DateTime LastSeen { get; set; }

        /// <summary>
        ///     The various types of "user levels" the viewer has.
        /// </summary>
        /// <remarks>
        ///     User levels are a system in which actions within the mod are
        ///     limited to certain viewers at, or above, a certain user level.
        ///     This means that if an action, like a command, can only be used by
        ///     subscribers, then only subscribers or higher (vips, moderators,
        ///     admins, and broadcasters) can use the action.
        /// </remarks>
        UserTypes UserTypes { get; set; }

        /// <summary>
        ///     The amount of "karma" the viewer has.
        /// </summary>
        /// <remarks>
        ///     Karma is a system within Twitch Toolkit that acts as a rate
        ///     limiter on how often viewers can purchase harmful products, like
        ///     tornadoes. Viewers that purchase a harmful product will have
        ///     their karma lowered, while viewers that purchase a helpful
        ///     product, like medicine, will receive a higher karma. The amount
        ///     of karma a viewer has has a direct effect on the amount of coins
        ///     a viewer receives every "reward tick", which is the rate at which
        ///     viewers receive coins from the mod.
        /// </remarks>
        int Karma { get; set; }

        /// <summary>
        ///     The amount of coins a viewer has.
        /// </summary>
        long Coins { get; set; }
    }
}
