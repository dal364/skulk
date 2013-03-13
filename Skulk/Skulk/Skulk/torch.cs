using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skulk
{
	public class torch : Object
	{

		int animationCount; // How many ticks since the last frame change.
        int animationMax = 10; // How many ticks to change frame after.

		public torch (Game game)
			:base(game)
		{
		}

		public override void Update (GameTime gameTime)
		{
       
			this.animationCount += 1;
			this.UpdateAnimation();
			base.Update(gameTime);
		}

		public void UpdateAnimation ()
		{
	
			if (this.animationCount > this.animationMax) {
				this.animationCount = 0;
				this.frameCount += 1;
			}

			if (this.frameCount > 2) {
				this.frameCount = 0;
			} 

        }



	}
}

	


