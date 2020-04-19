using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RimWorld;
using TwitchToolkit.Store;
using TwitchToolkit.Utilities;
using UnityEngine;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class ShopExpansionHelper
    {
        public static readonly string ExpansionFile = Path.Combine(SaveHelper.dataPath, "ShopExt_1.xml");

        public static readonly string ShopFile = Path.Combine(SaveHelper.dataPath, "ShopExt.json");
        public static readonly string CommandsFile = Path.Combine(SaveHelper.dataPath, "commands.json");
        public static readonly string ModsFile = Path.Combine(SaveHelper.dataPath, "modlist.json");

        public static void SaveData<T>(T xml, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (directory == null)
            {
                Logger.Warn($"File path @ {filePath} is invalid!");
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializer = new XmlSerializer(typeof(T));
            var tempFile = $"{filePath}.tmp";
            var backupFile = $"{filePath}.bak";

            try
            {
                if (File.Exists(filePath))
                {
                    using (var writer = File.Open(tempFile, FileMode.Truncate))
                    {
                        serializer.Serialize(writer, xml);
                    }

                    File.Replace(tempFile, filePath, backupFile);
                }
                else
                {
                    using (var writer = File.Open(filePath, FileMode.Truncate))
                    {
                        serializer.Serialize(writer, xml);
                    }
                }
            }
            catch (IOException e)
            {
                Logger.Error($"Could not save data to {filePath}", e);
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Error("File access denied", e);
            }
        }

        public static void SaveData(string data, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (directory == null)
            {
                Logger.Warn($"File path @ {filePath} is invalid!");
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempFile = $"{filePath}.tmp";
            var backupFile = $"{filePath}.bak";

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
                Logger.Error($"Could not save data to {filePath}", e);
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.Error("File access denied", e);
            }
        }

        public static T LoadData<T>(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directory))
            {
                throw new IOException($"Directory {directory} does not exist!");
            }

            var serializer = new XmlSerializer(typeof(T));

            using (var reader = File.OpenText(filePath))
            {
                return (T) serializer.Deserialize(reader);
            }
        }

        public static void DumpShopExpansion()
        {
            var traits = DefDatabase<TraitDef>.AllDefsListForReading;
            var container = new List<TraitDump>();

            foreach (var trait in TkUtils.ShopExpansion.Traits)
            {
                var t = new TraitDump
                {
                    addPrice = trait.AddPrice,
                    bypassLimit = trait.BypassLimit,
                    degree = trait.Degree,
                    canAdd = trait.CanAdd,
                    canRemove = trait.CanRemove,
                    name = trait.Name,
                    defName = trait.DefName,
                    removePrice = trait.RemovePrice
                };

                var def = traits.FirstOrDefault(i => i.defName.Equals(trait.DefName));

                if (def == null)
                {
                    continue;
                }

                var inst = new Trait(def, trait.Degree);

                t.conflicts = def.conflictingTraits.Select(i => i.label.NullOrEmpty() ? i.defName : i.label).ToArray();

                var statContainer = new List<string>();

                if (inst.CurrentData.statOffsets != null)
                {
                    foreach (var offset in inst.CurrentData.statOffsets)
                    {
                        statContainer.Add($"{offset.ValueToStringAsOffset} {offset.stat.LabelForFullStatListCap}");
                    }
                }

                if (inst.CurrentData.statFactors != null)
                {
                    foreach (var factor in inst.CurrentData.statFactors)
                    {
                        statContainer.Add($"{factor.ToStringAsFactor} {factor.stat.LabelForFullStatListCap}");
                    }
                }

                t.description = inst.CurrentData.description;
                t.stats = statContainer.ToArray();

                container.Add(t);
            }

            var jsonRaces = TkUtils.ShopExpansion.Races.Select(
                    r => new RaceDump {defName = r.DefName, enabled = r.Enabled, name = r.Name, price = r.Price}
                )
                .Select(JsonUtility.ToJson)
                .ToArray();

            var builder = new StringBuilder("{");

            builder.Append("\"traits\":[");
            builder.Append(string.Join(",", container.Select(JsonUtility.ToJson).ToArray()));
            builder.Append("],");
            builder.Append("\"races\":[");
            builder.Append(string.Join(",", jsonRaces.ToArray()));
            builder.Append("]}");

            SaveData(builder.ToString(), ShopFile);
        }

        public static void DumpModList()
        {
            var jsonMods = new List<ModDump>();
            var loaded = LoadedModManager.ModHandles.ToList();

            foreach (var mod in loaded)
            {
                var meta = ModLister.GetActiveModWithIdentifier(mod.Content.PackageId);
                var hook = meta.GetWorkshopItemHook();
                var steamIdInfo = hook.GetType().GetProperty("PublishedFileId");

                jsonMods.Add(
                    new ModDump
                    {
                        author = meta.Author,
                        name = hook.Name,
                        version = TkUtils.GetModListVersioned()
                                      .FirstOrDefault(i => i.Item1.Equals(mod.Content.Name))
                                      ?.Item2
                                  ?? "0.0.0",
                        steamId = meta.OnSteamWorkshop ? steamIdInfo?.GetValue(hook).ToString() : null
                    }
                );
            }

            var builder = new StringBuilder("[");
            builder.Append(string.Join(",", jsonMods.Select(JsonUtility.ToJson).ToArray()));
            builder.Append("]");

            SaveData(builder.ToString(), ModsFile);
        }

        public static void DumpCommands()
        {
            var commands = DefDatabase<Command>.AllDefsListForReading;
            var container = new List<CommandDump>();

            foreach (var command in commands)
            {
                if (!$"TKUtils.Commands.UserLevel.{command.defName}".CanTranslate() && command.enabled)
                {
                    continue;
                }

                container.Add(
                    new CommandDump
                    {
                        name = command.LabelCap.RawText,
                        description = $"TKUtils.Commands.Description.{command.defName}".Translate(),
                        usage =
                            TkSettings.Prefix
                            + $"TKUtils.Commands.Usage.{command.defName}".Translate(command.command),
                        shortcut = command.commandDriver.Name.Equals("Buy") && !command.defName.Equals("Buy"),
                        userLevel = $"TKUtils.Commands.UserLevel.{command.defName}".Translate()
                    }
                );
            }

            var builder = new StringBuilder();
            builder.Append("[");
            builder.Append(string.Join(",", container.Select(JsonUtility.ToJson).ToArray()));
            builder.Append("]");

            SaveData(builder.ToString(), CommandsFile);
        }

        public static void ValidateExpansionData()
        {
            Logger.Info("Validating shop expansion data...");
            var loadedTraits = DefDatabase<TraitDef>.AllDefsListForReading.ToHashSet();
            var raceDefs = DefDatabase<PawnKindDef>.AllDefsListForReading.Where(i => i.RaceProps.Humanlike).ToHashSet();
            var loadedRaces = raceDefs
                .GroupBy(i => i.race.defName)
                .Select(i => i.Key)
                .ToHashSet();
            var removedTraits = 0;
            var removedRaces = 0;

            try
            {
                for (var i = TkUtils.ShopExpansion.Traits.Count - 1; i >= 0; i--)
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
                    Logger.Info($"Removed {removedTraits} traits from the shop.");
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not validate trait data!", e);
            }

            var missingTraits = loadedTraits
                .Where(t => !TkUtils.ShopExpansion.Traits.Any(p => t.defName.EqualsIgnoreCase(p.DefName)))
                .ToList();

            foreach (var trait in missingTraits)
            {
                foreach (var t in TraitHelper.GetEffectiveTraits(trait))
                {
                    t.Name = Unrichify.StripTags(t.Name);
                    t.BypassLimit = TraitHelper.IsSexualityTrait(trait);

                    TkUtils.ShopExpansion.Traits.Add(t);
                }
            }

            try
            {
                for (var i = TkUtils.ShopExpansion.Races.Count - 1; i >= 0; i--)
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
                    Logger.Info($"Removed {removedRaces} races from the shop.");
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not validate race data!", e);
            }

            var missingRaces = loadedRaces
                .Where(t => !TkUtils.ShopExpansion.Races.Any(p => t.EqualsIgnoreCase(p.DefName)))
                .ToList();

            foreach (var race in missingRaces)
            {
                var raceName = raceDefs.FirstOrDefault(r => r.race.defName.Equals(race))?.label ?? race;
                var item = StoreInventory.items.FirstOrDefault(i => i.defname.Equals(race));
                var price = 2500;

                if (item != null && item.price >= 0)
                {
                    price = item.price;
                    item.price = -10;
                }

                TkUtils.ShopExpansion.Races.Add(
                    new XmlRace {DefName = race, Name = raceName, Price = price, Enabled = true}
                );
            }

            if (removedRaces <= 0 && removedTraits <= 0 && missingRaces.Count <= 0 && missingTraits.Count <= 0)
            {
                return;
            }

            Logger.Info("Trait/Race data changed between instances; saving new data...");
            SaveData(TkUtils.ShopExpansion, ExpansionFile);

            if (TkSettings.JsonShop)
            {
                DumpShopExpansion();
            }
        }

        internal static void TryMigrateData()
        {
            var traitCount = 0;
            foreach (var trait in TkUtils.ShopExpansion.Traits.Where(trait => trait.Name.Contains('<')))
            {
                trait.Name = trait.Name.StripTags();
                traitCount += 1;
            }

            if (traitCount > 0)
            {
                Logger.Info($"Cleaned up {traitCount} traits with lingering tags.");
            }

            var raceCount = 0;
            var races = DefDatabase<PawnKindDef>.AllDefsListForReading
                .Where(r => r.RaceProps.Humanlike)
                .Select(r => r.race)
                .ToHashSet();
            foreach (var race in TkUtils.ShopExpansion.Races.Where(r => r.DefName.Equals(r.Name)))
            {
                race.Name = races.FirstOrDefault(r => r.defName.Equals(race.DefName))?.label ?? race.DefName;
                raceCount += 1;
            }

            if (raceCount > 0)
            {
                Logger.Info($"Cleaned up {raceCount} races with wrong names.");
            }
        }

        internal static void TrySalvageData()
        {
            Logger.Info("Attempting to salvage shop data...");

            var buffer = new StringBuilder();
            var lines = File.ReadLines(ExpansionFile, Encoding.UTF8);

            foreach (var line in lines)
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

            Logger.Info("Salvaged?");
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

    [Serializable]
    public class TraitDump
    {
        public int addPrice;
        public bool bypassLimit;
        public bool canAdd;
        public bool canRemove;
        public string[] conflicts;
        public string defName;
        public int degree;
        public string description;
        public string name;
        public int removePrice;
        public string[] stats;
    }

    [Serializable]
    public class RaceDump
    {
        public string defName;
        public bool enabled;
        public string name;
        public int price;
    }

    [Serializable]
    public class ModDump
    {
        public string author;
        public string name;
        public string steamId;
        public string version;
    }

    [Serializable]
    public class CommandDump
    {
        public string description;
        public string name;
        public bool shortcut;
        public string usage;
        public string userLevel;
    }
}
