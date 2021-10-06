using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MultiplayerTetris
{
    public class TetrominoColorPalette
    {
        private static readonly Color DefaultColor = Color.White;
        private Dictionary<Tetromino.Type, Color> _palette { get; set; }

        public TetrominoColorPalette()
        {
            _palette = new Dictionary<Tetromino.Type, Color>();

            foreach (Tetromino.Type currentType in typeof(Tetromino.Type).GetEnumValues())
            {
                _palette.Add(currentType, Color.White);
            }
        }

        public TetrominoColorPalette(Color[] colors)
        {
            Array enumValues = typeof(Tetromino.Type).GetEnumValues();
            if (colors.Length > enumValues.Length)
                throw new ArgumentException("Collection needs to be smaller or equals to the number of Tetromino Types", nameof(colors));

            List<Color> tempList = new List<Color>(colors);
            if (colors.Length != enumValues.Length)
            {
                while (tempList.Count != enumValues.Length)
                {
                    tempList.Add(DefaultColor);
                }
            }
            
            _palette = new Dictionary<Tetromino.Type, Color>();
            
            int i = 0;
            foreach (Tetromino.Type currentType in enumValues)
            {
                _palette.Add(currentType, tempList[i]);
                i++;
            }
        }

        public Color this[Tetromino.Type key]
        {
            get => _palette[key];
            set => _palette[key] = value;
        }
    }
}