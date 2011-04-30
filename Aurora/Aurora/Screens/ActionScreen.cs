using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using ProjectMercury;
using Aurora.Bloom;
using Microsoft.Xna.Framework.Input;

namespace Aurora
{
    /* Main game screen */

    class ActionScreen : GameScreen
    {
        public static Background background;
        bool paused;
        bool pauseKeyDown;
        Texture2D PauseOverlay;

        Player player;
        Texture2D crosshair;
        Texture2D livesIcon;
        SpriteFont score;
        EnemyManager enemyManager;
        ParticleManager particleManager;
        PowerUpManager powerUpManager;
        public static Camera cam;
        BloomComponent bloom;
        
        public ActionScreen(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            game.IsMouseVisible = false;
            particleManager = new ParticleManager();
            powerUpManager = new PowerUpManager();
            bloom = new BloomComponent(game);
            bloom.Initialize();
            Components.Add(bloom);
            paused = false;
        }

        public void LoadContent(ContentManager content)
        {
            // Load particles
            particleManager.addEffect("SMALL_EXPLOSION", content.Load<ParticleEffect>("Particle Effects/Explosion-Red"));
            particleManager.addEffect("SMALL_EXPLOSION2", content.Load<ParticleEffect>("Particle Effects/Explosion-Orange"));
            particleManager.addEffect("MEDIUM_EXPLOSION_PINK", content.Load<ParticleEffect>("Particle Effects/Explosion-Medium-Pink"));
            particleManager.addEffect("LARGE_EXPLOSION_BLUE", content.Load<ParticleEffect>("Particle Effects/Large-Explosion-Blue-Remake"));
            particleManager.addEffect("Ship-Trail-Blue", content.Load<ParticleEffect>("Particle Effects/Ship-Trail-Blue"));
            particleManager.addEffect("MissleTrail-Orange", content.Load<ParticleEffect>("Particle Effects/MissleTrail-Orange"));
            particleManager.addEffect("Ricoshet", content.Load<ParticleEffect>("Particle Effects/Ricochet-Yellow"));
            particleManager.addEffect("WarpIn", content.Load<ParticleEffect>("Particle Effects/WarpIn2"));
            particleManager.LoadContent(content);

            score = content.Load<SpriteFont>("menuFont");
            livesIcon = content.Load<Texture2D>("lives_icon");
            background = new Background(content.Load<Texture2D>("grid_background"));

            player = new Player(content.Load<Texture2D>("PlayerShip"));
            crosshair = content.Load<Texture2D>("crosshair");

            // Load enemy textures
            enemyManager = new EnemyManager(player);
            enemyManager.enemyTextures.Add("SMALL_ASTEROID_RED", content.Load<Texture2D>("SMALL_ASTEROID_RED"));
            enemyManager.enemyTextures.Add("SMALL_ASTEROID_BLUE", content.Load<Texture2D>("SMALL_ASTEROID_BLUE"));
            enemyManager.enemyTextures.Add("SMALL_ASTEROID_GREEN", content.Load<Texture2D>("SMALL_ASTEROID_GREEN"));
            enemyManager.enemyTextures.Add("SMALL_ASTEROID_PINK", content.Load<Texture2D>("SMALL_ASTEROID_PINK"));
            enemyManager.enemyTextures.Add("SMALL_ASTEROID_PURPLE", content.Load<Texture2D>("SMALL_ASTEROID_PURPLE"));

            enemyManager.enemyTextures.Add("MEDIUM_ASTEROID_RED", content.Load<Texture2D>("MEDIUM_ASTEROID_RED"));
            enemyManager.enemyTextures.Add("MEDIUM_ASTEROID_BLUE", content.Load<Texture2D>("MEDIUM_ASTEROID_BLUE"));
            enemyManager.enemyTextures.Add("MEDIUM_ASTEROID_GREEN", content.Load<Texture2D>("MEDIUM_ASTEROID_GREEN"));
            enemyManager.enemyTextures.Add("MEDIUM_ASTEROID_PINK", content.Load<Texture2D>("MEDIUM_ASTEROID_PINK"));
            enemyManager.enemyTextures.Add("MEDIUM_ASTEROID_PURPLE", content.Load<Texture2D>("MEDIUM_ASTEROID_PURPLE"));

            enemyManager.enemyTextures.Add("LARGE_ASTEROID_RED", content.Load<Texture2D>("LARGE_ASTEROID_RED"));
            enemyManager.enemyTextures.Add("LARGE_ASTEROID_BLUE", content.Load<Texture2D>("LARGE_ASTEROID_BLUE"));
            enemyManager.enemyTextures.Add("LARGE_ASTEROID_GREEN", content.Load<Texture2D>("LARGE_ASTEROID_GREEN"));
            enemyManager.enemyTextures.Add("LARGE_ASTEROID_PINK", content.Load<Texture2D>("LARGE_ASTEROID_PINK"));
            enemyManager.enemyTextures.Add("LARGE_ASTEROID_PURPLE", content.Load<Texture2D>("LARGE_ASTEROID_PURPLE"));

            enemyManager.enemyTextures.Add("LARGE_SPINNER_BLUE", content.Load<Texture2D>("LARGE_SPINNER_BLUE"));
            enemyManager.enemyTextures.Add("SMALL_SPINNER_PINK", content.Load<Texture2D>("SMALL_SPINNER_Pink"));

            enemyManager.enemyTextures.Add("ALIEN", content.Load<Texture2D>("ALIEN"));
            enemyManager.enemyTextures.Add("ARMORED", content.Load<Texture2D>("ArmoredEnemy"));

            // Load projectile textures
            player.projectileTextures.Add("NORMAL_BULLET", content.Load<Texture2D>("Projectile2"));
            player.projectileTextures.Add("NORMAL_MISSLE", content.Load<Texture2D>("NORMAL_MISSLE"));

            PowerUpManager.PowerUpTextures.Add("LIFE_UP", content.Load<Texture2D>("powerup_lifeup"));
            PowerUpManager.PowerUpTextures.Add("WEAPON_UPGRADE", content.Load<Texture2D>("powerup_weaponPower"));

            // Load sounds
            player.ShootSound = content.Load<SoundEffect>("LaserShoot");

            PauseOverlay = content.Load<Texture2D>("pause_overlay");

            cam = new Camera();
            bloom.Settings = BloomSettings.PresetSettings[6];
            bloom.Visible = true;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState currentKState = Keyboard.GetState();
            bool pauseKeyDownThisFrame = currentKState.IsKeyDown(Keys.Escape);
            if (!pauseKeyDown && pauseKeyDownThisFrame)
            {
                if (paused == false)
                    paused = true;
                else
                    paused = false;
            }
            pauseKeyDown = pauseKeyDownThisFrame;
            if (!paused)
            {
                cam.Pos = player.Position;
                player.Update(gameTime);
                enemyManager.Update(gameTime, player);
                powerUpManager.Update(gameTime, player);
                particleManager.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {

            bloom.BeginDraw();
            Game1.graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.get_transformation(game.GraphicsDevice));

            background.Draw(spriteBatch);
            player.Draw(spriteBatch);

            enemyManager.Draw(spriteBatch);
            powerUpManager.Draw(spriteBatch);
            particleManager.Draw(cam.get_transformation(game.GraphicsDevice));

            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(crosshair, player.MousePosition - new Vector2(17/2, 9), Color.White);
            if (player.Score.ToString().Length > 5)
                spriteBatch.DrawString(score, player.Score.ToString(), new Vector2(Game1.SCREEN_WIDTH - 185, -10), Color.White);
            else
                spriteBatch.DrawString(score, player.Score.ToString(), new Vector2(Game1.SCREEN_WIDTH - 160, -10), Color.White);
            float xMargin = 15;
            for (int i = 0; i < player.Lives; i++)
            {
                spriteBatch.Draw(livesIcon, new Vector2(xMargin, 10), Color.White);
                xMargin += 50;
            }
            if (paused)
            {
                spriteBatch.Draw(PauseOverlay, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(score, "GAME PAUSED", new Vector2((game.GraphicsDevice.Viewport.Width / 2) - (score.MeasureString("GAME PAUSED").X / 2), (game.GraphicsDevice.Viewport.Height / 2) - (score.MeasureString("GAME PAUSED").Y / 2) - 100), Color.White);
                spriteBatch.DrawString(score, "Press ESC to resume", new Vector2((game.GraphicsDevice.Viewport.Width / 2) - (score.MeasureString("GAME PAUSED").X / 2) - 100, (game.GraphicsDevice.Viewport.Height / 2) - (score.MeasureString("GAME PAUSED").Y / 2)), Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
