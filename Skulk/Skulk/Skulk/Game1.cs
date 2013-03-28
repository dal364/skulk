#region Using Statements
using System;
using System.IO;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Threading;

#endregion

namespace Skulk
{
    public enum GameState
    {
        Menu,
        Game,
        Over,
        Alert
    }
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
        Texture2D blackTexture;

		Player player;
		torch testObject;
		ArrayList guards;
        GameOverScreen gameOver;
        SpriteFont gameOverFont;
        hud hud;

		//Tile Map stuff
		TileMap myMap;
		int squaresAcross;
		int squaresDown;

        //Menu
        Menu menu;
       
      
        GameState gameState;

        int bestScore;
       
		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            
            gameState = new GameState();
            gameState = GameState.Menu;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
            this.IsMouseVisible = true;

            //TileMap
            myMap = new TileMap();
            
            //Audio
            sound.normalMusic = Content.Load<Song>("hero");
            sound.Alert = Content.Load<Song>("emergence");

            SpriteFont font = Content.Load<SpriteFont>("SpriteFont1");
            blackTexture = new Texture2D(GraphicsDevice, 1, 1);
            blackTexture.SetData(new[] { Color.Black });
          
			Texture2D torchTexture = Content.Load<Texture2D> ("torch");
            gameOverFont = Content.Load<SpriteFont>("GameOver");
            Texture2D timerTexture = Content.Load<Texture2D>("hud");
            Texture2D viewTexture = Content.Load<Texture2D>("view");
            //Menu
            menu = new Menu(this);
            menu.initialize(blackTexture, font, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
           

            // +2 to compensate for tiles off screen
            squaresAcross = GraphicsDevice.Viewport.Width / 64 + 2;
            squaresDown = GraphicsDevice.Viewport.Height / 64 + 2;

            //hud
            hud = new hud(this);
            hud.initializeTimer(timerTexture, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width - 50, GraphicsDevice.Viewport.Height - 50,font);
            hud.initializeView(viewTexture, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
           
           
    

            //Load guards
            loadGuards();

            MediaPlayer.Volume = 0.25f;
			Vector2 start = new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2);
			Texture2D texture = Content.Load<Texture2D>("sprite");
			player = new Player(this);
            player.initialize(start, 0, texture, 9, 15, "Player", myMap);
			testObject = new torch(this);
			testObject.initialize(myMap, 1, 1, 0, 0, torchTexture, "torch");
			base.Initialize ();
				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
           
			//TODO: use this.Content to load your game content here 
			Tile.TileSetTexture = Content.Load<Texture2D>("tileset");
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			KeyboardState ks = Keyboard.GetState();
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				Exit ();
			}
			if(ks.IsKeyDown(Keys.Escape))
				this.Exit();

            if (gameState == GameState.Menu)
            {
                if (ks.IsKeyDown(Keys.Enter))
                    gameState = GameState.Game;
            }
            if (gameState == GameState.Game || gameState == GameState.Alert)
            {
                player.Update(myMap, squaresAcross, squaresDown, gameTime);
                testObject.Update(gameTime);
                testObject.isColliding(player);
                foreach (Npc guard in guards)
                {

                    if (guard.isColliding(player))
                    {
                        gameOver = new GameOverScreen(this);
                        int score = ((int)hud.timer / 1000);
                        if (score > bestScore)
                            bestScore = score;
                        gameOver.initialize(blackTexture, gameOverFont, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, bestScore.ToString());
                        gameState = GameState.Over;
                 
                        break;
                    }
                    if (guard.ISeeYou)
                    {
                        Console.WriteLine(guard.objectID);
                        gameState = GameState.Alert;
                        
                    }
                    
                    guard.Update(gameTime);
                }

                // If all guards can't see me change gamestate from alert to game
                int count = 0;
                foreach (Npc guard in guards)
                {
                    
                    if (!guard.ISeeYou)
                    {
                        count++;
                    }
                    if (count >= guards.Count)
                    {
                        gameState = GameState.Game;
                       
                    }

                }

                if (gameState == GameState.Game)
                {
                    if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong == sound.Alert)
                        MediaPlayer.Play(sound.normalMusic);
                }
                if (gameState == GameState.Alert)
                {
                    Console.WriteLine("ALERT");
                    if (MediaPlayer.Queue.ActiveSong != sound.Alert)
                        MediaPlayer.Play(sound.Alert);
                }
               
