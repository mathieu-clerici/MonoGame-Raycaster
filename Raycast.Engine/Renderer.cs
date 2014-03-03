using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Raycast.Engine.Extensions;
using Raycast.Engine.Models;
using System.Threading.Tasks;

namespace Raycast.Engine
{
	public class Renderer
	{
        protected float FOV { get; set; }
        protected int Width { get; set; }
        protected int Height { get; set; }
		protected char[][] Map { get; set; }
		protected Caster Caster { get; set; }

		public Renderer(int width, int height, float fov, char[][] map)
        {
            FOV = fov;
            Width = width;
            Height = height;
			Caster = new Caster (map);
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

		public IEnumerable<CastedRay> RenderFloors(Player player, Level level)
		{
			return null;
		}

		public IEnumerable<CastedRay> RenderWalls(Player player, Level level)
		{
			var screenDistance = ScreenDistance;

			var pointScreenLeft = new Vector2(player.Location.X - ScreenDistance.X / 2, 
				(float)Math.Round(player.Location.Y - ScreenDistance.Y, 1));

			var pointScreenRight = new Vector2(player.Location.X + ScreenDistance.X / 2, 
				(float)Math.Round(player.Location.Y - ScreenDistance.Y, 1));

			var pointScreenMiddle = new Vector2(player.Location.X, 
				(float)Math.Round(player.Location.Y - ScreenDistance.Y, 1));
				
			pointScreenLeft = pointScreenLeft.Rotate(MathHelper.ToRadians(player.Angle), player.Location);
			pointScreenRight = pointScreenRight.Rotate(MathHelper.ToRadians(player.Angle), player.Location);
			pointScreenMiddle = pointScreenMiddle.Rotate(MathHelper.ToRadians(player.Angle), player.Location);

			var tasks = new List<Task> ();
			var half1 =  DrawHalfAsyncTask(player, 0, 1, pointScreenMiddle, pointScreenLeft);
			var half2 =  DrawHalfAsyncTask (player, Width, -1, pointScreenMiddle, pointScreenRight);
			tasks.Add (half1);
			tasks.Add (half2);
			Task.WaitAll(tasks.ToArray());
			return half1.Result.Concat(half2.Result);
		}

		private Task<IEnumerable<CastedRay>> DrawHalfAsyncTask(Player player, int currentPixel, 
            int incOrdec, Vector2 middlePoint, Vector2 otherPoint)
		{
			Func< IEnumerable<CastedRay> > temp = () => {
				var castedRays = new List<CastedRay> ();
				var originalDirection = Vector2.Zero;
				var screenVector = Vector2.Subtract (middlePoint, otherPoint).Round (1);
				screenVector.Normalize ();
				screenVector = Vector2.Multiply (screenVector, 0.001f);

				for (int i = 0; i <= Width / 2; i++) {
					var translateOnScreenVector = Vector2.Multiply (screenVector, 1 + i);
					var point = Vector2.Add (otherPoint, translateOnScreenVector);

					var directionVector = Vector2.Subtract (point, player.Location);

					var originalStep = new Vector2 (directionVector.X, directionVector.Y);
					originalStep.Normalize ();

					if (originalDirection == Vector2.Zero) {
						originalDirection = directionVector;
					}

					var angle = otherPoint.AngleForPoints (player.Location, point);
					directionVector.Normalize ();

					var ray = Caster.CastRayForPixel (currentPixel, player.Location,
						Vector2.Multiply (directionVector, 0.1f), Vector2.Zero, angle, 1f);

					ray.Distance = CorrectFishEyeEffect(angle, ray);
					currentPixel += incOrdec;
					castedRays.Add (ray);
				}
				return castedRays;
			};
			return Task.Factory.StartNew<IEnumerable<CastedRay>> (temp);
		}

		protected float CorrectFishEyeEffect(float angle, CastedRay ray)
		{
			angle = angle - FOV / 2;
            var d = CorrectDistance(ray);
			return(float) (Math.Cos(MathHelper.ToRadians(angle)) * d);
		}

        float CorrectDistance(CastedRay ray)
        {
            var d = ray.TranslateVector.Magnitude();
            return d;
        }
	}
}

