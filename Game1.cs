using System;
using System.Collections;
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

        private Tetromino L_piece;

        public static TetrominoColorPalette CurrentColorPalette;

        private FrameCounter _frameCounter;

        private int i;
        private int j;
        private int k;

        private bool buffer;

        private BitArray rotation;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            this.MaxElapsedTime = TimeSpan.FromMilliseconds(16.67);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            CurrentColorPalette = new TetrominoColorPalette();
            CurrentColorPalette[Tetromino.Type.I] = new Color(49,199,239);
            CurrentColorPalette[Tetromino.Type.O] = new Color(247,211,8);
            CurrentColorPalette[Tetromino.Type.T] = new Color(173,77,156);
            CurrentColorPalette[Tetromino.Type.S] = new Color(66,182,66);
            CurrentColorPalette[Tetromino.Type.Z] = new Color(239,32,41);
            CurrentColorPalette[Tetromino.Type.J] = new Color(90,101,173);
            CurrentColorPalette[Tetromino.Type.L] = new Color(239,121,33);

            L_piece = new Tetromino(Tetromino.Type.I);

            _spriteFont = Content.Load<SpriteFont>("Font");

            i = 0;
            j = 0;
            k = 0;
            buffer = false;

            rotation = new BitArray(2);
            rotation[0] = false;
            rotation[1] = false;
            
            _frameCounter = new FrameCounter();

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
        
        protected override void Draw(GameTime gameTime)
        {


            // TODO: optimise display of tetrominos - currently creates new rectangles every frame which is very inefficient
            // Edit: kinda fixed maybe





            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);

            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            // TODO: Add your drawing code here

            //blobby_guy.Draw(_spriteBatch, _graphics);
            //Rectangle.Draw(200,200,25,74,Color.Red,_spriteBatch, _graphics);

            L_piece.Draw(100,100, rotation, _spriteBatch, _graphics);


            foreach (Rectangle rect in _gridRectangles)
            {
                rect.Draw(_spriteBatch, _graphics);
            }

            _spriteBatch.DrawString(_spriteFont, _frameCounter.CurrentFramesPerSecond.ToString(), Vector2.Zero, Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);


            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                buffer = true;
            }
            if (!Keyboard.GetState().IsKeyDown(Keys.Space) && buffer)
            {
                j++;
                
                L_piece = new Tetromino((Tetromino.Type)k);
                rotation[0] = ((rotation[0] ^ false) ^ (rotation[1] && true));
                rotation[1] = ((rotation[1] ^ true));
                buffer = false;
            }

            if (j > 3) { j = 0;k++; }
            
            if (k > 6) k = 0;


            i++;
            if (i > 1)
            {
                j++;
                
                L_piece = new Tetromino((Tetromino.Type)k);
                rotation[0] = ((rotation[0] ^ false) ^ (rotation[1] && true));
                rotation[1] = ((rotation[1] ^ true));
                i = 0;
            }
            // addition Console.WriteLine(((waaaaa[0]^ false) ^ (waaaaa[1] && true)) + " " + ((waaaaa[1]^ true) ));
            // subtract Console.WriteLine(((waaaaa[0]^ true) ^ (waaaaa[1] && true)) + " " + ((waaaaa[1]^ true)));
        }
    }
}
