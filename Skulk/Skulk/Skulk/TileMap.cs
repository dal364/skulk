using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Skulk
{

    public class TileMap
    {
        public int MapWidth;
        public int MapHeight;
        public MapCell[,] mapCell;

        public LinkedList<Point> obstacleTiles; //tiles that can not be walked on

        public TileMap(int lvl, int map)
        {
            obstacleTiles = new LinkedList<Point>();
            //parse spreadsheets for tiles
            TextReader tr = new StreamReader("lvl"+ lvl + "map" + map + "layer1" + ".csv");
            TextReader hr = new StreamReader("lvl"+ lvl + "map" + map + "layer1" + ".csv");

            TextReader obstacleTr = new StreamReader("lvl"+ lvl + "map" + map + "layer2" + ".csv");
            TextReader obstacleHr = new StreamReader("lvl"+ lvl + "map" + map + "layer2" + ".csv");

            String mapLine = tr.ReadLine();

            String mapLineObstacle = obstacleTr.ReadLine();

            MapHeight = hr.ReadToEnd().Split('\n').Length - 1;
            MapWidth = mapLine.Split(',').Length;

            mapCell = new MapCell[MapWidth, MapHeight];


            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {

                    this.mapCell[x, y] = new MapCell(Convert.ToInt32(mapLine.Split(',')[x]));
                    this.mapCell[x, y].AddBaseTile(Convert.ToInt32(mapLineObstacle.Split(',')[x]));
                    if (Convert.ToInt32(mapLineObstacle.Split(',')[x]) == 227)
                        this.mapCell[x, y].AddObject("hole");


                    if (Convert.ToInt32(mapLineObstacle.Split(',')[x]) != 0 && Convert.ToInt32(mapLineObstacle.Split(',')[x]) !=227)//integer corresponding to walls or boxes,etc
                    {
                        obstacleTiles.AddFirst(new Point(x, y));
                    }
                }
                mapLine = tr.ReadLine();
                mapLineObstacle = obstacleTr.ReadLine();
            }
            tr.Close();
            hr.Close();
            obstacleHr.Close();
            obstacleTr.Close();
            Console.WriteLine(MapHeight);

        }
    }

}