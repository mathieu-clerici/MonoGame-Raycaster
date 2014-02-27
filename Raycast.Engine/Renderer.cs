using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Raycast.Engine.Extensions;
using Raycast.Engine.Models;

namespace Raycast.Engine
{
	public class Renderer
	{
        protected float FOV { get; set; }
        protected int Width { get; set; }
        protected int Height { get; set; }

        public Renderer(int width, int height, float fov)
        {
            FOV = fov;
            Width = width;
            Height = height;
        }

		private Vector2 ScreenDistance
		{
			get
			{ 
				var screenWidth = (float)Width / 1000;
				var angle = MathHelper.ToRadians((180 - 90 - (FOV / 2)));
				var adj = screenWidth / 2;
				var oppose = adj * Math.Tan(angle);
				return new Vector2(screenWidth, (float)oppose);
			}
		}

		public IEnumerable<Wall> RenderWalls(Player player, Level level)
		{
			var originalDirection = Vector2.Zero;
			var screenDistance = ScreenDistance;

			var pointScreenLeft = new Vector2(player.Location.X - ScreenDistance.X / 2, 
				(float)Math.Round(player.Location.Y - ScreenDistance.Y, 1));

			var pointScreenRight = new Vector2(player.Location.X + ScreenDistance.X / 2, 
				(float)Math.Round(player.Location.Y - ScreenDistance.Y, 1));

			pointScreenLeft = pointScreenLeft.Rotate(MathHelper.ToRadians(player.Angle), player.Location);
			pointScreenRight = pointScreenRight.Rotate(MathHelper.ToRadians(player.Angle), player.Location);

			var screenVector = Vector2.Subtract(pointScreenRight, pointScreenLeft).Round(1);
			screenVector.Normalize();
			screenVector = Vector2.Multiply(screenVector, 0.001f);

			for (int i = 0; i < Width; i++) 
			{
				var translateOnScreenVector = Vector2.Multiply(screenVector, 1 + i);
				var point = Vector2.Add(pointScreenLeft, translateOnScreenVector);

				var directionVector = Vector2.Subtract(point, player.Location);

				var originalStep = new Vector2(directionVector.X, directionVector.Y);
				originalStep.Normalize();

				if (originalDirection == Vector2.Zero) {
					originalDirection = directionVector;
				}

				var angle = pointScreenLeft.AngleForPoints(player.Location, point);
				directionVector.Normalize();

				yield return CastRay(level.Map, player.Location, 
					Vector2.Multiply(directionVector, 0.05f), directionVector, angle);
			}
		}

		private Wall CastRay(char[][] map, Vector2 startPoint, Vector2 directionVector, Vector2 translateVector, float angle)
		{
			var currentLocation = Vector2.Add(startPoint, translateVector);
			int x = (int)Math.Round(currentLocation.X);
			int y = (int)Math.Round(currentLocation.Y);

			var distance = Vector2.Subtract(currentLocation, startPoint);
			var diffX = Math.Abs(currentLocation.X - x);
			var diffY = Math.Abs(currentLocation.Y - y);

			if (map[y][x] == ' ' || map[y][x] == 'Z') 
			{
				var newTranslate = Vector2.Add(translateVector, directionVector);
				return CastRay(map, startPoint, directionVector, newTranslate, angle);
			}
			else
			{
				var wall = new Wall();
				if (diffX < diffY) 
				{
					var realX = (int)(currentLocation.X * 64);
					wall.WallOffset = realX % 64;
				} 
				else if (diffX > diffY) 
				{
					var realY = (int)(currentLocation.Y * 64);
					wall.WallOffset = realY % 64;
				}
					
				wall.Location = new Vector2(y, x);
				wall.Distance = CorrectFishEyeEffect(angle, distance);
				return wall;
			}
		}

		private float CorrectFishEyeEffect(float angle, Vector2 distance)
		{
			angle = (float)Math.Round(angle, 0);

			var d = distance.Magnitude();

			if (angle < FOV / 2) { angle = FOV / 2 - angle; } 
			else if (angle > FOV / 2)  { angle = angle - FOV / 2; } 
			else { return distance.Magnitude(); }

			var result = (float) (Math.Cos(MathHelper.ToRadians(angle)) * d);
			return result;
		}
	}
}

