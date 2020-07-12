using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Models;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils.Utils
{
    public static class ShopExpansion
    {
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

        public static void DumpModList()
        {
            Task.Run(() => ShopInventory.SaveJson(TkUtils.ModListCache, Paths.ModListFilePath));
        }

        public static void DumpCommands()
        {
            List<Command> commands = DefDatabase<Command>.AllDefsListForReading;
            List<CommandItem> container = commands
                .Where(c => c.enabled && c.HasModExtension<CommandExtension>())
                .Select(
                    c =>
                    {
                        var ext = c.GetModExtension<CommandExtension>();

                        var dump = new CommandItem
                        {
                            Name = c.LabelCap.RawText,
                            Description = ext.Description,
                            Usage = $"!{c.command}",
                            UserLevel = ext.UserLevel,
                            Data = new CommandData
                            {
                                IsShortcut = c.commandDriver.Name.Equals("Buy") && !c.defName.Equals("Buy")
                            }
                        };

                        if (!ext.Parameters.NullOrEmpty())
                        {
                            dump.Usage += " ";
                            dump.Usage += string.Join(
                                " ",
                                ext.Parameters.Select(i => i.ToString().ToLowerInvariant()).ToArray()
                            );
                        }

                        if (c.requiresAdmin || c.requiresMod)
                        {
                            dump.UserLevel = UserLevels.Moderator;
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

                            var dump = new CommandItem
                            {
                                Name = c.LabelCap.RawText,
                                Description = ext.Description,
                                Usage = $"!{c.commandText}",
                                UserLevel = ext.UserLevel,
                                Data = new CommandData {IsShortcut = false}
                            };

                            if (!ext.Parameters.NullOrEmpty())
                            {
                                dump.Usage += " ";
                                dump.Usage += string.Join(
                                    " ",
                                    ext.Parameters.Select(i => i.ToString().ToLowerInvariant()).ToArray()
                                );
                            }

                            return dump;
                        }
                    )
            );

            Task.Run(() => ShopInventory.SaveJson(container, Paths.CommandListFilePath));
        }
    }
}
