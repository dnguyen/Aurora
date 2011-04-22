using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Aurora.Game_Objects
{
    enum PowerUpType
    {
        LIFE_UP,
        WEAPON_UPGRADE,
        WEAPON_SPEED
    }

    class PowerUp : Sprite
    {
        private PowerUpType type;

        public PowerUpType Type { get { return type; } set { type = value; } }

        public PowerUp(PowerUpType type_, Vector2 pos)
        {
            type = type_;
            sprite = PowerUpManager.PowerUpTextures[type.ToString()];
            position = pos;
            base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(sprite, position, null, Color.White, angle, Center, 1.0F, SpriteEffects.None, 0f);

        }
    }
}
