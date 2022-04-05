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
    public class ConsentWorker
    {
        private readonly Dictionary<string, ConsentContext> _consentData = new Dictionary<string, ConsentContext>();

        public void Create([NotNull] string asker, [NotNull] string askee)
        {
            var askerContext = new ConsentContext { User = askee };
            var askeeContext = new ConsentContext { User = asker, Agreed = true };

            _consentData[asker.ToLowerInvariant()] = askerContext;
            _consentData[askee.ToLowerInvariant()] = askeeContext;
        }

        [CanBeNull]
        public string GetAskee([NotNull] string username) => !_consentData.TryGetValue(username.ToLowerInvariant(), out ConsentContext context) ? null : context.User;

        public bool GetAskeeStatus([NotNull] string username) => _consentData.TryGetValue(username.ToLowerInvariant(), out ConsentContext context) && context.Agreed;

        [CanBeNull] public string GetAsker([NotNull] string username) => GetAskee(username);

        public bool GetAskerStatus([NotNull] string username) => GetAskeeStatus(username);

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

        public bool ClearContext([NotNull] string username) => _consentData.Remove(username.ToLowerInvariant());

        private struct ConsentContext
        {
            public string User { get; set; }
            public bool Agreed { get; set; }
        }
    }
}
