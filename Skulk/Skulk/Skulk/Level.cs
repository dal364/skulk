using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Skulk
{
    class Level
    {
        public TileMap[] maps;
        public TileMap currentMap;
        public int currentMapIndex;
        public Point[] goal;
        public Point currentGoal;
        public Point[] start;
        public ArrayList[] guards;
        public Point[] itemLocation;
        

        public Level(TileMap[] maps, Point[] goal, Point[] start, int mapIndex)
        {
            this.currentMapIndex = mapIndex;
            this.maps = maps;
            this.currentMap = maps[mapIndex];
            this.goal = goal;
            currentGoal = this.goal[mapIndex];
            this.start = start;
            guards = new ArrayList[maps.Length];
        }

        public void initiailizeGuards(ArrayList arr)
        {
            guards[this.currentMapIndex] = arr;
        }
    }
}
