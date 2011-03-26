using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aurora
{
    public enum ProjectileType
    {
        NORMAL_BULLET,
        DOUBLE_BULLET,
        NORMAL_MISSLE,
        DOUBLE_MISSLE
    }

    class Projectile : Sprite
    {
        private ProjectileType type;
        private int damage;

        public int Damage { get { return damage; } set { damage = value; } }

        public Projectile(ProjectileType type_, Texture2D texture, Vector2 pos, Vector2 direct, float ang)
        {
            type = type_;
            sprite = texture;
            position = pos;
            direct.Normalize();
            velocity = direct * 300 * -1;
            angle = ang;
            damage = 1;
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, angle, Center, 1.0F, SpriteEffects.None, 0f);
        }
    }
}
