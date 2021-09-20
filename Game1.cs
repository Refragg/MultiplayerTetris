using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace MultiplayerTetris
{
    public class Game1 : Game
    {



        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _spriteFont;

        //private Rectangle blobby_guy;


        public const int DisplayOffsetX = 175;
        public const int DisplayOffsetY = 75;

        public const int GridOffsetX = 25;
        public const int GridOffsetY = 25;
        public const int GridSquareSize = 25;
        public const int GridWidth = 10;      //250
        public const int GridHeight = 24;     //600
        
        public int xSpawnPosition = 75;
        public int ySpawnPosition = -25;

        private Tetromino.Type bufferedPiece;
        private bool swapped = false;

        private Color[] bufferedPieceGrid;
        private Texture2D bufferedPieceTexture;

        private int bufferedPieceX = -125;
        private int bufferedPieceY = 25;

        private bool firstBuffer;

        private Color[] backgroundGrid;
        private Texture2D backgroundTexture;
        

        private readonly static int _tetrominoTypeCount = typeof(Tetromino.Type).GetEnumValues().Length;

        private Queue<Tetromino.Type> _pieces;

        private Tetromino currentPiece;

        private Random _r;

        public static TetrominoColorPalette CurrentColorPalette;

        private FrameCounter _frameCounter;

        private BitArray rotation;

        private Inputs inputHandler;

        private int x_pos;
        private int y_pos;

        private int grid_l;
        private int grid_r;

        private Color[] gameGrid;
        private Texture2D gameTexture;
        private Texture2D gameTexture2;

        private Color[] phantomDropGrid;
        private Texture2D phantomDropTexture;
        private int phantomX;
        private int phantomY;

        private Color[] nextPiecesGrid;
        private Texture2D nextPiecesTexture;

        private const int nextPiecesX = 325;
        private const int nextPiecesY = 25;

        private const int nextPiecesWidth = 100;
        private const int nextPiecesHeight = 625;

        private Grid displayGrid;

        private Matrix Scale;

        private float gravity;
        private float realPosY;
        private float gravityMult;
        
        private SoundEffect SE_PlaceBlock;
        private SoundEffect SE_ClearRow;
        private SoundEffect SE_FastScroll;

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
            _graphics.PreferredBackBufferWidth = 650;
            _graphics.PreferredBackBufferHeight = 800;

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
            currentPiece.Update();

            _spriteFont = Content.Load<SpriteFont>(Path.Combine("font", "DebugFont"));


            rotation = new BitArray(2);
            rotation[0] = false;
            rotation[1] = false;

            x_pos = xSpawnPosition + GridOffsetX;
            y_pos = ySpawnPosition + GridOffsetY;
            

            grid_l = GridOffsetX;
            grid_r = GridOffsetX + GridWidth * GridSquareSize;

            _r = new Random();

            _pieces = new Queue<Tetromino.Type>();
            
            for (int i = 0; i < 5; i++)
            {
                _pieces.Enqueue((Tetromino.Type)_r.Next(_tetrominoTypeCount));
            }

            
            gameGrid = new Color[GridWidth*GridHeight];

            gameTexture = new Texture2D(_graphics.GraphicsDevice, GridWidth, GridHeight);
            gameTexture.SetData(gameGrid);
            
            gameTexture2 = new Texture2D(_graphics.GraphicsDevice, GridWidth, GridHeight);
            gameTexture2.SetData(gameGrid);


            
            nextPiecesTexture = new Texture2D(_graphics.GraphicsDevice,nextPiecesWidth/GridSquareSize,nextPiecesHeight/GridSquareSize);
            nextPiecesGrid = new Color[(nextPiecesTexture.Width*nextPiecesTexture.Height)];
            nextPiecesTexture.SetData(nextPiecesGrid);

            phantomDropTexture = new Texture2D(_graphics.GraphicsDevice, 4,4);
            
            List<Vector2> squares = new List<Vector2>();

            foreach (Rectangle r in currentPiece._r)
            {
                squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
            }
            
            phantomX = x_pos;
            phantomY = y_pos;
            
            UpdatePhantom();

            
            Vector3 scalingFactor = new Vector3(GridSquareSize, GridSquareSize, 1);
            Scale = Matrix.CreateScale(scalingFactor);

            
            _frameCounter = new FrameCounter();

            inputHandler = new Inputs();
            
            //int x, int y, int width, int height, int x_amount, int y_amount, Color color, int thickness, GraphicsDeviceManager _graphics
            
            displayGrid = new Grid(GridOffsetX+DisplayOffsetX,GridOffsetY+GridSquareSize*4+DisplayOffsetY,
                
                GridSquareSize,GridSquareSize,
                
                GridWidth,
                GridHeight-4,
                
                Color.Gray,1,_graphics);


            gravity = (2*GridSquareSize)/60f;
            realPosY = 0f;

            gravityMult = 1f;


            UpdateNextPiecesDisplay();

            SE_PlaceBlock = SoundEffect.FromFile(Path.Combine("Content", "se", "place_block.wav"));
            SE_ClearRow = SoundEffect.FromFile(Path.Combine("Content", "se", "clear_row.wav"));
            SE_FastScroll = SoundEffect.FromFile(Path.Combine("Content", "se", "fast_scroll.wav"));

            firstBuffer = true;
            bufferedPiece = Tetromino.Type.I;//(Tetromino.Type) _r.Next(_tetrominoTypeCount);

            bufferedPieceGrid = new Color[8];
            bufferedPieceTexture = new Texture2D(_graphics.GraphicsDevice,4,2);
            


            int screenGridWidth = _graphics.PreferredBackBufferWidth/GridSquareSize;
            int screenGridHeight= _graphics.PreferredBackBufferHeight/GridSquareSize;
            backgroundGrid = new Color[screenGridWidth*screenGridHeight];
            backgroundTexture = new Texture2D(_graphics.GraphicsDevice,screenGridWidth,screenGridHeight);

            int xOffset = bufferedPieceX + DisplayOffsetX - GridSquareSize;
            int yOffset = bufferedPieceY + DisplayOffsetY - GridSquareSize;

            xOffset /= GridSquareSize;
            yOffset /= GridSquareSize;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (!((i == 0 && j == 0) || (i == 5 && j == 4) || (i == 5 && j == 0) || (i == 0 && j == 4)))
                    {
                        backgroundGrid[(xOffset + i) + (yOffset + j)*screenGridWidth] = (i>0 && i<5 && j>0 && j<4)?new Color(30,30,30):new Color(50,50,50);
                    }

                }
            }
            
            xOffset = nextPiecesX + DisplayOffsetX - GridSquareSize;
            yOffset = nextPiecesY + DisplayOffsetY - GridSquareSize;
            
            xOffset /= GridSquareSize;
            yOffset /= GridSquareSize;
            
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    if (!((i == 0 && j == 0) || (i == 5 && j == 25) || (i == 5 && j == 0) || (i == 0 && j == 25)))
                    {
                        backgroundGrid[(xOffset + i) + (yOffset + j)*screenGridWidth] = (i>0 && i<5 && j>0 && j%5!=0)?new Color(30,30,30):new Color(50,50,50);
                    }

                }
            }
            
            
            backgroundTexture.SetData(backgroundGrid);
            
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


        private struct KickResult
        {
            public bool Succeeded;
            public Vector2 Result;

            public KickResult(bool succeeded, Vector2 result)
            {
                Succeeded = succeeded;
                Result = result;
            }

            public KickResult(bool succeeded)
            {
                Succeeded = succeeded;
                Result = new Vector2();
            }
        }
        


        private KickResult CheckKick(int rotationDir)
        {

            BitArray lastRotation = new BitArray(2);

            lastRotation[0] = currentPiece.rotation[0];
            lastRotation[1] = currentPiece.rotation[1];
            

            currentPiece.Rotate(rotationDir);
            currentPiece.Update();


            BitArray newRotation = currentPiece.rotation;

            List<Vector2> rotatedSquares = new List<Vector2>();
            foreach (Rectangle r in currentPiece._r)
            {
                rotatedSquares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
            }
            
            Vector2[] kickTransforms = Kick.GetWallKick(currentPiece.BlockType, lastRotation, newRotation);

            foreach (Vector2 transform in kickTransforms)
            {
                bool possible = true;
                
                foreach (Vector2 sq in rotatedSquares)
                {
                    int xCheck = (int) ((sq.X + x_pos ) / GridSquareSize + transform.X) -1;
                    int yCheck = (int) ((sq.Y + y_pos ) / GridSquareSize + transform.Y);
                    yCheck *= GridWidth;
                    
                    
                    if (xCheck>GridWidth-1 || xCheck<0 || yCheck<0 || yCheck>GridWidth*(GridHeight-1))
                    {
                        possible = false;
                        break;
                    }
                    
                    if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                    {
                        possible = false;
                        break;
                    }
                    
                }

                if (possible){
                    return new KickResult(true, transform);
                }
                
            }

            currentPiece.rotation = lastRotation;
            currentPiece.Update();
            return new KickResult(false);

        }


        private void Destroy()
        {

                List<int> toDestroy = new List<int>();
            
                for (int j = 0; j < GridWidth*GridHeight; j+=GridWidth)
                {

                    bool row = true;
                
                    for (int i = 0; i < GridWidth; i++)
                    {
                        if (gameGrid[j + i] == new Color(0, 0, 0, 0)) row = false;
                    }

                    if (row)
                    {
                        toDestroy.Add(j);
                    }
                
                }
            
                foreach(int d in toDestroy)
                {
                    for (int i = 0; i < GridWidth; i++)
                    {
                        gameGrid[d + i] = new Color(0,0,0,0);
                    }
                }



                int downwards = toDestroy.Count;

                if (downwards != 0)
                {
                
                    Color[] newGrid = new Color[GridWidth*GridHeight];
                    gameGrid.CopyTo(newGrid,0);
                
                    for (int j = 0; j < GridWidth*GridHeight; j += GridWidth)
                    {
                    
                        for (int i = 0; i < GridWidth; i++)
                        {
                            if (j + (downwards * GridWidth) + i < GridWidth*GridHeight)
                            {
                                newGrid[j + (downwards * GridWidth) + i] = gameGrid[j+i];
                            }
                            
                        }

                        if (toDestroy.Contains(j)) downwards--;

                    }
                
                    newGrid.CopyTo(gameGrid,0);

                    // play clear_row
                    SE_ClearRow.Play();

                }
            
        }


        private void UpdatePhantom()
        {
            List<Vector2> squares = new List<Vector2>();

            foreach (Rectangle r in currentPiece._r)
            {
                squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
            }

            phantomDropGrid = new Color[16];
            foreach (Vector2 square in squares)
            {
                int x = (int) (square.X / GridSquareSize);
                int y = (int) (square.Y / GridSquareSize);

                phantomDropGrid[x + y * 4] = new Color(120,120,120,100);

            }
                
            phantomDropTexture.SetData(phantomDropGrid);


            phantomX = x_pos;
            

            
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

            int minimumMove = GridHeight;
            for (int j = 0; j < lowestSquares.Count; j++)
            {
                
                int highestInLine = GridHeight-1;

                for (int k = ((int)(lowestSquares[j].Y+y_pos)/GridSquareSize); k < GridHeight; k++)
                {
                    int xCheck = (int)(lowestSquares[j].X+x_pos)/GridSquareSize -1;
                    int yCheck = GridWidth * k;
                    

                    if (gameGrid[xCheck+yCheck] != new Color(0,0,0,0))
                    {
                        highestInLine = k-1;
                        break;
                    }
                }

                int gridY = (int) lowestSquares[j].Y + y_pos;
                gridY /= GridSquareSize;
                

                minimumMove = Math.Min(minimumMove,highestInLine - gridY);

            }

            minimumMove++;
            phantomY = y_pos + minimumMove * GridSquareSize;


        }

        private void UpdateBufferedPiece()
        {
            if (!firstBuffer)
            {
                bufferedPieceGrid = new Color[8];
            
            
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in Tetromino.Rectangles[(int)bufferedPiece])
                {
                    squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
                }

                int yOffset = 0;

                if (bufferedPiece == Tetromino.Type.O) yOffset = -1;
                
                foreach (Vector2 sq in squares)
                {
                    int x = (int) sq.X / GridSquareSize;
                    int y = ((int) sq.Y / GridSquareSize) + yOffset;

                    bufferedPieceGrid[x + y * nextPiecesTexture.Width] = (swapped)?Color.Gray:CurrentColorPalette[bufferedPiece];
                }
            
            
                bufferedPieceTexture.SetData(bufferedPieceGrid);
            }

        }


        private void UpdateNextPiecesDisplay()
        {


            nextPiecesGrid = new Color[(nextPiecesTexture.Width*nextPiecesTexture.Height)];


            List<Vector2> squares;

            int x = 0;
            int y = 0;

            bool blocky = false;
            


            foreach (Tetromino.Type piece in _pieces)
            {

                if (!blocky)
                {
                    squares = new List<Vector2>();

                    foreach (Rectangle r in Tetromino.Rectangles[(int)piece])
                    {
                        squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
                    }

                    foreach (Vector2 sq in squares)
                    {
                        x = (int) sq.X / GridSquareSize;
                        int _y = y + ((int) sq.Y / GridSquareSize);

                        nextPiecesGrid[x + _y * nextPiecesTexture.Width] = CurrentColorPalette[piece];
                    }

                    y+=5;

                }
                else
                {
                    for(int i = 0; i < 4; i++)
                    {
                        x = 0;
                        for(int j = 0; j < 4; j++)
                        {

                            
                            nextPiecesGrid[x + y * nextPiecesTexture.Width] = CurrentColorPalette[piece];

                            x += 1;
                        }
                        y += 1;
                    }
                }

                if (blocky) y += 1;
            }

            nextPiecesTexture.SetData(nextPiecesGrid);
        }



        
        protected override void Draw(GameTime gameTime)
        {


            if ((int) realPosY > GridSquareSize)
            {
                realPosY = 0f;


                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in currentPiece._r)
                {
                    squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
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

                int minimumMove = GridHeight;
                for (int j = 0; j < lowestSquares.Count; j++)
                {
                    
                    int highestInLine = GridHeight-1;

                    for (int k = ((int)(lowestSquares[j].Y+y_pos)/GridSquareSize); k < GridHeight; k++)
                    {
                        int xCheck = (int)(lowestSquares[j].X+x_pos)/GridSquareSize -1;
                        int yCheck = GridWidth * k;

                        if (xCheck + yCheck >= gameGrid.Length)
                        {
                            break;
                        }
                        if (gameGrid[xCheck+yCheck] != new Color(0,0,0,0))
                        {
                            highestInLine = k-1;
                            break;
                        }
                    }

                    int gridY = (int) lowestSquares[j].Y + y_pos;
                    gridY /= GridSquareSize;
                    

                    minimumMove = Math.Min(minimumMove,highestInLine - gridY);

                }

                if (minimumMove>0)
                {
                    y_pos += GridSquareSize;
                    if (gravityMult>1f)
                    {
                        // play fast_scroll
                        SE_FastScroll.Play();
                    }
                }
                else
                {
                    foreach (Vector2 square in squares)
                    {
                        gameGrid[((int)((square.X+x_pos)/GridSquareSize)-1) + ((((int)((square.Y+y_pos)/GridSquareSize)))*GridWidth)] = CurrentColorPalette[currentPiece.BlockType];
                    }



                    x_pos = xSpawnPosition + GridOffsetX;
                    y_pos = ySpawnPosition + GridOffsetY;
                    
                    int next = (int) currentPiece.BlockType + 1;
                    if (next > 6) next = 0;
                    
                    currentPiece = new Tetromino(_pieces.Dequeue());
                    _pieces.Enqueue((Tetromino.Type)_r.Next(_tetrominoTypeCount));
                    currentPiece.Update();


                    // update display of next pieces
                    UpdateNextPiecesDisplay();
                    swapped = false;
                    UpdateBufferedPiece();

                    Destroy();

                    UpdatePhantom();
                }




            }
            
            


            // TODO: optimise display of tetrominos - currently creates new rectangles every frame which is very inefficient
            // Edit: kinda fixed maybe

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            

            GraphicsDevice.Clear(Color.Black);

            
            Color[] tempGrid = new Color[GridWidth*GridHeight];

            
            
            _spriteBatch.Begin();

            displayGrid.Draw(_spriteBatch,_graphics);

            _spriteBatch.DrawString(_spriteFont, _frameCounter.CurrentFramesPerSecond.ToString(), Vector2.Zero, Color.White);

            _spriteBatch.End();
            
            
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null,null, Scale);
            
            
            currentPiece.Draw(x_pos,y_pos, tempGrid);
            gameTexture2.SetData(tempGrid);
            gameTexture.SetData(gameGrid);


            _spriteBatch.Draw(gameTexture, 
                new Vector2(1+DisplayOffsetX/GridSquareSize,1+DisplayOffsetY/GridSquareSize), 
                Color.White);
            
      
            _spriteBatch.Draw(gameTexture2, 
                new Vector2(1+DisplayOffsetX/GridSquareSize,1+DisplayOffsetY/GridSquareSize), 
                Color.White);
            
            
            
            _spriteBatch.Draw(backgroundTexture,new Vector2(), Color.White);
            
            
            
            //phantomDropTexture.SetData(phantomDropGrid);
            _spriteBatch.Draw(phantomDropTexture, 
                new Vector2((phantomX+DisplayOffsetX)/GridSquareSize,(phantomY+DisplayOffsetY)/GridSquareSize), 
                Color.White);

            _spriteBatch.Draw(bufferedPieceTexture, 
                new Vector2((bufferedPieceX+DisplayOffsetX)/GridSquareSize,(bufferedPieceY+DisplayOffsetY)/GridSquareSize), 
                Color.White);
            
            
            _spriteBatch.Draw(nextPiecesTexture,
                new Vector2((nextPiecesX+DisplayOffsetX) / GridSquareSize, (nextPiecesY+DisplayOffsetY) / GridSquareSize),
                Color.White);

            _spriteBatch.End();
            

            
            

            base.Draw(gameTime);

            

            if (inputHandler.KeyPressed(Keys.LeftShift) && !swapped)
            {
                swapped = true;
                Tetromino.Type temp = currentPiece.BlockType;
                //currentPiece.BlockType = bufferedPiece;
                currentPiece = new Tetromino(bufferedPiece);
                bufferedPiece = temp;
                
                x_pos = xSpawnPosition + GridOffsetX;
                y_pos = ySpawnPosition + GridOffsetY;
                
                if (firstBuffer)
                {
                    firstBuffer = false;
                    currentPiece = new Tetromino(_pieces.Dequeue());
                    _pieces.Enqueue((Tetromino.Type)_r.Next(_tetrominoTypeCount));
                }
                
                UpdateBufferedPiece();

                currentPiece.Update();
                UpdateNextPiecesDisplay();
                UpdatePhantom();
                
            }
            //swapped = false;
            

            if (inputHandler.KeyPressed(Keys.Right))
            {

                KickResult result = CheckKick(1);
                if (result.Succeeded)
                {
                    x_pos += (int)(GridSquareSize * result.Result.X);
                    y_pos += (int)(GridSquareSize * result.Result.Y);
                }
                
                UpdatePhantom();
                
            }
            if (inputHandler.KeyPressed(Keys.Left))
            {
                KickResult result = CheckKick(-1);
                if (result.Succeeded)
                {
                    x_pos += (int)(GridSquareSize * result.Result.X);
                    y_pos += (int)(GridSquareSize * result.Result.Y);
                }
                
                UpdatePhantom();
                
            }

            if (inputHandler.TimedPress(Keys.D,5,15))
            {
            
                SE_FastScroll.Play();
                
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in currentPiece._r)
                {
                    squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
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
                    int xCheck = (int)(lowestSquares[j].X + x_pos )/GridSquareSize;
                    int yCheck = (int)((lowestSquares[j].Y + y_pos )/GridSquareSize) * GridWidth;

                    if (xCheck + yCheck >= gameGrid.Length)
                    {
                        canMove *= 0;
                        break;
                    }
                    
                    if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                    {
                        canMove *= 0;
                    }

                    
                }
                
                
                x_pos += GridSquareSize*canMove;
                MoveCheck(grid_l, grid_r);
                
                
                UpdatePhantom();
                
            }
            
            if (inputHandler.TimedPress(Keys.A,5,15))
            {
                
                SE_FastScroll.Play();
                
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in currentPiece._r)
                {
                    squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
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
                    int xCheck = (int)(lowestSquares[j].X + x_pos )/GridSquareSize -2;
                    int yCheck = (int)((lowestSquares[j].Y + y_pos)/GridSquareSize) * GridWidth;


                    if (xCheck < 0)
                    {
                        canMove *= 0;
                        break;
                    }
                    
                    if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                    {
                        canMove *= 0;
                    }


                    
                }
                
                x_pos -= GridSquareSize * canMove;
                MoveCheck(grid_l, grid_r);
                
                
                UpdatePhantom();
                
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                gravityMult = 10f;
            }
            else
            {
                gravityMult = 1f;
            }
            
            
            if (inputHandler.KeyPressed(Keys.S))
            {
                SE_PlaceBlock.Play();
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in currentPiece._r)
                {
                    squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
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

                int minimumMove = GridHeight;
                for (int j = 0; j < lowestSquares.Count; j++)
                {
                    
                    int highestInLine = GridHeight-1;

                    for (int k = ((int)(lowestSquares[j].Y+y_pos)/GridSquareSize); k < GridHeight; k++)
                    {
                        int xCheck = (int)(lowestSquares[j].X+x_pos)/GridSquareSize -1;
                        int yCheck = GridWidth * k;
                        

                        if (gameGrid[xCheck+yCheck] != new Color(0,0,0,0))
                        {
                            highestInLine = k-1;
                            break;
                        }
                    }

                    int gridY = (int) lowestSquares[j].Y + y_pos;
                    gridY /= GridSquareSize;
                    

                    minimumMove = Math.Min(minimumMove,highestInLine - gridY);

                }


                y_pos += GridSquareSize * minimumMove;


                foreach (Vector2 square in squares)
                {
                    gameGrid[((int)((square.X+x_pos)/GridSquareSize)-1) + ((((int)((square.Y+y_pos)/GridSquareSize)))*GridWidth)] = CurrentColorPalette[currentPiece.BlockType];
                }



                x_pos = xSpawnPosition + GridOffsetX;
                y_pos = ySpawnPosition + GridOffsetY;
                
                int next = (int) currentPiece.BlockType + 1;
                if (next > 6) next = 0;
                
                currentPiece = new Tetromino(_pieces.Dequeue());
                _pieces.Enqueue((Tetromino.Type)_r.Next(_tetrominoTypeCount));
                currentPiece.Update();

                // update display of next pieces
                UpdateNextPiecesDisplay();
                
                swapped = false;
                UpdateBufferedPiece();

                Destroy();
                
                UpdatePhantom();
            }

                
            realPosY+=gravity*gravityMult;
            
        }
    }
}
