using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Breakout
{
    class SoundHandler
    {
        private float volume, pitch, pan;

        private SoundEffect brickHit, platformHit, wallHit;

        public SoundHandler()
        {
            volume = 1.0f;
            pitch = 0.0f;
            pan = 0.0f;
        }

        public void loadSounds(ContentManager Content)
        {
            /* Load Sound Effects */
            wallHit = Content.Load<SoundEffect>("snd/wallHit");
            brickHit = Content.Load<SoundEffect>("snd/brickHit");
            platformHit = Content.Load<SoundEffect>("snd/platformHit");
        }

        public void unloadSounds()
        {
            wallHit.Dispose();
            brickHit.Dispose();
            platformHit.Dispose();
        }

        public void playBrickHit()
        {
            brickHit.Play(volume, pitch, pan);
        }

        public void playWallHit()
        {
            wallHit.Play(volume, pitch, pan);
        }

        public void playPlatformHit()
        {
            platformHit.Play(volume, pitch, pan);
        }

    }
}
