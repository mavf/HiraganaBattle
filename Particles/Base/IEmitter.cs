using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Particles.Base
{
    public interface IEmitter
    {
        Texture2D ParticleSprite { get; set; }
    }
}
