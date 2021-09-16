using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerTetris
{
    public class Game1 : Game
    {



        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //private Rectangle blobby_guy;

        private Tetromino L_piece;

        private int i;
        private int j;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            L_piece = new Tetromino(Tetromino.Type.L);

            i = 0;
            j = 0;

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
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            //blobby_guy.Draw(_spriteBatch, _graphics);
            //Rectangle.Draw(200,200,25,74,Color.Red,_spriteBatch, _graphics);

            L_piece = new Tetromino((Tetromino.Type)j);

            L_piece.Draw(100,100, _spriteBatch, _graphics);

            base.Draw(gameTime);

            i++; if (i > 30) { i = 0; j++; }
            if (j > 6) j = 0;
        }
    }
}
