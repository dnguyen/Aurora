using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ProjectMercury;
using ProjectMercury.Renderers;
using Aurora.Game_Objects;

namespace Aurora
{

    public enum EnemyType
    {
        SMALL_ASTEROID,
        MEDIUM_ASTEROID,
        LARGE_ASTEROID,
        COMET,
        LARGE_SPINNER,
        SMALL_SPINNER,
        ALIEN,
        ARMORED
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

        private List<Projectile> eBullets = new List<Projectile>();
        private float FIRE_DELAY = 0.7F;
        private float fireTime = 0F;

        public int Health { get { return health; } set { health = value; } }
        public EnemyType Type { get { return type; } set { type = value; } }
        public EnemyColor Color_ { get { return color; } set { color = value; } }
        public int PointValue { get { return pointValue; } set { pointValue = value; } }
        public int Speed { get { return speed; } set { speed = value; } }
        public float Rotation { get { return rotation; } set { rotation = value; } }
        public List<Projectile> EBullets { get { return eBullets; } }

        public Enemy(EnemyType eType, Texture2D texture, EnemyColor eColor)
        {
            type = eType;
            color = eColor;
            sprite = texture;
            getStatsForType(type);
            base.Initialize();
        }

        public void Update(GameTime gameTime, Player player)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (collided || health == 0)
            {
                ParticleManager.particleEffects["SMALL_EXPLOSION"].Trigger(position);
                if (type != EnemyType.SMALL_SPINNER)
                    SoundManager.soundEffects["explosion"].Play();
                int randn = Manager.rand.Next(0,100);
                if (randn <= 5 && type != EnemyType.SMALL_SPINNER)
                {
                    int powerUpTypeChance = Manager.rand.Next(0, 10);
                    if (powerUpTypeChance >= 0 && powerUpTypeChance <= 6)
                        PowerUpManager.PowerUps.Add(new PowerUp(PowerUpType.LIFE_UP, position));
                    else if (powerUpTypeChance > 6 && powerUpTypeChance <= 10)
                        PowerUpManager.PowerUps.Add(new PowerUp(PowerUpType.WEAPON_UPGRADE, position));
                    //else
                       // PowerUpManager.PowerUps.Add(new PowerUp(PowerUpType.WEAPON_SPEED, position));
                }
            } 
            if (type == EnemyType.ALIEN) // Follow + Shoot at player
            {
                fireTime += dt;

                float distanceFromPlayer = Vector2.Distance(position, player.Position);

                // Only follow player if the enemy is close enough to the player, else wander around the map
                if (distanceFromPlayer < 200.0F)
                {
                    Vector2 direction = player.Position - position;
                    direction.Normalize();
                    velocity = direction * speed;
                    angle = (float)(Math.Atan2(direction.Y, direction.X));

                    if (fireTime > FIRE_DELAY)
                    {
                        eBullets.Add(new Projectile(ProjectileType.NORMAL_BULLET, player.projectileTextures["NORMAL_BULLET"], position, direction * -1, angle));
                        fireTime = 0;
                    }
                }
                // Wander
                else
                {
                    Vector2 dir = player.Position;
                    Wander(position, ref dir, ref angle, 15.0F);
                    velocity += new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (1.2F);
                }

                foreach (Projectile eBullet in eBullets)
                {
                    eBullet.Update(gameTime);
                }
            }
            // Armored type enemies evade the player when too close
            else if (type == EnemyType.ARMORED)
            {
                float distanceFromPlayer = Vector2.Distance(position, player.Position);
                if (distanceFromPlayer < 200F)
                {
                    Vector2 seekPosition = 2 * position - player.Position;

                    angle = TurnToFace(position, seekPosition,
                        angle, 44.0F);
                    if (velocity.X < maxVelocity && velocity.Y < maxVelocity)
                        velocity += new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 1.2F;
                }
                else
                {
                    Vector2 dir = player.Position;
                    Wander(position, ref dir, ref angle, 15.0F);
                    velocity += new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (1.2F);
                }
            }
            else
            {
                angle += rotation;
            }
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
            foreach (Projectile eBullet in eBullets)
            {
                eBullet.Draw(spriteBatch);
            }
        }

        public void getStatsForType(EnemyType eType)
        {
            switch (eType)
            {
                case EnemyType.SMALL_ASTEROID:
                    health = 3;
                    pointValue = 40;
                    speed = 130;
                    rotation = .05F;
                    break;
                case EnemyType.MEDIUM_ASTEROID:
                    health = 5;
                    pointValue = 100;
                    speed = 100;
                    rotation = .05F;
                    break;
                case EnemyType.LARGE_ASTEROID:
                    health = 10;
                    pointValue = 200;
                    speed = 95;
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
                case EnemyType.ALIEN:
                    health = 10;
                    pointValue = 300;
                    speed = 140;
                    rotation = .07F;
                    break;
                case EnemyType.ARMORED:
                    health = 25;
                    pointValue = 350;
                    speed = 130;
                    rotation = .07F;
                    maxVelocity = 300F;
                    break;
            }
        }

