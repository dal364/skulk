using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skulk
{
	public class Object : GameComponent
	{
		public int originalOffsetX;
		public int originalOffsetY;
		public Texture2D texture;
		protected Rectangle source;
		protected string objectID;

        protected int curTileX;
        protected int curTileY;

		protected TileMap map;

		protected int frameCount = 0; // Which frame we are.  Values = {0, 1, 2}
        protected int frameSkipY = 32; // How much to move the frame in X when we increment a frame--X distance between top left corners.
        protected int frameStartX = 0; // X of top left corner of frame 0. 
        protected int frameStartY = 0; // Y of top left corner of frame 0.
        protected int frameWidth = 32; // X of right minus X of left. 
        protected int frameHeight = 32; // Y of bottom minus Y of top


		public Object (Game game)
			:base(game)
		{
		}

		public void initialize (TileMap map,int x, int y, int offsetX, int offsetY, Texture2D texture, string objectID)
		{

			this.originalOffsetX = offsetX;
			this.originalOffsetY = offsetY;
			this.texture = texture;
			this.map = map;
			this.objectID = objectID;
            this.curTileX = x;
            this.curTileY = y;


			map.mapCell[x,y].AddObject(objectID);

			source = new Rectangle(
				frameStartX,
				frameStartY + frameSkipY * frameCount,
				frameWidth,
				frameHeight
				);
		}

		public void draw (SpriteBatch spriteBatch, int x, int y, int firstX, int firstY, int offsetX, int offsetY)
		{
			this.source.Y = this.frameStartY + this.frameSkipY * this.frameCount;

            //go through each cell containing objects
			foreach (string objectID in map.mapCell[x + firstX,y + firstY].Objects) {
                // draw the specific object
				if(objectID.Equals(this.objectID)){
				spriteBatch.Draw(
					texture,
					new Rectangle(
					(x * 64) + this.originalOffsetX - offsetX,
					(y * 64) + this.originalOffsetY - offsetY, this.frameWidth, this.frameHeight),
        			source,
					Color.White);
				}
			}
		}

		public override void Update (GameTime gameTime)
		{

			base.Update(gameTime);
		}

	}
}

