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
        private KeyboardState _state;

        private Dictionary<Keys, bool> releasedBuffers;
        private Dictionary<Keys, bool> pressedBuffers;
        private Dictionary<Keys, int> timedBuffers;

        public Inputs()
        {
            releasedBuffers = new Dictionary<Keys,bool>();
            pressedBuffers = new Dictionary<Keys,bool>();
            timedBuffers = new Dictionary<Keys, int>();

            foreach (Keys currentKey in typeof(Keys).GetEnumValues())
            {
                releasedBuffers.Add(currentKey, false);
                pressedBuffers.Add(currentKey, false);
                timedBuffers.Add(currentKey, 0);
            }
            
            UpdateState();
        }

        public void UpdateState()
        {
            _state = Keyboard.GetState();
        }

        public bool KeyReleased(Keys key)
        {
            bool buffer = releasedBuffers[key];
            
            if (_state.IsKeyDown(key))
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
            bool buffer = pressedBuffers[key];

            if (_state.IsKeyDown(key))
            {
                if (!buffer)
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
        
        public bool TimedPress(Keys key, int rate, int wait)
        {
            int buffer = 0;

            buffer = timedBuffers[key];

            if (buffer >= rate)
            {
                buffer = 0;
                timedBuffers[key] = 0;
            }

            if (_state.IsKeyDown(key))
            {
                if (timedBuffers[key] == -1 * wait)
                {
                    timedBuffers[key]++;
                    return true;
                }
                
                timedBuffers[key]++;
                if (buffer == 0 && timedBuffers[key]>=0)
                {
                    return true;
                }
            }
            else
            {
                timedBuffers[key] = -1*wait;
            }

            
            return false;
        }

        
    
    }
}