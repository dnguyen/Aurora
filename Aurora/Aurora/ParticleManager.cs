using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMercury;
using ProjectMercury.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Aurora
{
    class ParticleManager
    {
        
        public Dictionary<string, ParticleEffect> particleEffects = new Dictionary<string, ParticleEffect>();
        public Renderer particleRenderer;

        public ParticleManager()
        {
            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = Game1.graphics
            };
        }

        public void LoadContent(ContentManager content)
        {
            foreach (KeyValuePair<string, ParticleEffect> effect in particleEffects)
            {
                effect.Value.Initialise();
                effect.Value.LoadContent(content);
            }
            particleRenderer.LoadContent(content);
        }

        public void Draw(SpriteBatch spriteBatch)
        {            
            foreach (KeyValuePair<string, ParticleEffect> effect in particleEffects)
            {
                particleRenderer.RenderEffect(effect.Value);
            }
        }

    }
}
