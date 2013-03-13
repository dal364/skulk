using System;
using System.Collections;
using System.Collections.Generic;
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
        int length; // Patrol list length or to let it know to use pathfinder length
        Boolean movingVertical;
        Boolean movingHorizontal;

        Point moveTo;

        Boolean init;
        Boolean ISeeYou;

		public Npc(Game game)
			:base(game)
		{
			base.frameHeight = 128 - 33;
			base.frameWidth = 128;
			base.frameStartY = 33;
		}

        public void initialize(TileMap map, int x, int y, int offsetX, int offsetY, Texture2D texture, string objectID, Point[] patrolTiles, int speed)
        {
            this.patrolTiles = patrolTiles;
            this.originalOffsetX = offsetX;
            this.originalOffsetY = offsetY;
            this.texture = texture;
            this.map = map;
            this.objectID = objectID;
            this.curTileX = x;
            this.curTileY = y;
            this.moveBy = speed;
            this.itr = 1; //index of next tile to move to, guard initially drawn at tile 0 it patrolTiles, so start itr at 1
            this.ISeeYou = false;
            this.moveTo = new Point(-1, -1);
            this.length = 0;
            this.init = true;

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

		

       

        /**
        * Finds the next closest point to the goal
        * start - is the starting position
        * goal - is the position you want to get to
        */

        public Point pathFinder(Point start, Point goal)
        {

            if ((start.X == goal.X) && (start.Y == goal.Y))
            {
                return goal;
            }

            LinkedList<Point> openset = new LinkedList<Point>();
            LinkedList<Point> closedset = new LinkedList<Point>();

            Point current;

            current.X = 0;
            current.Y = 0;

            Hashtable f_score = new Hashtable();

            // Calculation for the distance between guard and player
            f_score[start] = heuristicCostEstimate(start, goal);


            LinkedList<Point> neighbors = find_neighbors(start);
            // Adds the neighbors to the openset, if it's a possible place to move to and calculates there distance
            foreach (Point neighbor in neighbors)
            {
                // Checks if the neighbor is in the closedset (Walls, etc)
                if (closedset.Contains(neighbor))
                {
                    // Skip it
                }
                else if (!openset.Contains(neighbor))
                {
                    f_score[neighbor] = heuristicCostEstimate(neighbor, goal);

                    if (!openset.Contains(neighbor))
                    {
                        openset.AddLast(neighbor);
                    }
                }

            }

            // Finds the closest point
            int lowest = 9999;
            foreach (Point score in openset)
            {
                int check = (int)f_score[score];
                if (check < lowest)
                {
                    lowest = check;
                    current = score;
                }
            }

            if ((current.X == goal.X) && (current.Y == goal.Y))
            {
                return goal;
            }

            openset.Remove(current);
            closedset.AddLast(current);

            return current;
        }



       

     

        /**
* current is the Center point
* returns the neighbor points of the current point
*/
        public LinkedList<Point> find_neighbors(Point current)
        {
            LinkedList<Point> neighbor = new LinkedList<Point>();
            neighbor.AddLast(new Point(current.X + 1, current.Y));
            neighbor.AddLast(new Point(current.X, current.Y + 1));
            neighbor.AddLast(new Point(current.X - 1, current.Y));
            neighbor.AddLast(new Point(current.X, current.Y - 1));

            return neighbor;
        }

        /**
* Calculates the distance between the guard and player
*
*/
        public int heuristicCostEstimate(Point start, Point goal)
        {
            int x = start.X - goal.X;
            int y = start.Y - goal.Y;

            int distance = (x * x) + (y * y);

            return distance;
        }

        public Point detectionCheck()
        {
            ArrayList tilesInRange = new ArrayList();

            if (rotation == 0)
            {
                for (int i = 0; i <= 6; i++)
                {
                    for (int j = -3; j <= 3; j++)
                    {
                        tilesInRange.Add(new Point(curTileX + j, curTileY + i));
                    }
                }
            }

            else if (rotation == (float)Math.PI)
            {
                for (int i = 0; i >= -6; i--)
                    for (int j = -3; j <= 3; j++)
                        tilesInRange.Add(new Point(curTileX + j, curTileY + i));
            }

            else if (rotation == 3 * (float)Math.PI / 2)
            {

                for (int i = 0; i <= 6; i++)
                    for (int j = -3; j <= 3; j++)
                        tilesInRange.Add(new Point(curTileX + i, curTileY + j));
            }
            else if (rotation == (float)Math.PI / 2)
            {
                for (int i = 0; i >= -6; i--)
                    for (int j = -3; j <= 3; j++)
                        tilesInRange.Add(new Point(curTileX + i, curTileY + j));
            }

            ISeeYou = false;
            foreach (Point p in tilesInRange)
            {
                if (this.map.mapCell[p.X, p.Y].hasObject("Player"))
                {
                    Console.WriteLine("true");
                    ISeeYou = true;
                    return new Point(p.X, p.Y);
                }
            }
            return new Point(-1, -1);
        }

        
        public override void Update (GameTime gameTime)
        {
           //Debug//Console.WriteLine("offsetx: " +originalOffsetX + " " + "offsety: "+originalOffsetY +" "+ "tilex: " + curTileX + "tiley: " + curTileY) +" "+ "destx: " + patrolTiles[itr].X + " " + "desty: "+ patrolTiles[itr].Y);

            Point current;
            Point goal = detectionCheck();
            Console.WriteLine(goal.X + " " + goal.Y);

            // If the guard see's you it will create it's own path, otherwise use it's set path
            if (ISeeYou)
            {
                length = 2;
                current.X = curTileX;
                current.Y = curTileY;
                moveTo = pathFinder(current, goal);
            }
            else // Patrol
            {

                current.X = curTileX;
                current.Y = curTileY;

                if (patrolTiles.Length > 1)
                {
                    moveTo = pathFinder(current, patrolTiles[itr]);
                    length = patrolTiles.Length;
                }
                else
                {
                    moveTo = pathFinder(current, patrolTiles[0]);
                    if ((curTileX == patrolTiles[0].X) && (curTileY == patrolTiles[0].Y))
                    {
                        length = patrolTiles.Length;
                    }
                }
                
            }

            
            if (length > 1)
            {
                if (moveTo.Y - curTileY > 0) //still moving in terms of Y
                {
                    movingVertical = true;

                    movingHorizontal = false;
                    if (moveBy < 0)
                        moveBy = moveBy * -1;

                    this.rotation = 0;
                }
                else if (moveTo.Y - curTileY < 0)
                {
                    movingVertical = true;
                    movingHorizontal = false;
                    if (moveBy > 0)
                        moveBy = moveBy * -1;

                    this.rotation = (float)Math.PI;
                }
                else if (moveTo.X - curTileX > 0) //moving in terms of X
                {
                    movingVertical = false;
                    movingHorizontal = true;
                    if (moveBy < 0)
                        moveBy = moveBy * -1;

                    this.rotation = 3*(float)Math.PI/2;
                }
                else if (moveTo.X - curTileX < 0)
                {
                    movingVertical = false;
                    movingHorizontal = true;
                    if (moveBy > 0)
                        moveBy = moveBy * -1;

                    this.rotation = (float)Math.PI/2;
                }
                else
                {
                    //shouldn't happen
                }
                if (curTileX == moveTo.X && movingVertical) //move in terms of y
                {
                    //Debug//Console.WriteLine("inY");
                    this.originalOffsetY += moveBy;

                    if (curTileY == moveTo.Y)
                    {
                        if ((moveBy > 0 && originalOffsetY >= 64) || (moveBy < 0 && originalOffsetY <= 0))
                        {
                            goal = detectionCheck();
                            current.X = curTileX;
                            current.Y = curTileY;

                            if (!ISeeYou)
                            {
                                itr++;
                                if (itr >= patrolTiles.Length)
                                    itr = 0;

                                if (patrolTiles.Length > 1)
                                {
                                    moveTo = pathFinder(current, patrolTiles[itr]);
                                    length = patrolTiles.Length;
                                }
                                else
                                {
                                    moveTo = pathFinder(current, patrolTiles[0]);
                                    if ((curTileX == patrolTiles[0].X) && (curTileY == patrolTiles[0].Y))
                                    {
                                        length = patrolTiles.Length;
                                    }
                                }
                            }
                            else
                            {
                                moveTo = pathFinder(current, goal);
                            }
                            
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
                    if (curTileX == moveTo.X)
                    {
                        if ((moveBy > 0 && originalOffsetX >= 64) || (moveBy < 0 && originalOffsetX <= 0))
                        {
                            goal = detectionCheck(); // c
                            current.X = curTileX;
                            current.Y = curTileY;

                            // Checks whether the guard see you or not.
                            if (!ISeeYou)
                            {
                                itr++;
                                if (itr >= patrolTiles.Length)
                                    itr = 0;

                                if (patrolTiles.Length > 1)
                                {
                                    moveTo = pathFinder(current, patrolTiles[itr]);
                                    length = patrolTiles.Length;
                                }
                                else
                                {
                                    moveTo = pathFinder(current, patrolTiles[0]);
                                    if ((curTileX == patrolTiles[0].X) && (curTileY == patrolTiles[0].Y))
                                    {
                                        length = patrolTiles.Length;
                                    }
                                }
                            }
                            else
                            {
                                moveTo = pathFinder(current, goal);
                            }
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



        public override void draw(SpriteBatch spriteBatch, int x, int y, int firstX, int firstY, int offsetX, int offsetY)
        {

            this.source.Y = this.frameStartY + this.frameSkipY * this.frameCount;

            //go through each cell containing objects
            foreach (string objectID in map.mapCell[x + firstX, y + firstY].Objects)
            {
                // draw the specific object
                if (objectID.Equals(this.objectID))
                {
                    Vector2 origin = new Vector2(this.source.Width / 2, this.source.Height / 2);
                    destination = new Rectangle(
                    (x * 64) + this.originalOffsetX - offsetX,
                    (y * 64) + this.originalOffsetY - offsetY, this.frameWidth, this.frameHeight);
                    boundingBox = new Rectangle(
                    (x * 64) + this.originalOffsetX - offsetX - (source.Width / 2),
                    (y * 64) + this.originalOffsetY - offsetY - (source.Width / 2) + 16, this.frameWidth, this.frameHeight);
                    spriteBatch.Draw(
                        texture,
                        destination,
                        source,
                        Color.White,
                        this.rotation, origin, SpriteEffects.None, 0);
                }
           

            }

        }


        public void collide()
        {
            
            
        }
        public void UpdateAnimation()
        {

            //To do

        }
	}
}
