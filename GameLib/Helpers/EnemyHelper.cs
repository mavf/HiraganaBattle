using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GameLib.Entities;
using GameLib.Helpers;
using Common;

namespace GameLib.Helpers
{
    public class EnemyHelper
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        private static int lastPositionX, lastPositionY;
        private Texture2D explosion;

        private const float regularSpeed = 0.38f;
        private const float fastSpeed = 0.65f;

        public EnemyHelper(Texture2D explosion)
        {
            this.explosion = explosion;
        }

        public Enemy CreateEnemy(SpriteFont font)
        {
            int positionY = -50;
            int positionX = 0;
            float speed = 0.4f;

            Enemy e = null;

            if (MathLib.Random(10) > 4)
            {
                //positionY = -30;
                //speed = 0.45f;
                speed = regularSpeed;
            }

            if (lastPositionX < 330)
                positionX = MathLib.Random(380, 600);
            else //if (lastPositionX >= 370)
                positionX = MathLib.Random(90, 360);

            if (lastPositionY == -50)
                positionY = -30;

            e = new Enemy(font, positionX, positionY, CharacterHelper.Provider.Words.ToList().SelectRandom(), CharacterHelper.Provider, explosion);
            e.Speed = speed;

            lastPositionY = positionY;
            lastPositionX = positionX;

            return e;
        }

        internal Enemy CreateFastSimpleEnemy(SpriteFont font)
        {
            int positionY = -40;
            int positionX = MathLib.Random(100, 650);
            float speed = fastSpeed;

            Enemy e = null;

            //if (lastPositionX < 300)
            //    positionX = MathLib.Random(380, 600);
            //else if (lastPositionX >= 380)
            //    positionX = MathLib.Random(90, 360);

            if (lastPositionY == -40)
                positionY = -20;

            e = new Enemy(font, positionX, positionY, CharacterHelper.Provider.Words.Where(x=> x.Length == 1).ToList().SelectRandom(), CharacterHelper.Provider, explosion);
            e.Speed = speed;

            lastPositionY = positionY;
            lastPositionX = positionX;

            return e;
        }

        public Enemy CreateEnemy(SpriteFont font, EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Regular:
                    return CreateEnemy(font);
                    break;

                case EnemyType.FastSingle:
                    return CreateFastSimpleEnemy(font);
                    break;

                default:
                    return CreateEnemy(font);
                    break;
            }
        }
    }
}
