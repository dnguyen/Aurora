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
        protected Vector2 center;
        protected string assetName;
        protected Color[] textureData;
        protected bool collided = false;
        protected float angle;
        protected BoundingSphere bounds;
        protected Matrix transformation;

        #region Properties
        public Texture2D spriteImage { get { return sprite; } set { sprite = value; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public float X { get { return position.X; } set { position.X = value; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }
        public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
        public float VX { get { return velocity.X; } set { velocity.X = value; } }
        public float VY { get { return velocity.Y; } set { velocity.Y = value; } }
        public Color[] TextureData { get { return textureData; } set { textureData = value; } }
        public bool Collided { get { return collided; } set { collided = value; } }
        public float Angle { get { return angle; } set { angle = value; } }
        public BoundingSphere Bounds { get { return bounds; } set { bounds = value; } }
        public Matrix Transformation { get { return transformation; } set { transformation = value; } }
        #endregion

        public Vector2 Center
        {
            get
            {
                return center;
            }
        }

        public virtual void Initialize()
        {
            textureData = new Color[sprite.Width * sprite.Height];
            sprite.GetData(textureData);
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            bounds = new BoundingSphere(new Vector3(Center.X, Center.Y, 0), sprite.Width / 2);
        }

        public virtual void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>(assetName);
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            position += velocity * elapsedTime;
            bounds = new BoundingSphere(new Vector3(position.X + (sprite.Width / 2), position.Y + (sprite.Height / 2), 0), sprite.Width / 2);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }
    }
}
