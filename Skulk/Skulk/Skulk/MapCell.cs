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
		    BaseTiles.Add(tileID - 1);
		}

        public void RemoveBaseTile(int tileID)
        {
            BaseTiles.Remove(tileID - 1);
        }

		public void AddObject(string objectID)
		{
			Objects.Add(objectID);
		}

        public void RemoveObject(string objectID)
        {
            Objects.Remove(objectID);
        }


        public bool hasObject(String objectName)
        {
            if (Objects.Contains(objectName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
	}
}

