using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Background
{
    public interface IBackground
    {
        void Update(Rectangle screenRectangle);
        void Draw(SpriteBatch spriteBatch);
    }
}
