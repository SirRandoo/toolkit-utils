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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using ToolkitCore;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class CommandRouter : GameComponent
    {
        private static bool _isBusy;
        internal static readonly ConcurrentQueue<ITwitchMessage> CommandQueue = new ConcurrentQueue<ITwitchMessage>();
        internal static readonly ConcurrentQueue<Action> MainThreadCommands = new ConcurrentQueue<Action>();

        public CommandRouter(Game game) { }

        public override void LoadedGame()
        {
            CommandQueue.Clear();
        }

        public override void GameComponentUpdate()
        {
            while (!MainThreadCommands.IsEmpty)
            {
                if (!MainThreadCommands.TryDequeue(out Action action))
                {
                    break;
                }

                try
                {
                    action();
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (!TkSettings.CommandRouter || _isBusy)
            {
                return;
            }

            List<TwitchInterfaceBase> interfaces = null;
            while (!_isBusy && !CommandQueue.IsEmpty)
            {
                if (_isBusy || !CommandQueue.TryDequeue(out ITwitchMessage message))
                {
                    break;
                }

                interfaces ??= Current.Game.components.OfType<TwitchInterfaceBase>().ToList();

                Task.Run(
                    () =>
                    {
                        _isBusy = true;
                        var builder = new StringBuilder();
                        foreach (TwitchInterfaceBase @interface in interfaces)
                        {
                            try
                            {
                                @interface.ParseMessage(message);
                            }
                            catch (Exception e)
                            {
                                builder.Append($" - {@interface.GetType().FullDescription()}  |  {e.GetType().Name}({e.Message})\n");
                            }
                        }

                        if (builder.Length > 0)
                        {
                            LogHelper.Warn(builder.ToString());
                        }

                        _isBusy = false;
                    }
                );
            }
        }
    }
}
