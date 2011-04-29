using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;
using ProjectMercury.Renderers;
using ProjectMercury.Emitters;

namespace Aurora
{
    class EnemyManager : Manager
    {
        private List<Enemy> enemies = new List<Enemy>(); // List of enemy objects
        public Dictionary<string, Texture2D> enemyTextures = new Dictionary<string, Texture2D>(); // List of enemy textures, keys is the enemy type

        private float spawnDelay = 1.2F; // Time between each enemy spawn
        private TimeSpan delayTimer; // Timer for spawn delay

        int smallAsteroidChance = 50;
        int mediumAsteroidChance = 30;
        int largeAsteroidChance = 20;
        int spawnChance = 0;

        public EnemyManager(Player player)
        {
            enemies = new List<Enemy>();
            delayTimer = TimeSpan.FromSeconds(spawnDelay);
        }

        public void Update(GameTime gameTime, Player player)
        {
            float elapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (player.Lives > 0) // Keep spawning enemies until player runs out of lives
            {
                delayTimer = delayTimer.Subtract(gameTime.ElapsedGameTime);

                // Spawn enemies
                if (delayTimer.TotalSeconds <= 0)
                {
                    for (int i = 0; i < rand.Next(1, 2); i++)
                    {
                        spawnChance = rand.Next(100);
                        if (spawnChance <= largeAsteroidChance)
                        {
                            SpawnRandomEnemy(EnemyType.LARGE_ASTEROID);
                        }
                        else if (spawnChance >= mediumAsteroidChance && spawnChance < smallAsteroidChance)
                        {
                            SpawnRandomEnemy(EnemyType.MEDIUM_ASTEROID);
                        }
                        else if (spawnChance >= smallAsteroidChance)
                        {
                            SpawnRandomEnemy(EnemyType.SMALL_ASTEROID);
                        }
                    }
                    if (player.Score > 5000)
                    {
                        if (spawnChance < 10)
                        {
                            SpawnSpinnerGroup(0);
                        }
                        spawnDelay = 1.1F;
                    }
                    if (player.Score > 8000)
                    {
                        if (spawnChance < 15)
                        {
                            for (int i = 0; i < rand.Next(1, 3); i++)
                                SpawnAlien();
                        }

                        spawnDelay = 1F;
                    }
                    if (player.Score > 15000)
                    {
                        spawnDelay = .9F;
                    }

                    delayTimer = TimeSpan.FromSeconds(spawnDelay);
                }
                player.Collided = false;
                // Update all enemies in the enemies list
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    // Only update enemies that have not collided yet (Enemies that are not dead)
                    if (!enemies[i].Collided)
                    {
                        if (enemies[i].Type == EnemyType.SMALL_SPINNER || enemies[i].Type == EnemyType.LARGE_SPINNER)
                        {
                            if (enemies[i].OutOfViewPort(ActionScreen.background))
                            {
                                enemies[i].Collided = true;
                            }
                        }
                        // Collision detection for enemies and player
                        if (enemies[i].Bounds.Intersects(player.Bounds))
                        {
                            // Per pixel collision detection
                            if (!IntersectPixels(enemies[i].Transformation, enemies[i].spriteImage.Width, enemies[i].spriteImage.Height, enemies[i].TextureData,
                                                player.Transformation, player.spriteImage.Width, player.spriteImage.Height, player.TextureData))
                            {
                                enemies[i].Collided = true;
                                player.Collided = true;
                                player.Lives -= 1;
                                if (player.WeaponLevel > 1)
                                    player.WeaponLevel -= 1;
                            }
                        }

                        // Collision detection for enemies and bullets
                        for (int j = player.Bullets.Count - 1; j >= 0; j--)
                        {
                            if (!player.Bullets[j].Collided)
                            {
                                if (enemies[i].Bounds.Intersects(player.Bullets[j].Bounds))
                                {

                                    if (!IntersectPixels(enemies[i].Transformation, enemies[i].spriteImage.Width, enemies[i].spriteImage.Height, enemies[i].TextureData,
                                                        player.Bullets[j].Transformation, player.Bullets[j].spriteImage.Width, player.Bullets[j].spriteImage.Height, player.Bullets[j].TextureData))
                                    {
                                        enemies[i].Health -= player.Bullets[j].Damage;
                                        if (enemies[i].Health <= 0)
                                        {
                                            // Only "downgrade" if enemy is an asteroid
                                            if (enemies[i].Type == EnemyType.SMALL_ASTEROID || enemies[i].Type == EnemyType.MEDIUM_ASTEROID || enemies[i].Type == EnemyType.LARGE_ASTEROID)
                                            {
                                                if (enemies[i].Type == EnemyType.LARGE_ASTEROID || enemies[i].Type == EnemyType.MEDIUM_ASTEROID)
                                                {
                                                    downgradeEnemy(enemies[i]);
                                                    enemies[i].Center = new Vector2(enemies[i].spriteImage.Width / 2, enemies[i].spriteImage.Height / 2);
                                                }
                                                else
                                                {
                                                    enemies[i].Collided = true;
                                                }
                                            }
                                            else
                                            {
                                                enemies[i].Collided = true;
                                            }
                                            player.Score += enemies[i].PointValue;
                                        }
                                        player.Bullets[j].Collided = true;
                                        ParticleManager.particleEffects["Ricoshet"].Trigger(player.Bullets[j].Position);
                                    }
                                }
                                else
                                {
                                    if (player.Bullets[j].OutOfViewPort(ActionScreen.background))
                                    {
                                        player.Bullets[j].Collided = true;
                                        ParticleManager.particleEffects["Ricoshet"].Trigger(player.Bullets[j].Position);
                                    }
                                }
                            }
                            if (player.Bullets[j].Collided)
                            {
                                player.Bullets.Remove(player.Bullets[j]);
                            }
                        }

                        // Collision detection for alien bullets with player
                        if (enemies[i].Type == EnemyType.ALIEN)
                        {
                            for (int j = enemies[i].EBullets.Count - 1; j >= 0; j--)
                            {
                                if (player.Bounds.Intersects(enemies[i].EBullets[j].Bounds))
                                {

                                    if (!IntersectPixels(player.Transformation, player.spriteImage.Width, player.spriteImage.Height, player.TextureData,
                                                        enemies[i].EBullets[j].Transformation, enemies[i].EBullets[j].spriteImage.Width, enemies[i].EBullets[j].spriteImage.Height, enemies[i].EBullets[j].TextureData))
                                    {
                                        enemies[i].EBullets[j].Collided = true;
                                        player.Lives--;
                                    }
                                }
                                if (enemies[i].EBullets[j].Collided)
                                {
                                    enemies[i].EBullets.Remove(enemies[i].EBullets[j]);
                                }
                            }
                        }
                        enemies[i].Update(gameTime, player);
                    }
                    if (enemies[i].Collided)
                    {
                        enemies.Remove(enemies[i]);
                    }
                }
            }
            // Player lost
            else
            {
                player.Collided = false;
            }
            Console.WriteLine("bullet list size: " + player.Bullets.Count);
        }


