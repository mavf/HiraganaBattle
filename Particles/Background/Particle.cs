using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Common;
using Particles.Base;
using System.Runtime.Serialization;

namespace Particles.Background
{
    [DataContract]
    public class Particle
    {
        [DataMember]
        public Vector2 Position;
        [DataMember]
        public Vector2 StartDirection;
        [DataMember]
        public Vector2 EndDirection;
        [DataMember]
        public float LifeLeft;
        [DataMember]
        public float StartingLife;
        [DataMember]
        public float ScaleBegin;
        [DataMember]
        public float ScaleEnd;
        [DataMember]
        public Color StartColor;
        [DataMember]
        public Color EndColor;

        //[DataMember]
        public IEmitter Parent;
        //public BackgroundEmitter Parent;

        [DataMember]
        public float lifePhase;

        public Particle(Vector2 Position, Vector2 StartDirection, Vector2 EndDirection, float StartingLife, float ScaleBegin, float ScaleEnd, Color StartColor, Color EndColor, IEmitter Yourself)
        {
            this.Position = Position;
            this.StartDirection = StartDirection;
            this.EndDirection = EndDirection;
            this.StartingLife = StartingLife;
            this.LifeLeft = StartingLife;
            this.ScaleBegin = ScaleBegin;
            this.ScaleEnd = ScaleEnd;
            this.StartColor = StartColor;
            this.EndColor = EndColor;
            this.Parent = Yourself as BackgroundEmitter;
        }

        public bool Update(float dt)
        {
            //LifeLeft -= dt;
            LifeLeft -= 0.001F;
            if (LifeLeft <= 0 && Position.X > 800)
                return false;
            lifePhase = LifeLeft / StartingLife;  // 1 means newly created 0 means dead.
            Position += MathLib.LinearInterpolate(EndDirection, StartDirection, lifePhase) * dt;
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            float currScale = MathLib.LinearInterpolate(ScaleEnd, ScaleBegin, lifePhase);
            Color currCol = MathLib.LinearInterpolate(EndColor, StartColor, lifePhase);
            spriteBatch.Draw(Parent.ParticleSprite,
                     new Rectangle((int)((Position.X - 0.5f * currScale) * Scale + Offset.X),
                              (int)((Position.Y - 0.5f * currScale) * Scale + Offset.Y),
                              (int)(currScale * Scale),
                              (int)(currScale * Scale)),
                     null, currCol, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

    }

}
