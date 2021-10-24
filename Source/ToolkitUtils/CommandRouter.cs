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
        private static Task _interfaceTask;
        internal static readonly ConcurrentQueue<ITwitchMessage> CommandQueue = new ConcurrentQueue<ITwitchMessage>();
        internal static readonly ConcurrentQueue<Action> MainThreadCommands = new ConcurrentQueue<Action>();

        public CommandRouter(Game game) { }

        public override void LoadedGame()
        {
            CommandQueue.Clear();
        }

        public override void GameComponentUpdate()
        {
            ProcessCommands();

            if (_interfaceTask != null)
            {
                LogHelper.Info(_interfaceTask?.Status.ToStringSafe());
            }

            if (!TkSettings.CommandRouter || _interfaceTask is { IsCompleted: false })
            {
                return;
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
                        _interfaceTask ??= Task.Run(
                            () =>
                            {
                                @interface.ParseMessage(message);
                                LogHelper.Info($"Executing {@interface.GetType().FullDescription()} from a task");
                            }
                        );
                    }
                    else
                    {
                        _interfaceTask.ContinueWith(
                            t =>
                            {
                                @interface.ParseMessage(message);
                                LogHelper.Info($"Executing {@interface.GetType().FullDescription()} from a task");
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
