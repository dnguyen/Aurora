using System;
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
        private int power;
        private float fireTime = 0F;

        private Vector2 maxVelocity = new Vector2(1000, 1000);
        private Vector2 mousePosition = Vector2.Zero;
        private Vector2 direction;
        private bool moving;

        public Dictionary<string, Texture2D> projectileTextures = new Dictionary<string, Texture2D>();
        private SoundEffect shootSound;
        private List<Projectile> bullets = new List<Projectile>();

        public int Lives { get { return lives; } set { lives = value; } }
        public int Score { get { return score; } set { score = value; } }
        public int Power { get { return power; } set { power = value; } }
        public float FireDelay { get { return FIRE_DELAY; } set { FIRE_DELAY = value; } }
        public List<Projectile> Bullets { get { return bullets; } set { bullets = value; } }
        public SoundEffect ShootSound { get { return shootSound; } set { shootSound = value; } }
        public Vector2 MousePosition { get { return mousePosition; } set { mousePosition = value; } }

        public Player(Texture2D texture)
        {
            lives = 8;
            score = 0;
            power = 3;
            position = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2, Game1.graphics.GraphicsDevice.Viewport.Height / 2);
            velocity = Vector2.Zero;
            sprite = texture;
            moving = false;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float) gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mouseState = Mouse.GetState();
            mousePosition.X = mouseState.X;
            mousePosition.Y = mouseState.Y;

            KeyboardState currentKState = Keyboard.GetState();

            if (currentKState.IsKeyDown(Keys.Right) || currentKState.IsKeyDown(Keys.D))
            {
                if (velocity.X < maxVelocity.X)
                {
                    velocity.X += BASE_SPEED + ACCELERATION;
                    moving = true;
                }
            }
            if (currentKState.IsKeyDown(Keys.Left) || currentKState.IsKeyDown(Keys.A))
            {
                if (velocity.X < maxVelocity.X)
                {
                    velocity.X -= BASE_SPEED + ACCELERATION;
                    moving = true;
                }
            }
            if (currentKState.IsKeyDown(Keys.Up) || currentKState.IsKeyDown(Keys.W))
            {
                if (velocity.Y < maxVelocity.Y)
                {
                    velocity.Y -= BASE_SPEED + ACCELERATION;
                    moving = true;
                }
            }
            if (currentKState.IsKeyDown(Keys.Down) || currentKState.IsKeyDown(Keys.S))
            {
                if (velocity.Y < maxVelocity.Y)
                {
                    velocity.Y += BASE_SPEED + ACCELERATION;
                    moving = true;
                }
            }

            direction = new Vector2(Game1.SCREEN_WIDTH * 0.5f, Game1.SCREEN_HEIGHT * 0.5f) - mousePosition;
            angle = (float) (Math.Atan2(direction.Y, direction.X));

            fireTime += elapsedTime;
            if (mouseState.LeftButton == ButtonState.Pressed && fireTime > FIRE_DELAY)
            {
                /*for (int i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:
                            //direction += new Vector2(50, 50);
                            break;
                        case 2: 
                            //direction -= new Vector2(50, 50);
                            break;

                    }*/

                    ProjectileType newType = ProjectileType.NORMAL_BULLET;
                    Texture2D newTexture = projectileTextures["NORMAL_BULLET"];
                    switch (power)
                    {
                        case 1: 
                            newType = ProjectileType.NORMAL_BULLET;
                            newTexture = projectileTextures["NORMAL_BULLET"];
                            FIRE_DELAY = 0.1F;
                            break;
                        case 2:
                            newType = ProjectileType.DOUBLE_BULLET;
                            newTexture = projectileTextures["NORMAL_BULLET"];
                            FIRE_DELAY = 0.1F;
                            break;
                        case 3:
                            newType = ProjectileType.NORMAL_MISSLE;
                            newTexture = projectileTextures["NORMAL_MISSLE"];
                            FIRE_DELAY = 0.3F;
                            break;
                    }
                    Projectile bullet = new Projectile(newType, newTexture, position, direction, angle);
                    
                    bullets.Add(bullet);
                //}
                //shootSound.Play();
                fireTime = 0;
            }

            foreach (Projectile bullet in bullets)
            {
                bullet.Update(gameTime);
                bullet.Transformation = Matrix.CreateTranslation(new Vector3(-bullet.Center, 0.0f)) *
                    Matrix.CreateRotationZ(bullet.Angle) *
                    Matrix.CreateTranslation(new Vector3(bullet.Position, 0.0f));
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
