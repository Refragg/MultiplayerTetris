using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerTetris
{
    public class Rectangle
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Color Colour { get; set; }



        public Rectangle(int x, int y, int width, int height, Color color)
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;

            Colour = color;
        }

        public Rectangle(int x, int y, int width, int height) : this(x, y,width, height, Color.White) { }
        
        public void Draw(SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            Draw(Colour, _spriteBatch, _graphics);
        }

        public void Draw(Color _colour, SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {

            Draw(X, Y, Width, Height, _colour, _spriteBatch, _graphics);
        }

        public static void Draw(int x, int y, int w, int h, Color _colour, SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            _spriteBatch.Begin();

            Color[] data = new Color[w * h];
            Texture2D rectTexture = new Texture2D(_graphics.GraphicsDevice, w, h);

            for (int i = 0; i < data.Length; ++i) 
                data[i] = Color.White;

            rectTexture.SetData(data);
            var position = new Vector2(x, y);

            _spriteBatch.Draw(rectTexture, position, _colour);

            _spriteBatch.End();
        }
    
    
    
    }
}