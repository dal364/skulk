using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skulk
{
	public class torch : GameComponent
	{
		Vector2 position;
		Texture2D texture;
		Rectangle destination;
		Rectangle source;

		int frameCount = 0; // Which frame we are.  Values = {0, 1, 2}
        int frameSkipY = 32; // How much to move the frame in X when we increment a frame--X distance between top left corners.
        int frameStartX = 0; // X of top left corner of frame 0. 
        int frameStartY = 0; // Y of top left corner of frame 0.
        int frameWidth = 32; // X of right minus X of left. 
        int frameHeight = 32; // Y of bottom minus Y of top.


		int animationCount; // How many ticks since the last frame change.
        int animationMax = 10; // How many ticks to change frame after.

		public torch (Game game)
			:base(game)
		{
		}

		public void initialize (Vector2 position, Texture2D texture)
		{
			this.position = position;

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

		public void Update (GameTime gameTime)
		{
			this.animationCount += 1;
			this.UpdateAnimation();
			base.Update(gameTime);
		}

		public void draw (SpriteBatch spritebatch)
		{


			// Update the source rectangle, based on where in the animation we are.  
            this.source.Y = this.frameStartY + this.frameSkipY * this.frameCount;
			Vector2 origin = new Vector2(this.source.Width /2 , this.source.Height/2 );
			//spritebatch.Draw(texture,this.destination,Color.AliceBlue);
			spritebatch.Draw(texture, this.destination, this.source, Color.White, 0, origin, SpriteEffects.None, 0);
		}

		 public void UpdateAnimation()
        {
            if (this.animationCount > this.animationMax)
            {
                this.animationCount = 0;
                this.frameCount += 1;
            }

            if (this.frameCount == 2)
            {
                this.frameCount = 0;
            }

        }



	}
}

	


