using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Raycast.Engine;
using Raycast.Engine.Models;
using System.Threading.Tasks;

namespace Raycast.Mac
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		private Renderer Renderer;
		private Player player;
		private LevelTest levelTest;
		private Wall[] distances;

		private KeyboardState currentKeyboardState;
		private KeyboardState oldKeyboardState;

        public Game1()
        {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
			graphics.PreferredBackBufferWidth = Constants.SCREEN_WIDTH;
			graphics.PreferredBackBufferHeight = Constants.SCREEN_HEIGHT;
        }
			
        protected override void Initialize()
        {
			base.Initialize();
			currentKeyboardState = new KeyboardState();
			levelTest = new LevelTest();
			player = new Player();
			for (int i = 0; i < levelTest.Map.Length; ++i)
			{
				for (int j = 0; j < levelTest.Map[i].Length; ++j)
				{
					if (levelTest.Map[j][i] == 'Z') 
					{
						player.Location = new Vector2(i, j);
						levelTest.Map[j][i] = ' ';
					}
				}
			}
			Renderer = new Renderer(levelTest.Map);
        }

        protected override void LoadContent()
        {
			var graphicDevice = graphics.GraphicsDevice;
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Floor.CreateTexture(Content, graphicDevice, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT / 2);
			Wall.CreateTexture(Content, graphicDevice, Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT);
        }

		protected override void Update(GameTime gameTime)
        {
			base.Update(gameTime);
			TreatInputs();
			player.Update();
			distances = Renderer.RenderWalls(player, levelTest);
        }

		private void TreatInputs()
		{
			oldKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();

			if (currentKeyboardState.IsKeyDown(Keys.Escape))
				Exit();

			if (currentKeyboardState.IsKeyDown(Keys.Left))
				player.AngleAcceleration = MathHelper.ToRadians(-30f);

			if (currentKeyboardState.IsKeyDown(Keys.Right))
				player.AngleAcceleration = MathHelper.ToRadians(30f);

			if (currentKeyboardState.IsKeyDown(Keys.Up))
				player.Acceleration = new Vector2(0, -0.1f);

			if (currentKeyboardState.IsKeyDown(Keys.Down))
				player.Acceleration = new Vector2(0f, 0.1f);
		}
			
        protected override void Draw(GameTime gameTime)
        {
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin();
			DrawScene();
			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void DrawScene()
		{
			Wall.Screen.SetData(Wall.wallSurfaceColors);
			Floor.FloorSprite.SetData(Floor.floorSurfaceColors);
			Floor.CeilingSprite.SetData(Floor.ceilingSurfaceColors);


			spriteBatch.Draw(Floor.FloorSprite, new Vector2 (0, Constants.SCREEN_HEIGHT / 2), Color.White);
			spriteBatch.Draw(Floor.CeilingSprite, new Vector2 (0, 0), Color.White);
			spriteBatch.Draw(Wall.Screen, new Vector2 (0, 0), Color.White);
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
    }
}
