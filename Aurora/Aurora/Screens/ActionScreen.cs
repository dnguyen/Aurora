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

namespace Aurora
{
    /* Main game screen */

    class ActionScreen : GameScreen
    {
        Player player;
        public static Background background;

        Texture2D crosshair;

        SpriteFont playerLives;
        SpriteFont score;
        EnemyManager enemyManager;
        ParticleManager particleManager;
        public static Camera cam;
        BloomComponent bloom;

        public ActionScreen(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            
            game.IsMouseVisible = false;
            particleManager = new ParticleManager();
            bloom = new BloomComponent(game);
            bloom.Initialize();
            Components.Add(bloom);
        }

        public void LoadContent(ContentManager content)
        {

            // Load particles
            particleManager.addEffect("SMALL_EXPLOSION", content.Load<ParticleEffect>("Particle Effects/Explosion-Red"));
            particleManager.addEffect("SMALL_EXPLOSION2", content.Load<ParticleEffect>("Particle Effects/Explosion-Orange"));
            particleManager.addEffect("MEDIUM_EXPLOSION_PINK", content.Load<ParticleEffect>("Particle Effects/Explosion-Medium-Pink"));
            particleManager.addEffect("LARGE_EXPLOSION_BLUE", content.Load<ParticleEffect>("Particle Effects/Explosion-Large-Blue"));
            particleManager.addEffect("Ship-Trail-Blue", content.Load<ParticleEffect>("Particle Effects/Ship-Trail-Blue"));
            particleManager.addEffect("MissleTrail-Orange", content.Load<ParticleEffect>("Particle Effects/MissleTrail-Orange"));
            particleManager.LoadContent(content);

            playerLives = content.Load<SpriteFont>("menuFont");
            score = content.Load<SpriteFont>("menuFont");

            background = new Background(content.Load<Texture2D>("background_neon"));

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

            enemyManager.LoadContent(content);

            // Load projectile textures
            player.projectileTextures.Add("NORMAL_BULLET", content.Load<Texture2D>("Projectile2"));
            player.projectileTextures.Add("NORMAL_MISSLE", content.Load<Texture2D>("NORMAL_MISSLE"));

            // Load sounds
            player.ShootSound = content.Load<SoundEffect>("LaserShoot");

            cam = new Camera();
            
            bloom.Settings = BloomSettings.PresetSettings[6];
            bloom.Visible = true;
        }

        public override void Update(GameTime gameTime)
        {
            cam.Pos = player.Position;
            player.Update(gameTime);
            
            enemyManager.Update(gameTime, player);
            particleManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            bloom.BeginDraw();
            Game1.graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.get_transformation(game.GraphicsDevice));

            background.Draw(spriteBatch);
            player.Draw(spriteBatch);

            enemyManager.Draw(spriteBatch);

            particleManager.Draw(cam.get_transformation(game.GraphicsDevice));

            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(crosshair, player.MousePosition - new Vector2(17/2, 9), Color.White);
            spriteBatch.DrawString(playerLives, "Lives: " + player.Lives.ToString(), new Vector2(15, 10), Color.White);
            spriteBatch.DrawString(score, "Score: " + player.Score.ToString(), new Vector2(170, 10), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
