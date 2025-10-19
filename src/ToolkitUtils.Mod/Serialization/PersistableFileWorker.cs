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
using System.Threading.Tasks;
using ConcurrentCollections;
using JetBrains.Annotations;
using NLog;
using ToolkitUtils.Mod.Logging;

namespace ToolkitUtils.Mod.Serialization;

/// <summary>A class used to perform file operations on data types within the mod.</summary>
/// <param name="dataSerializer">The data serializer to used to transform data objects.</param>
/// <remarks>
///     The purpose of this class is to perform file operations within the mod. These operations are done in a
///     specific way as to ensure any errors that occur during a file operation will subsequently lock the file from
///     further modifications from the same file worker. These locks only apply to the current instance of the file worker,
///     and do not lock the file for other file workers, external programs, or the operating system.
/// </remarks>
[PublicAPI]
public sealed class PersistableFileWorker(IDataSerializer dataSerializer)
{
    private static readonly Logger Logger = UtilsLogFactory.Instance.GetCurrentClassLogger();
    private readonly ConcurrentHashSet<string> _blockedFiles = [];

    /// <summary>Returns whether a given path on disk is blocked from being written to.</summary>
    /// <param name="path">The path being queried about.</param>
    /// <remarks>
    ///     When a file worker encounters a problem loading data from disk, the file worker will forbid itself from
    ///     writing data to the file on disk to prevent data loss.
    /// </remarks>
    public bool IsSavingBlocked(string path) => _blockedFiles.Contains(Path.GetFullPath(path));

    /// <summary>Blocks a given path from being written to on disk by the file worker.</summary>
    /// <param name="path">The path being blocked.</param>
    /// <returns>Whether the path was blocked.</returns>
    /// <remarks>
    ///     When a file worker encounters a problems loading data from disk, the file worker will forbid itself from
    ///     writing data to a file on disk to prevent data loss.
    /// </remarks>
    public bool BlockSaving(string path) => _blockedFiles.Add(Path.GetFullPath(path));

    /// <summary>Loads and deserializes the contents of the file at the given path.</summary>
    /// <param name="path">The path to a file on disk to load data from.</param>
    public T? Load<T>(string path)
    {
        if (!File.Exists(path))
        {
            Logger.Warn(message: "File not found: {Path}", path);

            return default(T?);
        }

        try
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.SequentialScan))
            {
                return dataSerializer.Deserialize<T>(stream);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, message: "Could not load file from disk @ {FilePath}; further save attempts will be blocked", path);
            BlockSaving(path);

            return default(T?);
        }
    }

    /// <summary>Loads and deserializes the contents of the file at the given path asynchronously.</summary>
    /// <param name="path">The path to a file on disk to load data from.</param>
    public async Task<T?> LoadAsync<T>(string path)
    {
        if (!File.Exists(path))
        {
            Logger.Warn(message: "File not found: {Path}", path);

            return default(T?);
        }

        try
        {
            await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.SequentialScan | FileOptions.Asynchronous))
            {
                return await dataSerializer.DeserializeAsync<T>(stream);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, message: "Could not load file from disk @ {FilePath}; further save attempts will be blocked", path);
            BlockSaving(path);

            return default(T?);
        }
    }

    /// <summary>Atomically saves the data provided into the file at the given path.</summary>
    /// <param name="path">The path to a file on disk to save data to.</param>
    /// <param name="data">The data to save to disk.</param>
    public void Save<T>(string path, [DisallowNull] T data)
    {
        if (IsSavingBlocked(path))
        {
            Logger.Warn(message: "Save attempts are blocked on the file {Path}", path);

            return;
        }

        string? tempFileResult = GetTemporaryFile(path);

        if (string.IsNullOrEmpty(tempFileResult)) return;

        try
        {
            using (var stream = new FileStream(tempFileResult, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, bufferSize: 4096, FileOptions.SequentialScan))
            {
                dataSerializer.Serialize(stream, data);
            }

            ReplaceFile(tempFileResult!, path);
        }
        catch (Exception e) { Logger.Error(e, message: "Could not save file to disk @ {FilePath}", path); }
    }

    /// <summary>Atomically saves the data provided into the file at the given path asynchronously.</summary>
    /// <param name="path">The path to a file on disk to save data to.</param>
    /// <param name="data">The data to save to disk.</param>
    public async Task SaveAsync<T>(string path, [DisallowNull] T data)
    {
        if (IsSavingBlocked(path))
        {
            Logger.Warn(message: "Save attempts are blocked on the file {Path}", path);

            return;
        }

        string? tempFileResult = GetTemporaryFile(path);

        if (string.IsNullOrEmpty(tempFileResult)) return;

        try
        {
            await using (var stream = new FileStream(
                             tempFileResult,
                             FileMode.OpenOrCreate,
                             FileAccess.Write,
                             FileShare.Read,
                             bufferSize: 4096,
                             FileOptions.SequentialScan | FileOptions.Asynchronous
                         )) { await dataSerializer.SerializeAsync(stream, data); }

            ReplaceFile(tempFileResult!, path);
        }
        catch (Exception e) { Logger.Error(e, message: "Could not save file to disk @ {FilePath}", path); }
    }

    private string? GetTemporaryFile(string originalFilePath)
    {
        string? directory = Path.GetDirectoryName(originalFilePath);

        if (string.IsNullOrEmpty(directory))
        {
            Logger.Warn(message: "The path specified doesn't reside in a directory ({Path})", originalFilePath);

            return null;
        }

        string tempFileName = Path.GetRandomFileName();

        return Path.Combine(directory, tempFileName);
    }

    private void ReplaceFile(string sourceFile, string destinationFile)
    {
        if (!File.Exists(destinationFile))
        {
            Logger.Error(message: "The file at {Path} does not exist", destinationFile);

            return;
        }

        string fileExtension = Path.GetExtension(sourceFile);
        string backupFilePath = Path.ChangeExtension(sourceFile, $".bck.{fileExtension}");

        try { File.Replace(destinationFile, sourceFile, backupFilePath); }
        catch (Exception e) { Logger.Error(e, message: "Could not replace file {Target} with {Replacement}", sourceFile, destinationFile); }
    }
}
