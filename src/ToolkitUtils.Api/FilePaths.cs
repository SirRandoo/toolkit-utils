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
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace ToolkitUtils.Api;

/// <summary>
///     The <c>FilePaths</c> class provides static paths for various directories and files used by the application.
///     These paths are essential for organizing and accessing configuration and data files specific to the application.
/// </summary>
[PublicAPI]
[StaticConstructorOnStartup]
public static class FilePaths
{
    /// <summary>
    ///     Represents the root directory path for the Twitch Toolkit within the application's save data folder. This path
    ///     serves as the primary location for storing Toolkit-related data and configuration directories.
    /// </summary>
    public static readonly string ToolkitRoot = Path.Combine(GenFilePaths.SaveDataFolderPath, path2: "Twitch Toolkit");

    /// <summary>
    ///     Represents the root directory path within the Twitch Toolkit's directory where data files are stored. This
    ///     directory acts as the primary repository for data resources required by the Toolkit, facilitating easy management
    ///     and access.
    /// </summary>
    public static readonly string DataRoot = Path.Combine(ToolkitRoot, path2: "Data");

    /// <summary>
    ///     Represents the directory path dedicated to storing settings files within the application's data directory.
    ///     This path is used for reading and writing application configuration files that define user-specific settings and
    ///     preferences.
    /// </summary>
    public static readonly string SettingsRoot = Path.Combine(ToolkitRoot, path2: "Settings");

    /// <summary>
    ///     Represents the file path for the items configuration file within the Twitch Toolkit's data directory. This
    ///     file contains serialized data pertinent to item definitions and properties used by the application.
    /// </summary>
    public static readonly string ItemFile = Path.Combine(DataRoot, path2: "items.json");

    /// <summary>
    ///     Represents the file path for the events configuration file within the data directory of the Twitch Toolkit.
    ///     This JSON file contains definitions and parameters for various events, which are crucial for the operation and
    ///     customization of the toolkit's event handling system.
    /// </summary>
    public static readonly string EventFile = Path.Combine(DataRoot, path2: "events.json");

    /// <summary>
    ///     Represents the file path to the JSON file containing pawn data within the Twitch Toolkit's data directory.
    ///     This file is utilized to store and retrieve information related to pawns, enabling the management of pawn-related
    ///     operations and data processing.
    /// </summary>
    public static readonly string PawnFile = Path.Combine(DataRoot, path2: "pawns.json");

    /// <summary>
    ///     Represents the file path for storing trait data in JSON format within the application's data directory. This
    ///     path is utilized for accessing and managing trait-related configurations and information.
    /// </summary>
    public static readonly string TraitFile = Path.Combine(DataRoot, path2: "traits.json");

    /// <summary>
    ///     Represents the path to the file where viewer-related data is stored within the Toolkit's data directory. This
    ///     file contains information specific to the viewers interacting with the platform.
    /// </summary>
    public static readonly string ViewerFile = Path.Combine(DataRoot, path2: "viewers.json");

    /// <summary>
    ///     Represents a read-only list of load folder paths used by the module to access various directories related to
    ///     version-specific mod data. This list is constructed based on the current version being used and is initialized at
    ///     startup to facilitate organized data retrieval.
    /// </summary>
    public static readonly IReadOnlyList<string> LoadFolderPaths;

    /// <summary>
    ///     Contains a read-only list of directory paths used for loading data folders associated with the Twitch Toolkit
    ///     module. These paths are derived from the configured load folders per the current mod version, enabling the
    ///     application to locate and access the necessary data resources for toolkit operations.
    /// </summary>
    public static readonly IReadOnlyList<string> DataFolderLoadPaths;

    static FilePaths()
    {
        ModMetaData metadata = ModLister.GetModWithIdentifier("sirrandoo.tku");
        List<LoadFolder> folders = metadata.LoadFoldersForVersion(VersionControl.CurrentVersionStringWithoutBuild);

        int totalFolders = folders.Count;
        var loadFolderPaths = new string[totalFolders];
        var dataFolderLoadPaths = new string[totalFolders];

        for (var index = 0; index < totalFolders; index++)
        {
            LoadFolder folder = folders[index];

            string path = Path.Combine(metadata.RootDir.ToString(), folder.folderName);

            loadFolderPaths[index] = path;
            dataFolderLoadPaths[index] = Path.Combine(path, path2: "Data");
        }

        LoadFolderPaths = loadFolderPaths;
        DataFolderLoadPaths = dataFolderLoadPaths;
    }
}
