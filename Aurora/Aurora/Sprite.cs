using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Aurora
{
    class Sprite
    {
        protected Texture2D sprite;
        protected Vector2 position;
        protected Vector2 velocity;
        protected string assetName;

        public Texture2D spriteImage { get { return sprite; } set { sprite = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }

        public virtual void Initialize(string asset)
        {
            assetName = asset;
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += velocity * elapsedTime;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }
    }
}
