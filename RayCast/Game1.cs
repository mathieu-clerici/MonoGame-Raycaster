#region File Description
//-----------------------------------------------------------------------------
// RayCastGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

#endregion
namespace RayCast
{
	/// <summary>
	/// Default Project Template
	/// </summary>
	public class Game1 : Game
	{
		#region Fields

		public static int SCREEN_WIDTH = 1024;
		public static int SCREEN_HEIGHT = 768;
		public static int TEXTURE_SIZE = 64;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Player player;
		private LevelTest levelTest;
		private Wall[] distances;
		private KeyboardState currentKeyboardState;
		private KeyboardState oldKeyboardState;

		private Texture2D skyTexture;
		private Texture2D floorTexture;
		private Texture2D wallTexture;

		#endregion

		#region Initialization

		public Game1()
		{
			graphics = new GraphicsDeviceManager (this);	
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
			graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
		}

		/// <summary>
		/// Overridden from the base Game.Initialize. Once the GraphicsDevice is setup,
		/// we'll use the viewport to initialize some values.
		/// </summary>
		protected override void Initialize ()
		{
			base.Initialize();
			currentKeyboardState = new KeyboardState();
			levelTest = new LevelTest();
			player = new Player ();

			for (int i = 0; i < levelTest.Map.Length; ++i)
			{
				for (int j = 0; j < levelTest.Map[i].Length; ++j)
				{
					if (levelTest.Map[j][i] == 'Z')
						player.Location = new Vector2(i, j);
				}
			}
		}

		/// <summary>
		/// Load your graphics content.
		/// </summary>
		protected override void LoadContent ()
		{
			var graphicDevice = graphics.GraphicsDevice;
			spriteBatch = new SpriteBatch(graphicDevice);

			skyTexture = new Texture2D(graphicDevice, 1, 1, false, SurfaceFormat.Color);
			skyTexture.SetData<Color>(new Color[] { Color.LightBlue });

			floorTexture = new Texture2D(graphicDevice, 1, 1, false, SurfaceFormat.Color);
			floorTexture.SetData<Color>(new Color[] { Color.DarkGray });


			wallTexture = Content.Load<Texture2D>("redbrick");
		}

		#endregion

		#region Update and Draw

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			base.Update(gameTime);
			TreatInputs();
			UpdatePlayer();
			distances = Renderer.Instance.RenderWalls(player, levelTest)
				.Where(w => w != null)
				.ToArray();

			//floors = Renderer.Instance.RenderFloor(player, levelTest).ToArray();
		}

		private void TreatInputs()
		{
			oldKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			if (currentKeyboardState.IsKeyDown (Keys.Escape))
				Exit();

			if (currentKeyboardState.IsKeyDown (Keys.Left))
				player.AngleAcceleration = MathHelper.ToRadians(-33f);

			if (currentKeyboardState.IsKeyDown(Keys.Right))
				player.AngleAcceleration = MathHelper.ToRadians(33f);

			if (currentKeyboardState.IsKeyDown(Keys.Up))
				player.Acceleration = new Vector2(0, -0.1f);

			if (currentKeyboardState.IsKeyDown(Keys.Down))
				player.Acceleration = new Vector2(0f, 0.1f);
		}

		private void UpdatePlayer()
		{
			var newDistance = Vector2.Add(player.Acceleration, player.Velocity);
			player.Location = Vector2.Add(player.Location, newDistance);
			player.Angle = player.Angle + player.AngleAcceleration;
			player.Acceleration = Vector2.Zero;
			player.AngleAcceleration = 0f;
		}

		/// <summary>
		/// This is called when the game should draw itself. 
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			DrawSkyBox();
			DrawWalls();
			spriteBatch.End();

			base.Draw (gameTime);
		}

		private void DrawSkyBox()
		{
			spriteBatch.Draw(skyTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT / 2), Color.White);
			spriteBatch.Draw(floorTexture, new Rectangle(0, SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT / 2), Color.White);
		}

		private void DrawWalls()
		{
			var dCount = distances.Count();
			for (int i = 0; i < dCount; i++) 
			{
				var wall = distances[i];
				var y = (SCREEN_HEIGHT / wall.Distance);
				var yCenter = y / 2;
				var screenCenteredTop = SCREEN_HEIGHT / 2 - yCenter;
				//var screenCenteredBottom = SCREEN_HEIGHT / 2 + yCenter;
				var sourceRect = new Rectangle(wall.WallOffset, 0, 1, TEXTURE_SIZE);
				var destRect = new Rectangle (i, (int)screenCenteredTop, 1, (int)y);
				spriteBatch.Draw (wallTexture, destRect, sourceRect, Color.White);
			}
		}
			
		void DrawLine(SpriteBatch batch, Texture2D blank,
			float width, Color color, Vector2 point1, Vector2 point2)
		{
			float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
			float length = Vector2.Distance(point1, point2);

			batch.Draw(blank, point1, null, color,
				angle, Vector2.Zero, new Vector2(length, width),
				SpriteEffects.None, 0);
		}

		#endregion
	}
}
