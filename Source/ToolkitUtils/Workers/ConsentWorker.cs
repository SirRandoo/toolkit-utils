// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Workers
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
