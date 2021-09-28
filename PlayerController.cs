using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiplayerTetris
{
    public enum Controls
    {
        MoveRight,
        MoveLeft,
        RotateRight,
        RotateLeft,
        SoftDrop,
        HardDrop,
        Hold
    }
    
    public static class DefaultControls
    {
        public static readonly Dictionary<Controls, Keys>[] Defaults =
        {
            /*
            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, (Keys)102 },
                { Controls.MoveLeft, (Keys)100 },
                { Controls.RotateRight, (Keys)80 },
                { Controls.RotateLeft, (Keys)79 },
                { Controls.SoftDrop, (Keys)73 },
                { Controls.HardDrop, (Keys)101 },
                { Controls.Hold, (Keys)161 }
            },
            
            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, (Keys)39 },
                { Controls.MoveLeft, (Keys)37 },
                { Controls.RotateRight, (Keys)38 },
                { Controls.RotateLeft, (Keys)189 },
                { Controls.SoftDrop, (Keys)40 },
                { Controls.HardDrop, (Keys)32 },
                { Controls.Hold, (Keys)67 }
            },
            
            
            
            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, (Keys)68 },
                { Controls.MoveLeft, (Keys)65 },
                { Controls.RotateRight, (Keys)190 },
                { Controls.RotateLeft, (Keys)188 },
                { Controls.SoftDrop, (Keys)87 },
                { Controls.HardDrop, (Keys)83 },
                { Controls.Hold, (Keys)160 }
            }*/

            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.D },
                { Controls.MoveLeft, Keys.A },
                { Controls.RotateRight, Keys.H },
                { Controls.RotateLeft, Keys.G },
                { Controls.SoftDrop, Keys.W },
                { Controls.HardDrop, Keys.S },
                { Controls.Hold, Keys.LeftShift }
            },

            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.NumPad6 },
                { Controls.MoveLeft, Keys.NumPad4 },
                { Controls.RotateRight, Keys.Right },
                { Controls.RotateLeft, Keys.Left },
                { Controls.SoftDrop, Keys.NumPad8 },
                { Controls.HardDrop, Keys.NumPad5 },
                { Controls.Hold, Keys.RightControl }
            },


            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.OemMinus },
                { Controls.MoveLeft, Keys.OemMinus },
                { Controls.RotateRight, Keys.OemMinus },
                { Controls.RotateLeft, Keys.OemMinus },
                { Controls.SoftDrop, Keys.OemMinus },
                { Controls.HardDrop, Keys.OemMinus },
                { Controls.Hold, Keys.OemMinus }
            },

            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.OemMinus },
                { Controls.MoveLeft, Keys.OemMinus },
                { Controls.RotateRight, Keys.OemMinus },
                { Controls.RotateLeft, Keys.OemMinus },
                { Controls.SoftDrop, Keys.OemMinus },
                { Controls.HardDrop, Keys.OemMinus },
                { Controls.Hold, Keys.OemMinus }
            },

            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.OemMinus },
                { Controls.MoveLeft, Keys.OemMinus },
                { Controls.RotateRight, Keys.OemMinus },
                { Controls.RotateLeft, Keys.OemMinus },
                { Controls.SoftDrop, Keys.OemMinus },
                { Controls.HardDrop, Keys.OemMinus },
                { Controls.Hold, Keys.OemMinus }
            },

            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.OemMinus },
                { Controls.MoveLeft, Keys.OemMinus },
                { Controls.RotateRight, Keys.OemMinus },
                { Controls.RotateLeft, Keys.OemMinus },
                { Controls.SoftDrop, Keys.OemMinus },
                { Controls.HardDrop, Keys.OemMinus },
                { Controls.Hold, Keys.OemMinus }
            },

            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.OemMinus },
                { Controls.MoveLeft, Keys.OemMinus },
                { Controls.RotateRight, Keys.OemMinus },
                { Controls.RotateLeft, Keys.OemMinus },
                { Controls.SoftDrop, Keys.OemMinus },
                { Controls.HardDrop, Keys.OemMinus },
                { Controls.Hold, Keys.OemMinus }
            }


        };
    }

    public class PlayerControllerManager
    {
        private readonly string ControlsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "controls.json");
        
        private PlayerController[] _playerControllers;
        
        public Keys GetControl(int playerIndex, Controls control)
        {
            
            return _playerControllers[playerIndex].PlayerControls[control];
        }
        
        public Dictionary<Controls, Keys> GetControls(int playerIndex)
        {
            return _playerControllers[playerIndex].PlayerControls;
        }
        
        public PlayerControllerManager()
        {
            if (!File.Exists(ControlsFile))
            {

                PlayerController[] playerControllers = new PlayerController[DefaultControls.Defaults.Length];
                
                for (int i = 0; i < DefaultControls.Defaults.Length; i++)
                {
                    playerControllers[i] = new PlayerController(i);
                }
                

                SerializedPlayerControllers toSerialize = new SerializedPlayerControllers(playerControllers);
                
                File.WriteAllText(ControlsFile, JsonConvert.SerializeObject(toSerialize, Formatting.Indented));

                _playerControllers = playerControllers;
                return;
            }
            
            using (StreamReader reader = File.OpenText(ControlsFile))
            using (JsonTextReader treader = new JsonTextReader(reader))
            {
                JObject jsonObj = (JObject)JToken.ReadFrom(treader);
                //jsonObj.Value<string>("Version"); //PlayerController file version
                
                PlayerController[] playerControllersUnordered = jsonObj["Controllers"].ToObject<PlayerController[]>();

                //ordering the array based on the PlayerIndex field
                int unorderedArrayLength = playerControllersUnordered.Length;

                _playerControllers = new PlayerController[unorderedArrayLength];
                for (int i = 0; i < unorderedArrayLength; i++)
                {
                    int playerIndex = playerControllersUnordered[i].PlayerIndex;

                    _playerControllers[playerIndex] = playerControllersUnordered[i];
                }
            }
        }
    }

    public class SerializedPlayerControllers
    {
        public string Version { get; set; }
        
        public PlayerController[] Controllers { get; set; }

        [JsonConstructor]
        public SerializedPlayerControllers(PlayerController[] controller)
        {
            Version = "1.0";
            Controllers = controller;
        }
    }

    public class PlayerController
    {
        public int PlayerIndex;
        public Dictionary<Controls, Keys> PlayerControls;

        public PlayerController(int playerIndex)
        {
            PlayerIndex = playerIndex;

            PlayerControls = new Dictionary<Controls, Keys>();

            foreach (Controls control in typeof(Controls).GetEnumValues())
            {
                PlayerControls.Add(control, DefaultControls.Defaults[playerIndex][control]);
            }
        }
    }
}
