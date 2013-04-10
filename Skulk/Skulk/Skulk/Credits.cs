using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skulk
{
    public class Credits : GameComponent
    {
        SpriteFont font;
        Vector2 position;
        Texture2D black;
        Rectangle background;
        public Credits(Game game)
			:base(game)
		{
            
		}

        public void initialize(SpriteFont font, Texture2D black)
        {
            this.font = font;
            this.position.X = 360;
            this.position.Y = -300;
            this.black = black;
            background = new Rectangle(0, 0, 720, 480);

        }

        public override void Update(GameTime gameTime)
        {
            this.position.Y += 0.5f;
            base.Update(gameTime);
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(black, background, Color.Black);
            Vector2 origin = new Vector2(font.MeasureString("http://opengameart.org/content/117-stone-wall-tilable-textures-in-8-themes").X / 2, font.MeasureString("Skulk").Y / 2);
            spriteBatch.DrawString(font, "Game Art & Sound:\nemergence & hero\nby\nMatt McFarland - http://www.mattmcfarland.com/\n\n"+
                "Textures:\nhttp://opengameart.org/content/117-stone-wall-tilable-textures-in-8-themes\nGPLv2\n\nhttp://opengameart.org/content/castle-tiles-for-rpgs\n"+
                "by\nHyptosis, Zabin, Daniel Cook\nCC-BY 3.0", position, Color.White, 0, origin, 0.7f, 0, 0);
            
        }
    }
}
