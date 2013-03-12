using System;
using System.IO;
using System.Collections;
namespace Skulk
{
	public class TileMap
	{
		public int MapWidth;
		public int MapHeight;
		public MapCell[,] mapCell;
        public ArrayList obstacleTiles; //tiles that can not be walked on

		public TileMap ()
		{
            //parse spreadsheet for tiles
			TextReader tr = new StreamReader("map.csv");
			TextReader hr = new StreamReader("map.csv");
			String mapLine = tr.ReadLine();
			MapHeight = hr.ReadToEnd().Split('\n').Length - 1;
			MapWidth = mapLine.Split(';').Length;

			mapCell = new MapCell[MapWidth,MapHeight];


			for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
					this.mapCell[x,y] = new MapCell(Convert.ToInt32(mapLine.Split(';')[x]));
                    if (Convert.ToInt32(mapLine.Split(';')[x]) == 7)//integer corresponding to walls or boxes,etc
                    {
                        obstacleTiles.Add(this.mapCell[x, y]);
                    }
                }
				mapLine = tr.ReadLine();
            }
			tr.Close();
			hr.Close();
		}
	}
}

