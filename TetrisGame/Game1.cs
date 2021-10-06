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
using MultiplayerTetris.Network;

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

        private RenderTarget2D backgroundRenderTarget;
        private RenderTarget2D shakeRenderTarget;
        private RenderTarget2D nextPiecesRenderTarget;
        private RenderTarget2D bufferPieceRenderTarget;
        private RenderTarget2D[] nameRenderTargets;

        private int[] nameOffsets;

        private Vector2 pointsDisplayPos;
        private Microsoft.Xna.Framework.Rectangle playerPiecesRect;
        private Microsoft.Xna.Framework.Rectangle nextPiecesRect;
        private Microsoft.Xna.Framework.Rectangle placedPiecesRect;
        private Microsoft.Xna.Framework.Rectangle bufferedPieceRect;

        private float deltaTime;

        private bool moveRightPressed = false;
        private bool moveLeftPressed = false;
        private bool lastRight = false;
        private bool lastLeft = false;
        private bool rightLock = false;
        private bool leftLock = false;
        
        private const bool DeveloperMode = false;
        private const float AlphaDecreaseRate = 4f;
        private const float BonusAlphaDecreaseRate = 2.5f;

        private bool collidingLeft = false;
        private bool collidingRight = false;
        
        private float pointsCurrentAlpha = 255;
        private Color pointsColor;
        private bool displayingPointsEffect;
        private String pointEffectText = "";

        private int points = 0;
        private int totalRowsCleared = 0;
        private int combo = 0;
        private int b2b = 0;
        
        private float comboCurrentAlpha = 0f;
        private Color comboColor;
        private bool displayingComboEffect;
        private String comboEffectText = "";
        
        private float b2bCurrentAlpha = 255;
        private Color b2bColor;
        private bool displayingB2BEffect;
        private String b2bEffectText = "";
        

        private float shakeX = 0;
        private float shakeY = 0;
        private readonly float shakeDamping = 10;
        private float shakeWait = 0;
        

        private readonly Array tetrominoTypes =  typeof(Tetromino.Type).GetEnumValues();
        
        public static int GridWidth;
        
        private int displayOffsetX;
        private int displayOffsetY;

        private int gridOffsetX;
        private int gridOffsetY;

        private int gridL;
        private int gridR;

        private int xSpawnPosition;
        private int ySpawnPosition;

        private int xSpawnOffset;

        private int bufferedPieceX;
        private int bufferedPieceY;

        private int nextPiecesX;
        private int nextPiecesY;

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
        
        private Color[] gameBorderGrid;
        private Texture2D gameBorderTexture;

        private Texture2D squareIndicatorTexture;

        private Color[] backgroundGrid;
        private Texture2D backgroundTexture;
        
        private Texture2D whitePixel;
        private Texture2D grayPixel;
        private Texture2D darkGrayPixel;
        private Texture2D blackPixel;

        public static TetrominoColorPalette CurrentColorPalette;

        private FrameCounter frameCounter;
        private Inputs inputHandler;
        private PlayerControllerManager playerControllerManager;

        private SettingsManager settingsManager;
        public static Settings Settings;

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

        private NetworkManager networkManager;
        
        #endregion

        #endregion
        
        #region Setup
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            
        }

        protected override void Initialize()
        {


            #region Calculated Variables
            
            
            settingsManager = new SettingsManager();
            Settings = settingsManager.GetSettings(settingsManager.GetRequestedPreset());
            
            
            GridWidth = 10 * Settings.NumPlayers;
            
            displayOffsetX = Settings.GridSquareSize * ((Settings.ScreenWidth/Settings.GridSquareSize - GridWidth) / 2 -1);
            displayOffsetY = Settings.GridSquareSize;//* ((ScreenHeight / GridSquareSize - GridHeight) / 2 - 1);

            gridOffsetX = Settings.GridSquareSize;
            gridOffsetY = Settings.GridSquareSize;

            gridL = gridOffsetX;
            gridR = gridOffsetX + GridWidth * Settings.GridSquareSize;

            xSpawnPosition = 3* Settings.GridSquareSize;
            ySpawnPosition = -1* Settings.GridSquareSize;

            xSpawnOffset = (GridWidth / Settings.NumPlayers) * Settings.GridSquareSize;

            bufferedPieceX = -6* Settings.GridSquareSize;
            bufferedPieceY = Settings.GridSquareSize;

            nextPiecesX = (GridWidth + 3) * Settings.GridSquareSize;
            nextPiecesY = Settings.GridSquareSize;

            NextPiecesWidth = 5* Settings.GridSquareSize;
            NextPiecesHeight = 3* Settings.NextPiecesAmount * Settings.GridSquareSize;

            currentPieces = new Tetromino[Settings.NumPlayers];

            positions = new int[Settings.NumPlayers][];

            realPositionsY = new float[Settings.NumPlayers];
            gravityMultipliers = new float[Settings.NumPlayers];
            
            bufferedPieces = new Tetromino.Type[Settings.NumPlayers] ;
            swapped = new bool[Settings.NumPlayers];
            firstBuffers = new bool[Settings.NumPlayers];
            bufferedPieceGrids = new Color[Settings.NumPlayers][];
            bufferedPieceTextures = new Texture2D[Settings.NumPlayers];
            
            pieceQueues = new Queue<Tetromino.Type>[Settings.NumPlayers];
            
            nextPiecesGrids = new Color[Settings.NumPlayers][];
            nextPiecesTextures = new Texture2D[Settings.NumPlayers];
            
            phantomPositions = new Vector2[Settings.NumPlayers];
            
            phantomDropGrids = new Color[Settings.NumPlayers][];
            phantomDropTextures = new Texture2D[Settings.NumPlayers];
            
            Gravity = (2*Settings.GridSquareSize)/60f;
            
            gameGrid = new Color[GridWidth*Settings.GridHeight];


            squareIndicatorGrid = new Color[Settings.GridSquareSize*Settings.GridSquareSize];

            #endregion

            #region Display Settings
            
            graphics.IsFullScreen = Settings.Fullscreen;
            graphics.PreferredBackBufferWidth = Settings.ScreenWidth;
            graphics.PreferredBackBufferHeight = Settings.ScreenHeight;

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
            if (Settings.Seed < 0)
            {
                Random r = new Random();
                Settings.Seed = r.Next();
            }

            random = new Random(Settings.Seed);

            // other classes
            frameCounter = new FrameCounter();
            inputHandler = new Inputs();
            playerControllerManager = new PlayerControllerManager();


            pointsDisplayPos = new Vector2(
                Settings.GridSquareSize + displayOffsetX + (int) shakeX +
                ((float) GridWidth / 2f) * Settings.GridSquareSize - ((float) ((points + "").Length) * 20.25f),
                Settings.GridSquareSize + displayOffsetY + (int) shakeY +
                Settings.GridHeight * Settings.GridSquareSize + 12);
            
            
            
            // initialisation
            //scale = Matrix.CreateScale(new Vector3(settings.GridSquareSize, settings.GridSquareSize, 1));

            // Colours
            transparentBlack = new Color(0, 0, 0, 100);
            darkGray = new Color(50, 50, 50);
            darkerGray = new Color(30, 30, 30);
            phantomColourDefault = new Color(120,120,120,100);

            pointsColor = new Color(pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha);
            comboColor = new Color(comboCurrentAlpha,comboCurrentAlpha,comboCurrentAlpha,comboCurrentAlpha);
            b2bColor = new Color(b2bCurrentAlpha,b2bCurrentAlpha,b2bCurrentAlpha,b2bCurrentAlpha);

            // 3D audio stuff
            audioManager = new AudioManager(this);
            Components.Add(audioManager);
            audioEmitter = new AudioEmitter();
            soundPosition = Vector3.Zero;
            
            if (DeveloperMode)
            {
                for (int i = 0; i < GridWidth - 1; i++)
                {
                    for (int j = Settings.GridHeight - 16; j < Settings.GridHeight; j++)
                    {
                        int xPos = i;
                        int yPos = j * GridWidth;

                        int b = (int)(i * (255f / ((float)GridWidth - 1)));
                        int g = 255 - (int)((float)(j - (Settings.GridHeight - 17)) * 16f);
                        int r = 255 - b;

                        gameGrid[xPos + yPos] = new Color(r, g, b);
                    }
                }
            }

            
            
            // texture setup
            gameTexture = new Texture2D(graphics.GraphicsDevice, GridWidth, Settings.GridHeight);
            gameTexture.SetData(gameGrid);
            gameTexture2 = new Texture2D(graphics.GraphicsDevice, GridWidth, Settings.GridHeight);
            gameTexture2.SetData(gameGrid);

            whitePixel = new Texture2D(graphics.GraphicsDevice, 1, 1); 
            grayPixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            darkGrayPixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            blackPixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            
            whitePixel.SetData(new[]{Color.White});
            grayPixel.SetData(new[]{Color.Gray});
            darkGrayPixel.SetData(new[]{new Color(60,60,60)});        
            blackPixel.SetData(new[]{Color.Black});

            // decoration on squares

            squareIndicatorTexture = new Texture2D(graphics.GraphicsDevice, Settings.GridSquareSize, Settings.GridSquareSize);

            if (Settings.SquareIndicatorType != Settings.SIT.FullBorder)
            {
                

                if (Settings.SquareIndicators)
                {
                    int padding = Settings.GridSquareSize / Settings.SquareIndicatorPadding;
                    int thickness = 2;

                    for (int i = padding; i < Settings.GridSquareSize - padding; i++)
                    {
                        for (int j = padding; j < Settings.GridSquareSize - padding; j++)
                        {

                            if (Settings.SquareIndicatorType == Settings.SIT.Border)
                            {
                                if ((i > (padding + thickness - 1) && i < (Settings.GridSquareSize - padding - thickness))
                                    && (j > (padding + thickness - 1) && j < (Settings.GridSquareSize - padding - thickness)))
                                {
                                    continue;
                                }
                            }



                            squareIndicatorGrid[i + j * Settings.GridSquareSize] = transparentBlack;
                        }
                    }


                    
                }
            }
            else
            {
                int xpos, ypos;

                for (int i = 0; i < 2; i++)
                {
                    xpos = i*Settings.GridSquareSize -i;


                    for (int j = 0; j < Settings.GridSquareSize; j++)
                    {
                        ypos = j;
                        squareIndicatorGrid[xpos + ypos * Settings.GridSquareSize] = transparentBlack;
                    }

                    ypos = i * Settings.GridSquareSize - i;

                    for (int j = 0; j < Settings.GridSquareSize; j++)
                    {
                        xpos = j;
                        squareIndicatorGrid[xpos + ypos * Settings.GridSquareSize] = transparentBlack;
                    }

                }
            }
            
            squareIndicatorTexture.SetData(squareIndicatorGrid);



            // per player initialisation
            for (int i = 0; i < Settings.NumPlayers; i++)
            {
                // set positions for each player
                positions[i] = new[]
                {
                    gridOffsetX + xSpawnPosition + xSpawnOffset*i,
                    gridOffsetY + ySpawnPosition
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
                
                if (currentPieces[i].BlockType == Tetromino.Type.O)
                {
                    positions[i][1] -= 1;
                }
                
                currentPieces[i].Update();

                nextPiecesTextures[i] = new Texture2D(graphics.GraphicsDevice,2*NextPiecesWidth/Settings.GridSquareSize,2*NextPiecesHeight/Settings.GridSquareSize);
                nextPiecesGrids[i] = new Color[(nextPiecesTextures[i].Width*nextPiecesTextures[i].Height)];

                UpdateNextPiecesDisplay(i);
                
                
                
                
                // phantom
                phantomPositions[i].X = positions[i][0];
                phantomPositions[i].Y = positions[i][1];



                phantomDropTextures[i] = new Texture2D(graphics.GraphicsDevice, 4*((Settings.OutlinedPhantomDisplay)?Settings.GridSquareSize:1),4*((Settings.OutlinedPhantomDisplay)?Settings.GridSquareSize:1));
                UpdatePhantom(i);

            }

            networkManager = new NetworkManager();

            #endregion

            #region Grid Display

            if (Settings.DisplayGrid)
            {

                //int x, int y, int width, int height, int x_amount, int y_amount, Color color, int thickness, GraphicsDeviceManager _graphics
                displayGrid = new Grid(gridOffsetX + displayOffsetX, gridOffsetY + Settings.GridSquareSize * 4 + displayOffsetY,

                    Settings.GridSquareSize, Settings.GridSquareSize,

                    GridWidth,
                    Settings.GridHeight - 4,

                    Color.Gray, 1, graphics);

            }
            else
            {
                int borderWidth = GridWidth * Settings.GridSquareSize;
                int borderHeight = (Settings.GridHeight - 4) * Settings.GridSquareSize;

                gameBorderGrid = new Color[(GridWidth*Settings.GridSquareSize)*(Settings.GridHeight*Settings.GridSquareSize)];
                gameBorderTexture = new Texture2D(graphics.GraphicsDevice, (GridWidth * Settings.GridSquareSize) , (Settings.GridHeight * Settings.GridSquareSize));

                int xpos, ypos;

                for (int i = 0; i < 2; i++)
                {
                    xpos = i*borderWidth -i;


                    for (int j = 0; j < borderHeight; j++)
                    {
                        ypos = j;
                        gameBorderGrid[xpos + ypos * borderWidth] = Color.LightGray;
                    }

                    ypos = i * borderHeight - i;

                    for (int j = 0; j < borderWidth; j++)
                    {
                        xpos = j;
                        gameBorderGrid[xpos + ypos * borderWidth] = Color.LightGray;
                    }

                }

                gameBorderTexture.SetData(gameBorderGrid);
                
            
            }

            #endregion
            
            #region Pre Render

            backgroundRenderTarget = new RenderTarget2D(GraphicsDevice,
                Settings.ScreenWidth,
                Settings.ScreenHeight,
                false,SurfaceFormat.Color,DepthFormat.Depth24);
            

            shakeRenderTarget = new RenderTarget2D(GraphicsDevice,
                Settings.GridSquareSize * gameTexture.Width,
                Settings.GridSquareSize * (gameTexture.Height-4),
                false,SurfaceFormat.Color,DepthFormat.Depth24);
            
            nextPiecesRenderTarget = new RenderTarget2D(GraphicsDevice,
                (nextPiecesTextures[0].Width * Settings.GridSquareSize / 2 + 3),
                (nextPiecesTextures[0].Height * Settings.GridSquareSize / 2 + Settings.GridSquareSize + 3),
                false,SurfaceFormat.Color,DepthFormat.Depth24);

            bufferPieceRenderTarget = new RenderTarget2D(GraphicsDevice,
                (bufferedPieceTextures[0].Width + 1) * Settings.GridSquareSize + 3,
                (bufferedPieceTextures[0].Height + 1) * Settings.GridSquareSize + 3,
                false,SurfaceFormat.Color,DepthFormat.Depth24);
            

            
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Texture2D.FromFile(graphics.GraphicsDevice,Path.Combine("Content", "img", "background.png"));
            spriteFont = Content.Load<SpriteFont>(Path.Combine("font", "DebugFont"));
            
            
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            #region background
            

            float imageAspectRatioWidth = (float)background.Height/background.Width;
            float imageAspectRatioHeight = (float)background.Width/background.Height;
            
            int newWidth = Settings.ScreenWidth;
            int newHeight = (int) (newWidth * imageAspectRatioWidth);
            
            if (newHeight < Settings.ScreenHeight)
            {
                newHeight = Settings.ScreenHeight;
                newWidth = (int) (newHeight * imageAspectRatioHeight);
            }
            
            int dimming = 100;
            spriteBatch.Draw(background,
                new Microsoft.Xna.Framework.Rectangle((int)(((float)Settings.ScreenWidth - newWidth)/2), (int)(((float)Settings.ScreenHeight - newHeight)/2),
                    newWidth,newHeight),
                new Color(dimming,dimming,dimming,dimming));
            
            #endregion
            
            GraphicsDevice.SetRenderTarget(backgroundRenderTarget);
            spriteBatch.End();
            
            GraphicsDevice.SetRenderTarget(null);
            
            
            nameRenderTargets = new RenderTarget2D[Settings.NumPlayers];
            nameOffsets = new int[Settings.NumPlayers];
            
            for (int i = 0; i < Settings.NumPlayers; i++)
            {
                
                
                String nameText = playerControllerManager.GetName(Settings.ControlsUsedPreset,i);

                nameOffsets[i] = (Settings.GridSquareSize * 4 - (nameText.Length * 27)) / 2;
                
                nameRenderTargets[i] = new RenderTarget2D(GraphicsDevice,
                    27*nameText.Length,27,
                    false,SurfaceFormat.Color,DepthFormat.Depth24);
                
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
                GraphicsDevice.SetRenderTarget(nameRenderTargets[i]);
                GraphicsDevice.Clear(Color.Transparent);
                
                
                spriteBatch.DrawString(spriteFont, nameText,
                    new Vector2(0,0)
                    , Color.Gray, 0f, Vector2.Zero
                    , new Vector2(1, 1), SpriteEffects.None, 0f);
                
                
                spriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);
            }
            
            
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            GraphicsDevice.SetRenderTarget(shakeRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            
            #region shake
            
    
            if (Settings.DisplayGrid)
            {
                if (Settings.GridUnderneath)
                {
                    // draw the grid
                    displayGrid.Draw(spriteBatch, graphics);
                }

            }
            else
            {
                spriteBatch.Draw(gameBorderTexture,
                    new Vector2(0,0),
                    Color.White);
            }
            

            #endregion

            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            
            
            
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            GraphicsDevice.SetRenderTarget(bufferPieceRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            
            #region buffer piece
            
            spriteBatch.Draw(whitePixel,
                new Microsoft.Xna.Framework.Rectangle(0,
                    0,
                    1,
                    (bufferedPieceTextures[0].Height + 1) * Settings.GridSquareSize + 2),
                Color.White);

            spriteBatch.Draw(whitePixel,
                new Microsoft.Xna.Framework.Rectangle(
                    (bufferedPieceTextures[0].Width + 1) * Settings.GridSquareSize + 2,
                    0,
                    1,
                    (bufferedPieceTextures[0].Height + 1) * Settings.GridSquareSize + 2),
                Color.White);

            spriteBatch.Draw(whitePixel,
                new Microsoft.Xna.Framework.Rectangle(0,
                    0,
                    (bufferedPieceTextures[0].Width + 1) * Settings.GridSquareSize + 2,
                    1),
                Color.White);

            spriteBatch.Draw(whitePixel,
                new Microsoft.Xna.Framework.Rectangle(0,
                    (bufferedPieceTextures[0].Height + 1) * Settings.GridSquareSize + 2,
                    (bufferedPieceTextures[0].Width + 1) * Settings.GridSquareSize + 2,
                    1),
                Color.White);

            #endregion

            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            
            
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            GraphicsDevice.SetRenderTarget(nextPiecesRenderTarget);
            GraphicsDevice.Clear(Color.Black);
            
            #region next pieces
            
            #region 4 lines for next pieces

            spriteBatch.Draw(whitePixel,
                new Microsoft.Xna.Framework.Rectangle(0,
                    0,
                    1,
                    nextPiecesTextures[0].Height * Settings.GridSquareSize / 2 + Settings.GridSquareSize + 2),
                Color.White);

            spriteBatch.Draw(whitePixel,
                new Microsoft.Xna.Framework.Rectangle(
                    nextPiecesTextures[0].Width * Settings.GridSquareSize / 2 + 2,
                    0,
                    1,
                    nextPiecesTextures[0].Height * Settings.GridSquareSize / 2 + Settings.GridSquareSize + 2),
                Color.White);

            spriteBatch.Draw(whitePixel,
                new Microsoft.Xna.Framework.Rectangle(0,
                    0,
                    nextPiecesTextures[0].Width * Settings.GridSquareSize / 2 + 2,
                    1),
                Color.White);

            spriteBatch.Draw(whitePixel,
                new Microsoft.Xna.Framework.Rectangle(0,
                    nextPiecesTextures[0].Height * Settings.GridSquareSize / 2 +
                    Settings.GridSquareSize + 2,
                    nextPiecesTextures[0].Width * Settings.GridSquareSize / 2 + 2,
                    1),
                Color.White);

            #endregion
            
            #endregion
            
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            
            
            #endregion
            
            #region Rects
            
            playerPiecesRect = new Microsoft.Xna.Framework.Rectangle(
                Settings.GridSquareSize + displayOffsetX,
                Settings.GridSquareSize + displayOffsetY,
                Settings.GridSquareSize * gameTexture2.Width, Settings.GridSquareSize * gameTexture2.Height);
            
            nextPiecesRect = new Microsoft.Xna.Framework.Rectangle(
                (nextPiecesX + displayOffsetX),
                (nextPiecesY + displayOffsetY),
                nextPiecesTextures[0].Width * Settings.GridSquareSize / 2,
                nextPiecesTextures[0].Height * Settings.GridSquareSize / 2);
            
            placedPiecesRect = new Microsoft.Xna.Framework.Rectangle(
                Settings.GridSquareSize + displayOffsetX,
                Settings.GridSquareSize + displayOffsetY,
                Settings.GridSquareSize * gameTexture.Width, Settings.GridSquareSize * gameTexture.Height);

            bufferedPieceRect = new Microsoft.Xna.Framework.Rectangle((bufferedPieceX + displayOffsetX),
                (bufferedPieceY + displayOffsetY),
                bufferedPieceTextures[0].Width * Settings.GridSquareSize,
                bufferedPieceTextures[0].Height * Settings.GridSquareSize);
            
            #endregion
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            
            
            #region Sound Effect Loading
    
            sePlaceBlock = SoundEffect.FromFile(Path.Combine("Content", "se", "place_block.wav"));
            seClearRow = SoundEffect.FromFile(Path.Combine("Content", "se", "clear_row.wav"));
            seFastScroll = SoundEffect.FromFile(Path.Combine("Content", "se", "fast_scroll.wav"));
            sePerfect = SoundEffect.FromFile(Path.Combine("Content", "se", "perfect.wav"));
            
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

            if (DeveloperMode)
            {
                Tetromino.Type[] bag = new Tetromino.Type[7];
                for (int i = 0; i < 7; i++)
                {
                    bag[i] = Tetromino.Type.I;
                }

                return bag;
            }
            else
            {
                Tetromino.Type[] bag = new Tetromino.Type[7];

                int i = 0;
                foreach (Tetromino.Type p in tetrominoTypes)
                {
                    bag[i++] = p;
                }

                return bag.OrderBy((item) => random.Next()).ToArray();
            }
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
            

            int minimumMove = Settings.GridHeight;
            for (int j = 0; j < lowestSquares.Count; j++)
            {
                int highestInLine = Settings.GridHeight - 1;
                
                // loop k from the lowest square of the tetromino down to the bottom of the grid
                for (int k = ((int) (lowestSquares[j].Y + positions[currentPieceIndex][1]) / Settings.GridSquareSize); k < Settings.GridHeight; k++)
                {
                    int xCheck = (int) (lowestSquares[j].X + positions[currentPieceIndex][0]) / Settings.GridSquareSize - 1;
                    int yCheck = GridWidth * k;
                    
                    // if out of bounds
                    if (xCheck < 0 || xCheck >= GridWidth || yCheck < 0 || yCheck >= GridWidth*Settings.GridHeight)
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
                gridY /= Settings.GridSquareSize;
                
                // work out the minimum of all distances between the
                // bottom of the tetromino and the top of the stack at that x-pos
                minimumMove = Math.Min(minimumMove, highestInLine - gridY);
            }

            return minimumMove;
        }
        
        /*
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
        */
        
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
                rotatedSquares.AddRange(Rectangle.RectToSquares(r,Settings.GridSquareSize));
            }
            
            Vector2[] kickTransforms = Kick.GetWallKick(currentPiece.BlockType, lastRotation, newRotation);

            foreach (Vector2 transform in kickTransforms)
            {
                bool possible = true;
                
                foreach (Vector2 sq in rotatedSquares)
                {
                    int xCheck = (int) ((sq.X + positions[currentPieceIndex][0]) / Settings.GridSquareSize + transform.X) -1;
                    int yCheck = (int) ((sq.Y + positions[currentPieceIndex][1]) / Settings.GridSquareSize + transform.Y);
                    yCheck *= GridWidth;
                    
                    
                    if (xCheck>=GridWidth || xCheck<0 || yCheck<0 || yCheck>=GridWidth*Settings.GridHeight)
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
            
                for (int j = 0; j < GridWidth*Settings.GridHeight; j+=GridWidth)
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

                if (!DeveloperMode)
                {


                    foreach (int d in toDestroy)
                    {
                        for (int i = 0; i < GridWidth; i++)
                        {
                            gameGrid[d + i] = Color.Transparent;
                        }
                    }

                }
                else
                {
                    
                    gameGrid = new Color[GridWidth * Settings.GridHeight];
                    
                    for (int i = 0; i < GridWidth - 1; i++)
                    {
                        for (int j = Settings.GridHeight - 16; j < Settings.GridHeight; j++)
                        {
                            int xPos = i;
                            int yPos = j * GridWidth;

                            int b = (int)(i * (255f / ((float)GridWidth - 1)));
                            int g = 255 - (int)((float)(j - (Settings.GridHeight - 17)) * 16f);
                            int r = 255 - b;

                            gameGrid[xPos + yPos] = new Color(r, g, b);
                        }
                    }
                }



                int downwards = toDestroy.Count;

                if (downwards != 0)
                {

                    int rowsCleared = downwards;
                    
                    if (!DeveloperMode)
                    {
                    Color[] newGrid = new Color[GridWidth*Settings.GridHeight];
                    gameGrid.CopyTo(newGrid,0);
                
                    for (int j = 0; j < GridWidth*Settings.GridHeight; j += GridWidth)
                    {
                    
                        for (int i = 0; i < GridWidth; i++)
                        {
                            if (j + (downwards * GridWidth) + i < GridWidth*Settings.GridHeight)
                            {
                                newGrid[j + (downwards * GridWidth) + i] = gameGrid[j+i];
                            }
                            
                        }

                        if (toDestroy.Contains(j)) downwards--;

                    }

                    newGrid.CopyTo(gameGrid, 0);
                    
                    
                    }

                    displayingPointsEffect = true;
                    pointsCurrentAlpha = 255;
                    pointsColor = new Color(pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha);
                    totalRowsCleared += rowsCleared;

                    combo += 1;
                    if (combo > 1)
                    {
                        comboCurrentAlpha = 255f;
                        displayingComboEffect = true;
                        comboColor = new Color(comboCurrentAlpha,comboCurrentAlpha,comboCurrentAlpha,comboCurrentAlpha);
                        comboEffectText = "x"+combo;
                    }
                    else
                    {
                        comboCurrentAlpha = 0f;
                        displayingComboEffect = false;
                        comboColor = Color.Transparent;
                        comboEffectText = "";
                    }

                    if (rowsCleared == 4)
                    {
                        b2b++;
                        b2bCurrentAlpha = 255f;
                        displayingB2BEffect = true;
                        b2bColor = new Color(b2bCurrentAlpha,b2bCurrentAlpha,b2bCurrentAlpha,b2bCurrentAlpha);
                        b2bEffectText = b2b + "";

                    }else { b2b*=0; }

                    int incr;
                    
                    switch (rowsCleared)
                    {
                        case 1:
                            incr = 40 * combo;
                            points += incr;
                            pointEffectText = ""+incr;
                            seClearRow.Play();
                            break;
                        case 2:
                            incr = 100 * combo;
                            points += incr;
                            pointEffectText = ""+incr;
                            seClearRow.Play();
                            break;
                        case 3:
                            incr = 300 * combo;
                            points += incr;
                            pointEffectText = ""+incr;
                            seClearRow.Play();
                            break;
                        case 4:
                            incr = 1200 * combo;
                            points += incr;
                            pointEffectText = ""+incr;
                            // tetris :000000000000000000!!!!! no way omg look how cool
                            sePerfect.Play();
                            shakeY += 11f;
                            break;
                    }

                    
                    pointsDisplayPos = new Vector2(
                        Settings.GridSquareSize + displayOffsetX + (int) shakeX +
                        ((float) GridWidth / 2f) * Settings.GridSquareSize - ((float) ((points + "").Length) * 20.25f),
                        Settings.GridSquareSize + displayOffsetY + (int) shakeY +
                        Settings.GridHeight * Settings.GridSquareSize + 12);

                }
                else
                {
                    combo *= 0;
                }
            
        }
        
        private void UpdatePhantom(int currentPieceIndex)
        {

            Tetromino currentPiece = currentPieces[currentPieceIndex];

            List<Vector2> squares = new List<Vector2>();

            foreach (Rectangle r in currentPiece._r)
            {
                squares.AddRange(Rectangle.RectToSquares(r,Settings.GridSquareSize));
            }

            if (Settings.OutlinedPhantomDisplay)
            {
                phantomDropGrids[currentPieceIndex] = new Color[16* Settings.GridSquareSize*Settings.GridSquareSize];

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
                

                for (int i = 0; i < Settings.GridSquareSize; i++)
                {
                    foreach (Vector2 pos in minX)
                    {
                        phantomDropGrids[currentPieceIndex][(int)(pos.X+ + (pos.Y+i) * (4*Settings.GridSquareSize))] = Color.White;
                    }
                
                    foreach (Vector2 pos in maxX)
                    {
                        phantomDropGrids[currentPieceIndex][(int)(pos.X + Settings.GridSquareSize -1 + (pos.Y+i) * (4*Settings.GridSquareSize))] = Color.White;
                    }
                    
                    foreach (Vector2 pos in minY)
                    {
                        phantomDropGrids[currentPieceIndex][(int)(pos.X+i + (pos.Y) * (4*Settings.GridSquareSize))] = Color.White;
                    }
                    
                    foreach (Vector2 pos in maxY)
                    {
                        phantomDropGrids[currentPieceIndex][(int)(pos.X+i + (pos.Y+Settings.GridSquareSize-1) * (4*Settings.GridSquareSize))] = Color.White;
                    }
                    
                    
                }

                



            }
            else
            {
                phantomDropGrids[currentPieceIndex] = new Color[16];
                foreach (Vector2 square in squares)
                {
                    int x = (int) (square.X / Settings.GridSquareSize);
                    int y = (int) (square.Y / Settings.GridSquareSize);
                
                    Color currentColour;// = phantomColourDefault;

                    if (Settings.PerPlayerPhantomColours)
                    {
                        if (currentPieceIndex < Settings.PhantomColours.Length)
                        {
                            currentColour = Settings.PhantomColours[currentPieceIndex];
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
            phantomPositions[currentPieceIndex].Y = positions[currentPieceIndex][1] + minimumMove * Settings.GridSquareSize 
                    -((currentPieces[currentPieceIndex].BlockType == Tetromino.Type.O) ? Settings.GridSquareSize : 0);

            

        }
        
        private void UpdateBufferedPiece(int currentPieceIndex)
        {
            if (!firstBuffers[currentPieceIndex])
            {
                bufferedPieceGrids[currentPieceIndex] = new Color[8];
            
            
                List<Vector2> squares = new List<Vector2>();

                foreach (Rectangle r in Tetromino.Rectangles[(int)bufferedPieces[currentPieceIndex]])
                {
                    squares.AddRange(Rectangle.RectToSquares(r,Settings.GridSquareSize));
                }

                int yOffset = 0;

                if (bufferedPieces[currentPieceIndex] == Tetromino.Type.O) yOffset = -1;
                
                foreach (Vector2 sq in squares)
                {
                    int x = (int) sq.X / Settings.GridSquareSize;
                    int y = ((int) sq.Y / Settings.GridSquareSize) + yOffset;

                    bufferedPieceGrids[currentPieceIndex][x + y * 4] = (swapped[currentPieceIndex])?Color.Gray:CurrentColorPalette[bufferedPieces[currentPieceIndex]];
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
                if (next++ == Settings.NextPiecesAmount)
                {
                    break;
                }

                if (!Settings.BlockDisplayMode)
                {
                    List<Vector2> squares = new List<Vector2>();

                    foreach (Rectangle r in Tetromino.Rectangles[(int)piece])
                    {
                        squares.AddRange(Rectangle.RectToSquares(r,Settings.GridSquareSize));
                    }

                    foreach (Vector2 sq in squares)
                    {
                        x = 2*((int) sq.X / (Settings.GridSquareSize));
                        int yTemp = y + 2*((int) sq.Y / (Settings.GridSquareSize));

                        if (piece != Tetromino.Type.O)
                        {
                            if (piece != Tetromino.Type.I)
                            {
                                x += 2;
                                yTemp += 2;
                            }
                            else
                            {
                                x += 1;
                                yTemp += 1;
                            }
                            
                        }
                        else
                        {
                            x += 1;
                        }
                        
                        


                        int xInc = 0;
                        int yInc = 0;
                        
                        for (int offset = 0; offset < 4; offset++)
                        {

                            

                            if (offset == 2)
                            {
                                xInc -= 2;
                                yInc ++;
                            }

                            nextPiecesGrids[currentPieceIndex][x + xInc + (yTemp + yInc) * nextPiecesTextures[currentPieceIndex].Width] = CurrentColorPalette[piece];

                            xInc ++;
                        }
                        

                    }

                    y+=6;

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

                if (Settings.BlockDisplayMode) y += 1;
            }

            nextPiecesTextures[currentPieceIndex].SetData(nextPiecesGrids[currentPieceIndex]);
        }

        private Vector3 GetAudioPosition(int currentPieceIndex)
        {
            double angle = positions[currentPieceIndex][0] / ((double) GridWidth*Settings.GridSquareSize);

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
            
            
            
            deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            frameCounter.Update(deltaTime);

            deltaTime *= 60;
            
            
            inputHandler.UpdateState();

            // quit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // calculations and inputs for all players
            for (int currentPieceIndex = 0; currentPieceIndex < currentPieces.Length; currentPieceIndex++)
            {
                
                #region Apply Gravity
                
                // if it has gone down by a square
                if ((int) realPositionsY[currentPieceIndex] > Settings.GridSquareSize)
                {
                    // reset delta y
                    realPositionsY[currentPieceIndex] = 0f;

                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, Settings.GridSquareSize));
                    }
                    
                    // find minimum movement downwards until placement
                    int minimumMove = MinimumMove(squares,currentPieceIndex,false);

                    if (minimumMove > 0)
                    {
                        // move downwards
                        positions[currentPieceIndex][1] += Settings.GridSquareSize;
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
                            gameGrid[((int) ((square.X + positions[currentPieceIndex][0]) / Settings.GridSquareSize) - 1) +             // x position
                                     ((((int) ((square.Y + positions[currentPieceIndex][1]) / Settings.GridSquareSize))) * GridWidth)]  // y position
                                
                                = CurrentColorPalette[currentPieces[currentPieceIndex].BlockType];                             // current block's colour
                            
                            if ((int)((square.Y + positions[currentPieceIndex][1]) / Settings.GridSquareSize) < 3)
                            {
                                lost = true;
                            }
                        }

                        if (lost)
                        {
                            gameGrid = new Color[GridWidth * Settings.GridHeight];
                            
                            for (int p = 0; p < Settings.NumPlayers; p++)
                            {
                                UpdatePhantom(p);
                            }

                            seClearRow.Play();

                            pointEffectText = ":(";
                            displayingPointsEffect = true;
                            pointsCurrentAlpha = 255;
                            pointsColor = new Color(pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha, pointsCurrentAlpha);


                        }



                        positions[currentPieceIndex][0] = xSpawnPosition + gridOffsetX + xSpawnOffset*currentPieceIndex;
                        positions[currentPieceIndex][1] = ySpawnPosition + gridOffsetY;

                        currentPieces[currentPieceIndex] = NextRandom(currentPieceIndex);

                        if (currentPieces[currentPieceIndex].BlockType == Tetromino.Type.O)
                        {
                            positions[currentPieceIndex][1] -= 1;
                        }

                        currentPieces[currentPieceIndex].Update();
                        
                        // update display of next pieces
                        UpdateNextPiecesDisplay(currentPieceIndex);
                        swapped[currentPieceIndex] = false;
                        UpdateBufferedPiece(currentPieceIndex);

                        Destroy();

                        shakeY += 6f;

                        for (int player = 0; player < Settings.NumPlayers; player++)
                        {

                            List<Vector2> currentSquares = new List<Vector2>();
                            
                            foreach (Rectangle r in currentPieces[player]._r)
                            {
                                currentSquares.AddRange(Rectangle.RectToSquares(r, Settings.GridSquareSize));
                            }
                            
                            foreach (Vector2 square in currentSquares)
                            {

                                int push = 0;

                                while (true)
                                {
                                    int xCheck = (int) (square.X + positions[player][0]) / Settings.GridSquareSize;
                                    int yCheck = (int) (square.Y + positions[player][1]) / Settings.GridSquareSize;
                                    yCheck -= push;
                                    xCheck -= 1;

                                    yCheck *= GridWidth;


                                    // if player stuck inside placed piece...

                                    if (gameGrid[xCheck + yCheck] != Color.Transparent)
                                    {
                                        // push up until no longer stuck
                                        push++;

                                        if (!Settings.PushUp)
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

                                    if (Settings.PushUp)
                                    {
                                        // if piece needs to be pushed up

                                        // push piece up
                                        positions[player][1] -= (push + 2) * Settings.GridSquareSize;
                                    }
                                    else
                                    {
                                        positions[player][0] = xSpawnPosition + gridOffsetX +
                                                               xSpawnOffset * player;
                                        positions[player][1] = ySpawnPosition + gridOffsetY;
                                    }

                                }
                            }
                        }


                        // update all phantoms
                        for (int it = 0; it < Settings.NumPlayers; it++) { UpdatePhantom(it); }
                        
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
                
                
                bool rotateRightPressed = inputHandler.KeyPressed(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.RotateRight));
                bool rotateLeftPressed = inputHandler.KeyPressed(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.RotateLeft));
                moveRightPressed = (!rightLock) && (inputHandler.TimedPress(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.MoveRight),
                    Settings.InputSpeed, Settings.InputWait, deltaTime,false));
                moveLeftPressed = (!leftLock) && (inputHandler.TimedPress(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.MoveLeft),
                    Settings.InputSpeed,Settings.InputWait, deltaTime,false));

                bool moveRightHeld = (inputHandler.TimedPress(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.MoveRight),
                    Settings.InputSpeed, Settings.InputWait, deltaTime,true));
                bool moveLeftHeld = (inputHandler.TimedPress(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.MoveLeft),
                    Settings.InputSpeed,Settings.InputWait, deltaTime,true));
                
                bool moveRightExtraCheck = (collidingRight&&gravityMultipliers[currentPieceIndex]>1) && moveRightHeld;
                bool moveLeftExtraCheck = (collidingLeft&&gravityMultipliers[currentPieceIndex]>1) && moveLeftHeld;

                bool networkSCreate =
                    inputHandler.KeyPressed(new PlayerControl(false, 0, 0, 0f, 0f, Keys.O, Buttons.BigButton));
                bool networkCConnect = inputHandler.KeyPressed(new PlayerControl(false, 0, 0, 0f, 0f, Keys.P, Buttons.BigButton));

                if (networkSCreate)
                {
                    networkManager.CreateServer(1357, 1);
                }

                if (networkCConnect)
                {
                    networkManager.CreateClient();
                    networkManager.Connect("localhost", 1357);
                }

                if (moveRightHeld && moveLeftHeld && !rightLock && !leftLock)
                {

                    if (lastRight && !lastLeft)
                    {
                        rightLock = true;
                    }

                    if (lastLeft && !lastRight)
                    {
                        leftLock = true;
                    }
                }

                
                
                if (moveRightExtraCheck)
                {
                    moveRightPressed = false;
                }

                if (moveLeftExtraCheck)
                {
                    moveLeftPressed = false;
                }
                
                
                if (rotateRightPressed)
                {
                    // kick handles all rotation - rotation direction 1, as in clockwise
                    KickResult result = CheckKick(1, currentPieceIndex);
                    if (result.Succeeded)
                    {
                        positions[currentPieceIndex][0] += (int) (Settings.GridSquareSize * result.Result.X);
                        positions[currentPieceIndex][1] += (int) (Settings.GridSquareSize * result.Result.Y);
                    }

                    UpdatePhantom(currentPieceIndex);
                }

                if (rotateLeftPressed)
                {
                    // kick handles all rotation - rotation direction -1, as in counter-clockwise
                    KickResult result = CheckKick(-1, currentPieceIndex);
                    if (result.Succeeded)
                    {
                        positions[currentPieceIndex][0] += (int)(Settings.GridSquareSize * result.Result.X);
                        positions[currentPieceIndex][1] += (int)(Settings.GridSquareSize * result.Result.Y);
                    }
                    
                    UpdatePhantom(currentPieceIndex);
                }

                if (moveRightPressed || moveRightExtraCheck)
                {
                    if (!moveRightExtraCheck)
                    {
                        //seFastScroll.Play();
                        soundPosition = GetAudioPosition(currentPieceIndex);
                        audioEmitter.Update(soundPosition);
                        audioManager.Play3DSound(seFastScroll, false, audioEmitter);
                    }

                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, Settings.GridSquareSize));
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

                    bool outOfBounds = false;
                    
                    // loop through the right side of the tetromino
                    for (int j = 0; j < rightMostSquares.Count; j++)
                    {
                        // calculate relative grid positions
                        int xCheck = (int)(rightMostSquares[j].X + positions[currentPieceIndex][0]) / Settings.GridSquareSize;
                        int yCheck = (int)((rightMostSquares[j].Y + positions[currentPieceIndex][1]) / Settings.GridSquareSize) * GridWidth;
                        
                        // if out of bounds
                        if (xCheck >= GridWidth || yCheck >= Settings.GridHeight*GridWidth || xCheck < 0 || yCheck < 0)
                        {
                            outOfBounds = true;
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
                    positions[currentPieceIndex][0] += Settings.GridSquareSize * canMove;

                    collidingRight = false;
                    if (canMove == 0)
                    {
                        if (!moveRightExtraCheck)
                            shakeX += 3f;
                        
                        if (!outOfBounds)
                        {
                            collidingRight = true;
                        }
                    }
                    
                    

                    
                    UpdatePhantom(currentPieceIndex);

                }

                if (moveLeftPressed || moveLeftExtraCheck)
                {

                    if (!moveLeftExtraCheck)
                    {
                        //seFastScroll.Play();
                        soundPosition = GetAudioPosition(currentPieceIndex);
                        audioEmitter.Update(soundPosition);
                        audioManager.Play3DSound(seFastScroll,false,audioEmitter);
                    }

                    // split current tetromino into squares
                    List<Vector2> squares = new List<Vector2>();
                    foreach (Rectangle r in currentPieces[currentPieceIndex]._r)
                    {
                        squares.AddRange(Rectangle.RectToSquares(r, Settings.GridSquareSize));
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

                    bool outOfBounds = false;
                    
                    // loop through the left side of the tetromino
                    for (int j = 0; j < leftMostSquares.Count; j++)
                    {
                        // calculate relative grid positions
                        int xCheck = (int)(leftMostSquares[j].X + positions[currentPieceIndex][0]) / Settings.GridSquareSize - 2;
                        int yCheck = (int)((leftMostSquares[j].Y + positions[currentPieceIndex][1]) / Settings.GridSquareSize) * GridWidth;

                        // if out of bounds
                        if (xCheck >= GridWidth || yCheck >= Settings.GridHeight*GridWidth || xCheck < 0 || yCheck < 0)
                        {
                            outOfBounds = true;
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
                    positions[currentPieceIndex][0] -= Settings.GridSquareSize * canMove;
                    
                    collidingLeft=false;
                    if (canMove == 0)
                    {
                        if (!moveLeftExtraCheck)
                            shakeX -= 3f;
                        if (!outOfBounds)
                        {
                            collidingLeft = true;
                        }
                    }

                    


                    UpdatePhantom(currentPieceIndex);
                    
                    
                }



                lastRight = moveRightHeld;
                lastLeft = moveLeftHeld;
                if (!lastRight || !lastLeft)
                {
                    rightLock = false;
                    leftLock = false;
                }
                
                
                #endregion
                

                #region Extra Inputs

                bool holdPressed = inputHandler.KeyPressed(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.Hold));
                bool softDropPressed = inputHandler.KeyHeld(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.SoftDrop));
                bool hardDropPressed = inputHandler.KeyPressed(playerControllerManager.GetControl(Settings.ControlsUsedPreset, currentPieceIndex, Controls.HardDrop));

                if (holdPressed && !swapped[currentPieceIndex])
                {

                    // limit swaps per go to 1
                    swapped[currentPieceIndex] = true;
                    
                    // swap piece with buffered piece
                    Tetromino.Type temp = currentPieces[currentPieceIndex].BlockType;
                    currentPieces[currentPieceIndex] = new Tetromino(bufferedPieces[currentPieceIndex]);
                    bufferedPieces[currentPieceIndex] = temp;

                    // go back to spawn pos
                    positions[currentPieceIndex][0] = xSpawnPosition + gridOffsetX + xSpawnOffset*currentPieceIndex;
                    positions[currentPieceIndex][1] = ySpawnPosition + gridOffsetY;
                    
                    if (currentPieces[currentPieceIndex].BlockType == Tetromino.Type.O)
                    {
                        positions[currentPieceIndex][1] -= 1;
                    }

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
                    gravityMultipliers[currentPieceIndex] = Settings.SoftDropAmount;
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

                    int[] minimumMoves = new int[Settings.NumPlayers];
                    List<Vector2>[] eachLowestSquares = new List<Vector2>[Settings.NumPlayers];
                    List<Vector2>[] eachSquares = new List<Vector2>[Settings.NumPlayers];

                    for (int i = 0; i < Settings.NumPlayers; i++)
                    {
                        // split tetromino into squares
                        eachSquares[i] = new List<Vector2>();
                        foreach (Rectangle r in currentPieces[i]._r)
                        {
                            eachSquares[i].AddRange(Rectangle.RectToSquares(r, Settings.GridSquareSize));
                        }

                        eachLowestSquares[i] = LowestSquares(eachSquares[i]);
                    
                        // find amount to move current piece downwards to place it
                        minimumMoves[i] = MinimumMove(eachLowestSquares[i],i,true);
                        
                        
                        
                    }
                    
                    
                    // the players to force to hard drop
                    List<int> forceHardDrop = new List<int>();
                    
                    
                    for (int i = 0; i < Settings.NumPlayers; i++)
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
                        positions[index][1] += Settings.GridSquareSize * minimumMoves[index];

                        bool lost = false;

                        // set pixels on grid
                        foreach (Vector2 square in eachSquares[index])
                        {
                            gameGrid[((int)((square.X + positions[index][0]) / Settings.GridSquareSize) - 1) + ((((int)((square.Y + positions[index][1]) / Settings.GridSquareSize))) * GridWidth)] = CurrentColorPalette[currentPieces[index].BlockType];

                            if ((int) ((square.Y + positions[index][1]) / Settings.GridSquareSize) < 3)
                            {
                                lost = true;
                            }
                        }

                        if (lost)
                        {
                            gameGrid = new Color[GridWidth * Settings.GridHeight];
                            for (int p = 0; p < Settings.NumPlayers; p++)
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
                        positions[index][0] = xSpawnPosition + gridOffsetX + xSpawnOffset*index;
                        positions[index][1] = ySpawnPosition + gridOffsetY;

                        // change piece type
                        currentPieces[index] = NextRandom(currentPieceIndex);
                        
                        if (currentPieces[index].BlockType == Tetromino.Type.O)
                        {
                            positions[index][1] -= 1;
                        }

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
                    
                    
                    
                    for (int player = 0; player<Settings.NumPlayers; player++)
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
                                int xCheck = (int) (square.X+positions[player][0]) / Settings.GridSquareSize;
                                int yCheck = (int) (square.Y+positions[player][1]) / Settings.GridSquareSize;
                                yCheck -= push;
                                xCheck -= 1;

                                yCheck *= GridWidth;

                                
                                // if player stuck inside other player...
                                
                                if (gameGrid[xCheck+yCheck] != Color.Transparent)
                                {
                                    // push up until no longer stuck
                                    push++;

                                    if (!Settings.PushUp)
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

                                if (Settings.PushUp)
                                {
                                    // if piece needs to be pushed up

                                    // push piece up
                                    positions[player][1] -= (push + 2) * Settings.GridSquareSize;
                                }
                                else
                                {
                                    positions[player][0] = xSpawnPosition + gridOffsetX +
                                                           xSpawnOffset * player;
                                    positions[player][1] = ySpawnPosition + gridOffsetY;
                                }

                            }

                            
                        }

                        

                        
                        
                    }
                    

                    // check if finished row
                    Destroy();

                    shakeY += 6f;
                    
                    // update all phantoms
                    for (int it = 0; it < Settings.NumPlayers; it++) { UpdatePhantom(it); }

                }
                #endregion
                
                
                // + buffer in next position after applying gravity
                realPositionsY[currentPieceIndex] += Gravity * ((positions[currentPieceIndex][1] > 1)? gravityMultipliers[currentPieceIndex]:1f) * deltaTime;


            }
            
            
            if (shakeWait > 1)
            {
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
                    pointsColor.R = (byte)pointsCurrentAlpha;
                    pointsColor.G = (byte)pointsCurrentAlpha;
                    pointsColor.B = (byte)pointsCurrentAlpha;
                    pointsColor.A = (byte)pointsCurrentAlpha;
                }
                else
                {
                    displayingPointsEffect = false;
                    pointsCurrentAlpha = 0f;
                    pointsColor = Color.Transparent;
                }

                
                if ((comboCurrentAlpha -= BonusAlphaDecreaseRate) > 0)
                {
                    comboColor.R = (byte)comboCurrentAlpha;
                    comboColor.G = (byte)comboCurrentAlpha;
                    comboColor.B = (byte)comboCurrentAlpha;
                    comboColor.A = (byte)comboCurrentAlpha;
                }
                else
                {
                    displayingComboEffect = false;
                    comboCurrentAlpha = 0f;
                    comboColor = Color.Transparent;
                }

                
                if ((b2bCurrentAlpha -= BonusAlphaDecreaseRate) > 0)
                {
                    b2bColor.R = (byte)b2bCurrentAlpha;
                    b2bColor.G = (byte)b2bCurrentAlpha;
                    b2bColor.B = (byte)b2bCurrentAlpha;
                    b2bColor.A = (byte)b2bCurrentAlpha;
                }
                else
                {
                    displayingB2BEffect = false;
                    b2bCurrentAlpha = 0f;
                    b2bColor = Color.Transparent;
                }

                shakeWait = 0;

            }


            shakeWait += deltaTime;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            # region Pre-Draw
            
            
            //GraphicsDevice.Clear(Color.Black);

            #endregion

            #region Sprite Batch

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);

            spriteBatch.Draw(backgroundRenderTarget,Vector2.Zero, Color.White);
            
            spriteBatch.Draw(bufferPieceRenderTarget,
                new Vector2((bufferedPieceX + displayOffsetX) - 1,(bufferedPieceY + displayOffsetY) - 1),
                Color.White);
            
            
            spriteBatch.Draw(nextPiecesRenderTarget,
                new Vector2((nextPiecesX + displayOffsetX) - 1,(nextPiecesY + displayOffsetY) - 1),
                Color.White);


            int shakeXpos = (int) shakeX + (Settings.GridSquareSize + displayOffsetX);
            spriteBatch.Draw(shakeRenderTarget,
                new Vector2(shakeXpos,(int)shakeY+(Settings.GridSquareSize * 5 + displayOffsetY)),
            Color.White);
            
            
            
            if (Settings.OutlinedPhantomDisplay)
            {
                // draw phantom pieces
                for (int i = 0; i < Settings.NumPlayers; i++)
                {
                
                    spriteBatch.Draw(phantomDropTextures[i],
                        new Vector2((phantomPositions[i].X + displayOffsetX + (int)shakeX), (phantomPositions[i].Y + displayOffsetY + (int)shakeY)),
                        Color.White);
                
                }
            }



            bool oddX = !(bufferedPieces[0] == Tetromino.Type.I || bufferedPieces[0] == Tetromino.Type.O);
            bool oddY = (bufferedPieces[0] == Tetromino.Type.I);
            
            
            
            Microsoft.Xna.Framework.Rectangle rect = 
                new Microsoft.Xna.Framework.Rectangle(bufferedPieceRect.X,bufferedPieceRect.Y,bufferedPieceRect.Width,bufferedPieceRect.Height);

            rect.X += ((oddX) ? Settings.GridSquareSize : Settings.GridSquareSize / 2);
            rect.Y += ((oddY) ? 0 : Settings.GridSquareSize / 2);
            
            spriteBatch.Draw(bufferedPieceTextures[0],
                rect,
                Color.White);


            // draw next pieces queue (only 0th player's currently)
            spriteBatch.Draw(nextPiecesTextures[0], nextPiecesRect, Color.White);
            
            
            if (!Settings.OutlinedPhantomDisplay)
            {
                // draw phantom pieces
                for (int i = 0; i < Settings.NumPlayers; i++)
                {

                    Microsoft.Xna.Framework.Rectangle currentRect = 
                        new Microsoft.Xna.Framework.Rectangle((int) (phantomPositions[i].X + displayOffsetX + shakeX)+1,
                            (int) (phantomPositions[i].Y + displayOffsetY + (int) shakeY)+1,
                            Settings.GridSquareSize * phantomDropTextures[i].Width -1, Settings.GridSquareSize * phantomDropTextures[i].Height -1);
                    
                    spriteBatch.Draw(phantomDropTextures[i], currentRect, Color.White);
                
                }
            }
            

            // render all pieces to gameTexture2
            
            Color[] pieceDisplayGrid = new Color[GridWidth * Settings.GridHeight];
            for (int i = 0; i < Settings.NumPlayers; i++)
            {
                currentPieces[i].Draw(positions[i][0], positions[i][1], pieceDisplayGrid);
                gameTexture2.SetData(pieceDisplayGrid);
            }
            
            gameTexture.SetData(gameGrid);

            
            
            rect = 
                new Microsoft.Xna.Framework.Rectangle(placedPiecesRect.X,placedPiecesRect.Y,placedPiecesRect.Width,placedPiecesRect.Height);
            
            
            rect.X += (int) shakeX;
            rect.Y += (int) shakeY;

            // placed pieces
            spriteBatch.Draw(gameTexture,
                rect,
                Color.White);

            

            rect = 
                new Microsoft.Xna.Framework.Rectangle(playerPiecesRect.X,playerPiecesRect.Y,playerPiecesRect.Width,playerPiecesRect.Height);
            
            rect.X += (int) shakeX;
            rect.Y += (int) shakeY;
            // player pieces
            spriteBatch.Draw(gameTexture2,
                rect,
                Color.White);



            if (Settings.SquareIndicators)
            {

                for (int i = 0; i < GridWidth; i++)
                {
                    for (int j = 0; j < Settings.GridHeight; j++)
                    {
                        if (gameGrid[i + j * GridWidth] != Color.Transparent)
                        {
                            spriteBatch.Draw(squareIndicatorTexture,
                                new Vector2(displayOffsetX + (1 + i) * Settings.GridSquareSize + (int)shakeX, (int)shakeY + displayOffsetY + (1 + j) * Settings.GridSquareSize),
                                Color.White);
                        }

                        if (Settings.SquareIndicatorsOnFalling)
                        {
                            if (pieceDisplayGrid[i + j * GridWidth] != Color.Transparent)
                            {
                                spriteBatch.Draw(squareIndicatorTexture,
                                    new Vector2(displayOffsetX + (1 + i) * Settings.GridSquareSize + (int)shakeX, (int)shakeY + displayOffsetY + (1 + j) * Settings.GridSquareSize),
                                    Color.White);
                            }
                        }

                    }
                }

            }
            
            
            // draw the fps
            spriteBatch.DrawString(spriteFont, (Math.Ceiling(frameCounter.CurrentFramesPerSecond)).ToString(CultureInfo.InvariantCulture),
                Vector2.Zero,
                Color.Gray);
            

            spriteBatch.DrawString(spriteFont, points+"",
                pointsDisplayPos,
                Color.Gray, 0f, Vector2.Zero,
                new Vector2(1.5f, 1.5f), SpriteEffects.None, 0f);




            if (Settings.DisplayingNames)
            {
                for (int p = 0; p < Settings.NumPlayers; p++)
                {
                    
                    int nameLength = playerControllerManager.GetName(Settings.ControlsUsedPreset,p).Length *27;

                    bool odd = currentPieces[p].BlockType != Tetromino.Type.O &&
                               currentPieces[p].BlockType != Tetromino.Type.I;


                    int xpos = displayOffsetX + positions[p][0] + (int)shakeX + nameOffsets[p] - ((odd)?Settings.GridSquareSize/2:0);
                    int ypos = (int)(shakeY + positions[p][1] +displayOffsetY - 10);

                    if (xpos < shakeXpos)
                        xpos = shakeXpos;


                    if (xpos + nameLength > shakeXpos + Settings.GridSquareSize * gameTexture.Width)
                        xpos = shakeXpos + Settings.GridSquareSize * gameTexture.Width - nameLength;
                    
                    spriteBatch.Draw(nameRenderTargets[p],
                        new Vector2(xpos,
                            ypos),
                        Color.White);
                }
                

                //displayOffsetX + positions[i][0] + (int)shakeX
                //shakeY + positions[i][1] +displayOffsetY - 10

                //for (int p = 0; p < Settings.NumPlayers; p++)
                //{
                //    String nameText = playerControllerManager.GetName(Settings.ControlsUsedPreset,p);

                //    int textOffsetX = (Settings.GridSquareSize * 4 - (nameText.Length*27))/2;
                //    
                //    
                //    spriteBatch.DrawString(spriteFont, nameText,
                //        new Vector2(displayOffsetX + positions[p][0] +textOffsetX + (int)shakeX,shakeY + positions[p][1] +displayOffsetY - 10)
                //        , Color.Gray, 0f, Vector2.Zero
                //        , new Vector2(1, 1), SpriteEffects.None, 0f);
                //}

            }


            if (displayingPointsEffect)
            {
                //int textOffsetX = (settings.GridSquareSize * 4 - (pointEffectText.Length * 25 - 1)) / 2;

                float scale = 2;

                int xpos = (int)(Settings.GridSquareSize + displayOffsetX + (int) shakeX +
                    (GridWidth * Settings.GridSquareSize / 2) - (pointEffectText.Length * 27) * (scale / 2));

                spriteBatch.DrawString(spriteFont, pointEffectText,
                    new Vector2(xpos, (float)Settings.ScreenHeight / 2 - 25 * (scale / 2))
                    , pointsColor, 0f, Vector2.Zero
                    , new Vector2(scale, scale), SpriteEffects.None, 0f);
            }
            
            
            if (displayingComboEffect)
            {
                float scale = combo;
                if (scale > 5) { scale = 5; }

                int xpos = (int)(displayOffsetX - 20 - comboEffectText.Length*(27 * scale));

                spriteBatch.DrawString(spriteFont, comboEffectText,
                    new Vector2(xpos, (float)Settings.ScreenHeight / 2 - 25 * (scale / 2))
                    , comboColor, 0f, Vector2.Zero
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
