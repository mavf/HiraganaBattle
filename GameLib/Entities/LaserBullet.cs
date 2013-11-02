using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace GameLib.Entities
{
    [DataContract]
    public class LaserBullet : BaseEntity<Texture2D>
    {
        //Texture2D texture;
        public float x, y;

        private float vx = 1.6f;

        public float Angle { get; set; }
        public float Alpha { get; set; }
        public float Bparam { get; set; }
        public float Vs
        {
            set
            {
                vx = value;
            }
        }

        public LaserBullet(Texture2D texture, int x, int y)
        {
            base.texture = texture;
            this.x = x;
            this.y = y;
        }

        public override void Update()
        {
            if (Alpha > 0)
                x -= vx;
            if (Alpha < 0)
                x += vx;

            //y = x * 1.35f - 165;
            y = x * Alpha + Bparam;

            //Debug.WriteLine(String.Format("{0}    {1}", x, y));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, new Vector2(x, y), Color.White);
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width, texture.Height);

            spriteBatch.Draw(texture, new Vector2(x, y), sourceRectangle, Color.White, Angle, origin, 1.0f, SpriteEffects.None, 1);
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)x, (int)y, texture.Width, texture.Height);
            }
        }

    }

}
