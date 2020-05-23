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
    public abstract class Particle : GameObject
    {
        AnimatedSprite sprite;
        Texture2D texture;
        int timeAlive, timeToLive;
        Behaviour behaviour;
        float scale, rotation;

        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public Particle(Vector3 position, Vector3 movement, int timeToLive) : base(Type.Particle, "Particle")
        {
            this.position = position;
            this.movement = movement;
            this.timeToLive = timeToLive;
            timeAlive = 0;
        }

        public static void LoadContent(ContentManager content)
        {
            textures.Add("Smoke", content.Load<Texture2D>("Particles\\Smoke"));
        }

        public override void Update()
        {
            if (timeAlive >= timeToLive)
                remove = true;
            else
            {
                timeAlive++;
                if (behaviour != null)
                    behaviour.Update(ref position, ref movement, timeAlive, timeToLive);
                
                if (behaviour == null || behaviour.allowBaseUpdate)
                    base.Update();
            }
        }

        public override void Draw(DrawBatch drawBatch)
        {
            if (sprite != null)
                sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0, -position.Z), 0f, sprite.Measurements.ToVector2() / 2, scale, DrawBatch.CalculateDepth(PositionXY));
        }

        public class Smoke : Particle
        {
            public Smoke(Vector3 position, Vector3 movement, int timeToLive, float scale, float rotation) : base(position, movement, timeToLive)
            {
                this.scale = scale; this.rotation = rotation;
                useGravity = false;
                usePhysics = false;
                behaviour = new Behaviour.Smoke();
                sprite = new AnimatedSprite(textures["Smoke"], new Point(16), 4);
                sprite.lightBleedThrough = .8f;
                sprite.speed = 4f / timeToLive;
                airResistance = .88f;
            }

            public override void Update()
            {
                movement *= airResistance;
                base.Update();
            }
        }

        public class Ember : Particle
        {
            Color color;
            public Ember(Vector3 position, Vector3 movement, int timeToLive, Color color, float scale) : base(position, movement, timeToLive)
            {
                this.scale = scale;
                this.color = color;
                useGravity = false;
                usePhysics = false;
            }

            public override void Update()
            {
                movement.Z += .1f;
                movement = new Vector3(movement.X * .9f, movement.Y * .9f, movement.Z * .8f);
                base.Update();
            }

            public override void Draw(DrawBatch drawBatch)
            {
                Color newColor = Color.Lerp(color, Color.Transparent, MathTransformations.Transform(MathTransformations.Type.NormalizedSmoothStart2, timeAlive / (float)timeToLive) + .4f);
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, Game1.pixel, new Rectangle((int)(PositionXY.X - 1 * scale), (int)((position.Y - position.Z) - 1 * scale), (int)(2 * scale), (int)(2 * scale)), new Rectangle(0,0,1,1), newColor, DrawBatch.CalculateDepth(PositionXY), (short) 2, 1);
                drawBatch.Draw(DrawBatch.DrawCall.Tag.Light, Game1.fader, new Vector2(PositionXY.X, PositionXY.Y - position.Z), .35f * scale, 0f, new Vector2(Game1.fader.Width * .5f), Color.Lerp(newColor, Color.DarkGray, .05f), DrawBatch.CalculateDepth(PositionXY));

                base.Draw(drawBatch);
            }
        }


        public abstract class Behaviour
        {
            /// <summary>
            /// Determines wether the base update for the GameObject is allowed to run
            /// </summary>
            public bool allowBaseUpdate = true;
            public virtual void Update(ref Vector3 position, ref Vector3 movement, int timeAlive, int timeToLive)
            {
                
            }

            public class Smoke : Behaviour
            {
                public override void Update(ref Vector3 position, ref Vector3 movement, int timeAlive, int timeToLive)
                {
                    movement.Z += World.gravity * .05f;
                    base.Update(ref position, ref movement, timeAlive, timeToLive);
                }
            }
        }
    }
}
