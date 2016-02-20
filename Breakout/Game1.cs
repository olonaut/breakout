using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

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
        float ballangle;  // Angle in which the ball is moving. 0 = 90 degree upwards; -1 = 0 Degrees ( completly right ); +1 = 180 degrees.
        int basespeed;
        bool yinv;
        private SpriteFont font;
        string debug;

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
            basespeed = 6;
            yinv = false;
            debug = "debug loading";
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
            font = Content.Load<SpriteFont>("NewSpriteFont");

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
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            platform.Dispose();
            ball.Dispose();
            for (int i = 0; i < bricks.Length - 1; i++) bricks[i].texture.Dispose();
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
            
            //Check for unstuck
            if(kbstate.IsKeyDown(Keys.Space) | padstate.Buttons.A == ButtonState.Pressed) //Space on keyboard or Button A on the GamePad unstucks ball
            {
                isstuck = false;
                ballangle = 0.25f; //Ball will initially move straight upwards.
            }

            //Ball Stick thing
            if (isstuck) // Ball Position is being constantly updated and set according to the platform position.
            {
                ball_pos.Y = platform_pos.Y - ball.Height;
                ball_pos.X = platform_pos.X + platform.Width/2 - ball.Width/2;
            }
            else
            {
                //Basic Ball movement
                float xmv, ymv;
                xmv = (ballangle * 2)*basespeed;
                if (ballangle < 0) ymv =( -2 - (ballangle * 2)) * basespeed;
                else ymv =( -2 - (ballangle * -2) ) * basespeed ;
                ball_pos.X += xmv;
                if(yinv) { ball_pos.Y += (ymv * -1) ; }
                else { ball_pos.Y += ymv; }
                
                //System.Diagnostics.Debug.WriteLine("ymv " + ymv + "xmv" + xmv);

                //Wall Collisions
                if (ball_pos.X <= 0 || (ball_pos.X + ball.Width) >= graphics.GraphicsDevice.Viewport.Width) ballangle *= -1;

                //TODO Platform Collisions
                
                if ((ball_pos.Y + ball.Height) >= platform_pos.Y) //if ball on same y level as platform
                {
                    if ((ball_pos.X + (ball.Width / 2)) >= platform_pos.X && (ball_pos.X + (ball.Width / 2)) <= platform_pos.X + platform.Width) //if bottom center of ball on same X level as platform
                    {
                        yinv = false;
                        System.Diagnostics.Debug.WriteLine("inverting");
                        int impactscore_single = 200 / platform.Width;
                        int impactscore = ((int)(ball_pos.X + ball.Width) - (int)platform_pos.X)*impactscore_single;
                        if (impactscore < 100) impactscore -= 100;
                        ballangle = (float)impactscore / 100;
                        debug = "impactscore = " + impactscore + "; angle = " + ballangle;

                    }
                }

                //TODO Sideway collisions
                for (int i = 0; i < bricks.Length; i++) {
                    
                if (bricks[i].active) { 
                        if( ball_pos.X < bricks[i].position.X + bricks [i].size.X && ball_pos.X > bricks[i].position.X)
                        if ( ball_pos.Y < bricks[i].position.Y + bricks[i].size.Y && ball_pos.Y > bricks[i].position.Y)
                        {
                            System.Diagnostics.Debug.WriteLine("Collision detected with brick " + i);
                            bricks[i].active = false;
                            yinv = true;
                        }
                    }
                }
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
                if(bricks[i].active) spriteBatch.Draw(bricks[i].texture, bricks[i].position);
 //             System.Diagnostics.Debug.WriteLine("drawing brick " + i + "at pos " + bricks[i].position.ToString());
            }
            spriteBatch.DrawString(font, debug, new Vector2(200,200),Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
