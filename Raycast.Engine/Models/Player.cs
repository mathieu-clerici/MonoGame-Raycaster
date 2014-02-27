using System;
using Microsoft.Xna.Framework;
using Raycast.Engine.Extensions;

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

		public void Update()
		{
			Acceleration = Vector2.Add(Acceleration, Velocity);

			if (Acceleration.Y != 0f) {
				var futureLocation = Vector2.Add(Acceleration, Location);
				futureLocation = futureLocation.Rotate(MathHelper.ToRadians(Angle), Location);
				Acceleration = Vector2.Subtract(futureLocation, Location);
			}

			Location = Vector2.Add(Location, Acceleration);
			Angle = Angle + AngleAcceleration;
			Acceleration = Vector2.Zero;
			AngleAcceleration = 0f;
		}
	}
}

