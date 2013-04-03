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

        private Texture2D[] textures;
        Rectangle timerDestination;
        Rectangle viewDestination;
        public double timer;
        private Vector2 vec;
        SpriteFont font;

        public hud(Game game)
            : base(game)
        {
            textures = new Texture2D[2];
        }

        public void initializeTimer(Texture2D texture, int width, int height, int tPosX, int tPosY, SpriteFont font)
        {
            this.textures[0] = texture;
            this.timerDestination = new Rectangle(0, 0, width, height);
            timer = 0.0f;
            vec = new Vector2(tPosX, tPosY );
            this.font = font;

        }
        public void initializeView(Texture2D texture, int width, int height)
        {
            this.textures[1] = texture;
            this.viewDestination = new Rectangle(0, 0, width, height);

        }


        public void Draw(SpriteBatch spriteBatch)
        {
          
            

            if (textures[1] != null)
                     spriteBatch.Draw(textures[1], viewDestination, Color.White);
            if (textures[0] != null)
                spriteBatch.Draw(textures[0], timerDestination, Color.White);
            

 
         
            spriteBatch.DrawString(font, ((int)timer / 1000).ToString(), vec, Color.Gold);
           
            
        }

        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            base.Update(gameTime);
        }
    }
}
