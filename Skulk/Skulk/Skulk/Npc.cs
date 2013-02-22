using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skulk
{
	public class Npc: Object
	{

		int animationCount; // How many ticks since the last frame change.
        //int animationMax = 10; // How many ticks to change frame after.

	

		public Npc(Game game)
			:base(game)
		{
			base.frameHeight = 128 - 33;
			base.frameWidth = 128;
			base.frameStartY = 33;
		}

		public override void Update (GameTime gameTime)
		{
			this.animationCount += 1;
			this.UpdateAnimation();
			base.Update(gameTime);
		}

		public void UpdateAnimation ()
		{
	
		//To do

        }



	}
}
