using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Breakout
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private KeyboardState kbstate;
        private GamePadState padstate;
        private Texture2D platform;
        private Texture2D ball;
        private Vector2 platform_pos;
        private Vector2 ball_pos;
        private Brick[] bricks;
        private bool isstuck; // for determening whether or not the ball is stuck to the platform.
        private int brickammount;
        private float ballangle;  // Angle in which the ball is moving. 0 = 90 degree upwards; -1 = 0 Degrees ( completly right ); +1 = 180 degrees.
        private int basespeed;
        private bool yinv;
        private SpriteFont font;
        private string debug;
        private bool is_gameover;
        private SpriteFont font_gameover;
        static DateTime startRumble;

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
            // values
            brickammount = 9; // per row
            basespeed = 6;
            debug = "debug loading";
            isstuck = true;
            yinv = false;
            is_gameover = false;

            //Sizes and settings
            platform_pos = new Vector2(graphics.GraphicsDevice.Viewport.Width/2 - 64, graphics.GraphicsDevice.Viewport.Height - 16);
            ball = new Texture2D(graphics.GraphicsDevice, 20, 20);
            ball_pos = new Vector2(50,50);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load Textures
            platform = this.Content.Load<Texture2D>("platform_128");
            System.Diagnostics.Debug.WriteLine("platform dimensions: " + platform.Width + "," + platform.Height);

            //Load Ball Data
            Color[] balldata = new Color[20 * 20];
            for (int i = 0; i < balldata.Length; i++) balldata[i] = Color.Black;
            ball.SetData(balldata);

            //Load Fonts
            font = Content.Load<SpriteFont>("NewSpriteFont");
            font_gameover = Content.Load<SpriteFont>("font_gameover");

            Color brickcolor = Color.Red;
            System.Diagnostics.Debug.WriteLine("creating " + brickammount + " bricks.");
            bricks = new Brick[brickammount];
            for (int i = 0; i < bricks.Length; i++)
            {
                bricks[i] = new Brick(graphics, new Vector2((85 * i) + (5 * i), 0), new Vector2(85, 15), brickcolor);
            }
        }

        protected override void UnloadContent()
        {
            platform.Dispose();
            ball.Dispose();
            for (int i = 0; i < bricks.Length - 1; i++) bricks[i].Dispose();
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            // Controlls
            kbstate = Keyboard.GetState();
            padstate = GamePad.GetState(PlayerIndex.One);
            // Get Thumbsticks
            float xfloatright = padstate.ThumbSticks.Right.X * 10;
            int xintright = (int)xfloatright;
            float xfloatleft = padstate.ThumbSticks.Left.X * 10;
            int xintleft = (int)xfloatleft;
            //Sprinting
            if (kbstate.IsKeyDown(Keys.LeftShift) | kbstate.IsKeyDown(Keys.RightShift))
            {
                if (platform_pos.X > 0) if (kbstate.IsKeyDown(Keys.A) | kbstate.IsKeyDown(Keys.Left)) platform_pos.X -= 10;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (kbstate.IsKeyDown(Keys.D) | kbstate.IsKeyDown(Keys.Right)) platform_pos.X += 10;
            }
            else {
                if (platform_pos.X > 0) if (kbstate.IsKeyDown(Keys.A) | kbstate.IsKeyDown(Keys.Left)) platform_pos.X -= 5;
                if (platform_pos.X + platform.Width < graphics.GraphicsDevice.Viewport.Width) if (kbstate.IsKeyDown(Keys.D) | kbstate.IsKeyDown(Keys.Right)) platform_pos.X += 5;
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
            
            //Reset
            if(padstate.Buttons.Start == ButtonState.Pressed || kbstate.IsKeyDown(Keys.Enter))
            {
                isstuck = true;
                yinv = false;
                is_gameover = false;
                basespeed = 6;   
            }


            //Check for unstuck
            if(kbstate.IsKeyDown(Keys.Space) | padstate.Buttons.A == ButtonState.Pressed) //Space on keyboard or Button A on the GamePad unstucks ball
            {
                isstuck = false;
                ballangle = 0f; //Ball will initially move straight upwards.
            }

            //Ball Stick thing
            if (isstuck) // Ball Position is being constantly updated and set according to the platform position.
            {
                ball_pos.Y = platform_pos.Y - ball.Height;
                ball_pos.X = platform_pos.X + platform.Width/2 - ball.Width/2;
            }
            else
            {
                //Wall collisions
                if (ball_pos.X <= 0 || (ball_pos.X + ball.Width) >= graphics.GraphicsDevice.Viewport.Width)
                {
                    invertX();
                }
                //Ceiling collision
                if (ball_pos.Y <= 0) yinv = true;
                
                //Platform collision
                if ((ball_pos.Y + ball.Height) >= platform_pos.Y) //if ball on same y level as platform
                {
                    if ((ball_pos.X + (ball.Width / 2)) >= platform_pos.X && (ball_pos.X + (ball.Width / 2)) <= platform_pos.X + platform.Width) //if bottom center of ball on same X level as platform
                    {
                        yinv = false;
                        int ballpos = (((int)(ball_pos.X + (ball.Width / 2)) - (int)platform_pos.X));
                        double impactscore = ballpos * (200 / (float)platform.Width);
                        impactscore -= 100;
                        ballangle = (float)impactscore / 100;
                        debug = "impactscore = " + impactscore + "; angle = " + ballangle + "; ballpos = " + ballpos;

                        //Controller Rumble
                        if(!is_gameover) startrumble();
                    }
                }

                //Basic Ball movement
                float xmv, ymv;
                xmv = (ballangle * 2)*basespeed;
                if (ballangle < 0) ymv = ( -2 - (ballangle * 2)) * basespeed;
                else ymv = ( -2 - (ballangle * -2) ) * basespeed ;
                ball_pos.X += xmv;
                if(yinv) ball_pos.Y += (ymv * -1) ;
                else ball_pos.Y += ymv; 
                
                //Brick(s) collision
                for (int i = 0; i < bricks.Length; i++) {
                if (bricks[i].active) { 
                        if( ball_pos.X < bricks[i].position.X + bricks [i].size.X && ball_pos.X > bricks[i].position.X)
                        if( ball_pos.Y < bricks[i].position.Y + bricks[i].size.Y && ball_pos.Y > bricks[i].position.Y)
                        {
                            System.Diagnostics.Debug.WriteLine("Collision detected with brick " + i);
                            bricks[i].active = false;
                            yinv = true;
                        }
                    }
                }


                if (checkGameOver(ball_pos.Y))
                {
                    basespeed = 0;
                    is_gameover = true;
                } 

            }

            //Controller Rumble Check
            checkrumble();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin();
            spriteBatch.Draw(platform, platform_pos);
            spriteBatch.Draw(ball, ball_pos);

            for (int i = 0; i < bricks.Length; i++)
            {
                if(bricks[i].active) spriteBatch.Draw(bricks[i].texture, bricks[i].position);
            }
            if (is_gameover)
            {
                if (padstate.IsConnected)
                {
                    spriteBatch.DrawString(font_gameover, "Press START", new Vector2((graphics.GraphicsDevice.Viewport.Width/2) - (font_gameover.MeasureString("Press START").X/2), (graphics.GraphicsDevice.Viewport.Height / 2) - (font_gameover.MeasureString("Press START").Y / 2)), Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(font_gameover, "Press ENTER", new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (font_gameover.MeasureString("Press ENTER").X / 2), (graphics.GraphicsDevice.Viewport.Height / 2) - (font_gameover.MeasureString("Press ENTER").Y / 2)), Color.Red);
                }
            }
            else
            {
             spriteBatch.DrawString(font, debug, new Vector2(200, 200), Color.Black);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Simple check funciton for better code organizing
        private bool checkGameOver(float pos) {
            if (pos > graphics.GraphicsDevice.Viewport.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void checkrumble()
        {
            if (padstate.IsConnected)
            {
                TimeSpan timePassed = DateTime.Now - startRumble;
                if(timePassed.TotalSeconds >= 0.07)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

                }
            }
        }


        private void startrumble()
        {
            if ( padstate.IsConnected )
            {
                GamePad.SetVibration(PlayerIndex.One, 1f, 0f);
                startRumble = DateTime.Now;
            }
        }

        private void invertX()
        {
            ballangle *= -1;
        }
    }
}
