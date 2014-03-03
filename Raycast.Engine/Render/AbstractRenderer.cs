using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Raycast.Engine.Extensions;
using Raycast.Engine.Models;
using System.Threading.Tasks;

namespace Raycast.Engine
{
	public abstract class AbstractRenderer
	{		
		protected Task[] RenderWallTasks { get; set; }
		protected Floor[][] Floors { get; set; }
		protected Wall[] Walls { get; set; }
		protected Caster Caster { get; set; }
		protected Vector2 screenDistance;
		protected Dictionary<float, Dictionary<float, Vector2>> PointsScreenLeft { get; set; }
		protected Dictionary<float, Dictionary<float, Vector2>> PointsScreenRight { get; set; }
		protected Dictionary<float, Dictionary<float, Vector2>> PointsScreenMiddle { get; set; }
		protected char[][] Map { get; set; }

		public AbstractRenderer (char[][] map)
		{
			Map = map;
			Caster = new Caster (map);
			RenderWallTasks = new Task[2];
			Wall.BuildLookupTable();
			Floor.BuildLookupTable();
			PointsScreenLeft = new Dictionary<float, Dictionary<float, Vector2>>();
			PointsScreenRight = new Dictionary<float, Dictionary<float, Vector2>>();
			PointsScreenMiddle = new Dictionary<float, Dictionary<float, Vector2>>();
			AllocateWalls();
			AllocateFloors();
			AllocateAllPointScreen();
		}

		protected Vector2 ScreenDistance
		{
			get
			{ 
				if (screenDistance == Vector2.Zero) 
				{
					var screenWidth = (float)Constants.SCREEN_WIDTH / 1000;
					var angle = MathHelper.ToRadians((180 - 90 - (Constants.FOV / 2)));
					var adj = screenWidth / 2;
					var oppose = adj * Math.Tan(angle);
					screenDistance = new Vector2(screenWidth, (float)oppose);
				}
				return screenDistance;
			}
		}

		private void AllocateFloors()
		{
			var floorCount = (Constants.SCREEN_HEIGHT / 2);
			Floors = new Floor[Constants.SCREEN_WIDTH + 1][];
			for (int i = 0; i <= Constants.SCREEN_WIDTH; i++) 
			{
				Floors[i] = new Floor[floorCount];
				for (int j = 0; j < floorCount; j++) 
				{
					Floors[i][j] = new Floor();
				}
			}
		}

		private void AllocateWalls()
		{
			Walls = new Wall[Constants.SCREEN_WIDTH + 1];
			for (int i = 0; i <= Constants.SCREEN_WIDTH; i++) 
			{
				Walls[i] = new Wall();
			}
		}

		private void AllocateAllPointScreen()
		{
			var sizeX = Map[0].Length * 10;
			var sizeY = Map.Length * 10;
			for (float i = 0f; i < sizeX; i += 0.1f) 
			{
				for (float j = 0f; j < sizeY; j += 0.1f) 
				{
					i = (float)Math.Round(i, 1);
					j = (float)Math.Round(j, 1);
					if (!PointsScreenLeft.ContainsKey(i)) 
					{
						PointsScreenLeft[i] = new Dictionary<float, Vector2>();
						PointsScreenRight[i] = new Dictionary<float, Vector2>();
						PointsScreenMiddle[i] = new Dictionary<float, Vector2>();
					}
					PointsScreenLeft[i][j] = new Vector2(i - ScreenDistance.X / 2, j - ScreenDistance.Y);
					PointsScreenRight[i][j] = new Vector2(i + ScreenDistance.X / 2, j - ScreenDistance.Y);
					PointsScreenMiddle[i][j] = new Vector2(i, j - ScreenDistance.Y);
				}
			}
		}

		protected void CleanWallArray(int floorCount, Wall wall)
		{
			for (int j = floorCount; j <= wall.YBottom; j++) 
			{
				Floors [wall.X] [j - floorCount].ToDraw = false;
			}
		}
	}
}

