using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

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
            new int[] { -1, -1, 0, -1 },    // Z
            new int[] { -1, -1, 0, -1 },    // J
            new int[] { -1, 0, -1, -1, 0, -1 }     // L
        };

        public enum Type { I, O, T, S, Z, J, L }

        public BitArray rotation;
        
        public Rectangle[] Shape { get; set; }

        public Color Colour;

        public Type BlockType;
        
        //FIXME: better variable naming

        public Rectangle[] _r;
        public Vector2[][] _v;
        ////////////////////////

        public Tetromino(Type type)
        {
            BlockType=type;
            rotation = new BitArray(2);

            Shape = Rectangles[(int)type];
            Colour = Game1.CurrentColorPalette[type];
        }

        public void Rotate(int dir)
        {
            if (dir > 0)
            {
                for (int i = 0; i < dir; i++)
                {
                    rotation[0] = ((rotation[0] ^ false) ^ (rotation[1] && true));
                    rotation[1] = ((rotation[1] ^ true));
                }
            }
            
            if (dir < 0)
            {
                for (int i = 0; i > dir; i--)
                {
                    rotation[0] = ((rotation[0] ^ true) ^ (rotation[1] && true));
                    rotation[1] = ((rotation[1] ^ true));
                }
            }


        }

        public void Update(int x, int y, SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            if (BlockType != Type.O)
            {

                _v = new Vector2[Shape.Length][];
                _r = new Rectangle[Shape.Length];
                for (int index = 0; index < Shape.Length; index++)
                {
                    Vector2[] _vertices = new Vector2[4];

                    int i = 0;
                    foreach (Vector2 v in Rectangle.RectToVertices(Shape[index]))
                    {

                        int _x = (int) v.X;
                        int _y = (int) v.Y;

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



                        int[] currentTransform = Transforms[(int) BlockType];
                        if (BlockType != Type.I)
                        {
                            _x -= SquareSize * ((rotation[0] ^ rotation[1]) ? 1 : 0);
                            _y -= SquareSize * ((rotation[0]) ? 1 : 0);


                        }


                        // set this vertex in the array
                        _vertices[i] = new Vector2(_x, _y);
                        i++;

                        


                    }

                    // draw
                    _v[index] = _vertices;
                    _r[index] = Rectangle.VerticesToRect(_vertices);

                }


            }
            else
            {
                _r = Rectangles[(int) BlockType];
                _v = new Vector2[1][];
                _v[0] = Rectangle.RectToVertices(_r[0]);
            }
        }

        public void Draw(int x, int y, Color[] grid)
        {

            //Rectangle.Draw(r.X+x,r.Y+y,r.Width,r.Height, Colour, _spriteBatch, _graphics);
            
            List<Vector2> squares = new List<Vector2>();
            foreach (Rectangle r in _r)
            {
                squares.AddRange(Rectangle.RectToSquares(r,Tetromino.SquareSize));
            }
            
            
            foreach (Vector2 square in squares)
            {
                grid[((int)((x + square.X)/Tetromino.SquareSize)-1) + (((int)((y + square.Y)/Tetromino.SquareSize))*14)] = Game1.CurrentColorPalette[BlockType];
            }
                
            
        }
    }
}