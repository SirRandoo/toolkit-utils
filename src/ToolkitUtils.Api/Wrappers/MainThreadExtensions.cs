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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Verse;

namespace ToolkitUtils.Api.Wrappers;

/// <summary>
///     The MainThreadExtensions class provides a set of extension methods for executing tasks and actions on the main
///     thread using a specific task scheduler.
/// </summary>
[PublicAPI]
[StaticConstructorOnStartup]
public static class MainThreadExtensions
{
    /// <summary>
    ///     Represents a task scheduler associated with the main thread's synchronization context, allowing scheduling of
    ///     tasks to be run on the main thread.
    /// </summary>
    public static readonly TaskScheduler MainThreadScheduler = TaskScheduler.FromCurrentSynchronizationContext();

    /// <summary>
    ///     Provides a task factory for creating tasks that are scheduled on the main thread's task scheduler, ensuring
    ///     that tasks are executed in synchronization with the main application thread.
    /// </summary>
    public static readonly TaskFactory MainThreadFactory = new(
        CancellationToken.None,
        TaskCreationOptions.DenyChildAttach,
        TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.RunContinuationsAsynchronously,
        MainThreadScheduler
    );

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task<T> OnMainAsync<T>(this Func<T> func) => await MainThreadFactory.StartNew(func, TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Executes a function with one argument asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <typeparam name="T1">The type of the argument passed to the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <param name="arg1">The argument to pass to the function.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task<T> OnMainAsync<T, T1>(this Func<T1, T> func, T1 arg1)
    {
        return await MainThreadFactory.StartNew(function: () => func(arg1), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function that takes two arguments asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <typeparam name="T1">The type of the first argument passed to the function.</typeparam>
    /// <typeparam name="T2">The type of the second argument passed to the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <param name="arg1">The first argument to pass to the function.</param>
    /// <param name="arg2">The second argument to pass to the function.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task<T> OnMainAsync<T, T1, T2>(this Func<T1, T2, T> func, T1 arg1, T2 arg2)
    {
        return await MainThreadFactory.StartNew(function: () => func(arg1, arg2), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread with specified arguments and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the function.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the function.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <param name="arg1">The first argument to pass to the function.</param>
    /// <param name="arg2">The second argument to pass to the function.</param>
    /// <param name="arg3">The third argument to pass to the function.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task<T> OnMainAsync<T, T1, T2, T3>(this Func<T1, T2, T3, T> func, T1 arg1, T2 arg2, T3 arg3)
    {
        return await MainThreadFactory.StartNew(function: () => func(arg1, arg2, arg3), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a parameterless function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function.</returns>
    public static async Task<T> OnMainAsync<T, T1, T2, T3, T4>(this Func<T1, T2, T3, T4, T> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        return await MainThreadFactory.StartNew(function: () => func(arg1, arg2, arg3, arg4), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync(this Action func)
    {
        await MainThreadFactory.StartNew(func, TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T>(this Action<T> func, T arg1)
    {
        await MainThreadFactory.StartNew(action: () => func(arg1), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2>(this Action<T1, T2> func, T1 arg1, T2 arg2)
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2, T3>(this Action<T1, T2, T3> func, T1 arg1, T2 arg2, T3 arg3)
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2, arg3), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2, arg3, arg4), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a specified function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2, arg3, arg4, arg5), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2, arg3, arg4, arg5, arg6), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
        this Action<T1, T2, T3, T4, T5, T6, T7, T8> func,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7,
        T8 arg8
    )
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> func,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7,
        T8 arg8,
        T9 arg9
    )
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> func,
        T1 arg1,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7,
        T8 arg8,
        T9 arg9,
        T10 arg10
    )
    {
        await MainThreadFactory.StartNew(action: () => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10), TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>Executes a function asynchronously on the main thread and returns the result.</summary>
    /// <typeparam name="T">The type of the result returned by the function.</typeparam>
    /// <param name="func">The function to execute on the main thread.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the function.</returns>
    public static async Task OnMainAsync(this Task task)
    {
        task.Start(MainThreadScheduler);

        await task;
    }

    /// <summary>Schedules and executes a given asynchronous task on the main thread, returning its result.</summary>
    /// <typeparam name="T">The type of the result produced by the task.</typeparam>
    /// <param name="task">The task to be executed on the main thread.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the executed task.</returns>
    public static async Task<T> OnMainAsync<T>(this Task<T> task)
    {
        task.Start(MainThreadScheduler);

        return await task;
    }
}
