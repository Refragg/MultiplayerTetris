using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MultiplayerTetris
{
    public class AudioManager : Microsoft.Xna.Framework.GameComponent
    {


        public AudioListener Listener
        {
            get { return listener; }
        }

        AudioListener listener = new AudioListener();


        // The emitter describes an entity which is making a 3D sound.
        Microsoft.Xna.Framework.Audio.AudioEmitter emitter = new Microsoft.Xna.Framework.Audio.AudioEmitter();


        // Store all the sound effects that are available to be played.
        Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();

        
        // Keep track of all the 3D sounds that are currently playing.
        List<ActiveSound> activeSounds = new List<ActiveSound>();



        public AudioManager(Game game)
            : base(game)
        { }



        public override void Initialize()
        {
            SoundEffect.DistanceScale = 2000;
            SoundEffect.DopplerScale = 0.1f;
            

            base.Initialize();
        }


        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    foreach (SoundEffect soundEffect in soundEffects.Values)
                    {
                        soundEffect.Dispose();
                    }

                    soundEffects.Clear();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        

        public override void Update(GameTime gameTime)
        {
            // Loop over all the currently playing 3D sounds.
            int index = 0;

            while (index < activeSounds.Count)
            {
                ActiveSound activeSound = activeSounds[index];

                if (activeSound.Instance.State == SoundState.Stopped)
                {
                    // If the sound has stopped playing, dispose it.
                    activeSound.Instance.Dispose();
                    activeSounds.RemoveAt(index);
                }
                else
                {
                    Apply3D(activeSound);
                    index++;
                }
            }

            base.Update(gameTime);
        }


        public SoundEffectInstance Play3DSound(SoundEffect se, bool isLooped, AudioEmitter emitter)
        {
            ActiveSound activeSound = new ActiveSound();
            
            activeSound.Instance = se.CreateInstance();
            activeSound.Instance.IsLooped = isLooped;

            activeSound.Emitter = emitter;
            Apply3D(activeSound);
            activeSound.Instance.Play();
            activeSounds.Add(activeSound);

            return activeSound.Instance;
        }


        private void Apply3D(ActiveSound activeSound)
        {
            emitter.Position = activeSound.Emitter.Position;
            emitter.Forward = activeSound.Emitter.Forward;
            emitter.Up = activeSound.Emitter.Up;
            emitter.Velocity = activeSound.Emitter.Velocity;

            activeSound.Instance.Apply3D(listener, emitter);
        }

        private class ActiveSound
        {
            public SoundEffectInstance Instance;
            public AudioEmitter Emitter;
        }
    }
}
