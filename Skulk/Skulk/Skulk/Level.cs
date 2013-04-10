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
        public List<List<Npc>> guards;
        public int winGoal; // winning door index
        public Point[] itemLocation;
        

        public Level(TileMap[] maps, Point[] goal, Point[] start, int mapIndex, int win)
        {
            this.currentMapIndex = mapIndex;
            this.maps = maps;
            this.currentMap = maps[mapIndex];
            this.goal = goal;
            currentGoal = this.goal[mapIndex];
            this.start = start;
            guards = new List<List<Npc>>();
            this.winGoal = win;
        }

        public void initiailizeGuards(List<Npc> arr)
        {
            guards.Insert(this.currentMapIndex,arr);
        }
    }
}
