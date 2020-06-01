using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using RimWorld;
using SirRandoo.ToolkitUtils.Windows;
using ToolkitCore.Models;
using TwitchToolkit.Utilities;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class ShopExpansionHelper
    {
        public static readonly string ExpansionFile = Path.Combine(SaveHelper.dataPath, "ShopExt_1.xml");

        private static readonly string ShopFile = Path.Combine(SaveHelper.dataPath, "ShopExt.json");
        private static readonly string CommandsFile = Path.Combine(SaveHelper.dataPath, "commands.json");
        private static readonly string ModsFile = Path.Combine(SaveHelper.dataPath, "modlist.json");
        private static readonly string TraitsFile = Path.Combine(SaveHelper.dataPath, "Traits.json");
        private static readonly string RaceFile = Path.Combine(SaveHelper.dataPath, "Races.json");

        public static void SaveData<T>(T xml, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            if (directory == null)
            {
                TkLogger.Warn($"File path @ {filePath} is invalid!");
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializer = new XmlSerializer(typeof(T));
            string tempFile = $"{filePath}.tmp";
            string backupFile = $"{filePath}.bak";

            try
            {
                if (File.Exists(filePath))
                {
                    using (FileStream writer = File.Open(tempFile, FileMode.Create, FileAccess.Write))
                    {
                        serializer.Serialize(writer, xml);
                    }

                    File.Replace(tempFile, filePath, backupFile);
                }
                else
                {
                    using FileStream writer = File.Open(filePath, FileMode.Create, FileAccess.Write);
                    serializer.Serialize(writer, xml);
                }
            }
            catch (IOException e)
            {
                TkLogger.Error($"Could not save data to {filePath}", e);
            }
            catch (UnauthorizedAccessException e)
            {
                TkLogger.Error("File access denied", e);
            }
        }

        private static void SaveData(string data, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            if (directory == null)
            {
                TkLogger.Warn($"File path @ {filePath} is invalid!");
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string tempFile = $"{filePath}.tmp";
            string backupFile = $"{filePath}.bak";

            try
            {
                if (File.Exists(filePath))
                {
                    File.WriteAllText(tempFile, data);
                    File.Replace(tempFile, filePath, backupFile);
                }
                else
                {
                    File.WriteAllText(filePath, data);
                }
            }
            catch (IOException e)
            {
                TkLogger.Error($"Could not save data to {filePath}", e);
            }
            catch (UnauthorizedAccessException e)
            {
                TkLogger.Error("File access denied", e);
            }
        }

        public static T LoadData<T>(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                throw new IOException($"Directory {directory} does not exist!");
            }

            var serializer = new XmlSerializer(typeof(T));

            using StreamReader reader = File.OpenText(filePath);
            return (T) serializer.Deserialize(reader);
        }

        public static void DumpShopExpansion()
        {
            List<TraitDef> traits = DefDatabase<TraitDef>.AllDefsListForReading;
            var container = new List<TraitDump>();

            foreach (XmlTrait trait in TkUtils.ShopExpansion.Traits)
            {
                var t = new TraitDump
                {
                    addPrice = trait.AddPrice,
                    bypassLimit = trait.BypassLimit,
                    degree = trait.Degree,
                    canAdd = trait.CanAdd,
                    canRemove = trait.CanRemove,
                    name = trait.Name.ToToolkit(),
                    defName = trait.DefName,
                    removePrice = trait.RemovePrice
                };

                TraitDef def = traits.FirstOrDefault(i => i.defName.Equals(trait.DefName));

                if (def == null)
                {
                    continue;
                }

                var inst = new Trait(def, trait.Degree);

                t.conflicts = def.conflictingTraits
                    .SelectMany(
                        i => TraitHelper.GetEffectiveTraits(i)?.Select(c => Unrichify.StripTags(c.Name).ToToolkit())
                    )
                    .ToArray();

                var statContainer = new List<string>();

                if (inst.CurrentData.statOffsets != null)
                {
                    statContainer.AddRange(
                        inst.CurrentData.statOffsets.Select(
                            offset => $"{offset.ValueToStringAsOffset} {offset.stat.LabelForFullStatListCap}"
                        )
                    );
                }

                if (inst.CurrentData.statFactors != null)
                {
                    statContainer.AddRange(
                        inst.CurrentData.statFactors.Select(
                            factor => $"{factor.ToStringAsFactor} {factor.stat.LabelForFullStatListCap}"
                        )
                    );
                }

                t.description = inst.CurrentData.description;
                t.stats = statContainer.ToArray();

                container.Add(t);
            }

            List<RaceDump> jsonRaces = TkUtils.ShopExpansion.Races.Select(
                    r => new RaceDump
                    {
                        defName = r.DefName, enabled = r.Enabled, name = r.Name.ToToolkit(), price = r.Price
                    }
                )
                .ToList();

            if (TkSettings.DumpStyle.EqualsIgnoreCase("SingleFile"))
            {
                SaveData(
                    JsonConvert.SerializeObject(
                        new ShopDump {races = jsonRaces, traits = container},
                        Formatting.Indented
                    ),
                    ShopFile
                );
                return;
            }

            if (!TkSettings.DumpStyle.EqualsIgnoreCase("MultiFile"))
            {
                return;
            }

            SaveData(JsonConvert.SerializeObject(container, Formatting.Indented), TraitsFile);
            SaveData(JsonConvert.SerializeObject(jsonRaces, Formatting.Indented), RaceFile);
        }

        public static void DumpModList()
        {
            SaveData(JsonConvert.SerializeObject(TkUtils.ModListCache, Formatting.Indented), ModsFile);
        }

        public static void DumpCommands()
        {
            List<Command> commands = DefDatabase<Command>.AllDefsListForReading;
            List<CommandDump> container = commands
                .Where(c => c.enabled && c.HasModExtension<CommandExtension>())
                .Select(
                    c =>
                    {
                        var ext = c.GetModExtension<CommandExtension>();

                        var dump = new CommandDump
                        {
                            name = c.LabelCap.RawText,
                            description = ext.Description,
                            usage = $"!{c.command}",
                            shortcut = c.commandDriver.Name.Equals("Buy") && !c.defName.Equals("Buy"),
                            userLevel = nameof(ext.UserLevel)
                        };

                        if (!ext.Parameters.NullOrEmpty())
                        {
                            dump.usage += " ";
                            dump.usage += string.Join(
                                " ",
                                ext.Parameters.Select(i => i.ToString().ToLowerInvariant()).ToArray()
                            );
                        }

                        if (c.requiresAdmin || c.requiresMod)
                        {
                            dump.userLevel = nameof(UserLevels.Moderator);
                        }

                        return dump;
                    }
                )
                .ToList();

            container.AddRange(
                DefDatabase<ToolkitChatCommand>.AllDefsListForReading
                    .Where(c => c.enabled && c.HasModExtension<CommandExtension>())
                    .Select(
                        c =>
                        {
                            var ext = c.GetModExtension<CommandExtension>();

                            var dump = new CommandDump
                            {
                                name = c.LabelCap.RawText,
                                description = ext.Description,
                                usage = $"!{c.commandText}",
                                shortcut = false,
                                userLevel = Enum.GetName(typeof(UserLevels), ext.UserLevel)
                            };

                            if (!ext.Parameters.NullOrEmpty())
                            {
                                dump.usage += " ";
                                dump.usage += string.Join(
                                    " ",
                                    ext.Parameters.Select(i => i.ToString().ToLowerInvariant()).ToArray()
                                );
                            }

                            return dump;
                        }
                    )
            );

            SaveData(JsonConvert.SerializeObject(container, Formatting.Indented), CommandsFile);
        }

        public static void ValidateExpansionData()
        {
            TkLogger.Info("Validating shop expansion data...");
            HashSet<TraitDef> loadedTraits = DefDatabase<TraitDef>.AllDefsListForReading.ToHashSet();
            HashSet<PawnKindDef> raceDefs = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(i => i.RaceProps.Humanlike)
                .ToHashSet();
            HashSet<string> loadedRaces = raceDefs
                .GroupBy(i => i.race.defName)
                .Select(i => i.Key)
                .ToHashSet();

            var removedTraits = 0;
            var removedRaces = 0;

            try
            {
                for (int i = TkUtils.ShopExpansion.Traits.Count - 1; i >= 0; i--)
                {
                    if (loadedTraits.Any(t => t.defName.Equals(TkUtils.ShopExpansion.Traits[i].DefName)))
                    {
                        continue;
                    }

                    TkUtils.ShopExpansion.Traits.RemoveAt(i);
                    removedTraits += 1;
                }

                if (removedTraits > 0)
                {
                    TkLogger.Info($"Removed {removedTraits} traits from the shop.");
                }
            }
            catch (Exception e)
            {
                TkLogger.Error("Could not validate trait data!", e);
            }

            List<TraitDef> missingTraits = loadedTraits
                .Where(t => !TkUtils.ShopExpansion.Traits.Any(p => t.defName.EqualsIgnoreCase(p.DefName)))
                .ToList();

            foreach (TraitDef trait in missingTraits)
            {
                foreach (XmlTrait t in TraitHelper.GetEffectiveTraits(trait))
                {
                    t.Name = Unrichify.StripTags(t.Name);
                    t.BypassLimit = TraitHelper.IsSexualityTrait(trait);

                    TkUtils.ShopExpansion.Traits.Add(t);
                }
            }

            try
            {
                for (int i = TkUtils.ShopExpansion.Races.Count - 1; i >= 0; i--)
                {
                    if (loadedRaces.Any(r => r.Equals(TkUtils.ShopExpansion.Races[i].DefName)))
                    {
                        continue;
                    }

                    TkUtils.ShopExpansion.Races.RemoveAt(i);
                    removedRaces += 1;
                }

                if (removedRaces > 0)
                {
                    TkLogger.Info($"Removed {removedRaces} races from the shop.");
                }
            }
            catch (Exception e)
            {
                TkLogger.Error("Could not validate race data!", e);
            }

            List<string> missingRaces = loadedRaces
                .Where(t => !TkUtils.ShopExpansion.Races.Any(p => t.EqualsIgnoreCase(p.DefName)))
                .ToList();

            foreach (string race in missingRaces)
            {
                PawnKindDef raceDef = raceDefs.FirstOrDefault(r => r.race.defName.Equals(race));
                string raceName = raceDef?.race.label ?? race;
                int price = raceDef != null ? StoreDialog.CalculateToolkitPrice(raceDef.race.BaseMarketValue) : 3500;

                TkUtils.ShopExpansion.Races.Add(
                    new XmlRace {DefName = race, Name = raceName, Price = price, Enabled = true}
                );
            }

            if (removedRaces <= 0 && removedTraits <= 0 && missingRaces.Count <= 0 && missingTraits.Count <= 0)
            {
                return;
            }

            TkLogger.Info("Trait/Race data changed between instances; saving new data...");
            SaveData(TkUtils.ShopExpansion, ExpansionFile);

            if (TkSettings.JsonShop)
            {
                DumpShopExpansion();
            }
        }

        internal static void TryMigrateData()
        {
            var traitCount = 0;
            foreach (XmlTrait trait in TkUtils.ShopExpansion.Traits.Where(trait => trait.Name.Contains('<')))
            {
                trait.Name = Unrichify.StripTags(trait.Name);
                traitCount += 1;
            }

            if (traitCount > 0)
            {
                TkLogger.Info($"Cleaned up {traitCount} traits with lingering tags.");
            }

            var raceCount = 0;
            HashSet<ThingDef> races = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(r => r.RaceProps.Humanlike)
                .Select(r => r.race)
                .ToHashSet();
            foreach (XmlRace race in TkUtils.ShopExpansion.Races)
            {
                string name = races.FirstOrDefault(r => r.defName.EqualsIgnoreCase(race.DefName))?.label
                              ?? race.DefName;

                if (race.Name.Equals(name))
                {
                    continue;
                }

                {
                    race.Name = races.FirstOrDefault(r => r.defName.Equals(race.DefName))?.label ?? race.DefName;
                    raceCount += 1;
                }
            }

            if (raceCount > 0)
            {
                TkLogger.Info($"Cleaned up {raceCount} races with wrong names.");
            }

            string oldMods = Path.Combine(SaveHelper.dataPath, "Mods.json");
            string oldModsBack = Path.Combine(SaveHelper.dataPath, "Mods.json.bak");

            if (File.Exists(oldMods))
            {
                File.Delete(oldMods);
            }

            if (File.Exists(oldModsBack))
            {
                File.Delete(oldModsBack);
            }
        }

        internal static void TrySalvageData()
        {
            TkLogger.Info("Attempting to salvage shop data...");

            var buffer = new StringBuilder();
            IEnumerable<string> lines = File.ReadLines(ExpansionFile, Encoding.UTF8);

            foreach (string line in lines)
            {
                if (!line.StartsWith("</ShopExpansion>"))
                {
                    buffer.Append(line);
                }
                else
                {
                    buffer.Append("</ShopExpansion>");
                    break;
                }
            }

            var reader = new StringReader(buffer.ToString());
            var serializer = new XmlSerializer(typeof(XmlShop));
            TkUtils.ShopExpansion = (XmlShop) serializer.Deserialize(reader);

            TkLogger.Info("Salvaged?");
            SaveData(TkUtils.ShopExpansion, ExpansionFile);
            reader.Dispose();
        }
    }

    [XmlRoot("ShopExpansion", IsNullable = false, Namespace = null)]
    public class XmlShop
    {
        public List<XmlRace> Races;
        public List<XmlTrait> Traits;
    }

    public class XmlRace
    {
        [XmlAttribute]
        public string DefName;

        public bool Enabled;
        public string Name;
        public int Price;
    }

    public class XmlTrait
    {
        public int AddPrice;
        public bool BypassLimit;
        public bool CanAdd;
        public bool CanRemove;
        public string DefName;
        public int Degree;
        public string Name;
        public int RemovePrice;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class TraitDump
    {
        public int addPrice { get; set; }
        public bool bypassLimit { get; set; }
        public bool canAdd { get; set; }
        public bool canRemove { get; set; }
        public string[] conflicts { get; set; }
        public string defName { get; set; }
        public int degree { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public int removePrice { get; set; }
        public string[] stats { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class RaceDump
    {
        public string defName { get; set; }
        public bool enabled { get; set; }
        public string name { get; set; }
        public int price { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ModDump
    {
        public string author { get; set; }
        public string name { get; set; }
        public string steamId { get; set; }
        public string version { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class CommandDump
    {
        public string description { get; set; }
        public string name { get; set; }
        public bool shortcut { get; set; }
        public string usage { get; set; }
        public string userLevel { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ShopDump
    {
        public List<TraitDump> traits { get; set; }
        public List<RaceDump> races { get; set; }
    }
}
