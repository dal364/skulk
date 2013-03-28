using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Skulk
{
    public class GameOverScreen : GameComponent
    {
        SpriteFont font;
        Rectangle destination;
        Vector2 position;
        Vector2 positionScore;
        Texture2D texture;
        string bestScore;
        
        public GameOverScreen(Game game)
			:base(game)
		{
		}

        public void initialize(Texture2D texture, SpriteFont font, int width, int height, string bestScore)
        {
            this.font = font;
            this.destination = new Rectangle(0, 0, width, height);
            this.position.X = width / 2;
            this.position.Y = height / 2;
            this.texture = texture;
            this.bestScore = bestScore;
            this.positionScore.X = this.position.X;
            this.positionScore.Y = this.position.Y + 50;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
           
                spriteBatch.Draw(texture,destination, Color.Black);
                Vector2 origin = new Vector2(font.MeasureString("Sorry Tim.").X / 2, font.MeasureString("Sorry Tim.").Y / 2);
                spriteBatch.DrawString(font, "Sorry Tim.",position,Color.White, 0, origin,1,0,0 );

                origin = new Vector2(font.MeasureString("Best Score: " + bestScore).X / 2, font.MeasureString("Best Score: " + bestScore).Y / 2);
                spriteBatch.DrawString(font, "Best Score: "+bestScore, positionScore, Color.White, 0, origin, 0.5f, 0, 0);
        }
    }
}