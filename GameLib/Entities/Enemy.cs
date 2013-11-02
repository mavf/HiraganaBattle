using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Characters;
using Common;
using System.Runtime.Serialization;

namespace GameLib.Entities
{
    [DataContract]
    public class Enemy : BaseEntity<SpriteFont>
    {
        private ICharacterProvider characterProvider;
        [DataMember]
        public Vector2 position;

        const float vx = 0;

        [DataMember]
        public float vy = 0.3f;

        [DataMember]
        public Color color;
        //[DataMember]
        //private bool isHit { get; set; }

        Texture2D explosionTexture;

        [DataMember]
        public string text = string.Empty;

        [DataMember]
        public string internalText = null;

        [DataMember]
        public bool IsExploding = false;

        private AnimatedSprite explosion;

        public bool IsExploded
        {
            get
            {
                if (explosion == null)
                    return false;
                return explosion.IsFinished;
            }
        }



        public Enemy(SpriteFont font, int x, int y, string text, ICharacterProvider characterProvider, Texture2D explosionTx)
        {
            position = new Vector2(x, y);
            this.texture = font;
            this.characterProvider = characterProvider;
            this.text = text;
            this.internalText = text;
            explosionTexture = explosionTx;

            ShouldBeRemoved = false;
            InitialLength = text.Length;

            color = MathLib.LinearInterpolate(Color.White, Color.Red, new Random().NextDouble());
        }

        public Enemy(SpriteFont font, Enemy enemy, Texture2D explosionTx)
        {
            this.texture = font;
            this.position = enemy.position;
            this.text = enemy.text;
            this.internalText = enemy.internalText;
            this.ShouldBeRemoved = enemy.ShouldBeRemoved;
            this.InitialLength = enemy.InitialLength;
            this.color = enemy.color;
            ///TODO
            this.characterProvider = new HiraganaCharacterProvider();
            //this.isHit = enemy.isHit;

            explosionTexture = explosionTx;
        }


        public void Explode()
        {
            explosion = new AnimatedSprite(explosionTexture, 5, 5);
            IsExploding = true;
        }

        public override void Update()
        {
            //position.X += vx;
            position.Y += vy;

            if (IsExploding)
                explosion.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsExploding)
                spriteBatch.DrawString(texture, text, position, color);
            else
                explosion.Draw(spriteBatch, position);
        }

        public void Hit()
        {
            if (text.Length > 0)
            {
                var offsetX = texture.MeasureString(text.Substring(0, 1)).X;
                position.X += offsetX;
            }

            text = text.Substring(1);

            if (text.Length == 0)
            {
                ShouldBeRemoved = true;
                //Debug.WriteLine("item should be removed");
            }
        }

        public void Shoot()
        {
            //isHit = true;

            internalText = internalText.Substring(1);
        }

        public string FirstCharacter
        {
            get
            {
                //if (!String.IsNullOrEmpty(text))
                //{
                //    var character = text.ToCharArray().FirstOrDefault().ToString();

                //    return characterProvider.Character(character);
                //}
                //return string.Empty;

                if (!String.IsNullOrEmpty(internalText))
                {
                    var character = internalText.ToCharArray().FirstOrDefault().ToString();

                    return characterProvider.Character(character);
                }
                return string.Empty;
            }
        }

        public string Text
        {
            get
            {
                string result = string.Empty;

                foreach (var item in text.ToCharArray())
                {
                    //result += CharacterHelper.Characters[item.ToString()];
                    result += characterProvider.Character(item.ToString());
                }

                return result;
            }
        }

        public int InitialLength
        {
            get;
            set;
        }

        public int Length
        {
            get
            {
                return text.Length;
            }
        }

        public float Speed
        {
            set
            {
                vy = value;
            }
        }

        public bool ShouldBeRemoved { get; set; }

        public int CenterX
        {
            get
            {
                return (int)(position.X + (texture.MeasureString(text).X / 2));
            }
        }

        public int CenteryY
        {
            get
            {
                return (int)(position.Y + (texture.MeasureString(text).Y / 2));
            }
        }

        public int Height
        {
            get
            {
                return (int)texture.MeasureString(text).Y;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), Convert.ToInt32(texture.MeasureString(text).X), Convert.ToInt32(texture.MeasureString(text).Y));
            }
        }


    }

}
