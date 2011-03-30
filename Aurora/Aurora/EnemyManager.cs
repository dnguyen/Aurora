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
        private List<Enemy> enemies = new List<Enemy>(); // List of enemy objects
        public Dictionary<string, Texture2D> enemyTextures = new Dictionary<string, Texture2D>(); // List of enemy textures, keys is the enemy type

        private float spawnDelay = 2.0F; // Time between each enemy spawn
        private TimeSpan delayTimer; // Timer for spawn delay
        static Random rand = new Random();

        public EnemyManager()
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

                if (delayTimer.TotalSeconds <= 0)
                {
                    // TODO: Create some kind of "algorithm" to figure out what enemies to spawn. Base it on
                    // points scored? Total time the player has survived?

                    SpawnRandomEnemy(EnemyType.SMALL_ASTEROID);
                    delayTimer = TimeSpan.FromSeconds(spawnDelay);
                }

                // Update all enemies in the enemies list
                for (int i = enemies.Count - 1; i >= 0; i--)
                {
                    // Only update enemies that have no collided yet (Enemies that are not dead)
                    if (!enemies[i].Collided)
                    {
                        enemies[i].Update(gameTime);
                        
                        // Collision detection for enemies and player
                        if (enemies[i].Bounds.Intersects(player.Bounds))
                        {
                            // Per pixel collision detection
                            if (!IntersectPixels(enemies[i].Transformation, enemies[i].spriteImage.Width, enemies[i].spriteImage.Height, enemies[i].TextureData,
                                                player.Transformation, player.spriteImage.Width, player.spriteImage.Height, player.TextureData))
                            {
                                enemies[i].Collided = true;
                                player.Lives -= 1;
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
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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
                    enemy.spriteImage = enemyTextures["MEDIUM_ASTEROID"];
                    break;
                case EnemyType.MEDIUM_ASTEROID:
                    enemy.Type = EnemyType.SMALL_ASTEROID;
                    enemy.spriteImage = enemyTextures["SMALL_ASTEROID"];
                    break;
            }
        }

        private void SpawnRandomEnemy(EnemyType eType)
        {
            Enemy enemy = new Enemy(eType, enemyTextures[eType.ToString()]);
            int direction = rand.Next(0, 3); // Generate a number 0 - 3 [0, 1, 2, 3] to determine what side to spawn on.
            switch (direction)
            {
                case 0: // Top
                    enemy.Position = new Vector2(rand.Next(-50, Game1.SCREEN_WIDTH + 50), rand.Next(-50, 50));
                    break;
                case 1: // Bottom
                    enemy.Position = new Vector2(rand.Next(-50, Game1.SCREEN_WIDTH + 50), rand.Next(Game1.SCREEN_HEIGHT - 50, Game1.SCREEN_HEIGHT + 50));
                    break;
                case 2: // Left
                    enemy.Position = new Vector2(rand.Next(-50, 50), rand.Next(-50, Game1.SCREEN_HEIGHT + 50));
                    break;
                case 3: // Right
                    enemy.Position = new Vector2(rand.Next(Game1.SCREEN_WIDTH + 50, Game1.SCREEN_WIDTH - 50), rand.Next(-50, Game1.SCREEN_HEIGHT + 50));
                    break;
            }
            if (enemy.Position.Y < 50)
            {
                enemy.VY += enemy.Speed;
            }
            if (enemy.Position.X < 50)
            {
                enemy.VX += enemy.Speed;
            }
            if (enemy.Position.X > Game1.SCREEN_WIDTH - 50)
            {
                enemy.VX -= enemy.Speed;
            }
            if (enemy.Position.Y > Game1.SCREEN_HEIGHT - 50)
            {
                enemy.VY -= enemy.Speed;
            }
            enemy.Velocity += new Vector2(rand.Next(-enemy.Speed, enemy.Speed), rand.Next(-enemy.Speed, enemy.Speed));
            enemy.Transformation = Matrix.CreateTranslation(new Vector3(-enemy.Center, 0.0f)) *
                // Matrix.CreateScale(block.Scale) *  would go here
            Matrix.CreateRotationZ(enemy.Angle) *
            Matrix.CreateTranslation(new Vector3(enemy.Position, 0.0f));
            enemies.Add(enemy);
        }

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                           Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}
