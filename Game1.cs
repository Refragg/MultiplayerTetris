using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        
        
        #region Variables

        #region Monogame
        
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont spriteFont;

        #endregion


        #region User Defined



        private const int NumPlayers = 2;

        private const int ScreenHeight = 800;

        // screen width relative to grid square size
        //public const int GridSquareSize = 18;
        //private const int ScreenWidth = (16 + 10 * NumPlayers) * GridSquareSize;

        // grid square size relative to screen width
        private const int ScreenWidth = 800;
        public const int GridSquareSize = ScreenWidth/(16+10* NumPlayers);


        public const int GridHeight = 24;

        private const int NextPiecesAmount = 5;
        
        private const bool BlockDisplayMode = false;

        private const bool PushUp = false;

        #endregion


        #region Calculated

        public const int GridWidth = 10 * NumPlayers;
        
        private const int DisplayOffsetX = GridSquareSize * ((ScreenWidth/GridSquareSize - GridWidth) / 2 -1);
        private const int DisplayOffsetY = GridSquareSize * ((ScreenHeight / GridSquareSize - GridHeight) / 2 - 1);

        private const int GridOffsetX = GridSquareSize;
        private const int GridOffsetY = GridSquareSize;
        
        private int gridL = GridOffsetX;
        private int gridR = GridOffsetX + GridWidth * GridSquareSize;

        private const int XSpawnPosition = 3* GridSquareSize;
        private const int YSpawnPosition = -1* GridSquareSize;

        private const int XSpawnOffset = (GridWidth / NumPlayers) * GridSquareSize;

        private const int BufferedPieceX = -5* GridSquareSize;
        private const int BufferedPieceY = GridSquareSize;

        private const int NextPiecesX = (GridWidth + 3) * GridSquareSize;
        private const int NextPiecesY = GridSquareSize;

        private const int NextPiecesWidth = 4* GridSquareSize;
        private const int NextPiecesHeight = 5* NextPiecesAmount * GridSquareSize;

        private readonly Tetromino[] currentPieces = new Tetromino[NumPlayers];

        private readonly int[][] positions = new int[NumPlayers][];

        private readonly float[] realPositionsY = new float[NumPlayers];
        private readonly float[] gravityMultipliers = new float[NumPlayers];
        
        private readonly Tetromino.Type[] bufferedPieces = new Tetromino.Type[NumPlayers] ;
        private readonly bool[] swapped = new bool[NumPlayers];
        private readonly bool[] firstBuffers = new bool[NumPlayers];
        private readonly Color[][] bufferedPieceGrids = new Color[NumPlayers][];
        private readonly Texture2D[] bufferedPieceTextures = new Texture2D[NumPlayers];
        
        private readonly Queue<Tetromino.Type>[] pieceQueues = new Queue<Tetromino.Type>[NumPlayers];
        
        private readonly Color[][] nextPiecesGrids = new Color[NumPlayers][];
        private readonly Texture2D[] nextPiecesTextures = new Texture2D[NumPlayers];
        
        private readonly Vector2[] phantomPositions = new Vector2[NumPlayers];
        
        private readonly Color[][] phantomDropGrids = new Color[NumPlayers][];
        private readonly Texture2D[] phantomDropTextures = new Texture2D[NumPlayers];
        
        private const float Gravity = (2*GridSquareSize)/60f;
        
        private readonly Color[] gameGrid = new Color[GridWidth*GridHeight];
        
        private static readonly int TetrominoTypeCount = typeof(Tetromino.Type).GetEnumValues().Length;
        
        #endregion
        
        
        #region Needs Initializing
        
        private Color[] phantomColours;
        
        private Color[] backgroundGrid;
        private Texture2D backgroundTexture;

        private Random random;

        public static TetrominoColorPalette CurrentColorPalette;

        private FrameCounter frameCounter;
        private Inputs inputHandler;
        private PlayerControllerManager playerControllerManager;

        private Texture2D gameTexture;
        private Texture2D gameTexture2;

        private Grid displayGrid;

        private Matrix scale;
        
        private SoundEffect sePlaceBlock;
        private SoundEffect seClearRow;
        private SoundEffect seFastScroll;
        private SoundEffect sePerfect;

        private AudioManager audioManager;
        private AudioEmitter audioEmitter;
        private Vector3 soundPosition;
        
        #endregion

        #endregion
        
        #region Setup
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            
            #region Display Settings
            
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;

            graphics.ApplyChanges();
            
            #endregion

            #region Variable Setting
            
            #region Colour Palette
            
            CurrentColorPalette = new TetrominoColorPalette(new[]
            {
                new Color(49,199,239),      // I
                new Color(247,211,8),       // O
                new Color(173,77,156),      // T
                new Color(66,182,66),       // S
                new Color(239,32,41),       // Z
                new Color(90,101,173),      // J
                new Color(239,121,33)       // L
            });
            
            #endregion
            
            // other classes
            frameCounter = new FrameCounter();
            inputHandler = new Inputs();
            playerControllerManager = new PlayerControllerManager();
            
            // initialisation
            scale = Matrix.CreateScale(new Vector3(GridSquareSize, GridSquareSize, 1));
            random = new Random();


            phantomColours = new Color[]
            {
                new Color(130,106,155,100),
                new Color(106,155,130,100)
            };
            
            
            
            // 3D audio stuff
            audioManager = new AudioManager(this);
            Components.Add(audioManager);
            audioEmitter = new AudioEmitter();
            soundPosition = Vector3.Zero;
            
            
            
            // texture setup
            gameTexture = new Texture2D(graphics.GraphicsDevice, GridWidth, GridHeight);
            gameTexture.SetData(gameGrid);
            gameTexture2 = new Texture2D(graphics.GraphicsDevice, GridWidth, GridHeight);
            gameTexture2.SetData(gameGrid);
            
            
            // per player initialisation
            for (int i = 0; i < NumPlayers; i++)
            {
                // set positions for each player
                positions[i] = new[]
                {
                    GridOffsetX + XSpawnPosition + XSpawnOffset*i,
                    GridOffsetY + YSpawnPosition
                };
                
                
                // gravity
                gravityMultipliers[i] = 1f;
                realPositionsY[i] = 0f;
                
                
                // buffer
                firstBuffers[i] = true;
                bufferedPieces[i] = Tetromino.Type.I;
                bufferedPieceGrids[i] = new Color[8];
                bufferedPieceTextures[i] = new Texture2D(graphics.GraphicsDevice,4,2);
                
                
                // next pieces
                pieceQueues[i] = new Queue<Tetromino.Type>();
                for (int _ = 0; _ < NextPiecesAmount; _++)
                {
                    pieceQueues[i].Enqueue((Tetromino.Type)random.Next(TetrominoTypeCount));
                }
                nextPiecesTextures[i] = new Texture2D(graphics.GraphicsDevice,NextPiecesWidth/GridSquareSize,NextPiecesHeight/GridSquareSize);
                nextPiecesGrids[i] = new Color[(nextPiecesTextures[i].Width*nextPiecesTextures[i].Height)];
                nextPiecesTextures[i].SetData(nextPiecesGrids[i]);
                UpdateNextPiecesDisplay(i);
                
                
                // current piece
                currentPieces[i] = new Tetromino((Tetromino.Type)random.Next(TetrominoTypeCount));
                currentPieces[i].Update();
                
                
                // phantom
                phantomPositions[i].X = positions[i][0];
                phantomPositions[i].Y = positions[i][1];
                phantomDropTextures[i] = new Texture2D(graphics.GraphicsDevice, 4,4);
                UpdatePhantom(i);

            }
            
            #endregion
            
            #region Grid Display
            
            //int x, int y, int width, int height, int x_amount, int y_amount, Color color, int thickness, GraphicsDeviceManager _graphics
            displayGrid = new Grid(GridOffsetX+DisplayOffsetX,GridOffsetY+GridSquareSize*4+DisplayOffsetY,
                
                GridSquareSize,GridSquareSize,
                
                GridWidth,
                GridHeight-4,
                
                Color.Gray,1,graphics);

            #endregion
            
            #region Decorative Border For Buffered & Next Pieces

            int screenGridWidth = graphics.PreferredBackBufferWidth/GridSquareSize;
            int screenGridHeight= graphics.PreferredBackBufferHeight/GridSquareSize;
            backgroundGrid = new Color[screenGridWidth*screenGridHeight];
            backgroundTexture = new Texture2D(graphics.GraphicsDevice,screenGridWidth,screenGridHeight);


            int xOffset;
            int yOffset;
            
            for (int multI = 1; multI < 2; multI++)
            {
                            
                xOffset = (multI*BufferedPieceX) + DisplayOffsetX - GridSquareSize - GridSquareSize*2*(multI-1);
                yOffset = BufferedPieceY + DisplayOffsetY - GridSquareSize;

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
                
                
                
                xOffset = NextPiecesX + DisplayOffsetX - GridSquareSize + 7*GridSquareSize*(multI-1);
                yOffset = NextPiecesY + DisplayOffsetY - GridSquareSize;
            
                xOffset /= GridSquareSize;
                yOffset /= GridSquareSize;
            
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 5* NextPiecesAmount +1; j++)
                    {
                        if (!((i == 0 && j == 0) || (i == 5 && j == 25) || (i == 5 && j == 0) || (i == 0 && j == 25)))
                        {
                            backgroundGrid[(xOffset + i) + (yOffset + j)*screenGridWidth] = (i>0 && i<5 && j>0 && j%5!=0)?new Color(30,30,30):new Color(50,50,50);
                        }

                    }
                }
                
                
                
            }



            
            
            
            backgroundTexture.SetData(backgroundGrid);
            
            #endregion
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            #region Sound Effect Loading
    
            sePlaceBlock = SoundEffect.FromFile(Path.Combine("Content", "se", "place_block.wav"));
            seClearRow = SoundEffect.FromFile(Path.Combine("Content", "se", "clear_row.wav"));
            seFastScroll = SoundEffect.FromFile(Path.Combine("Content", "se", "fast_scroll.wav"));
            sePerfect = SoundEffect.FromFile(Path.Combine("Content", "se", "perfect.wav"));
            
            #endregion
            
            #region Font Loading
            
            spriteFont = Content.Load<SpriteFont>(Path.Combine("font", "DebugFont"));
            
            #endregion
            
        }
        
        #endregion

        #region Tetris Functions

        private List<Vector2> LowestSquares(List<Vector2> squares)
        {
            
            List<Vector2> lowestSquares = new List<Vector2>();
            
            // loop through all squares
            for (int i = 0; i < squares.Count; i++)
            {
                // find the current lowest square for this x position in our list
                int index = lowestSquares.FindIndex(v => (int) v.X == (int) squares[i].X);
                
                // if no square is at this x position yet, this one is now the lowest
                if (index == -1)
                {
                    lowestSquares.Add(squares[i]);
                }
                else
                {
                    // if this square is lower than the lowest square at this x position, set this as the current minimum
                    lowestSquares[index] = (lowestSquares[index].Y < squares[i].Y) ? squares[i] : lowestSquares[index];
                }
            }

            return lowestSquares;


        }
        
        private int MinimumMove(List<Vector2> squares, int currentPieceIndex, bool usesLowest)
        {
            // the lowest y value of the current tetromino for each column of squares

            List<Vector2> lowestSquares;
            
            if (usesLowest)
            {
                lowestSquares = new List<Vector2>(squares);
            }
            else
            {
                lowestSquares = LowestSquares(squares);
            }
            

            int minimumMove = GridHeight;
            for (int j = 0; j < lowestSquares.Count; j++)
            {
                int highestInLine = GridHeight - 1;
                
                // loop k from the lowest square of the tetromino down to the bottom of the grid
                for (int k = ((int) (lowestSquares[j].Y + positions[currentPieceIndex][1]) / GridSquareSize); k < GridHeight; k++)
                {
                    int xCheck = (int) (lowestSquares[j].X + positions[currentPieceIndex][0]) / GridSquareSize - 1;
                    int yCheck = GridWidth * k;
                    
                    // if out of bounds
                    if (xCheck < 0 || xCheck >= GridWidth || yCheck < 0 || yCheck >= GridWidth*GridHeight)
                    {
                        break;
                    }
                    
                    // if the square we are checking is already occupied
                    if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                    {
                        highestInLine = k - 1;
                        break;
                    }
                }
                
                // get current actual y position (relative to the bottom of the tetromino)
                int gridY = (int) lowestSquares[j].Y + positions[currentPieceIndex][1];
                gridY /= GridSquareSize;
                
                // work out the minimum of all distances between the
                // bottom of the tetromino and the top of the stack at that x-pos
                minimumMove = Math.Min(minimumMove, highestInLine - gridY);
            }

            return minimumMove;
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
        
        private readonly struct KickResult
        {
            public readonly bool Succeeded;
            public readonly Vector2 Result;

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

            BitArray lastRotation = new BitArray(2)
            {
                [0] = currentPiece.rotation[0],
                [1] = currentPiece.rotation[1]
            };


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
                    int yCheck = (int) ((sq.Y + positions[currentPieceIndex][1]) / GridSquareSize + transform.Y);
                    yCheck *= GridWidth;
                    
                    
                    if (xCheck>=GridWidth || xCheck<0 || yCheck<0 || yCheck>=GridWidth*GridHeight)
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

                if (possible)
                {
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

                    int rows_cleared = downwards;
                
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

                    if (rows_cleared < 4)
                    {
                        // play clear_row
                        seClearRow.Play();
                    }
                    else
                    {
                        // tetris :000000000000000000!!!!! no way omg look how cool
                        sePerfect.Play();
                    }

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

            phantomDropGrids[currentPieceIndex] = new Color[16];
            foreach (Vector2 square in squares)
            {
                int x = (int) (square.X / GridSquareSize);
                int y = (int) (square.Y / GridSquareSize);

                Color currentColour;// = new Color(120, 120, 120, 100);
                if (currentPieceIndex < phantomColours.Length)
                {
                    currentColour = phantomColours[currentPieceIndex];
                }
                else
                {
                    currentColour = new Color(120, 120, 120, 100);
                }
                phantomDropGrids[currentPieceIndex][x + y * 4] = currentColour;

            }
                
            phantomDropTextures[currentPieceIndex].SetData(phantomDropGrids[currentPieceIndex]);

            phantomPositions[currentPieceIndex].X = positions[currentPieceIndex][0];

            int minimumMove = MinimumMove(squares,currentPieceIndex,false);

            minimumMove++;
            phantomPositions[currentPieceIndex].Y = positions[currentPieceIndex][1] + minimumMove * GridSquareSize;


        }
        
        private void UpdateBufferedPiece(int currentPieceIndex)
        {
            if (!firstBuffers[currentPieceIndex])
            {
                bufferedPieceGrids[currentPieceIndex] = new Color[8];
            
            
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in Tetromino.Rectangles[(int)bufferedPieces[currentPieceIndex]])
                {
                    squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
                }

                int yOffset = 0;

                if (bufferedPieces[currentPieceIndex] == Tetromino.Type.O) yOffset = -1;
                
                foreach (Vector2 sq in squares)
                {
                    int x = (int) sq.X / GridSquareSize;
                    int y = ((int) sq.Y / GridSquareSize) + yOffset;

                    bufferedPieceGrids[currentPieceIndex][x + y * nextPiecesTextures[currentPieceIndex].Width] = (swapped[currentPieceIndex])?Color.Gray:CurrentColorPalette[bufferedPieces[currentPieceIndex]];
                }
            
            
                bufferedPieceTextures[currentPieceIndex].SetData(bufferedPieceGrids[currentPieceIndex]);
            }

        }
        
        private void UpdateNextPiecesDisplay(int currentPieceIndex)
        {


            nextPiecesGrids[currentPieceIndex] = new Color[(nextPiecesTextures[currentPieceIndex].Width*nextPiecesTextures[currentPieceIndex].Height)];


            int x = 0;
            int y = 0;
            
            
            foreach (Tetromino.Type piece in pieceQueues[currentPieceIndex])
            {

                if (!BlockDisplayMode)
                {
                    List<Vector2> squares = new List<Vector2>();

                    foreach (Rectangle r in Tetromino.Rectangles[(int)piece])
                    {
                        squares.AddRange(Rectangle.RectToSquares(r,GridSquareSize));
                    }

                    foreach (Vector2 sq in squares)
                    {
                        x = (int) sq.X / GridSquareSize;
                        int yTemp = y + ((int) sq.Y / GridSquareSize);

                        nextPiecesGrids[currentPieceIndex][x + yTemp * nextPiecesTextures[currentPieceIndex].Width] = CurrentColorPalette[piece];
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

                            
                            nextPiecesGrids[currentPieceIndex][x + y * nextPiecesTextures[currentPieceIndex].Width] = CurrentColorPalette[piece];

                            x += 1;
                        }
                        y += 1;
                    }
                }

                if (BlockDisplayMode) y += 1;
            }

            nextPiecesTextures[currentPieceIndex].SetData(nextPiecesGrids[currentPieceIndex]);
        }

        private Vector3 GetAudioPosition(int currentPieceIndex)
        {
            double angle = ((double) positions[currentPieceIndex][0]) / ((double) GridWidth*GridSquareSize);

            angle *= 0.7;
            angle += 0.15;
            angle *= Math.PI;
            
            float dx = (float)-Math.Cos(angle);
            float dz = (float)-Math.Sin(angle);
            
            return new Vector3(dx, 0, dz) * 10;

        }
        
        #endregion

        #region Game

        protected override void Update(GameTime gameTime)
        {
            
            inputHandler.UpdateState();

            // quit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // calculations and inputs for all players
            for (int currentPieceIndex = 0; currentPieceIndex < currentPieces.Length; currentPieceIndex++)
            {
                
                #region Apply Gravity
                
                // if it has gone down by a square
                if ((int) realPositionsY[currentPieceIndex] > GridSquareSize)
                {
                    // reset delta y
                    realPositionsY[currentPieceIndex] = 0f;

                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                    }
                    
                    // find minimum movement downwards until placement
                    int minimumMove = MinimumMove(squares,currentPieceIndex,false);

                    if (minimumMove > 0)
                    {
                        // move downwards
                        positions[currentPieceIndex][1] += GridSquareSize;
                        if (gravityMultipliers[currentPieceIndex] > 1f)
                        {
                            // play fast_scroll
                            //seFastScroll.Play();

                            soundPosition = GetAudioPosition(currentPieceIndex);
                            audioEmitter.Update(soundPosition);
                            audioManager.Play3DSound(seFastScroll,false,audioEmitter);
                        }
                    }
                    else
                    {
                        // set piece in place
                        foreach (Vector2 square in squares)
                        {
                            // set square on game grid
                            gameGrid[((int) ((square.X + positions[currentPieceIndex][0]) / GridSquareSize) - 1) +             // x position
                                     ((((int) ((square.Y + positions[currentPieceIndex][1]) / GridSquareSize))) * GridWidth)]  // y position
                                
                                = CurrentColorPalette[currentPieces[currentPieceIndex].BlockType];                             // current block's colour
                        }

                        positions[currentPieceIndex][0] = XSpawnPosition + GridOffsetX + XSpawnOffset*currentPieceIndex;
                        positions[currentPieceIndex][1] = YSpawnPosition + GridOffsetY;

                        currentPieces[currentPieceIndex] = new Tetromino(pieceQueues[currentPieceIndex].Dequeue());
                        pieceQueues[currentPieceIndex].Enqueue((Tetromino.Type) random.Next(TetrominoTypeCount));
                        
                        currentPieces[currentPieceIndex].Update();
                        
                        // update display of next pieces
                        UpdateNextPiecesDisplay(currentPieceIndex);
                        swapped[currentPieceIndex] = false;
                        UpdateBufferedPiece(currentPieceIndex);

                        Destroy();
                        
                        
                        
                            


                        for (int player = 0; player < NumPlayers; player++)
                        {

                            List<Vector2> currentSquares = new List<Vector2>();
                            
                            foreach (Rectangle r in currentPieces[player]._r)
                            {
                                currentSquares.AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                            }
                            
                            foreach (Vector2 square in currentSquares)
                            {

                                int push = 0;

                                while (true)
                                {
                                    int xCheck = (int) (square.X + positions[player][0]) / GridSquareSize;
                                    int yCheck = (int) (square.Y + positions[player][1]) / GridSquareSize;
                                    yCheck -= push;
                                    xCheck -= 1;
                                    Debug.WriteLine("push " + push + " " + xCheck + " " + yCheck + " " + square.X +
                                                    " " + square.Y + " " + positions[player][0] + " " +
                                                    positions[player][1]);

                                    yCheck *= GridWidth;


                                    // if player stuck inside placed piece...

                                    if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                                    {
                                        // push up until no longer stuck
                                        push++;

                                        if (!PushUp)
                                        {
                                            break;
                                        }
                                        
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }



                                if (push != 0)
                                {

                                    if (PushUp)
                                    {
                                        // if piece needs to be pushed up

                                        // push piece up
                                        positions[player][1] -= (push + 2) * GridSquareSize;
                                    }
                                    else
                                    {
                                        positions[player][0] = XSpawnPosition + GridOffsetX +
                                                               XSpawnOffset * player;
                                        positions[player][1] = YSpawnPosition + GridOffsetY;
                                    }

                                }
                            }
                        }







                        // update all phantoms
                        for (int it = 0; it < NumPlayers; it++) { UpdatePhantom(it); }
                        
                    }

                }
                
                #endregion
                
                
                #region Movement and Rotation

                if (inputHandler.KeyPressed(playerControllerManager.GetControl(currentPieceIndex, Controls.RotateRight)))
                {
                    // kick handles all rotation - rotation direction 1, as in clockwise
                    KickResult result = CheckKick(1, currentPieceIndex);
                    if (result.Succeeded)
                    {
                        positions[currentPieceIndex][0] += (int) (GridSquareSize * result.Result.X);
                        positions[currentPieceIndex][1] += (int) (GridSquareSize * result.Result.Y);
                    }

                    UpdatePhantom(currentPieceIndex);
                }

                if (inputHandler.KeyPressed(playerControllerManager.GetControl(currentPieceIndex, Controls.RotateLeft)))
                {
                    // kick handles all rotation - rotation direction -1, as in counter-clockwise
                    KickResult result = CheckKick(-1, currentPieceIndex);
                    if (result.Succeeded)
                    {
                        positions[currentPieceIndex][0] += (int)(GridSquareSize * result.Result.X);
                        positions[currentPieceIndex][1] += (int)(GridSquareSize * result.Result.Y);
                    }
                    
                    UpdatePhantom(currentPieceIndex);
                }

                if (inputHandler.TimedPress(playerControllerManager.GetControl(currentPieceIndex, Controls.MoveRight),5,15))
                {
                    //seFastScroll.Play();
                    soundPosition = GetAudioPosition(currentPieceIndex);
                    audioEmitter.Update(soundPosition);
                    audioManager.Play3DSound(seFastScroll,false,audioEmitter);
                    
                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                    }
                    
                    // find the squares with the largest x value for each row
                    List<Vector2> rightMostSquares = new List<Vector2>();

                    for (int i = 0; i < squares.Count; i++)
                    {
                        // find current maximum x value for this y value
                        int index = rightMostSquares.FindIndex(v => (int)v.Y == (int)squares[i].Y);
                        
                        // if there isn't an x value for this y value
                        if (index == -1)
                        {
                            // set this square to the rightmost x value for the current y value
                            rightMostSquares.Add(squares[i]);
                        }
                        else
                        {
                            // compare this x value with the previous maximum x value, if this is higher, this is the new maximum
                            rightMostSquares[index] = (rightMostSquares[index].X > squares[i].X) ? rightMostSquares[index]:squares[i];
                        }
                    }
                    
                    int canMove = 1;
                    
                    // loop through the right side of the tetromino
                    for (int j = 0; j < rightMostSquares.Count; j++)
                    {
                        // calculate relative grid positions
                        int xCheck = (int)(rightMostSquares[j].X + positions[currentPieceIndex][0]) / GridSquareSize;
                        int yCheck = (int)((rightMostSquares[j].Y + positions[currentPieceIndex][1]) / GridSquareSize) * GridWidth;
                        
                        // if out of bounds
                        if (xCheck >= GridWidth || yCheck >= GridHeight*GridWidth || xCheck < 0 || yCheck < 0)
                        {
                            canMove *= 0;
                            break;
                        }
                        
                        // if this square is filled in
                        if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                        {
                            canMove *= 0;
                        }


                    }

                    // inc. x position
                    positions[currentPieceIndex][0] += GridSquareSize * canMove;
                    
                    // if this piece is outside of the grid, push it back inside
                    // (a workaround which shouldn't be necessary but prevents any possible glitches)
                    MoveCheck(gridL, gridR, currentPieceIndex);
                    
                    UpdatePhantom(currentPieceIndex);

                }

                if (inputHandler.TimedPress(playerControllerManager.GetControl(currentPieceIndex, Controls.MoveLeft),5,15))
                {
                    //seFastScroll.Play();
                    soundPosition = GetAudioPosition(currentPieceIndex);
                    audioEmitter.Update(soundPosition);
                    audioManager.Play3DSound(seFastScroll,false,audioEmitter);
                    
                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[0]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                    }
                    
                    // find the squares with the smallest x value for each row
                    List<Vector2> leftMostSquares = new List<Vector2>();

                    for (int i = 0; i < squares.Count; i++)
                    {
                        // find current minimum x value for this y value
                        int index = leftMostSquares.FindIndex(v => (int)v.Y == (int)squares[i].Y);
                        
                        // if there isn't an x value for this y value
                        if (index == -1)
                        {
                            // set this square to the leftmost x value for the current y value
                            leftMostSquares.Add(squares[i]);
                        }
                        else
                        {
                            // compare this x value with the previous minimum x value, if this is lower, this is the new minimum
                            leftMostSquares[index] = (leftMostSquares[index].X < squares[i].X) ? leftMostSquares[index]:squares[i];
                        }
                    }

                    int canMove = 1;
                    
                    // loop through the left side of the tetromino
                    for (int j = 0; j < leftMostSquares.Count; j++)
                    {
                        // calculate relative grid positions
                        int xCheck = (int)(leftMostSquares[j].X + positions[currentPieceIndex][0]) / GridSquareSize - 2;
                        int yCheck = (int)((leftMostSquares[j].Y + positions[currentPieceIndex][1]) / GridSquareSize) * GridWidth;

                        // if out of bounds
                        if (xCheck + yCheck >= gameGrid.Length || xCheck + yCheck < 0)
                        {
                            canMove *= 0;
                        }
                        else
                        {
                            // if this square is filled in
                            if (gameGrid[xCheck + yCheck] != new Color(0, 0, 0, 0))
                            {
                                canMove *= 0;
                            }
                        }




                    }
                    
                    // dec. x position
                    positions[currentPieceIndex][0] -= GridSquareSize * canMove;
                    
                    // if this piece is outside of the grid, push it back inside
                    // (a workaround which shouldn't be necessary but prevents any possible glitches)
                    MoveCheck(gridL, gridR, currentPieceIndex);

                    UpdatePhantom(currentPieceIndex);
                    
                    
                }

                #endregion
                
                
                
                #region Extra Inputs
                
                if (inputHandler.KeyPressed(playerControllerManager.GetControl(currentPieceIndex, Controls.Hold)) && !swapped[currentPieceIndex])
                {

                    // limit swaps per go to 1
                    swapped[currentPieceIndex] = true;
                    
                    // swap piece with buffered piece
                    Tetromino.Type temp = currentPieces[currentPieceIndex].BlockType;
                    currentPieces[currentPieceIndex] = new Tetromino(bufferedPieces[currentPieceIndex]);
                    bufferedPieces[currentPieceIndex] = temp;

                    // go back to spawn pos
                    positions[currentPieceIndex][0] = XSpawnPosition + GridOffsetX + XSpawnOffset*currentPieceIndex;
                    positions[currentPieceIndex][1] = YSpawnPosition + GridOffsetY;

                    // if this was the first swap
                    if (firstBuffers[currentPieceIndex])
                    {
                        // no longer the first swap for next time
                        firstBuffers[currentPieceIndex] = false;
                        
                        // queue next piece as nothing to swap with
                        currentPieces[currentPieceIndex] = new Tetromino(pieceQueues[currentPieceIndex].Dequeue());
                        pieceQueues[currentPieceIndex].Enqueue((Tetromino.Type) random.Next(TetrominoTypeCount));
                    }
                    
                    // update relevant displays
                    UpdateBufferedPiece(currentPieceIndex);
                    currentPieces[currentPieceIndex].Update();
                    UpdateNextPiecesDisplay(currentPieceIndex);
                    UpdatePhantom(currentPieceIndex);

                }
                
                if (Keyboard.GetState().IsKeyDown(playerControllerManager.GetControl(currentPieceIndex, Controls.SoftDrop)))
                {
                    // 10x gravity speed
                    gravityMultipliers[currentPieceIndex] = 10f;
                }
                else
                {
                    // normal gravity speed
                    gravityMultipliers[currentPieceIndex] = 1f;
                }
                
                if (inputHandler.KeyPressed(playerControllerManager.GetControl(currentPieceIndex, Controls.HardDrop)))
                {
                    // play placement sound effect
                    sePlaceBlock.Play();

                    int[] minimumMoves = new int[NumPlayers];
                    List<Vector2>[] eachLowestSquares = new List<Vector2>[NumPlayers];
                    List<Vector2>[] eachSquares = new List<Vector2>[NumPlayers];

                    for (int i = 0; i < NumPlayers; i++)
                    {
                        // split tetromino into squares
                        eachSquares[i] = new List<Vector2>();
                        foreach (Rectangle r in currentPieces[i]._r)
                        {
                            eachSquares[i].AddRange(Rectangle.RectToSquares(r, GridSquareSize));
                        }

                        eachLowestSquares[i] = LowestSquares(eachSquares[i]);
                    
                        // find amount to move current piece downwards to place it
                        minimumMoves[i] = MinimumMove(eachLowestSquares[i],i,true);
                        
                        
                        
                    }
                    
                    
                    // the players to force to hard drop
                    List<int> forceHardDrop = new List<int>();
                    
                    
                    for (int i = 0; i < NumPlayers; i++)
                    {
                        if (minimumMoves[i] == 0)
                        {
                            if (i == currentPieceIndex)
                            {
                                continue;
                            }
                            foreach (Vector2 square in eachLowestSquares[currentPieceIndex])
                            {
   
                                List<Vector2> otherSquares = eachLowestSquares[i];

                                foreach (Vector2 otherSquare in otherSquares)
                                {
                                    
                                    
                                    if (Math.Abs((positions[i][0]+otherSquare.X) - (positions[currentPieceIndex][0]+square.X)) < 0.01f)
                                    {
                                        if (!forceHardDrop.Contains(i))
                                        {
                                            forceHardDrop.Add(i);
                                        }

                                    }
                                }
                            }
                        }
                    }


                    forceHardDrop.Add(currentPieceIndex);

                    foreach (int index in forceHardDrop)
                    {

                        // move piece
                        positions[index][1] += GridSquareSize * minimumMoves[index];

                        // set pixels on grid
                        foreach (Vector2 square in eachSquares[index])
                        {
                            gameGrid[((int)((square.X + positions[index][0]) / GridSquareSize) - 1) + ((((int)((square.Y + positions[index][1]) / GridSquareSize))) * GridWidth)] = CurrentColorPalette[currentPieces[index].BlockType];
                        }

                        // go to spawn pos
                        positions[index][0] = XSpawnPosition + GridOffsetX + XSpawnOffset*index;
                        positions[index][1] = YSpawnPosition + GridOffsetY;

                        // change piece type
                        currentPieces[index] = new Tetromino(pieceQueues[index].Dequeue());
                        pieceQueues[index].Enqueue((Tetromino.Type)random.Next(TetrominoTypeCount));
                    
                        currentPieces[index].Update();

                        // update display of next and buffer pieces
                        UpdateNextPiecesDisplay(index);
                        swapped[index] = false;
                        UpdateBufferedPiece(index);
                        
                        
                        // if there's more than 1 piece to force to drop
                        if (forceHardDrop.Count > 1)
                        {
                            // recalculate taking into account new drops
                            minimumMoves[currentPieceIndex] = MinimumMove(eachLowestSquares[currentPieceIndex],currentPieceIndex,true);
                        }

                    }
                    
                    
                    
                    for (int player = 0; player<NumPlayers; player++)
                    {

                        if (player == currentPieceIndex)
                        {
                            continue;
                        }
                        
                        // loop through all other players

                        
                        
                        foreach (Vector2 square in eachSquares[player])
                        {
                            
                            int push = 0;
                            
                            while (true)
                            {
                                int xCheck = (int) (square.X+positions[player][0]) / GridSquareSize;
                                int yCheck = (int) (square.Y+positions[player][1]) / GridSquareSize;
                                yCheck -= push;
                                xCheck -= 1;
                                Debug.WriteLine("push "+push+" "+xCheck+" "+yCheck+" "+square.X+" "+square.Y+" "+positions[player][0]+" "+positions[player][1]);
                                
                                yCheck *= GridWidth;

                                
                                // if player stuck inside other player...
                                
                                if (gameGrid[xCheck+yCheck] != new Color(0,0,0,0))
                                {
                                    // push up until no longer stuck
                                    push++;

                                    if (!PushUp)
                                    {
                                        break;
                                    }
                                    
                                }
                                else
                                {
                                    break;
                                }
                            }
                            
                            
                            if (push != 0)
                            {

                                if (PushUp)
                                {
                                    // if piece needs to be pushed up

                                    // push piece up
                                    positions[player][1] -= (push + 2) * GridSquareSize;
                                }
                                else
                                {
                                    positions[player][0] = XSpawnPosition + GridOffsetX +
                                                           XSpawnOffset * player;
                                    positions[player][1] = YSpawnPosition + GridOffsetY;
                                }

                            }

                            
                        }

                        

                        
                        
                    }
                    
                    
                    

                    // check if finished row
                    Destroy();
                    
                    // update all phantoms
                    for (int it = 0; it < NumPlayers; it++) { UpdatePhantom(it); }

                }
                #endregion
                
                
                // + buffer in next position after applying gravity
                realPositionsY[currentPieceIndex] += Gravity * gravityMultipliers[currentPieceIndex];

            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            # region Pre-Draw
            
            float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);
            
            GraphicsDevice.Clear(Color.Black);
            
            #endregion

            #region No Zoom Sprite Batch

            spriteBatch.Begin();
            
            // draw the grid
            displayGrid.Draw(spriteBatch, graphics);
            
            // draw the fps
            spriteBatch.DrawString(spriteFont, frameCounter.CurrentFramesPerSecond.ToString(CultureInfo.InvariantCulture), Vector2.Zero,
                Color.White);

            spriteBatch.End();
            
            #endregion

            #region Zoomed In Sprite Batch
            
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null, null, scale);
            
            // render all pieces to gameTexture2
            
            Color[] pieceDisplayGrid = new Color[GridWidth * GridHeight];
            for (int i = 0; i < NumPlayers; i++)
            {
                currentPieces[i].Draw(positions[i][0], positions[i][1], pieceDisplayGrid);
                gameTexture2.SetData(pieceDisplayGrid);
            }
            
            gameTexture.SetData(gameGrid);


            // placed pieces
            spriteBatch.Draw(gameTexture,
                new Vector2(1 + DisplayOffsetX / GridSquareSize, 1 + DisplayOffsetY / GridSquareSize),
                Color.White);

            // player pieces
            spriteBatch.Draw(gameTexture2,
                new Vector2(1 + DisplayOffsetX / GridSquareSize, 1 + DisplayOffsetY / GridSquareSize),
                Color.White);


            // decorative borders for buffer/next pieces
            spriteBatch.Draw(backgroundTexture, new Vector2(), Color.White);

            
            // draw phantom pieces
            for (int i = 0; i < NumPlayers; i++)
            {
                
                spriteBatch.Draw(phantomDropTextures[i],
                    new Vector2((phantomPositions[i].X + DisplayOffsetX) / GridSquareSize, (phantomPositions[i].Y + DisplayOffsetY) / GridSquareSize),
                    Color.White);
                
            }

            // draw buffered piece (only 0th player's currently)
            spriteBatch.Draw(bufferedPieceTextures[0],
                new Vector2(((float)BufferedPieceX + DisplayOffsetX) / GridSquareSize,
                    ((float)BufferedPieceY + DisplayOffsetY) / GridSquareSize),
                Color.White);
            
            /*
            spriteBatch.Draw(bufferedPieceTextures[1],
                new Vector2(((float)BufferedPieceX*2 + DisplayOffsetX) / GridSquareSize -2,
                    ((float)BufferedPieceY + DisplayOffsetY) / GridSquareSize),
                Color.White)6;
            */


            // draw next pieces queue (only 0th player's currently)
            spriteBatch.Draw(nextPiecesTextures[0],
                new Vector2(((float)NextPiecesX + DisplayOffsetX) / GridSquareSize,
                    ((float)NextPiecesY + DisplayOffsetY) / GridSquareSize),
                Color.White);
            
            /*
            spriteBatch.Draw(nextPiecesTextures[1],
                new Vector2(((float)NextPiecesX + DisplayOffsetX + NextPiecesWidth + GridSquareSize*3) / GridSquareSize,
                    ((float)NextPiecesY + DisplayOffsetY) / GridSquareSize),
                Color.White);
            */

            spriteBatch.End();
            
            #endregion
            
            #region Post-Draw
            
            base.Draw(gameTime);
            
            #endregion
            
            
        }
        
        #endregion
        

    }
}
