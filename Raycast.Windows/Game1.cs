﻿using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Raycast.Engine;
using Raycast.Engine.Models;
using System.Threading.Tasks;

namespace Raycast.Windows
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public static int SCREEN_WIDTH = 800;
        public static int SCREEN_HEIGHT = 600;
        public static int TEXTURE_SIZE = 64;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Renderer Renderer;
        private Player player;
        private LevelTest levelTest;
        private IEnumerable<CastedRay> distances;

        private KeyboardState currentKeyboardState;
        private KeyboardState oldKeyboardState;

        private Texture2D skyTexture;
        private Texture2D floorTexture;
        private Texture2D wallTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
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

            Renderer = new Renderer(SCREEN_WIDTH, SCREEN_HEIGHT, 66, levelTest.Map);
        }

        protected override void LoadContent()
        {
            var graphicDevice = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteBatch = new SpriteBatch(graphicDevice);

            skyTexture = Content.Load<Texture2D>("wood");
            floorTexture = Content.Load<Texture2D>("greystone");
            wallTexture = Content.Load<Texture2D>("redbrick");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            TreatInputs();
            player.Update();
            distances = Renderer.RenderWalls(player, levelTest);
        }

        protected override void Draw(GameTime gameTime)
        {

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawWalls();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawWalls()
        {
            foreach (var wall in distances)
            {
                var y = (SCREEN_HEIGHT / wall.Distance);
                var yCenter = y / 2;
                var screenCenteredTop = SCREEN_HEIGHT / 2 - yCenter;
                var screenCenteredBottom = SCREEN_HEIGHT / 2 + yCenter;
                var sourceRect = new Rectangle(wall.TextureOffsetX, 0, 1, TEXTURE_SIZE);
                var destRect = new Rectangle(wall.PixelOnScreen, (int)screenCenteredTop, 1, (int)y);
                spriteBatch.Draw(wallTexture, destRect, sourceRect, Color.White);

                //FLOOR casting
                var wallDistance = wall.Distance;
                var floorLocation = Vector2.Zero;
                floorLocation = new Vector2((float)wall.HitLocation.X, (float)wall.HitLocation.Y);
    
                for (int i = (int)screenCenteredBottom; i <= SCREEN_HEIGHT; i++)
                {
                    var currentDistance = SCREEN_HEIGHT / (2.0 * i - SCREEN_HEIGHT);
                    var weight = currentDistance / wallDistance;

                    var currentFloorX = weight * floorLocation.X + (1.0 - weight) * player.Location.X;
                    var currentFloorY = weight * floorLocation.Y + (1.0 - weight) * player.Location.Y;
                    var floorTexX = (int)(currentFloorX * TEXTURE_SIZE) % TEXTURE_SIZE;
                    var floorTexY = (int)(currentFloorY * TEXTURE_SIZE) % TEXTURE_SIZE;

                    var floorSource = new Rectangle(floorTexX, floorTexY, 1, 1);
                    var floorDest = new Rectangle(wall.PixelOnScreen, i, 1, 1);
                    var ceilingDest = new Rectangle(wall.PixelOnScreen, SCREEN_HEIGHT - i, 1, 1);

                    spriteBatch.Draw(floorTexture, floorDest, floorSource, Color.White);
                    spriteBatch.Draw(skyTexture, ceilingDest, floorSource, Color.White);
                }
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

    }
}