        /// <summary>
        /// Wander contains functionality that is shared between both the mouse and the
        /// tank, and does just what its name implies: makes them wander around the
        /// screen. The specifics of the function are described in more detail in the
        /// accompanying doc.
        /// </summary>
        /// <param name="position">the position of the character that is wandering
        /// </param>
        /// <param name="wanderDirection">the direction that the character is currently
        /// wandering. this parameter is passed by reference because it is an input and
        /// output parameter: Wander accepts it as input, and will update it as well.
        /// </param>
        /// <param name="orientation">the character's orientation. this parameter is
        /// also passed by reference and is an input/output parameter.</param>
        /// <param name="turnSpeed">the character's maximum turning speed.</param>
        private void Wander(Vector2 position, ref Vector2 wanderDirection,
            ref float orientation, float turnSpeed)
        {
            // The wander effect is accomplished by having the character aim in a random
            // direction. Every frame, this random direction is slightly modified.
            // Finally, to keep the characters on the center of the screen, we have them
            // turn to face the screen center. The further they are from the screen
            // center, the more they will aim back towards it.

            // the first step of the wander behavior is to use the random number
            // generator to offset the current wanderDirection by some random amount.
            // .25 is a bit of a magic number, but it controls how erratic the wander
            // behavior is. Larger numbers will make the characters "wobble" more,
            // smaller numbers will make them more stable. we want just enough
            // wobbliness to be interesting without looking odd.
            wanderDirection.X +=
                MathHelper.Lerp(-.25f, .25f, (float)Manager.rand.NextDouble());
            wanderDirection.Y +=
                MathHelper.Lerp(-.25f, .25f, (float)Manager.rand.NextDouble());

            // we'll renormalize the wander direction, ...
            if (wanderDirection != Vector2.Zero)
            {
                wanderDirection.Normalize();
            }
            // ... and then turn to face in the wander direction. We don't turn at the
            // maximum turning speed, but at 15% of it. Again, this is a bit of a magic
            // number: it works well for this sample, but feel free to tweak it.
            orientation = TurnToFace(position, position + wanderDirection, orientation,
                .15f * turnSpeed);


            // next, we'll turn the characters back towards the center of the screen, to
            // prevent them from getting stuck on the edges of the screen.
            Vector2 screenCenter = Vector2.Zero;
            screenCenter.X = Game1.graphics.GraphicsDevice.Viewport.Width / 2;
            screenCenter.Y = Game1.graphics.GraphicsDevice.Viewport.Height / 2;

            // Here we are creating a curve that we can apply to the turnSpeed. This
            // curve will make it so that if we are close to the center of the screen,
            // we won't turn very much. However, the further we are from the screen
            // center, the more we turn. At most, we will turn at 30% of our maximum
            // turn speed. This too is a "magic number" which works well for the sample.
            // Feel free to play around with this one as well: smaller values will make
            // the characters explore further away from the center, but they may get
            // stuck on the walls. Larger numbers will hold the characters to center of
            // the screen. If the number is too large, the characters may end up
            // "orbiting" the center.
            float distanceFromScreenCenter = Vector2.Distance(screenCenter, position);
            float MaxDistanceFromScreenCenter =
                Math.Min(screenCenter.Y, screenCenter.X);

            float normalizedDistance =
                distanceFromScreenCenter / MaxDistanceFromScreenCenter;

            float turnToCenterSpeed = .3f * normalizedDistance * normalizedDistance *
                turnSpeed;

            // once we've calculated how much we want to turn towards the center, we can
            // use the TurnToFace function to actually do the work.
            orientation = TurnToFace(position, screenCenter, orientation,
                turnToCenterSpeed);
        }


        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        private static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            // consider this diagram:
            //         B 
            //        /|
            //      /  |
            //    /    | y
            //  / o    |
            // A--------
            //     x
            // 
            // where A is the position of the object, B is the position of the target,
            // and "o" is the angle that the object should be facing in order to 
            // point at the target. we need to know what o is. using trig, we know that
            //      tan(theta)       = opposite / adjacent
            //      tan(o)           = y / x
            // if we take the arctan of both sides of this equation...
            //      arctan( tan(o) ) = arctan( y / x )
            //      o                = arctan( y / x )
            // so, we can use x and y to find o, our "desiredAngle."
            // x and y are just the differences in position between the two objects.
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            // we'll use the Atan2 function. Atan will calculates the arc tangent of 
            // y / x for us, and has the added benefit that it will use the signs of x
            // and y to determine what cartesian quadrant to put the result in.
            // http://msdn2.microsoft.com/en-us/library/system.math.atan2.aspx
            float desiredAngle = (float)Math.Atan2(y, x);

            // so now we know where we WANT to be facing, and where we ARE facing...
            // if we weren't constrained by turnSpeed, this would be easy: we'd just 
            // return desiredAngle.
            // instead, we have to calculate how much we WANT to turn, and then make
            // sure that's not more than turnSpeed.

            // first, figure out how much we want to turn, using WrapAngle to get our
            // result from -Pi to Pi ( -180 degrees to 180 degrees )
            float difference = WrapAngle(desiredAngle - currentAngle);

            // clamp that between -turnSpeed and turnSpeed.
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            // so, the closest we can get to our target is currentAngle + difference.
            // return that, using WrapAngle again.
            return WrapAngle(currentAngle + difference);
        }

        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// <param name="radians">the angle to wrap, in radians.</param>
        /// <returns>the input value expressed in radians from -Pi to Pi.</returns>
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
    }
}
