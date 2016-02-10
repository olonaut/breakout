using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Breakout
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState kbstate;
        GamePadState padstate;
        Texture2D platform;
        Texture2D ball;
        Vector2 platform_pos;
        Vector2 ball_pos;
        
        Texture2D[] bricks; 
        bool isstuck; // for determening whether or not the ball is stuck to the platform.
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            platform_pos = new Vector2(graphics.GraphicsDevice.Viewport.Width/2 - 64, graphics.GraphicsDevice.Viewport.Height - 16);
            ball_pos = new Vector2(50,50);
            ball = new Texture2D(graphics.GraphicsDevice, 20, 20);
            base.Initialize();
            isstuck = false;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            platform = this.Content.Load<Texture2D>("platform_128");
            //Load Ball Data
            Color[] balldata;
            balldata = new Color[20 * 20];
            for (int i = 0; i < balldata.Length; i++) balldata[i] = Color.Black;
            ball.SetData(balldata);
            // ball = this.Content.Load<Texture2D>("ball_64");    //TODO: Create ball texture;


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            platform.Dispose();
            ball.Dispose();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            kbstate = Keyboard.GetState();
            if(kbstate.IsKeyDown(Keys.LeftShift) | kbstate.IsKeyDown(Keys.RightShift))
            {
                if (platform_pos.X > 0) if (kbstate.IsKeyDown(Keys.A)) platform_pos.X -= 10;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (kbstate.IsKeyDown(Keys.D)) platform_pos.X += 10;
            }
            else {
                if (platform_pos.X > 0) if (kbstate.IsKeyDown(Keys.A)) platform_pos.X -= 5;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (kbstate.IsKeyDown(Keys.D)) platform_pos.X += 5;
            }

            padstate = GamePad.GetState(PlayerIndex.One);
            if(padstate.DPad.Left == ButtonState.Pressed | padstate.DPad.Right == ButtonState.Pressed)
            {
                if (platform_pos.X > 0) if (padstate.DPad.Left == ButtonState.Pressed) platform_pos.X -= 10;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (padstate.DPad.Right == ButtonState.Pressed) platform_pos.X += 10;
            }
            else
            {
                if (GamePad.GetCapabilities(PlayerIndex.One).HasRightXThumbStick)
                {
                    float xfloat = padstate.ThumbSticks.Right.X * 10;
                    int xint = (int)xfloat;
                    if (xint < 0) if(platform_pos.X > 0) platform_pos.X += xint;
                    if(xint > 0) if(platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) platform_pos.X += xint;
                }
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            spriteBatch.Draw(platform, platform_pos);
            spriteBatch.Draw(ball, ball_pos);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
