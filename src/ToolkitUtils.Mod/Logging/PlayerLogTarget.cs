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
using System.Threading;
using NLog;
using NLog.Targets;
using Verse;

namespace ToolkitUtils.Mod.Logging;

/// <summary>A custom NLog target for handling player log messages with specific synchronization context behavior.</summary>
/// <remarks>
///     This target directs log events to be processed in a synchronization context, allowing for thread-safe
///     operations. Depending on the log level, it posts the log messages with different color highlights: - Warning,
///     Fatal, and Error levels are highlighted differently for visibility. - Other levels are processed normally.
/// </remarks>
[Target("utils-player-log")]
internal sealed class PlayerLogTarget : TargetWithContext
{
    private static readonly SynchronizationContext Context = SynchronizationContext.Current;

    /// <inheritdoc />
    protected override void Write(LogEventInfo logEvent)
    {
        string logMessage = RenderLogEvent(Layout, logEvent);

        if (logEvent.Level == LogLevel.Warn || logEvent.Level == LogLevel.Fatal || logEvent.Level == LogLevel.Error)
            Context.Post(LogRedMessage, logMessage);
        else
            Context.Post(LogMessage, logMessage);
    }

    private static void LogMessage(object message)
    {
        Log.Message((string)message);
    }

    private static void LogRedMessage(object message)
    {
        Log.Warning((string)message);
    }
}
