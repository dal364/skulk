#region Using Statements
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using Microsoft.Xna.Framework.GamerServices;

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
        Vector2 start;
        Texture2D timTexture;
       
		
        GameOverScreen gameOver;
        SpriteFont gameOverFont;
        hud hud;

		//Tile Map stuff
		
		int squaresAcross;
		int squaresDown;
        public int tileSize = 64;
        Texture2D minimap;
        Texture2D dot;
        Texture2D arrow;

        //Levels
   

        Level currentLevel;

        

        //Menu
        Menu menu;
        Rectangle[,] drawnRectangles;
      
        GameState gameState;

        int bestScore = 9999999;
       
		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 480;

			Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            gameState = new GameState();
            gameState = GameState.Menu;
            Components.Add(new GamerServicesComponent(this));
           
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
            TileMap[] lvl1 = new TileMap[2];
            lvl1[0] = new TileMap(1,1);
            lvl1[1] = new TileMap(1,2);
            //Level
            Point[] goal = new Point[2];
            goal[0].X = 25;
            goal[0].Y = 4;
            goal[1].X = 39;
            goal[1].Y = 4;
            Point[] starts = new Point[2];
            starts[0].X = 11;
            starts[0].Y = 21;
            starts[1].X = 10;
            starts[1].Y = 13;
            currentLevel = new Level(lvl1 , goal, starts, 0,2);
          

            
            //Audio
            sound.normalMusic = Content.Load<Song>("hero");
            sound.Alert = Content.Load<Song>("emergence");
            sound.coinSound = Content.Load<SoundEffect>("coinbag");
            sound.fallSound = Content.Load<SoundEffect>("fall");

            SpriteFont font = Content.Load<SpriteFont>("SpriteFont1");
            blackTexture = new Texture2D(GraphicsDevice, 1, 1);
            blackTexture.SetData(new[] { Color.White });
          
			Texture2D torchTexture = Content.Load<Texture2D> ("torch");
            gameOverFont = Content.Load<SpriteFont>("GameOver");
            Texture2D timerTexture = Content.Load<Texture2D>("hud");
            Texture2D viewTexture = Content.Load<Texture2D>("view");
            //Menu
            menu = new Menu(this);
            menu.initialize(blackTexture, font, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
           

            // +2 to compensate for tiles off screen
            squaresAcross = 13 ;
            squaresDown = 9 ;
            
            //hud
            hud = new hud(this);
            hud.initializeTimer(timerTexture, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width - 50, GraphicsDevice.Viewport.Height - 50,font);
            hud.initializeView(viewTexture, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            drawnRectangles = new Rectangle[squaresAcross, squaresDown];
            dot = Content.Load<Texture2D>("whitedot");
            minimap = Content.Load<Texture2D>("minimap");
            arrow = Content.Load<Texture2D>("arrow");

            //Load guards
            loadGuards("lvl1map" + (currentLevel.currentMapIndex + 1)  + "guards.csv");

            //Load Gold
            loadGold("lvl1map" + (currentLevel.currentMapIndex + 1) + "gold.csv");

            MediaPlayer.Volume = 0.25f;
		    start = new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2);
			timTexture = Content.Load<Texture2D>("sprite");
			player = new Player(this);
            player.initialize(start, (float)Math.PI, timTexture,currentLevel.start[0].X,currentLevel.start[0].Y, "Player", currentLevel.currentMap);
			
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
			Tile.TileSetTexture = Content.Load<Texture2D>("Castle2");

		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			KeyboardState ks = Keyboard.GetState();
            GamePadState gs = GamePad.GetState(PlayerIndex.One);
            
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				Exit ();
			}
			if(ks.IsKeyDown(Keys.Escape))
				this.Exit();

            if (gameState == GameState.Menu)
            {
                if (ks.IsKeyDown(Keys.Enter) || gs.Buttons.Start == ButtonState.Pressed)
                {
                    gameState = GameState.Game;
                    Pause.pauseKeyDown = true; 
                }
            }

            

            if (gameState == GameState.Game || gameState == GameState.Alert)
            {
                // Check to see if the user has paused or unpaused
                Pause.checkPauseKey(ks, gs);

                Pause.checkPauseGuide();

                // If the user hasn't paused, Update normally
                if (!Pause.paused)
                {
                    if (ks.IsKeyDown(Keys.Q))
                    {
                        for (int i = 0; i <= currentLevel.itemLocation.Length - 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    if (player.tileX + j == currentLevel.itemLocation[i].X && player.tileY + k == currentLevel.itemLocation[i].Y)
                                    {
                                        player.addGold();
                                        currentLevel.currentMap.mapCell[currentLevel.itemLocation[i].X, currentLevel.itemLocation[i].Y].RemoveBaseTile(229);
                                        currentLevel.currentMap.mapCell[currentLevel.itemLocation[i].X, currentLevel.itemLocation[i].Y].RemoveObject("Gold2");
                                        sound.coinSound.Play();
                                        currentLevel.itemLocation[i] = new Point(-1, -1);


                                    }
                                }
                            }
                        }
                    }

                    player.Update(currentLevel.currentMap, squaresAcross, squaresDown, gameTime, drawnRectangles);

                    //check to see if there are no guards left

                    if (currentLevel.guards.ElementAt(currentLevel.currentMapIndex).Count <= 0)
                    {
                        gameState = GameState.Game;
                    }
                     

                    for (int i = 0; i < currentLevel.guards.ElementAt(currentLevel.currentMapIndex).Count; i++ )
                    {
                        List<Npc> guardList = currentLevel.guards.ElementAt(currentLevel.currentMapIndex);

                        if (guardList.ElementAt(i).isColliding(player))
                        {
                            gameOver = new GameOverScreen(this);
                            gameOver.initialize(blackTexture, gameOverFont, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, bestScore.ToString(), "Sorry Tim.");
                            gameState = GameState.Over;

                            break;
                        }
                        if (guardList.ElementAt(i).ISeeYou)
                        {
                            //Console.WriteLine(guard.objectID);
                            gameState = GameState.Alert;

                        }


                        if (guardList.ElementAt(i).isDead())
                        {
                            currentLevel.guards.ElementAt(currentLevel.currentMapIndex).RemoveAt(i);

                            sound.fallSound.Play();
                        }
                        else
                            currentLevel.guards.ElementAt(currentLevel.currentMapIndex).ElementAt(i).Update(player.whereOnTile, gameTime);
                            

                    }

                    // If all guards can't see me change gamestate from alert to game
                    int count = 0;
                    foreach (Npc guard in currentLevel.guards.ElementAt(currentLevel.currentMapIndex))
                    {

                        if (!guard.ISeeYou)
                        {
                            count++;
                        }
                        if (count >= currentLevel.guards.ElementAt(currentLevel.currentMapIndex).Count)
                        {
                            gameState = GameState.Game;

                        }
                    }

                    if (player.isDead())
                    {
                        gameOver = new GameOverScreen(this);
                        gameOver.initialize(blackTexture, gameOverFont, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, bestScore.ToString(), "Sorry Tim.");
                        gameState = GameState.Over;
                    }

                    if (gameState == GameState.Game)
                    {
                        if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong == sound.Alert)
                            MediaPlayer.Play(sound.normalMusic);
                    }
                    if (gameState == GameState.Alert)
                    {
                        if (MediaPlayer.Queue.ActiveSong != sound.Alert)
                            MediaPlayer.Play(sound.Alert);
                    }

                    if (player.tileX == currentLevel.currentGoal.X && player.tileY == currentLevel.currentGoal.Y && currentLevel.currentMapIndex + 1 < currentLevel.winGoal)
                    {
                        currentLevel.currentMapIndex++;
                        currentLevel.currentMap = currentLevel.maps[currentLevel.currentMapIndex];
                        currentLevel.currentGoal = currentLevel.goal[currentLevel.currentMapIndex];

                        int currNumGold = player.numGold;
                        player.initialize(start, 0, timTexture, currentLevel.start[currentLevel.currentMapIndex].X, currentLevel.start[currentLevel.currentMapIndex].Y, "Player", currentLevel.currentMap);
                        player.numGold = currNumGold;
                      //  player.Location.X = currentLevel.start[currentLevel.currentMapIndex].X * tileSize - (6 * tileSize);
                        //player.Location.Y = currentLevel.start[currentLevel.currentMapIndex].Y * tileSize - (4 * tileSize);

                        loadGuards("lvl1map" + (currentLevel.currentMapIndex + 1) + "guards.csv");
 
                    }
                    else if (player.tileX == currentLevel.currentGoal.X && player.tileY == currentLevel.currentGoal.Y && currentLevel.currentMapIndex + 1 == currentLevel.winGoal)
                    {
                        gameOver = new GameOverScreen(this);
                        int score = ((int)hud.timer / 1000);
                        if (score < bestScore)
                            bestScore = score;
                        gameOver.initialize(blackTexture, gameOverFont, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, bestScore.ToString(), "Congratulations");
                        gameState = GameState.Over;
                    }
                    hud.Update(gameTime);
                    
                }
            }
         

            if (gameState == GameState.Over)
            {
                MediaPlayer.Stop();
                if (ks.IsKeyDown(Keys.Enter) || gs.Buttons.Start == ButtonState.Pressed)
                {
                    gameState = GameState.Game;
                    Pause.pauseKeyDown = true;
      
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

            Vector2 firstSquare = new Vector2(player.Location.X / tileSize, player.Location.Y / tileSize);
			int firstX = (int)firstSquare.X ;
			int firstY = (int)firstSquare.Y ;

            Vector2 squareOffset = new Vector2(player.Location.X % tileSize, player.Location.Y % tileSize);
			int offsetX = (int)squareOffset.X;
			int offsetY = (int)squareOffset.Y;
	
			//draw tile map
			for (int y = 0; y < squaresDown; y++) {
           
				for (int x = 0; x < squaresAcross; x++) {
					foreach (int tileID in currentLevel.currentMap.mapCell[x + firstX,y + firstY].BaseTiles) {
                        drawnRectangles[x, y] = new Rectangle((x * tileSize) - offsetX, (y * tileSize) - offsetY, tileSize, tileSize);
						spriteBatch.Draw (
        				Tile.TileSetTexture,
        				drawnRectangles[x,y],
        					Tile.GetSourceRectangle (tileID, tileSize),
        					Color.White);

					}
				}
			}

			//draw objects on top of tile map
			for (int y = 0; y < squaresDown; y++) {
           
				for (int x = 0; x < squaresAcross; x++) {

                    foreach (Npc guard in currentLevel.guards[currentLevel.currentMapIndex])
                    {
                        guard.draw(spriteBatch, x, y, firstX, firstY, offsetX, offsetY);
                    }
				}
			}


          
			//draw player
			player.draw (this.spriteBatch);
          


            //draw hud
            if (gameState == GameState.Game || gameState == GameState.Alert)
            {
                hud.Draw(this.spriteBatch);
            }

            if (gameState == GameState.Menu)
            {
                menu.Draw(spriteBatch);
            }


            //MINI MAP
            spriteBatch.Draw(minimap, new Rectangle(GraphicsDevice.Viewport.Width - 128, -15, 128, 128), Color.White);
            Vector2 origin = new Vector2(4, 4);
            spriteBatch.Draw(arrow, new Rectangle(GraphicsDevice.Viewport.Width - 64 -4, 64-15-4, 8, 8), null, Color.Blue, player.rotation, origin, SpriteEffects.None, 0);
          //  spriteBatch.Draw(dot, new Rectangle(GraphicsDevice.Viewport.Width - 64 -4, 64-15-4, 8, 8), Color.Blue);

                    foreach (Npc guard in currentLevel.guards[currentLevel.currentMapIndex])
                    {
                        if (player.tileY - guard.currentTile.Y < 10 && guard.currentTile.Y - player.tileY < 10 && player.tileX - guard.currentTile.X < 12 && guard.currentTile.X - player.tileX < 12)
                            spriteBatch.Draw(dot, new Rectangle((GraphicsDevice.Viewport.Width - 128 + guard.currentTile.X * 4 ) - ((int)player.Location.X/16 -32), (guard.currentTile.Y * 4) - ((int)player.Location.Y/16 - 32), 8, 8), Color.Red);
                    }
            if(Math.Abs(player.nextTileX -currentLevel.currentGoal.X) < 10 && Math.Abs(player.nextTileY -currentLevel.currentGoal.Y) <10)
                spriteBatch.Draw(dot, new Rectangle((GraphicsDevice.Viewport.Width - 128 + currentLevel.currentGoal.X * 4) - ((int)player.Location.X / 16 - 32), (currentLevel.currentGoal.Y * 4) - ((int)player.Location.Y / 16 - 32), 8, 8), Color.Yellow);
             
            if (gameState == GameState.Over)
            {
                gameOver.Draw(spriteBatch);
            }

            if (gameState == GameState.Menu)
            {
                menu.Draw(spriteBatch);
            }

            //Console.WriteLine(player.tileX);
			spriteBatch.End ();
			base.Draw (gameTime);
		}

        /* Read in guards from text file. Each line contains the patrol 
         * points for a guard and a single integer at the end of the line for speed.
         * */
        public void loadGuards(string csv)
        {
            currentLevel.initiailizeGuards(new List<Npc>());
            Texture2D guardTexture = Content.Load<Texture2D>("guard");
            TextReader gr = new StreamReader(csv);
            TextReader nr = new StreamReader(csv);
            String line;
            int numGuards = nr.ReadToEnd().Split('\n').Length - 1;
            for (int i = 0; i < numGuards; i++)
            {
                line = gr.ReadLine();
                int speed = Convert.ToInt32(line.Split('>')[line.Split('>').Length - 1]);
                int numPoints = Convert.ToInt32(line.Split(';').Length - 1);
                String[] points = line.Split(';'); //last part will be speed, not used
                Point[] patrolPoints = new Point[numPoints];
                int[] directions = new int[numPoints];

                for (int j = 0; j < numPoints; j++)
                {
                    String[] point = points[j].Split(',');
                    int posX = Convert.ToInt32(point[0]);
                    int posy = Convert.ToInt32(point[1]);
                    int direction = Convert.ToInt32(point[2]);
                    patrolPoints[j] = new Point(posX, posy);
                    directions[j] = direction;

                }

                Npc guard = new Npc(this);
                guard.initialize(currentLevel.currentMap, patrolPoints[0].X, patrolPoints[0].Y, 0, 0, guardTexture, "guard" + i, patrolPoints, directions, speed);
                currentLevel.guards[currentLevel.currentMapIndex].Add(guard);

            }
            gr.Close();
            nr.Close();
        }

        public void loadGold(string csv)
        {



            TextReader gr = new StreamReader(csv);
            TextReader nr = new StreamReader(csv);
            String line;
            int numGold = nr.ReadToEnd().Split('\n').Length - 1;
            this.currentLevel.itemLocation = new Point[numGold];
            for (int i = 0; i < numGold; i++)
            {
                line = gr.ReadLine();
                Boolean hidden = false;
                if (line.Contains("!"))
                {
                    line = line.Replace("!", "");
                    hidden = true;
                }

                String[] points = line.Split(',');
                int posX = Convert.ToInt32(points[0]);
                int posY = Convert.ToInt32(points[1]);
                currentLevel.itemLocation[i] = new Point(posX, posY);

                if (!hidden)
                {
                    currentLevel.currentMap.mapCell[posX, posY].AddBaseTile(229);
                    currentLevel.currentMap.mapCell[posX, posY].AddObject("Gold2");
                }
            }
            gr.Close();
            nr.Close();
        }
	}
}

