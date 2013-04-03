using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Skulk
{
	public static class Tile
	{
		static public Texture2D TileSetTexture;

		static public Rectangle GetSourceRectangle (int tileIndex, int size)
		{
            int tileY = tileIndex / (TileSetTexture.Width / size);
            int tileX = tileIndex % (TileSetTexture.Width / size);
    		return new Rectangle(tileX * size, tileY * size, size, size);	
		}
	}
}

