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
        private int speed;
        public int Damage { get { return damage; } set { damage = value; } }

        public Projectile(ProjectileType type_, Texture2D texture, Vector2 pos, Vector2 direct, float ang)
        {
            type = type_;
            sprite = texture;
            position = pos;
            getStatsForType(type);
            direct.Normalize();
            velocity = direct * speed * -1;
            angle = ang;
            damage = 1;
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (OutOfViewPort()) {
                collided = true;
            }
            if (!collided)
                spriteBatch.Draw(sprite, position, null, Color.White, angle, Center, 1.0F, SpriteEffects.None, 0f);
        }

        void getStatsForType(ProjectileType type_)
        {
            switch (type_)
            {
                case ProjectileType.NORMAL_BULLET:
                    damage = 1;
                    speed = 200;
                    break;
                case ProjectileType.DOUBLE_BULLET:
                    damage = 2;
                    speed = 200;
                    break;
                case ProjectileType.NORMAL_MISSLE:
                    damage = 3;
                    speed = 150;
                    break;
                case ProjectileType.DOUBLE_MISSLE:
                    damage = 4;
                    speed = 150;
                    break;
            }
        }

    }
}
