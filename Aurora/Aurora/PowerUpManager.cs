using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Aurora.Game_Objects;
using Microsoft.Xna.Framework;

namespace Aurora
{
    class PowerUpManager : Manager
    {
        private static Dictionary<string, Texture2D> powerUpTextures;
        private static List<PowerUp> powerUps;

        public static Dictionary<string, Texture2D> PowerUpTextures { get { return powerUpTextures; } }
        public static List<PowerUp> PowerUps { get { return powerUps; } }
        
        public PowerUpManager()
        {
            powerUpTextures = new Dictionary<string, Texture2D>();
            powerUps = new List<PowerUp>();
        }

        public void Update(GameTime gameTime, Player player)
        {
            if (player.Lives > 0)
            {
                for (int i = powerUps.Count - 1; i >= 0; i--)
                {
                    if (!powerUps[i].Collided)
                    {
                        if (powerUps[i].Bounds.Intersects(player.Bounds))
                        {
                            if (!IntersectPixels(powerUps[i].Transformation, powerUps[i].spriteImage.Width, powerUps[i].spriteImage.Height, powerUps[i].TextureData,
                                                    player.Transformation, player.spriteImage.Width, player.spriteImage.Height, player.TextureData))
                            {
                                switch (powerUps[i].Type)
                                {
                                    case PowerUpType.LIFE_UP:
                                        if (player.Lives < 8)
                                            player.Lives++;
                                        break;
                                    case PowerUpType.WEAPON_UPGRADE:
                                        if (player.WeaponLevel < 3)
                                            player.WeaponLevel++;
                                        break;
                                    case PowerUpType.WEAPON_SPEED:
                                        break;
                                }
                                powerUps[i].Collided = true;
                            }
                        }
                        powerUps[i].Update(gameTime);
                    }
                    if (powerUps[i].Collided)
                        powerUps.Remove(powerUps[i]);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (PowerUp pUp in powerUps)
            {
                if (!pUp.Collided)
                    pUp.Draw(spriteBatch);
            }
        }
    }
}
