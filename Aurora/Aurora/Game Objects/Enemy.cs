using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectMercury;
using ProjectMercury.Renderers;

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
        private int pointValue;
        private int speed;

        public int Health { get { return health; } set { health = value; } }
        public EnemyType Type { get { return type; } set { type = value; } }
        public int PointValue { get { return pointValue; } set { pointValue = value; } }
        public int Speed { get { return speed; } set { speed = value; } }

        public Enemy(EnemyType eType, Texture2D texture)
        {
            type = eType;
            sprite = texture;
            getStatsForType(type);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (collided || health == 0)
            {
                ParticleManager.particleEffects["SMALL_EXPLOSION"].Trigger(position);
            }
            
            angle += .05F;
            base.Update(gameTime);
            ReflectOffViewport(50);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
           
            spriteBatch.Draw(sprite, position, null, Color.White, angle, Center, 1.0F, SpriteEffects.None, 0f);
           
        }

        public void getStatsForType(EnemyType eType)
        {
            switch (eType)
            {
                case EnemyType.SMALL_ASTEROID:
                    health = 3;
                    pointValue = 10;
                    speed = 100;
                    break;
                case EnemyType.MEDIUM_ASTEROID:
                    health = 5;
                    pointValue = 50;
                    speed = 85;
                    break;
                case EnemyType.LARGE_ASTEROID:
                    health = 10;
                    pointValue = 100;
                    speed = 50;
                    break;
            }
        }
    }
}
