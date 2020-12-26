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
