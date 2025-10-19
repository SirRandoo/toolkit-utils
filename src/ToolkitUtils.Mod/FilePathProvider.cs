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
using System.IO;
using JetBrains.Annotations;
using NLog;
using ToolkitUtils.Mod.Logging;
using Verse;

namespace ToolkitUtils.Mod;

/// <summary>Provides methods to retrieve file paths for various types of files used by the mod.</summary>
[PublicAPI]
public static class FilePathProvider
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();
    private static readonly string BaseDirectory = GetDirectory(GenFilePaths.SaveDataFolderPath, directory: "Twitch Toolkit");
    private static readonly string DataBase = GetDirectory(BaseDirectory, directory: "data");
    private static readonly string SettingsBase = GetDirectory(BaseDirectory, directory: "settings");

    /// <summary>Retrieves the full file path for a data file in the designated data directory.</summary>
    /// <param name="fileName">The name of the data file.</param>
    /// <returns>The full path to the specified data file.</returns>
    public static string GetDataFile(string fileName) => Path.Combine(DataBase, fileName);

    /// <summary>Retrieves the full file path for a root-level file in the designated base directory.</summary>
    /// <param name="fileName">The name of the root-level file.</param>
    /// <returns>The full path to the specified root-level file.</returns>
    public static string GetRootFile(string fileName) => Path.Combine(BaseDirectory, fileName);

    /// <summary>Retrieves the full file path for a settings file in the designated settings directory.</summary>
    /// <param name="fileName">The name of the settings file.</param>
    /// <returns>The full path to the specified settings file.</returns>
    public static string GetSettingsFile(string fileName) => Path.Combine(SettingsBase, fileName);

    private static string GetDirectory(string parent, string directory, bool ensureExists = true)
    {
        string path = Path.Combine(parent, directory);

        if (!ensureExists || Directory.Exists(path)) return path;

        try { Directory.CreateDirectory(path); }
        catch (Exception e) { Logger.Error(e, message: "{Path} could not be created. Things may not work correctly, or at all", path); }

        return path;
    }
}
