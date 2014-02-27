using System;
using Microsoft.Xna.Framework;

namespace Raycast.Engine.Extensions
{
	public static class Vector2Extensions
	{
		public static Vector2 Rotate(this Vector2 point, float radians, Vector2 pivot)
		{
			float cosRadians = (float)Math.Cos(radians);
			float sinRadians = (float)Math.Sin(radians);

			Vector2 translatedPoint = new Vector2();
			translatedPoint.X = point.X - pivot.X;
			translatedPoint.Y = point.Y - pivot.Y;


			Vector2 rotatedPoint = new Vector2();
			rotatedPoint.X = translatedPoint.X * cosRadians - translatedPoint.Y * sinRadians + pivot.X;
			rotatedPoint.Y = translatedPoint.X * sinRadians + translatedPoint.Y * cosRadians + pivot.Y;

			return rotatedPoint;
		}

		public static Vector2 Rotate(this Vector2 point, float radians)
		{
			float cosRadians = (float)Math.Cos(radians);
			float sinRadians = (float)Math.Sin(radians);

			return new Vector2(
				point.X * cosRadians - point.Y * sinRadians,
				point.X * sinRadians + point.Y * cosRadians);
		}
			
		//Al-Kashi
		public static float AngleForPoints(this Vector2 pointA, Vector2 pointB, Vector2 pointC)
		{
			var ab = Vector2.Subtract(pointA, pointB).Magnitude();
			var bc = Vector2.Subtract(pointB, pointC).Magnitude();
			var ac = Vector2.Subtract(pointA, pointC).Magnitude();

			var	cosB = Math.Pow (ac, 2) - Math.Pow (ab, 2) - Math.Pow (bc, 2);
			cosB = cosB / (2 * ab * bc);
			return (float)(180 - MathHelper.ToDegrees((float)Math.Acos(cosB)));
		}

		public static Vector2 Round(this Vector2 vector, int precision)
		{
			return new Vector2((float)Math.Round(vector.X, precision), 
				(float)Math.Round(vector.Y, precision));
		}

		public static float Magnitude(this Vector2 vector)
		{
			return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
		}
	}
}

