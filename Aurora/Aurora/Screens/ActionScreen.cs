﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using ProjectMercury;

namespace Aurora
{
    /* Main game screen */

    class ActionScreen : GameScreen
    {
        Player player;
        Texture2D background;

        SpriteFont playerLives;
        SpriteFont score;
        EnemyManager enemyManager;
        ParticleManager particleManager;

        public ActionScreen(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            particleManager = new ParticleManager();
        }

        public void LoadContent(ContentManager content)
        {

            // Load particles
            particleManager.addEffect("SMALL_EXPLOSION", content.Load<ParticleEffect>("Explosion-Red"));
            particleManager.addEffect("SMALL_EXPLOSION2", content.Load<ParticleEffect>("Explosion-Orange"));
            particleManager.addEffect("MEDIUM_EXPLOSION_PINK", content.Load<ParticleEffect>("Explosion-Medium-Pink"));
            particleManager.addEffect("LARGE_EXPLOSION_BLUE", content.Load<ParticleEffect>("Explosion-Large-Blue"));
            particleManager.addEffect("Ship-Trail-Blue", content.Load<ParticleEffect>("Ship-Trail-Blue"));
            particleManager.LoadContent(content);

            playerLives = content.Load<SpriteFont>("menuFont");
            score = content.Load<SpriteFont>("menuFont");

            player = new Player(content.Load<Texture2D>("PlayerShip"));
            player.LoadContent(content);

            background = content.Load<Texture2D>("black_background");

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
            player.projectileTextures.Add("NORMAL_BULLET", content.Load<Texture2D>("Projectile1"));

            // Load sounds
            player.ShootSound = content.Load<SoundEffect>("LaserShoot");

           // player.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            enemyManager.Update(gameTime, player);
            particleManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0,0), Color.White);
            spriteBatch.End();
            spriteBatch.Begin();
            enemyManager.Draw(spriteBatch);
            player.Draw(spriteBatch);

            particleManager.Draw();
            spriteBatch.DrawString(playerLives, "Lives: " + player.Lives.ToString(), new Vector2(15, 10), Color.White);
            spriteBatch.DrawString(score, "Score: " + player.Score.ToString(), new Vector2(170, 10), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
