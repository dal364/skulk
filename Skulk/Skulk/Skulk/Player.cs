using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Skulk
{
	public class Player : GameComponent
	{
       
        public Vector2 Location = Vector2.Zero;
        public Rectangle boundingBox;
		public Texture2D texture;
        SoundEffect coinSound;

        public int numGold;
        bool goldKeyPressed;

        public int numGold;
        bool goldKeyPressed;

        float acceleration = 2;

        int tileSize = 64;
		public Rectangle destination;
		Rectangle source;

        Vector2 position;
		float rotation;

		int frameCount = 0; // Which frame we are.  Values = {0, 1, 2}
        int frameSkipY = 64; // How much to move the frame in X when we increment a frame--X distance between top left corners.
        int frameStartX = 0; // X of top left corner of frame 0. 
        int frameStartY = 0; // Y of top left corner of frame 0.
        int frameWidth = 64 ; // X of right minus X of left. 
        int frameHeight = 64  ; // Y of bottom minus Y of top.


		int animationCount; // How many ticks since the last frame change.
        int animationMax = 10; // How many ticks to change frame after. 

        
      
        public int tileX;
        public int tileY;
        public int nextTileX;
        public int nextTileY;
        protected string objectID;

        protected TileMap map;
     

		public Player (Game game)
			:base(game)
		{
		}

        public void initialize(Vector2 position, float rotation, Texture2D texture,SoundEffect coinSound, int tileX, int tileY, string objectID, TileMap map)
        {
            this.texture = texture;
            this.coinSound = coinSound;
            this.animationCount = 0;
            this.objectID = objectID;
            this.map = map;
            this.tileX = tileX;
            this.tileY = tileY;
            this.position = position;

            this.Location.X = tileSize * tileX - (6 * tileSize);
            this.Location.Y = tileSize * tileY - (4 * tileSize);
            this.numGold = 3;
            this.goldKeyPressed = false;
            this.rotation = rotation;

            this.Location.X = tileSize * tileX;
            this.Location.Y = tileSize * tileY;
            this.numGold = 3;
            this.goldKeyPressed = false;
            
            
            this.map.mapCell[tileX, tileY].AddObject(objectID);

            destination = new Rectangle(
            (int)position.X - frameStartX / 2,
            (int)position.Y - frameStartY / 2,
            frameWidth,
            frameHeight
            );



            source = new Rectangle(
            frameStartX,
            frameStartY + frameSkipY * frameCount,
            frameWidth,
            frameHeight
            );
            

            boundingBox = new Rectangle(
            (int)position.X - (source.Width/2),
            (int)position.Y - (source.Height/2),
            frameWidth,
            frameHeight
            );
        }

        public void Update(TileMap myMap, int squaresAcross, int squaresDown, GameTime gameTime, Rectangle[,] drawnRectangles)
		{
            GamePadState gps = GamePad.GetState(PlayerIndex.One);
			KeyboardState ks = Keyboard.GetState ();
            MouseState ms = Mouse.GetState();
            Vector2 mouseLoc = new Vector2(ms.X, ms.Y);
            Vector2 direction;
            direction.X = mouseLoc.X - position.X;
            direction.Y = mouseLoc.Y  - position.Y;

            if (gps.IsConnected)
            {
                direction.X = gps.ThumbSticks.Left.X;
                direction.Y = gps.ThumbSticks.Left.Y * -1;
            }

            if (direction.X + direction.Y != 0)
                this.rotation = (float)(Math.Atan2(direction.Y, direction.X)) - (float)Math.PI/2;
            //Console.WriteLine(nextTileX);
           
            //Console.WriteLine(myMap.obstacleTiles.Last.Value);

            if (direction.X < 0)
                nextTileX = tileX - 1;
            if (direction.X > 0)
                nextTileX = tileX ;
            if (direction.Y > 0)
                nextTileY = tileY + 1;
            if (direction.Y < 0)
                nextTileY = tileY - 1;

            int xCount;
            int yCount;
            if (ks.IsKeyDown(Keys.W) || (gps.IsConnected && (direction.X + direction.Y != 0)))
            {
                this.animationCount += 1;

             

               if (!myMap.obstacleTiles.Contains(new Point(nextTileX, tileY)))
                    this.Location.X = MathHelper.Clamp(this.Location.X - (float)Math.Sin(this.rotation) * acceleration, 0, (myMap.MapWidth - squaresAcross) * tileSize);
               if (!myMap.obstacleTiles.Contains(new Point(tileX, nextTileY)))
                   this.Location.Y = MathHelper.Clamp(this.Location.Y + (float)Math.Cos(this.rotation) * acceleration, 0, (myMap.MapHeight - squaresDown) * tileSize);
            }
            /*
            if (ks.IsKeyDown(Keys.Left))
            {
               // this.rotation = (float)Math.PI / 2;
                this.animationCount += 1;
                this.Location.X = MathHelper.Clamp(this.Location.X - acceleration, 0, (myMap.MapWidth - squaresAcross) * 64);

            }

            if (ks.IsKeyDown(Keys.Right))
            {
               // this.rotation = (float)(3 * Math.PI / 2); ;
                this.animationCount += 1;
                this.Location.X = MathHelper.Clamp(this.Location.X + acceleration, 0, (myMap.MapWidth - squaresAcross) * 64);

            }

            if (ks.IsKeyDown(Keys.Up))
            {
              //  this.rotation = (float)Math.PI;
                this.animationCount += 1;
                this.Location.Y = MathHelper.Clamp(this.Location.Y - acceleration, 0, (myMap.MapHeight - squaresDown) * 64);
            }

            if (ks.IsKeyDown(Keys.Down))
            {
               // this.rotation = 0;
                this.animationCount += 1;
                this.Location.Y = MathHelper.Clamp(this.Location.Y + acceleration, 0, (myMap.MapHeight - squaresDown) * 64);
            }*/
            if (ks.IsKeyDown(Keys.E) || gps.Buttons.A == ButtonState.Pressed)
            {
                if (numGold > 0 && !goldKeyPressed)
                {
                    myMap.mapCell[tileX, tileY].AddBaseTile(229);
                    myMap.mapCell[tileX, tileY].AddObject("Gold");
                    numGold--;

                    coinSound.Play();

                    goldKeyPressed = true;
                }
            }
            if (ks.IsKeyUp(Keys.E) && gps.Buttons.A == ButtonState.Released)
            {
                goldKeyPressed = false;
            }
            xCount = (int)(this.Location.X) / tileSize + (squaresAcross ) / 2 ;
            yCount = (int)(this.Location.Y) / tileSize + (squaresDown) / 2;
          //  Console.WriteLine(xCount + " " + yCount);
            this.updateTilePosition(xCount, yCount);
            UpdateAnimation();
            base.Update(gameTime);



		}

		public void draw (SpriteBatch spritebatch)
		{
			// Basic destination rectangle updating from last time. 
            //this.destination.X = (int)Math.Round(this.position.X - this.destination.Width );
            //this.destination.Y = (int)Math.Round(this.position.Y - this.destination.Height );

			// Update the source rectangle, based on where in the animation we are.  
            this.source.Y = this.frameStartY + this.frameSkipY * this.frameCount;
			Vector2 origin = new Vector2(this.source.Width /2 , this.source.Height/2 );
            //Console.WriteLine(this.destination.X + ", " + this.destination.Y);

            spritebatch.Draw(texture, this.destination, this.source, Color.White, this.rotation, origin, SpriteEffects.None, 0);
            //boundingBox Debug
            /*spritebatch.Draw(
                        texture,
                        boundingBox,
                        source,
                        Color.Black
                        );*/
            /*spritebatch.Draw(
                   texture,
                   destination,
                   source,
                   Color.Black
                   );*/
          
		}

		 public void UpdateAnimation()
        {
            if (this.animationCount > this.animationMax)
            {
                this.animationCount = 0;
                this.frameCount += 1;
            }

            if (this.frameCount == 5)
            {
                this.frameCount = 0;
            }

        }

         public void updateTilePosition(int newX, int newY)
         {
             this.map.mapCell[this.tileX, this.tileY].RemoveObject(this.objectID);
             this.tileX = newX;
             this.tileY = newY;
             this.map.mapCell[newX, newY].AddObject(this.objectID);
         }

        
         

	}
}

