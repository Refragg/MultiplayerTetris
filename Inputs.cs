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
        private KeyboardState _keyboardState;
        private GamePadState[] _gamePadStates;

        private Dictionary<Keys, bool> releasedBuffers;
        private Dictionary<Keys, bool> pressedBuffers;
        private Dictionary<Keys, int> timedBuffers;

        private Dictionary<Buttons, bool>[] releasedGamePadBuffers;
        private Dictionary<Buttons, bool>[] pressedGamePadBuffers;
        private Dictionary<Buttons, int>[] timedGamePadBuffers;

        public Inputs()
        {
            _gamePadStates = new GamePadState[GamePad.MaximumGamePadCount];
            
            releasedBuffers = new Dictionary<Keys,bool>();
            pressedBuffers = new Dictionary<Keys,bool>();
            timedBuffers = new Dictionary<Keys, int>();

            foreach (Keys currentKey in typeof(Keys).GetEnumValues())
            {
                releasedBuffers.Add(currentKey, false);
                pressedBuffers.Add(currentKey, false);
                timedBuffers.Add(currentKey, 0);
            }

            releasedGamePadBuffers = new Dictionary<Buttons, bool>[GamePad.MaximumGamePadCount];
            pressedGamePadBuffers = new Dictionary<Buttons, bool>[GamePad.MaximumGamePadCount];
            timedGamePadBuffers = new Dictionary<Buttons, int>[GamePad.MaximumGamePadCount];

            for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
            {
                releasedGamePadBuffers[i] = new Dictionary<Buttons, bool>();
                pressedGamePadBuffers[i] = new Dictionary<Buttons, bool>();
                timedGamePadBuffers[i] = new Dictionary<Buttons, int>();
                
                foreach (Buttons currentButton in typeof(Buttons).GetEnumValues())
                {
                    releasedGamePadBuffers[i].Add(currentButton, false);
                    pressedGamePadBuffers[i].Add(currentButton, false);
                    timedGamePadBuffers[i].Add(currentButton, 0);
                }
            }

            UpdateState();
        }

        public void UpdateState()
        {
            _keyboardState = Keyboard.GetState();
            for (int i = 0; i < _gamePadStates.Length; i++)
            {
                _gamePadStates[i] = GamePad.GetState(i, GamePadDeadZone.None, GamePadDeadZone.None);
            }
        }

        public bool KeyReleased(Keys key)
        {
            bool buffer = releasedBuffers[key];
            
            if (_keyboardState.IsKeyDown(key))
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
        
        public bool KeyPressed(PlayerControl control)
        {
            if (!control.IsGamePad)
            {
                bool buffer = pressedBuffers[control.Key];

                if (_keyboardState.IsKeyDown(control.Key))
                {
                    if (!buffer)
                    {
                        pressedBuffers[control.Key] = true;
                        return true;
                    }
                }
                else
                {
                    pressedBuffers[control.Key] = false;
                }
                
                return false;
            }
            
            bool gamePadBuffer = pressedGamePadBuffers[control.GamePadIndex][control.Button];

            if (_gamePadStates[control.GamePadIndex].IsButtonDown(control.Button, control.StickDeadZone, control.TriggerDeadZone))
            {
                if (!gamePadBuffer)
                {
                    pressedGamePadBuffers[control.GamePadIndex][control.Button] = true;
                    return true;
                }
            }
            else
            {
                pressedGamePadBuffers[control.GamePadIndex][control.Button] = false;
            }
                
            return false;
        }

        public bool KeyHeld(PlayerControl control)
        {
            if (!control.IsGamePad)
            {
                return _keyboardState.IsKeyDown(control.Key);
            }

            return _gamePadStates[control.GamePadIndex].IsButtonDown(control.Button, control.StickDeadZone, control.TriggerDeadZone);
        }

        public bool TimedPress(PlayerControl control, int rate, int wait)
        {
            if (!control.IsGamePad)
            {
                int buffer = 0;

                buffer = timedBuffers[control.Key];

                if (buffer >= rate)
                {
                    buffer = 0;
                    timedBuffers[control.Key] = 0;
                }

                if (_keyboardState.IsKeyDown(control.Key))
                {
                    if (timedBuffers[control.Key] == -1 * wait)
                    {
                        timedBuffers[control.Key]++;
                        return true;
                    }
                    
                    timedBuffers[control.Key]++;
                    if (buffer == 0 && timedBuffers[control.Key]>=0)
                    {
                        return true;
                    }
                }
                else
                {
                    timedBuffers[control.Key] = -1*wait;
                }

                
                return false;
            }
            
            int gamePadBuffer = 0;

            gamePadBuffer = timedGamePadBuffers[control.GamePadIndex][control.Button];

            if (gamePadBuffer >= rate)
            {
                gamePadBuffer = 0;
                timedGamePadBuffers[control.GamePadIndex][control.Button] = 0;
            }

            if (_gamePadStates[control.GamePadIndex].IsButtonDown(control.Button, control.StickDeadZone, control.TriggerDeadZone))
            {
                if (timedGamePadBuffers[control.GamePadIndex][control.Button] == -1 * wait)
                {
                    timedGamePadBuffers[control.GamePadIndex][control.Button]++;
                    return true;
                }
                    
                timedGamePadBuffers[control.GamePadIndex][control.Button]++;
                if (gamePadBuffer == 0 && timedGamePadBuffers[control.GamePadIndex][control.Button]>=0)
                {
                    return true;
                }
            }
            else
            {
                timedGamePadBuffers[control.GamePadIndex][control.Button] = -1*wait;
            }

                
            return false;
        }
    }
}