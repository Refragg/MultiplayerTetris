using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public const float DefaultStickDeadZone = 0.3f;

        public const float DefaultTriggerDeadZone = 0.1f;
        
        public static readonly Dictionary<Controls, Keys>[] KeyboardDefaults =
        {
            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.D },
                { Controls.MoveLeft, Keys.A },
                { Controls.RotateRight, Keys.Right },
                { Controls.RotateLeft, Keys.Left },
                { Controls.SoftDrop, Keys.W },
                { Controls.HardDrop, Keys.S },
                { Controls.Hold, Keys.LeftShift }
            },

            new Dictionary<Controls, Keys>()
            {
                { Controls.MoveRight, Keys.NumPad6 },
                { Controls.MoveLeft, Keys.NumPad4 },
                { Controls.RotateRight, Keys.G },
                { Controls.RotateLeft, Keys.R },
                { Controls.SoftDrop, Keys.NumPad8 },
                { Controls.HardDrop, Keys.NumPad5 },
                { Controls.Hold, Keys.Z }
            }
            
        };

        public static readonly Dictionary<Controls, Buttons>[] GamePadDefaults =
        {
            new Dictionary<Controls, Buttons>()
            {
                { Controls.MoveRight, Buttons.DPadRight },
                { Controls.MoveLeft, Buttons.DPadLeft },
                { Controls.RotateRight, Buttons.B },
                { Controls.RotateLeft, Buttons.X },
                { Controls.SoftDrop, Buttons.Y },
                { Controls.HardDrop, Buttons.A },
                { Controls.Hold, Buttons.LeftShoulder }
            }
        };
    }

    public struct PlayerControl
    {
        public bool IsGamePad;

        public int PlayerIndex;

        public int GamePadIndex;

        public float StickDeadZone;

        public float TriggerDeadZone;
        
        public Keys Key;

        public Buttons Button;

        public PlayerControl(bool isGamePad, int playerIndex, int gamePadIndex, float stickDeadZone, float triggerDeadZone, Keys key, Buttons button)
        {
            IsGamePad = isGamePad;
            PlayerIndex = playerIndex;
            GamePadIndex = gamePadIndex;
            StickDeadZone = stickDeadZone;
            TriggerDeadZone = triggerDeadZone;
            Key = key;
            Button = button;
        }
    }
    
    public class PlayerControllerManager
    {
        private readonly string ControlsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "controls.json");
        
        private Dictionary<string, ControllerPreset> _controllerPresets;

        public string[] GetPresets()
        {
            return _controllerPresets.Keys.ToArray();
        }
        
        public PlayerControl GetControl(string preset, int playerIndex, Controls control)
        {
            PlayerController playerController = _controllerPresets[preset].PlayerControllers[playerIndex];

            if (!playerController.IsGamePad)
            {
                return new PlayerControl(playerController.IsGamePad, playerController.PlayerIndex, playerController.GamePadIndex, playerController.StickDeadZone, playerController.TriggerDeadZone, playerController.PlayerKeyboardControls[control], 0);
            }
            
            return new PlayerControl(playerController.IsGamePad, playerController.PlayerIndex, playerController.GamePadIndex, playerController.StickDeadZone, playerController.TriggerDeadZone, 0, playerController.PlayerGamePadControls[control]);
        }
        
        public String GetName(string preset, int playerIndex)
        {
            return _controllerPresets[preset].PlayerControllers[playerIndex].PlayerName;
        }
        
        public Dictionary<Controls, Keys> GetControls(string preset, int playerIndex)
        {
            return _controllerPresets[preset].PlayerControllers[playerIndex].PlayerKeyboardControls;
        }
        
        public PlayerControllerManager()
        {
            if (!File.Exists(ControlsFile))
            {
                SerializedControllerPreset[] controllerPresets =
                {
                    new SerializedControllerPreset("Keyboards", false),
                    new SerializedControllerPreset("Gamepads", true)
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
        
        public SerializedControllerPreset(string presetName, bool isGamePad)
        {
            PresetName = presetName;

            if (!isGamePad)
            {
                PlayerControllers = new SerializedPlayerController[DefaultControls.KeyboardDefaults.Length];
                for (int i = 0; i < DefaultControls.KeyboardDefaults.Length; i++)
                {
                    PlayerControllers[i] = new SerializedPlayerController("Guest", i, i, false, DefaultControls.DefaultStickDeadZone, DefaultControls.DefaultTriggerDeadZone);
                }
                return;
            }
            
            PlayerControllers = new SerializedPlayerController[DefaultControls.GamePadDefaults.Length];
            for (int i = 0; i < DefaultControls.GamePadDefaults.Length; i++)
            {
                PlayerControllers[i] = new SerializedPlayerController("Guest", i, i, true, DefaultControls.DefaultStickDeadZone, DefaultControls.DefaultTriggerDeadZone);
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

        public int GamePadIndex;

        public bool IsGamePad;

        public float StickDeadZone;

        public float TriggerDeadZone;
        
        public Dictionary<Controls, String> PlayerControls;
        
        public SerializedPlayerController(string playerName, int playerIndex, int gamePadIndex, bool isGamePad, float stickDeadZone, float triggerDeadZone)
        {
            PlayerName = playerName;
            
            PlayerIndex = playerIndex;

            GamePadIndex = gamePadIndex;
            
            IsGamePad = isGamePad;

            StickDeadZone = stickDeadZone;

            TriggerDeadZone = triggerDeadZone;

            PlayerControls = new Dictionary<Controls, string>();

            if (!isGamePad)
            {
                foreach (Controls control in typeof(Controls).GetEnumValues())
                {
                    PlayerControls.Add(control, DefaultControls.KeyboardDefaults[playerIndex][control].ToString());
                }
                return;
            }
            
            foreach (Controls control in typeof(Controls).GetEnumValues())
            {
                PlayerControls.Add(control, DefaultControls.GamePadDefaults[playerIndex][control].ToString());
            }
        }

        public PlayerController ToPlayerController()
        {
            Dictionary<Controls, Keys> playerKeyboardControls = null;
            Dictionary<Controls, Buttons> playerGamePadControls = null;
            if (!IsGamePad)
            {
                //parsing the key string to the corresponding enum value
                playerKeyboardControls = new Dictionary<Controls, Keys>();
                foreach ((Controls control, string key) in PlayerControls)
                {
                    if(!Keys.TryParse(key, out Keys keyOrNone))
                        Console.WriteLine($"Could not parse the key {key} off of the controls");
                                 
                    playerKeyboardControls.Add(control, keyOrNone);
                }
                
                return new PlayerController(PlayerName, PlayerIndex, GamePadIndex, IsGamePad, StickDeadZone, TriggerDeadZone, playerKeyboardControls, playerGamePadControls);
            }
            
            playerGamePadControls = new Dictionary<Controls, Buttons>();
            foreach ((Controls control, string button) in PlayerControls)
            {
                if (!Buttons.TryParse(button, out Buttons buttonOrNone))
                {
                    buttonOrNone = Buttons.BigButton;
                    Console.WriteLine($"Could not parse the button {button} off of the controls");
                }
                                 
                playerGamePadControls.Add(control, buttonOrNone);
            }
            
            return new PlayerController(PlayerName, PlayerIndex, GamePadIndex, IsGamePad, StickDeadZone, TriggerDeadZone, playerKeyboardControls, playerGamePadControls);
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

            PlayerControllers = new PlayerController[DefaultControls.KeyboardDefaults.Length];
            for (int i = 0; i < DefaultControls.KeyboardDefaults.Length; i++)
            {
                PlayerControllers[i] = new PlayerController("Guest", i, i, false, DefaultControls.DefaultStickDeadZone, DefaultControls.DefaultTriggerDeadZone);
            }
        }
    }

    public class PlayerController
    {
        public string PlayerName;
        
        public int PlayerIndex;
        
        public int GamePadIndex;

        public bool IsGamePad;

        public float StickDeadZone;

        public float TriggerDeadZone;
        
        public Dictionary<Controls, Keys> PlayerKeyboardControls;

        public Dictionary<Controls, Buttons> PlayerGamePadControls;

        public PlayerController(string playerName, int playerIndex, int gamePadIndex, bool isGamePad, float stickDeadZone, float triggerDeadZone)
        {
            PlayerName = playerName;
            
            PlayerIndex = playerIndex;

            GamePadIndex = gamePadIndex;

            IsGamePad = isGamePad;

            StickDeadZone = stickDeadZone;

            TriggerDeadZone = triggerDeadZone;

            if (!isGamePad)
            {
                PlayerKeyboardControls = new Dictionary<Controls, Keys>();

                foreach (Controls control in typeof(Controls).GetEnumValues())
                {
                    PlayerKeyboardControls.Add(control, DefaultControls.KeyboardDefaults[playerIndex][control]);
                }
                return;
            }
            
            PlayerGamePadControls = new Dictionary<Controls, Buttons>();

            foreach (Controls control in typeof(Controls).GetEnumValues())
            {
                PlayerGamePadControls.Add(control, DefaultControls.GamePadDefaults[playerIndex][control]);
            }
        }
        
        public PlayerController(string playerName, int playerIndex, int gamePadIndex, bool isGamePad, float stickDeadZone, float triggerDeadZone, Dictionary<Controls, Keys> playerKeyboardControls, Dictionary<Controls, Buttons> playerGamePadControls)
        {
            PlayerName = playerName;
            
            PlayerIndex = playerIndex;

            GamePadIndex = gamePadIndex;
            
            IsGamePad = isGamePad;

            StickDeadZone = stickDeadZone;

            TriggerDeadZone = triggerDeadZone;

            PlayerKeyboardControls = playerKeyboardControls;

            PlayerGamePadControls = playerGamePadControls;
        }
    }
    #endregion
}
