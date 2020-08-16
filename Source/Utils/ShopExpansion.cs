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
            if (TkSettings.Offload)
            {
                Task.Run(() => Data.SaveJson(Data.Mods, Paths.ModListFilePath));
            }
            else
            {
                Data.SaveJson(Data.Mods, Paths.ModListFilePath);
            }
        }

        public static void DumpCommands()
        {
            List<CommandItem> container = Verse.DefDatabase<Command>.AllDefs.Where(c => c.enabled)
               .Select(CommandItem.FromToolkit)
               .ToList();

            container.AddRange(
                DefDatabase<ToolkitChatCommand>.AllDefsListForReading.Where(c => c.enabled)
                   .Select(CommandItem.FromToolkitCore)
            );

            if (TkSettings.Offload)
            {
                Task.Run(() => Data.SaveJson(container, Paths.CommandListFilePath));
            }
            else
            {
                Data.SaveJson(container, Paths.CommandListFilePath);
            }
        }
    }
}
