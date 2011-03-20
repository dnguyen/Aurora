using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

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
        private float angle;

        public Enemy(EnemyType eType, Texture2D texture)
        {
            type = eType;
            switch (type)
            {
                case EnemyType.SMALL_ASTEROID:
                    sprite = texture;
                    break;
            }
        }
    }
}
