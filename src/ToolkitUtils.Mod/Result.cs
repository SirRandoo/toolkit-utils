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
using System.Threading.Tasks;
using JetBrains.Annotations;
using ToolkitUtils.Mod.Localization;

namespace ToolkitUtils.Mod;

/// <summary>Defines a contract that represents the result of an operation, including its success state.</summary>
public interface IResult
{
    /// <summary>Represents whether the operation yielded a successful outcome.</summary>
    /// <remarks>
    ///     A value of <langword cref="true" /> denotes a successful execution of the operation. Conversely, a value of
    ///     <langword cref="false" /> suggests the operation did not succeed. Additional context about the outcome could be
    ///     accessed via related members, where available.
    /// </remarks>
    bool IsSuccess { get; }

    /// <summary>Represents the localized translation key associated with the operation's result status or message.</summary>
    /// <remarks>
    ///     This property provides a <see cref="Translation" /> key that can be used to retrieve a localized,
    ///     user-friendly message related to the result of the operation. The translation key encapsulated in this property can
    ///     be resolved into human-readable text via a registered translation service, offering context to the success or
    ///     failure of the operation.
    /// </remarks>
    Translation Message { get; }
}

/// <summary>Represents a result from an operation, including success status and error information.</summary>
[PublicAPI]
public readonly record struct Result : IResult
{
    private static readonly Result OkConstant = new(success: true, Translation.None);
    private static readonly Result FailConstant = new(success: false, Translation.None);

    private Result(bool success, Translation message)
    {
        IsSuccess = success;
        Message = message;
    }

    /// <inheritdoc />
    public bool IsSuccess { get; }

    public Translation Message { get; }

    /// <summary>Creates a successful result.</summary>
    /// <returns>A successful result of type <see cref="Result" />.</returns>
    public static Result Ok() => OkConstant;

    /// <summary>Creates a successful result with a given message.</summary>
    /// <param name="message">The additional message describing the result.</param>
    /// <returns>A successful result of type <see cref="Result" /> with the specified message.</returns>
    public static Result Ok(Translation message) => new(success: true, message);

    /// <summary>Creates a successful result with a value of the specified type.</summary>
    /// <typeparam name="T">The type of the value included in the result.</typeparam>
    /// <param name="value">The value associated with the successful result.</param>
    /// <returns>A <see cref="Result{T}" /> indicating a successful operation with the provided value.</returns>
    public static Result<T> Ok<T>(T? value) => new(value, success: true, Translation.None);

    /// <summary>Creates a successful result.</summary>
    /// <returns>A successful result of type <see cref="Result" />.</returns>
    public static Result<T> Ok<T>(T value, Translation message) => new(value, success: true, message);

    /// <summary>Creates a failure result.</summary>
    /// <returns>A <see cref="Result" /> instance indicating failure.</returns>
    public static Result Fail() => FailConstant;

    /// <summary>Creates a failure result with the specified error message.</summary>
    /// <param name="error">The error message describing the failure.</param>
    /// <returns>A <see cref="Result" /> instance indicating failure and containing the provided error message.</returns>
    public static Result Fail(Translation error) => new(success: false, error);

    /// <summary>Creates a failed result.</summary>
    /// <returns>A failed result of type <see cref="Result" />.</returns>
    public static Result<T> Fail<T>(T value, Translation error) => new(value, success: false, error);

    /// <summary>Creates a failed result.</summary>
    /// <returns>A failed result of type <see cref="Result" />.</returns>
    public static Result<T> Fail<T>(Translation error) => new(default(T?), success: false, error);
}

/// <summary>Represents a result that includes a value of type T along with operation status information.</summary>
[PublicAPI]
public readonly record struct Result<T> : IResult
{
    internal Result(T? value, bool success, Translation message)
    {
        Value = value;
        IsSuccess = success;
        Message = message;
    }

    /// <summary>Provides access to the underlying value associated with the result.</summary>
    /// <remarks>
    ///     When the result indicates success, this property contains the value produced by the operation. If the result
    ///     denotes a failure, this property may be null or unset depending on the implementation.
    /// </remarks>
    public T? Value { get; }

    /// <inheritdoc />
    public Translation Message { get; }

    /// <inheritdoc />
    public bool IsSuccess { get; }

    /// <summary>
    ///     Maps the current result to a new result of type <typeparamref name="TResult" /> using the provided mapping
    ///     function.
    /// </summary>
    /// <param name="mapper">
    ///     A function that maps the value of the current result to a new result of type
    ///     <typeparamref name="TResult" />.
    /// </param>
    /// <returns>
    ///     A <see cref="Result{TResult}" /> containing the mapped value if the operation was successful; otherwise, a
    ///     failure result with the original error and exception.
    /// </returns>
    public Result<TResult> Map<TResult>(Func<T, TResult> mapper) => IsSuccess ? Result.Ok(mapper(Value!)) : Result.Fail<TResult>(Message);

    /// <summary>
    ///     Asynchronously maps the current result's value to a new result of type <typeparamref name="TResult" /> using
    ///     the specified asynchronous mapper function.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value after mapping.</typeparam>
    /// <param name="mapper">The asynchronous function to transform the current result value.</param>
    /// <returns>
    ///     A new <see cref="Result{TResult}" /> representing the mapped value if the operation is successful, or a
    ///     failure result if the current result is not successful.
    /// </returns>
    public async Task<Result<TResult>> MapAsync<TResult>(Func<T, Task<TResult>> mapper) => IsSuccess ? Result.Ok(await mapper(Value!)) : Result.Fail<TResult>(Message);

    public static implicit operator bool(Result<T> result) => result.IsSuccess;

    public static implicit operator Result(Result<T> result) => result.IsSuccess ? Result.Ok() : Result.Fail(result.Message);
}
