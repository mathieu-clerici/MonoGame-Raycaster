#region Using Statements
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Raycast.Engine.Models;
using Raycast.Engine;
using System.Threading.Tasks;
#endregion

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

        Task<IEnumerable<CastedRay>> UpdateTask;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Renderer Renderer;
        private Player player;
        private LevelTest levelTest;
        private CastedRay[] distances;

        private KeyboardState currentKeyboardState;
        private KeyboardState oldKeyboardState;

        private Texture2D skyTexture;
        private Texture2D floorTexture;
        private Texture2D wallTexture;

        public Game1()
            : base()
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

            skyTexture = new Texture2D(graphicDevice, 1, 1, false, SurfaceFormat.Color);
            skyTexture.SetData<Color>(new Color[] { Color.LightBlue });

            floorTexture = new Texture2D(graphicDevice, 1, 1, false, SurfaceFormat.Color);
            floorTexture.SetData<Color>(new Color[] { Color.DarkGray });

            wallTexture = Content.Load<Texture2D>("redbrick");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override async void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            TreatInputs();
            player.Update();
            var tmp = await Renderer.RenderWalls(player, levelTest);
            distances = tmp.ToArray();
        }

        private void TreatInputs()
        {
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (currentKeyboardState.IsKeyDown(Keys.Left))
                player.AngleAcceleration = MathHelper.ToRadians(-33f);

            if (currentKeyboardState.IsKeyDown(Keys.Right))
                player.AngleAcceleration = MathHelper.ToRadians(33f);

            if (currentKeyboardState.IsKeyDown(Keys.Up))
                player.Acceleration = new Vector2(0, -0.1f);

            if (currentKeyboardState.IsKeyDown(Keys.Down))
                player.Acceleration = new Vector2(0f, 0.1f);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawSkyBox();
            DrawWalls();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawSkyBox()
        {
            spriteBatch.Draw(skyTexture, 
                new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT / 2), 
                Color.White);

            spriteBatch.Draw(floorTexture, 
                new Rectangle(0, SCREEN_HEIGHT / 2, SCREEN_WIDTH, SCREEN_HEIGHT / 2), 
                Color.White);
        }

        private void DrawWalls()
        {
            foreach (var wall in distances)
            {
                var y = (SCREEN_HEIGHT / wall.Distance);
                var yCenter = y / 2;
                var screenCenteredTop = SCREEN_HEIGHT / 2 - yCenter;
                var sourceRect = new Rectangle(wall.TextureOffsetX, 0, 1, TEXTURE_SIZE);
                var destRect = new Rectangle(wall.PixelOnScreen, (int)screenCenteredTop, 1, (int)y);
                spriteBatch.Draw(wallTexture, destRect, sourceRect, Color.White);
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

    }
}
