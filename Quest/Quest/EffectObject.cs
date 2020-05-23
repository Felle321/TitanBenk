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
    public abstract class EffectObject
    {
        public Vector3 position;
        public bool hasLight = false;
        AnimatedSprite sprite, spriteLights;
        public bool remove = false;
        public Vector2 scale = new Vector2(1);
        public Vector2 origin = Vector2.Zero;
        public float rotation = 0f;
        public float speed = 1f;

        public AnimatedSprite SpriteLights
        {
            get
            {
                return spriteLights;
            }
            set
            {
                if (value == null)
                    hasLight = false;
                else
                    hasLight = true;

                spriteLights = value;
            }
        }

        public Vector2 PositionXY { get { return new Vector2(position.X, position.Y); } }

        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static void LoadContent(ContentManager content)
        {
            textures.Add("Explosion", content.Load<Texture2D>("Explosion"));
            textures.Add("ExplosionLights", content.Load<Texture2D>("EffectSprites\\ExplosionLights"));
        }

        public EffectObject(Vector3 position, AnimatedSprite sprite)
        {
            this.position = position;
            this.sprite = sprite;
        }

        public EffectObject(Vector3 position, AnimatedSprite sprite, Vector2 scale, Vector2 origin, float rotation)
        {
            this.position = position;
            this.sprite = sprite;
            this.scale = scale;
            this.origin = origin;
            this.rotation = rotation;
        }

        public virtual void Draw(DrawBatch drawBatch)
        {
            sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0, -position.Z), rotation, origin, scale, DrawBatch.CalculateDepth(PositionXY));

            if(hasLight)
                SpriteLights.Draw(drawBatch, DrawBatch.DrawCall.Tag.Light, PositionXY + new Vector2(0, -position.Z), rotation, origin, scale, DrawBatch.CalculateDepth(PositionXY) -.0000001f);
            if (sprite.Finished)
                remove = true;
        }


        public class Explosion : EffectObject
        {
            public Explosion(Vector3 position, Vector2 scale) : base(position, new AnimatedSprite(EffectObject.textures["Explosion"], new Point(64), 8, .25f), scale, new Vector2(32), 0f)
            {
                int particles = Game1.random.Next(8, 12);
                this.SpriteLights = new AnimatedSprite(EffectObject.textures["ExplosionLights"], new Point(128), 8, .25f);

                sprite.affectedByLight = 2;

                Vector2 randomMovement;
                for (int i = 0; i < particles; i++)
                {
                    randomMovement = new Vector2((float)(Game1.random.NextDouble() * 2) - 1, (float)(Game1.random.NextDouble() * 2) - 1);
                    randomMovement *= 3.5f * scale;
                    Game1.gameObjects.Add(new Particle.Ember(position, new Vector3((Game1.random.Next(10) - 5) * scale.X, (Game1.random.Next(10) - 5) * scale.X, (float)(Game1.random.NextDouble() * 1.2) * scale.X), Game1.random.Next(50) + 40, Color.Gold, (float)(Game1.random.NextDouble() + .1f) * scale.X *.3f));
                    Game1.gameObjects.Add(new Particle.Smoke(new Vector3(position.X + randomMovement.X, position.Y + randomMovement.Y, position.Z), randomMovement.ToVector3((float)Game1.random.NextDouble()), (int)(Game1.random.Next(45) * scale.X) + 25, ((float)Game1.random.NextDouble() * .5f + .5f) * scale.X, (float)Game1.random.NextDouble() * 6.28f));
                }
            }

            public override void Draw(DrawBatch drawBatch)
            {
                sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.GameObject, PositionXY + new Vector2(0, -position.Z), rotation, origin, scale, DrawBatch.CalculateDepth(PositionXY));
                //sprite.frame -= sprite.speed;
                //sprite.Draw(drawBatch, DrawBatch.DrawCall.Tag.Light, PositionXY + new Vector2(0, -position.Z), rotation, origin, scale, DrawBatch.CalculateDepth(PositionXY) + .01f);

                if (hasLight)
                    SpriteLights.Draw(drawBatch, DrawBatch.DrawCall.Tag.Light, PositionXY + new Vector2(0, -position.Z), rotation, origin * 2, scale * 2, Color.Lerp(Color.LightGoldenrodYellow, Color.OrangeRed, sprite.frame / sprite.frames), DrawBatch.CalculateDepth(PositionXY) + .01f);

                base.Draw(drawBatch);
            }
        }

    }
}
