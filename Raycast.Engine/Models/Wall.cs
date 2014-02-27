using System;
using Microsoft.Xna.Framework;

namespace Raycast.Engine.Models
{
	public class Wall
	{
		public Color Color { get; set; }
		public float Distance { get; set; }
		public Vector2 Location { get; set; }
		public int WallOffset { get; set; }

		public Wall()
		{
		}
	}
}

