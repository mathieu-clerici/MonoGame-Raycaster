using System;
using Microsoft.Xna.Framework;

namespace Raycast.Engine.Models
{
	public class Player
	{
		public static float SCREEN_DISTANCE;

		public Player(Vector2 location)
		{
			Location = location;
		}

		public Player() : this(Vector2.Zero)
		{
			Velocity = Vector2.Zero;
			Acceleration = Vector2.Zero;
		}

		public Vector2 Location { get; set; }
		public Vector2 Velocity { get; set; }
		public Vector2 Acceleration { get; set; }

		public float Angle { get; set; }
		public float AngleAcceleration { get; set; }
	}
}

