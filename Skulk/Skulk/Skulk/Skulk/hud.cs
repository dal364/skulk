using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Skulk
{
    class hud : GameComponent
    {

        private Texture2D texture;
        Rectangle destination;
        private double timer;
        private Vector2 vec;
        SpriteFont font;

        public hud(Game game)
            : base(game)
        {
        }

        public void initialize(Texture2D texture, int width, int height, int tPosX, int tPosY, SpriteFont font)
        {
            this.texture = texture;
            this.destination = new Rectangle(0, 0, width, height);
            timer = 0.0f;
            vec = new Vector2(tPosX, tPosY );
            this.font = font;

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, destination, Color.White);

 
         
            spriteBatch.DrawString(font, ((int)timer / 1000).ToString(), vec, Color.White);
           
            
        }

        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            base.Update(gameTime);
        }
    }
}
