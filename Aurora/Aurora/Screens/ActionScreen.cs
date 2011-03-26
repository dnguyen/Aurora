using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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

        public ActionScreen(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            enemyManager = new EnemyManager();
        }

        public void LoadContent(ContentManager content)
        {
            playerLives = content.Load<SpriteFont>("menuFont");
            score = content.Load<SpriteFont>("menuFont");
            player = new Player(content.Load<Texture2D>("PlayerShip"));
            player.LoadContent(content);
            background = content.Load<Texture2D>("testbg");

            // Load enemy textures
            enemyManager.enemyTextures.Add("SMALL_ASTEROID", content.Load<Texture2D>("SMALL_ASTEROID"));
            enemyManager.enemyTextures.Add("MEDIUM_ASTEROID", content.Load<Texture2D>("MEDIUM_ASTEROID"));
            enemyManager.enemyTextures.Add("LARGE_ASTEROID", content.Load<Texture2D>("LARGE_ASTEROID"));

            // Load projectile textures
            player.projectileTextures.Add("NORMAL_BULLET", content.Load<Texture2D>("Projectile1"));
           // player.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            enemyManager.Update(gameTime, player);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //spriteBatch.Begin();
            //spriteBatch.Draw(background, (player.Position / 4) * -1, Color.White);
            //spriteBatch.End();
            spriteBatch.Begin();
            enemyManager.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.DrawString(playerLives, "Lives: " + player.Lives.ToString(), new Vector2(15, 10), Color.White);
            spriteBatch.DrawString(score, "Score: " + player.Score.ToString(), new Vector2(170, 10), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
