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
using System.Threading.Tasks;

namespace ToolkitUtils.Mod.Services;

/// <summary>
///     Provides utility methods to route function or action executions to the main thread. This service handles
///     routing synchronously or asynchronously with support for multiple parameters.
/// </summary>
public sealed class RouterService
{
    private static readonly TaskScheduler MainThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();

    private static readonly TaskFactory MainThreadTaskFactory = new(
        CancellationToken.None,
        TaskCreationOptions.DenyChildAttach,
        TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.RunContinuationsAsynchronously,
        MainThreadScheduler
    );

    private static readonly Lazy<RouterService> _instance = new(() => new RouterService());

    public static RouterService Instance => _instance.Value;

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TResult">The return type of the function to route.</typeparam>
    /// <param name="func">The function to be executed on the main thread.</param>
    /// <returns>A task representing the asynchronous operation with the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult>(Func<TResult> func) => MainThreadTaskFactory.StartNew(func, TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Routes the execution of a function to the main thread and returns the result.</summary>
    /// <typeparam name="TResult">The type of the result returned by the function.</typeparam>
    /// <typeparam name="TArg1">The type of the argument passed to the function.</typeparam>
    /// <param name="func">The function to be executed on the main thread.</param>
    /// <param name="arg1">The argument to be passed to the function.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1>(Func<TArg1, TResult> func, TArg1 arg1)
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes the provided function on the main thread and returns the result.</summary>
    /// <typeparam name="TResult">The type of the result produced by the function.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument passed to the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument passed to the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <param name="arg1">The first argument to pass to the function.</param>
    /// <param name="arg2">The second argument to pass to the function.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2>(Func<TArg1, TArg2, TResult> func, TArg1 arg1, TArg2 arg2)
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1, arg2), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function on the main thread and returns the result of the function's execution.</summary>
    /// <param name="func">The function to execute.</param>
    /// <param name="arg1">The first argument to pass to the function.</param>
    /// <param name="arg2">The second argument to pass to the function.</param>
    /// <param name="arg3">The third argument to pass to the function.</param>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument.</typeparam>
    /// <returns>A task that represents the asynchronous operation and contains the result of the function's execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1, arg2, arg3), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the execution of the specified function with four arguments to the main thread.</summary>
    /// <typeparam name="TResult">The type of the result produced by the function.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument of the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument of the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument of the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument of the function.</typeparam>
    /// <param name="func">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument of the function.</param>
    /// <param name="arg2">The second argument of the function.</param>
    /// <param name="arg3">The third argument of the function.</param>
    /// <param name="arg4">The fourth argument of the function.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1, arg2, arg3, arg4), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function with five arguments to execute on the main thread.</summary>
    /// <typeparam name="TResult">The return type of the function to execute.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument.</typeparam>
    /// <param name="func">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument for the function.</param>
    /// <param name="arg2">The second argument for the function.</param>
    /// <param name="arg3">The third argument for the function.</param>
    /// <param name="arg4">The fourth argument for the function.</param>
    /// <param name="arg5">The fifth argument for the function.</param>
    /// <returns>A task representing the asynchronous operation with the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2, TArg3, TArg4, TArg5>(
        Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> func,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5
    )
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1, arg2, arg3, arg4, arg5), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TResult">The return type of the function to route.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument of the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument of the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument of the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument of the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument of the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument of the function.</typeparam>
    /// <param name="func">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to supply to the function.</param>
    /// <param name="arg2">The second argument to supply to the function.</param>
    /// <param name="arg3">The third argument to supply to the function.</param>
    /// <param name="arg4">The fourth argument to supply to the function.</param>
    /// <param name="arg5">The fifth argument to supply to the function.</param>
    /// <param name="arg6">The sixth argument to supply to the function.</param>
    /// <returns>A task representing the asynchronous operation with the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
        Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> func,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6
    )
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1, arg2, arg3, arg4, arg5, arg6), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TResult">The return type of the function to route.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument of the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument of the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument of the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument of the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument of the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument of the function.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument of the function.</typeparam>
    /// <param name="func">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to pass to the function.</param>
    /// <param name="arg2">The second argument to pass to the function.</param>
    /// <param name="arg3">The third argument to pass to the function.</param>
    /// <param name="arg4">The fourth argument to pass to the function.</param>
    /// <param name="arg5">The fifth argument to pass to the function.</param>
    /// <param name="arg6">The sixth argument to pass to the function.</param>
    /// <param name="arg7">The seventh argument to pass to the function.</param>
    /// <returns>A task representing the asynchronous operation with the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(
        Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> func,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6,
        TArg7 arg7
    )
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TResult">The return type of the function to route.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument of the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument of the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument of the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument of the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument of the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument of the function.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument of the function.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument of the function.</typeparam>
    /// <param name="func">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to pass to the function.</param>
    /// <param name="arg2">The second argument to pass to the function.</param>
    /// <param name="arg3">The third argument to pass to the function.</param>
    /// <param name="arg4">The fourth argument to pass to the function.</param>
    /// <param name="arg5">The fifth argument to pass to the function.</param>
    /// <param name="arg6">The sixth argument to pass to the function.</param>
    /// <param name="arg7">The seventh argument to pass to the function.</param>
    /// <param name="arg8">The eighth argument to pass to the function.</param>
    /// <returns>A task representing the asynchronous operation with the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(
        Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> func,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6,
        TArg7 arg7,
        TArg8 arg8
    )
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TResult">The return type of the function to route.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument of the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument of the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument of the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument of the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument of the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument of the function.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument of the function.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument of the function.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument of the function.</typeparam>
    /// <param name="func">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to pass to the function.</param>
    /// <param name="arg2">The second argument to pass to the function.</param>
    /// <param name="arg3">The third argument to pass to the function.</param>
    /// <param name="arg4">The fourth argument to pass to the function.</param>
    /// <param name="arg5">The fifth argument to pass to the function.</param>
    /// <param name="arg6">The sixth argument to pass to the function.</param>
    /// <param name="arg7">The seventh argument to pass to the function.</param>
    /// <param name="arg8">The eighth argument to pass to the function.</param>
    /// <param name="arg9">The ninth argument to pass to the function.</param>
    /// <returns>A task representing the asynchronous operation with the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(
        Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TResult> func,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6,
        TArg7 arg7,
        TArg8 arg8,
        TArg9 arg9
    )
    {
        return MainThreadTaskFactory.StartNew(function: () => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function with ten arguments to run on the main thread.</summary>
    /// <typeparam name="TResult">The return type of the function to route.</typeparam>
    /// <typeparam name="TArg1">The type of the first argument.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <param name="arg1">The first argument to pass to the function.</param>
    /// <param name="arg2">The second argument to pass to the function.</param>
    /// <param name="arg3">The third argument to pass to the function.</param>
    /// <param name="arg4">The fourth argument to pass to the function.</param>
    /// <param name="arg5">The fifth argument to pass to the function.</param>
    /// <param name="arg6">The sixth argument to pass to the function.</param>
    /// <param name="arg7">The seventh argument to pass to the function.</param>
    /// <param name="arg8">The eighth argument to pass to the function.</param>
    /// <param name="arg9">The ninth argument to pass to the function.</param>
    /// <param name="arg10">The tenth argument to pass to the function.</param>
    /// <returns>A task representing the asynchronous operation with the result of the function execution.</returns>
    public Task<TResult> RouteToMainAsync<TResult, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(
        Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10, TResult> func,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6,
        TArg7 arg7,
        TArg8 arg8,
        TArg9 arg9,
        TArg10 arg10
    )
    {
        return MainThreadTaskFactory.StartNew(
            function: () => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10),
            TaskCreationOptions.RunContinuationsAsynchronously
        );
    }

    /// <summary>Routes the specified action to run on the main thread.</summary>
    /// <param name="action">The action to be executed on the main thread.</param>
    /// <returns>A task representing the asynchronous operation of executing the action.</returns>
    public Task RouteToMainAsync(Action action) => MainThreadTaskFactory.StartNew(action, TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Routes the specified function to run on the main thread with the provided argument.</summary>
    /// <typeparam name="TArg1">The type of the argument to pass to the function.</typeparam>
    /// <param name="action">The action to execute on the main thread.</param>
    /// <param name="arg1">The argument to pass to the action.</param>
    /// <returns>A task representing the asynchronous execution of the action on the main thread.</returns>
    public Task RouteToMainAsync<TArg1>(Action<TArg1> action, TArg1 arg1)
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2>(Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2)
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1, arg2), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <param name="arg3">The third argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3)
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1, arg2, arg3), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument for the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <param name="arg3">The third argument to be passed to the function.</param>
    /// <param name="arg4">The fourth argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2, TArg3, TArg4>(Action<TArg1, TArg2, TArg3, TArg4> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1, arg2, arg3, arg4), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument for the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument for the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <param name="arg3">The third argument to be passed to the function.</param>
    /// <param name="arg4">The fourth argument to be passed to the function.</param>
    /// <param name="arg5">The fifth argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2, TArg3, TArg4, TArg5>(Action<TArg1, TArg2, TArg3, TArg4, TArg5> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1, arg2, arg3, arg4, arg5), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument for the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument for the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument for the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <param name="arg3">The third argument to be passed to the function.</param>
    /// <param name="arg4">The fourth argument to be passed to the function.</param>
    /// <param name="arg5">The fifth argument to be passed to the function.</param>
    /// <param name="arg6">The sixth argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> action,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6
    )
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1, arg2, arg3, arg4, arg5, arg6), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument for the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument for the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument for the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument for the function.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <param name="arg3">The third argument to be passed to the function.</param>
    /// <param name="arg4">The fourth argument to be passed to the function.</param>
    /// <param name="arg5">The fifth argument to be passed to the function.</param>
    /// <param name="arg6">The sixth argument to be passed to the function.</param>
    /// <param name="arg7">The seventh argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> action,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6,
        TArg7 arg7
    )
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1, arg2, arg3, arg4, arg5, arg6, arg7), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument for the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument for the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument for the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument for the function.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument for the function.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <param name="arg3">The third argument to be passed to the function.</param>
    /// <param name="arg4">The fourth argument to be passed to the function.</param>
    /// <param name="arg5">The fifth argument to be passed to the function.</param>
    /// <param name="arg6">The sixth argument to be passed to the function.</param>
    /// <param name="arg7">The seventh argument to be passed to the function.</param>
    /// <param name="arg8">The eighth argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> action,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6,
        TArg7 arg7,
        TArg8 arg8
    )
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument for the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument for the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument for the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument for the function.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument for the function.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument for the function.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <param name="arg3">The third argument to be passed to the function.</param>
    /// <param name="arg4">The fourth argument to be passed to the function.</param>
    /// <param name="arg5">The fifth argument to be passed to the function.</param>
    /// <param name="arg6">The sixth argument to be passed to the function.</param>
    /// <param name="arg7">The seventh argument to be passed to the function.</param>
    /// <param name="arg8">The eighth argument to be passed to the function.</param>
    /// <param name="arg9">The ninth argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9>(
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9> action,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6,
        TArg7 arg7,
        TArg8 arg8,
        TArg9 arg9
    )
    {
        return MainThreadTaskFactory.StartNew(action: () => action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Routes the specified function to run on the main thread.</summary>
    /// <typeparam name="TArg1">The type of the first argument for the function.</typeparam>
    /// <typeparam name="TArg2">The type of the second argument for the function.</typeparam>
    /// <typeparam name="TArg3">The type of the third argument for the function.</typeparam>
    /// <typeparam name="TArg4">The type of the fourth argument for the function.</typeparam>
    /// <typeparam name="TArg5">The type of the fifth argument for the function.</typeparam>
    /// <typeparam name="TArg6">The type of the sixth argument for the function.</typeparam>
    /// <typeparam name="TArg7">The type of the seventh argument for the function.</typeparam>
    /// <typeparam name="TArg8">The type of the eighth argument for the function.</typeparam>
    /// <typeparam name="TArg9">The type of the ninth argument for the function.</typeparam>
    /// <typeparam name="TArg10">The type of the tenth argument for the function.</typeparam>
    /// <param name="action">The function to be executed on the main thread.</param>
    /// <param name="arg1">The first argument to be passed to the function.</param>
    /// <param name="arg2">The second argument to be passed to the function.</param>
    /// <param name="arg3">The third argument to be passed to the function.</param>
    /// <param name="arg4">The fourth argument to be passed to the function.</param>
    /// <param name="arg5">The fifth argument to be passed to the function.</param>
    /// <param name="arg6">The sixth argument to be passed to the function.</param>
    /// <param name="arg7">The seventh argument to be passed to the function.</param>
    /// <param name="arg8">The eighth argument to be passed to the function.</param>
    /// <param name="arg9">The ninth argument to be passed to the function.</param>
    /// <param name="arg10">The tenth argument to be passed to the function.</param>
    /// <returns>A task that represents the asynchronous execution of the function.</returns>
    public Task RouteToMainAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10>(
        Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TArg9, TArg10> action,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        TArg4 arg4,
        TArg5 arg5,
        TArg6 arg6,
        TArg7 arg7,
        TArg8 arg8,
        TArg9 arg9,
        TArg10 arg10
    )
    {
        return MainThreadTaskFactory.StartNew(
            action: () => action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10),
            TaskCreationOptions.RunContinuationsAsynchronously
        );
    }
}
