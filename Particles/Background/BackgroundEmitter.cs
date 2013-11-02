using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Particles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Common;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Particles.Background
{
    [DataContractAttribute]
    public class BackgroundEmitter : IEmitter
    {
        [DataMember]
        public Vector2 RelPosition;             // Position relative to collection.
        [DataMember]
        public int Budget;                      // Max number of alive particles.
        [DataMember]
        public float NextSpawnIn;                      // This is a random number generated using the SecPerSpawn.
        [DataMember]
        public float SecPassed;                        // Time pased since last spawn.

        public LinkedList<Particle> ActiveParticles;   // A list of all the active particles.

        public Texture2D ParticleSprite { get; set; }        // This is what the particle looks like.
        [DataMember]
        public Random random;                   // Pointer to a random object passed trough constructor.

        [DataMember]
        public Vector2 SecPerSpawn;
        [DataMember]
        public Vector2 SpawnDirection;
        [DataMember]
        public Vector2 SpawnNoiseAngle;
        [DataMember]
        public Vector2 StartLife;
        [DataMember]
        public Vector2 StartScale;
        [DataMember]
        public Vector2 EndScale;
        [DataMember]
        public Color StartColor1;
        [DataMember]
        public Color StartColor2;
        [DataMember]
        public Color EndColor1;

        [DataMember]
        public Color EndColor2;
        [DataMember]
        public Vector2 StartSpeed;
        [DataMember]
        public Vector2 EndSpeed;

        public BackgroundParticleSystem Parent;

        public BackgroundEmitter(Vector2 SecPerSpawn, Vector2 SpawnDirection, Vector2 SpawnNoiseAngle, Vector2 StartLife, Vector2 StartScale,
                    Vector2 EndScale, Color StartColor1, Color StartColor2, Color EndColor1, Color EndColor2, Vector2 StartSpeed,
                    Vector2 EndSpeed, int Budget, Vector2 RelPosition, Texture2D ParticleSprite, Random random, BackgroundParticleSystem parent)
        {
            this.SecPerSpawn = SecPerSpawn;
            this.SpawnDirection = SpawnDirection;
            this.SpawnNoiseAngle = SpawnNoiseAngle;
            this.StartLife = StartLife;
            this.StartScale = StartScale;
            this.EndScale = EndScale;
            this.StartColor1 = StartColor1;
            this.StartColor2 = StartColor2;
            this.EndColor1 = EndColor1;
            this.EndColor2 = EndColor2;
            this.StartSpeed = StartSpeed;
            this.EndSpeed = EndSpeed;
            this.Budget = Budget;
            this.RelPosition = RelPosition;
            this.ParticleSprite = ParticleSprite;
            this.random = random;
            this.Parent = parent;
            ActiveParticles = new LinkedList<Particle>();
            this.NextSpawnIn = MathLib.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
            this.SecPassed = 0.0f;
        }

        public void Update(float dt)
        {
            SecPassed += dt;
            while (SecPassed > NextSpawnIn)
            {
                //Debug.WriteLine(ActiveParticles.Count);
                if (ActiveParticles.Count < Budget)
                {
                    // Spawn a particle
                    Vector2 StartDirection = Vector2.Transform(SpawnDirection, Matrix.CreateRotationZ(MathLib.LinearInterpolate(SpawnNoiseAngle.X, SpawnNoiseAngle.Y, random.NextDouble())));
                    StartDirection.Normalize();
                    Vector2 EndDirection = StartDirection * MathLib.LinearInterpolate(EndSpeed.X, EndSpeed.Y, random.NextDouble());
                    StartDirection *= MathLib.LinearInterpolate(StartSpeed.X, StartSpeed.Y, random.NextDouble());
                    ActiveParticles.AddLast(new Particle(
                        new Vector2(-5, MathLib.Random(Parent.minY, Parent.maxY)),
                        //RelPosition + Parent.Position,
                        StartDirection,
                        EndDirection,
                        MathLib.LinearInterpolate(StartLife.X, StartLife.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(StartScale.X, StartScale.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(EndScale.X, EndScale.Y, random.NextDouble()),
                        MathLib.LinearInterpolate(StartColor1, StartColor2, random.NextDouble()),
                        MathLib.LinearInterpolate(EndColor1, EndColor2, random.NextDouble()),
                        this)
                    );
                    ActiveParticles.Last.Value.Update(SecPassed);
                }
                SecPassed -= NextSpawnIn;
                NextSpawnIn = MathLib.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
            }

            LinkedListNode<Particle> node = ActiveParticles.First;
            while (node != null)
            {
                bool isAlive = node.Value.Update(dt);
                node = node.Next;
                if (!isAlive)
                {
                    if (node == null)
                    {
                        ActiveParticles.RemoveLast();
                    }
                    else
                    {
                        ActiveParticles.Remove(node.Previous);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            LinkedListNode<Particle> node = ActiveParticles.First;
            while (node != null)
            {
                node.Value.Draw(spriteBatch, Scale, Offset);
                node = node.Next;
            }
        }

        public void Clear()
        {
            ActiveParticles.Clear();
        }


    }

}