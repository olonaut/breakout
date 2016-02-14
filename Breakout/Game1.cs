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
        Brick[] bricks;
        bool isstuck; // for determening whether or not the ball is stuck to the platform.
        int brickammount;
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
            isstuck = true;

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
            Color[] balldata = new Color[20 * 20];
            for (int i = 0; i < balldata.Length; i++) balldata[i] = Color.Black;
            ball.SetData(balldata);

            brickammount = 9;


            Color brickcolor = Color.Red;
            System.Diagnostics.Debug.WriteLine("creating " + brickammount + " bricks.");
            bricks = new Brick[brickammount];
            for (int i = 0; i < bricks.Length; i++)
            {
            bricks[i] = new Brick(graphics,(85*i) + (5 * i),0,85,15,brickcolor);
            }


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


            //Controlls
            kbstate = Keyboard.GetState();
            float xfloatright = padstate.ThumbSticks.Right.X * 10;
            int xintright = (int)xfloatright;
            float xfloatleft = padstate.ThumbSticks.Left.X * 10;
            int xintleft = (int)xfloatleft;
            padstate = GamePad.GetState(PlayerIndex.One);

            if (kbstate.IsKeyDown(Keys.LeftShift) | kbstate.IsKeyDown(Keys.RightShift))
            {
                if (platform_pos.X > 0) if (kbstate.IsKeyDown(Keys.A)) platform_pos.X -= 10;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (kbstate.IsKeyDown(Keys.D)) platform_pos.X += 10;
            }
            else {
                if (platform_pos.X > 0) if (kbstate.IsKeyDown(Keys.A)) platform_pos.X -= 5;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (kbstate.IsKeyDown(Keys.D)) platform_pos.X += 5;
            }

            if (padstate.DPad.Left == ButtonState.Pressed | padstate.DPad.Right == ButtonState.Pressed | kbstate.IsKeyDown(Keys.A) | kbstate.IsKeyDown(Keys.D))
            {
                if (platform_pos.X > 0) if (padstate.DPad.Left == ButtonState.Pressed) platform_pos.X -= 10;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (padstate.DPad.Right == ButtonState.Pressed) platform_pos.X += 10;
            }
            else
            {
                if (GamePad.GetCapabilities(PlayerIndex.One).HasRightXThumbStick)
                {
                    if (xintleft == 0)
                    { 
                        if (xintright < 0) if (platform_pos.X > 0) platform_pos.X += xintright;
                        if (xintright > 0) if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) platform_pos.X += xintright;
                    }
                }
                if (GamePad.GetCapabilities(PlayerIndex.One).HasLeftXThumbStick)
                {
                    if (xintright == 0)
                    {
                        if (xintleft < 0) if (platform_pos.X > 0) platform_pos.X += xintleft;
                        if (xintleft > 0) if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) platform_pos.X += xintleft;
                    }
                }
            }
            if (padstate.DPad.Left == ButtonState.Released && padstate.DPad.Right == ButtonState.Released && kbstate.IsKeyUp(Keys.A) && kbstate.IsKeyUp(Keys.D) && xintleft == 0 && xintright == 0)
            
            {
                if (platform_pos.X > 0) if (padstate.Buttons.LeftShoulder == ButtonState.Pressed) platform_pos.X -= 10;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (padstate.Buttons.RightShoulder == ButtonState.Pressed) platform_pos.X += 10;
            }



            //Ball Stick thing
            if (isstuck)
            {
                ball_pos.Y = platform_pos.Y - ball.Height;
                ball_pos.X = platform_pos.X + platform.Width/2 - ball.Width/2;
            }


            //Check for unstuck
            if(kbstate.IsKeyDown(Keys.Space) | padstate.Buttons.A == ButtonState.Pressed) //Space on keyboard or Button A on the GamePad unstucks ball
            {
                isstuck = false;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(platform, platform_pos);
            spriteBatch.Draw(ball, ball_pos);
            for(int i = 0; i < bricks.Length; i++)
            {
                spriteBatch.Draw(bricks[i].texture, bricks[i].position);
 //             System.Diagnostics.Debug.WriteLine("drawing brick " + i + "at pos " + bricks[i].position.ToString());
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
