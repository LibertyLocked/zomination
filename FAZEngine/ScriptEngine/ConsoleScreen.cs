// ScriptConsole for FAZEngine
// This class can only be used on Windows!
#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

namespace FAZEngine.ScriptEngine
{
    public class ConsoleScreen : GameScreen
    {
        public GameScreen HookedScreen;
        public Game HookedGame;

        WinInputHook winInputHook;
        KeyboardState kbs, lastKbs;
        int timeSinceLastOn = 0, timeToTurnOn = 500;
        bool isCursorOn = false;
        string msgIn = "", msgOut = "Script Engine Console for FAZEngine\nCopyright 2013-2014 3LT Games. All rights reserved.\n\n";

        const int MAX_OUTPUT_LINES = 20;
        const float FONT_SCALE = 0.6f;

        public ConsoleScreen(GameScreen hookedScreen)
        {
            HookedScreen = hookedScreen;
            HookedGame = hookedScreen.ScreenManager.Game;
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                AddOutput(DateTime.Now.ToString() + ": Activated from an instance of " + HookedScreen.GetType().ToString());
                winInputHook = new WinInputHook(HookedGame.Window.Handle);
                winInputHook.KeyPress += HandleWinInput;
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Flash the cursor
            timeSinceLastOn += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastOn >= timeToTurnOn)
            {
                timeSinceLastOn -= timeToTurnOn;
                isCursorOn = !isCursorOn;
            }

            // Check output string
            if (msgOut.Length > 0)
            {
                if (msgOut.Contains("\n") && msgOut.Split('\n').Length > MAX_OUTPUT_LINES) DeleteOutputLines(1);
            }

            // Handle keyboard
            lastKbs = kbs;
            kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Escape) && lastKbs.IsKeyUp(Keys.Escape))
                this.ExitScreen();
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = HookedScreen.ScreenManager.SpriteBatch;
            spriteBatch.Begin();
            spriteBatch.DrawString(HookedScreen.ScreenManager.Font, msgOut + ">" + msgIn + (isCursorOn ? "_" : ""), 
                Vector2.Zero, Color.White, 0, Vector2.Zero, FONT_SCALE, SpriteEffects.None, 1f);
            spriteBatch.End();
        }

        public override void Unload()
        {
            winInputHook.Dispose();
        }

        private void HandleWinInput(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == '\b')  // backspace
            {
                if (msgIn.Length > 0) msgIn = msgIn.Substring(0, msgIn.Length - 1);
            }
            else if (e.KeyChar == '\r') // enter
            {
                EvalInput(msgIn);
                msgIn = "";
            }
            else if (e.KeyChar == '`')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == 27)   // ESC. don't handle here. handle in Update
            {
                e.Handled = true;
            }
            else if (HookedScreen.ScreenManager.Font.Characters.Contains(e.KeyChar))
                msgIn += e.KeyChar.ToString();
        }

        void EvalInput(string msgIn)
        {
            if (msgIn.Contains("echo"))
            {
                if (msgIn.Split(' ').Length > 1)
                    AddOutput(msgIn.Substring(5, msgIn.Length - 5));
                else
                    AddOutput("invalid usage of echo");
            }
            else if (msgIn == "clear") msgOut = "";
            else if (msgIn == "hide") this.ExitScreen();
            else
                AddOutput("unknown command: " + msgIn);
        }

        void AddOutput(string add)
        {
            msgOut += add + System.Environment.NewLine;
        }

        void DeleteOutputLines(int linesToRemove)
        {
            msgOut = msgOut.Split(System.Environment.NewLine.ToCharArray(),
                           linesToRemove + 1,
                           StringSplitOptions.RemoveEmptyEntries).Skip(linesToRemove).FirstOrDefault();
        }
    }
}
#endif