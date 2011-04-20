using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aurora
{
    class Background
    {
        private Texture2D image;
        private Rectangle bounds;

        public int Height { get { return image.Height; } }
        public int Width { get { return image.Width; } }

        public Background(Texture2D image_)
        {
            image = image_;
            bounds = new Rectangle(0, 0, image.Width, image.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, bounds, Color.White);
        }
    }
}
