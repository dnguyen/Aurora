using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aurora
{
    class Projectile : Sprite
    {
        private int type;
        private float angle;
        public Projectile(int type_, Texture2D texture, Vector2 pos, Vector2 direct, float ang)
        {
            type = type_;
            sprite = texture;
            position = pos;
            direct.Normalize();
            velocity = direct * 300 * -1;
            angle = ang;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, angle, new Vector2(sprite.Width / 2, sprite.Height / 2), 1.0F, SpriteEffects.None, 0f);
        }
    }
}
