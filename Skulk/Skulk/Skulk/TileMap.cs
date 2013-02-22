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
			TextReader tr = new StreamReader("map.csv");
			TextReader hr = new StreamReader("map.csv");
			String mapLine = tr.ReadLine();
			MapHeight = hr.ReadToEnd().Split('\n').Length - 1;
			MapWidth = mapLine.Split(';').Length;
			Console.WriteLine(MapHeight);
			Console.WriteLine(MapWidth); 

			mapCell = new MapCell[MapWidth,MapHeight];


			for (int y = 0; y < MapHeight; y++)
            {

				//Console.WriteLine(mapLine);
                for (int x = 0; x < MapWidth; x++)
                {
					//Console.WriteLine(mapLine.Split(';')[x] + "x");
					this.mapCell[x,y] = new MapCell(Convert.ToInt32(mapLine.Split(';')[x]));
					//Console.WriteLine(mapCell[x,y].TileID);
                }
				mapLine = tr.ReadLine();
            }
			tr.Close();
			hr.Close();
		}
	}
}

