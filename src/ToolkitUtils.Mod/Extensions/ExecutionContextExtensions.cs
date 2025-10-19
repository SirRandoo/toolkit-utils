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
using System.Threading.Tasks;

namespace ToolkitUtils.Mod.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="ExecutionContext" /> interface, enabling additional
///     functionality such as sending a reply to a message within the current execution context.
/// </summary>
public static class ExecutionContextExtensions
{
    /// <summary>Sends a reply to the message within the specified execution context with the provided content.</summary>
    /// <param name="context">The execution context containing information about the current operation and message.</param>
    /// <param name="message">The content of the reply to be sent.</param>
    public static ValueTask<Result> SendReplyAsync(this ExecutionContext context, string message) => context.Message.SendReply(message);

    /// <summary>
    ///     Retrieves a service of type <typeparamref name="TService" /> based on the execution variant provided in the
    ///     current execution context.
    /// </summary>
    /// <typeparam name="TService">The type of the service to retrieve.</typeparam>
    /// <param name="context">The current execution context containing the execution variant to identify the service.</param>
    /// <returns>The service instance of type <typeparamref name="TService" /> associated with the execution variant.</returns>
    public static TService GetService<TService>(this ExecutionContext context) => ServiceFactory<TService>.GetService(context.Variant);
}
