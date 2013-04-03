using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Skulk
{
    public class GameOverScreen : GameComponent
    {
        private Texture2D texture;
        Rectangle destination;

        public GameOverScreen(Game game)
			:base(game)
		{
		}

        public void initialize(Texture2D texture, int width, int height)
        {
            this.texture = texture;
            this.destination = new Rectangle(0, 0, width, height);

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, destination, Color.White);
        }
    }
}