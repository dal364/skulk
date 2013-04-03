using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;


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
        Boolean ableToMove;

        // For Guards and player interaction
        LinkedList<Point> closedISeeYou;
        public Boolean ISeeYou;
        Boolean wait;
        int waitTime;
        Point lastSpot;
        int time;
        int timeAtGold;
        ArrayList tilesInRange;

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

            this.itr = 0; //index of next tile to move to, guard initially drawn at tile 0 it patrolTiles, so start itr at 1
            this.moveTo = new Point(-1, -1);
            this.ableToMove = true;
            this.ISeeYou = false;
            this.wait = false;
            this.waitTime = 0;
            this.time = 1000;
            this.timeAtGold = 0;
            this.lastSpot = new Point(0, 0);
            tilesInRange = new ArrayList();

            closedISeeYou = new LinkedList<Point>();

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

            Point goal = detectionCheck();
            ableToMove = true;

            // If there is gold on the ground go to it and stop there for a few seconds
            if (this.map.mapCell[currentTile.X, currentTile.Y].hasObject("Gold"))
            {
                timeAtGold += gameTime.ElapsedGameTime.Milliseconds;
                if (timeAtGold > 1500)
                {
                    this.map.mapCell[currentTile.X, currentTile.Y].RemoveObject("Gold");
                    this.map.mapCell[currentTile.X, currentTile.Y].RemoveBaseTile(229);

                    timeAtGold = 0;
                }
                ableToMove = false;
            }
            // If guard lost the player/prisoner then go to last seen place and look around
            else if (wait)
            {
                if (currentTile.X == lastSpot.X && currentTile.Y == lastSpot.Y)
                {
                    ableToMove = false;
                    waitTime = waitTime + gameTime.ElapsedGameTime.Milliseconds;
                    if (time >= 6000)
                    {
                        time = 1000;
                        wait = false;
                        waitTime = 0;
                        lastSpot.X = 0;
                        lastSpot.Y = 0;
                    }
                    else if (waitTime >= time)
                    {
                        this.rotation = this.rotation + (float)Math.PI / 2;
                        this.time = time + 1000;
                    }
                }
                else
                {
                    goal = lastSpot;
                }
            }
            // If the guard see's you then find the shortest distance to the play/prisoner and follow them    
            else if (ISeeYou)
            {
                timeAtGold = 0;
                if (!map.mapCell[currentTile.X, currentTile.Y].hasObject("Gold"))
                {
                    if (currentTile.X == moveTo.X && currentTile.Y == moveTo.Y)
                    {
                        this.originalOffsetX += moveByX;
                        this.originalOffsetY += moveByY;
                    }
                }
                else
                {
                    ableToMove = false;
                }

            }
            // If none of the conditions above are true then go to set path, Patrol the area
            else
            {

                if (patrolTiles.Length == 1)
                {
                    if ((currentTile.X == patrolTiles[0].X) && (currentTile.Y == patrolTiles[0].Y))
                    {
                        ableToMove = false;
                    }
                }

                goal = patrolTiles[itr];
            }

            // Gets the next position to move to
            moveTo = pathFinder(currentTile, goal);

            // Only if the guard/prisoner is moving
            if (ableToMove)
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

                if (currentTile.X != moveTo.X || currentTile.Y != moveTo.Y)
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
                else if (currentTile.X == moveTo.X && currentTile.Y == moveTo.Y)
                {
                    // For the patrol, only increment to next position if it made it to the next tile
                    if (!ISeeYou)
                    {
                        itr++;
                        if (itr >= patrolTiles.Length)
                            itr = 0;
                    }
                }

                this.animationCount += 1;
                this.UpdateAnimation();
            }

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
            // if there the same skip everything and return
            if ((goal.X == start.X) && (goal.Y == start.Y))
            {
                return goal;
            }

            // Clear out the closed list for finding a way back to patrol point, don't need it if the guard is going to be somewhere else
            if (ISeeYou)
            {
                closedISeeYou.Clear();
            }


            LinkedList<Point> openset = new LinkedList<Point>();
            LinkedList<Point> closedset = map.obstacleTiles;

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
                    // Skip
                }
                else if (!openset.Contains(neighbor))
                {
                    // For finding his way back home
                    if (closedISeeYou.Contains(neighbor) && (!ISeeYou))
                    {
                        // Skip
                    }
                    else
                    {
                        f_score[neighbor] = heuristicCostEstimate(neighbor, goal);
                        openset.AddLast(neighbor);
                    }
                }

            }

            // Finds the closest point
            int lowest = 9999;
            foreach (Point score in openset)
            {
                int check = (int)f_score[score];


                if (check == 0)
                {
                    closedISeeYou.Clear();
                    return goal;
                }
                else if (check < lowest)
                {
                    lowest = check;
                    current = score;
                }

                // Randomize direction for going around objects????? Not working
                /*else if (check == lowest)
                {
                    Random rand = new Random();
                    int coin = rand.Next(-10, 10);
                    Console.WriteLine(coin + "******************");
                    if (coin <= 0)
                    {
                        current = score;
                    }
                }*/
            }

            if ((current.X == start.X) && (current.Y == start.Y))
            {
                closedISeeYou.AddLast(current);
                return pathFinder(start, goal);
            }
            else
            {
                return current;
            }
        }


        /**
        * current is the Center point
        * returns the neighbor points of the current point
        */
        public LinkedList<Point> find_neighbors(Point current)
        {
            LinkedList<Point> neighbor = new LinkedList<Point>();
            neighbor.AddLast(new Point(current.X, current.Y));
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
        * Calculates the distance between the guard and player/prisoner
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
           
            tilesInRange.Clear();

            int lookAhead = 4;
            int lookAcross = 2;//each way (left/right)

            if (rotation == 0) //down
            {
                for (int i = (lookAcross * -1); i <= lookAcross; i++)
                {
                    for (int j = 0; j <= lookAhead; j++)
                    {
                        if (map.obstacleTiles.Contains(new Point(currentTile.X + i, currentTile.Y + j)))
                        {
                            break;
                        }
                        else
                            tilesInRange.Add(new Point(currentTile.X + i, currentTile.Y + j));
                    }
                }
            }

            else if (rotation == (float)Math.PI) //up
            {
                for (int i = (lookAcross * -1); i <= lookAcross; i++)
                {
                    for (int j = 0; j <= lookAhead; j++)
                    {
                        if (map.obstacleTiles.Contains(new Point(currentTile.X + i, currentTile.Y - j)))
                        {
                            break;
                        }
                        else
                            tilesInRange.Add(new Point(currentTile.X + i, currentTile.Y - j));
                    }
                }
            }

            else if (rotation == 3 * (float)Math.PI / 2) //right
            {

                for (int i = (lookAcross * -1); i <= lookAcross; i++)
                {
                    for (int j = 0; j <= lookAhead; j++)
                    {
                        if (map.obstacleTiles.Contains(new Point(currentTile.X + j, currentTile.Y + i)))
                        {
                            break;
                        }
                        else
                            tilesInRange.Add(new Point(currentTile.X + j, currentTile.Y + i));
                    }
                }
            }
            else if (rotation == (float)Math.PI / 2) //left
            {
                for (int i = (lookAcross * -1); i <= lookAcross; i++)
                {
                    for (int j = 0; j <= lookAhead; j++)
                    {
                        if (map.obstacleTiles.Contains(new Point(currentTile.X - j, currentTile.Y + i)))
                        {
                            break;
                        }
                        else
                            tilesInRange.Add(new Point(currentTile.X - j, currentTile.Y + i));
                    }
                }
            }
            else if (rotation == (float)Math.PI / 4)
            {
                for (int i = 0; i <= lookAhead; i++)
                {
                    for (int j = 0; j <= lookAhead; j++)
                    {
                        if (map.obstacleTiles.Contains(new Point(currentTile.X - j, currentTile.Y + i)))
                            break;
                        else
                            tilesInRange.Add(new Point(currentTile.X - j, currentTile.Y + i));
                    }
                }

            }
            else if (rotation == 3 * (float)Math.PI / 4)
            {
                for (int i = 0; i <= lookAhead; i++)
                {
                    for (int j = 0; j <= lookAhead; j++)
                    {
                        if (map.obstacleTiles.Contains(new Point(currentTile.X - j, currentTile.Y - i)))
                            break;
                        else
                            tilesInRange.Add(new Point(currentTile.X - j, currentTile.Y - i));
                    }
                }
            }
            else if (rotation == 5 * (float)Math.PI / 4)
            {
                for (int i = 0; i <= lookAhead; i++)
                {
                    for (int j = 0; j <= lookAhead; j++)
                    {
                        if (map.obstacleTiles.Contains(new Point(currentTile.X + j, currentTile.Y - i)))
                            break;
                        else
                            tilesInRange.Add(new Point(currentTile.X + j, currentTile.Y - i));
                    }
                }
            }
            else if (rotation == 7 * (float)Math.PI / 4)
            {
                for (int i = 0; i <= lookAhead; i++)
                {
                    for (int j = 0; j <= lookAhead; j++)
                    {
                        if (map.obstacleTiles.Contains(new Point(currentTile.X + j, currentTile.Y + i)))
                            break;
                        else
                            tilesInRange.Add(new Point(currentTile.X + j, currentTile.Y + i));
                    }
                }
            }


            // If the guard is following the player/prisoner, get ready to go to last point and look around (if loosing them)
            if (ISeeYou)
            {
                wait = true;
            }
            ISeeYou = false;
            Point point = currentTile;
            Hashtable f_score = new Hashtable();
            LinkedList<Point> closestList = new LinkedList<Point>();

            // Goes through the detection list to see if gold, player, or prisoner is seen
            foreach (Point p in tilesInRange)
            {
               // map.mapCell[p.X, p.Y].AddBaseTile(229);
                if (this.map.mapCell[p.X, p.Y].hasObject("Gold"))
                {
                    ISeeYou = true;
                    wait = false;
                    lastSpot.X = 0;
                    lastSpot.Y = 0;
                    point.X = p.X;
                    point.Y = p.Y;
                    break;
                }
                else if (this.map.mapCell[p.X, p.Y].hasObject("Player"))
                {
                    ISeeYou = true;
                    wait = false;
                    lastSpot.X = 0;
                    lastSpot.Y = 0;
                    point.X = p.X;
                    point.Y = p.Y;
                    // Calculation for the distance between guard and player/prisoner
                    f_score[point] = heuristicCostEstimate(currentTile, point);
                    closestList.AddLast(point);
                }
                else if (this.map.mapCell[p.X, p.Y].hasObject("Prisoner"))
                {
                    ISeeYou = true;
                    wait = false;
                    lastSpot.X = 0;
                    lastSpot.Y = 0;
                    point.X = p.X;
                    point.Y = p.Y;
                    // Calculation for the distance between guard and player/prisoner
                    f_score[point] = heuristicCostEstimate(currentTile, point);
                    closestList.AddLast(point);
                }
            }

            if (!this.map.mapCell[point.X, point.Y].hasObject("Gold"))
            {
                // Checks which object is closer to the guard, Player or Prisoner, and return the closest point
                int lowest = 9999;
                foreach (Point score in closestList)
                {
                    int check = (int)f_score[score];

                    if (check < lowest)
                    {
                        lowest = check;
                        point = score;
                    }
                }
            }

            // Sets the last point, where the guard seen the player/prisoner
            if (lastSpot.X == 0 && lastSpot.Y == 0)
            {
                lastSpot = point;
            }

            return point;
        }
    }
}