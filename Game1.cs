using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

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


        #region Calculated

        private const bool DeveloperMode = true;
        private const int AlphaDecreaseRate = 4;
        private int pointsCurrentAlpha = 255;
        private Color pointsColor;
        private bool displayingPointsEffect;
        private String pointEffectText = "";

        private int points = 0;
        private int TotalRowsCleared = 0;

        private float shakeX = 0;
        private float shakeY = 0;
        private float shakeDamping = 10;
        

        private readonly Array tetrominoTypes =  typeof(Tetromino.Type).GetEnumValues();
        
        public static int GridWidth;
        
        private int DisplayOffsetX;
        private int DisplayOffsetY;

        private int GridOffsetX;
        private int GridOffsetY;

        private int gridL;
        private int gridR;

        private int XSpawnPosition;
        private int YSpawnPosition;

        private int XSpawnOffset;

        private int BufferedPieceX;
        private int BufferedPieceY;

        private int NextPiecesX;
        private int NextPiecesY;

        private int NextPiecesWidth;
        private int NextPiecesHeight;

        private Tetromino[] currentPieces;

        private int[][] positions;

        private float[] realPositionsY;
        private float[] gravityMultipliers;
        
        private Tetromino.Type[] bufferedPieces;
        private bool[] swapped;
        private bool[] firstBuffers;
        private Color[][] bufferedPieceGrids;
        private Texture2D[] bufferedPieceTextures;
        
        private Queue<Tetromino.Type>[] pieceQueues;
        
        private Color[][] nextPiecesGrids;
        private Texture2D[] nextPiecesTextures;
        
        private Vector2[] phantomPositions;
        
        private Color[][] phantomDropGrids;
        private Texture2D[] phantomDropTextures;
        
        private float Gravity;
        
        private Color[] gameGrid;

        private Color[] squareIndicatorGrid;

        #endregion


        #region Needs Initializing


        private Texture2D background;
        
        private Random random;
        
        // Colours
        private Color transparentBlack;
        private Color darkGray;
        private Color darkerGray;
        
        private Color phantomColourDefault;
        
        private Color[] GameBorderGrid;
        private Texture2D GameBorderTexture;

        private Texture2D squareIndicatorTexture;

        private Color[] backgroundGrid;
        private Texture2D backgroundTexture;

        private Texture2D darkGrayPixel;
        private Texture2D blackPixel;

        public static TetrominoColorPalette CurrentColorPalette;

        private FrameCounter frameCounter;
        private Inputs inputHandler;
        private PlayerControllerManager playerControllerManager;

        private SettingsManager settingsManager;
        public static Settings settings;

        private Texture2D gameTexture;
        private Texture2D gameTexture2;

        private Grid displayGrid;

        //private Matrix scale;
        
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


            #region Calculated Variables
            
            
            settingsManager = new SettingsManager();
            settings = settingsManager.GetSettings(settingsManager.GetRequestedPreset());
            
            
            GridWidth = 10 * settings.NumPlayers;
            
            DisplayOffsetX = settings.GridSquareSize * ((settings.ScreenWidth/settings.GridSquareSize - GridWidth) / 2 -1);
            DisplayOffsetY = settings.GridSquareSize;//* ((ScreenHeight / GridSquareSize - GridHeight) / 2 - 1);

            GridOffsetX = settings.GridSquareSize;
            GridOffsetY = settings.GridSquareSize;

            gridL = GridOffsetX;
            gridR = GridOffsetX + GridWidth * settings.GridSquareSize;

            XSpawnPosition = 3* settings.GridSquareSize;
            YSpawnPosition = -1* settings.GridSquareSize;

            XSpawnOffset = (GridWidth / settings.NumPlayers) * settings.GridSquareSize;

            BufferedPieceX = -5* settings.GridSquareSize;
            BufferedPieceY = settings.GridSquareSize;

            NextPiecesX = (GridWidth + 3) * settings.GridSquareSize;
            NextPiecesY = settings.GridSquareSize;

            NextPiecesWidth = 4* settings.GridSquareSize;
            NextPiecesHeight = 5* settings.NextPiecesAmount * settings.GridSquareSize;

            currentPieces = new Tetromino[settings.NumPlayers];

            positions = new int[settings.NumPlayers][];

            realPositionsY = new float[settings.NumPlayers];
            gravityMultipliers = new float[settings.NumPlayers];
            
            bufferedPieces = new Tetromino.Type[settings.NumPlayers] ;
            swapped = new bool[settings.NumPlayers];
            firstBuffers = new bool[settings.NumPlayers];
            bufferedPieceGrids = new Color[settings.NumPlayers][];
            bufferedPieceTextures = new Texture2D[settings.NumPlayers];
            
            pieceQueues = new Queue<Tetromino.Type>[settings.NumPlayers];
            
            nextPiecesGrids = new Color[settings.NumPlayers][];
            nextPiecesTextures = new Texture2D[settings.NumPlayers];
            
            phantomPositions = new Vector2[settings.NumPlayers];
            
            phantomDropGrids = new Color[settings.NumPlayers][];
            phantomDropTextures = new Texture2D[settings.NumPlayers];
            
            Gravity = (2*settings.GridSquareSize)/60f;
            
            gameGrid = new Color[GridWidth*settings.GridHeight];


            squareIndicatorGrid = new Color[settings.GridSquareSize*settings.GridSquareSize];            
            
            
            
            
            
            
            #endregion
            
            


            #region Display Settings
            
            graphics.IsFullScreen = settings.Fullscreen;
            graphics.PreferredBackBufferWidth = settings.ScreenWidth;
            graphics.PreferredBackBufferHeight = settings.ScreenHeight;

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
            
            // Random
            if (settings.Seed < 0)
            {
                Random r = new Random();
                settings.Seed = r.Next();
            }

            random = new Random(settings.Seed);

            // other classes
            frameCounter = new FrameCounter();
            inputHandler = new Inputs();
            playerControllerManager = new PlayerControllerManager();


            
            // initialisation
            //scale = Matrix.CreateScale(new Vector3(settings.GridSquareSize, settings.GridSquareSize, 1));

            // Colours
            transparentBlack = new Color(0, 0, 0, 100);
            darkGray = new Color(50, 50, 50);
            darkerGray = new Color(30, 30, 30);
            phantomColourDefault = new Color(120,120,120,100);

            pointsColor = new Color(pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha);


            // 3D audio stuff
            audioManager = new AudioManager(this);
            Components.Add(audioManager);
            audioEmitter = new AudioEmitter();
            soundPosition = Vector3.Zero;
            
            
            
            // texture setup
            gameTexture = new Texture2D(graphics.GraphicsDevice, GridWidth, settings.GridHeight);
            gameTexture.SetData(gameGrid);
            gameTexture2 = new Texture2D(graphics.GraphicsDevice, GridWidth, settings.GridHeight);
            gameTexture2.SetData(gameGrid);

            darkGrayPixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            blackPixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            darkGrayPixel.SetData(new Color[]{new Color(60,60,60)});        
            blackPixel.SetData(new Color[]{Color.Black});

            // decoration on squares

            squareIndicatorTexture = new Texture2D(graphics.GraphicsDevice, settings.GridSquareSize, settings.GridSquareSize);

            if (settings.SquareIndicatorType != Settings.SIT.FullBorder)
            {
                

                if (settings.SquareIndicators)
                {
                    int padding = settings.GridSquareSize / settings.SquareIndicatorPadding;
                    int thickness = 2;

                    for (int i = padding; i < settings.GridSquareSize - padding; i++)
                    {
                        for (int j = padding; j < settings.GridSquareSize - padding; j++)
                        {

                            if (settings.SquareIndicatorType == Settings.SIT.Border)
                            {
                                if ((i > (padding + thickness - 1) && i < (settings.GridSquareSize - padding - thickness))
                                    && (j > (padding + thickness - 1) && j < (settings.GridSquareSize - padding - thickness)))
                                {
                                    continue;
                                }
                            }



                            squareIndicatorGrid[i + j * settings.GridSquareSize] = transparentBlack;
                        }
                    }


                    
                }
            }
            else
            {
                int xpos, ypos;

                for (int i = 0; i < 2; i++)
                {
                    xpos = i*settings.GridSquareSize -i;


                    for (int j = 0; j < settings.GridSquareSize; j++)
                    {
                        ypos = j;
                        squareIndicatorGrid[xpos + ypos * settings.GridSquareSize] = transparentBlack;
                    }

                    ypos = i * settings.GridSquareSize - i;

                    for (int j = 0; j < settings.GridSquareSize; j++)
                    {
                        xpos = j;
                        squareIndicatorGrid[xpos + ypos * settings.GridSquareSize] = transparentBlack;
                    }

                }
            }
            
            squareIndicatorTexture.SetData(squareIndicatorGrid);



            // per player initialisation
            for (int i = 0; i < settings.NumPlayers; i++)
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
                for (int j = 0; j < 2; j++)
                {
                    Tetromino.Type[] bag = GenerateBag();
                    foreach (Tetromino.Type piece in bag)
                    {
                        pieceQueues[i].Enqueue(piece);
                    }
                }
                
                // current piece
                currentPieces[i] = NextRandom(i);
                currentPieces[i].Update();

                nextPiecesTextures[i] = new Texture2D(graphics.GraphicsDevice,NextPiecesWidth/settings.GridSquareSize,NextPiecesHeight/settings.GridSquareSize);
                nextPiecesGrids[i] = new Color[(nextPiecesTextures[i].Width*nextPiecesTextures[i].Height)];

                UpdateNextPiecesDisplay(i);
                
                
                
                
                // phantom
                phantomPositions[i].X = positions[i][0];
                phantomPositions[i].Y = positions[i][1];



                phantomDropTextures[i] = new Texture2D(graphics.GraphicsDevice, 4*((settings.OutlinedPhantomDisplay)?settings.GridSquareSize:1),4*((settings.OutlinedPhantomDisplay)?settings.GridSquareSize:1));
                UpdatePhantom(i);

            }

            #endregion

            #region Grid Display

            if (settings.DisplayGrid)
            {

                //int x, int y, int width, int height, int x_amount, int y_amount, Color color, int thickness, GraphicsDeviceManager _graphics
                displayGrid = new Grid(GridOffsetX + DisplayOffsetX, GridOffsetY + settings.GridSquareSize * 4 + DisplayOffsetY,

                    settings.GridSquareSize, settings.GridSquareSize,

                    GridWidth,
                    settings.GridHeight - 4,

                    Color.Gray, 1, graphics);

            }
            else
            {
                int borderWidth = GridWidth * settings.GridSquareSize;
                int borderHeight = (settings.GridHeight - 4) * settings.GridSquareSize;

                GameBorderGrid = new Color[(GridWidth*settings.GridSquareSize)*(settings.GridHeight*settings.GridSquareSize)];
                GameBorderTexture = new Texture2D(graphics.GraphicsDevice, (GridWidth * settings.GridSquareSize) , (settings.GridHeight * settings.GridSquareSize));

                int xpos, ypos;

                for (int i = 0; i < 2; i++)
                {
                    xpos = i*borderWidth -i;


                    for (int j = 0; j < borderHeight; j++)
                    {
                        ypos = j;
                        GameBorderGrid[xpos + ypos * borderWidth] = Color.LightGray;
                    }

                    ypos = i * borderHeight - i;

                    for (int j = 0; j < borderWidth; j++)
                    {
                        xpos = j;
                        GameBorderGrid[xpos + ypos * borderWidth] = Color.LightGray;
                    }

                }

                GameBorderTexture.SetData(GameBorderGrid);
                
            
            }

            #endregion
            
            #region Decorative Border For Buffered & Next Pieces

            int screenGridWidth = graphics.PreferredBackBufferWidth/settings.GridSquareSize;
            int screenGridHeight= graphics.PreferredBackBufferHeight/settings.GridSquareSize;
            backgroundGrid = new Color[screenGridWidth*screenGridHeight];
            backgroundTexture = new Texture2D(graphics.GraphicsDevice,screenGridWidth,screenGridHeight);


            int xOffset;
            int yOffset;
            
            for (int multI = 1; multI < 2; multI++)
            {
                            
                xOffset = (multI*BufferedPieceX) + DisplayOffsetX - settings.GridSquareSize - settings.GridSquareSize*2*(multI-1);
                yOffset = BufferedPieceY + DisplayOffsetY - settings.GridSquareSize;

                xOffset /= settings.GridSquareSize;
                yOffset /= settings.GridSquareSize;

                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (!((i == 0 && j == 0) || (i == 5 && j == 4) || (i == 5 && j == 0) || (i == 0 && j == 4)))
                        {
                            backgroundGrid[(xOffset + i) + (yOffset + j)*screenGridWidth] = (i>0 && i<5 && j>0 && j<4)?darkerGray:darkGray;
                        }

                    }
                }
                
                
                
                xOffset = NextPiecesX + DisplayOffsetX - settings.GridSquareSize + 7*settings.GridSquareSize*(multI-1);
                yOffset = NextPiecesY + DisplayOffsetY - settings.GridSquareSize;
            
                xOffset /= settings.GridSquareSize;
                yOffset /= settings.GridSquareSize;
            
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 5* settings.NextPiecesAmount +1; j++)
                    {
                        if (!((i == 0 && j == 0) || (i == 5 && j == 25) || (i == 5 && j == 0) || (i == 0 && j == 25)))
                        {
                            backgroundGrid[(xOffset + i) + (yOffset + j)*screenGridWidth] = (i>0 && i<5 && j>0 && j%5!=0)?darkerGray:darkGray;
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
            
            #region Image Loading

            if (settings.Fullscreen)
            {
                background = Texture2D.FromFile(graphics.GraphicsDevice,Path.Combine("Content", "img", "background.jpg"));
            }
            

            #endregion

        }
        
        #endregion

        #region Tetris Functions



        private Tetromino NextRandom(int currentPieceIndex)
        {

            Tetromino next = new Tetromino(pieceQueues[currentPieceIndex].Dequeue());

            if (pieceQueues[currentPieceIndex].Count == 7)
            {
                Tetromino.Type[] bag = GenerateBag();
                foreach (Tetromino.Type piece in bag)
                {
                    pieceQueues[currentPieceIndex].Enqueue(piece);
                }
            }

            return next;

        }
        
        private Tetromino.Type[] GenerateBag()
        {
            Tetromino.Type[] bag = new Tetromino.Type[7];

            int i = 0;
            foreach (Tetromino.Type p in tetrominoTypes)
            {
                bag[i++] = p;
            }

            return bag.OrderBy((item) => random.Next()).ToArray();
        }
        
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
            

            int minimumMove = settings.GridHeight;
            for (int j = 0; j < lowestSquares.Count; j++)
            {
                int highestInLine = settings.GridHeight - 1;
                
                // loop k from the lowest square of the tetromino down to the bottom of the grid
                for (int k = ((int) (lowestSquares[j].Y + positions[currentPieceIndex][1]) / settings.GridSquareSize); k < settings.GridHeight; k++)
                {
                    int xCheck = (int) (lowestSquares[j].X + positions[currentPieceIndex][0]) / settings.GridSquareSize - 1;
                    int yCheck = GridWidth * k;
                    
                    // if out of bounds
                    if (xCheck < 0 || xCheck >= GridWidth || yCheck < 0 || yCheck >= GridWidth*settings.GridHeight)
                    {
                        break;
                    }
                    
                    // if the square we are checking is already occupied
                    if (gameGrid[xCheck + yCheck] != Color.Transparent)
                    {
                        highestInLine = k - 1;
                        break;
                    }
                }
                
                // get current actual y position (relative to the bottom of the tetromino)
                int gridY = (int) lowestSquares[j].Y + positions[currentPieceIndex][1];
                gridY /= settings.GridSquareSize;
                
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
                rotatedSquares.AddRange(Rectangle.RectToSquares(r,settings.GridSquareSize));
            }
            
            Vector2[] kickTransforms = Kick.GetWallKick(currentPiece.BlockType, lastRotation, newRotation);

            foreach (Vector2 transform in kickTransforms)
            {
                bool possible = true;
                
                foreach (Vector2 sq in rotatedSquares)
                {
                    int xCheck = (int) ((sq.X + positions[currentPieceIndex][0]) / settings.GridSquareSize + transform.X) -1;
                    int yCheck = (int) ((sq.Y + positions[currentPieceIndex][1]) / settings.GridSquareSize + transform.Y);
                    yCheck *= GridWidth;
                    
                    
                    if (xCheck>=GridWidth || xCheck<0 || yCheck<0 || yCheck>=GridWidth*settings.GridHeight)
                    {
                        possible = false;
                        break;
                    }
                    
                    if (gameGrid[xCheck + yCheck] != Color.Transparent)
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
            
                for (int j = 0; j < GridWidth*settings.GridHeight; j+=GridWidth)
                {

                    bool row = true;
                
                    for (int i = 0; i < GridWidth; i++)
                    {
                        if (gameGrid[j + i] == Color.Transparent) row = false;
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
                        gameGrid[d + i] = Color.Transparent;
                    }
                }



                int downwards = toDestroy.Count;

                if (downwards != 0)
                {

                    int rows_cleared = downwards;
                
                    Color[] newGrid = new Color[GridWidth*settings.GridHeight];
                    gameGrid.CopyTo(newGrid,0);
                
                    for (int j = 0; j < GridWidth*settings.GridHeight; j += GridWidth)
                    {
                    
                        for (int i = 0; i < GridWidth; i++)
                        {
                            if (j + (downwards * GridWidth) + i < GridWidth*settings.GridHeight)
                            {
                                newGrid[j + (downwards * GridWidth) + i] = gameGrid[j+i];
                            }
                            
                        }

                        if (toDestroy.Contains(j)) downwards--;

                    }
                
                    newGrid.CopyTo(gameGrid,0);

                    displayingPointsEffect = true;
                    pointsCurrentAlpha = 255;
                    pointsColor = new Color(pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha);
                    TotalRowsCleared += rows_cleared;
                    switch (rows_cleared)
                    {
                        case 1:
                            points += 40;
                            pointEffectText = "40";
                            seClearRow.Play();
                            break;
                        case 2:
                            points += 100;
                            pointEffectText = "100";
                            seClearRow.Play();
                            break;
                        case 3:
                            points += 300;
                            pointEffectText = "300";
                            seClearRow.Play();
                            break;
                        case 4:
                            points += 1200;
                            pointEffectText = "1200";
                            // tetris :000000000000000000!!!!! no way omg look how cool
                            sePerfect.Play();
                            shakeY += 11f;
                            break;
                    }


                }
            
        }
        
        private void UpdatePhantom(int currentPieceIndex)
        {

            Tetromino currentPiece = currentPieces[currentPieceIndex];

            List<Vector2> squares = new List<Vector2>();

            foreach (Rectangle r in currentPiece._r)
            {
                squares.AddRange(Rectangle.RectToSquares(r,settings.GridSquareSize));
            }

            if (settings.OutlinedPhantomDisplay)
            {
                phantomDropGrids[currentPieceIndex] = new Color[16* settings.GridSquareSize*settings.GridSquareSize];

                List<Vector2> minX = new List<Vector2>();
                List<Vector2> maxX = new List<Vector2>();
                // for each y position, find min and max x position
                
                List<Vector2> minY = new List<Vector2>();
                List<Vector2> maxY = new List<Vector2>();
                // for each x position, find min and max y position

                for (int i = 0; i < squares.Count; i++)
                {
                    #region find indices
                    int indexMinX = minX.FindIndex(v => (int) v.Y == (int) squares[i].Y);
                    int indexMaxX = maxX.FindIndex(v => (int) v.Y == (int) squares[i].Y);
                    int indexMinY = minY.FindIndex(v => (int) v.X == (int) squares[i].X);
                    int indexMaxY = maxY.FindIndex(v => (int) v.X == (int) squares[i].X);
                    #endregion
                    
                    #region add to list || change current pos to new min / new max
                    if (indexMinX == -1)
                    {
                        minX.Add(squares[i]);
                    }
                    else
                    {
                        // current pos  =  (current pos x > square pos x) -> yes: square pos, no: current pos
                        minX[indexMinX] = (minX[indexMinX].X > squares[i].X) ? squares[i] : minX[indexMinX];
                    }
                    
                    if (indexMaxX == -1)
                    {
                        maxX.Add(squares[i]);
                    }
                    else
                    {
                        // current pos  =  (current pos x < square pos x) -> yes: square pos, no: current pos
                        maxX[indexMaxX] = (maxX[indexMaxX].X < squares[i].X) ? squares[i] : maxX[indexMaxX];
                        
                    }
                    
                    if (indexMinY == -1)
                    {
                        minY.Add(squares[i]);
                    }
                    else
                    {
                        // current pos  =  (current pos y > square pos y) -> yes: square pos, no: current pos
                        minY[indexMinY] = (minY[indexMinY].Y > squares[i].Y) ? squares[i] : minY[indexMinY];
                    }
                    
                    if (indexMaxY == -1)
                    {
                        maxY.Add(squares[i]);
                    }
                    else
                    {
                        // current pos  =  (current pos y < square pos y) -> yes: square pos, no: current pos
                        maxY[indexMaxY] = (maxY[indexMaxY].Y < squares[i].Y) ? squares[i] : maxY[indexMaxY];
                    }
                    
                    #endregion

                    
                }
                

                for (int i = 0; i < settings.GridSquareSize; i++)
                {
                    foreach (Vector2 pos in minX)
                    {
                        phantomDropGrids[currentPieceIndex][(int)(pos.X+ + (pos.Y+i) * (4*settings.GridSquareSize))] = Color.White;
                    }
                
                    foreach (Vector2 pos in maxX)
                    {
                        phantomDropGrids[currentPieceIndex][(int)(pos.X + settings.GridSquareSize -1 + (pos.Y+i) * (4*settings.GridSquareSize))] = Color.White;
                    }
                    
                    foreach (Vector2 pos in minY)
                    {
                        phantomDropGrids[currentPieceIndex][(int)(pos.X+i + (pos.Y) * (4*settings.GridSquareSize))] = Color.White;
                    }
                    
                    foreach (Vector2 pos in maxY)
                    {
                        phantomDropGrids[currentPieceIndex][(int)(pos.X+i + (pos.Y+settings.GridSquareSize-1) * (4*settings.GridSquareSize))] = Color.White;
                    }
                    
                    
                }

                



            }
            else
            {
                phantomDropGrids[currentPieceIndex] = new Color[16];
                foreach (Vector2 square in squares)
                {
                    int x = (int) (square.X / settings.GridSquareSize);
                    int y = (int) (square.Y / settings.GridSquareSize);
                
                    Color currentColour;// = phantomColourDefault;

                    if (settings.PerPlayerPhantomColours)
                    {
                        if (currentPieceIndex < settings.PhantomColours.Length)
                        {
                            currentColour = settings.PhantomColours[currentPieceIndex];
                        }
                        else
                        {
                            currentColour = phantomColourDefault;
                        }
                    }

                    else
                    {
                        currentColour = currentPieces[currentPieceIndex].Colour;
                        currentColour.A = 100;
                    }
                

                    phantomDropGrids[currentPieceIndex][x + y * 4] = currentColour;

                }
            }

                
            phantomDropTextures[currentPieceIndex].SetData(phantomDropGrids[currentPieceIndex]);

            phantomPositions[currentPieceIndex].X = positions[currentPieceIndex][0];

            int minimumMove = MinimumMove(squares,currentPieceIndex,false);

            minimumMove++;
            phantomPositions[currentPieceIndex].Y = positions[currentPieceIndex][1] + minimumMove * settings.GridSquareSize;


        }
        
        private void UpdateBufferedPiece(int currentPieceIndex)
        {
            if (!firstBuffers[currentPieceIndex])
            {
                bufferedPieceGrids[currentPieceIndex] = new Color[8];
            
            
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in Tetromino.Rectangles[(int)bufferedPieces[currentPieceIndex]])
                {
                    squares.AddRange(Rectangle.RectToSquares(r,settings.GridSquareSize));
                }

                int yOffset = 0;

                if (bufferedPieces[currentPieceIndex] == Tetromino.Type.O) yOffset = -1;
                
                foreach (Vector2 sq in squares)
                {
                    int x = (int) sq.X / settings.GridSquareSize;
                    int y = ((int) sq.Y / settings.GridSquareSize) + yOffset;

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

            int next = 0;
            foreach (Tetromino.Type piece in pieceQueues[currentPieceIndex])
            {
                if (next++ == settings.NextPiecesAmount)
                {
                    break;
                }

                if (!settings.BlockDisplayMode)
                {
                    List<Vector2> squares = new List<Vector2>();

                    foreach (Rectangle r in Tetromino.Rectangles[(int)piece])
                    {
                        squares.AddRange(Rectangle.RectToSquares(r,settings.GridSquareSize));
                    }

                    foreach (Vector2 sq in squares)
                    {
                        x = (int) sq.X / settings.GridSquareSize;
                        int yTemp = y + ((int) sq.Y / settings.GridSquareSize);

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

                if (settings.BlockDisplayMode) y += 1;
            }

            nextPiecesTextures[currentPieceIndex].SetData(nextPiecesGrids[currentPieceIndex]);
        }

        private Vector3 GetAudioPosition(int currentPieceIndex)
        {
            double angle = ((double) positions[currentPieceIndex][0]) / ((double) GridWidth*settings.GridSquareSize);

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
                if ((int) realPositionsY[currentPieceIndex] > settings.GridSquareSize)
                {
                    // reset delta y
                    realPositionsY[currentPieceIndex] = 0f;

                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, settings.GridSquareSize));
                    }
                    
                    // find minimum movement downwards until placement
                    int minimumMove = MinimumMove(squares,currentPieceIndex,false);

                    if (minimumMove > 0)
                    {
                        // move downwards
                        positions[currentPieceIndex][1] += settings.GridSquareSize;
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

                        bool lost = false;

                        // set piece in place
                        foreach (Vector2 square in squares)
                        {
                            // set square on game grid
                            gameGrid[((int) ((square.X + positions[currentPieceIndex][0]) / settings.GridSquareSize) - 1) +             // x position
                                     ((((int) ((square.Y + positions[currentPieceIndex][1]) / settings.GridSquareSize))) * GridWidth)]  // y position
                                
                                = CurrentColorPalette[currentPieces[currentPieceIndex].BlockType];                             // current block's colour
                            
                            if ((int)((square.Y + positions[currentPieceIndex][1]) / settings.GridSquareSize) < 3)
                            {
                                lost = true;
                            }
                        }

                        if (lost)
                        {
                            gameGrid = new Color[GridWidth * settings.GridHeight];
                            
                            for (int p = 0; p < settings.NumPlayers; p++)
                            {
                                UpdatePhantom(p);
                            }

                            seClearRow.Play();

                            pointEffectText = ":(";
                            displayingPointsEffect = true;
                            pointsCurrentAlpha = 255;
                            pointsColor = new Color(pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha);


                        }



                        positions[currentPieceIndex][0] = XSpawnPosition + GridOffsetX + XSpawnOffset*currentPieceIndex;
                        positions[currentPieceIndex][1] = YSpawnPosition + GridOffsetY;

                        currentPieces[currentPieceIndex] = NextRandom(currentPieceIndex);

                        currentPieces[currentPieceIndex].Update();
                        
                        // update display of next pieces
                        UpdateNextPiecesDisplay(currentPieceIndex);
                        swapped[currentPieceIndex] = false;
                        UpdateBufferedPiece(currentPieceIndex);

                        Destroy();

                        shakeY += 6f;
                        
                            


                        for (int player = 0; player < settings.NumPlayers; player++)
                        {

                            List<Vector2> currentSquares = new List<Vector2>();
                            
                            foreach (Rectangle r in currentPieces[player]._r)
                            {
                                currentSquares.AddRange(Rectangle.RectToSquares(r, settings.GridSquareSize));
                            }
                            
                            foreach (Vector2 square in currentSquares)
                            {

                                int push = 0;

                                while (true)
                                {
                                    int xCheck = (int) (square.X + positions[player][0]) / settings.GridSquareSize;
                                    int yCheck = (int) (square.Y + positions[player][1]) / settings.GridSquareSize;
                                    yCheck -= push;
                                    xCheck -= 1;

                                    yCheck *= GridWidth;


                                    // if player stuck inside placed piece...

                                    if (gameGrid[xCheck + yCheck] != Color.Transparent)
                                    {
                                        // push up until no longer stuck
                                        push++;

                                        if (!settings.PushUp)
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

                                    if (settings.PushUp)
                                    {
                                        // if piece needs to be pushed up

                                        // push piece up
                                        positions[player][1] -= (push + 2) * settings.GridSquareSize;
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
                        for (int it = 0; it < settings.NumPlayers; it++) { UpdatePhantom(it); }
                        
                    }

                }
                
                #endregion
                
                
                #region Developer Inputs
                
                /*
                if (inputHandler.KeyPressed(Keys.R) && DeveloperMode)
                {
                    gameGrid = new Color[GridWidth * settings.GridHeight];
                    UpdatePhantom(currentPieceIndex);
                }

                if (inputHandler.KeyPressed(Keys.I) && DeveloperMode)
                {
                    currentPieces[currentPieceIndex] = new Tetromino(Tetromino.Type.I);
                    positions[currentPieceIndex][0] = XSpawnPosition + GridOffsetX + XSpawnOffset * currentPieceIndex;
                    positions[currentPieceIndex][1] = YSpawnPosition + GridOffsetY;

                    currentPieces[currentPieceIndex].Update();
                    UpdatePhantom(currentPieceIndex);
                }


                if (inputHandler.KeyPressed(Keys.LeftControl) && DeveloperMode)
                {
                    positions[currentPieceIndex][0] = XSpawnPosition + GridOffsetX + XSpawnOffset * currentPieceIndex;
                    positions[currentPieceIndex][1] = YSpawnPosition + GridOffsetY;

                    currentPieces[currentPieceIndex] = NextRandom(currentPieceIndex);

                    currentPieces[currentPieceIndex].Update();
                    UpdateNextPiecesDisplay(currentPieceIndex);
                    UpdatePhantom(currentPieceIndex);
                }

                if (inputHandler.KeyPressed(Keys.Enter) && DeveloperMode)
                {
                    gameGrid = new Color[GridWidth * settings.GridHeight];
                    currentPieces[currentPieceIndex] = new Tetromino(Tetromino.Type.I);
                    currentPieces[currentPieceIndex].Rotate(1);

                    for (int i = 0; i < GridWidth - 1; i++)
                    {
                        for (int j = settings.GridHeight - 4; j < settings.GridHeight; j++)
                        {
                            int xPos = i;
                            int yPos = j * GridWidth;

                            int b = (int)(i * (255f / ((float)GridWidth - 1)));
                            int g = 255 - (int)((float)(j - (settings.GridHeight - 5)) * 63.75f);
                            int r = 255 - b;

                            gameGrid[xPos + yPos] = new Color(r, g, b);
                        }
                    }

                    positions[currentPieceIndex][0] = (GridWidth - 2) * settings.GridSquareSize;//XSpawnPosition + GridOffsetX + XSpawnOffset * currentPieceIndex;
                    positions[currentPieceIndex][1] = YSpawnPosition + GridOffsetY;

                    currentPieces[currentPieceIndex].Update();
                    UpdatePhantom(currentPieceIndex);

                }
                */
                #endregion
                
                
                #region Movement and Rotation
                
                bool rotateRightPressed = inputHandler.KeyPressed(playerControllerManager.GetControl(settings.ControlsUsedPreset, currentPieceIndex, Controls.RotateRight));
                bool rotateLeftPressed = inputHandler.KeyPressed(playerControllerManager.GetControl(settings.ControlsUsedPreset, currentPieceIndex, Controls.RotateLeft));
                bool moveRightPressed = inputHandler.TimedPress(playerControllerManager.GetControl(settings.ControlsUsedPreset, currentPieceIndex, Controls.MoveRight), settings.InputSpeed, settings.InputWait);
                bool moveLeftPressed = inputHandler.TimedPress(playerControllerManager.GetControl(settings.ControlsUsedPreset, currentPieceIndex, Controls.MoveLeft),settings.InputSpeed,settings.InputWait);

                if (rotateRightPressed)
                {
                    // kick handles all rotation - rotation direction 1, as in clockwise
                    KickResult result = CheckKick(1, currentPieceIndex);
                    if (result.Succeeded)
                    {
                        positions[currentPieceIndex][0] += (int) (settings.GridSquareSize * result.Result.X);
                        positions[currentPieceIndex][1] += (int) (settings.GridSquareSize * result.Result.Y);
                    }

                    UpdatePhantom(currentPieceIndex);
                }

                if (rotateLeftPressed)
                {
                    // kick handles all rotation - rotation direction -1, as in counter-clockwise
                    KickResult result = CheckKick(-1, currentPieceIndex);
                    if (result.Succeeded)
                    {
                        positions[currentPieceIndex][0] += (int)(settings.GridSquareSize * result.Result.X);
                        positions[currentPieceIndex][1] += (int)(settings.GridSquareSize * result.Result.Y);
                    }
                    
                    UpdatePhantom(currentPieceIndex);
                }

                if (moveRightPressed)
                {
                    //seFastScroll.Play();
                    soundPosition = GetAudioPosition(currentPieceIndex);
                    audioEmitter.Update(soundPosition);
                    audioManager.Play3DSound(seFastScroll,false,audioEmitter);
                    
                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, settings.GridSquareSize));
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
                        int xCheck = (int)(rightMostSquares[j].X + positions[currentPieceIndex][0]) / settings.GridSquareSize;
                        int yCheck = (int)((rightMostSquares[j].Y + positions[currentPieceIndex][1]) / settings.GridSquareSize) * GridWidth;
                        
                        // if out of bounds
                        if (xCheck >= GridWidth || yCheck >= settings.GridHeight*GridWidth || xCheck < 0 || yCheck < 0)
                        {
                            canMove *= 0;
                            break;
                        }
                        
                        // if this square is filled in
                        if (gameGrid[xCheck + yCheck] != Color.Transparent)
                        {
                            canMove *= 0;
                        }


                    }

                    // inc. x position
                    positions[currentPieceIndex][0] += settings.GridSquareSize * canMove;

                    if (canMove == 0)
                    {
                        shakeX += 3f;
                    }
                    
                    

                    
                    UpdatePhantom(currentPieceIndex);

                }

                if (moveLeftPressed)
                {
                    //seFastScroll.Play();
                    soundPosition = GetAudioPosition(currentPieceIndex);
                    audioEmitter.Update(soundPosition);
                    audioManager.Play3DSound(seFastScroll,false,audioEmitter);
                    
                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, settings.GridSquareSize));
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
                        int xCheck = (int)(leftMostSquares[j].X + positions[currentPieceIndex][0]) / settings.GridSquareSize - 2;
                        int yCheck = (int)((leftMostSquares[j].Y + positions[currentPieceIndex][1]) / settings.GridSquareSize) * GridWidth;

                        // if out of bounds
                        if (xCheck >= GridWidth || yCheck >= settings.GridHeight*GridWidth || xCheck < 0 || yCheck < 0)
                        {
                            canMove *= 0;
                            break;
                        }
                        else
                        {
                            // if this square is filled in
                            if (gameGrid[xCheck + yCheck] != Color.Transparent)
                            {
                                canMove *= 0;
                            }
                        }

                    }
                    
                    // dec. x position
                    positions[currentPieceIndex][0] -= settings.GridSquareSize * canMove;
                    
                    if (canMove == 0)
                    {
                        shakeX -= 3f;
                    }

                    


                    UpdatePhantom(currentPieceIndex);
                    
                    
                }

                #endregion
                

                #region Extra Inputs

                bool holdPressed = inputHandler.KeyPressed(playerControllerManager.GetControl(settings.ControlsUsedPreset, currentPieceIndex, Controls.Hold));
                bool softDropPressed = inputHandler.KeyHeld(playerControllerManager.GetControl(settings.ControlsUsedPreset, currentPieceIndex, Controls.SoftDrop));
                bool hardDropPressed = inputHandler.KeyPressed(playerControllerManager.GetControl(settings.ControlsUsedPreset, currentPieceIndex, Controls.HardDrop));

                if (holdPressed && !swapped[currentPieceIndex])
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
                        currentPieces[currentPieceIndex] = NextRandom(currentPieceIndex);
                    }
                    
                    // update relevant displays
                    UpdateBufferedPiece(currentPieceIndex);
                    currentPieces[currentPieceIndex].Update();
                    UpdateNextPiecesDisplay(currentPieceIndex);
                    UpdatePhantom(currentPieceIndex);

                }
                
                if (softDropPressed)
                {
                    gravityMultipliers[currentPieceIndex] = settings.SoftDropAmount;
                }
                else
                {
                    // normal gravity speed
                    gravityMultipliers[currentPieceIndex] = 1f;
                }
                
                if (hardDropPressed)
                {
                    // play placement sound effect
                    sePlaceBlock.Play();

                    int[] minimumMoves = new int[settings.NumPlayers];
                    List<Vector2>[] eachLowestSquares = new List<Vector2>[settings.NumPlayers];
                    List<Vector2>[] eachSquares = new List<Vector2>[settings.NumPlayers];

                    for (int i = 0; i < settings.NumPlayers; i++)
                    {
                        // split tetromino into squares
                        eachSquares[i] = new List<Vector2>();
                        foreach (Rectangle r in currentPieces[i]._r)
                        {
                            eachSquares[i].AddRange(Rectangle.RectToSquares(r, settings.GridSquareSize));
                        }

                        eachLowestSquares[i] = LowestSquares(eachSquares[i]);
                    
                        // find amount to move current piece downwards to place it
                        minimumMoves[i] = MinimumMove(eachLowestSquares[i],i,true);
                        
                        
                        
                    }
                    
                    
                    // the players to force to hard drop
                    List<int> forceHardDrop = new List<int>();
                    
                    
                    for (int i = 0; i < settings.NumPlayers; i++)
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
                        positions[index][1] += settings.GridSquareSize * minimumMoves[index];

                        bool lost = false;

                        // set pixels on grid
                        foreach (Vector2 square in eachSquares[index])
                        {
                            gameGrid[((int)((square.X + positions[index][0]) / settings.GridSquareSize) - 1) + ((((int)((square.Y + positions[index][1]) / settings.GridSquareSize))) * GridWidth)] = CurrentColorPalette[currentPieces[index].BlockType];

                            if ((int) ((square.Y + positions[index][1]) / settings.GridSquareSize) < 3)
                            {
                                lost = true;
                            }
                        }

                        if (lost)
                        {
                            gameGrid = new Color[GridWidth * settings.GridHeight];
                            for (int p = 0; p < settings.NumPlayers; p++)
                            {
                                UpdatePhantom(p);
                            }

                            seClearRow.Play();

                            pointEffectText = ":(";
                            displayingPointsEffect = true;
                            pointsCurrentAlpha = 255;
                            pointsColor = new Color(pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha);

                        }

                        // go to spawn pos
                        positions[index][0] = XSpawnPosition + GridOffsetX + XSpawnOffset*index;
                        positions[index][1] = YSpawnPosition + GridOffsetY;

                        // change piece type
                        currentPieces[index] = NextRandom(currentPieceIndex);

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
                    
                    
                    
                    for (int player = 0; player<settings.NumPlayers; player++)
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
                                int xCheck = (int) (square.X+positions[player][0]) / settings.GridSquareSize;
                                int yCheck = (int) (square.Y+positions[player][1]) / settings.GridSquareSize;
                                yCheck -= push;
                                xCheck -= 1;

                                yCheck *= GridWidth;

                                
                                // if player stuck inside other player...
                                
                                if (gameGrid[xCheck+yCheck] != Color.Transparent)
                                {
                                    // push up until no longer stuck
                                    push++;

                                    if (!settings.PushUp)
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

                                if (settings.PushUp)
                                {
                                    // if piece needs to be pushed up

                                    // push piece up
                                    positions[player][1] -= (push + 2) * settings.GridSquareSize;
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

                    shakeY += 6f;
                    
                    // update all phantoms
                    for (int it = 0; it < settings.NumPlayers; it++) { UpdatePhantom(it); }

                }
                #endregion
                
                
                // + buffer in next position after applying gravity
                realPositionsY[currentPieceIndex] += Gravity * ((positions[currentPieceIndex][1] > 1)? gravityMultipliers[currentPieceIndex]:1f);


            }

            if (shakeY > 0)
            {
                shakeY -= (float) Math.Round(shakeY / shakeDamping, 2);
            }
            
            if (Math.Abs(shakeX) > 0)
            {
                shakeX -= (float) Math.Round(shakeX / shakeDamping, 2);
            }


            if ((pointsCurrentAlpha-= AlphaDecreaseRate) > 0)
            {
                //pointsCurrentAlpha -= AlphaDecreaseRate;

                pointsColor.R = (byte)pointsCurrentAlpha;
                pointsColor.G = (byte)pointsCurrentAlpha;
                pointsColor.B = (byte)pointsCurrentAlpha;
                pointsColor.A = (byte)pointsCurrentAlpha;
            }
            else
            {
                displayingPointsEffect = false;
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

            #region Sprite Batch

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, null);

            if (settings.Fullscreen)
            {
                int dimming = 100;
                spriteBatch.Draw(background,Vector2.Zero, new Color(dimming,dimming,dimming,dimming));
            }

            
            //blackPixel
            
            spriteBatch.Draw(blackPixel,
                new Microsoft.Xna.Framework.Rectangle(settings.GridSquareSize + DisplayOffsetX + (int)shakeX, settings.GridSquareSize + DisplayOffsetY + (int)shakeY,
                    settings.GridSquareSize*gameTexture.Width, settings.GridSquareSize*gameTexture.Height),
                Color.White);
            

            if (settings.DisplayGrid)
            {
                if (settings.GridUnderneath)
                {
                    // draw the grid
                    displayGrid.Draw(spriteBatch, graphics);
                }

            }
            else
            {
                spriteBatch.Draw(GameBorderTexture,
                    new Vector2(GridOffsetX + DisplayOffsetX + (int)shakeX, (int)shakeY+ GridOffsetY + DisplayOffsetY + 4*settings.GridSquareSize),
                    Color.White);
            }

            
            
            if (settings.OutlinedPhantomDisplay)
            {
                // draw phantom pieces
                for (int i = 0; i < settings.NumPlayers; i++)
                {
                
                    spriteBatch.Draw(phantomDropTextures[i],
                        new Vector2((phantomPositions[i].X + DisplayOffsetX + (int)shakeX), (phantomPositions[i].Y + DisplayOffsetY + (int)shakeY)),
                        Color.White);
                
                }
            }
            
            
            // gray rectangle above grid
            spriteBatch.Draw(darkGrayPixel,
                new Microsoft.Xna.Framework.Rectangle(settings.GridSquareSize + DisplayOffsetX + (int)shakeX, settings.GridSquareSize + DisplayOffsetY + (int)shakeY,
                    settings.GridSquareSize*gameTexture2.Width, settings.GridSquareSize*4),
                Color.DarkGray);
            



            // decorative borders for buffer/next pieces
            spriteBatch.Draw(backgroundTexture, 
                new Microsoft.Xna.Framework.Rectangle(0,0, backgroundTexture.Width*settings.GridSquareSize,
                backgroundTexture.Height * settings.GridSquareSize),
                Color.White);
            


            // draw buffered piece (only 0th player's currently)
            spriteBatch.Draw(bufferedPieceTextures[0],
                new Microsoft.Xna.Framework.Rectangle((BufferedPieceX + DisplayOffsetX),
                    (BufferedPieceY + DisplayOffsetY),
                    bufferedPieceTextures[0].Width * settings.GridSquareSize,
                    bufferedPieceTextures[0].Height * settings.GridSquareSize),
                Color.White);
            
            /*
            spriteBatch.Draw(bufferedPieceTextures[1],
                new Vector2(((float)BufferedPieceX*2 + DisplayOffsetX) / GridSquareSize -2,
                    ((float)BufferedPieceY + DisplayOffsetY) / GridSquareSize),
                Color.White)6;
            */


            // draw next pieces queue (only 0th player's currently)
            spriteBatch.Draw(nextPiecesTextures[0],
                new Microsoft.Xna.Framework.Rectangle((NextPiecesX + DisplayOffsetX),
                    (NextPiecesY + DisplayOffsetY),
                    nextPiecesTextures[0].Width * settings.GridSquareSize,
                    nextPiecesTextures[0].Height * settings.GridSquareSize),
                Color.White);
            
            /*
            spriteBatch.Draw(nextPiecesTextures[1],
                new Vector2(((float)NextPiecesX + DisplayOffsetX + NextPiecesWidth + GridSquareSize*3) / GridSquareSize,
                    ((float)NextPiecesY + DisplayOffsetY) / GridSquareSize),
                Color.White);
            */

            
            
            
            
            if (!settings.OutlinedPhantomDisplay)
            {
                // draw phantom pieces
                for (int i = 0; i < settings.NumPlayers; i++)
                {

                    Microsoft.Xna.Framework.Rectangle currentRect = 
                        new Microsoft.Xna.Framework.Rectangle((int) (phantomPositions[i].X + DisplayOffsetX + shakeX)+1,
                        (int) (phantomPositions[i].Y + DisplayOffsetY + (int) shakeY)+1,
                        settings.GridSquareSize * phantomDropTextures[i].Width -1, settings.GridSquareSize * phantomDropTextures[i].Height -1);
                    
                    spriteBatch.Draw(phantomDropTextures[i], currentRect, Color.White);
                
                }
            }
            

            // render all pieces to gameTexture2
            
            Color[] pieceDisplayGrid = new Color[GridWidth * settings.GridHeight];
            for (int i = 0; i < settings.NumPlayers; i++)
            {
                currentPieces[i].Draw(positions[i][0], positions[i][1], pieceDisplayGrid);
                gameTexture2.SetData(pieceDisplayGrid);
            }
            
            gameTexture.SetData(gameGrid);


            // placed pieces
            spriteBatch.Draw(gameTexture,
                new Microsoft.Xna.Framework.Rectangle(settings.GridSquareSize + DisplayOffsetX + (int)shakeX, settings.GridSquareSize + DisplayOffsetY + (int)shakeY,
                    settings.GridSquareSize*gameTexture.Width, settings.GridSquareSize*gameTexture.Height),
                Color.White);
            
            // player pieces
            spriteBatch.Draw(gameTexture2,
                new Microsoft.Xna.Framework.Rectangle(settings.GridSquareSize + DisplayOffsetX + (int)shakeX, settings.GridSquareSize + DisplayOffsetY + (int)shakeY,
                    settings.GridSquareSize*gameTexture2.Width, settings.GridSquareSize*gameTexture2.Height),
                Color.White);



            if (settings.DisplayGrid)
            {
                if (!settings.GridUnderneath)
                {
                    // draw the grid
                    displayGrid.Draw(spriteBatch, graphics);
                }

            }
            else
            {
                spriteBatch.Draw(GameBorderTexture,
                    new Vector2(GridOffsetX+ DisplayOffsetX + (int)shakeX, (int) shakeY + GridOffsetY + DisplayOffsetY + 4 * settings.GridSquareSize),
                    Color.White);
            }

            if (settings.SquareIndicators)
            {

                for (int i = 0; i < GridWidth; i++)
                {
                    for (int j = 0; j < settings.GridHeight; j++)
                    {
                        if (gameGrid[i + j * GridWidth] != Color.Transparent)
                        {
                            spriteBatch.Draw(squareIndicatorTexture,
                                new Vector2(DisplayOffsetX + (1 + i) * settings.GridSquareSize + (int)shakeX, (int)shakeY + DisplayOffsetY + (1 + j) * settings.GridSquareSize),
                                Color.White);
                        }

                        if (settings.SquareIndicatorsOnFalling)
                        {
                            if (pieceDisplayGrid[i + j * GridWidth] != Color.Transparent)
                            {
                                spriteBatch.Draw(squareIndicatorTexture,
                                    new Vector2(DisplayOffsetX + (1 + i) * settings.GridSquareSize + (int)shakeX, (int)shakeY + DisplayOffsetY + (1 + j) * settings.GridSquareSize),
                                    Color.White);
                            }
                        }

                    }
                }

            }
            
            
            /*
            // draw the fps
            spriteBatch.DrawString(spriteFont, (Math.Ceiling(frameCounter.CurrentFramesPerSecond)).ToString(CultureInfo.InvariantCulture),
                Vector2.Zero,
                Color.Gray);
            */
            
            //spriteBatch.DrawString(spriteFont, points+"",
            //    new Vector2(0,settings.ScreenHeight-60),
            //    Color.Gray);
            
            spriteBatch.DrawString(spriteFont, points+"",
                new Vector2(0,settings.ScreenHeight-45),
                Color.Gray, 0f, Vector2.Zero,
                new Vector2(1.5f, 1.5f), SpriteEffects.None, 0f);
            
            //spriteBatch.DrawString(spriteFont, "rows: "+TotalRowsCleared,
            //    new Vector2(0,ScreenHeight-60),
            //    Color.Gray);
            
            
            // draw name
            //spriteBatch.DrawString(spriteFont, ("Luna"),
            //    new Vector2(positions[0][0] + GridSquareSize*7,positions[0][1]),
            //    Color.Gray);


            if (settings.DisplayingNames)
            {

                for (int p = 0; p < settings.NumPlayers; p++)
                {
                    String nameText = playerControllerManager.GetName(settings.ControlsUsedPreset,p);

                    int textOffsetX = (settings.GridSquareSize * 4 - (nameText.Length*27))/2;
                    
                    
                    spriteBatch.DrawString(spriteFont, nameText,
                        new Vector2(DisplayOffsetX + positions[p][0] +textOffsetX + (int)shakeX,shakeY + positions[p][1] +DisplayOffsetY - 10)
                        , Color.Gray, 0f, Vector2.Zero
                        , new Vector2(1, 1), SpriteEffects.None, 0f);
                }

            }


            if (displayingPointsEffect)
            {


                //int textOffsetX = (settings.GridSquareSize * 4 - (pointEffectText.Length * 25 - 1)) / 2;

                float scale = 2;

                spriteBatch.DrawString(spriteFont, pointEffectText,
                    new Vector2(settings.ScreenWidth/2 - (pointEffectText.Length * 27)*(scale/2), settings.ScreenHeight / 2 - 25 * (scale / 2))
                    , pointsColor, 0f, Vector2.Zero
                    , new Vector2(scale, scale), SpriteEffects.None, 0f);
            }


            
            spriteBatch.End();

            #endregion

            #region Post-Draw

            base.Draw(gameTime);
            
            #endregion


        }
        
        #endregion
        

    }
}
