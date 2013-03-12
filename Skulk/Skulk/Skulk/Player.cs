using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Skulk
{
	public class Player : GameComponent
	{
		Vector2 velocity;
		public Texture2D texture;

		Rectangle destination;
		Rectangle source;

		float rotation;

		int frameCount = 0; // Which frame we are.  Values = {0, 1, 2}
        int frameSkipY = 128; // How much to move the frame in X when we increment a frame--X distance between top left corners.
        int frameStartX = 0; // X of top left corner of frame 0. 
        int frameStartY = 0; // Y of top left corner of frame 0.
        int frameWidth = 128; // X of right minus X of left. 
        int frameHeight = 128; // Y of bottom minus Y of top.


		int animationCount; // How many ticks since the last frame change.
        int animationMax = 10; // How many ticks to change frame after. 

        // Camera Movement
        public Vector2 Location = Vector2.Zero;

        // Player Movement
        Boolean upMove;
        Boolean downMove;
        Boolean leftMove;
        Boolean rightMove;
        Vector2 position;

		public Player (Game game)
			:base(game)
		{
		}

		public void initialize (Vector2 position, float rotation, Texture2D texture)
		{
			this.texture = texture;
			this.animationCount = 0;

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
			KeyboardState ks = Keyboard.GetState ();
			this.velocity.X = 0;
			this.velocity.Y = 0;

            position.X = (400 + (int)this.Location.X) / 64;
            position.Y = (240 + (int)this.Location.Y) / 64;
            myMap.mapCell[(int)position.X, (int)position.Y].RemoveObject("Player");

			if (ks.IsKeyDown (Keys.Down)) {
                if (this.downMove)
                {
                    // Add code for player to move in camera
                    this.downMove = false;
                }
                else
                {
                    this.Location.Y = MathHelper.Clamp(this.Location.Y + 2, 0, (myMap.MapHeight - squaresDown) * 64);
                    if (this.Location.Y == (myMap.MapHeight - squaresDown) * 64)
                    {
                        this.downMove = true;
                    }
                }
				this.rotation = 0;
				this.animationCount += 1;
			}

			if (ks.IsKeyDown (Keys.Up)) {
                if (this.upMove)
                {
                    // Add code for player to move in camera
                    this.upMove = false;
                }
                else
                {
                    this.Location.Y = MathHelper.Clamp(this.Location.Y - 2, 0, (myMap.MapHeight - squaresDown) * 64);
                    if (this.Location.Y == 0)
                    {
                        this.upMove = true;
                    }
                }

				this.rotation = (float)Math.PI;
				this.animationCount += 1;
			}

			if(ks.IsKeyDown(Keys.Left)){
                if (this.leftMove)
                {
                    // Add code for player to move in camera
                    this.leftMove = false;
                }
                else
                {
                    this.Location.X = MathHelper.Clamp(this.Location.X - 2, 0, (myMap.MapWidth - squaresAcross) * 64);
                    if (this.Location.X == 0)
                    {
                        this.leftMove = true;
                    }
                }

				this.rotation = (float)Math.PI/2;
				this.animationCount += 1;
			}

			if(ks.IsKeyDown(Keys.Right)){
                if (this.rightMove)
                {
                    // Add code for player to move in camera
                    this.rightMove = false;
                }
                else
                {
                    this.Location.X = MathHelper.Clamp(this.Location.X + 2, 0, (myMap.MapHeight - squaresDown) * 64);
                    if (this.Location.X == (myMap.MapHeight - squaresDown) * 64)
                    {
                        this.rightMove = true;
                    }
                }
				this.rotation = (float)(3*Math.PI/2) ;
				this.animationCount += 1;
			}

            position.X = (400 + (int)this.Location.X) / 64;
            position.Y = (240 + (int)this.Location.Y) / 64;
            myMap.mapCell[(int)position.X, (int)position.Y].AddObject("Player");

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
            Console.WriteLine(this.destination.X + ", " + this.destination.Y);
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



	}
}

