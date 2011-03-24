﻿using System;
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

        public Projectile(int type_, Texture2D texture, Vector2 pos, Vector2 direct, float ang)
        {
            type = type_;
            sprite = texture;
            position = pos;
            direct.Normalize();
            velocity = direct * 300 * -1;
            angle = ang;
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, angle, Center, 1.0F, SpriteEffects.None, 0f);
        }
    }
}