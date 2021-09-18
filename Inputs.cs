﻿using System;
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

            foreach (Keys currentKey in typeof(Keys).GetEnumValues())
            {
                releasedBuffers.Add(currentKey, false);
                pressedBuffers.Add(currentKey, false);
                timedBuffers.Add(currentKey, 0);
            }
        }

        public bool KeyReleased(Keys key)
        {
            bool buffer = releasedBuffers[key];
            
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
            bool buffer = pressedBuffers[key];

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(key))
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
        
        public bool TimedPress(Keys key, int rate)
        {
            int buffer = 0;

            buffer = timedBuffers[key];

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