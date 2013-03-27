using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Skulk
{

    public class Npc : Object
    {

        int animationCount; // How many ticks since the last frame change.
        int animationMax = 10; // How many ticks to change frame after.

        Point[] patrolTiles;
        int itr; //iterator for patrolTiles
        int moveByY;
        int moveByX;
        int speed;


        Point moveTo;

        int length; // Patrol list length or to let it know to use pathfinder length
        Boolean init;
        Boolean ISeeYou;

        public Npc(Game game)
            : base(game)
        {
            base.frameHeight = 64;
            base.frameWidth = 64;
            base.frameStartY = 0;
            base.frameSkipY = 64;
        }

        public void initialize(TileMap map, int x, int y, int offsetX, int offsetY, Texture2D texture, string objectID, Point[] patrolTiles, int speed)
        {
            this.patrolTiles = patrolTiles;
            this.originalOffsetX = offsetX; //offset in current tile
            this.originalOffsetY = offsetY; //offset in current tile
            this.speed = speed;
            this.texture = texture;
            this.map = map;
            this.objectID = objectID;
            this.currentTile = new Point(x, y);
            this.moveByX = speed;
            this.moveByY = speed;

            this.itr = 1; //index of next tile to move to, guard initially drawn at tile 0 it patrolTiles, so start itr at 1
            this.moveTo = new Point(-1, -1);
            this.length = 0;
            this.init = true;
            this.ISeeYou = false;

            map.mapCell[x, y].AddObject(objectID);

            source = new Rectangle(
                frameStartX,
                frameStartY + frameSkipY * frameCount,
                frameWidth,
                frameHeight
                );
        }
        public override void Update(GameTime gameTime)
        {
            //Console.WriteLine("offsetx: " +originalOffsetX + " " + "offsety: "+originalOffsetY +" "+ "tilex: " + currentTile.X + "tiley: " + currentTile.Y +" "+ "destx: " + moveTo.X + " " + "desty: "+ moveTo.Y);

            Point goal = detectionCheck();
            //Console.WriteLine(goal.X + " " + goal.Y);

            // If the guard see's you it will create it's own path, otherwise use it's set path
            if (ISeeYou)
            {
                length = 2;
                moveTo = pathFinder(currentTile, goal);
            }
            else // Patrol
            {

                if (patrolTiles.Length > 1)
                {
                    moveTo = pathFinder(currentTile, patrolTiles[itr]);
                    length = patrolTiles.Length;
                }
                else
                {
                    moveTo = pathFinder(currentTile, patrolTiles[0]);
                    if ((currentTile.X == patrolTiles[0].X) && (currentTile.Y == patrolTiles[0].Y))
                    {
                        length = patrolTiles.Length;
                    }
                }

            }
            if (length > 1)
            {
                if (moveTo.Y - currentTile.Y > 0)//moving down
                {
                    if (moveByY < 0)
                    {
                        moveByY *= -1;
                    }
                    this.rotation = 0;
                }
                else if (moveTo.Y - currentTile.Y < 0) //moving up
                {
                    if (moveByY > 0)
                    {
                        moveByY *= -1;
                    }
                    this.rotation = (float)Math.PI;
                }

                if (moveTo.X - currentTile.X > 0) //moving right
                {
                    if (moveByX < 0)
                        moveByX *= -1;

                    this.rotation = 3 * (float)Math.PI / 2;

                    if (moveTo.Y - currentTile.Y > 0)
                        this.rotation = 7 * (float)Math.PI / 4;
                    else if (moveTo.Y - currentTile.Y < 0)
                        this.rotation = 5 * (float)Math.PI / 4;
                }
                else if (moveTo.X - currentTile.X < 0) //moving left
                {
                    if (moveByX > 0)
                        moveByX *= -1;

                    this.rotation = (float)Math.PI / 2;

                    if (moveTo.Y - currentTile.Y > 0)
                        this.rotation = (float)Math.PI / 4;
                    else if (moveTo.Y - currentTile.Y < 0)
                        this.rotation = 3 * (float)Math.PI / 4;
                }

                if (currentTile.X == moveTo.X && currentTile.Y == moveTo.Y)
                {
                    /*
                    if ((moveByX > 0 && originalOffsetX >= 64) || (moveByX < 0 && originalOffsetX <= 0))
                    {
                        //do nothing 
                    }
                    else
                        this.originalOffsetX += moveByX;
                     * */
                    if (!ISeeYou)
                    {
                        itr++;
                        if (itr >= patrolTiles.Length)
                            itr = 0;

                        if (patrolTiles.Length > 1)
                        {
                            moveTo = pathFinder(currentTile, patrolTiles[itr]);
                            length = patrolTiles.Length;
                        }
                        else
                        {
                            moveTo = pathFinder(currentTile, patrolTiles[0]);
                            if ((currentTile.X == patrolTiles[0].X) && (currentTile.Y == patrolTiles[0].Y))
                            {
                                length = patrolTiles.Length;
                            }
                        }
                    }
                    else
                        moveTo = pathFinder(currentTile, goal);


                }
                else //update positions
                {
                    if (currentTile.Y != moveTo.Y)
                    {
                        //update y position
                        int lastOffsetY = this.originalOffsetY;

                        this.originalOffsetY += moveByY;
                        if (this.originalOffsetY >= 64)
                        {
                            this.map.mapCell[currentTile.X, currentTile.Y].RemoveObject(objectID);
                            this.currentTile.Y += 1;
                            this.map.mapCell[currentTile.X, currentTile.Y].AddObject(objectID);
                            this.originalOffsetY = moveByY - (64 - lastOffsetY);
                        }
                        else if (this.originalOffsetY < 0)
                        {
                            this.map.mapCell[currentTile.X, currentTile.Y].RemoveObject(objectID);
                            this.currentTile.Y -= 1;
                            this.map.mapCell[currentTile.X, currentTile.Y].AddObject(objectID);
                            this.originalOffsetY = 64 + moveByY + lastOffsetY;
                        }
                    }

                    if (currentTile.X != moveTo.X)
                    {
                        //update x position
                        int lastOffsetX = this.originalOffsetX;
                        this.originalOffsetX += moveByX;

                        if (this.originalOffsetX >= 64) //moving right and reach end of tile
                        {
                            this.map.mapCell[currentTile.X, currentTile.Y].RemoveObject(objectID);
                            this.currentTile.X += 1;
                            this.map.mapCell[currentTile.X, currentTile.Y].AddObject(objectID);
                            this.originalOffsetX = moveByX - (64 - lastOffsetX);
                        }
                        else if (this.originalOffsetX < 0)//moving left and reach end of tile
                        {
                            this.map.mapCell[currentTile.X, currentTile.Y].RemoveObject(objectID);
                            this.currentTile.X -= 1;
                            this.map.mapCell[currentTile.X, currentTile.Y].AddObject(objectID);
                            this.originalOffsetX = 64 + moveByX + lastOffsetX;
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
                    (y * 64) + this.originalOffsetY - offsetY - (source.Width / 2), this.frameWidth, this.frameHeight);
                    spriteBatch.Draw(
                        texture,
                        destination,
                        source,
                        Color.White,
                        this.rotation, origin, SpriteEffects.None, 0);
                }
                /*boundingBox Debug
           spritebatch.Draw(
                       texture,
                       boundingBox,
                       source,
                       Color.Black
                       );*/
                

            }

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
            neighbor.AddLast(new Point(current.X - 1, current.Y - 1));
            neighbor.AddLast(new Point(current.X + 1, current.Y + 1));
            neighbor.AddLast(new Point(current.X - 1, current.Y + 1));
            neighbor.AddLast(new Point(current.X + 1, current.Y - 1));

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
                for (int i = 0; i <= 4; i++)
                {
                    for (int j = -3; j <= 3; j++)
                    {
                        tilesInRange.Add(new Point(currentTile.X + j, currentTile.Y + i));
                    }
                }
            }

            else if (rotation == (float)Math.PI)
            {
                for (int i = 0; i <= 4; i++)
                    for (int j = -3; j <= 3; j++)
                        tilesInRange.Add(new Point(currentTile.X + j, currentTile.Y - i));
            }

            else if (rotation == 3 * (float)Math.PI / 2)
            {

                for (int i = 0; i <= 4; i++)
                    for (int j = -3; j <= 3; j++)
                        tilesInRange.Add(new Point(currentTile.X + i, currentTile.Y + j));
            }
            else if (rotation == (float)Math.PI / 2)
            {
                for (int i = 0; i <= 4; i++)
                    for (int j = -3; j <= 3; j++)
                        tilesInRange.Add(new Point(currentTile.X - i, currentTile.Y + j));
            }
            else if (rotation == (float)Math.PI / 4)
            {
                for (int i = 0; i <= 4; i++)
                    for (int j = 0; j <= 4; j++)
                        tilesInRange.Add(new Point(currentTile.X - j, currentTile.Y + i));

            }
            else if (rotation == 3 * (float)Math.PI / 4)
            {
                for (int i = 0; i <= 4; i++)
                    for (int j = 0; j <= 4; j++)
                        tilesInRange.Add(new Point(currentTile.X - j, currentTile.Y - i));
            }
            else if (rotation == 5 * (float)Math.PI / 4)
            {
                for (int i = 0; i <= 4; i++)
                    for (int j = 0; j <= 4; j++)
                        tilesInRange.Add(new Point(currentTile.X + j, currentTile.Y - i));
            }
            else if (rotation == 7 * (float)Math.PI / 4)
            {
                for (int i = 0; i <= 4; i++)
                    for (int j = 0; j <= 4; j++)
                        tilesInRange.Add(new Point(currentTile.X + j, currentTile.Y + i));
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
    }
}
