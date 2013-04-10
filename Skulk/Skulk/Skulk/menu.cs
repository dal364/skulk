using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace Skulk
{
    public class Menu : GameComponent
    {
        SpriteFont font;
        Rectangle destination;
        Vector2 positionTitle;
        Vector2 positionButtons;
        Vector2 positionCredits;

        Rectangle arrowDes;
        Texture2D texture;
        Texture2D arrow;


        public  enum arrowKey
        {
            start,
            credits
        };

        public arrowKey state;
        public Menu(Game game)
			:base(game)
		{
            state = arrowKey.start;
		}

        public void initialize(Texture2D texture, Texture2D arrow, SpriteFont font, int width, int height)
        {
            this.font = font;
            this.destination = new Rectangle(0, 0, width, height);
            this.positionTitle.X = width / 2;
            this.positionTitle.Y = height / 2;
            this.positionButtons.X = width / 2;
            this.positionButtons.Y = height / 2 + 100;
            this.positionCredits.X = width / 2;
            this.positionCredits.Y = height / 2 + 110;
            this.texture = texture;
            this.arrow = arrow;
            


            this.arrowDes = new Rectangle(width / 2 - 35, height / 2 + 100, 8, 8);
        }

        public override void Update(GameTime gameTime)
        {
            Console.WriteLine(state);
            KeyboardState ks = Keyboard.GetState();
            GamePadState gs = GamePad.GetState(PlayerIndex.One);
            if((ks.IsKeyDown(Keys.Down) || gs.DPad.Down == ButtonState.Pressed) && state != arrowKey.credits )
            {
                arrowDes.Y += 10;
                state = arrowKey.credits;
                Console.WriteLine("down");
            }
            if ((ks.IsKeyDown(Keys.Up) || gs.DPad.Up == ButtonState.Pressed) && state != arrowKey.start)
            {
                arrowDes.Y -= 10;
                state = arrowKey.start;
            }
            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
                Vector2 origin = new Vector2(arrowDes.Width / 2, arrowDes.Height / 2);
              
                spriteBatch.Draw(texture,destination, Color.Black);
                spriteBatch.Draw(arrow, arrowDes, null, Color.White, (float)(3*Math.PI)/2, origin, SpriteEffects.None, 0);
               origin = new Vector2(font.MeasureString("Skulk").X / 2, font.MeasureString("Skulk").Y / 2);
                spriteBatch.DrawString(font, "SKULK",positionTitle,Color.White, 0, origin,1,0,0 );
                origin = new Vector2(font.MeasureString("Start Game").X / 2, font.MeasureString("Start Game").Y / 2);
                spriteBatch.DrawString(font, "Start Game", positionButtons, Color.White, 0, origin, 0.5f, 0, 0);
                origin = new Vector2(font.MeasureString("Credits").X / 2, font.MeasureString("Credits").Y / 2);
                
                spriteBatch.DrawString(font, "Credits", positionCredits, Color.White, 0, origin, 0.5f, 0, 0);
        }
    }
}