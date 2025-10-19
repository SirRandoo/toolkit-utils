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
using System.Diagnostics;
using System.Threading;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>Provides a centralized logging facility for the TwitchToolkit. Manages the creation and retrieval of loggers.</summary>
[PublicAPI]
[StaticConstructorOnStartup]
public static class ToolkitLogManager
{
    private const string Layout = "${time} [${level:uppercase=true}][Thread ${threadid}] (${logger}) :: ${message:withexception=true}";

    private static readonly Lazy<LogFactory> Factory = new(CreateFactory);

    /// <summary>Retrieves a logger instance by name.</summary>
    /// <param name="name">The name of the logger to retrieve.</param>
    /// <returns>A logger instance associated with the specified name.</returns>
    public static Logger GetLogger(string name) => Factory.Value.GetLogger(name);

    /// <summary>Retrieves a logger instance for the specified <see cref="Type" />.</summary>
    /// <param name="type">The type for which the logger is being requested.</param>
    /// <returns>A logger instance associated with the specified type.</returns>
    public static Logger GetLogger(Type type) => Factory.Value.GetLogger(type.FullDescription());

    /// <summary>Retrieves a logger for the specified name.</summary>
    /// <typeparam name="T">The type for which the logger is being requested.</typeparam>
    /// <returns>A logger instance associated with the specified type.</returns>
    public static Logger GetLogger<T>() where T : class => Factory.Value.GetLogger(typeof(T).FullDescription());

    private static LogFactory CreateFactory()
    {
        var factory = new LogFactory();
        var config = new LoggingConfiguration();

        var playerLogTarget = new PlayerLogTarget();
        playerLogTarget.Name = "PlayerLog";
        playerLogTarget.Layout = Layout;

        config.AddTarget(playerLogTarget);
        config.AddRule(LogLevel.Warn, LogLevel.Fatal, playerLogTarget);

        RegisterConsoleTarget(config);

        factory.Configuration = config;

        return factory;
    }

    [Conditional("DEBUG")]
    private static void RegisterConsoleTarget(LoggingConfiguration config)
    {
        if (!GenCommandLine.TryGetCommandLineArg(key: "-logfile", out string? value) && string.Equals(value, b: "-", StringComparison.Ordinal)) return;

        var consoleTarget = new ColoredConsoleTarget("Console");
        consoleTarget.Layout = Layout;

        var wrapper = new AsyncTargetWrapper(consoleTarget);

        config.AddTarget(wrapper);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, wrapper);
    }

    /// <summary>
    ///     Custom NLog target designed to handle player-specific log events and messages. Facilitates logging within a
    ///     synchronization context to ensure thread safety.
    /// </summary>
    [Target(TargetName)]
    public sealed class PlayerLogTarget : TargetWithContext
    {
        /// <summary>Represents the name assigned to the logging target in NLog configuration.</summary>
        private const string TargetName = "PlayerLog";

        /// <summary>
        ///     Represents the current synchronization context for the PlayerLogTarget. This context is used to post logging
        ///     messages, ensuring that log events are handled on the appropriate thread, maintaining thread safety and consistency
        ///     in log message processing.
        /// </summary>
        private static readonly SynchronizationContext CurrentContext = SynchronizationContext.Current;

        /// <summary>Writes the specified log event to the target.</summary>
        /// <param name="logEvent">The log event information to write.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = RenderLogEvent(Layout, logEvent);

            if (logEvent.Level == LogLevel.Warn || logEvent.Level == LogLevel.Error || logEvent.Level == LogLevel.Fatal)
                CurrentContext.Post(LogRedMessage, logMessage);
            else
                CurrentContext.Post(LogMessage, logMessage);
        }

        /// <summary>Logs a standard message to the application.</summary>
        /// <param name="state">The message state object to be logged.</param>
        private static void LogMessage(object state)
        {
            Log.Message(state.ToString());
        }

        /// <summary>Logs the specified message as a warning.</summary>
        /// <param name="state">The message to log as a warning.</param>
        private static void LogRedMessage(object state)
        {
            Log.Warning(state.ToString());
        }
    }
}
