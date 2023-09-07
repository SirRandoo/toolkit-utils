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
using System.IO;
using TwitchToolkit.Utilities;

namespace SirRandoo.ToolkitUtils;

/// <summary>
///     A class containing the various file paths used throughout the mod
///     for storing and loading data.
/// </summary>
public static class Paths
{
    public static readonly string TraitFilePath = Path.Combine(SaveHelper.dataPath, "traits.json");
    public static readonly string ModListFilePath = Path.Combine(SaveHelper.dataPath, "modlist.json");
    public static readonly string ItemDataFilePath = Path.Combine(SaveHelper.dataPath, "itemdata.json");
    public static readonly string EventDataFilePath = Path.Combine(SaveHelper.dataPath, "eventdata.json");
    public static readonly string PawnKindFilePath = Path.Combine(SaveHelper.dataPath, "pawnkinds.json");
    public static readonly string CommandListFilePath = Path.Combine(SaveHelper.dataPath, "commands.json");
    public static readonly string LegacyShopDumpFilePath = Path.Combine(SaveHelper.dataPath, "ShopExt.json");
    public static readonly string ToolkitItemFilePath = Path.Combine(SaveHelper.dataPath, "StoreItems.json");
    public static readonly string EditorPath = Path.Combine(SaveHelper.dataPath, "Editor");
    public static readonly string PartialPath = Path.Combine(EditorPath, "Partials");

    static Paths()
    {
        if (Directory.Exists(PartialPath))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(PartialPath);
        }
        catch (IOException e)
        {
            TkUtils.Logger.Error($"Could not create partial directory @ {PartialPath}", e);
        }
        catch (UnauthorizedAccessException e)
        {
            TkUtils.Logger.Error($"Could not create partial directory @ {PartialPath} -- Insufficient permissions", e);
        }
    }
}
