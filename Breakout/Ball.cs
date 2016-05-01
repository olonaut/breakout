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

    }
}
