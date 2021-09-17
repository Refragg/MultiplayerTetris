using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerTetris
{
    public class Tetromino
    {
        public static class Rectangles
        {
            public const int SquareSize = 20;
            
            public static readonly Rectangle[] I = new Rectangle[] { new Rectangle(SquareSize, 0, SquareSize, SquareSize * 4) }; 
            public static readonly Rectangle[] O = new Rectangle[] { new Rectangle(SquareSize, SquareSize, SquareSize * 2, SquareSize * 2) }; 

            public static readonly Rectangle[] T = new Rectangle[] 
            { 
                new Rectangle(0, SquareSize, SquareSize, SquareSize),
                new Rectangle(SquareSize, 0, SquareSize, SquareSize * 3)
            };

            public static readonly Rectangle[] S = new Rectangle[] 
            { 
                new Rectangle(0, 0, SquareSize, SquareSize * 2),
                new Rectangle(SquareSize, SquareSize, SquareSize, SquareSize * 2)
            };
            public static readonly Rectangle[] Z = new Rectangle[] 
            { 
                new Rectangle(0, SquareSize, SquareSize, SquareSize * 2),
                new Rectangle(SquareSize, 0, SquareSize, SquareSize * 2)
            };

            public static readonly Rectangle[] J = new Rectangle[]
            { 
                new Rectangle(0, SquareSize * 2, SquareSize, SquareSize),
                new Rectangle(SquareSize, 0, SquareSize, SquareSize * 3)
            };
            public static readonly Rectangle[] L = new Rectangle[]
            { 
                new Rectangle(0, 0, SquareSize * 2, SquareSize),
                new Rectangle(SquareSize, SquareSize, SquareSize, SquareSize * 2)
            };
        }
        public enum Type { I, O, T, S, Z, J, L }
        
        public Rectangle[] Shape { get; set; }

        public BitArray Rotation;

        public Color Colour;

        public Tetromino(Type type)
        {

            Rotation = new BitArray(2);

            switch (type) 
            {
                case Type.I:
                    Shape = Rectangles.I;
                    Colour = new Color(49,199,239);
                    break;
                case Type.O:
                    Shape = Rectangles.O;
                    Colour = new Color(247,211,8);
                    break;
                case Type.T:
                    Shape = Rectangles.T;
                    Colour = new Color(173,77,156);
                    break;
                case Type.S:
                    Shape = Rectangles.S;
                    Colour = new Color(66,182,66);
                    break;
                case Type.Z:
                    Shape = Rectangles.Z;
                    Colour = new Color(239,32,41);
                    break;
                case Type.J:
                    Shape = Rectangles.J;
                    Colour = new Color(90,101,173);
                    break;
                case Type.L:
                    Shape = Rectangles.L;
                    Colour = new Color(239,121,33);
                    break;
            }

        }


        public void Draw(int x, int y, SpriteBatch spriteBatch, GraphicsDeviceManager graphics) 
        {

            foreach (Rectangle r in Shape)
            {

                int _x = r.X;
                int _y = r.Y;

                Rectangle.Draw(_x + 2*x, _y + 2*y, r.Width, r.Height, Colour, spriteBatch, graphics);

                _x -= SquareSize * 2;
                _y -= SquareSize * 2;



                int temp = _x;
                _x = -_y;
                _y = temp;

                _x += SquareSize * 2;
                _y += SquareSize * 2;

                Rectangle.Draw(_x+ x - ((r.Height > 2*SquareSize)? SquareSize*2:0) , _y+y - ((r.Width > SquareSize)? SquareSize:0), r.Height, r.Width, Colour, spriteBatch, graphics);
            }

        }

    }
}