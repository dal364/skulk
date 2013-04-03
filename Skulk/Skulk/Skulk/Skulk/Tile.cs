using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Skulk
{
	public static class Tile
	{
		static public Texture2D TileSetTexture;

		static public Rectangle GetSourceRectangle (int tileIndex)
		{
    		return new Rectangle(0, tileIndex * 64, 64, 64);	
		}
	}
}

