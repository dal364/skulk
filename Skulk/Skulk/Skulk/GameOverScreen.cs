using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Skulk
{
    public class GameOverScreen : GameComponent
    {
        private Texture2D texture;

        public GameOverScreen(Game game)
			:base(game)
		{
		}

        public void initialize(Texture2D texture)
        {
            this.texture = texture;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, new Vector2(0f, 0f), Color.White);
        }
    }
}