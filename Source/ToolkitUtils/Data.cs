using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
using SirRandoo.ToolkitUtils.Utils.ModComp;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Models;
using TwitchToolkit;
using TwitchToolkit.Store;
using Verse;
using Command = TwitchToolkit.Command;
using Viewers = TwitchToolkit.Viewers;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class Data
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = {new StringEnumConverter()},
            Formatting = Formatting.Indented
        };

        private static readonly JsonSerializer AltJsonSerializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new DefaultContractResolver(),
            Converters = {new StringEnumConverter()},
            Formatting = Formatting.Indented
        };

        static Data()
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

            if (File.Exists(Paths.LegacyShopFilePath) && PawnKinds.NullOrEmpty() && Traits.NullOrEmpty())
            {
                MigrateFromLegacy(Paths.LegacyShopFilePath);

                try
                {
                    File.Move(Paths.LegacyShopFilePath, Path.ChangeExtension(Paths.LegacyShopFilePath, ".bak")!);
                }
                catch (IOException)
                {
                    LogHelper.Warn("Could not move old shop file; a similar one exists.");
                }
            }

            if (ItemData == null || ItemData.Count <= 0)
            {
                LoadItemData(Paths.ItemDataFilePath);
            }

            ValidateItems();
            ValidateItemData();
            ValidatePawnKinds();
            ValidateTraits();
            ValidateModList();
            ValidateSurgeryList();

            if (TkSettings.Offload)
            {
                Task.Run(DumpAllData).ConfigureAwait(false);
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
        public static List<SurgeryItem> Surgeries { get; set; }

        private static void ValidateItems()
        {
            Items = StoreDialog.GetTradeables().Select(t => new ThingItem {Thing = t}).ToList();
        }

        private static void MigrateFromLegacy(string path)
        {
            XmlShop data;
            try
            {
                data = ShopExpansion.LoadData<XmlShop>(path);
            }
            catch (IOException e)
            {
                LogHelper.Error("Could not read legacy old data!", e);
                return;
            }

            if (data == null)
            {
                return;
            }

            Traits = data.Traits.Select(TraitItem.MigrateFrom).ToList();
            PawnKinds = data.Races.Select(PawnKindItem.MigrateFrom).ToList();
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
        internal static T LoadJson<T>(string path, bool ignoreErrors = false, JsonSerializer serializer = null)
            where T : class
        {
            if (!File.Exists(path) && !ignoreErrors)
            {
                LogHelper.Warn($"Could not load file at {path} -- Does not exist!");
                return null;
            }

            JsonSerializer s = serializer ?? JsonSerializer;

            try
            {
                using (FileStream file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(file))
                    {
                        using (JsonReader jReader = new JsonTextReader(reader))
                        {
                            s.Formatting = TkSettings.MinifyData ? Formatting.None : Formatting.Indented;
                            var data = (T) s.Deserialize(jReader, typeof(T));
                            s.Formatting = Formatting.Indented;

                            return data;
                        }
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

        internal static void SaveJson<T>(T obj, string path, JsonSerializer serializer = null)
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            string tempPath = Path.GetTempFileName();
            JsonSerializer s = serializer ?? JsonSerializer;

            try
            {
                using (FileStream writer = File.Open(tempPath, FileMode.Create, FileAccess.Write))
                {
                    using (var streamWriter = new StreamWriter(writer))
                    {
                        using (JsonWriter jWriter = new JsonTextWriter(streamWriter))
                        {
                            s.Formatting = TkSettings.MinifyData ? Formatting.Indented : Formatting.None;
                            s.Serialize(jWriter, obj);
                            s.Formatting = Formatting.Indented;
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

        public static void LoadTraits(string path, bool ignoreErrors = false)
        {
            Traits = LoadJson<List<TraitItem>>(path, ignoreErrors) ?? new List<TraitItem>();
        }

        public static void SaveTraits(string path)
        {
            SaveJson(Traits, path);
        }

        public static void LoadPawnKinds(string path, bool ignoreErrors = false)
        {
            PawnKinds = LoadJson<List<PawnKindItem>>(path, ignoreErrors) ?? new List<PawnKindItem>();
        }

        public static void SavePawnKinds(string path)
        {
            SaveJson(PawnKinds, path);
        }

        public static void LoadItemData(string path)
        {
            ItemData = LoadJson<Dictionary<string, ItemData>>(path, true, AltJsonSerializer)
                       ?? new Dictionary<string, ItemData>();
        }

        public static void SaveItemData(string path)
        {
            SaveJson(ItemData, path, AltJsonSerializer);
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

        private static void ValidateItemData()
        {
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
                var data = new ItemData {QuantityLimit = -1, IsStuffAllowed = true};

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
                        $"Failed to gather weapon data for item '{item.label ?? "Unknown"}' from mod '{item.modContentPack?.Name ?? "Unknown"}'"
                    );
                    builder.AppendLine($" -- Exception: {e.GetType().Name}({e.Message ?? "No message"})");
                }
            }

            foreach (ItemData data in ItemData.Values.Where(data => data.Mod.EqualsIgnoreCase("core")))
            {
                data.Mod = "RimWorld";
            }
        }

        private static void ValidateModList()
        {
            List<ModMetaData> running = ModsConfig.ActiveModsInLoadOrder.ToList();

            Mods = running.Where(m => m.Active)
               .Where(mod => !mod.Official)
               .Where(mod => !File.Exists(Path.Combine(mod.RootDir.ToString(), "About/IgnoreMe.txt")))
               .Select(ModItem.FromMetadata)
               .ToArray();
        }

        private static void ValidateSurgeryList()
        {
            Surgeries = new List<SurgeryItem>();
            foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefs)
            {
                if (Androids.Active && Androids.IsAndroidSurgery(recipe))
                {
                    Surgeries.Add(new SurgeryItem {IsForAndroids = true, Surgery = recipe});
                }
                else if (recipe.IsSurgery)
                {
                    Surgeries.Add(new SurgeryItem {IsForAndroids = false, Surgery = recipe});
                }
            }
        }

        public static void SaveLegacyShop(string path)
        {
            SaveJson(new ShopLegacy {Races = PawnKinds, Traits = Traits}, path);
        }

        public static void DumpAllData()
        {
            SaveItemData(Paths.ItemDataFilePath);
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

        public static void DumpCommands()
        {
            List<CommandItem> container = DefDatabase<Command>.AllDefs.Where(c => c.enabled)
               .Select(CommandItem.FromToolkit)
               .ToList();

            container.AddRange(
                DefDatabase<ToolkitChatCommand>.AllDefsListForReading.Where(c => c.enabled)
                   .Select(CommandItem.FromToolkitCore)
            );

            if (TkSettings.Offload)
            {
                Task.Run(() => SaveJson(container, Paths.CommandListFilePath)).ConfigureAwait(false);
            }
            else
            {
                SaveJson(container, Paths.CommandListFilePath);
            }
        }

        public static void SaveModList()
        {
            if (TkSettings.Offload)
            {
                Task.Run(() => SaveJson(Mods, Paths.ModListFilePath)).ConfigureAwait(false);
            }
            else
            {
                SaveJson(Mods, Paths.ModListFilePath);
            }
        }

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

        public static IEnumerable<string> GetTraitResults(string input)
        {
            return Traits
               .Where(
                    t => t.Name.StripTags().StartsWith(input.StripTags(), StringComparison.InvariantCultureIgnoreCase)
                )
               .Where(t => t.CanAdd || t.CanRemove)
               .Select(t => t.Name.StripTags().ToToolkit());
        }

        public static IEnumerable<string> GetKindResults(string input)
        {
            return PawnKinds.Where(k => k.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
               .Where(k => k.Enabled)
               .Select(k => k.Name.ToToolkit());
        }

        public static IEnumerable<string> GetItemResults(string input)
        {
            return Items.Where(i => i.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
               .Where(i => i.Price > 0)
               .Select(i => i.Name.ToToolkit());
        }

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

        public static IEnumerable<string> GetCommandResults(string input, string viewer = null)
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

        public static string GetViewerColorCode(string viewer)
        {
            return !ToolkitSettings.ViewerColorCodes.TryGetValue(viewer.ToLowerInvariant(), out string color)
                ? null
                : color;
        }
    }
}