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
using System;
using System.Threading;
using ConcurrentCollections;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ToolkitUtils.Mod.Data;

/// <summary>
///     Defines a contract for a viewer in the module, providing properties and methods to manage viewer attributes
///     such as login details, user type classifications, and resource management including karma and coins.
/// </summary>
[PublicAPI]
[JsonObject(MemberSerialization.OptIn)]
public sealed class Viewer : IIdentifiable
{
    private readonly ConcurrentHashSet<Transaction> _transactions = [];
    private long _coins;
    private int _karma;

    [JsonProperty("id")]
    public string Id { get; init; }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("login")] public string Login { get; init; }

    [JsonProperty("last_seen")] public DateTime LastSeen { get; init; }

    [JsonProperty("user_types")] public UserTypes UserTypes { get; init; }

    [JsonProperty("karma")] public int Karma
    {
        get => _karma;
        init => _karma = value;
    }

    [JsonProperty("coins")] public long Coins
    {
        get => _coins;
        init => _coins = value;
    }

    /// <summary>Adds a specified amount of coins to the viewer's current coin balance.</summary>
    /// <param name="amount">The number of coins to be added to the viewer's balance. This value must be non-negative.</param>
    public void AddCoins(int amount) => Interlocked.Add(ref _coins, amount);

    /// <summary>Sets the viewer's coin balance to a specified amount.</summary>
    /// <param name="amount">The exact number of coins to set the viewer's balance to. This value must be non-negative.</param>
    public void SetCoins(int amount) => Interlocked.Exchange(ref _coins, amount);

    /// <summary>Removes a specified amount of coins from the viewer's current coin balance.</summary>
    /// <param name="amount">The number of coins to be deducted from the viewer's balance. This value must be non-negative.</param>
    public void RemoveCoins(int amount) => Interlocked.Add(ref _coins, -amount);

    /// <summary>
    ///     Reserves a specified amount of coins from the viewer's balance for a transaction, without deducting them
    ///     immediately.
    /// </summary>
    /// <param name="amount">
    ///     The number of coins to reserve from the viewer's balance. This value must be non-negative and
    ///     should not exceed the current balance.
    /// </param>
    /// <returns>A result wrapping a transaction that represents the reserved coins, which must be either finalized or aborted.</returns>
    public Result<Transaction> ReserveCoins(int amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount), message: "Amount must be non-negative.");
        if (Coins < amount) throw new ArgumentOutOfRangeException(nameof(amount), message: "Amount exceeds current balance.");

        var transaction = new Transaction
        {
            Amount = amount, Viewer = this,
        };

        _transactions.Add(transaction);
        RemoveCoins(amount);

        return Result.Ok(transaction);
    }

    /// <summary>Aborts the specified transaction, reverting any tentative resource allocation associated with it.</summary>
    /// <param name="transaction">
    ///     The transaction to be aborted. This transaction must have been initiated and not yet
    ///     finalized.
    /// </param>
    public void AbortTransaction(Transaction transaction)
    {
        if (!_transactions.Contains(transaction)) throw new InvalidOperationException("Pending transaction not found.");

        _transactions.TryRemove(transaction);

        AddCoins(transaction.Amount);
    }

    /// <summary>Finalizes a coin transaction for the viewer.</summary>
    /// <param name="transaction">The transaction object containing the details of the coin transaction to finalize.</param>
    /// <returns>A <see cref="Result" /> indicating the success or failure of the transaction finalization process.</returns>
    public Result FinalizeTransaction(Transaction transaction)
    {
        if (!_transactions.Contains(transaction)) throw new InvalidOperationException("Pending transaction not found.");

        bool removed = _transactions.TryRemove(transaction);

        return !removed ? throw new InvalidOperationException("Failed to finalize transaction.") : Result.Ok();
    }

    /// <summary>Adds a specified amount of karma to the viewer's current karma balance.</summary>
    /// <param name="amount">The number of karma points to be added to the viewer's balance. This value must be non-negative.</param>
    public void AddKarma(int amount) => Interlocked.Add(ref _karma, amount);

    /// <summary>Sets the viewer's karma to a specified amount.</summary>
    /// <param name="amount">The exact amount to set the viewer's karma to. This value must be non-negative.</param>
    public void SetKarma(int amount) => Interlocked.Exchange(ref _karma, amount);

    /// <summary>Removes a specified amount of karma from the viewer's current karma balance.</summary>
    /// <param name="amount">
    ///     The number of karma points to be deducted from the viewer's balance. This value must be
    ///     non-negative.
    /// </param>
    public void RemoveKarma(int amount) => Interlocked.Add(ref _karma, -amount);
}
