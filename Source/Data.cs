using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Utils;
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
            if (Traits.NullOrEmpty())
            {
                LoadTraits(Paths.TraitFilePath, true);
            }

            if (PawnKinds.NullOrEmpty())
            {
                LoadPawnKinds(Paths.PawnKindFilePath, true);
            }

            if (File.Exists(Paths.LegacyShopFilePath) && (PawnKinds.NullOrEmpty() || Traits.NullOrEmpty()))
            {
                MigrateFromLegacy(Paths.LegacyShopFilePath);
                File.Move(Paths.LegacyShopFilePath, Path.ChangeExtension(Paths.LegacyShopFilePath, ".bak")!);
            }

            if (ItemData == null || ItemData.Count <= 0)
            {
                LoadItemData(Paths.ItemDataFilePath);
            }

            ValidateItemData();
            ValidatePawnKinds();
            ValidateTraits();
            ValidateModList();
            ValidateSurgeryList();

            if (TkSettings.Offload)
            {
                Task.Run(DumpAllData);
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
        public static List<RecipeDef> Surgeries { get; set; }

        private static void MigrateFromLegacy(string path)
        {
            XmlShop data;
            try
            {
                data = ShopExpansion.LoadData<XmlShop>(path);
            }
            catch (IOException e)
            {
                TkLogger.Error("Could not read legacy old data!", e);
                return;
            }

            if (data == null)
            {
                return;
            }

            Traits = data.Traits.Select(TraitItem.MigrateFrom).ToList();
            PawnKinds = data.Races.Select(PawnKindItem.MigrateFrom).ToList();
        }

        [CanBeNull]
        internal static T LoadJson<T>(string path, bool ignoreErrors = false, JsonSerializer serializer = null)
            where T : class
        {
            if (!File.Exists(path) && !ignoreErrors)
            {
                TkLogger.Warn($"Could not load file at {path} -- Does not exist!");
                return null;
            }

            try
            {
                using (FileStream file = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(file))
                    {
                        using (JsonReader jReader = new JsonTextReader(reader))
                        {
                            return (T) (serializer ?? JsonSerializer).Deserialize(jReader, typeof(T));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!ignoreErrors)
                {
                    TkLogger.Error($"Could not load file at {path}", e);
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

            try
            {
                using (FileStream writer = File.Open(tempPath, FileMode.Create, FileAccess.Write))
                {
                    using (var streamWriter = new StreamWriter(writer))
                    {
                        using (JsonWriter jWriter = new JsonTextWriter(streamWriter))
                        {
                            (serializer ?? JsonSerializer).Serialize(jWriter, obj);
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
                TkLogger.Error($"Could not save json to path {path}", e);
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
            List<string> tradeables = Store_ItemEditor.GetDefaultItems().Select(t => t.defName).ToList();
            List<string> toCull = ItemData.Keys.Where(dataKey => !tradeables.Contains(dataKey.CapitalizeFirst()))
               .ToList();

            foreach (string defName in toCull)
            {
                ItemData.Remove(defName);
            }

            foreach (ThingDef item in tradeables.Where(t => !ItemData.ContainsKey(t))
               .Select(i => DefDatabase<ThingDef>.GetNamed(i)))
            {
                ItemData[item.defName] = new ItemData
                {
                    IsMelee = item.IsMeleeWeapon,
                    IsRanged = item.IsRangedWeapon,
                    IsWeapon = item.IsWeapon,
                    Mod = item.modContentPack?.Name ?? "Unknown",
                    KarmaType = KarmaType.Neutral,
                    QuantityLimit = -1,
                    IsStuffAllowed = true
                };
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
            Surgeries = DefDatabase<RecipeDef>.AllDefs.Where(r => r.IsSurgery).ToList();
        }

        public static void SaveLegacyShop(string path)
        {
            SaveJson(new ShopLegacy {Races = PawnKinds, Traits = Traits}, path);
        }

        public static void DumpAllData()
        {
            SaveItemData(Paths.ItemDataFilePath);
            SaveModList();
            SaveCommands();

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

        public static void SaveCommands()
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
                Task.Run(() => SaveJson(container, Paths.CommandListFilePath));
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
                Task.Run(() => SaveJson(Mods, Paths.ModListFilePath));
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
                trait = Traits.FirstOrDefault(t => t.Name.StripTags().EqualsIgnoreCase(input.StripTags()));
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
                kind = PawnKinds.FirstOrDefault(t => t.Name.EqualsIgnoreCase(input));
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
               .Select(t => t.Name);
        }

        public static IEnumerable<string> GetKindResults(string input)
        {
            return PawnKinds.Where(k => k.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
               .Where(k => k.Enabled)
               .Select(k => k.Name);
        }

        public static IEnumerable<string> GetItemResults(string input)
        {
            return Items.Where(i => i.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
               .Where(i => i.Price > 0)
               .Select(i => i.Name);
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
                         && Viewers.GetViewer(viewer.ToLowerInvariant()) is {} v
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
