using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Common;
using System.Runtime.Serialization;

namespace GameLib.Entities
{
    [DataContract]
    public class EndGameScore : BaseEntity<Texture2D>
    {
        Vector2 position;
        SpriteFont font;
        Color startColor = Color.Yellow;
        Color endColor = Color.Black;
        float step = 0.0f;
        float f = 0.03f;
        string text = "tap to continue";
        float centerX;

        public EndGameScore(Vector2 position, SpriteFont font, Texture2D gameOverTexture)
        {
            this.position = position;
            this.font = font;
            base.texture = gameOverTexture;

            this.position.X = position.X - font.MeasureString(text).X / 2;
            centerX = position.X;
        }

        public override void Update()
        {
            if (step >= 1)
            {
                f = -0.03f;  
            }
            if (step <= 0)
                f = 0.03f;

            step += f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color color = MathLib.LinearInterpolate(startColor, endColor, step);

            spriteBatch.DrawString(font, text, position, color);

            spriteBatch.Draw(texture, new Vector2(centerX - texture.Width/2, 150), Color.White);
        }
    }
}
