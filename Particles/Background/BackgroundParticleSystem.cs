using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace Particles.Background
{
    [DataContractAttribute]
    public class BackgroundParticleSystem
    {
        [DataMember]
        public List<BackgroundEmitter> EmitterList;

        [DataMember]
        public Vector2 Position;

        [DataMember]
        public Random random;

        [DataMember]
        public int minY, maxY;

        public BackgroundParticleSystem(Vector2 Position, int minY, int maxY)
        {
            this.Position = Position;
            random = new Random();
            EmitterList = new List<BackgroundEmitter>();
            this.minY = minY;
            this.maxY = maxY;
        }

        public void Update(float dt)
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Update(dt);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Draw(spriteBatch, Scale, Offset);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < EmitterList.Count; i++)
            {
                if (EmitterList[i].Budget > 0)
                {
                    EmitterList[i].Clear();
                }
            }
        }

        public void AddEmitter(Vector2 SecPerSpawn, Vector2 SpawnDirection, Vector2 SpawnNoiseAngle, Vector2 StartLife, Vector2 StartScale,
                    Vector2 EndScale, Color StartColor1, Color StartColor2, Color EndColor1, Color EndColor2, Vector2 StartSpeed,
                    Vector2 EndSpeed, int Budget, Vector2 RelPosition, Texture2D ParticleSprite)
        {
            BackgroundEmitter emitter = new BackgroundEmitter(SecPerSpawn, SpawnDirection, SpawnNoiseAngle,
                                        StartLife, StartScale, EndScale, StartColor1,
                                        StartColor2, EndColor1, EndColor2, StartSpeed,
                                        EndSpeed, Budget, RelPosition, ParticleSprite, this.random, this);
            EmitterList.Add(emitter);
        }
    }

}
