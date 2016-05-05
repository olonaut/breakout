/*

    Simple Breakout Game made in C# using the MonoGame SDK
    Copyright (C) 2016 Marcel Kurz

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Breakout
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private KeyboardState kbstate;
        private GamePadState padstate;
        private Ball ball;

        private Texture2D platform;
        private Vector2 platform_pos;

        private Brick[] bricks;
        private int brickammount;
        private int rows;
        private int totalbricks;
        private Color[] brickColors;

        private int basespeed;

        /* Debugging Information */
        private SpriteFont font;
        private string debug;

        private bool is_gameover;
        private SpriteFont font_gameover;

        private SoundHandler sound;

        /* Save current time for controller rumble handler */
        static DateTime startRumble;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            /* values */
            brickammount = 9; /* per row */
            rows = 5;
            basespeed = 6;
            debug = "DEBUG";
            is_gameover = false;
            totalbricks = rows * brickammount;

            /* Sizes and settings */
            platform_pos = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 64, graphics.GraphicsDevice.Viewport.Height - 16);
            ball = new Ball();
            brickColors = new Color[] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange };

            sound = new SoundHandler();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            /* Create a new SpriteBatch, which can be used to draw textures. */
            spriteBatch = new SpriteBatch(GraphicsDevice);

            /* Load Textures */
            platform = this.Content.Load<Texture2D>("platform_128");
            System.Diagnostics.Debug.WriteLine("platform dimensions: " + platform.Width + "," + platform.Height);

            /* Load Ball Data */
            ball.loadTexture(graphics.GraphicsDevice, Color.Black);

            /* Load Fonts */
            font = Content.Load<SpriteFont>("NewSpriteFont");
            font_gameover = Content.Load<SpriteFont>("font_gameover");

            /* Create Bricks */
            bricks = new Brick[totalbricks];

            for (int _rows = 0; _rows < rows; _rows++)
                for (int i = 0; i < brickammount; i++)
                {
                    bricks[i+(_rows*brickammount)] = new Brick(graphics, new Vector2((85 * i) + (5 * i), (20 * _rows)), new Vector2(85, 15), brickColors[_rows]);
                }
            sound.loadSounds(Content);

        }

        protected override void UnloadContent()
        {
            platform.Dispose();
            ball.texture.Dispose();
            for (int i = 0; i < bricks.Length - 1; i++) bricks[i].Dispose();
            sound.unloadSounds();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            ball.resetHandler();
            
            /* Controlls */
            kbstate = Keyboard.GetState();
            padstate = GamePad.GetState(PlayerIndex.One);

            /* Get Thumbsticks */
            float xfloatright = padstate.ThumbSticks.Right.X * 10;
            int xintright = (int)xfloatright;
            float xfloatleft = padstate.ThumbSticks.Left.X * 10;
            int xintleft = (int)xfloatleft;

            /* Sprinting */
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

            /* Reset */
            if (padstate.Buttons.Start == ButtonState.Pressed || kbstate.IsKeyDown(Keys.Enter))
            {
                ball.isstuck = true;
                ball.yinv = false;
                is_gameover = false;
                basespeed = 6;
            }

            /* Check for unstuck */
            if (ball.isstuck)
                if (kbstate.IsKeyDown(Keys.Space) | padstate.Buttons.A == ButtonState.Pressed) /* Space on keyboard or Button A on the GamePad unstucks ball */
                {
                    ball.isstuck = false;
                    ball.ballangle = 0f; /* Ball will initially move straight upwards. */
                }

            /* Ball Stick thing */
            if (ball.isstuck) /* Ball Position is being constantly updated and set according to the platform position. */
            {
                ball.pos.Y = platform_pos.Y - ball.texture.Height;
                ball.pos.X = platform_pos.X + platform.Width / 2 - ball.texture.Width / 2;
            }
            else
            {
                /* Wall collisions */
                if (ball.pos.X <= 0 && ball.ballangle < 0)
                {
                    ball.doXinv = true;
                    sound.playWallHit();
                }
                else if((ball.pos.X + ball.texture.Width) >= graphics.GraphicsDevice.Viewport.Width && ball.ballangle > 0)
                {
                    ball.doXinv = true;
                    sound.playWallHit();
                }

                /* Ceiling collision */
                if (ball.pos.Y <= 0 && !ball.yinv)
                {
                    ball.doYinv = true;
                    sound.playWallHit();
                }

                /* Platform collision */
                if ((ball.pos.Y + ball.texture.Height) >= platform_pos.Y) /* if ball on same y level as platform */
                {
                    if ((ball.pos.X + (ball.texture.Width / 2)) >= platform_pos.X && (ball.pos.X + (ball.texture.Width / 2)) <= platform_pos.X + platform.Width) //if bottom center of ball on same X level as platform
                    {
                        ball.yinv = false;
                        int ballpos = (((int)(ball.pos.X + (ball.texture.Width / 2)) - (int)platform_pos.X));
                        double impactscore = ballpos * (200 / (float)platform.Width);
                        impactscore -= 100;
                        ball.ballangle = (float)impactscore / 100;

                       
                        if (!is_gameover)
                        {
                            startrumble(); /* Controller Rumble */
                            sound.playPlatformHit(); /* Play Sound Effect */

                        }

                        
                    }
                }

                /* Basic Ball movement */
                /* TODO check for consistent ball speed */
                float xmv, ymv;
                xmv = (ball.ballangle * 2) * basespeed;
                if (ball.ballangle < 0) ymv = (-2 - (ball.ballangle * 2)) * basespeed;
                else ymv = (-2 - (ball.ballangle * -2)) * basespeed;
                ball.pos.X += xmv;
                if (ball.yinv) ball.pos.Y += (ymv * -1);
                else ball.pos.Y += ymv;

                /* Brick collision */
                for (int i = 0; i < bricks.Length; i++)
                {
                    if (bricks[i].active)
                    {
                        if (ball.pos.X < bricks[i].position.X + bricks[i].size.X && ball.pos.X + ball.texture.Width > bricks[i].position.X) // Check for X match
                            if (ball.pos.Y < bricks[i].position.Y + bricks[i].size.Y && ball.pos.Y + ball.texture.Height > bricks[i].position.Y) // Check for Y match
                            {
                                Vector2 relDist;
                                bricks[i].active = false;
                                sound.playBrickHit();
                                if (ball.ballangle == 0) /* Ball move Straight up or down */
                                {
                                    ball.doYinv = true;
                                }
                                else if (ball.ballangle > 0) /* Ball moves left to right */
                                {
                                    if (!ball.yinv) /* Ball moves upwards */ /* TODO Fix */
                                    {
                                        relDist = calcRelDist(new Vector2((ball.pos.X + ball.texture.Width), ball.pos.Y), new Vector2(bricks[i].position.X, (bricks[i].position.Y + bricks[i].size.Y)));
                                    }
                                    else /* Ball moves downwards */
                                    {
                                        relDist = calcRelDist(new Vector2((ball.pos.X + ball.texture.Width), (ball.pos.Y + ball.texture.Height)), new Vector2(bricks[i].position.X, bricks[i].position.Y));
                                    }


                                    if (relDist.X < relDist.Y)
                                    {
                                        ball.doXinv = true;
                                    }
                                    else
                                    {
                                        ball.doYinv = true;
                                    }


                                }

                                else /* Ball moves right to left */
                                {
                                    if (!ball.yinv) /* Ball moves upwards */
                                    {
                                        relDist = calcRelDist(ball.pos, new Vector2((bricks[i].position.X + bricks[i].size.X), (bricks[i].position.Y + bricks[i].size.Y)));
                                    }
                                    else /* Ball moves downwards */
                                    {
                                        relDist = calcRelDist(new Vector2((ball.pos.X), (ball.pos.Y + ball.texture.Width)), new Vector2((bricks[i].position.X + bricks[i].size.X), bricks[i].position.Y));
                                    }

                                    if (relDist.X < relDist.Y)
                                    {
                                        ball.doXinv = true;
                                    }
                                    else
                                    {
                                        ball.doYinv = true;
                                    }

                                }

                            }
                    }
                }


                if (checkGameOver(ball.pos.Y))
                {
                    basespeed = 0;
                    is_gameover = true;
                }

            }

            /* Controller Rumble Check */
            checkrumble();

            /* Execute Ball Movement Changes */
            execBallDirChange(ball.doXinv,ball.doYinv);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(platform, platform_pos);
            spriteBatch.Draw(ball.texture, ball.pos);

            for (int i = 0; i < bricks.Length; i++)
            {
                if (bricks[i].active) spriteBatch.Draw(bricks[i].texture, bricks[i].position);
            }
            if (is_gameover)
            {
                if (padstate.IsConnected)
                {
                    spriteBatch.DrawString(font_gameover, "Press START", new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (font_gameover.MeasureString("Press START").X / 2), (graphics.GraphicsDevice.Viewport.Height / 2) - (font_gameover.MeasureString("Press START").Y / 2)), Color.Red);
                }
                else
                {
                    spriteBatch.DrawString(font_gameover, "Press ENTER", new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - (font_gameover.MeasureString("Press ENTER").X / 2), (graphics.GraphicsDevice.Viewport.Height / 2) - (font_gameover.MeasureString("Press ENTER").Y / 2)), Color.Red);
                }
            }
            else
            {
                spriteBatch.DrawString(font, debug, new Vector2(0, graphics.GraphicsDevice.Viewport.Height - font.MeasureString(debug).Y), Color.Red);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /* Simple check funciton for better code organizing */
        private bool checkGameOver(float pos)
        {
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
                if (timePassed.TotalSeconds >= 0.07)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);

                }
            }
        }


        private void startrumble()
        {
            if (padstate.IsConnected)
            {
                GamePad.SetVibration(PlayerIndex.One, 1f, 0f);
                startRumble = DateTime.Now;
            }
        }

        private Vector2 calcRelDist(Vector2 point, Vector2 pointBrick) /* calculate relative distance between two points (for brick collision) */
        {
            float tmpangle;
            Vector2 dist;
            Vector2 relAngle;

            /* set temporary ball.ballangle */
            if (ball.ballangle < 0) tmpangle = ball.ballangle * -1; /* if ball moves Right to Left */
            else tmpangle = ball.ballangle; /* if ball moves Left to Right */

            /* calculate relative angle */

            relAngle.X = (1 - tmpangle); /* % of movement speed vertical */
            relAngle.Y = tmpangle; /* % of movement speed horizontal */

            
            if(ball.ballangle>0) dist.X = (point.X - pointBrick.X) * relAngle.X;/* left to right */
            else dist.X = (pointBrick.X - point.X) * relAngle.X; /* right to left */

            
            /* TODO Fix it better. */
            if(!ball.yinv) dist.Y = (pointBrick.Y - point.Y) * relAngle.Y; /* down to up */
            else dist.Y = (point.Y - pointBrick.Y) * relAngle.Y; /* up to down */

            debug = "DIST = " + dist;
            System.Diagnostics.Debug.WriteLine(debug);
            return dist;
        }

        private void execBallDirChange(bool doXinv, bool doYinv)
        {
            if (doXinv)
            {
                ball.invertX();
            }

            if (doYinv)
            {
                ball.invertY();
            }
        }

    }
}
