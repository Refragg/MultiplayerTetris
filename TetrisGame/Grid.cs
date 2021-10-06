using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MultiplayerTetris
{
    public class Grid
    {


        public Texture2D texture;
        public Vector2 position;
        public Color Colour;


        public Grid(int x, int y, int width, int height, int x_amount, int y_amount, Color color, int thickness, GraphicsDeviceManager _graphics)
        {

            Colour = color;

            Color[] pixelArray = new Color[(width*x_amount+1)*(height*y_amount+1)];
            
            texture = new Texture2D(_graphics.GraphicsDevice,width*x_amount +1, height*y_amount +1);
            
            
            int _x = 0;
            int _y = 0;

            for (int i = 0; i < x_amount+1; i++)
            {
                Rectangle.DrawToTexture(pixelArray,texture.Width,texture.Height,_x,_y,1,height*y_amount,color,_graphics);

                _x += width;
            }

            _x = 0;
            for (int j = 0; j < y_amount+1; j++)
            {
                Rectangle.DrawToTexture(pixelArray,texture.Width,texture.Height,_x,_y,width*x_amount,1,color,_graphics);
                
                _y += height;
            }
            
            //Rectangle.DrawToTexture(pixelArray,texture.Width,texture.Height,100,100,5,10,color,_graphics);
            
            position = new Vector2(x, y);
            texture.SetData(pixelArray);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager _graphics)
        {


            spriteBatch.Draw(texture, position, Colour);
            
            
        }

        public static void Draw(int x, int y, int width, int height, int x_amount, int y_amount, Color color, int thickness, SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {

            int _x = x;
            int _y = y;

            for (int i = 0; i < x_amount+1; i++)
            {
                Rectangle.Draw(_x,_y,thickness,height*y_amount + (thickness-1)*y_amount,color,_spriteBatch,_graphics);
                
                _x += width+thickness-1;
            }

            _x = x;
            for (int j = 0; j < y_amount+1; j++)
            {
                Rectangle.Draw(_x,_y,width*x_amount + (thickness-1)*x_amount,thickness,color,_spriteBatch,_graphics);
                
                _y += height+thickness-1;
            }

        }
        
        public static void Draw(int x, int y, int width, int height, int x_amount, int y_amount, SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            Draw(x, y, width, height, x_amount, y_amount, Color.White, 1, _spriteBatch, _graphics);
        } 
        
    
    }
}