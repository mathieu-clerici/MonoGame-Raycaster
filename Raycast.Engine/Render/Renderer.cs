using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Raycast.Engine.Extensions;
using Raycast.Engine.Models;
using System.Threading.Tasks;

namespace Raycast.Engine
{
	public class Renderer : AbstractRenderer
	{
		public Renderer(char[][] map) : base(map){}


		public Wall[] RenderWalls(Player player, Level level)
		{
			var screenDistance = ScreenDistance;
			var playerX = (float)Math.Round(player.Location.X, 1);
			var playerY = (float)Math.Round(player.Location.Y, 1);

			var pointScreenLeft = PointsScreenLeft[playerX][playerY];
			var pointScreenRight = PointsScreenRight[playerX][playerY];
			var pointScreenMiddle = PointsScreenMiddle[playerX][playerY];
				
			pointScreenLeft = pointScreenLeft.RotateInstance(MathHelper.ToRadians(player.Angle), player.Location);
			pointScreenRight = pointScreenRight.RotateInstance(MathHelper.ToRadians(player.Angle), player.Location);
			pointScreenMiddle = pointScreenMiddle.RotateInstance(MathHelper.ToRadians(player.Angle), player.Location);

			RenderWallTasks[0] = DrawHalfAsyncTask(player, 0, 1, pointScreenMiddle, pointScreenLeft);
			RenderWallTasks[1] =  DrawHalfAsyncTask(player, Constants.SCREEN_WIDTH, -1, pointScreenMiddle, pointScreenRight);
			Task.WaitAll(RenderWallTasks);

			return Walls;
		}

		private Task DrawHalfAsyncTask(Player player, int currentPixel, 
            int incOrdec, Vector2 middlePoint, Vector2 otherPoint)
		{
			Action temp = () => 
			{
				var screenVector = Vector2.Subtract(middlePoint, otherPoint).RoundInstance(1);
				screenVector.Normalize();
				screenVector = screenVector.MultiplyInstance(0.001f);

				for (int i = 0; i <= (Constants.SCREEN_WIDTH / 2); i++) 
				{
					var translateOnScreenVector = Vector2.Multiply(screenVector, i);
					var point = Vector2.Add(otherPoint, translateOnScreenVector);
					var directionVector = Vector2.Subtract(point, player.Location);
					var angle = i * (((float)Constants.FOV)/2) / (((float)Constants.SCREEN_WIDTH) / 2);
					var ray = Caster.CastRayForPixel (currentPixel, player.Location,
						directionVector.MultiplyInstance(0.1f), Vector2.Zero, angle, 1f);

					ray.Distance = CorrectFishEyeEffect(angle, ray);
					currentPixel += incOrdec;
					Walls[currentPixel].Compute(ray);
					ComputeFloor(Walls[currentPixel], player);
				}
			};
			return Task.Factory.StartNew(temp);
		}

		public void ComputeFloor(Wall wall, Player player)
		{
			if (wall.YBottom < 0) {
				wall.YBottom = Constants.SCREEN_HEIGHT;
			}

			var wallDistance = wall.Ray.Distance;
			var floorLocationX = wall.Ray.HitLocation.X;
			var floorLocationY = wall.Ray.HitLocation.Y;
			int begin = wall.YBottom; 

			var floorCount = (Constants.SCREEN_HEIGHT / 2);
			CleanWallArray(floorCount, wall);
			for (int i = wall.YBottom; i < Constants.SCREEN_HEIGHT; i++) 
			{
				Floors[wall.X][i - floorCount].Compute(wall, player, i, 
					wallDistance, floorLocationX, floorLocationY);
			}

			wall.Floor = Floors[wall.X];
		}

		protected float CorrectFishEyeEffect(float angle, CastedRay ray)
		{
			if (angle == 33f) {
				return CorrectDistance (ray);
			}

			var halfFOV = (int) Constants.FOV / 2;
			angle =  halfFOV - angle;
			var d = CorrectDistance(ray);
			return (float) (Math.Cos(MathHelper.ToRadians(angle)) * d);
		}

        float CorrectDistance(CastedRay ray)
        {
			var d = (float)ray.TranslateVector.Length();
            return d;
        }
	}
}

