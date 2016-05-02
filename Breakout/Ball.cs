using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout
{
    class Ball
    {

        public Texture2D texture;
        public Vector2 pos;
        public bool isstuck;
        public float ballangle;
        public int ballspeed;
        public bool doYinv, doXinv;
        public bool yinv;

        public Ball()
        {
            isstuck = true;
            yinv = false;
            pos = new Vector2(50, 50);
        }

        public void loadTexture(GraphicsDevice gDevice)
        {
            texture = new Texture2D(gDevice,20,20);
            Color[] balldata = new Color[20 * 20];
            for (int i = 0; i < balldata.Length; i++) balldata[i] = Color.Black;
            texture.SetData(balldata);
        }

        public void loadTexture(GraphicsDevice gDevice, Color color)
        {
            texture = new Texture2D(gDevice, 20, 20);
            Color[] balldata = new Color[20 * 20];
            for (int i = 0; i < balldata.Length; i++) balldata[i] = color;
            texture.SetData(balldata);
        }
    }
}
