// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Models;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils
{
    public static partial class Data
    {
        /// <summary>
        ///     The various commands loaded within the game, as well as their
        ///     accompanying data.
        /// </summary>
        public static List<CommandItem> Commands { get; private set; }

        /// <summary>
        ///     Saves all commands indexed by the mod to its associated file.
        /// </summary>
        public static void DumpCommands()
        {
            List<CommandItem> container = Verse.DefDatabase<Command>.AllDefs.Where(c => c.enabled && !string.IsNullOrEmpty(c.command))
               .Select(CommandItem.FromToolkit)
               .ToList();

            container.AddRange(DefDatabase<ToolkitChatCommand>.AllDefsListForReading.Where(c => c.enabled).Select(CommandItem.FromToolkitCore));
            Commands = container;

            SaveJson(container, Paths.CommandListFilePath);
        }
        
        /// <summary>
        ///     Saves all commands indexed by the mod to its associated file.
        /// </summary>
        public static async Task DumpCommandsAsync()
        {
            List<CommandItem> container = DefDatabase<Command>.AllDefs.Where(c => c.enabled).Where(c => !c.command.NullOrEmpty()).Select(CommandItem.FromToolkit).ToList();

            container.AddRange(DefDatabase<ToolkitChatCommand>.AllDefs.Where(c => c.enabled).Select(CommandItem.FromToolkitCore));
            Commands = container;

            await SaveJsonAsync(container, Paths.CommandListFilePath);
        }
    }
}
