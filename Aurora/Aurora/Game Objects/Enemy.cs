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
        COMET,
        LARGE_SPINNER,
        SMALL_SPINNER
    }

    public enum EnemyColor
    {
        BLUE,
        RED,
        PURPLE,
        PINK,
        GREEN,
        NONE
    }

    class Enemy : Sprite
    {
        private EnemyType type;
        private EnemyColor color;
        private int health;
        private int pointValue;
        private int speed;
        private float rotation;

        public int Health { get { return health; } set { health = value; } }
        public EnemyType Type { get { return type; } set { type = value; } }
        public EnemyColor Color_ { get { return color; } set { color = value; } }
        public int PointValue { get { return pointValue; } set { pointValue = value; } }
        public int Speed { get { return speed; } set { speed = value; } }
        public float Rotation { get { return rotation; } set { rotation = value; } }

        public Enemy(EnemyType eType, Texture2D texture, EnemyColor eColor)
        {
            type = eType;
            color = eColor;
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
            
            angle += rotation;
            base.Update(gameTime);
            if (type == EnemyType.SMALL_SPINNER || type == EnemyType.LARGE_SPINNER)
            {

            }
            else
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
                    rotation = .05F;
                    break;
                case EnemyType.MEDIUM_ASTEROID:
                    health = 5;
                    pointValue = 50;
                    speed = 85;
                    rotation = .05F;
                    break;
                case EnemyType.LARGE_ASTEROID:
                    health = 10;
                    pointValue = 100;
                    speed = 50;
                    rotation = .05F;
                    break;
                case EnemyType.SMALL_SPINNER:
                    health = 3;
                    pointValue = 75;
                    speed = 130;
                    rotation = .07F;
                    break;
                case EnemyType.LARGE_SPINNER:
                    health = 8;
                    pointValue = 110;
                    speed = 110;
                    rotation = .07F;
                    break;
            }
        }
    }
}
