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

		public Caster (char[][] map)
		{
			Map = map;
		}

		public CastedRay CastRayForPixel(int pixelNumber, Vector2 startPoint, 
			Vector2 directionVector, Vector2 translateVector, float angle)
		{
			var currentLocation = Vector2.Add(startPoint, translateVector);
			int x = (int)Math.Round(currentLocation.X);
			int y = (int)Math.Round(currentLocation.Y);

			if (Map[y][x] == ' ') 
			{
				var test = PerformDDA(currentLocation, directionVector);
				var newTranslate = Vector2.Add(translateVector, test);
				return CastRayForPixel(pixelNumber, startPoint, directionVector, newTranslate, angle);
			}
			else
			{
				var ray = new CastedRay();
				ray.PixelOnScreen = pixelNumber;
				ray.HitLocation = currentLocation;
				ray.CalculateTextureOffset(x, y);
				ray.StartPoint = startPoint;
				ray.TranslateVector = translateVector;
				return ray;
			}
		}

		private Vector2 PerformDDA(Vector2 currentLocation, Vector2 direction)
		{
			int x = (int)Math.Round(currentLocation.X);
			int y = (int)Math.Round(currentLocation.Y);

			var newX = direction.X > 0 ? x + 1f : x - 1f;
			//float deltaX = currentLocation.X - newX;
			//newX = currentLocation.X - deltaX;
			var resultingY = newX *  currentLocation.Y / currentLocation.X;
			var resultingXLocation = new Vector2(newX, resultingY);
			var distanceX = Vector2.Subtract(resultingXLocation, currentLocation);

			var newY = direction.Y > 0 ? y + 1f : y - 1f;
			//float deltaY = currentLocation.Y - newY;
			//newY = currentLocation.Y - deltaY;
			var resultingX = newY *  currentLocation.X / currentLocation.Y;
			var resultingYLocation = new Vector2(resultingX, newY);
			var distanceY = Vector2.Subtract(resultingYLocation, currentLocation);

			return direction;
		}
	}
}