                hud.Update(gameTime);
            }
            if (gameState == GameState.Over)
            {
                MediaPlayer.Stop();
                if (ks.IsKeyDown(Keys.Enter))
                {
                    gameState = GameState.Game;
                    this.Initialize();

                }
            }
			// TODO: Add your update logic here			
			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.Black);
			spriteBatch.Begin ();

            Vector2 firstSquare = new Vector2(player.Location.X / 64, player.Location.Y / 64);
			int firstX = (int)firstSquare.X;
			int firstY = (int)firstSquare.Y;

            Vector2 squareOffset = new Vector2(player.Location.X % 64, player.Location.Y % 64);
			int offsetX = (int)squareOffset.X;
			int offsetY = (int)squareOffset.Y;
	
			//draw tile map
			for (int y = 0; y < squaresDown; y++) {
           
				for (int x = 0; x < squaresAcross; x++) {
					foreach (int tileID in myMap.mapCell[x + firstX,y + firstY].BaseTiles) {
						spriteBatch.Draw (
        				Tile.TileSetTexture,
        				new Rectangle (
            				(x * 64) - offsetX, (y * 64) - offsetY, 64, 64),
        					Tile.GetSourceRectangle (tileID),
        					Color.White);
					}
				

				}
			}

			//draw objects on top of tile map
			for (int y = 0; y < squaresDown; y++) {
           
				for (int x = 0; x < squaresAcross; x++) {
					testObject.draw(spriteBatch, x, y, firstX, firstY, offsetX, offsetY);
                    foreach (Npc guard in guards)
                    {
                        guard.draw(spriteBatch, x, y, firstX, firstY, offsetX, offsetY);
                    }
				}
			}

			//draw player
			player.draw (this.spriteBatch);
          

            if (gameState == GameState.Over)
            {
                
                gameOver.Draw(spriteBatch);
            }

            //draw hud
            if (gameState == GameState.Game || gameState == GameState.Alert)
            {
                hud.Draw(this.spriteBatch);
            }

            if (gameState == GameState.Menu)
            {
                menu.Draw(spriteBatch);
            }


            
            
			spriteBatch.End ();
			base.Draw (gameTime);
		}

        /* Read in guards from text file. Each line contains the patrol 
         * points for a guard and a single integer at the end of the line for speed.
         * */
        public void loadGuards()
        {
            guards = new ArrayList();
            Texture2D guardTexture = Content.Load<Texture2D>("guard");
            TextReader gr = new StreamReader("guardData.csv");
            TextReader nr = new StreamReader("guardData.csv");
            String line;
            int numGuards = nr.ReadToEnd().Split('\n').Length - 1;
            for (int i = 0; i < numGuards; i++)
            {
                line = gr.ReadLine();
                int speed = Convert.ToInt32(line.Split('>')[line.Split('>').Length - 1]);
                int numPoints = Convert.ToInt32(line.Split(';').Length - 1);
                String[] points = line.Split(';'); //last part will be speed, not used
                Point[] patrolPoints = new Point[numPoints];
                for (int j = 0; j < numPoints; j++)
                {
                    String[] point = points[j].Split(',');
                    int posX = Convert.ToInt32(point[0]);
                    int posy = Convert.ToInt32(point[1]);
                    patrolPoints[j] = new Point(posX, posy);

                }
                Npc guard = new Npc(this);
                guard.initialize(myMap, patrolPoints[0].X, patrolPoints[0].Y, 0, 0, guardTexture, "guard" + i, patrolPoints, speed);
                guards.Add(guard);

            }
            gr.Close();
            nr.Close();
        }
	}
}

