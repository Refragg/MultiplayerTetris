using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace MultiplayerTetris
{
    public class Inputs
    {

        private Dictionary<Keys, bool> releasedBuffers;
        private Dictionary<Keys, bool> pressedBuffers;
        private Dictionary<Keys, int> timedBuffers;

        public Inputs()
        {
            releasedBuffers = new Dictionary<Keys,bool>();
            pressedBuffers = new Dictionary<Keys,bool>();
            timedBuffers = new Dictionary<Keys, int>();
        }

        public bool KeyReleased(Keys key)
        {
            bool buffer = false;

            try
            {
                buffer = releasedBuffers[key];
            }
            catch
            {
                releasedBuffers.Add(key,false);
            }
            
            
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(key))
            {
                releasedBuffers[key] = true;
            }
            else
            {
                if (buffer)
                {
                    releasedBuffers[key] = false;
                    return true;
                }
            }
            
            return false;
        }
        
        public bool KeyPressed(Keys key)
        {
            bool buffer = false;

            try
            {
                buffer = pressedBuffers[key];
            }
            catch
            {
                pressedBuffers.Add(key,false);
            }
            
            
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(key))
            {
                if (!pressedBuffers[key])
                {
                    pressedBuffers[key] = true;
                    return true;
                }
            }
            else
            {
                pressedBuffers[key] = false;
            }
            
            return false;
        }
        
        public bool TimedPress(Keys key, int rate)
        {
            int buffer = 0;

            try
            {
                buffer = timedBuffers[key];
            }
            catch
            {
                timedBuffers.Add(key,0);
            }

            if (buffer >= rate)
            {
                buffer = 0;
                timedBuffers[key] = 0;
            }
            
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(key))
            {
                timedBuffers[key]++;
                if (buffer == 0)
                {
                    return true;
                }
            }
            else
            {
                timedBuffers[key] = 0;
            }

            
            return false;
        }

        
    
    }
}