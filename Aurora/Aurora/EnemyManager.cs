using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Aurora
{
    class EnemyManager
    {
        private List<Enemy> enemies;
        public Dictionary<string, Texture2D> enemyTextures = new Dictionary<string, Texture2D>();

        static Random rand = new Random();

        public EnemyManager()
        {
            enemies = new List<Enemy>();
        }

        public void Update(GameTime gameTime, Player player)
        {
            if (player.Lives > 0) // Keep spawning enemies until player runs out of lives
            {
                for (int i = 0; i < 10; i++)
                {
                    Enemy newEnemy = new Enemy(EnemyType.SMALL_ASTEROID, enemyTextures["SMALL_ASTEROID"]);

                    newEnemy.Position = new Vector2(rand.Next(10, Game1.SCREEN_WIDTH), rand.Next(10, Game1.SCREEN_HEIGHT));
                    enemies.Add(newEnemy);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }

        /*
         * Loads all enemy textures
         */
        public void LoadContent(ContentManager content)
        {

        }
    }
}
