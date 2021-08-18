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
using Viewers = TwitchToolkit.Viewers;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class Data
    {
        internal static readonly Dictionary<string, Color> ColorIndex =
            new Dictionary<string, Color>(GetSomeNamedColors());

        internal static readonly List<KarmaType> KarmaTypes = Enum.GetNames(typeof(KarmaType))
           .Select(i => (KarmaType)Enum.Parse(typeof(KarmaType), i))
           .ToList();

        internal static readonly Dictionary<string, QualityCategory> Qualities = Enum.GetNames(typeof(QualityCategory))
           .Select(q => (q.ToLowerInvariant(), (QualityCategory)Enum.Parse(typeof(QualityCategory), q)))
           .ToDictionary(p => p.Item1, q => q.Item2);

        internal static readonly Dictionary<string, Gender> Genders = Enum.GetNames(typeof(Gender))
           .Select(g => (g.ToLowerInvariant(), (Gender)Enum.Parse(typeof(Gender), g)))
           .ToDictionary(p => p.Item1, p => p.Item2);

        internal static readonly List<TechLevel> TechLevels = Enum.GetNames(typeof(TechLevel))
           .Select(t => (TechLevel)Enum.Parse(typeof(TechLevel), t))
           .ToList();

        internal static readonly IEnumerable<ComparisonTypes> ComparisonTypes = Enum.GetNames(typeof(ComparisonTypes))
           .Select(i => (ComparisonTypes)Enum.Parse(typeof(ComparisonTypes), i))
           .ToList();

        public static readonly List<HealthReport> HealthReports = new List<HealthReport>();

        static Data()
        {
            LoadShopData();
            LoadItemData(Paths.ItemDataFilePath);
            ValidateModList();
            ValidateItems();
            ValidateItemData();
            ValidatePawnKinds();
            ValidatePawnKindData();
            ValidateTraits();
            ValidateTraitData();
            ValidateSurgeryList();
            ValidateEventList();
            ValidateEventData();

            if (TkSettings.Offload)
            {
                Task.Run(async () => await DumpAllDataAsync()).ConfigureAwait(false);
            }
            else
            {
                DumpAllData();
            }
        }

        public static List<TraitItem> Traits { get; private set; }
        public static List<PawnKindItem> PawnKinds { get; private set; }
        public static Dictionary<string, ItemData> ItemData { get; private set; }
        public static ModItem[] Mods { get; private set; }
        public static List<ThingItem> Items { get; set; }
        public static List<SurgeryItem> Surgeries { get; private set; }
        public static List<EventItem> Events { get; private set; }

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

        private static void ValidateItems()
        {
            Items = StoreDialog.GetTradeables().Select(t => new ThingItem { Thing = t }).ToList();
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
                LogHelper.Warn($"Could not load file at {path} -- Does not exist!");
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
                    LogHelper.Error($"Could not load file at {path}", e);
                }

                return null;
            }
        }

        internal static async Task<T> LoadJsonAsync<T>(string path, bool ignoreErrors = false) where T : class
        {
            if (!File.Exists(path) && !ignoreErrors)
            {
                LogHelper.Warn($"Could not load file at {path} -- Does not exist!");
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
                    LogHelper.Error($"Could not load file at {path}", e);
                }

                return null;
            }
        }

        internal static async Task<T> LoadCompressedJsonAsync<T>(string path, bool ignoreErrors = false) where T : class
        {
            if (!File.Exists(path) && !ignoreErrors)
            {
                LogHelper.Warn($"Could not load file at {path} -- Does not exist!");
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
                    LogHelper.Error($"Could not load file at {path}", e);
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

            string tempPath = Path.GetTempFileName();

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

                if (File.Exists(path))
                {
                    File.Replace(tempPath, path, null);
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            catch (IOException e)
            {
                LogHelper.Error($"Could not save json to path {path}", e);
            }
        }

        internal static async Task SaveJsonAsync<T>(T obj, string path) where T : class
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            string tempPath = Path.GetTempFileName();

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

                if (File.Exists(path))
                {
                    File.Replace(tempPath, path, null);
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            catch (IOException e)
            {
                LogHelper.Error($"Could not save json to path {path}", e);
            }
        }

        internal static async Task SaveCompressedJsonAsync<T>(T obj, string path) where T : class
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            string tempPath = Path.GetTempFileName();

            try
            {
                using (FileStream file = File.Open(tempPath, FileMode.Create, FileAccess.Write))
                {
                    using (var zipStream = new GZipStream(file, CompressionMode.Compress))
                    {
                        await JsonSerializer.SerializeAsync(zipStream, obj);
                    }
                }

                if (File.Exists(path))
                {
                    File.Replace(tempPath, path, null);
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            catch (IOException e)
            {
                LogHelper.Error($"Could not save json to path {path}", e);
            }
        }

        public static void LoadTraits(string path, bool ignoreErrors = false)
        {
            Traits = LoadJson<List<TraitItem>>(path, ignoreErrors) ?? new List<TraitItem>();
        }

        public static async Task LoadTraitsAsync(string path, bool ignoreErrors = false)
        {
            Traits = await LoadJsonAsync<List<TraitItem>>(path, ignoreErrors) ?? new List<TraitItem>();
        }

        public static void SaveTraits(string path)
        {
            SaveJson(Traits, path);
        }

        public static async Task SaveTraitsAsync(string path)
        {
            await SaveJsonAsync(Traits, path);
        }

        public static void LoadPawnKinds(string path, bool ignoreErrors = false)
        {
            PawnKinds = LoadJson<List<PawnKindItem>>(path, ignoreErrors) ?? new List<PawnKindItem>();
        }

        public static async Task LoadPawnKindsAsync(string path, bool ignoreErrors = false)
        {
            PawnKinds = await LoadJsonAsync<List<PawnKindItem>>(path, ignoreErrors) ?? new List<PawnKindItem>();
        }

        public static void SavePawnKinds(string path)
        {
            SaveJson(PawnKinds, path);
        }

        public static async Task SavePawnKindsAsync(string path)
        {
            await SaveJsonAsync(PawnKinds, path);
        }

        public static void LoadItemData(string path)
        {
            ItemData = LoadJson<Dictionary<string, ItemData>>(path, true) ?? new Dictionary<string, ItemData>();
        }

        public static async Task LoadItemDataAsync(string path)
        {
            ItemData = await LoadJsonAsync<Dictionary<string, ItemData>>(path, true)
                       ?? new Dictionary<string, ItemData>();
        }

        public static void SaveItemData(string path)
        {
            SaveJson(ItemData, path);
        }

        public static async Task SaveItemDataAsync(string path)
        {
            await SaveJsonAsync(ItemData, path);
        }

        public static void SaveEventData(string path)
        {
            SaveJson(Events.ToDictionary(e => e.Name, e => e.EventData), path);
        }

        public static async Task SaveEventDataAsync(string path)
        {
            await SaveJsonAsync(Events.ToDictionary(e => e.Name, e => e.EventData), path);
        }

        private static void ValidateTraits()
        {
            List<TraitDef> traitDefs = DefDatabase<TraitDef>.AllDefsListForReading;
            Traits.RemoveAll(t => traitDefs.Find(d => d.defName.Equals(t.DefName)) == null);

            foreach (TraitDef def in traitDefs)
            {
                List<TraitItem> existing = Traits.FindAll(t => t.DefName.Equals(def.defName));

                if (existing.NullOrEmpty())
                {
                    Traits.AddRange(def.ToTraitItems());
                    continue;
                }

                TraitItem[] traitItems =
                    def.ToTraitItems().Where(t => !existing.Any(e => e.Degree == t.Degree)).ToArray();

                if (traitItems.Length > 0)
                {
                    Traits.AddRange(traitItems);
                }
            }
        }

        private static void ValidateTraitData()
        {
            var builder = new StringBuilder();
            foreach (TraitItem trait in Traits)
            {
                trait.TraitData ??= new TraitData();

                try
                {
                    trait.TraitData.Mod = trait.TraitDef.TryGetModName();
                }
                catch (Exception)
                {
                    builder.AppendLine($" - {trait.Name ?? trait.DefName}");
                }
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following traits could not be processed:\n");
            LogHelper.Warn(builder.ToString());
        }

        private static void ValidatePawnKinds()
        {
            List<PawnKindDef> kindDefs = DefDatabase<PawnKindDef>.AllDefs.Where(k => k.RaceProps.Humanlike).ToList();
            PawnKinds.RemoveAll(k => kindDefs.Find(d => d.race.defName.Equals(k.DefName)) == null);

            foreach (PawnKindDef def in kindDefs)
            {
                List<PawnKindItem> item = PawnKinds.FindAll(k => k.DefName.Equals(def.race.defName));

                if (item.Count > 0)
                {
                    continue;
                }

                PawnKinds.Add(
                    new PawnKindItem
                    {
                        DefName = def.race.defName,
                        Enabled = true,
                        Name = def.race.label ?? def.race.defName,
                        Cost = def.race.CalculateStorePrice()
                    }
                );
            }
        }

        private static void ValidatePawnKindData()
        {
            var builder = new StringBuilder();
            foreach (PawnKindItem pawn in PawnKinds)
            {
                pawn.PawnData ??= new PawnKindData();

                try
                {
                    pawn.PawnData.Mod = pawn.ColonistKindDef.TryGetModName();
                    pawn.UpdateStats();
                }
                catch (Exception)
                {
                    builder.AppendLine($" - {pawn.Name ?? pawn.DefName}");
                }
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following pawn kinds could not be processed:\n");
            LogHelper.Warn(builder.ToString());
        }

        private static void ValidateItemData()
        {
            ItemData ??= new Dictionary<string, ItemData>();
            List<string> tradeables = StoreDialog.GetTradeables().Select(t => t.defName).ToList();
            List<string> toCull = ItemData.Keys.Where(dataKey => !tradeables.Contains(dataKey.CapitalizeFirst()))
               .ToList();

            foreach (string defName in toCull)
            {
                ItemData.Remove(defName);
            }

            var builder = new StringBuilder();
            foreach (ThingDef item in tradeables.Where(t => !ItemData.ContainsKey(t))
               .Select(i => DefDatabase<ThingDef>.GetNamed(i)))
            {
                ModContentPack contentPack = item.modContentPack;
                var data = new ItemData
                {
                    Version = Models.ItemData.CurrentVersion,
                    QuantityLimit = -1,
                    IsStuffAllowed = true,
                    IsUsable = GameHelper.GetDefaultUsability(item),
                    IsWearable = true,
                    IsEquippable = true
                };

                if (contentPack != null)
                {
                    data.Mod = contentPack.IsCoreMod ? "RimWorld" : contentPack.Name ?? "Unknown";
                }

                ItemData[item.defName] = data;

                try
                {
                    data.IsMelee = item.IsMeleeWeapon;
                    data.IsRanged = item.IsRangedWeapon;
                    data.IsWeapon = item.IsWeapon;
                }
                catch (Exception e)
                {
                    builder.Append(
                        $"Failed to gather weapon data for item '{item.label ?? "Unknown"}' from mod '{item.TryGetModName()}'"
                    );
                    builder.AppendLine($" -- Exception: {e.GetType().Name}({e.Message ?? "No message"})");
                }
            }

            foreach ((string defName, ItemData data) in ItemData.Where(
                data => data.Value.Version < Models.ItemData.CurrentVersion
            ))
            {
                ThingItem item = Items.Find(i => i.DefName?.Equals(defName) == true);

                data.IsUsable = item?.Thing == null || GameHelper.GetDefaultUsability(item.Thing);
                data.IsWearable = true;
                data.IsEquippable = true;
                data.Version = Models.ItemData.CurrentVersion;
            }
        }

        private static void ValidateModList()
        {
            List<ModMetaData> running = ModsConfig.ActiveModsInLoadOrder.ToList();

            var list = new List<ModItem>();
            var builder = new StringBuilder();
            foreach (ModMetaData metaData in running.Where(m => m.Active)
               .Where(mod => !mod.Official)
               .Where(mod => !File.Exists(Path.Combine(mod.RootDir.ToString(), "About/IgnoreMe.txt"))))
            {
                ModItem item;

                try
                {
                    item = ModItem.FromMetadata(metaData);
                }
                catch (Exception)
                {
                    builder.AppendLine($" - {metaData?.Name ?? metaData?.FolderName}");
                    continue;
                }

                list.Add(item);
            }

            if (builder.Length > 0)
            {
                builder.Insert(0, "The following mods could not be processed:\n");
                LogHelper.Warn(builder.ToString());
            }

            Mods = list.ToArray();
        }

        private static void ValidateSurgeryList()
        {
            Surgeries = new List<SurgeryItem>();
            foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefs)
            {
                ISurgeryHandler handler = CompatRegistry.SurgeryHandlers.FirstOrDefault(i => i.IsSurgery(recipe));

                if (handler == null)
                {
                    continue;
                }

                Surgeries.Add(new SurgeryItem { Surgery = recipe, Handler = handler });
            }
        }

        private static void ValidateEventList()
        {
            Store_IncidentEditor.LoadCopies(); // Just to ensure the actual incidents are loaded.
            Events = DefDatabase<StoreIncident>.AllDefs.Select(i => new EventItem { Incident = i }).ToList();
        }

        private static void ValidateEventData()
        {
            var builder = new StringBuilder();
            foreach (EventItem ev in Events)
            {
                ev.EventData ??= new EventData();

                try
                {
                    ev.EventData.Mod = ev.Incident.TryGetModName();
                    ev.EventData.EventType =
                        ev.Incident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.Default;
                }
                catch (Exception)
                {
                    builder.AppendLine($" - {ev.Name ?? ev.DefName}");
                }
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following events could not be processed:\n");
            LogHelper.Warn(builder.ToString());
        }

        public static void SaveLegacyShop(string path)
        {
            SaveJson(new ShopLegacy { Races = PawnKinds, Traits = Traits }, path);
        }

        public static async Task SaveLegacyShopAsync(string path)
        {
            await SaveJsonAsync(new ShopLegacy { Races = PawnKinds, Traits = Traits }, path);
        }

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

        public static void DumpCommands()
        {
            List<CommandItem> container = DefDatabase<Command>.AllDefs.Where(c => c.enabled)
               .Where(c => !c.command.NullOrEmpty())
               .Select(CommandItem.FromToolkit)
               .ToList();

            container.AddRange(
                DefDatabase<ToolkitChatCommand>.AllDefsListForReading.Where(c => c.enabled)
                   .Select(CommandItem.FromToolkitCore)
            );

            SaveJson(container, Paths.CommandListFilePath);
        }

        public static async Task DumpCommandsAsync()
        {
            List<CommandItem> container = DefDatabase<Command>.AllDefs.Where(c => c.enabled)
               .Where(c => !c.command.NullOrEmpty())
               .Select(CommandItem.FromToolkit)
               .ToList();

            container.AddRange(
                DefDatabase<ToolkitChatCommand>.AllDefs.Where(c => c.enabled).Select(CommandItem.FromToolkitCore)
            );

            await SaveJsonAsync(container, Paths.CommandListFilePath);
        }

        public static void SaveModList()
        {
            SaveJson(Mods, Paths.ModListFilePath);
        }

        public static async Task SaveModListAsync()
        {
            await SaveJsonAsync(Mods, Paths.ModListFilePath);
        }

        [ContractAnnotation("input:notnull => true,trait:notnull; input:notnull => false,trait:null")]
        public static bool TryGetTrait(string input, out TraitItem trait)
        {
            if (input.StartsWith("$"))
            {
                input = input.Substring(1);

                trait = Traits.FirstOrDefault(t => t.DefName.Equals(input));
            }
            else
            {
                trait = Traits.FirstOrDefault(
                    t => t.Name.ToToolkit().StripTags().EqualsIgnoreCase(input.ToToolkit().StripTags())
                );
            }

            return trait != null;
        }

        [ContractAnnotation("input:notnull => true,kind:notnull; input:notnull => false,kind:null")]
        public static bool TryGetPawnKind(string input, out PawnKindItem kind)
        {
            if (input.StartsWith("$"))
            {
                input = input.Substring(1);

                kind = PawnKinds.FirstOrDefault(t => t.DefName.Equals(input));
            }
            else
            {
                kind = PawnKinds.FirstOrDefault(t => t.Name.ToToolkit().EqualsIgnoreCase(input.ToToolkit()));
            }

            return kind != null;
        }

        [NotNull]
        public static IEnumerable<string> GetTraitResults(string input)
        {
            return Traits
               .Where(
                    t => t.Name.StripTags().StartsWith(input.StripTags(), StringComparison.InvariantCultureIgnoreCase)
                )
               .Where(t => t.CanAdd || t.CanRemove)
               .Select(t => t.Name.StripTags().ToToolkit());
        }

        [NotNull]
        public static IEnumerable<string> GetKindResults(string input)
        {
            return PawnKinds.Where(k => k.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
               .Where(k => k.Enabled)
               .Select(k => k.Name.ToToolkit());
        }

        [NotNull]
        public static IEnumerable<string> GetItemResults(string input)
        {
            return Items.Where(i => i.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
               .Where(i => i.Cost > 0)
               .Select(i => i.Name.ToToolkit());
        }

        [NotNull]
        public static IEnumerable<string> GetEventResults(string input)
        {
            foreach (string simpleIncidentName in Purchase_Handler.allStoreIncidentsSimple.Where(i => i.cost > 0)
               .Where(i => i.abbreviation.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
               .Select(i => i.abbreviation))
            {
                yield return simpleIncidentName;
            }

            foreach (string variablesIncidentName in Purchase_Handler.allStoreIncidentsVariables
               .Where(i => i.cost > 0 || i.defName.Equals("Item") && i.cost >= 0)
               .Where(i => i.abbreviation.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
               .Select(i => i.abbreviation))
            {
                yield return variablesIncidentName;
            }
        }

        [NotNull]
        public static IEnumerable<string> GetCommandResults(string input, [CanBeNull] string viewer = null)
        {
            return DefDatabase<Command>.AllDefs.Where(c => c.enabled)
               .Where(c => c.command.EqualsIgnoreCase(input))
               .Where(
                    c => viewer != null
                         && Viewers.GetViewer(viewer.ToLowerInvariant()) is { } v
                         && (v.mod && c.requiresMod || v.username == ToolkitSettings.Channel && c.requiresAdmin)
                )
               .Select(c => c.command);
        }

        [CanBeNull]
        public static string GetViewerColorCode([NotNull] string viewer)
        {
            return !ToolkitSettings.ViewerColorCodes.TryGetValue(viewer.ToLowerInvariant(), out string color)
                ? null
                : color;
        }

        public static void LoadItemPartial([NotNull] IEnumerable<ItemPartial> partialData)
        {
            var builder = new StringBuilder();
            foreach (ItemPartial partial in partialData)
            {
                if (partial.DefName == null)
                {
                    builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}\n");
                    continue;
                }

                ThingItem existing = Items.Find(i => i.DefName!.Equals(partial.DefName));

                if (existing == null)
                {
                    ThingDef thing = DefDatabase<ThingDef>.GetNamed(partial.DefName, false);
                    var item = Item.GetItemFromDefName(partial.DefName);

                    if (thing == null || item == null)
                    {
                        builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}\n");
                        continue;
                    }

                    item.price = partial.Cost;
                    Items.Add(new ThingItem { Thing = thing, Item = item, ItemData = partial.ItemData });
                    continue;
                }

                existing.Name = partial.Name;
                existing.Cost = partial.Cost;
                existing.ItemData = partial.ItemData;
                existing.Update();
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following items could not be loaded from the partial data provided:\n");
            LogHelper.Warn(builder.ToString());
        }

        public static void LoadEventPartial([NotNull] IEnumerable<EventPartial> partialData)
        {
            var builder = new StringBuilder();
            foreach (EventPartial partial in partialData)
            {
                EventItem existing = Events.Find(i => i.DefName.Equals(partial.DefName));

                if (existing == null)
                {
                    StoreIncident incident = DefDatabase<StoreIncident>.GetNamed(partial.DefName, false);

                    if (incident == null)
                    {
                        builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}\n");
                        continue;
                    }

                    var e = new EventItem
                    {
                        Incident = incident,
                        Name = partial.Name,
                        Cost = partial.Cost,
                        EventCap = partial.EventCap,
                        KarmaType = partial.KarmaType
                    };

                    if (e.IsVariables)
                    {
                        e.MaxWager = partial.MaxWager;
                    }

                    e.EventData = partial.EventData;
                    Events.Add(e);
                    continue;
                }

                existing.Name = partial.Name;
                existing.Cost = partial.Cost;
                existing.EventCap = partial.EventCap;
                existing.KarmaType = partial.KarmaType;

                if (existing.IsVariables)
                {
                    existing.MaxWager = partial.MaxWager;
                }

                existing.EventData = partial.EventData;
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following events could not be loaded from the partial data provided:\n");
            LogHelper.Warn(builder.ToString());
        }

        public static void LoadTraitPartial([NotNull] IEnumerable<TraitItem> partialData)
        {
            var builder = new StringBuilder();
            foreach (TraitItem partial in partialData)
            {
                TraitItem existing = Traits.Find(i => i.DefName.Equals(partial.DefName) && i.Degree == partial.Degree);

                if (existing == null)
                {
                    if (partial.TraitDef == null)
                    {
                        builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}:{partial.Degree}\n");
                        continue;
                    }

                    Traits.Add(partial);
                    continue;
                }

                existing.Name = partial.Name;
                existing.CanAdd = partial.CanAdd;
                existing.CostToAdd = partial.CostToAdd;
                existing.CanRemove = partial.CanRemove;
                existing.CostToRemove = partial.CostToRemove;
                existing.Data = partial.Data;
            }

            if (builder.Length <= 0)
            {
                return;
            }

            builder.Insert(0, "The following traits could not be loaded from the partial data provided:\n");
            LogHelper.Warn(builder.ToString());
        }

        public static void LoadPawnPartial([NotNull] IEnumerable<PawnKindItem> partialData)
        {
            var builder = new StringBuilder();
            foreach (PawnKindItem partial in partialData)
            {
                PawnKindItem existing = PawnKinds.Find(i => i.DefName.Equals(partial.DefName));

                if (existing == null)
                {
                    if (partial.ColonistKindDef == null)
                    {
                        builder.Append($"  - {partial.Data?.Mod ?? "UNKNOWN"}:{partial.DefName}\n");
                        continue;
                    }

                    PawnKinds.Add(partial);
                    continue;
                }

                existing.Name = partial.Name;
                existing.Cost = partial.Cost;
                existing.Enabled = partial.Enabled;
                existing.Data = partial.Data;
            }
        }

        private static IEnumerable<KeyValuePair<string, Color>> GetSomeNamedColors()
        {
            yield return KeyValuePair.Create("red", Color.red);
            yield return KeyValuePair.Create("blue", Color.blue);
            yield return KeyValuePair.Create("lime", Color.green);
            yield return KeyValuePair.Create("limegreen", Color.green);
            yield return KeyValuePair.Create("black", Color.black);
            yield return KeyValuePair.Create("white", Color.white);
            yield return KeyValuePair.Create("yellow", Color.yellow);
            yield return KeyValuePair.Create("cyan", Color.cyan);
            yield return KeyValuePair.Create("gray", Color.gray);
            yield return KeyValuePair.Create("grey", Color.grey);
            yield return KeyValuePair.Create("magenta", Color.magenta);

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

                yield return new KeyValuePair<string, Color>(name.ToLowerInvariant(), color);
            }
        }
    }
}
