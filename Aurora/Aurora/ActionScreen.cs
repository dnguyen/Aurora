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

        public ActionScreen(Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
        }

        public void LoadContent(ContentManager content)
        {
            player = new Player(content.Load<Texture2D>("PlayerShip"), content.Load<Texture2D>("Projectile1"));
            background = content.Load<Texture2D>("testbg");
           // player.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, (player.Position / 4) * -1, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            player.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
