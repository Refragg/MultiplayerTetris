using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerTetris
{
    public class Game1 : Game
    {



        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _spriteFont;

        //private Rectangle blobby_guy;


        private const int GridOffsetX = 100;
        private const int GridOffsetY = 100;
        private const int GridSize = 50;
        private readonly Rectangle[] _gridRectangles =
        {
            new Rectangle(GridOffsetX, GridOffsetY, 1, GridSize * 4),
            new Rectangle(GridOffsetX + (GridSize * 1), GridOffsetY, 1, GridSize * 4),
            new Rectangle(GridOffsetX + (GridSize * 2), GridOffsetY, 1, GridSize * 4),
            new Rectangle(GridOffsetX + (GridSize * 3), GridOffsetY, 1, GridSize * 4),
            new Rectangle(GridOffsetX + (GridSize * 4), GridOffsetY, 1, GridSize * 4),
            
            new Rectangle(GridOffsetX, GridOffsetY, GridSize * 4, 1),
            new Rectangle(GridOffsetX, GridOffsetY + (GridSize * 1), GridSize * 4, 1),
            new Rectangle(GridOffsetX, GridOffsetY + (GridSize * 2), GridSize * 4, 1),
            new Rectangle(GridOffsetX, GridOffsetY + (GridSize * 3), GridSize * 4, 1),
            new Rectangle(GridOffsetX, GridOffsetY + (GridSize * 4), GridSize * 4, 1),
        };

        private Tetromino currentPiece;

        public static TetrominoColorPalette CurrentColorPalette;

        private FrameCounter _frameCounter;

        private BitArray rotation;

        private Inputs inputHandler;

        private int x_pos;
        private int y_pos;

        private Color[] backgroundPixels;
        private Texture2D backgroundTexture;

        private int grid_l;
        private int grid_r;

        private Color[] gameGrid;
        private Texture2D gameTexture;
        private Texture2D gameTexture2;

        private int scaleMult;
        private Matrix Scale;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
            
            
            // TODO: Add your initialization logic here
            CurrentColorPalette = new TetrominoColorPalette(new Color[]
            {
                new Color(49,199,239),      // I
                new Color(247,211,8),       // O
                new Color(173,77,156),      // T
                new Color(66,182,66),       // S
                new Color(239,32,41),       // Z
                new Color(90,101,173),      // J
                new Color(239,121,33)       // L
            });

            currentPiece = new Tetromino(Tetromino.Type.I);
            currentPiece.Update(x_pos, y_pos, _spriteBatch, _graphics);

            _spriteFont = Content.Load<SpriteFont>("Font");


            rotation = new BitArray(2);
            rotation[0] = false;
            rotation[1] = false;

            x_pos = 100;
            y_pos = 100;
            
            scaleMult = 50;

            grid_l = scaleMult;
            grid_r = scaleMult * 15;

            Random r = new Random();
            
            gameGrid = new Color[140];

            gameTexture = new Texture2D(_graphics.GraphicsDevice, 14, 10);
            gameTexture.SetData(gameGrid);
            
            gameTexture2 = new Texture2D(_graphics.GraphicsDevice, 14, 10);
            gameTexture2.SetData(gameGrid);
            
            Vector3 ScalingFactor = new Vector3(50, 50, 1);
            Scale = Matrix.CreateScale(ScalingFactor);

            
            _frameCounter = new FrameCounter();

            inputHandler = new Inputs();
            
            backgroundPixels = new Color[_graphics.PreferredBackBufferWidth * _graphics.PreferredBackBufferHeight];
            backgroundTexture = new Texture2D(_graphics.GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        
        private void MoveCheck(int minX, int maxX)
        {
            List<Vector2> vertices = new List<Vector2>();
            foreach (Vector2[] v in currentPiece._v) { vertices.AddRange(v); }
            
            int left = (int) vertices.Min(v => v.X);
            int right = (int) vertices.Max(v => v.X);
            
            
            if (x_pos+left < minX)
            {
                x_pos = minX - left;
            }
            
            if (x_pos+right > maxX)
            {
                x_pos = maxX - right;
            }
            
        }
        
        protected override void Draw(GameTime gameTime)
        {


            // TODO: optimise display of tetrominos - currently creates new rectangles every frame which is very inefficient
            // Edit: kinda fixed maybe

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);

            
            GraphicsDevice.Clear(Color.Black);

            Color[] tempGrid = new Color[140];
            
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null,null, Scale);
            
            
            currentPiece.Draw(x_pos,y_pos, tempGrid);
            gameTexture2.SetData(tempGrid);
            
            _spriteBatch.Draw(gameTexture2, 
                new Vector2(1,1), 
                Color.White);
            
            _spriteBatch.Draw(gameTexture, 
                new Vector2(1,1), 
                Color.White);

            _spriteBatch.End();
            
            
            _spriteBatch.Begin();
            
            gameTexture.SetData(gameGrid);
            

            Grid.Draw(50-1,50-1,
                
                50-1,50-1,
                
                14,
                10,
                
                Color.Gray, 2, _spriteBatch,_graphics);
            

            _spriteBatch.DrawString(_spriteFont, _frameCounter.CurrentFramesPerSecond.ToString(), Vector2.Zero, Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);

            if (inputHandler.KeyPressed(Keys.Space))
            {
                int next = (int) currentPiece.BlockType + 1;
                if (next > 6) next = 0;
                
                currentPiece = new Tetromino((Tetromino.Type)(next));
                currentPiece.Update(x_pos, y_pos, _spriteBatch, _graphics);
            }
            if (inputHandler.KeyPressed(Keys.Right))
            {
                currentPiece.Rotate(1);
                if (currentPiece.BlockType!=Tetromino.Type.O)
                    currentPiece.Update(x_pos, y_pos, _spriteBatch, _graphics);
                
                MoveCheck(grid_l, grid_r);
            }
            if (inputHandler.KeyPressed(Keys.Left))
            {
                currentPiece.Rotate(-1);
                if (currentPiece.BlockType!=Tetromino.Type.O)
                    currentPiece.Update(x_pos, y_pos, _spriteBatch, _graphics);
                
                MoveCheck(grid_l, grid_r);
            }

            if (inputHandler.TimedPress(Keys.D,5))
            {
                
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in currentPiece._r)
                {
                    squares.AddRange(Rectangle.RectToSquares(r,scaleMult));
                }

                List<Vector2> lowestSquares = new List<Vector2>();
                
                for (int i = 0; i < squares.Count; i++)
                {

                    int index = lowestSquares.FindIndex(v => (int) v.Y == (int) squares[i].Y);

                    if (index==-1)
                    {
                        lowestSquares.Add(squares[i]);
                    }
                    else
                    {
                        lowestSquares[index] = (lowestSquares[index].X < squares[i].X) ? squares[i] : lowestSquares[index];
                    }
                }
                
                int canMove = 1;
                for (int j = 0; j < lowestSquares.Count; j++)
                {
                    int xCheck = (int)(lowestSquares[j].X + x_pos )/50;
                    int yCheck = (int)((lowestSquares[j].Y + y_pos)/50) * 14;


                    if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                    {
                        canMove *= 0;
                    }

                    
                }
                
                
                x_pos += scaleMult*canMove;
                MoveCheck(grid_l, grid_r);
                
                
                
                
            }
            
            if (inputHandler.TimedPress(Keys.A,5))
            {
                
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in currentPiece._r)
                {
                    squares.AddRange(Rectangle.RectToSquares(r,scaleMult));
                }

                List<Vector2> lowestSquares = new List<Vector2>();
                
                for (int i = 0; i < squares.Count; i++)
                {

                    int index = lowestSquares.FindIndex(v => (int) v.Y == (int) squares[i].Y);

                    if (index==-1)
                    {
                        lowestSquares.Add(squares[i]);
                    }
                    else
                    {
                        lowestSquares[index] = (lowestSquares[index].X > squares[i].X) ? squares[i] : lowestSquares[index];
                    }
                }
                
                int canMove = 1;
                for (int j = 0; j < lowestSquares.Count; j++)
                {
                    int xCheck = (int)(lowestSquares[j].X + x_pos )/50 -2;
                    int yCheck = (int)((lowestSquares[j].Y + y_pos)/50) * 14;


                    if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                    {
                        canMove *= 0;
                    }


                    
                }
                
                x_pos -= scaleMult * canMove;
                MoveCheck(grid_l, grid_r);
            }
            
            
            if (inputHandler.KeyPressed(Keys.Down))
            {
                
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in currentPiece._r)
                {
                    squares.AddRange(Rectangle.RectToSquares(r,scaleMult));
                }

                List<Vector2> lowestSquares = new List<Vector2>();

                for (int i = 0; i < squares.Count; i++)
                {

                    int index = lowestSquares.FindIndex(v => (int) v.X == (int) squares[i].X);

                    if (index==-1)
                    {
                        lowestSquares.Add(squares[i]);
                    }
                    else
                    {
                        lowestSquares[index] = (lowestSquares[index].Y < squares[i].Y) ? squares[i] : lowestSquares[index];
                    }
                }

                int minimumMove = 10;
                for (int j = 0; j < lowestSquares.Count; j++)
                {
                    
                    int highestInLine = 9;

                    for (int k = ((int)(lowestSquares[j].Y+y_pos)/scaleMult); k < 10; k++)
                    {
                        int xCheck = (int)(lowestSquares[j].X+x_pos)/scaleMult -1;
                        int yCheck = 14 * k;
                        

                        if (gameGrid[xCheck+yCheck] != new Color(0,0,0,0))
                        {
                            highestInLine = k-1;
                            break;
                        }
                    }

                    int gridY = (int) lowestSquares[j].Y + y_pos;
                    gridY /= 50;
                    

                    minimumMove = Math.Min(minimumMove,highestInLine - gridY);

                }


                y_pos += 50 * minimumMove;


                foreach (Vector2 square in squares)
                {
                    gameGrid[((int)((square.X+x_pos)/scaleMult)-1) + ((((int)((square.Y+y_pos)/scaleMult)))*14)] = CurrentColorPalette[currentPiece.BlockType];
                }



                x_pos = 100;
                y_pos = 100;
                
                int next = (int) currentPiece.BlockType + 1;
                if (next > 6) next = 0;
                
                currentPiece = new Tetromino((Tetromino.Type)(next));
                currentPiece.Update(x_pos, y_pos, _spriteBatch, _graphics);
                
            }

            
            

            
        }
    }
}
