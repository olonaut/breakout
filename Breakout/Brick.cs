using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Breakout
{
    class Brick
    {
        public bool active;
        public Vector2 position;
        public Vector2 size;
        public Texture2D texture;
        private Color[] texdata;

        public Brick(GraphicsDeviceManager graphics, int x, int y, int width, int height, Color color)
        {
            active = false;
            position = new Vector2(x,y);
            size = new Vector2(width,height);
            texture = new Texture2D(graphics.GraphicsDevice, (int)size.X, (int)size.Y);
            texdata = new Color[width * height];
            for (int i = 0; i < texdata.Length; i++) texdata[i] = color;
            texture.SetData(texdata);
            System.Diagnostics.Debug.WriteLine("x=" + x + " y=" + y + " w=" + width + " h=" + height + " c=" + color.ToString());
        }

    }
}
