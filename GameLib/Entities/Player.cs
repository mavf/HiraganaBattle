using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLib.Entities
{
    public class Player : BaseEntity<Texture2D>
    {
        private Vector2 position;
        private AnimatedSprite explosion;
        private bool isExplode = false;
        public bool IsExploded
        {
            get
            {
                return explosion.IsFinished;
            }
        }

        public Player(Texture2D texture, int x, int y, Texture2D explosion)
        {
            base.texture = texture;
            position = new Vector2(x - (texture.Width / 2), y);
            this.explosion = new AnimatedSprite(explosion, 5, 5);
        }

        public void Explode()
        {
            isExplode = true;
        }

        public override void Update()
        {
            if (isExplode)
                explosion.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(this.texture, new Vector2(centerX - (player.Width / 2), playerYposition), Color.White);
            if (!isExplode)
                spriteBatch.Draw(this.texture, position, Color.White);
            else
                explosion.Draw(spriteBatch, position);
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            }
        }
    }
}
