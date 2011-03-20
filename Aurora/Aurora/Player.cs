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

namespace Aurora
{
    class Player : Sprite
    {
        private const float DRAG = 0.9F;
        private const int ACCELERATION = 1;
        private const int BASE_SPEED = 7;
        private const float FIRE_DELAY = 0.1F;

        private int lives;
        private int score;
        private float angle;
        private float fireTime = 0F;

        private Vector2 maxVelocity = new Vector2(1000, 1000);
        private Vector2 mousePosition = Vector2.Zero;
        private Vector2 direction;

        ParticleEffect particleEffect;
        Renderer particleRenderer;

        Texture2D bulletSprite;
        List<Projectile> bullets = new List<Projectile>();

        Rectangle bounds;

        public float Angle { get { return angle; } set { angle = value; } }
        public int Lives { get { return lives; } set { lives = value; } }

        public Player(Texture2D texture, Texture2D bulletTexture)
        {
            lives = 8;
            score = 0;
            position = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Height / 2);
            velocity = Vector2.Zero;
            #region Particles
            particleEffect = new ParticleEffect {
                new Emitter {
                    Budget = 400, // How many particles
                    Term = .5F,
                    Name = "FirstEmitter",
                    BlendMode = EmitterBlendMode.Alpha,
                    ReleaseQuantity = 3,
                    ReleaseScale = 16F,
                    ReleaseRotation = new VariableFloat { Value = 0F, Variation = MathHelper.Pi },
                    ReleaseSpeed = new VariableFloat { Value = 32F, Variation = 16F },
                    ParticleTextureAssetName = "Spikey001",
                    Modifiers = new ModifierCollection {
                        new OpacityModifier {
                            Initial = 1F,
                            Ultimate = 0F
                        },
                        new ColourModifier {
                            InitialColour = Color.SkyBlue.ToVector3(),
                            UltimateColour = Color.AliceBlue.ToVector3()
                        },
                        new RotationModifier {
                            RotationRate = 2.3F
                        }
                    }
                },
            };
            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = Game1.graphics
            };
            particleEffect.Initialise();
            #endregion
            sprite = texture;
            bulletSprite = bulletTexture;
        }

        public override void LoadContent(ContentManager content)
        {
            //base.LoadContent(content);
            particleEffect.LoadContent(content);
            particleRenderer.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouseState = Mouse.GetState();
            mousePosition.X = mouseState.X;
            mousePosition.Y = mouseState.Y;

            KeyboardState currentKState = Keyboard.GetState();

            if (currentKState.IsKeyDown(Keys.Right))
            {
                if (velocity.X < maxVelocity.X)
                    velocity.X += BASE_SPEED + ACCELERATION;
            }
            if (currentKState.IsKeyDown(Keys.Left))
            {
                if (velocity.X < maxVelocity.X)
                    velocity.X -= BASE_SPEED + ACCELERATION;
            }
            if (currentKState.IsKeyDown(Keys.Up))
            {
                if (velocity.Y < maxVelocity.Y)
                    velocity.Y -= BASE_SPEED + ACCELERATION;
            }
            if (currentKState.IsKeyDown(Keys.Down))
            {
                if (velocity.Y < maxVelocity.Y)
                    velocity.Y += BASE_SPEED + ACCELERATION;
            }
            
            direction = position - mousePosition;
            angle = (float) (Math.Atan2(direction.Y, direction.X));

            fireTime += elapsedTime;
            if (mouseState.LeftButton == ButtonState.Pressed && fireTime > FIRE_DELAY)
            {
                Projectile bullet = new Projectile(1, bulletSprite, position, direction, angle);
                
                bullets.Add(bullet);
                fireTime = 0;
            }

            foreach (Projectile bullet in bullets)
            {
                bullet.Update(gameTime);
            }

            
            base.Update(gameTime);

           
            velocity.X -= (velocity.X * DRAG) * elapsedTime;
            velocity.Y -= (velocity.Y * DRAG) * elapsedTime;

            ClampToViewPort();

            if (velocity != Vector2.Zero)
            {
                particleEffect.Trigger(position);
                float deltaseconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                particleEffect.Update(deltaseconds);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, angle, Center, 1.0F, SpriteEffects.None, 0f);
            particleRenderer.RenderEffect(particleEffect);
            foreach (Projectile bullet in bullets)
            {
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

        private void ClampToViewPort()
        {
            if (position.X < sprite.Width / 2)
                position.X = sprite.Width / 2;
            if (position.X > Game1.SCREEN_WIDTH - sprite.Width / 2)
                position.X = Game1.SCREEN_WIDTH - sprite.Width / 2;
            if (position.Y < sprite.Height / 2)
                position.Y = sprite.Height / 2;
            if (position.Y > Game1.SCREEN_HEIGHT - sprite.Height / 2)
                position.Y = Game1.SCREEN_HEIGHT - sprite.Height / 2;
        }
    }
}
