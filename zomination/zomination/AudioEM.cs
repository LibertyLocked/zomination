using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace zomination
{
    public class AudioEM
    {
        ContentManager content;

        AudioEngine audioEngine;
        SoundBank musicSounds;
        WaveBank musicWaves;

        public AudioEngine AudioEngine
        {
            get { return audioEngine; }
        }

        public AudioEM(Microsoft.Xna.Framework.Game game)
        {
            content = new ContentManager(game.Services, "main");

            audioEngine = new AudioEngine(@"main\audio\soundEngine.xgs");
            musicWaves = new WaveBank(audioEngine, @"main\audio\musicWaves.xwb");
            musicSounds = new SoundBank(audioEngine, @"main\audio\musicSounds.xsb");
        }

        public Cue GetCue(string cueName, float volumeOffset)
        {
            Cue cue = musicSounds.GetCue(cueName);
            return cue;
        }

        public Cue GetCue(string cueName)
        {
            return musicSounds.GetCue(cueName);
        }

        public void Update()
        {
            audioEngine.Update();
        }

        public void Unload()
        {
            content.Unload();
        }
    }
}
