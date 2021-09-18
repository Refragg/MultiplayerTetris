using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MultiplayerTetris
{
    public class TetrominoColorPalette
    {
        private Dictionary<Tetromino.Type, Color> _palette { get; set; }

        public TetrominoColorPalette()
        {
            _palette = new Dictionary<Tetromino.Type, Color>();

            foreach (Tetromino.Type currentType in typeof(Tetromino.Type).GetEnumValues())
            {
                _palette.Add(currentType, Color.White);
            }
        }

        public Color this[Tetromino.Type key]
        {
            get => _palette[key];
            set => _palette[key] = value;
        }
    }
}