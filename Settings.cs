using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MultiplayerTetris
{
    public static class DefaultSettings
    {
        
        public const int NumPlayers = 1;
        public const int Seed = -1;
        
        public const int GridSquareSize = 30;
        public const int ScreenWidth = (16 + 10 * NumPlayers) * GridSquareSize;
        public const int ScreenHeight = (30) * GridSquareSize;

        public const bool Fullscreen = false;
        
        public const int GridHeight = 24;

        public const int NextPiecesAmount = 5;

        public const float SoftDropAmount = 20f;

        public const int InputWait = 10;
        public const int InputSpeed = 2;
        
        public const bool BlockDisplayMode = false;
        public const bool PushUp = false;

        public const bool DisplayGrid = false;
        public const bool GridUnderneath = true;

        public const string ControlsUsedPreset = "Keyboards";

        public const bool DisplayingNames = true;

        public const bool PerPlayerPhantomColours = true;
        public const bool OutlinedPhantomDisplay = true;
        
        public static readonly Color[] PhantomColours =
        {
            new Color(130,106,155,100),
            new Color(106,155,130,100)
        };
        public static readonly Color PhantomColourDefault = new Color(120,120,120,100);
        
        // square indicators
        public const bool SquareIndicators = true;
        public const bool SquareIndicatorsOnFalling = true;
        public const int SquareIndicatorPadding = 3; // padding will be (1/SquareIndicatorPadding)*square width
        public const Settings.SIT SquareIndicatorType = Settings.SIT.FullBorder;
    }
    
    public class Settings
    {
        public enum SIT // square indicator type
        {
            Square,
            Border,
            FullBorder
        }
        
        public int NumPlayers;
        public int Seed;
        
        public int GridSquareSize;
        public int ScreenWidth;
        public int ScreenHeight;

        public bool Fullscreen;

        public int GridHeight;

        public int NextPiecesAmount;

        public float SoftDropAmount;

        public int InputWait;
        public int InputSpeed;

        public bool BlockDisplayMode;
        public bool PushUp;

        public bool DisplayGrid;
        public bool GridUnderneath;

        public string ControlsUsedPreset;

        public bool DisplayingNames;

        public bool PerPlayerPhantomColours;
        public bool OutlinedPhantomDisplay;

        public Color[] PhantomColours;
        public Color PhantomColourDefault;
        
        // square indicators
        public bool SquareIndicators;
        public bool SquareIndicatorsOnFalling;
        public int SquareIndicatorPadding; // padding will be (1/SquareIndicatorPadding)*square width
        public SIT SquareIndicatorType;

        [JsonConstructor]
        public Settings(int numPlayers, int seed, int gridSquareSize, int screenWidth, int screenHeight, bool fullscreen, int gridHeight, int nextPiecesAmount, float softDropAmount, int inputWait, int inputSpeed, bool blockDisplayMode, bool pushUp, bool displayGrid, bool gridUnderneath, string controlsUsedPreset, bool displayingNames, bool perPlayerPhantomColours, bool outlinedPhantomDisplay, Color[] phantomColours, Color phantomColourDefault, bool squareIndicators, bool squareIndicatorsOnFalling, int squareIndicatorPadding, SIT squareIndicatorType)
        {
            NumPlayers = numPlayers;
            Seed = seed;
            
            GridSquareSize = gridSquareSize;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            if (GridSquareSize < 0)
            {
                
                
                if (ScreenWidth >= 0)
                {
                    GridSquareSize = ScreenWidth / (16 + 10 * NumPlayers);
                    if (ScreenHeight >= 0)
                    {
                        if (ScreenHeight / 30 < GridSquareSize)
                        {
                            GridSquareSize = ScreenHeight / 30;
                        }
                    }
                }
                else
                {
                    GridSquareSize = ScreenHeight / 30;
                }
            }

            if (ScreenWidth < 0)
            {
                ScreenWidth = (16 + 10 * NumPlayers) * GridSquareSize;
            }

            if (ScreenHeight < 0)
            {
                ScreenHeight = (30) * GridSquareSize;
            }

            Fullscreen = fullscreen;

            GridHeight = gridHeight;
            NextPiecesAmount = nextPiecesAmount;
            SoftDropAmount = softDropAmount;
            InputWait = inputWait;
            InputSpeed = inputSpeed;
            
            BlockDisplayMode = blockDisplayMode;
            PushUp = pushUp;
            DisplayGrid = displayGrid;
            GridUnderneath = gridUnderneath;
            ControlsUsedPreset = controlsUsedPreset;
            DisplayingNames = displayingNames;
            PerPlayerPhantomColours = perPlayerPhantomColours;
            OutlinedPhantomDisplay = outlinedPhantomDisplay;

            PhantomColours = phantomColours;
            PhantomColourDefault = phantomColourDefault;
            
            SquareIndicators = squareIndicators;
            SquareIndicatorsOnFalling = squareIndicatorsOnFalling;
            SquareIndicatorPadding = squareIndicatorPadding;
            SquareIndicatorType = squareIndicatorType;
        }

        public Settings()
        {
            NumPlayers = DefaultSettings.NumPlayers;
            Seed = DefaultSettings.Seed;

            GridSquareSize = DefaultSettings.GridSquareSize;
            ScreenWidth = DefaultSettings.ScreenWidth;
            ScreenHeight = DefaultSettings.ScreenHeight;

            Fullscreen = DefaultSettings.Fullscreen;
            
            GridHeight = DefaultSettings.GridHeight;
            NextPiecesAmount = DefaultSettings.NextPiecesAmount;
            SoftDropAmount = DefaultSettings.SoftDropAmount;
            InputWait = DefaultSettings.InputWait;
            InputSpeed = DefaultSettings.InputSpeed;
            
            BlockDisplayMode = DefaultSettings.BlockDisplayMode;
            PushUp = DefaultSettings.PushUp;
            DisplayGrid = DefaultSettings.DisplayGrid;
            GridUnderneath = DefaultSettings.GridUnderneath;
            ControlsUsedPreset = DefaultSettings.ControlsUsedPreset;
            DisplayingNames = DefaultSettings.DisplayingNames;
            PerPlayerPhantomColours = DefaultSettings.PerPlayerPhantomColours;
            OutlinedPhantomDisplay = DefaultSettings.OutlinedPhantomDisplay;
            
            PhantomColours = DefaultSettings.PhantomColours;
            PhantomColourDefault = DefaultSettings.PhantomColourDefault;

            SquareIndicators = DefaultSettings.SquareIndicators;
            SquareIndicatorsOnFalling = DefaultSettings.SquareIndicatorsOnFalling;
            SquareIndicatorPadding = DefaultSettings.SquareIndicatorPadding;
            SquareIndicatorType = DefaultSettings.SquareIndicatorType;
        }
    }

    public class SettingsManager
    {
        private readonly string SettingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        private readonly Dictionary<string, SettingsPreset> _settingsPresets;

        private readonly string _requestedPreset;

        public string GetRequestedPreset() => _requestedPreset;
        
        public SettingsPreset[] GetSettings()
        {
            return _settingsPresets.Values.ToArray();
        }
        
        public Settings GetSettings(string preset)
        {
            return _settingsPresets[preset].Settings;
        }

        public SettingsManager()
        {
            if (!File.Exists(SettingsFile))
            {
                SettingsPreset[] settingsPresets = 
                {
                    new SettingsPreset("Default"),
                    new SettingsPreset("Custom")
                };
                
                SerializedSettingsPresets toSerialize = new SerializedSettingsPresets(settingsPresets[0].PresetName, settingsPresets);
                
                File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(toSerialize, Formatting.Indented));

                _requestedPreset = settingsPresets[0].PresetName;
                _settingsPresets = new Dictionary<string, SettingsPreset>();
                foreach (SettingsPreset preset in settingsPresets)
                {
                    _settingsPresets.Add(preset.PresetName, preset);
                }
                
                return;
            }
            
            using (StreamReader reader = File.OpenText(SettingsFile))
            using (JsonTextReader treader = new JsonTextReader(reader))
            {
                JObject jsonObj = (JObject)JToken.ReadFrom(treader);
                //jsonObj.Value<string>("Version"); //Settings file version
                
                SettingsPreset[] settingsPresets = jsonObj[nameof(SerializedSettingsPresets.Presets)].ToObject<SettingsPreset[]>();
                _requestedPreset = jsonObj[nameof(SerializedSettingsPresets.UsedPreset)].ToObject<string>();
                _settingsPresets = new Dictionary<string, SettingsPreset>();

                foreach (SettingsPreset preset in settingsPresets)
                {
                    _settingsPresets.Add(preset.PresetName, preset);
                }
            }
        }
    }
    
    //root object of the json
    public class SerializedSettingsPresets
    {
        public string Version { get; set; }
        
        public string UsedPreset { get; set; }
        
        public SettingsPreset[] Presets { get; set; }

        [JsonConstructor]
        public SerializedSettingsPresets(string usedPreset, SettingsPreset[] presets)
        {
            Version = "1.0";
            UsedPreset = usedPreset;
            Presets = presets;
        }
    }

    public class SettingsPreset
    {
        public string PresetName;

        public Settings Settings;
        
        [JsonConstructor]
        public SettingsPreset(string presetName, Settings settings)
        {
            PresetName = presetName;
            Settings = settings;
        }
        
        public SettingsPreset(string presetName)
        {
            PresetName = presetName;

            Settings = new Settings();
        }
    }
}