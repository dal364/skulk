#region Using Statements
using System;
using System.IO;
using System.Collections;
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
		torch testObject;
		ArrayList guards;

		//Tile Map stuff
		TileMap myMap = new TileMap();
		int squaresAcross;
		int squaresDown;

		//Camera
		Camera camera;


		public Game1 ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;

		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{                                                   // +2 to compensate for tiles off screen
			squaresAcross = GraphicsDevice.Viewport.Width / 64 + 2;
		    squaresDown = GraphicsDevice.Viewport.Height / 64 + 2;

			camera = new Camera(this);
			Texture2D torchTexture = Content.Load<Texture2D> ("torch");
    

            //Load guards
            loadGuards();


			Vector2 start = new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2);
			Texture2D texture = Content.Load<Texture2D>("sprite");
			player = new Player(this);
			player.initialize(start, 0, texture);
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

			camera.Update(myMap,squaresAcross,squaresDown,gameTime);
			player.Update(gameTime);
			testObject.Update(gameTime);
            foreach (Npc guard in guards)
            {
                guard.Update(gameTime);
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

			Vector2 firstSquare = new Vector2 (camera.Location.X / 64, camera.Location.Y / 64);
			int firstX = (int)firstSquare.X;
			int firstY = (int)firstSquare.Y;

			Vector2 squareOffset = new Vector2 (camera.Location.X % 64, camera.Location.Y % 64);
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
                int speed = Convert.ToInt32(line.Split('>')[line.Split('>').Length-1]);
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
                guard.initialize(myMap, patrolPoints[0].X, patrolPoints[0].Y, 0, 0, guardTexture, "guard"+i, patrolPoints, speed);
                guards.Add(guard);

            }
        }
	}
}

