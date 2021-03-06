﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;
using ProjectMercury.Renderers;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Aurora
{
    class Player : Sprite
    {
        private const float DRAG = 0.9F;
        private const int ACCELERATION = 1;
        private const int BASE_SPEED = 7;
        private float FIRE_DELAY = 0.1F;

        private int lives;
        private int score;
        private int weaponLevel;
        private float fireTime = 0F;

        private Vector2 mousePosition = Vector2.Zero;
        private Vector2 direction;
        private bool moving;

        public Dictionary<string, Texture2D> projectileTextures = new Dictionary<string, Texture2D>();
        private List<Projectile> bullets = new List<Projectile>();

        public int Lives { get { return lives; } set { lives = value; } }
        public int Score { get { return score; } set { score = value; } }
        public int WeaponLevel { get { return weaponLevel; } set { weaponLevel = value; } }
        public float FireDelay { get { return FIRE_DELAY; } set { FIRE_DELAY = value; } }
        public List<Projectile> Bullets { get { return bullets; } set { bullets = value; } }
        public Vector2 MousePosition { get { return mousePosition; } set { mousePosition = value; } }

        public Player(Texture2D texture)
        {
            lives = 5;
            score = 0;
            weaponLevel = 1;
            position = new Vector2(ActionScreen.background.Width / 2, ActionScreen.background.Height / 2);
            velocity = Vector2.Zero;
            maxVelocity = 500F;
            sprite = texture;
            moving = false;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (lives > 0)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                MouseState mouseState = Mouse.GetState();
                mousePosition.X = mouseState.X;
                mousePosition.Y = mouseState.Y;

                KeyboardState currentKState = Keyboard.GetState();

                if (currentKState.IsKeyDown(Keys.Right) || currentKState.IsKeyDown(Keys.D))
                {
                    if (velocity.X < maxVelocity)
                    {
                        velocity.X += BASE_SPEED + ACCELERATION;
                        moving = true;
                    }
                }
                if (currentKState.IsKeyDown(Keys.Left) || currentKState.IsKeyDown(Keys.A))
                {
                    if (velocity.X < maxVelocity)
                    {
                        velocity.X -= BASE_SPEED + ACCELERATION;
                        moving = true;
                    }
                }
                if (currentKState.IsKeyDown(Keys.Up) || currentKState.IsKeyDown(Keys.W))
                {
                    if (velocity.Y < maxVelocity)
                    {
                        velocity.Y -= BASE_SPEED + ACCELERATION;
                        moving = true;
                    }
                }
                if (currentKState.IsKeyDown(Keys.Down) || currentKState.IsKeyDown(Keys.S))
                {
                    if (velocity.Y < maxVelocity)
                    {
                        velocity.Y += BASE_SPEED + ACCELERATION;
                        moving = true;
                    }
                }

                if (currentKState.IsKeyDown(Keys.OemTilde))
                {
                    score += 5000;
                }

                direction = new Vector2(Game1.SCREEN_WIDTH * 0.5f, Game1.SCREEN_HEIGHT * 0.5f) - mousePosition;
                angle = (float)(Math.Atan2(direction.Y, direction.X));

                fireTime += elapsedTime;
                if (mouseState.LeftButton == ButtonState.Pressed && fireTime > FIRE_DELAY)
                {
                    if (weaponLevel == 1) // Normal bullet
                    {
                        FIRE_DELAY = 0.1F;
                        Projectile bullet = new Projectile(ProjectileType.NORMAL_BULLET, projectileTextures["NORMAL_BULLET"], position, direction, angle);
                        bullets.Add(bullet);
                    }
                    else if (weaponLevel == 2) // Normal bullet upgraded
                    {
                        FIRE_DELAY = 0.1F;
                        Projectile bullet = new Projectile(ProjectileType.NORMAL_BULLET_UPGRADE, projectileTextures["NORMAL_BULLET"], position, direction, angle);
                        bullets.Add(bullet);
                    }
                    else if (weaponLevel == 3) // Double normal bullet
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            direction.Normalize();
                            Vector2 cross = Vector2.Multiply(new Vector2(-direction.Y, direction.X), 8F);
                            Projectile bullet = new Projectile(ProjectileType.NORMAL_BULLET, projectileTextures["NORMAL_BULLET"], position, direction, angle);

                            if (i == 0)
                                bullet.Position += cross;
                            else
                                bullet.Position -= cross;

                            bullets.Add(bullet);
                        }
                        FIRE_DELAY = 0.1F;
                    }
                    //SoundManager.soundEffects["player_weapon"].Play();
                    fireTime = 0;
                }

                foreach (Projectile bullet in bullets)
                {
                    bullet.Update(gameTime);
                }


                base.Update(gameTime);

                velocity.X -= (velocity.X * DRAG) * elapsedTime;
                velocity.Y -= (velocity.Y * DRAG) * elapsedTime;

                if ((velocity.X >= -40 && velocity.X <= 40) && (velocity.Y >= -40 && velocity.Y <= 40))
                    moving = false;

                if (moving)
                    ParticleManager.particleEffects["Ship-Trail-Blue"].Trigger(position);

                transformation = Matrix.CreateTranslation(new Vector3(position, 0.0F));

                ClampToViewPort(ActionScreen.background);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, angle, Center, 1.0F, SpriteEffects.None, 0f);

            foreach (Projectile bullet in bullets)
            {
                if (!bullet.Collided)
                    bullet.Draw(spriteBatch);
            }
        }

        public Vector2 getCenterPosition()
        {
            Vector2 newPos;
            newPos.X = position.X + sprite.Width / 2;
            newPos.Y = position.Y + sprite.Height / 2;

            return newPos;
        }
    }
}
