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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils;

public class RimLogger(string name)
{
    private bool _debugChecked;
    private bool _debugEnabled;

    public string FormatMessage(string message) => $"{name} :: {message}";

    public string FormatMessage(string level, string message) => $"{level.ToUpperInvariant()} {name} :: {message}";

    public string FormatMessage(string level, string message, string color) => $@"<color=""#{color.TrimStart('#')}"">{FormatMessage(level, message)}</color>";

    public virtual void Log(string message)
    {
        LogInternal(FormatMessage(message));
    }

    public virtual void Info(string message)
    {
        LogInternal(FormatMessage("INFO", message));
    }

    public virtual void Warn(string message)
    {
        LogInternal(FormatMessage("WARN", message, "#FF6B00"));
    }

    public virtual void Error(string message)
    {
        LogInternal(FormatMessage("ERR", message, "#FF768CE"));
        Verse.Log.TryOpenLogWindow();
    }

    public virtual void Error(string message, Exception exception)
    {
        Error($"{message} :: {exception.GetType().Name}({exception.Message})\n\n{exception.ToStringSafe()}");
    }

    public virtual void Debug(string message)
    {
        if (!_debugChecked)
        {
            _debugEnabled = Assembly.GetCallingAssembly().GetCustomAttribute<DebuggableAttribute>()?.DebuggingFlags
                == DebuggableAttribute.DebuggingModes.DisableOptimizations;

            _debugChecked = true;
        }

        if (_debugEnabled)
        {
            LogInternal(FormatMessage("DEBUG", message, ColorUtility.ToHtmlStringRGB(ColorLibrary.LightPink)));
        }
    }

    protected virtual void LogInternal(string message)
    {
        Verse.Log.Message(message);
    }
}

[StaticConstructorOnStartup]
internal sealed class RimThreadedLogger(string name) : RimLogger(name)
{
    /// <inheritdoc />
    public override void Log(string message)
    {
        if (UnityData.IsInMainThread)
        {
            base.Log(message);
        }
        else
        {
            TkUtils.Context.Post(o => base.Log(message), null);
        }
    }

    /// <inheritdoc />
    public override void Error(string message)
    {
        if (UnityData.IsInMainThread)
        {
            base.Error(message);
        }
        else
        {
            TkUtils.Context.Post(o => base.Error(message), null);
        }
    }
}
