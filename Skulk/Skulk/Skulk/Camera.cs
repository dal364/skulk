using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Skulk
{
	public class Camera : GameComponent
	{
		public Vector2 Location = Vector2.Zero;

		public Camera (Game game)
			:base(game)
		{
		}

		public void Update(TileMap myMap, int squaresAcross, int squaresDown, GameTime gameTime)
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

			base.Update(gameTime);
				
		}
	}

}

