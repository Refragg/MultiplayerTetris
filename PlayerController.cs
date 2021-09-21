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
                { Controls.MoveRight, Keys.L },
                { Controls.MoveLeft, Keys.J },
                { Controls.RotateRight, Keys.NumPad6 },
                { Controls.RotateLeft, Keys.NumPad4 },
                { Controls.SoftDrop, Keys.I },
                { Controls.HardDrop, Keys.K },
                { Controls.Hold, Keys.NumPad5 }
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
                PlayerController[] playerControllers =
                {
                    new PlayerController(0),
                    new PlayerController(1),
                };

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
                    
                _playerControllers = jsonObj["Controllers"].ToObject<PlayerController[]>();
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
