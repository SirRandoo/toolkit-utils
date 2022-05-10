// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zlib;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Entities;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Models;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using UnityEngine;
using Utf8Json;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils
{
    /// <summary>
    ///     The main class responsible for loaded, saving, and indexing
    ///     various kinds of data within the mod, and game.
    /// </summary>
    [StaticConstructorOnStartup]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static partial class Data
    {
        internal static readonly Dictionary<string, Color> ColorIndex = GetDefaultColors();

        internal static readonly EnumRegistrar<KarmaType> KarmaTypes = new EnumRegistrar<KarmaType>();
        internal static readonly EnumRegistrar<QualityCategory> Qualities = new EnumRegistrar<QualityCategory>();
        internal static readonly EnumRegistrar<Gender> Genders = new EnumRegistrar<Gender>();
        internal static readonly EnumRegistrar<TechLevel> TechLevels = new EnumRegistrar<TechLevel>();
        internal static readonly EnumRegistrar<ComparisonTypes> ComparisonTypes = new EnumRegistrar<ComparisonTypes>();

        static Data()
        {
            // Just something to ensure the DomainIndexer's static constructor runs before
            // this ones.... as gross as that is.
            try
            {
                int _ = DomainIndexer.Mutators.Length;
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not index current game environment. Things will not work properly, if at all, you should report this immediately.", e);
            }

            LoadShopData();
            LoadItemData(Paths.ItemDataFilePath);

            ValidateData();

            if (TkSettings.Offload)
            {
                Task.Run(async () => await DumpAllDataAsync()).ConfigureAwait(false);
            }
            else
            {
                DumpAllData();
            }
        }

        private static void ValidateData()
        {
            try
            {
                ValidateModList();
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not serialize active mod list", e);
            }

            try
            {
                ValidateSurgeryList();
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not validate surgery list", e);
            }

            ValidateShopData();
        }

        private static void ValidateShopData()
        {
            try
            {
                ValidateItems();
                ValidateItemData();
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not validate item shop data", e);
            }

            try
            {
                ValidatePawnKinds();
                ValidatePawnKindData();
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not validate pawn shop data", e);
            }

            try
            {
                ValidateTraits();
                ValidateTraitData();
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not validate trait shop data", e);
            }

            try
            {
                ValidateEventList();
                ValidateEventData();
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not validate event shop data", e);
            }
        }

        private static void LoadShopData()
        {
            switch (TkSettings.DumpStyle)
            {
                case "MultiFile":
                {
                    if (Traits.NullOrEmpty())
                    {
                        LoadTraits(Paths.TraitFilePath, true);
                    }

                    if (PawnKinds.NullOrEmpty())
                    {
                        LoadPawnKinds(Paths.PawnKindFilePath, true);
                    }

                    break;
                }
                case "SingleFile":
                {
                    LoadFromLegacy(Paths.LegacyShopDumpFilePath);

                    break;
                }
            }
        }

        private static void LoadFromLegacy(string path)
        {
            var contents = LoadJson<ShopLegacy>(path, true);

            if (contents == null)
            {
                Traits = new List<TraitItem>();
                PawnKinds = new List<PawnKindItem>();

                return;
            }

            if (Traits.NullOrEmpty())
            {
                Traits = contents.Traits;
            }

            if (PawnKinds.NullOrEmpty())
            {
                PawnKinds = contents.Races;
            }
        }

        [CanBeNull]
        internal static T LoadJson<T>(string path, bool ignoreErrors = false) where T : class
        {
            if (!File.Exists(path) && !ignoreErrors)
            {
                TkUtils.Logger.Warn($"Could not load file at {path} -- Does not exist!");

                return null;
            }

            try
            {
                using (FileStream file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    return JsonSerializer.Deserialize<T>(file);
                }
            }
            catch (Exception e)
            {
                if (!ignoreErrors)
                {
                    TkUtils.Logger.Error($"Could not load file at {path}", e);
                }

                return null;
            }
        }

        internal static async Task<T> LoadJsonAsync<T>(string path, bool ignoreErrors = false) where T : class
        {
            if (!File.Exists(path) && !ignoreErrors)
            {
                TkUtils.Logger.Warn($"Could not load file at {path} -- Does not exist!");

                return null;
            }

            try
            {
                using (FileStream file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    return await JsonSerializer.DeserializeAsync<T>(file);
                }
            }
            catch (Exception e)
            {
                if (!ignoreErrors)
                {
                    TkUtils.Logger.Error($"Could not load file at {path}", e);
                }

                return null;
            }
        }

        internal static async Task<T> LoadCompressedJsonAsync<T>(string path, bool ignoreErrors = false) where T : class
        {
            if (!File.Exists(path) && !ignoreErrors)
            {
                TkUtils.Logger.Warn($"Could not load file at {path} -- Does not exist!");

                return null;
            }

            try
            {
                using (FileStream file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var zipStream = new GZipStream(file, CompressionMode.Decompress))
                    {
                        return await JsonSerializer.DeserializeAsync<T>(zipStream);
                    }
                }
            }
            catch (Exception e)
            {
                if (!ignoreErrors)
                {
                    TkUtils.Logger.Error($"Could not load file at {path}", e);
                }

                return null;
            }
        }

        internal static void SaveJson<T>(T obj, string path) where T : class
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                using (FileStream writer = File.Open(tempPath, FileMode.Create, FileAccess.Write))
                {
                    if (!TkSettings.MinifyData)
                    {
                        using (var sWriter = new StreamWriter(writer, Encoding.UTF8))
                        {
                            sWriter.Write(JsonSerializer.PrettyPrint(JsonSerializer.ToJsonString(obj)));
                        }
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, obj);
                    }
                }
            }
            catch (IOException e)
            {
                TkUtils.Logger.Error($"Could not save json to temporary file @ {tempPath}", e);
            }

            if (TryReplaceFile(tempPath, path))
            {
                return;
            }

            using (FileStream f = File.Open(Path.ChangeExtension(path, $"-{DateTime.Now.ToFileTime()}.json"), FileMode.Create, FileAccess.Write))
            {
                if (TkSettings.MinifyData)
                {
                    JsonSerializer.Serialize(f, obj);

                    return;
                }

                using (var writer = new StreamWriter(f))
                {
                    writer.Write(JsonSerializer.PrettyPrint(JsonSerializer.ToJsonString(obj)));
                }
            }
        }

        internal static async Task SaveJsonAsync<T>(T obj, string path) where T : class
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                using (FileStream file = File.Open(tempPath, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        if (!TkSettings.MinifyData)
                        {
                            await writer.WriteAsync(JsonSerializer.PrettyPrint(JsonSerializer.ToJsonString(obj)));
                        }
                        else
                        {
                            await JsonSerializer.SerializeAsync(file, obj);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                TkUtils.Logger.Error($"Could not save json to temporary file @ {tempPath}", e);
            }

            if (!TryReplaceFile(tempPath, path))
            {
                using (FileStream f = File.Open(Path.ChangeExtension(path, $"-{DateTime.Now.ToFileTime()}.json"), FileMode.Create, FileAccess.Write))
                {
                    if (TkSettings.MinifyData)
                    {
                        await JsonSerializer.SerializeAsync(f, obj);

                        return;
                    }

                    using (var writer = new StreamWriter(f))
                    {
                        await writer.WriteAsync(JsonSerializer.PrettyPrint(JsonSerializer.ToJsonString(obj)));
                    }
                }
            }
        }

        internal static async Task SaveCompressedJsonAsync<T>(T obj, string path) where T : class
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                using (FileStream file = File.Open(tempPath, FileMode.Create, FileAccess.Write))
                {
                    using (var zipStream = new GZipStream(file, CompressionMode.Compress))
                    {
                        await JsonSerializer.SerializeAsync(zipStream, obj);
                    }
                }
            }
            catch (IOException e)
            {
                TkUtils.Logger.Error($"Could not save json to temporary file @ {tempPath}", e);
            }

            if (TryReplaceFile(tempPath, path))
            {
                return;
            }

            using (FileStream f = File.Open(Path.ChangeExtension(path, $"-{DateTime.Now.ToFileTime()}.json"), FileMode.Create, FileAccess.Write))
            {
                using (var zipStream = new GZipStream(f, CompressionMode.Compress))
                {
                    await JsonSerializer.SerializeAsync(zipStream, obj);
                }
            }
        }

        /// <summary>
        ///     Saves shop data to the given file in the legacy format.
        /// </summary>
        /// <param name="path">The file to save shop data to</param>
        public static void SaveLegacyShop(string path)
        {
            SaveJson(new ShopLegacy { Races = PawnKinds, Traits = Traits }, path);
        }

        /// <summary>
        ///     Saves shop data to the given file in the legacy format.
        /// </summary>
        /// <param name="path">The file to save shop data to</param>
        public static async Task SaveLegacyShopAsync(string path)
        {
            await SaveJsonAsync(new ShopLegacy { Races = PawnKinds, Traits = Traits }, path);
        }

        /// <summary>
        ///     Saves all data within the mod to their associated files.
        /// </summary>
        public static void DumpAllData()
        {
            SaveItemData(Paths.ItemDataFilePath);
            SaveEventData(Paths.EventDataFilePath);
            SaveModList();
            DumpCommands();

            if (TkSettings.DumpStyle.Equals("SingleFile"))
            {
                SaveLegacyShop(Paths.LegacyShopDumpFilePath);
            }
            else
            {
                SaveTraits(Paths.TraitFilePath);
                SavePawnKinds(Paths.PawnKindFilePath);
            }
        }

        /// <summary>
        ///     Saves all data within the mod to their associated files.
        /// </summary>
        public static async Task DumpAllDataAsync()
        {
            await SaveItemDataAsync(Paths.ItemDataFilePath);
            await SaveEventDataAsync(Paths.EventDataFilePath);
            await SaveModListAsync();
            await DumpCommandsAsync();

            switch (TkSettings.DumpStyle)
            {
                case "SingleFile":
                    await SaveLegacyShopAsync(Paths.LegacyShopDumpFilePath);

                    break;
                default:
                    await SaveTraitsAsync(Paths.TraitFilePath);
                    await SavePawnKindsAsync(Paths.PawnKindFilePath);

                    break;
            }
        }

        [NotNull]
        private static Dictionary<string, Color> GetDefaultColors()
        {
            var container = new Dictionary<string, Color>
            {
                { "red", Color.red },
                { "blue", Color.blue },
                { "lime", Color.green },
                { "black", Color.black },
                { "white", Color.white },
                { "yellow", Color.yellow },
                { "cyan", Color.cyan },
                { "grey", Color.grey },
                { "gray", Color.gray },
                { "magenta", Color.magenta }
            };

            foreach (FieldInfo field in typeof(ColorLibrary).GetFields(BindingFlags.Static))
            {
                string name = field.Name;

                if (name.NullOrEmpty())
                {
                    continue;
                }

                object value = field.GetValue(typeof(ColorLibrary));

                if (!(value is Color color))
                {
                    continue;
                }

                container[name.ToLowerInvariant()] = color;
            }

            return container;
        }

        private static bool TryReplaceFile(string source, string dest)
        {
            string backupPath = Path.ChangeExtension(dest, ".bak");

            if (File.Exists(dest))
            {
                try
                {
                    File.Replace(source, dest, backupPath);

                    return true;
                }
                catch (IOException e1)
                {
                    TkUtils.Logger.Error($"Could not replace {dest} with {source}. Trying a different method...", e1);
                }
            }

            try
            {
                DeleteIfExists(backupPath);

                if (File.Exists(dest))
                {
                    File.Move(dest, backupPath);
                }

                File.Move(source, dest);

                return true;
            }
            catch (IOException e2)
            {
                TkUtils.Logger.Error($"Could not aggressively replace {dest} with {source}", e2);
                TkUtils.Context?.Post(s => Messages.Message($"{Path.GetFileName(dest)} could not be updated", MessageTypeDefOf.TaskCompletion, false), null);

                return false;
            }
        }

        private static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
