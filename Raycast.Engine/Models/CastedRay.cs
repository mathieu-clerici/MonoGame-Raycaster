using System;
using Microsoft.Xna.Framework;

namespace Raycast.Engine
{
	public class CastedRay
	{
		public Vector2 HitLocation { get; set; }
		public int PixelOnScreen { get; set; }
		public float Distance { get; set; }
		public int TextureOffsetX { get; set; }
		public Vector2 StartPoint { get; set; }
		public Vector2 TranslateVector { get; set; }

		public CastedRay (){}
		public void CalculateTextureOffset(int x, int y)
		{
			var diffX = Math.Abs(HitLocation.X - x);
			var diffY = Math.Abs(HitLocation.Y - y);

			if (diffX < diffY) 
			{
				var realX = (int)(HitLocation.X * 64);
				TextureOffsetX = realX % 64;
			} 
			else if (diffX > diffY) 
			{
				var realY = (int)(HitLocation.Y * 64);
				TextureOffsetX = realY % 64;
			}
		}
	}
}

