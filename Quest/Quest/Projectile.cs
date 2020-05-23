using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Quest
{
    public class Projectile : GameObject
    {
        Collision finalCollision;
        AnimatedSprite sprite;
        int timeAlive = 0;

        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        public static void LoadContent(ContentManager content)
        {
            textures.Add("Fireball", content.Load<Texture2D>("Projectile\\Fireball"));
            textures.Add("BouncingBall", content.Load<Texture2D>("Projectile\\BouncingBall"));
        }

        public Projectile(Vector3 position, Vector3 movement, Agent.Tag ignore, string name) : base(Type.Projectile, name)
        {
            this.position = position;
            this.movement = movement;
            this.collideWithTiles = true;
            this.usePhysics = true;
        }
        public override void GroundCollisionTrigger()
        {
            base.GroundCollisionTrigger();
        }

        public override void CollisionResolve()
        {
            int index = -1;
            for (int i = 0; i < collisions.Count; i++)
            {
                if (collisions[i].type == Type.Agent)
                {
                    index = Game1.GetAgentIndex(collisions[i].ID);
                    {
                        finalCollision = new Collision(Type.Agent, index);
                        TriggerProjectileCollision();
                        break;
                    }
                }
                else
                {
                    index = Game1.GetGameObjectIndex(collisions[i].ID);
                    finalCollision = new Collision(Type.Agent, index);
                    TriggerProjectileCollision();
                    break;
                }
            }

            base.CollisionResolve();
        }

        /// <summary>
        /// The function triggered when collision with object is true
        /// </summary>
        public virtual void TriggerProjectileCollision()
        {
            Game1.damageZones.Add(new DamageZone(new Collider.Circle(PositionXY, 0, 20, false), 10, 1, 1));
            Game1.damageZones[Game1.damageZones.Count - 1].IgnoreTag(Agent.Tag.Player);
            remove = true;
        }

        public override void Update()
        {
            timeAlive++;
            base.Update();
        }

        public override void Draw(DrawBatch drawBatch)
        {
            sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0, -position.Z), DrawBatch.CalculateDepth(PositionXY));
        }

        public class Fireball : Projectile
        {

            public Fireball(Vector3 position, Vector3 movement, Agent.Tag ignore) : base(position, movement, ignore, "Fireball")
            {
                position.Z = 10;
                collider = new Collider.Circle(PositionXY, position.Z, 6f, false);
                agentTagsToIgnore.Add(Agent.Tag.Player);
                sprite = new AnimatedSprite(textures["Fireball"], new Point(64, 32), 16);
                sprite.speed = .3f;
                sprite.repeat = true;
                useGravity = false;
                airResistance = 1f;
                collisionTypesToIgnore.Add(Type.Projectile);
                collisionTypesToIgnore.Add(Type.Particle);
                collisionTypesToIgnore.Add(Type.Null);
                sprite.affectedByLight = 2;
                bounceFactor = 1;
                friction = 1;
            }

            public override void TriggerProjectileCollision()
            {
                Game1.SpawnExplosionEffect(position, .5f);
                base.TriggerProjectileCollision();
            }
            public override void Draw(DrawBatch drawBatch)
            {
                float flack = (float)Math.Cos(timeAlive / 4.3f);
                //sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0), MovementXY.GetAngle(), new Vector2(56, 16), new Vector2(.5f), Color.Lerp(Color.Black, Color.Transparent, .5f), DrawBatch.CalculateDepth(PositionXY));
                sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0, -position.Z), MovementXY.GetAngle(), new Vector2(56, 16), DrawBatch.CalculateDepth(PositionXY));

                drawBatch.Draw(DrawBatch.DrawCall.Tag.Light, Game1.fader, PositionXY + new Vector2(0, -position.Z), Extensions.LerpFloat(.3f, .6f, flack * flack), 0f, new Vector2(Game1.fader.Width / 2, Game1.fader.Height / 2), Color.Lerp(Color.OrangeRed, new Color(240, 220, 0), .1f + flack * .8f), DrawBatch.CalculateDepth(PositionXY) + .0001f);
                drawBatch.Draw(DrawBatch.DrawCall.Tag.Light, Game1.fader, PositionXY + new Vector2(0, -position.Z), .6f, 0f, new Vector2(Game1.fader.Width / 2, Game1.fader.Height / 2), Color.Lerp(Color.LightGoldenrodYellow, Color.Black, .4f), DrawBatch.CalculateDepth(PositionXY) + .0001f);
            }
        }
        public class BouncingBall : Projectile
        {
            int bounceCounter = 0;
            bool nextFrameRed = false;

            public BouncingBall(Vector3 position, Vector3 movement, Agent.Tag ignore) : base(position, movement, ignore, "BouncingBall")
            {
                position.Z = 10;
                collider = new Collider.Circle(PositionXY, position.Z, 8f, false);
                agentTagsToIgnore.Add(Agent.Tag.Player);
                sprite = new AnimatedSprite(textures["BouncingBall"], new Point(32, 32), 0);
                sprite.speed = .3f;
                sprite.repeat = true;
                useGravity = true;
                usePhysics = true;
                airResistance = .99f;
                friction = .8f;
                bounceFactor = .99f;
                collisionTypesToIgnore.Add(Type.Projectile);
                collisionTypesToIgnore.Add(Type.Particle);
                collisionTypesToIgnore.Add(Type.Null);
            }

            public override void GroundCollisionTrigger()
            {
                bounceCounter++;
                nextFrameRed = true;
                if (bounceCounter > 4)
                    TriggerProjectileCollision();
                base.GroundCollisionTrigger();
            }

            public override void TriggerProjectileCollision()
            {
                Game1.SpawnExplosionEffect(position, .5f);
                base.TriggerProjectileCollision();
            }
            public override void Draw(DrawBatch drawBatch)
            {
                sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0), 0f, new Vector2(16, 16), new Vector2(.5f), Color.Lerp(Color.Black, Color.Transparent, .5f), DrawBatch.CalculateDepth(PositionXY));
                if (nextFrameRed)
                {
                    sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0, -position.Z), MovementXY.GetAngle(), new Vector2(16, 16), new Vector2(.5f), Color.Red, DrawBatch.CalculateDepth(PositionXY));
                    nextFrameRed = false;
                }
                else
                    sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0, -position.Z), MovementXY.GetAngle(), new Vector2(16, 16), .5f, DrawBatch.CalculateDepth(PositionXY));
            }
        }
    }
}
