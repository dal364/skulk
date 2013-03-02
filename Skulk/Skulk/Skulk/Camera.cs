using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Skulk
{
	public class Camera : GameComponent
	{
		public Vector2 Location = Vector2.Zero;
        int xCount = 0;
        int yCount = 0;

		public Camera (Game game)
			:base(game)
		{
		}

		public void Update(TileMap myMap, int squaresAcross, int squaresDown, GameTime gameTime, Player p)
		{
			KeyboardState ks = Keyboard.GetState ();
			if (ks.IsKeyDown (Keys.Left)) {
                
				this.Location.X = MathHelper.Clamp (this.Location.X - 2, 0, (myMap.MapWidth - squaresAcross) * 64);
                
			}

			if (ks.IsKeyDown (Keys.Right)) {
				this.Location.X = MathHelper.Clamp (this.Location.X + 2, 0, (myMap.MapWidth - squaresAcross) * 64);
                
			}

			if (ks.IsKeyDown (Keys.Up)) {
				this.Location.Y = MathHelper.Clamp (this.Location.Y - 2, 0, (myMap.MapHeight - squaresDown) * 64);
			}

			if (ks.IsKeyDown (Keys.Down)) {
				this.Location.Y = MathHelper.Clamp (this.Location.Y + 2, 0, (myMap.MapHeight - squaresDown) * 64);
			}
            xCount = (int)this.Location.X / 64 + squaresAcross/2;
            yCount = (int)this.Location.Y / 64 + squaresDown/2;
            p.updateTilePosition(xCount, yCount);
          

			base.Update(gameTime);
				
		}

       
        
	}

}

