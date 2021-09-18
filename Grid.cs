using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MultiplayerTetris
{
    public class Grid
    {

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