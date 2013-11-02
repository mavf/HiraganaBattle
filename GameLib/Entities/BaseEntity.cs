using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace GameLib.Entities
{
    [DataContract]
    public abstract class BaseEntity<T> where T : class
    {
        protected T texture;

        abstract public void Update();

        abstract public void Draw(SpriteBatch spriteBatch);

    }

}
