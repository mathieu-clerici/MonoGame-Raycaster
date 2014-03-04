using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Raycast.Engine.Models
{
	public class Floor
	{
		public bool ToDraw { get; set;}

		private static Color[,] floorColors;
		private static Color[,] ceilingColors;

		public static Color[] floorSurfaceColors;
		public static Color[] ceilingSurfaceColors;

		private static Texture2D floorTexture{ get; set; }
		private static Texture2D ceilingTexture{ get; set; }
		public static Texture2D FloorSprite{ get; set; }
		public static Texture2D CeilingSprite{ get; set; }
		public static float[] DistanceLookupTable { get; set; }

		public static void BuildLookupTable()
		{
			var middle = (int)(Constants.SCREEN_HEIGHT / 2);
			DistanceLookupTable = new float[middle];
			for (int i = middle; i < Constants.SCREEN_HEIGHT; i++) 
			{
				var index = Math.Abs(i - Constants.SCREEN_HEIGHT);
				var result =  Constants.SCREEN_HEIGHT / (2.0 * i - Constants.SCREEN_HEIGHT);
				DistanceLookupTable [index - 1] = (float)result;
			}
		}

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
			FloorSprite = new Texture2D(gf, w, h);
			CeilingSprite = new Texture2D(gf, w, h);

			floorTexture = content.Load<Texture2D>("wood");
			ceilingTexture = content.Load<Texture2D>("ceiling");

			floorColors = TextureTo2DArray(floorTexture);
			ceilingColors = TextureTo2DArray (ceilingTexture);

			floorSurfaceColors = TextureToArray(FloorSprite);
			ceilingSurfaceColors = TextureToArray(CeilingSprite);

			for (int i = 0; i <  floorSurfaceColors.Length; i++) 
			{
				floorSurfaceColors[i] = Color.Black;
				ceilingSurfaceColors [i] = Color.Black;
			}
		}

		public Floor()
		{
			ToDraw = false;
		}

		public void Compute(Wall wall, Player player, int i, float wallDistance, float floorLocationX, float floorLocationY)
		{
			var currentDistance = DistanceLookupTable[Math.Abs(i - Constants.SCREEN_HEIGHT)];
			var weight = currentDistance / wallDistance;

			var currentFloorX = weight * floorLocationX + (1.0 - weight) * player.Location.X;
			var currentFloorY = weight * floorLocationY + (1.0 - weight) * player.Location.Y;

			var floorTexX = (int)(currentFloorX * Constants.TEXTURE_SIZE) % Constants.TEXTURE_SIZE;
			var floorTexY = (int)(currentFloorY * Constants.TEXTURE_SIZE) % Constants.TEXTURE_SIZE;

			var test = i - (Constants.SCREEN_HEIGHT / 2);
			var indexInSurfaceFloor =  (wall.X - 1) + test * (Constants.SCREEN_WIDTH);
			var indexInSurfaceCeiling =  (wall.X - 1) +  ((Constants.SCREEN_HEIGHT / 2) - test) * (Constants.SCREEN_WIDTH);

			floorSurfaceColors[indexInSurfaceFloor] = floorColors[floorTexX, floorTexY];
			ceilingSurfaceColors[indexInSurfaceCeiling] = ceilingColors[floorTexX, floorTexY];
			ToDraw = true;
		}

	}
}

