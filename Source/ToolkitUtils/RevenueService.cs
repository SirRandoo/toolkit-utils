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

using System.Collections.Generic;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
#if DEBUG
using System.Threading.Tasks;
#endif

namespace SirRandoo.ToolkitUtils
{
    public static class RevenueService
    {
        private static Dictionary<string, List<Transaction>> _transactions;

        static RevenueService()
        {
        #if DEBUG
            Task.Run(
                    async () =>
                    {
                        _transactions =
                            await Data.LoadCompressedJsonAsync<Dictionary<string, List<Transaction>>>(
                                Paths.RevenueService
                            );
                        _transactions ??= new Dictionary<string, List<Transaction>>();
                    }
                )
               .ConfigureAwait(false);
        #endif
        }

        [CanBeNull]
        public static List<Transaction> GetTransactionsFor([NotNull] string username) =>
            _transactions.TryGetValue(username.ToLowerInvariant(), out List<Transaction> transactions) ? transactions : null;

        public static void RegisterTransactionFor([NotNull] string username, Transaction transaction)
        {
            if (!_transactions.TryGetValue(username.ToLowerInvariant(), out List<Transaction> transactions))
            {
                _transactions.Add(username.ToLowerInvariant(), new List<Transaction> { transaction });

                return;
            }

            transactions.Add(transaction);
        }
    }
}
