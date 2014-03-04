using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Raycast.Engine.Extensions;
using Raycast.Engine.Models;

namespace Raycast.Engine
{
	public class Caster
	{
		protected char[][] Map { get; set; }
		protected CastedRay[] Rays { get; set; }

		public Caster (char[][] map)
		{
			Map = map;
			Rays = new CastedRay[Constants.SCREEN_WIDTH + 1];
			for (int i = 0; i <= Constants.SCREEN_WIDTH; i++) {
				Rays[i] = new CastedRay();
			}
		}

		public CastedRay CastRayForPixel(int pixelNumber, Vector2 startPoint, 
			Vector2 directionVector, Vector2 translateVector, float angle, float count)
		{
			var currentLocation = Vector2.Add(startPoint, translateVector);
			int x = (int)Math.Round(currentLocation.X);
			int y = (int)Math.Round(currentLocation.Y);
			count+= 0.5f;

			if (Map[y][x] == ' ') 
			{
				var newTranslate = Vector2.Multiply(directionVector, count);
				return CastRayForPixel(pixelNumber, startPoint, directionVector, newTranslate, angle, count);
			}
			else
			{
				var ray = Rays[pixelNumber];
				ray.PixelOnScreen = pixelNumber;
				ray.HitLocation = currentLocation;
				ray.CalculateTextureOffset(x, y);
				ray.StartPoint = startPoint;
				ray.TranslateVector = translateVector;
				return ray;
			}
		}
	}
}

