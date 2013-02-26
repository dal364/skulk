using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Skulk
{
	public class Player : GameComponent
	{
        public Vector2 Location = Vector2.Zero;
        float acceleration = 2;
	
		public Texture2D texture;
        int tileX;
        int tileY;
        protected string objectID;

        protected TileMap map;
      
		Rectangle destination;
		Rectangle source;

		float rotation;

		int frameCount = 0; // Which frame we are.  Values = {0, 1, 2}
        int frameSkipY = 128; // How much to move the frame in X when we increment a frame--X distance between top left corners.
        int frameStartX = 0; // X of top left corner of frame 0. 
        int frameStartY = 21; // Y of top left corner of frame 0.
        int frameWidth = 128; // X of right minus X of left. 
        int frameHeight = 128 - 21; // Y of bottom minus Y of top.

        
		int animationCount; // How many ticks since the last frame change.
        int animationMax = 10; // How many ticks to change frame after. 

        bool isInCollision; 

		public Player (Game game)
			:base(game)
		{
		}

		public void initialize (Vector2 position, float rotation, Texture2D texture, int tileX, int tileY, string objectID, TileMap map)
		{
			this.texture = texture;
			this.animationCount = 0;
            this.objectID = objectID;
            this.map = map;
            this.tileX = tileX;
            this.tileY = tileY;

            this.map.mapCell[tileX, tileY].AddObject(objectID);
        
			destination = new Rectangle(
				(int)position.X - frameStartX/2,
				(int)position.Y - frameStartY/2,
				frameWidth,
				frameHeight
				);


			source = new Rectangle(
				frameStartX,
				frameStartY + frameSkipY * frameCount,
				frameWidth,
				frameHeight
				);
		}

        public void Update(TileMap myMap, int squaresAcross, int squaresDown, GameTime gameTime)
        {
           
            int xCount;
            int yCount;
            KeyboardState ks = Keyboard.GetState();
            if (isInCollision)
            {
                this.collide(ks);
            }
            if (ks.IsKeyDown(Keys.Left))
            {
                this.rotation = (float)Math.PI / 2;
                this.animationCount += 1;
                this.Location.X = MathHelper.Clamp(this.Location.X - acceleration, 0, (myMap.MapWidth - squaresAcross) * 64);

            }

            if (ks.IsKeyDown(Keys.Right))
            {
                this.rotation = (float)(3 * Math.PI / 2); ;
                this.animationCount += 1;
                this.Location.X = MathHelper.Clamp(this.Location.X + acceleration, 0, (myMap.MapWidth - squaresAcross) * 64);

            }

            if (ks.IsKeyDown(Keys.Up))
            {
                this.rotation = (float)Math.PI;
                this.animationCount += 1;
                this.Location.Y = MathHelper.Clamp(this.Location.Y - acceleration, 0, (myMap.MapHeight - squaresDown) * 64);
            }

            if (ks.IsKeyDown(Keys.Down))
            {
                this.rotation = 0;
                this.animationCount += 1;
                this.Location.Y = MathHelper.Clamp(this.Location.Y + acceleration, 0, (myMap.MapHeight - squaresDown) * 64);
            }
            xCount = (int)this.Location.X / 64 + squaresAcross / 2;
            yCount = (int)this.Location.Y / 64 + squaresDown / 2;
            this.updateTilePosition(xCount, yCount);

            
           
            this.UpdateAnimation();
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
			//spritebatch.Draw(texture,this.destination,Color.AliceBlue);
			spritebatch.Draw(texture, this.destination, this.source, Color.White, this.rotation, origin, SpriteEffects.None, 0);

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
            Console.WriteLine(tileX+" "+ tileY);
            
         }

        public void isColliding(Object Object)
        {
            if ((Object.tileX >= this.tileX - 1 && Object.tileX <= this.tileX + 1) &&
                (Object.tileY >= this.tileY - 1 && Object.tileY <= this.tileY + 1))
            {
                Console.WriteLine(this.destination.Intersects(Object.destination));
               
                isInCollision = this.destination.Intersects(Object.destination );
            }
           
        }

        public void collide(KeyboardState ks)
        {
            acceleration = 0;
            
            isInCollision = false;
            
        }


	}
}

