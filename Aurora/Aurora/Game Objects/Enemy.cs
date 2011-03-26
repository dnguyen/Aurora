using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Aurora
{

    public enum EnemyType
    {
        SMALL_ASTEROID,
        MEDIUM_ASTEROID,
        LARGE_ASTEROID,
        COMET
    }

    class Enemy : Sprite
    {
        private EnemyType type;
        private int health;

        public int Health { get { return health; } set { health = value; } }

        public Enemy(EnemyType eType, Texture2D texture)
        {
            type = eType;
            sprite = texture;
            switch (type)
            {
                case EnemyType.SMALL_ASTEROID:
                    health = 3;
                    break;
                case EnemyType.MEDIUM_ASTEROID:
                    health = 5;
                    break;
                case EnemyType.LARGE_ASTEROID:
                    health = 10;
                    break;
            }
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            angle += .05F;
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, angle, Center, 1.0F, SpriteEffects.None, 0f);
           
        }
    }
}
