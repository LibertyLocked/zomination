using System;
using Microsoft.Xna.Framework;
using GameStateManagement;

namespace FAZEngine.ScriptEngine
{
    public class Script
    {
        public string FileName { get; set; }
        public GameScreen GameScreen { get; set; }
        public Type[] SupportedScreenTypes;

        public virtual void Init() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }

        public virtual void Unload() { }
    }
}