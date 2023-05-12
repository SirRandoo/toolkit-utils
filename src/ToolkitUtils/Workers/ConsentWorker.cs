// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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

namespace ToolkitUtils.Workers
{
    /// <summary>
    ///     A class for containing consent data between two viewers. Events
    ///     and commands desiring a consent between two viewers should use
    ///     this worker as a means of performing this functionality easily.
    /// </summary>
    public class ConsentWorker
    {
        private readonly Dictionary<string, ConsentContext> _consentData = new Dictionary<string, ConsentContext>();

        /// <summary>
        ///     Creates a new context for the given user pair.
        /// </summary>
        /// <param name="asker">The person that initiated a consent gated action</param>
        /// <param name="askee">
        ///     The person <see cref="askee"/> is asking consent
        ///     from
        /// </param>
        public void Create([NotNull] string asker, [NotNull] string askee)
        {
            var askerContext = new ConsentContext { User = askee };
            var askeeContext = new ConsentContext { User = asker, Agreed = true };

            _consentData[asker.ToLowerInvariant()] = askerContext;
            _consentData[askee.ToLowerInvariant()] = askeeContext;
        }

        /// <summary>
        ///     Returns the person that was asked for consent about a given
        ///     consent action.
        /// </summary>
        /// <param name="username">
        ///     The username of the person that asked for
        ///     consent
        /// </param>
        [CanBeNull]
        public string GetAskee([NotNull] string username) => !_consentData.TryGetValue(username.ToLowerInvariant(), out ConsentContext context) ? null : context.User;

        /// <summary>
        ///     Returns whether the person asked for consent consented.
        /// </summary>
        /// <param name="username">
        ///     The person that asked the given person for
        ///     consent
        /// </param>
        public bool GetAskeeStatus([NotNull] string username) => _consentData.TryGetValue(username.ToLowerInvariant(), out ConsentContext context) && context.Agreed;

        /// <summary>
        ///     Returns the person that triggered a consent action.
        /// </summary>
        /// <param name="username">The person that was asked for consent</param>
        [CanBeNull]
        public string GetAsker([NotNull] string username) => GetAskee(username);

        /// <summary>
        ///     Returns whether the person that triggered a consent action
        ///     consented to the action.
        /// </summary>
        /// <param name="username">The person that was asked for consent</param>
        public bool GetAskerStatus([NotNull] string username) => GetAskeeStatus(username);

        /// <summary>
        ///     Returns all offers created by a given user.
        /// </summary>
        /// <param name="username">
        ///     The username of the person that triggered a
        ///     given consensual action
        /// </param>
        public IEnumerable<string> GetAllOffersFor(string username)
        {
            foreach (KeyValuePair<string, ConsentContext> pair in _consentData)
            {
                string asker = pair.Key;
                ConsentContext context = pair.Value;

                if (string.Equals(username, context.User, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return asker;
                }
            }
        }

        /// <summary>
        ///     Removes data associated with the given username.
        /// </summary>
        /// <param name="username">The username of the person to remove data for</param>
        public bool ClearContext([NotNull] string username) => _consentData.Remove(username.ToLowerInvariant());

        private struct ConsentContext
        {
            public string User { get; set; }
            public bool Agreed { get; set; }
        }
    }
}
