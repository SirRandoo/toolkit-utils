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

using System;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.CommonLib.Entities;
using SirRandoo.CommonLib.Interfaces;
using TaskExtensions = ToolkitUtils.Wrappers.Async.TaskExtensions;

namespace ToolkitUtils.Api
{
    /// <summary>
    ///     A class for handling exceptions that were raised.
    /// </summary>
    public static class ExceptionHandler
    {
        private static readonly IRimLogger Logger = new RimThreadedLogger("ToolkitUtils.Exceptions");
        private static readonly MethodInfo HandleExceptionMethod = AccessTools.Method("VisualExceptions.ExceptionState:Handle", new[] { typeof(Exception) });

        public static async Task HandleExceptionAsync(Exception exception)
        {
            if (HandleExceptionMethod != null && await TaskExtensions.OnMainAsync(PassToVisualEx, exception))
            {
                return;
            }

            Logger.Error(exception.Message, exception);
        }

        public static void HandleException(Exception exception)
        {
            if (HandleExceptionMethod != null && PassToVisualEx(exception))
            {
                return;
            }

            Logger.Error(exception.Message, exception);
        }

        private static bool PassToVisualEx([CanBeNull] Exception exception)
        {
            return HandleExceptionMethod?.Invoke(null, new object[] { exception }) == exception;
        }
    }
}
