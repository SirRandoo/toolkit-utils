// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using JetBrains.Annotations;

namespace ToolkitUtils.Mod.Data;

/// <summary>
///     Represents a financial transaction associated with a viewer, including the transaction amount and its
///     fulfillment status.
/// </summary>
/// <remarks>
///     When an instance of <see cref="Transaction" /> is disposed, it will either finalize or abort the transaction
///     based on the fulfillment status.
/// </remarks>
[PublicAPI]
public readonly record struct Transaction(Viewer Viewer, int Amount);

[PublicAPI]
public static class TransactionExtensions
{
    public static Result<Transaction> ChangeAmount(this Transaction transaction, int amount)
    {
        transaction.Viewer.AbortTransaction(transaction);

        return transaction.Viewer.ReserveCoins(amount);
    }

    public static Result PostTransaction(this Transaction transaction) => transaction.Viewer.FinalizeTransaction(transaction);
}
