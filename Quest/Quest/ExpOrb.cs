using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public class ExpOrb : GameObject
    {
        float value;
        int spawnEmberTimer;
        public ExpOrb(Vector3 position, Vector3 movement, float value) : base(Type.Particle, "ExpOrb")
        {
            this.position = position;
            this.movement = movement;
            this.value = value;
            usePhysics = true;
            useGravity = true;
            collisionTypesToIgnore.Add(Type.Agent);
            bounceFactor = .15f;
            spawnEmberTimer = Game1.random.Next(400);
        }

        public override void Update()
        {
            if (spawnEmberTimer <= 0)
            {
                spawnEmberTimer = Game1.random.Next(400) + 200;
                Game1.gameObjects.Add(new Particle.Ember(position, new Vector3((float)Game1.random.NextDouble() * 3 - 1.5f, (float)Game1.random.NextDouble() - 0.5f, (float)Game1.random.NextDouble()*.5f), 25, Color.MistyRose, .5f));
            }
            else
            {
                spawnEmberTimer--;
            }

            if (remove)
                return;
            Vector2 target = Game1.agents[0].PositionXY;
            float distance = (target - PositionXY).Length();

            if(distance < 100)
            {
                if(distance < 16)
                {
                    Collect();
                    return;
                } 
                float angle = (target - PositionXY).GetAngle();
                float zDistance = Game1.agents[0].position.Z - position.Z;
                float movementLength = (100 - distance) / 100;
                target = Extensions.GetVector2(angle, movementLength);
                movement += new Vector3(target.X, target.Y, zDistance * movementLength);
            }



            base.Update();
        }

        private void Collect()
        {
            ((Player)Game1.agents[0]).EXP = ((Player)Game1.agents[0]).EXP + value;
            
            Destroy();
        }

        private void Destroy()
        {
            remove = true;
        }

        public override void Draw(DrawBatch drawBatch)
        {
            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, Game1.pixel, new Rectangle((int)PositionXY.X - 3, (int)(position.Y - position.Z) - 3, 6, 6), Color.Purple, DrawBatch.CalculateDepth(PositionXY));
            drawBatch.Draw(DrawBatch.DrawCall.Tag.Light, Game1.fader, new Vector2(PositionXY.X, PositionXY.Y), .2f, 0f, new Vector2(Game1.fader.Width * .5f), Color.Purple, DrawBatch.CalculateDepth(PositionXY));

            base.Draw(drawBatch);
        }
    }
}
