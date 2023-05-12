// MIT License
// 
// Copyright (c) 2023 SirRandoo
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

using System.Threading.Tasks;

namespace ToolkitUtils.Data.Models
{
    /// <summary>
    ///     Represents a handler for the action system within ToolkitUtils.
    /// </summary>
    public interface IActionHandler
    {
        /// <summary>
        ///     The unique id of the handler.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     The human-readable name of the handler.
        /// </summary>
        /// <remarks>
        ///     The value of this text is always displayed to the user through
        ///     debug logs issued by ToolkitUtils.
        /// </remarks>
        string Name { get; }

        /// <summary>
        ///     Returns an <see cref="IExecutionContext"/> instance suitable for
        ///     its action implementation.
        /// </summary>
        /// <param name="arguments">
        ///     The raw text from the viewer deconstructed
        ///     into individual segments via a UNIX-like command parser.
        /// </param>
        Task<IExecutionContext> CreateContextAsync(params string[] arguments);

        /// <summary>
        ///     Returns whether the action can run at the given moment in time.
        /// </summary>
        /// <param name="context">
        ///     An <see cref="IExecutionContext"/> instance
        ///     returned by <see cref="CreateContextAsync"/>.
        /// </param>
        Task<bool> CanExecuteAsync(IExecutionContext context);

        /// <summary>
        ///     Executes the action.
        /// </summary>
        /// <param name="context">
        ///     An <see cref="IExecutionContext"/> instance
        ///     returned by <see cref="CreateContextAsync"/>.
        /// </param>
        /// <returns>Whether the action successfully executed.</returns>
        Task<bool> ExecuteAsync(IExecutionContext context);
    }
}
