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
        
        private Dictionary<string, ControllerPreset> _controllerPresets;
        
        public Keys GetControl(string preset, int playerIndex, Controls control)
        {
            return _controllerPresets[preset].PlayerControllers[playerIndex].PlayerControls[control];
        }
        
        public String GetName(string preset, int playerIndex)
        {
            return _controllerPresets[preset].PlayerControllers[playerIndex].PlayerName;
        }
        
        public Dictionary<Controls, Keys> GetControls(string preset, int playerIndex)
        {
            return _controllerPresets[preset].PlayerControllers[playerIndex].PlayerControls;
        }
        
        public PlayerControllerManager()
        {
            if (!File.Exists(ControlsFile))
            {
                SerializedControllerPreset[] controllerPresets =
                {
                    new SerializedControllerPreset("Keyboards"),
                    new SerializedControllerPreset("Controllers")
                };
                
                SerializedControllerPresets toSerialize = new SerializedControllerPresets(controllerPresets);
                
                File.WriteAllText(ControlsFile, JsonConvert.SerializeObject(toSerialize, Formatting.Indented));

                _controllerPresets = new Dictionary<string, ControllerPreset>();
                foreach (SerializedControllerPreset preset in controllerPresets)
                {
                    ControllerPreset controllerPreset = preset.ToControllerPreset();
                    
                    _controllerPresets.Add(controllerPreset.PresetName, controllerPreset);
                }
                
                return;
            }
            
            using (StreamReader reader = File.OpenText(ControlsFile))
            using (JsonTextReader treader = new JsonTextReader(reader))
            {
                JObject jsonObj = (JObject)JToken.ReadFrom(treader);
                //jsonObj.Value<string>("Version"); //PlayerController file version
                
                SerializedControllerPreset[] controllerPresets = jsonObj[nameof(SerializedControllerPresets.Presets)].ToObject<SerializedControllerPreset[]>();

                _controllerPresets = new Dictionary<string, ControllerPreset>();

                foreach (SerializedControllerPreset preset in controllerPresets)
                {
                    ControllerPreset controllerPreset = preset.ToControllerPreset();

                    int unorderedArrayLength = controllerPreset.PlayerControllers.Length;

                    PlayerController[] playerControllersOrdered = new PlayerController[unorderedArrayLength];
                        
                    //ordering the PlayerControllers array based on the PlayerIndex field
                    for (int i = 0; i < unorderedArrayLength; i++)
                    {
                        int playerIndex = controllerPreset.PlayerControllers[i].PlayerIndex;

                        playerControllersOrdered[playerIndex] = controllerPreset.PlayerControllers[i];
                    }

                    controllerPreset.PlayerControllers = playerControllersOrdered;
                    
                    _controllerPresets.Add(controllerPreset.PresetName, controllerPreset);
                }
            }
        }
    }

    #region Serialization Classes
    //root object of the json
    public class SerializedControllerPresets
    {
        public string Version { get; set; }
        
        public SerializedControllerPreset[] Presets { get; set; }

        [JsonConstructor]
        public SerializedControllerPresets(SerializedControllerPreset[] presets)
        {
            Version = "1.0";
            Presets = presets;
        }
    }

    public class SerializedControllerPreset
    {
        public string PresetName;

        public SerializedPlayerController[] PlayerControllers;
        
        [JsonConstructor]
        public SerializedControllerPreset(string presetName, SerializedPlayerController[] playerControllers)
        {
            PresetName = presetName;
            PlayerControllers = playerControllers;
        }
        
        public SerializedControllerPreset(string presetName)
        {
            PresetName = presetName;

            PlayerControllers = new SerializedPlayerController[DefaultControls.Defaults.Length];
            for (int i = 0; i < DefaultControls.Defaults.Length; i++)
            {
                PlayerControllers[i] = new SerializedPlayerController("Guest", i);
            }
        }

        public ControllerPreset ToControllerPreset()
        {
            PlayerController[] playerControllers = new PlayerController[PlayerControllers.Length];

            for (int i = 0; i < PlayerControllers.Length; i++)
            {
                playerControllers[i] = PlayerControllers[i].ToPlayerController();
            }

            return new ControllerPreset(PresetName, playerControllers);
        }
    }
    
    public class SerializedPlayerController
    {
        public string PlayerName;
        
        public int PlayerIndex;
        
        public Dictionary<Controls, String> PlayerControls;
        
        public SerializedPlayerController(string playerName, int playerIndex)
        {
            PlayerName = playerName;
            
            PlayerIndex = playerIndex;

            PlayerControls = new Dictionary<Controls, string>();

            foreach (Controls control in typeof(Controls).GetEnumValues())
            {
                PlayerControls.Add(control, DefaultControls.Defaults[playerIndex][control].ToString());
            }
        }

        public PlayerController ToPlayerController()
        {
            //parsing the key string to the corresponding enum value
            Dictionary<Controls, Keys> playerControls = new Dictionary<Controls, Keys>();
            foreach ((Controls control, string key) in PlayerControls)
            {
                if(!Keys.TryParse(key, out Keys keyOrNone))
                    Console.WriteLine($"Could not parse the key {key} off of the controls");
                             
                playerControls.Add(control, keyOrNone);
            }

            return new PlayerController(PlayerName, PlayerIndex, playerControls);
        }
    }
    #endregion

    #region Deserialized objects
    public class ControllerPreset
    {
        public string PresetName;

        public PlayerController[] PlayerControllers;
        
        public ControllerPreset(string presetName, PlayerController[] playerControllers)
        {
            PresetName = presetName;
            PlayerControllers = playerControllers;
        }
            
        public ControllerPreset(string presetName)
        {
            PresetName = presetName;

            PlayerControllers = new PlayerController[DefaultControls.Defaults.Length];
            for (int i = 0; i < DefaultControls.Defaults.Length; i++)
            {
                PlayerControllers[i] = new PlayerController("Guest", i);
            }
        }
    }

    public class PlayerController
    {
        public string PlayerName;
        
        public int PlayerIndex;
        
        public Dictionary<Controls, Keys> PlayerControls;

        public PlayerController(string playerName, int playerIndex)
        {
            PlayerName = playerName;
            
            PlayerIndex = playerIndex;

            PlayerControls = new Dictionary<Controls, Keys>();

            foreach (Controls control in typeof(Controls).GetEnumValues())
            {
                PlayerControls.Add(control, DefaultControls.Defaults[playerIndex][control]);
            }
        }
        
        public PlayerController(string playerName, int playerIndex, Dictionary<Controls, Keys> playerControls)
        {
            PlayerName = playerName;
            
            PlayerIndex = playerIndex;

            PlayerControls = playerControls;
        }
    }
    #endregion
}
