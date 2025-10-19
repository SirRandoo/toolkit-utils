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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using ToolkitUtils.Mod.Logging;

namespace ToolkitUtils.Mod.Serialization;

/// <summary>
///     Represents a specialized class that handles the reading, writing, and management of application or module
///     settings. Provides functionality for serialization and deserialization of settings data, along with the ability to
///     generate default settings if required or save updated settings persistently.
/// </summary>
[PublicAPI]
public sealed class SettingsSerializer
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();

    /// <summary>
    ///     Attempts to load the settings from the specified file path and deserializes them into an instance of the
    ///     specified type.
    /// </summary>
    /// <typeparam name="T">The type of the settings object to deserialize.</typeparam>
    /// <param name="path">The path of the file to load the settings from.</param>
    /// <param name="settings">
    ///     When this method returns, contains the deserialized settings object if successful; otherwise,
    ///     <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the settings were successfully loaded and deserialized; otherwise, <c>false</c>.</returns>
    public bool TryLoadSettings<T>(string path, [NotNullWhen(true)] out T? settings) where T : class
    {
        if (!File.Exists(path))
        {
            Logger.Trace(message: "File {Path} does not exist :: Failing early...", path);

            settings = null;

            return false;
        }

        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var result = DataSerializerFactory.GetSerializer(DataFormat.Toml).Deserialize<T?>(stream);

            if (result == null)
            {
                Logger.Warn(message: "Could not load settings from path {Path} :: Continuing may result in your settings being lost", path);

                settings = null;

                return false;
            }

            Logger.Trace(message: "Successfully loaded settings from path {Path}", path);
            settings = result;

            return true;
        }
    }

    /// <summary>Creates a new instance of the specified settings type with default values by invoking its default constructor.</summary>
    /// <typeparam name="T">The type of the settings object to initialize.</typeparam>
    /// <returns>A new instance of the specified settings type initialized with default values.</returns>
    public T GenerateDefaultSettings<T>() where T : class
    {
        Logger.Debug(message: "Generating default settings for type {QualifiedName} (constructor invocation)", typeof(T).FullDescription());

        return (T)Activator.CreateInstance(typeof(T), []);
    }

    /// <summary>Attempts to save the specified settings object to the specified file path by serializing it.</summary>
    /// <typeparam name="T">The type of the settings object to serialize.</typeparam>
    /// <param name="path">The file path where the settings object will be saved.</param>
    /// <param name="settings">The settings object to serialize and save.</param>
    /// <returns><c>true</c> if the settings were successfully saved; otherwise, <c>false</c>.</returns>
    public bool TrySaveSettings<T>(string path, [DisallowNull] T settings) where T : class
    {
        using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
            try
            {
                DataSerializerFactory.GetSerializer(DataFormat.Toml).Serialize(stream, settings);

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, message: "Could not save settings to file {Path}", path);

                return false;
            }
        }
    }
}
