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
using System.Threading.Tasks;
using JetBrains.Annotations;
using ToolkitCore;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    /// <summary>
    ///     A <see cref="GameComponent"/> responsible for scheduling commands
    ///     out of the Twitch thread, as well as executing any actions that
    ///     may need to be run on the Unity thread.
    /// </summary>
    [UsedImplicitly]
    public class CommandRouter : GameComponent
    {
        private static Task _interfaceTask;
        internal static readonly ConcurrentQueue<ITwitchMessage> CommandQueue = new ConcurrentQueue<ITwitchMessage>();
        internal static readonly ConcurrentQueue<Action> MainThreadCommands = new ConcurrentQueue<Action>();

        public CommandRouter(Game game)
        {
        }

        public override void LoadedGame()
        {
            CommandQueue.Clear();
        }

        public override void GameComponentUpdate()
        {
            ProcessCommands();

            if (!TkSettings.CommandRouter || _interfaceTask is { IsCompleted: false })
            {
                return;
            }

            if (_interfaceTask?.Exception != null)
            {
                foreach (Exception exception in _interfaceTask.Exception.Flatten().InnerExceptions)
                {
                    TkUtils.HandleException("A message handler encountered an exception", exception, "ToolkitUtils - Command Router");
                }

                _interfaceTask = null;
            }

            ProcessCommandQueue();
        }

        private static void ProcessCommandQueue()
        {
            List<TwitchInterfaceBase> interfaces = null;
            bool taskDone = _interfaceTask == null || _interfaceTask.IsCompleted;

            while (taskDone && !CommandQueue.IsEmpty)
            {
                if (!CommandQueue.TryDequeue(out ITwitchMessage message))
                {
                    break;
                }

                interfaces ??= Current.Game.components.OfType<TwitchInterfaceBase>().ToList();

                foreach (TwitchInterfaceBase @interface in interfaces)
                {
                    if (_interfaceTask == null)
                    {
                        _interfaceTask = Task.Run(
                            () =>
                            {
                                @interface.ParseMessage(message);
                            }
                        );
                    }
                    else
                    {
                        _interfaceTask.ContinueWith(
                            t =>
                            {
                                @interface.ParseMessage(message);
                            }
                        );
                    }
                }
            }
        }

        private static void ProcessCommands()
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
        }
    }
}
