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
using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Verse;
using LoggingConfiguration = NLog.Config.LoggingConfiguration;
using LogLevel = NLog.LogLevel;

namespace ToolkitUtils.Mod.Logging;

/// <summary>Provides extension methods for configuring logging services.</summary>
public sealed class UtilsLogFactory : LogFactory
{
    /// <summary>Defines the layout format for log messages to be used in NLog targets.</summary>
    /// <remarks>
    ///     This layout string specifies the structure of log messages, including the timestamp, log level, thread ID,
    ///     logger name, and the actual log message. The exception details are also included if present. Format: ${time}
    ///     [${level:uppercase=true}][Thread ${threadid}] (${logger}) :: ${message:withexception=true}
    /// </remarks>
    private const string LogMessageLayout = "${time} [${level:uppercase=true}][Thread ${threadid}] (${logger}) :: ${message:withexception=true}";

    private static readonly Lazy<LogFactory> InstanceField = new(BuildLogFactory);

    /// <summary>Provides a singleton instance of the logging factory configured for use with NLog.</summary>
    /// <remarks>
    ///     This property initializes the logging factory using a lazy-loaded approach to ensure that the logging
    ///     configuration is built only when accessed, optimizing resource utilization. The instance delivers a pre-configured
    ///     <see cref="NLog.LogFactory" /> with defined logging rules and targets for various logging outputs.
    /// </remarks>
    public static LogFactory Instance => InstanceField.Value;

    /// <summary>Creates and configures an NLog factory instance for logging.</summary>
    /// <returns>A <see cref="NLog.LogFactory" /> instance configured with the required logging settings.</returns>
    private static LogFactory BuildLogFactory()
    {
        LoggingConfiguration configuration = BuildNLogConfiguration();
        InjectConsoleTarget(configuration);

        return configuration.LogFactory;
    }

    /// <summary>Builds the NLog configuration for the logging system.</summary>
    /// <returns>A configured <c>NLog.Config.LoggingConfiguration</c> object.</returns>
    private static LoggingConfiguration BuildNLogConfiguration()
    {
        var factory = new UtilsLogFactory();
        var config = new LoggingConfiguration(factory);

        var playerLogTarget = new PlayerLogTarget
        {
            Layout = LogMessageLayout,
        };
        var asyncTargetWrapper = new AsyncTargetWrapper(playerLogTarget);

        config.AddTarget(asyncTargetWrapper);
        config.AddRule(LogLevel.Warn, LogLevel.Fatal, playerLogTarget);


        var fileTarget = new FileTarget
        {
            Layout = LogMessageLayout, DeleteOldFileOnStartup = true,
        };
        var asyncFileWrapper = new AsyncTargetWrapper(fileTarget);
        fileTarget.FileName = Path.Combine(GenFilePaths.SaveDataFolderPath, path2: "TwitchToolkit", path3: "ToolkitUtils.log");

        config.AddTarget(asyncFileWrapper);
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, asyncFileWrapper);

        return config;
    }

    /// <summary>
    ///     Injects a console target into the provided logging configuration, but only if running in debug mode and
    ///     specific conditions are met.
    /// </summary>
    /// <param name="config">The logging configuration to inject the console target into.</param>
    [Conditional("DEBUG")]
    private static void InjectConsoleTarget(LoggingConfiguration config)
    {
        if (!GenCommandLine.TryGetCommandLineArg(key: "-logfile", out string? value) && !string.Equals(value, b: "-", StringComparison.Ordinal)) return;

        var consoleTarget = new ColoredConsoleTarget
        {
            Layout = LogMessageLayout,
        };
        var consoleTargetWrapper = new BufferingTargetWrapper(consoleTarget);

        config.AddTarget(consoleTargetWrapper);
        config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTargetWrapper);
    }
}
