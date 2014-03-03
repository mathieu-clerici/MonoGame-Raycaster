using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Raycast.Engine.Models
{
	public class Wall
	{
		public int X { get; set;}
		public float Ratio { get; set; }
		public float YCenter { get; set; }
		public int YTop { get; set; }
		public int YBottom { get; set; }
		public CastedRay Ray { get; set; }
		public Rectangle Source { get; set; }
		public Rectangle Dest { get; set; }
		public Floor[] Floor { get; set; }

		public Wall(){ }

		protected static Rectangle[] SourceTextureLokkupTable;

		public static void BuildLookupTable()
		{
			SourceTextureLokkupTable = new Rectangle[Constants.TEXTURE_SIZE + 1];
			for (int i = 0; i <= Constants.TEXTURE_SIZE; i++) 
			{
				SourceTextureLokkupTable[i] =  new Rectangle(i, 0, 1, Constants.TEXTURE_SIZE);
			}
		}

		public void Compute(CastedRay ray)
		{
			Ray = ray;
			Ratio = (Constants.SCREEN_HEIGHT / ray.Distance);
			YCenter = Ratio / 2;
			YTop = (int)(Constants.SCREEN_HEIGHT / 2 - YCenter);
			YBottom = (int)(Constants.SCREEN_HEIGHT / 2 + YCenter);
			Source = SourceTextureLokkupTable[ray.TextureOffsetX];
			Dest = new Rectangle(ray.PixelOnScreen, (int)YTop, 1, (int)Ratio);
			X = ray.PixelOnScreen;
		}
	}
}

