using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Data;

namespace MultiplayerTetris
{
    public class Tetromino
    {

        public const int SquareSize = 50;

        public static readonly Rectangle[][] Rectangles =
        {
            new Rectangle[] { new Rectangle(SquareSize, 0, SquareSize, SquareSize * 4) },
            new Rectangle[] { new Rectangle(SquareSize, SquareSize, SquareSize * 2, SquareSize * 2) },
            
            new Rectangle[]
            {
                new Rectangle(0, SquareSize, SquareSize, SquareSize),
                new Rectangle(SquareSize, 0, SquareSize, SquareSize * 3)
            },

            new Rectangle[]
            {
                new Rectangle(0, 0, SquareSize, SquareSize * 2),
                new Rectangle(SquareSize, SquareSize, SquareSize, SquareSize * 2)
            },
            new Rectangle[]
            {
                new Rectangle(0, SquareSize, SquareSize, SquareSize * 2),
                new Rectangle(SquareSize, 0, SquareSize, SquareSize * 2)
            },

            new Rectangle[]
            {
                new Rectangle(0, SquareSize * 2, SquareSize, SquareSize),
                new Rectangle(SquareSize, 0, SquareSize, SquareSize * 3)
            },
            new Rectangle[]
            {
                new Rectangle(0, 0, SquareSize * 2, SquareSize),
                new Rectangle(SquareSize, SquareSize, SquareSize, SquareSize * 2)
            }
        };

        public static readonly int[][] Transforms =
        {
            new int[] { },                  // I
            new int[] { },                  // O
            new int[] { -1, -1, 0, -1 },    // T
            new int[] { -1, -1, 0, -1 },    // S
            new int[] { -1, -1, 1, -1 },    // Z
            new int[] { -1, -1, 0, -1 },    // J
            new int[] { -1, -1, 0, -1 }     // L
        };

        public enum Type { I, O, T, S, Z, J, L }
        
        public Rectangle[] Shape { get; set; }

        public BitArray Rotation;

        public Color Colour;

        public Type BlockType;
        
        //FIXME: better variable naming
        public bool RequiresUpdate = true;

        private Rectangle[] _r;
        ////////////////////////

        public Tetromino(Type type)
        {
            BlockType=type;
            Rotation = new BitArray(2);

            Shape = Rectangles[(int)type];
            Colour = Game1.CurrentColorPalette[type];
        }

        public void Update(int x, int y, BitArray rotation, SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            _r = new Rectangle[Shape.Length];
            for (int index = 0; index < Shape.Length; index++)
            {
                Vector2[] _vertices = new Vector2[4];

                int i = 0;
                foreach (Vector2 v in Rectangle.RectToVertices(Shape[index]))
                {
                    
                    int _x = (int) v.X;
                    int _y = (int)v.Y;
                    
                    // transform so it's relative to the middle instead of the top left
                    _x -= SquareSize * 2;
                    _y -= SquareSize * 2;

                    int tempx = _x;
                    int tempy = _y;
                    
                    // 2 bit binary number 90*n degree rotation matrix bit fuckery
                    _x = ((rotation[1]) ? tempy : tempx) * ((rotation[0] ^ rotation[1]) ? -1 : 1);
                    _y = ((rotation[1]) ? tempx : tempy) * ((rotation[0]) ? -1 : 1);
                    
                    // transform it back
                    _x += SquareSize * 2;
                    _y += SquareSize * 2;

                    int intRot = 0;
                    intRot += (rotation[1]) ? 1:0;
                    intRot += (rotation[0]) ? 2:0;

                    int[] currentTransform = Transforms[(int)BlockType];
                    switch (intRot)
                    {
                        case 0:
                            break;

                        case 1:
                            if (currentTransform.Length>0)_x-=SquareSize;
                            break;

                        case 2:
                        case 3:
                            if (currentTransform.Length>0){
                                _x+=currentTransform[2*(intRot-2) ] * SquareSize;
                                _y+=currentTransform[2*(intRot-2) +1] * SquareSize;
                            }
                            

                            break;
                            

                    }

                    // set this vertex in the array
                    _vertices[i] = new Vector2(_x,_y);
                    i++;
                    

                }

                // draw
                _r[index] = Rectangle.VerticesToRect(_vertices);
                
            }
            RequiresUpdate = false; //FIXME: make this better x)
        }

        public void Draw(int x, int y, BitArray rotation, SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            if (RequiresUpdate)
                Update(x, y, rotation, _spriteBatch, _graphics);
            foreach (Rectangle r in _r)
            {
                Rectangle.Draw(r.X+x,r.Y+y,r.Width,r.Height, Colour, _spriteBatch, _graphics);
            }
        }
    }
}