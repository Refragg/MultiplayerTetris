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



        // changeable vars //

        private const int screenWidth = 650;
        private const int screenHeight = 800;

        public const int GridSquareSize = 14;

        public const int GridHeight = 24;
        public const int GridWidth = 20;

        private const int nextPiecesAmount = 5;

        private const int numPlayers = 1;

        // // // // // // //


        // calculated vars // 
        public const int DisplayOffsetX = GridSquareSize * ((screenWidth/GridSquareSize - GridWidth) / 2 -1);
        public const int DisplayOffsetY = GridSquareSize * ((screenHeight / GridSquareSize - GridHeight) / 2 - 1);

        public const int GridOffsetX = GridSquareSize;
        public const int GridOffsetY = GridSquareSize;

        public int xSpawnPosition = 3* GridSquareSize;
        public int ySpawnPosition = -1* GridSquareSize;

        private int xSpawnOffset = (GridWidth / numPlayers) * GridSquareSize;

        private int bufferedPieceX = -5* GridSquareSize;
        private int bufferedPieceY = GridSquareSize;

        private const int nextPiecesX = (GridWidth + 3) * GridSquareSize;
        private const int nextPiecesY = GridSquareSize;

        private const int nextPiecesWidth = 4* GridSquareSize;
        private const int nextPiecesHeight = 5* nextPiecesAmount * GridSquareSize;

        private Tetromino[] currentPieces = new Tetromino[numPlayers];

        private int[][] positions = new int[numPlayers][];

        private float[] realPositionsY = new float[numPlayers];
        // // // // // // //



        private Tetromino.Type bufferedPiece;
        private bool swapped = false;

        private Color[] bufferedPieceGrid;
        private Texture2D bufferedPieceTexture;

        private bool firstBuffer;

        private Color[] backgroundGrid;
        private Texture2D backgroundTexture;
        

        private readonly static int _tetrominoTypeCount = typeof(Tetromino.Type).GetEnumValues().Length;

        private Queue<Tetromino.Type> _pieces;

        private Random _r;

        public static TetrominoColorPalette CurrentColorPalette;

        private FrameCounter _frameCounter;

        private BitArray rotation;

        private Inputs inputHandler;

        private PlayerControllerManager _playerControllerManager;

        //private int x_pos;
        //private int y_pos;

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

        private Grid displayGrid;

        private Matrix Scale;

        private float gravity;
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
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;

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


            for (int i = 0; i < numPlayers; i++)
            {
                positions[i] = new int[]
                {
                    GridOffsetX + xSpawnPosition + xSpawnOffset*i,
                    GridOffsetY + ySpawnPosition
                };

            }


            _spriteFont = Content.Load<SpriteFont>(Path.Combine("font", "DebugFont"));


            rotation = new BitArray(2);
            rotation[0] = false;
            rotation[1] = false;

            //x_pos = xSpawnPosition + GridOffsetX;
            //y_pos = ySpawnPosition + GridOffsetY;
            

            grid_l = GridOffsetX;
            grid_r = GridOffsetX + GridWidth * GridSquareSize;

            _r = new Random();

            _pieces = new Queue<Tetromino.Type>();
            
            for (int i = 0; i < nextPiecesAmount; i++)
            {
                _pieces.Enqueue((Tetromino.Type)_r.Next(_tetrominoTypeCount));
            }

            for (int i = 0; i < currentPieces.Length; i++)
            {
                currentPieces[i] = new Tetromino((Tetromino.Type)_r.Next(_tetrominoTypeCount));
                currentPieces[i].Update();
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

            // TODO: phantom pieces, not just 1 phantom piece
            //
            // for (int i = 0; i < currentPieces.Length; i++)
            //
            foreach (Rectangle r in currentPieces[0]._r)
            {
                squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
            }
            
            // ++ loop
            phantomX = positions[0][0];
            phantomY = positions[0][1];
            
            // ++ loop
            UpdatePhantom(0);

            
            Vector3 scalingFactor = new Vector3(GridSquareSize, GridSquareSize, 1);
            Scale = Matrix.CreateScale(scalingFactor);

            
            _frameCounter = new FrameCounter();

            inputHandler = new Inputs();

            _playerControllerManager = new PlayerControllerManager();
            
            //int x, int y, int width, int height, int x_amount, int y_amount, Color color, int thickness, GraphicsDeviceManager _graphics
            
            displayGrid = new Grid(GridOffsetX+DisplayOffsetX,GridOffsetY+GridSquareSize*4+DisplayOffsetY,
                
                GridSquareSize,GridSquareSize,
                
                GridWidth,
                GridHeight-4,
                
                Color.Gray,1,_graphics);


            gravity = (2*GridSquareSize)/60f;
            for (int i = 0; i < numPlayers; i++)
            {
                realPositionsY[i] = 0f;
            }

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
                for (int j = 0; j < 5* nextPiecesAmount +1; j++)
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
        
        private void MoveCheck(int minX, int maxX, int currentPieceIndex)
        {

            Tetromino currentPiece = currentPieces[currentPieceIndex];

            List<Vector2> vertices = new List<Vector2>();
            foreach (Vector2[] v in currentPiece._v) { vertices.AddRange(v); }
            
            int left = (int) vertices.Min(v => v.X);
            int right = (int) vertices.Max(v => v.X);
            
            
            if (positions[currentPieceIndex][0] +left < minX)
            {
                positions[currentPieceIndex][0] = minX - left;
            }
            
            if (positions[currentPieceIndex][0] + right > maxX)
            {
                positions[currentPieceIndex][0] = maxX - right;
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
        


        private KickResult CheckKick(int rotationDir, int currentPieceIndex)
        {

            Tetromino currentPiece = currentPieces[currentPieceIndex];

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
                    int xCheck = (int) ((sq.X + positions[currentPieceIndex][0]) / GridSquareSize + transform.X) -1;
                    int yCheck = (int) ((sq.Y + positions[currentPieceIndex][0]) / GridSquareSize + transform.Y);
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


        private void UpdatePhantom(int currentPieceIndex)
        {

            Tetromino currentPiece = currentPieces[currentPieceIndex];

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


            phantomX = positions[currentPieceIndex][0];
            

            
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

                for (int k = ((int)(lowestSquares[j].Y+ positions[currentPieceIndex][1]) /GridSquareSize); k < GridHeight; k++)
                {
                    int xCheck = (int)(lowestSquares[j].X+ positions[currentPieceIndex][0]) /GridSquareSize -1;
                    int yCheck = GridWidth * k;
                    

                    if (gameGrid[xCheck+yCheck] != new Color(0,0,0,0))
                    {
                        highestInLine = k-1;
                        break;
                    }
                }

                int gridY = (int) lowestSquares[j].Y + positions[currentPieceIndex][1];
                gridY /= GridSquareSize;
                

                minimumMove = Math.Min(minimumMove,highestInLine - gridY);

            }

            minimumMove++;
            phantomY = positions[currentPieceIndex][1] + minimumMove * GridSquareSize;


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



        /*
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         *  here's a fat amount of comment lines so i can find the draw method
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         *  i keep missing it when i scroll through the program
         * 
         * 
         * 
         * 
         * 
         * 
        */

        
        protected override void Draw(GameTime gameTime)
        {


            for (int currentPieceIndex = 0; currentPieceIndex < currentPieces.Length; currentPieceIndex++)
            {
                if ((int)realPositionsY[currentPieceIndex] > GridSquareSize)
                {
                    realPositionsY[currentPieceIndex] = 0f;


                    List<Vector2> squares = new List<Vector2>();

                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                    }

                    List<Vector2> lowestSquares = new List<Vector2>();

                    for (int i = 0; i < squares.Count; i++)
                    {

                        int index = lowestSquares.FindIndex(v => (int)v.X == (int)squares[i].X);

                        if (index == -1)
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

                        int highestInLine = GridHeight - 1;

                        for (int k = ((int)(lowestSquares[j].Y + positions[currentPieceIndex][1]) / GridSquareSize); k < GridHeight; k++)
                        {
                            int xCheck = (int)(lowestSquares[j].X + positions[currentPieceIndex][0]) / GridSquareSize - 1;
                            int yCheck = GridWidth * k;

                            if (xCheck + yCheck >= gameGrid.Length)
                            {
                                break;
                            }
                            if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                            {
                                highestInLine = k - 1;
                                break;
                            }
                        }

                        int gridY = (int)lowestSquares[j].Y + positions[currentPieceIndex][1];
                        gridY /= GridSquareSize;


                        minimumMove = Math.Min(minimumMove, highestInLine - gridY);

                    }

                    if (minimumMove > 0)
                    {
                        // TODO: position looping again
                        positions[currentPieceIndex][1] += GridSquareSize;
                        if (gravityMult > 1f)
                        {
                            // play fast_scroll
                            SE_FastScroll.Play();
                        }
                    }
                    else
                    {
                        foreach (Vector2 square in squares)
                        {
                            // TODO: position looping again again
                            gameGrid[((int)((square.X + positions[currentPieceIndex][0]) / GridSquareSize) - 1) + ((((int)((square.Y + positions[currentPieceIndex][1]) / GridSquareSize))) * GridWidth)] = CurrentColorPalette[currentPieces[currentPieceIndex].BlockType];
                        }



                        positions[currentPieceIndex][0] = xSpawnPosition + GridOffsetX;
                        positions[currentPieceIndex][1] = ySpawnPosition + GridOffsetY;

                        //int next = (int)currentPieces[0].BlockType + 1;
                        //if (next > 6) next = 0;

                        if (currentPieceIndex == 0)
                        {
                            currentPieces[0] = new Tetromino(_pieces.Dequeue());
                            _pieces.Enqueue((Tetromino.Type)_r.Next(_tetrominoTypeCount));
                        }


                        currentPieces[currentPieceIndex].Update();
                        

                        // update display of next pieces
                        UpdateNextPiecesDisplay();
                        swapped = false;
                        UpdateBufferedPiece();

                        Destroy();

                        // TODO: loop instead of using 0
                        UpdatePhantom(0);
                    }




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

            for (int i = 0; i < currentPieces.Length; i++)
            {
                currentPieces[i].Draw(positions[i][0], positions[i][1], tempGrid);
                    gameTexture2.SetData(tempGrid);
                    gameTexture.SetData(gameGrid);
            }



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
            
            if (inputHandler.KeyPressed(_playerControllerManager.GetControl(0, Controls.Hold)) && !swapped)
            {

                // TODO: add n amount of bufferedPieces and loop through
                //
                // for (int i = 0; i < currentPieces.Length; i++)
                //

                swapped = true;
                Tetromino.Type temp = currentPieces[0].BlockType;
                //currentPiece.BlockType = bufferedPiece;
                currentPieces[0] = new Tetromino(bufferedPiece);
                bufferedPiece = temp;

                // TODO: loop first index
                positions[0][0] = xSpawnPosition + GridOffsetX;
                positions[0][1] = ySpawnPosition + GridOffsetY;
                
                if (firstBuffer)
                {
                    firstBuffer = false;
                    currentPieces[0] = new Tetromino(_pieces.Dequeue());
                    _pieces.Enqueue((Tetromino.Type)_r.Next(_tetrominoTypeCount));
                }
                
                UpdateBufferedPiece();

                currentPieces[0].Update();
                UpdateNextPiecesDisplay();
                // TODO: loop instead of using 0
                UpdatePhantom(0);
                
            }
            //swapped = false;
            

            if (inputHandler.KeyPressed(_playerControllerManager.GetControl(0, Controls.RotateRight)))
            {

                for (int i = 0; i < currentPieces.Length; i++)
                {
                    KickResult result = CheckKick(1, i);
                    if (result.Succeeded)
                    {
                        positions[i][0] += (int)(GridSquareSize * result.Result.X);
                        positions[i][1] += (int)(GridSquareSize * result.Result.Y);
                    }
                }

                
                // TODO: loop instead of using 0
                UpdatePhantom(0);
                
            }
            if (inputHandler.KeyPressed(_playerControllerManager.GetControl(0, Controls.RotateLeft)))
            {

                for (int i = 0; i < currentPieces.Length; i++)
                {
                    KickResult result = CheckKick(-1, i);
                    if (result.Succeeded)
                    {
                        positions[i][0] += (int)(GridSquareSize * result.Result.X);
                        positions[i][1] += (int)(GridSquareSize * result.Result.Y);
                    }
                }

                // ^+
                UpdatePhantom(0);
                
            }

            if (inputHandler.TimedPress(_playerControllerManager.GetControl(0, Controls.MoveRight),5,15))
            {

                for (int currentPieceIndex = 0; currentPieceIndex < currentPieces.Length; currentPieceIndex++)
                {

                    SE_FastScroll.Play();

                    List<Vector2> squares = new List<Vector2>();

                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                    }

                    List<Vector2> lowestSquares = new List<Vector2>();

                    for (int i = 0; i < squares.Count; i++)
                    {

                        int index = lowestSquares.FindIndex(v => (int)v.Y == (int)squares[i].Y);

                        if (index == -1)
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
                        int xCheck = (int)(lowestSquares[j].X + positions[currentPieceIndex][0]) / GridSquareSize;
                        int yCheck = (int)((lowestSquares[j].Y + positions[currentPieceIndex][1]) / GridSquareSize) * GridWidth;

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


                    positions[currentPieceIndex][0] += GridSquareSize * canMove;
                    MoveCheck(grid_l, grid_r, currentPieceIndex);

                }
                
                // ^+
                UpdatePhantom(0);
                
            }
            
            if (inputHandler.TimedPress(_playerControllerManager.GetControl(0, Controls.MoveLeft),5,15))
            {

                for (int currentPieceIndex = 0; currentPieceIndex < currentPieces.Length; currentPieceIndex++)
                {

                    SE_FastScroll.Play();

                    List<Vector2> squares = new List<Vector2>();

                    foreach (Rectangle r in currentPieces[0]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                    }

                    List<Vector2> lowestSquares = new List<Vector2>();

                    for (int i = 0; i < squares.Count; i++)
                    {

                        int index = lowestSquares.FindIndex(v => (int)v.Y == (int)squares[i].Y);

                        if (index == -1)
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
                        int xCheck = (int)(lowestSquares[j].X + positions[currentPieceIndex][0]) / GridSquareSize - 2;
                        int yCheck = (int)((lowestSquares[j].Y + positions[currentPieceIndex][1]) / GridSquareSize) * GridWidth;


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

                    positions[currentPieceIndex][0] -= GridSquareSize * canMove;
                    MoveCheck(grid_l, grid_r, currentPieceIndex);
                }

                // ^+
                UpdatePhantom(0);
                
            }

            if (Keyboard.GetState().IsKeyDown(_playerControllerManager.GetControl(0, Controls.SoftDrop)))
            {
                gravityMult = 10f;
            }
            else
            {
                gravityMult = 1f;
            }
            
            
            if (inputHandler.KeyPressed(_playerControllerManager.GetControl(0, Controls.HardDrop)))
            {

                for (int currentPieceIndex = 0; currentPieceIndex < currentPieces.Length; currentPieceIndex++)
                {

                    SE_PlaceBlock.Play();
                    List<Vector2> squares = new List<Vector2>();

                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                    }

                    List<Vector2> lowestSquares = new List<Vector2>();

                    for (int i = 0; i < squares.Count; i++)
                    {

                        int index = lowestSquares.FindIndex(v => (int)v.X == (int)squares[i].X);

                        if (index == -1)
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

                        int highestInLine = GridHeight - 1;

                        for (int k = ((int)(lowestSquares[j].Y + positions[currentPieceIndex][1]) / GridSquareSize); k < GridHeight; k++)
                        {
                            int xCheck = (int)(lowestSquares[j].X + positions[currentPieceIndex][0]) / GridSquareSize - 1;
                            int yCheck = GridWidth * k;


                            if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                            {
                                highestInLine = k - 1;
                                break;
                            }
                        }

                        int gridY = (int)lowestSquares[j].Y + positions[currentPieceIndex][1];
                        gridY /= GridSquareSize;


                        minimumMove = Math.Min(minimumMove, highestInLine - gridY);

                    }


                    positions[currentPieceIndex][1] += GridSquareSize * minimumMove;


                    foreach (Vector2 square in squares)
                    {
                        gameGrid[((int)((square.X + positions[currentPieceIndex][0]) / GridSquareSize) - 1) + ((((int)((square.Y + positions[currentPieceIndex][1]) / GridSquareSize))) * GridWidth)] = CurrentColorPalette[currentPieces[currentPieceIndex].BlockType];
                    }



                    positions[currentPieceIndex][0] = xSpawnPosition + GridOffsetX + xSpawnOffset*currentPieceIndex;
                    positions[currentPieceIndex][1] = ySpawnPosition + GridOffsetY;

                    //int next = (int)currentPieces[0].BlockType + 1;
                    //if (next > 6) next = 0;

                    if (currentPieceIndex == 0)
                    {
                        // TODO: multiple "next pieces" - 1 per player
                        currentPieces[0] = new Tetromino(_pieces.Dequeue());
                        _pieces.Enqueue((Tetromino.Type)_r.Next(_tetrominoTypeCount));
                    }

                    currentPieces[currentPieceIndex].Update();

                    // update display of next pieces
                    UpdateNextPiecesDisplay();

                    swapped = false;
                    UpdateBufferedPiece();

                    Destroy();

                }

                // ^+
                UpdatePhantom(0);
            }

            for (int i = 0; i < numPlayers; i++)
            {
                realPositionsY[i] += gravity * gravityMult;
            }
            
            
        }
    }
}
