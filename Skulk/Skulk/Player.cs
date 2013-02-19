using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Skulk
{
    public class Player : GameComponent
    {
        Vector2 velocity;
        Texture2D texture;

        public Vector2 position;

        Rectangle destination;
        Rectangle source;

        float rotation;

        int frameCount = 0; // Which frame we are. Values = {0, 1, 2}
        int frameSkipY = 128; // How much to move the frame in X when we increment a frame--X distance between top left corners.
        int frameStartX = 0; // X of top left corner of frame 0.
        int frameStartY = 0; // Y of top left corner of frame 0.
        int frameWidth = 128; // X of right minus X of left.
        int frameHeight = 128; // Y of bottom minus Y of top.


        int animationCount; // How many ticks since the last frame change.
        int animationMax = 10; // How many ticks to change frame after.

        public Player(Game game)
            : base(game)
        {
        }

        public void initialize(Vector2 position, float rotation, Texture2D texture)
        {
            this.position = position;

            this.texture = texture;
            this.animationCount = 0;

            destination = new Rectangle(
            (int)position.X - frameStartX / 2,
            (int)position.Y - frameStartY / 2,
            frameWidth,
            frameHeight
            );

            source = new Rectangle(
            frameStartX,
            frameStartY + frameSkipY * frameCount,
            frameWidth,
            frameHeight
            );
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            this.velocity.X = 0;
            this.velocity.Y = 0;

            if (ks.IsKeyDown(Keys.Down))
            {
                this.velocity.Y += 3;
                this.rotation = 0;
                this.animationCount += 1;
            }

            if (ks.IsKeyDown(Keys.Up))
            {
                this.velocity.Y -= 3;
                this.rotation = (float)Math.PI;
                this.animationCount += 1;
            }

            if (ks.IsKeyDown(Keys.Left))
            {
                this.velocity.X -= 3;
                this.rotation = (float)Math.PI / 2;
                this.animationCount += 1;
            }

            if (ks.IsKeyDown(Keys.Right))
            {
                this.velocity.X += 3;
                this.rotation = (float)(-Math.PI / 2);
                this.animationCount += 1;
            }

            if (ks.IsKeyDown(Keys.Right) && ks.IsKeyDown(Keys.Up))
            {
                this.velocity.X = 3;
                this.rotation = (float)(-3*Math.PI / 4);
                this.animationCount += 1;
            }

            if (ks.IsKeyDown(Keys.Right) && ks.IsKeyDown(Keys.Down))
            {
                this.velocity.X = 3;
                this.rotation = (float)(- Math.PI / 4);
                this.animationCount += 1;
            }

            if (ks.IsKeyDown(Keys.Left) && ks.IsKeyDown(Keys.Down))
            {
                this.velocity.X = -3;
                this.velocity.Y = 3;
                this.rotation = (float)(Math.PI / 4);
                this.animationCount += 1;
            }

            if (ks.IsKeyDown(Keys.Left) && ks.IsKeyDown(Keys.Up))
            {
                this.velocity.X = -3;
                this.velocity.Y = -3;
                this.rotation = (float)(3 * Math.PI / 4);
                this.animationCount += 1;
            }

            this.position.Y += this.velocity.Y;
            this.position.X += this.velocity.X;

            this.UpdateAnimation();
            base.Update(gameTime);
        }

        public void draw(SpriteBatch spritebatch)
        {
            // Basic destination rectangle updating from last time.
            this.destination.X = (int)Math.Round(this.position.X - this.destination.Width);
            this.destination.Y = (int)Math.Round(this.position.Y - this.destination.Height);

            // Update the source rectangle, based on where in the animation we are.
            this.source.Y = this.frameStartY + this.frameSkipY * this.frameCount;
            Vector2 origin = new Vector2(this.source.Width / 2, this.source.Height / 2);
            //spritebatch.Draw(texture,this.destination,Color.AliceBlue);
            spritebatch.Draw(texture, this.destination, this.source, Color.White, this.rotation, origin, SpriteEffects.None, 9/10);
        }

        public void UpdateAnimation()
        {
            if (this.animationCount > this.animationMax)
            {
                this.animationCount = 0;
                this.frameCount += 1;
            }

            if (this.frameCount == 5)
            {
                this.frameCount = 0;
            }

        }





    }
}