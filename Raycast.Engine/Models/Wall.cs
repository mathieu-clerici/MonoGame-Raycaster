using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
		public Floor[] Floor { get; set; }

		private static Color[,] wallColors;
		public static Color[] wallSurfaceColors;
		private static Texture2D wallTexture{ get; set; }
		public static Texture2D Screen{ get; set; }

		public Wall(){ }

		private static Color[,] TextureTo2DArray(Texture2D texture)
		{
			Color[] colors1D = new Color[texture.Width * texture.Height];
			texture.GetData(colors1D);

			Color[,] colors2D = new Color[texture.Width, texture.Height];
			for (int x = 0; x < texture.Width; x++)
				for (int y = 0; y < texture.Height; y++)
					colors2D[x, y] = colors1D[x + y * texture.Width];

			return colors2D;
		}

		private static Color[] TextureToArray(Texture2D texture)
		{
			Color[] colors1D = new Color[texture.Width * texture.Height];
			texture.GetData(colors1D);
			return colors1D;
		}

		public static void CreateTexture(ContentManager content, GraphicsDevice gf, int w, int h)
		{
			Screen = new Texture2D(gf, w, h);
			wallTexture = content.Load<Texture2D>("redbrick");
			wallColors = TextureTo2DArray(wallTexture);
			wallSurfaceColors = TextureToArray(Screen);
			ClearWalls();
		}

		public static void ClearWalls()
		{
			for (int i = 0; i <  wallSurfaceColors.Length; i++) 
			{
				wallSurfaceColors[i] = Color.Transparent;
			}
		}

		public static void BuildLookupTable()
		{
		}

		public void Compute(CastedRay ray)
		{
			Ray = ray;
			Ratio = (Constants.SCREEN_HEIGHT / ray.Distance);
			YCenter = Ratio / 2;
			YTop = (int)(Constants.SCREEN_HEIGHT / 2 - YCenter);
			YBottom = (int)(Constants.SCREEN_HEIGHT / 2 + YCenter);
			X = ray.PixelOnScreen;

			int j = 0;
			for (int i = (int)YTop; i < YBottom; i++) 
			{
				var index =  X + i * (Constants.SCREEN_WIDTH);
				var current = ((float)j / Ratio);
				if (current > 1f) { current = 1f; }
				if (current < 0f) { current = 0f; }
				var offsetY = (int)MathHelper.Lerp(0, Constants.TEXTURE_SIZE - 1, current);
				wallSurfaceColors [index] = wallColors[ray.TextureOffsetX, offsetY];
				j++;
			}
		}
	}
}

