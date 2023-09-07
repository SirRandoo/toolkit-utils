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
using System.Threading.Tasks;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Models;
using Verse;
using Command = TwitchToolkit.Command;

namespace SirRandoo.ToolkitUtils;

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
        SaveJson(Commands, Paths.CommandListFilePath);
    }

    /// <summary>
    ///     Saves all commands indexed by the mod to its associated file.
    /// </summary>
    public static async Task SaveCommandsAsync()
    {
        await SaveJsonAsync(Commands, Paths.CommandListFilePath);
    }

    public static void LoadCommands(string filePath, bool ignoreErrors = false)
    {
        Commands = LoadJson<List<CommandItem>>(filePath, ignoreErrors) ?? new List<CommandItem>();
    }

    public static void ValidateCommands()
    {
        RemoveInvalidCommands();
        TranscribeFromCore();
        TranscribeFromToolkit();
    }

    private static void TranscribeFromCore()
    {
        foreach (ToolkitChatCommand command in DefDatabase<ToolkitChatCommand>.AllDefs)
        {
            CommandItem extracted = CommandItem.FromToolkitCore(command);
            CommandItem existing = Commands.Find(c => string.Equals(c.DefName, extracted.DefName) && string.Equals(c.Data!.Mod, extracted.Data!.Mod));

            if (existing != null)
            {
                Commands.Remove(existing);
            }

            Commands.Add(CommandItem.FromToolkitCore(command));
        }
    }

    private static void TranscribeFromToolkit()
    {
        foreach (Command command in DefDatabase<Command>.AllDefs)
        {
            CommandItem extracted = CommandItem.FromToolkit(command);
            CommandItem item = Commands.Find(c => string.Equals(c.DefName, command.defName) && string.Equals(c.Data!.Mod, extracted.Data!.Mod));

            if (item == null)
            {
                Commands.Add(extracted);

                continue;
            }

            item.Description = extracted.Description;
            item.Name = extracted.Name;
            item.Usage = extracted.Usage;
            item.UserLevel = extracted.UserLevel;

            item.Data!.Mod = extracted.Data!.Mod;
            item.Data.IsBalance = extracted.Data.IsBalance;
            item.Data.IsBuy = extracted.Data.IsBuy;
            item.Data.IsShortcut = extracted.Data.IsShortcut;
        }
    }

    private static void RemoveInvalidCommands()
    {
        for (int i = Commands.Count - 1; i >= 0; i--)
        {
            CommandItem command = Commands[i];

            if (!string.IsNullOrEmpty(command.DefName) && DefDatabase<Command>.GetNamedSilentFail(command.DefName) == null)
            {
                Commands.RemoveAt(i);
            }
        }
    }
}