        public void LoadContent(ContentManager content)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Console.WriteLine("Enemy List Size: " + enemies.Count);
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.Collided)
                    enemy.Draw(spriteBatch);
            }
        }

        private void downgradeEnemy(Enemy enemy)
        {
            enemy.getStatsForType(enemy.Type);
            switch (enemy.Type)
            {
                case EnemyType.LARGE_ASTEROID:
                    enemy.Type = EnemyType.MEDIUM_ASTEROID;
                    enemy.spriteImage = enemyTextures["MEDIUM_ASTEROID_" + enemy.Color_.ToString()];
                    ParticleManager.particleEffects["LARGE_EXPLOSION_BLUE"].Trigger(enemy.Position);
                    break;
                case EnemyType.MEDIUM_ASTEROID:
                    enemy.Type = EnemyType.SMALL_ASTEROID;
                    enemy.spriteImage = enemyTextures["SMALL_ASTEROID_" + enemy.Color_.ToString()];
                    ParticleManager.particleEffects["MEDIUM_EXPLOSION_PINK"].Trigger(enemy.Position);
                    break;
            }
        }

        private void SpawnRandomEnemy(EnemyType eType)
        {
            int color = rand.Next(0, 4);
            EnemyColor eColor = EnemyColor.NONE;
            switch (color)
            {
                case 0:
                    eColor = EnemyColor.RED;
                    break;
                case 1:
                    eColor = EnemyColor.BLUE;
                    break;
                case 2:
                    eColor = EnemyColor.GREEN;
                    break;
                case 3:
                    eColor = EnemyColor.PURPLE;
                    break;
                case 4:
                    eColor = EnemyColor.PINK;
                    break;
            }
            Enemy enemy = new Enemy(eType, enemyTextures[eType.ToString() + "_" + eColor.ToString()], eColor);
            int direction = rand.Next(0, 3); // Generate a number 0 - 3 [0, 1, 2, 3] to determine what side to spawn on.
            switch (direction)
            {
                case 0: // Top
                    enemy.Position = new Vector2(rand.Next(100, ActionScreen.background.Width - 100), 100);
                    break;
                case 1: // Bottom
                    enemy.Position = new Vector2(rand.Next(100, ActionScreen.background.Width - 100), ActionScreen.background.Height - 100);
                    break;
                case 2: // Left
                    enemy.Position = new Vector2(100, rand.Next(100, ActionScreen.background.Height - 100));
                    break;
                case 3: // Right
                    enemy.Position = new Vector2(rand.Next(ActionScreen.background.Width + 50, ActionScreen.background.Width - 50), rand.Next(-50, ActionScreen.background.Height + 50));
                    break;
            }
            enemy.Velocity += new Vector2(rand.Next(-enemy.Speed, enemy.Speed), rand.Next(-enemy.Speed, enemy.Speed));
            enemy.Transformation = Matrix.CreateTranslation(new Vector3(-enemy.Center, 0.0f)) *
                // Matrix.CreateScale(block.Scale) *  would go here
            Matrix.CreateRotationZ(enemy.Angle) *
            Matrix.CreateTranslation(new Vector3(enemy.Position, 0.0f));
            enemies.Add(enemy);
            ParticleManager.particleEffects["WarpIn"].Trigger(enemy.Position);
        }

        private void SpawnSpinnerGroup(int size)
        {
            //List<Enemy> group = new List<Enemy>();
            EnemyColor color_ = EnemyColor.NONE; 
            int direction = rand.Next(0, 3); // Generate a number 0 - 3 [0, 1, 2, 3] to determine what side to spawn on.

            for (int i = 0; i < 15; i++)
            {
                Enemy enemy;
                if (size == 0) // small
                {
                    color_ = EnemyColor.PINK;
                    enemy = new Enemy(EnemyType.SMALL_SPINNER, enemyTextures[EnemyType.SMALL_SPINNER.ToString() + "_" + color_.ToString()], color_);
                }
                else
                {
                    color_ = EnemyColor.BLUE;
                    enemy = new Enemy(EnemyType.LARGE_SPINNER, enemyTextures[EnemyType.SMALL_SPINNER.ToString() + "_" + color_.ToString()], color_);
                } 
                switch (direction)
                {
                    case 0: // Top
                        enemy.Position = new Vector2(rand.Next(50, ActionScreen.background.Width - 50), 50);
                        enemy.VY += enemy.Speed;
                        break;
                    case 1: // Bottom
                        enemy.Position = new Vector2(rand.Next(50, ActionScreen.background.Width - 50), ActionScreen.background.Height - 50);
                        enemy.VY -= enemy.Speed;
                        break;
                    case 2: // Left
                        enemy.Position = new Vector2(100, rand.Next(50, ActionScreen.background.Height - 50));
                        enemy.VX += enemy.Speed;
                        break;
                    case 3: // Right
                        enemy.Position = new Vector2(rand.Next(ActionScreen.background.Width + 50, ActionScreen.background.Width - 50), rand.Next(-50, ActionScreen.background.Height + 50));
                        enemy.VX -= enemy.Speed;
                        break;
                }
                enemies.Add(enemy);
            }
        }

        private void SpawnAlien()
        {
            Enemy alien = new Enemy(EnemyType.ALIEN, enemyTextures["ALIEN"], EnemyColor.NONE);
            alien.Position = new Vector2(rand.Next(100, ActionScreen.background.Width), rand.Next(100, ActionScreen.background.Height));
            enemies.Add(alien);
        }
    }
}
