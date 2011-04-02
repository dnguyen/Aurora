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

        public static Dictionary<string, ParticleEffect> particleEffects = new Dictionary<string, ParticleEffect>();
        public static Renderer particleRenderer;

        public ParticleManager()
        {
            particleEffects = new Dictionary<string, ParticleEffect>(); 
            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = Game1.graphics
            };
        }

        public void LoadContent(ContentManager content)
        {
            foreach (KeyValuePair<string, ParticleEffect> effectPair in particleEffects)
            {
                effectPair.Value.LoadContent(content);
                effectPair.Value.Initialise();
            }
            particleRenderer.LoadContent(content);
        }

        public void addEffect(string key, ParticleEffect effect)
        {
            particleEffects.Add(key, effect);

        }

    }
}
