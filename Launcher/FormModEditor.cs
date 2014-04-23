using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Microsoft.CSharp;
using System.IO;

namespace Launcher
{
    public partial class FormModEditor : Form
    {
        string fileName = "";
        string references = "";

        public FormModEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initial loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormModEditor_Load(object sender, EventArgs e)
        {
            openModDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            saveModDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            //textBox1.Text = SampleStarterCode;
            try
            {
                using (StreamReader sr = new StreamReader(@".\configs\editor_references.txt"))
                {
                    String line = sr.ReadToEnd();
                    references = line;
                }
            }
            catch { }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                if (saveModDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    fileName = saveModDialog.FileName;
                else
                    return;
            }
            System.IO.File.WriteAllText(fileName, textBox1.Text);
            MessageBox.Show("Saved to " + fileName);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveModDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                fileName = saveModDialog.FileName;
            else
                return;
            System.IO.File.WriteAllText(fileName, textBox1.Text);
            MessageBox.Show("Saved to " + fileName);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openModDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                fileName = openModDialog.FileName;
            else
                return;

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    String line = sr.ReadToEnd();
                    textBox1.Text = line;
                }
            }
            catch
            {
                MessageBox.Show("Failed to open " + fileName);
            }
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Save the script before compiling!");
                saveToolStripMenuItem_Click(null, null);
                if (string.IsNullOrEmpty(fileName)) return;
            }
            else
            {
                System.IO.File.WriteAllText(fileName, textBox1.Text);
            }

            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
            string Output = fileName.Substring(0, fileName.Length - Path.GetExtension(fileName).Length) + ".fazemod";

            textBox2.Text = "";
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.OutputAssembly = Output;
            // Try loading commonly used assemblies
            try
            {
                parameters.ReferencedAssemblies.Add("zomination.exe");
                parameters.ReferencedAssemblies.Add("FAZEngine_dx10.dll");
                parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Vector2)).Location);
                parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Game)).Location);
                parameters.ReferencedAssemblies.Add(Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Graphics.GraphicsDevice)).Location);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to locate a required assembly.\n" + ex.Message, "Compilation failed.");
                return;
            }
            // Adding additional references
            using (StringReader reader = new StringReader(references))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    parameters.ReferencedAssemblies.Add(line);
                }
            }
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, textBox1.Text);

            if (results.Errors.Count > 0)
            {
                textBox2.ForeColor = System.Drawing.Color.Red;
                foreach (CompilerError CompErr in results.Errors)
                {
                    textBox2.Text = textBox2.Text +
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine;
                }
            }
            else
            {
                //Successful Compile
                textBox2.ForeColor = System.Drawing.Color.Blue;
                textBox2.Text = "Build succeeded.";
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string itemText = ((ToolStripMenuItem)sender).Text;
            if (itemText == "MoneyHack")
            {
                textBox1.Text = SampleMoneyHackCode;
            }
            else if (itemText == "SlowMo")
            {
                textBox1.Text = SampleSlowMoCode;
            }
            else if (itemText == "ConsoleHook")
            {
                textBox1.Text = ConsoleHookCode;
            }
            else
                textBox1.Text = SampleStarterCode;
            ResetFileName();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Font = fontDialog1.Font;
            }
        }

        private void referencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            references = Prompt.ShowDialog("Add or remove additional references. One per line.", references, "Script References");
        }

        private void objectBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("// TODO: object browser window here");
        }

        private void FormModEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ask if want to save
            DialogResult dialogResult = MessageBox.Show("Do you want to save your script?", "Exit confirmation", MessageBoxButtons.YesNoCancel);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                // Save the code
                if (string.IsNullOrEmpty(fileName))
                {
                    if (saveModDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        fileName = saveModDialog.FileName;
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                System.IO.File.WriteAllText(fileName, textBox1.Text);
                MessageBox.Show("Saved to " + fileName);
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Script Editor for FAZEngine\n\nEarly alpha build",
                "About Script Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateCurrentLineNumber();
        }

        private void textBox1_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateCurrentLineNumber();
        }

        private void ResetFileName()
        {
            fileName = "";
            saveModDialog.FileName = "";
            openModDialog.FileName = "";
        }

        private void UpdateCurrentLineNumber()
        {
            int index = this.textBox1.SelectionStart;
            int line = this.textBox1.GetLineFromCharIndex(index) + 1;

            lineNumLabel.Text = " Ln " + line.ToString("N0");
        }

        const string SampleStarterCode = @"using System;
