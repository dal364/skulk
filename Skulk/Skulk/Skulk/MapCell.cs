using System;
using System.Collections.Generic;

namespace Skulk
{
	public class MapCell
	{
		public List<int> BaseTiles = new List<int>();
		public List<string> Objects = new List<string>();
		public int TileID {
			get { return BaseTiles.Count > 0 ? BaseTiles [0] : 0; }
			set {
				if (BaseTiles.Count > 0)
					BaseTiles [0] = value;
				else
					AddBaseTile (value);
			}
		}

		public MapCell(int tileID)
		{
    		TileID = tileID;
		}

		public void AddBaseTile(int tileID)
		{
		    BaseTiles.Add(tileID);
		}

		public void AddObject(string objectID)
		{
			Objects.Add(objectID);
		}

        public void RemoveObject(string objectID)
        {
            Objects.Remove(objectID);
        }

	}
}

