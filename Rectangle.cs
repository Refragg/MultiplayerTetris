using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

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


        public static Vector2[] RectToVertices(Rectangle rectangle)
        {

            Vector2[] vertices = new Vector2[4];


            vertices[0] = new Vector2(rectangle.X, rectangle.Y);
            vertices[1] = new Vector2(rectangle.X + rectangle.Width, rectangle.Y);
            vertices[2] = new Vector2(rectangle.X, rectangle.Y + rectangle.Height);
            vertices[3] = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

            return vertices;

        }

        public static Rectangle VerticesToRect(Vector2[] vertices)
        {
            //vertices.Min(v => v.X);
            int x = (int) vertices.Min(v => v.X);
            int y = (int) vertices.Min(v => v.Y);
            return new Rectangle(x,y,
                (int)vertices.Max(v => v.X)-x,(int)vertices.Max(v => v.Y)-y);
        }

        public static Vector2[] RectToSquares(Rectangle rect,int squareSize)
        {
            int x_amount = rect.Width / squareSize;
            int y_amount = rect.Height / squareSize;
            
            Vector2[] squares = new Vector2[x_amount*y_amount];
            
            for (int i = 0; i < x_amount; i++)
            {
                for (int j = 0; j < y_amount; j++)
                {
                    squares[i*y_amount + j] = new Vector2(rect.X + i*squareSize,rect.Y + j*squareSize);
                }
            }

            return squares;
        }


        public Rectangle(int x, int y, int width, int height) : this(x, y,width, height, Color.White) { }
        
        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            Draw(Colour, spriteBatch, graphics);
        }

        public void Draw(Color colour, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {

            Draw(X, Y, Width, Height, colour, spriteBatch, graphics);
        }

        public static void Draw(int x, int y, int w, int h, Color colour, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            Color[] data = new Color[w * h];
            Texture2D rectTexture = new Texture2D(graphics.GraphicsDevice, w, h);

            for (int i = 0; i < data.Length; ++i) 
                data[i] = Color.White;

            rectTexture.SetData(data);
            var position = new Vector2(x, y);

            spriteBatch.Draw(rectTexture, position, colour);
        }

        public static void DrawToTexture(Color[] data,int maxW,int maxH, int x, int y, int w, int h, Color colour, GraphicsDeviceManager graphics)
        {

            //Texture2D rectTexture = new Texture2D(graphics.GraphicsDevice, w, h);

            int start = x+y*maxW;

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
  
                    data[start + (j * maxW) + i] = Color.White;

                    
                }
            }
                


        }
    
    
    
    }
}