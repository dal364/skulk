#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Skulk
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;
        torch[] torch;
        Texture2D backgroundTexture;
        Vector2 backgroundPosition;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            backgroundPosition = new Vector2(0, 0);
            backgroundTexture = Content.Load<Texture2D>("background");
            //Rectangle background = new Rectangle (0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            Texture2D torchTexture = Content.Load<Texture2D>("torch");
            torch = new torch[20];
            for (int i = 0; i < 10; i++)
            {
                Vector2 torchStart = new Vector2(100, 100 + 50 * i);
                torch[i] = new torch(this);
                torch[i].initialize(torchStart, torchTexture);
            }

            Vector2 start = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Texture2D texture = Content.Load<Texture2D>("sprite");
            player = new Player(this);
            player.initialize(start, 0, texture);
            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();

            for (int i = 0; i < 10; i++)
                torch[i].Update(gameTime);

            player.Update(gameTime);
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, backgroundPosition, Color.White);
            player.draw(this.spriteBatch);

            for (int i = 0; i < 10; i++)
                torch[i].draw(this.spriteBatch);
            //TODO: Add your drawing code here
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
