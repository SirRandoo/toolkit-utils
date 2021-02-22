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

using MoonSharp.Interpreter;
using SirRandoo.ToolkitUtils.Helpers;

namespace SirRandoo.ToolkitUtils.Models
{
    public class ScriptEngine
    {
        private const CoreModules DefaultEnvironment = CoreModules.Basic
                                                       | CoreModules.Table
                                                       | CoreModules.TableIterators
                                                       | CoreModules.String
                                                       | CoreModules.Math
                                                       | CoreModules.Bit32;

        private readonly string id;

        private readonly Script script;

        private ScriptEngine(string id)
        {
            script = new Script(DefaultEnvironment) {Options = {DebugPrint = PrintPassthrough}};
            this.id = id;
        }

        private void PrintPassthrough(string message)
        {
            LogHelper.Info($"[ScriptEngine] {id} :: {message}");
        }

        public static ScriptEngine CreateInstance(string id)
        {
            return new ScriptEngine(id);
        }

        public DynValue Invoke(string code, Table global = null)
        {
            try
            {
                return script.DoString(code, global, id);
            }
            catch (InterpreterException e)
            {
                LogHelper.Error(
                    $"Could not invoke script with code! Reason: {e.DecoratedMessage}\n\nPassed code:\n{code}",
                    e
                );
                return DynValue.Void;
            }
        }
    }
}
