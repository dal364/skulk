using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Skulk
{
	public class Npc: Object
	{

		int animationCount; // How many ticks since the last frame change.
        //int animationMax = 10; // How many ticks to change frame after.

        Point[] patrolTiles;
        int itr; //iterator for patrolTiles
        int moveBy;
        Boolean movingVertical;
        Boolean movingHorizontal;
		public Npc(Game game)
			:base(game)
		{
			base.frameHeight = 128 - 33;
			base.frameWidth = 128;
			base.frameStartY = 33;
		}

        public void initialize(TileMap map, int x, int y, int offsetX, int offsetY, Texture2D texture, string objectID, Point[] patrolTiles)
        {
            this.patrolTiles = patrolTiles;
            this.originalOffsetX = offsetX;
            this.originalOffsetY = offsetY;
            this.texture = texture;
            this.map = map;
            this.objectID = objectID;
            this.curTileX = x;
            this.curTileY = y;
            this.moveBy = 2;
            this.itr = 1;

            movingVertical = false;
            movingHorizontal = false;

            map.mapCell[x, y].AddObject(objectID);

            source = new Rectangle(
                frameStartX,
                frameStartY + frameSkipY * frameCount,
                frameWidth,
                frameHeight
                );
        }
		public override void Update (GameTime gameTime)
		{
          
           //Debug//Console.WriteLine("offsetx: " +originalOffsetX + "       " + "offsety: "+originalOffsetY +"              "+ "tilex: " + curTileX + "tiley: " + curTileY) +"                "+ "destx: " + patrolTiles[itr].X + "   " + "desty: "+ patrolTiles[itr].Y);
            if (patrolTiles.Length > 1)
            {
                if (patrolTiles[itr].Y - curTileY > 0) //still moving in terms of Y
                {
                    movingVertical = true;

                    movingHorizontal = false;
                    if (moveBy < 0)
                        moveBy = moveBy * -1;
                }
                else if (patrolTiles[itr].Y - curTileY < 0)
                {
                    movingVertical = true;
                    movingHorizontal = false;
                    if (moveBy > 0)
                        moveBy = moveBy * -1;
                }
                else if (patrolTiles[itr].X - curTileX > 0) //moving in terms of X
                {
                    movingVertical = false;
                    movingHorizontal = true;
                    if (moveBy < 0)
                        moveBy = moveBy * -1;
                }
                else if (patrolTiles[itr].X - curTileX < 0)
                {
                    movingVertical = false;
                    movingHorizontal = true;
                    if (moveBy > 0)
                        moveBy = moveBy * -1;
                }
                else
                {
                    //shouldn't happen
                }
                if (curTileX == patrolTiles[itr].X && movingVertical) //move in terms of y
                {
                    //Debug//Console.WriteLine("inY");    
                    this.originalOffsetY += moveBy;

                    if (curTileY == patrolTiles[itr].Y)
                    {
                        if ((moveBy > 0 && originalOffsetY >= 64) || (moveBy < 0 && originalOffsetY <= 0))
                        {
                            itr++;
                            if (itr >= patrolTiles.Length)
                                itr = 0;
                        }
                    }
                    else
                    {
                        if (this.originalOffsetY > 64)
                        {
                            this.map.mapCell[curTileX, curTileY].RemoveObject(objectID);
                            this.curTileY += 1;
                            this.map.mapCell[curTileX, curTileY].AddObject(objectID);
                            this.originalOffsetY = moveBy;
                        }
                        else if (this.originalOffsetY < 0)
                        {
                            this.map.mapCell[curTileX, curTileY].RemoveObject(objectID);
                            this.curTileY -= 1;
                            this.map.mapCell[curTileX, curTileY].AddObject(objectID);
                            this.originalOffsetY = 64 + moveBy;
                        }
                    }
                }
                else //move in terms of x
                {
                    //Debug//Console.WriteLine("inX");
                    this.originalOffsetX += moveBy;
                    if (curTileX == patrolTiles[itr].X)
                    {
                        if ((moveBy > 0 && originalOffsetX >= 64) || (moveBy < 0 && originalOffsetX <= 0))
                        {
                            itr++;
                            if (itr >= patrolTiles.Length)
                                itr = 0;
                        }

                    }
                    else
                    {
                        if (this.originalOffsetX > 64) //moving right and reach end of tile
                        {
                            this.map.mapCell[curTileX, curTileY].RemoveObject(objectID);
                            this.curTileX += 1;
                            this.map.mapCell[curTileX, curTileY].AddObject(objectID);
                            this.originalOffsetX = moveBy;
                        }
                        else if (this.originalOffsetX < 0)//moving left and reach end of tile
                        {
                            this.map.mapCell[curTileX, curTileY].RemoveObject(objectID);
                            this.curTileX -= 1;
                            this.map.mapCell[curTileX, curTileY].AddObject(objectID);
                            this.originalOffsetX = 64 + moveBy;
                        }
                    }
                }
            }
			this.animationCount += 1;
			this.UpdateAnimation();
			base.Update(gameTime);
		}

		public void UpdateAnimation ()
		{
	
		//To do

        }



	}
}
