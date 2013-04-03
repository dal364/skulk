using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Skulk
{
    public class Menu : GameComponent
    {
        SpriteFont font;
        Rectangle destination;
        Vector2 positionTitle;
        Vector2 positionButtons;
        Texture2D texture;
        
        public Menu(Game game)
			:base(game)
		{
		}

        public void initialize(Texture2D texture, SpriteFont font, int width, int height)
        {
            this.font = font;
            this.destination = new Rectangle(0, 0, width, height);
            this.positionTitle.X = width / 2;
            this.positionTitle.Y = height / 2;
            this.positionButtons.X = width / 2;
            this.positionButtons.Y = height / 2 + 100;
            this.texture = texture;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
           
                spriteBatch.Draw(texture,destination, Color.Black);
                Vector2 origin = new Vector2(font.MeasureString("Skulk").X / 2, font.MeasureString("Skulk").Y / 2);
                spriteBatch.DrawString(font, "SKULK",positionTitle,Color.White, 0, origin,1,0,0 );
                origin = new Vector2(font.MeasureString("Start Game").X / 2, font.MeasureString("Start Game").Y / 2);
                spriteBatch.DrawString(font, "Start Game", positionButtons, Color.White, 0, origin, 0.5f, 0, 0);
        }
    }
}