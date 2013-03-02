using System;
using System.IO;

namespace Skulk
{
	public class TileMap
	{
		public int MapWidth;
		public int MapHeight;
		public MapCell[,] mapCell;

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
                }
				mapLine = tr.ReadLine();
            }
			tr.Close();
			hr.Close();
		}
	}
}

