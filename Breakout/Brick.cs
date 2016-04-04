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

        public Brick(GraphicsDeviceManager graphics, Vector2 pos, Vector2 size, Color color)
        {
            active = true;
            position = pos;
            this.size = size;
            texture = new Texture2D(graphics.GraphicsDevice, (int)size.X, (int)size.Y);
            texdata = new Color[(int)size.X * (int)size.Y];
            for (int i = 0; i < texdata.Length; i++) texdata[i] = color;
            texture.SetData(texdata);
            System.Diagnostics.Debug.WriteLine("x=" + pos.X + " y=" + pos.Y + " w=" + size.X + " h=" + size.Y + " c=" + color.ToString());
        }

    }
}
