using System;
using System.Reflection;
using System.IO;
using GameStateManagement;

namespace FAZEngine.ScriptEngine
{
    public class ScriptLoader
    {
        /// <summary>
        /// Gets the main script in an external assembly.
        /// </summary>
        /// <param name="fileName">Filename of the assembly</param>
        /// <returns>External script</returns>
        private static Script GetScriptFromAssembly(string fileName)
        {
            Assembly asm = Assembly.LoadFrom(fileName);
            Type type = asm.GetType("ModScript.ScriptMain");
            Script script = Activator.CreateInstance(type) as Script;
            return script;
        }

        // this is not a permanent solution!
        public static void LoadModScripts(GameScreen gameScreen, string dir)
        {
            foreach (string filename in Directory.GetFiles(dir))
            {
                if (filename.EndsWith(".fazemod"))
                {
                    bool add = false;   // add this script to screen or not?
                    Script scriptToAdd = GetScriptFromAssembly(filename);
                    // we have got the script and it's initialized, but is it the one we want?
                    if (scriptToAdd.SupportedScreenTypes != null)
                    {
                        foreach (Type type in scriptToAdd.SupportedScreenTypes)
                            if (type == gameScreen.GetType()) add = true;
                    }
                    else
                        add = true;
                    if (add)
                    {
                        scriptToAdd.GameScreen = gameScreen;
                        scriptToAdd.FileName = Path.GetFileName(filename);
                        gameScreen.Scripts.Add(scriptToAdd);
                        scriptToAdd.Init(); // dont forget to init them
                    }
                }
            }
        }

        public static void LoadModScripts(GameScreen gameScreen)
        {
            LoadModScripts(gameScreen, Directory.GetCurrentDirectory());
        }

    } // End of class
}
