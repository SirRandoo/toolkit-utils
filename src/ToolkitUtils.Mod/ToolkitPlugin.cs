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
using JetBrains.Annotations;

namespace ToolkitUtils.Mod;

/// <summary>Defines a base abstraction for creating modular plugins with configurable behavior and lifecycle management.</summary>
/// <remarks>
///     ToolkitPlugin provides an extensible framework for developing plugins within an application. It standardizes
///     plugin initialization, shutdown, and configuration processes while allowing developers to implement custom
///     functionality as necessary. The associated metadata offers descriptive details enabling the handling of plugins in
///     a structured and manageable way.
/// </remarks>
[PublicAPI]
public abstract class ToolkitPlugin
{
    /// <summary>Gets the metadata describing the plugin, including its identifier, name, and version.</summary>
    /// <remarks>
    ///     The metadata contains essential descriptive information used for plugin identification, versioning, and
    ///     compatibility management within the application.
    /// </remarks>
    public PluginMetadata Metadata { get; init; } = null!;

    /// <summary>
    ///     Prepares the plugin for operation. By default, performs no actions. Designed to be overridden by derived
    ///     classes to implement initialization logic specific to the plugin.
    /// </summary>
    public virtual void Initialize()
    {
        // Do nothing by default.
    }

    /// <summary>Shuts down the plugin, performing any necessary cleanup or resource deallocation tasks.</summary>
    public virtual void Shutdown()
    {
        // Do nothing by default.
    }

    /// <summary>
    ///     Loads the configuration settings for the plugin. Override this method to define custom logic for reading and
    ///     applying configuration values.
    /// </summary>
    public virtual void LoadConfig()
    {
        // Do nothing by default.
    }

    /// <summary>Saves the current configuration of the plugin.</summary>
    public virtual void SaveConfig()
    {
        // Do nothing by default.   
    }

    /// <summary>Retrieves the settings page renderer associated with this plugin.</summary>
    /// <returns>A renderer for the plugin's settings page or null if no settings page is available.</returns>
    public virtual IPageRenderer? GetSettingsPage() =>
        // Do nothing by default.
        null;
}