using FAZEngine;
using FAZEngine.ScriptEngine;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using zomination;

namespace ModScript
{
    public class ScriptMain : Script
    {
        // TODO: Change this to the screen you want your script to run on.
        SurvivalGameplayScreen thisScreen;

        public ScriptMain()
        {
            // TODO: Change this to an array of your supported screens
            SupportedScreenTypes = new Type[] { typeof(SurvivalGameplayScreen) };
        }

        public override void Init()
        {
            // TODO: Change this to the screen you want your script to run on.
            thisScreen = GameScreen as zomination.SurvivalGameplayScreen;
            
            // TODO: Initialize variables or load contents here
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add update logic for your script
        }

        public override void Draw(GameTime gameTime)
        {
            // TODO: Add draw logic for your script
        }

        public override void Unload()
        {
            // TODO: Unload contents and dispose variables if necessary
        }

    }
}
";

        const string SampleMoneyHackCode = @"using System;
using FAZEngine;
using FAZEngine.ScriptEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using zomination;

namespace ModScript
{
    public class ScriptMain : Script
    {
        // This is the screen we want our script to run on
        SurvivalGameplayScreen myScreen;

        // Constructor
        public ScriptMain()
        {
            // We only want this script to run on SurvivalGameplayScreen
            // So that it won't be loaded into other screens
            // You can assign null if you want your script to run on any screen
            SupportedScreenTypes = new Type[] { typeof(SurvivalGameplayScreen) };
        }

        // Initialize script
        public override void Init()
        {
            myScreen = GameScreen as SurvivalGameplayScreen;
        }

        // Update script
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) // When NumPad0 is pressed
                myScreen.cowboy.AddMoney(2); // Add $2
        }
    }
}
";

        const string SampleSlowMoCode = @"using System;
using FAZEngine;
using FAZEngine.ScriptEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using zomination;

namespace ModScript
{
    public class ScriptMain : Script
    {
        // This is the screen we want our script to run on
        SurvivalGameplayScreen myScreen;

        // Constructor
        public ScriptMain()
        {
            // We only want this script to run on SurvivalGameplayScreen
            // So that it won't be loaded into other screens
            // You can assign null if you want your script to run on any screen
            SupportedScreenTypes = new Type[] { typeof(SurvivalGameplayScreen) };
        }

        // Initialize script
        public override void Init()
        {
            myScreen = GameScreen as SurvivalGameplayScreen;
        }

        // Update script
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) // When NumPad1 is pressed
                myScreen.world.SlowDown(10000, 0.2f); // Slow the world down to 20% for 10 seconds
        }
    }
}
";
        const string ConsoleHookCode = @"using System;
using FAZEngine;
using FAZEngine.ScriptEngine;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using zomination;

namespace ModScript
{
    public class ScriptMain : Script
    {
        FAZEngine.ScriptEngine.ConsoleScreen consoleScreen;

        public ScriptMain()
        {
        }

        public override void Init()
        {
            consoleScreen = new FAZEngine.ScriptEngine.ConsoleScreen(GameScreen);
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde))
                GameScreen.ScreenManager.AddScreen(consoleScreen, null);
        }

        public override void Unload()
        {
            if (consoleScreen != null) consoleScreen.Unload();
        }

    }
}";

    }




    public static class Prompt
    {
        public static string ShowDialog(string text, string textBoxText, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 450;
            prompt.Text = caption;
            prompt.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Label textLabel = new Label() { AutoSize = true, Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Text = textBoxText, Left = 40, Top= 50, Width = 400, Height = 300, ScrollBars = ScrollBars.Both, Multiline = true, TabIndex = 0 };
            Button confirmation = new Button() { Text = "OK", Left = 300, Width = 100, Height = 30, Top = 360, TabIndex = 2 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.ShowDialog();
            // Also save it to a file
            System.IO.File.WriteAllText(@".\configs\editor_references.txt", textBox.Text);
            return textBox.Text;
        }
    }
}
