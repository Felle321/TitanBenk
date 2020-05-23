using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
    public class AnimatedSprite
    {
        public Texture2D texture;
        public float frame = 0f, speed = 1f;
        public int frames;
        Point measurements, sheetMeasurements;
        public bool repeat = false;
        /// <summary>
        /// Decides wether the object drawn is affected by lights or not
        /// 0 - always dark, 1 - affected, 2 - always bright
        /// </summary>
        public short affectedByLight;
        /// <summary>
        /// The amount of light that passes through the object
        /// </summary>
        public float lightBleedThrough;

        public Point sourceRectangleOffset = Point.Zero;

        public Point Measurements
        {
            get { return measurements; }
            set 
            { 
                measurements = value;
                sheetMeasurements = new Point(texture.Width / measurements.X, texture.Height / measurements.Y);
            }
        }
        public Point SheetMeasurements
        {
            get { return sheetMeasurements; }
        }
        public bool Finished
        {
            get
            {
                return (int)frame == frames;
            }
        }

        public AnimatedSprite(Texture2D texture, Point measurements, int frames)
        {
            this.texture = texture;
            sheetMeasurements = new Point(texture.Width / measurements.X, texture.Height / measurements.Y);
            this.measurements = measurements;
            this.frames = frames;
        }
        public AnimatedSprite(Texture2D texture, Point measurements, int frames, float speed)
        {
            this.texture = texture;
            sheetMeasurements = new Point(texture.Width / measurements.X, texture.Height / measurements.Y);
            this.measurements = measurements;
            this.frames = frames;
            this.speed = speed;
        }
        public AnimatedSprite(Texture2D texture, Point measurements, int frames, float speed, bool repeat)
        {
            this.texture = texture;
            sheetMeasurements = new Point(texture.Width / measurements.X, texture.Height / measurements.Y);
            this.measurements = measurements;
            this.frames = frames;
            this.speed = speed;
            this.repeat = repeat;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="measurements"></param>
        /// <param name="frames"></param>
        /// <param name="speed"></param>
        /// <param name="repeat"></param>
        /// <param name="sourceRectangleOffset">Marks the starting point on the sheet</param>
        public AnimatedSprite(Texture2D texture, Point measurements, int frames, float speed, bool repeat, Point sourceRectangleOffset)
        {
            this.texture = texture;
            sheetMeasurements = new Point(texture.Width / measurements.X, texture.Height / measurements.Y);
            this.measurements = measurements;
            this.frames = frames;
            this.speed = speed;
            this.repeat = repeat;
            this.sourceRectangleOffset = sourceRectangleOffset;
        }

        public Rectangle GetSourceRectangle()
        {
            int currentFrame = (int)frame;

            if (currentFrame == frames)
            {
                currentFrame--;
            }

            Point pos = new Point((int)currentFrame % sheetMeasurements.X, (int)(currentFrame / sheetMeasurements.X) % sheetMeasurements.Y);
            Rectangle ret = new Rectangle(pos.X * measurements.X, pos.Y * measurements.Y, measurements.X, measurements.Y);

            //Offset
            ret.Location = new Point(ret.X + sourceRectangleOffset.X, ret.Y + sourceRectangleOffset.Y);

            while (ret.X > texture.Width)
            {
                ret.X -= texture.Width;
                ret.Y += Measurements.Y;
            }

            return ret;
        }

        public void Increment()
        {
            frame += speed;
            if (frame >= frames)
            {
                if (repeat)
                    frame %= (float)frames;
                else
                    frame = frames;
            }
        }

        public void Draw(DrawBatch drawBatch, DrawBatch.DrawCall.Tag tag, Vector2 position, float depth)
        {
            drawBatch.Draw(tag, texture, position, GetSourceRectangle(), new Vector2(1), 0f, Vector2.Zero, Color.White, depth, SpriteEffects.None, affectedByLight, lightBleedThrough);
            Increment();
        }
        public void Draw(DrawBatch drawBatch, DrawBatch.DrawCall.Tag tag, Vector2 position, float rotation, Vector2 origin, float depth)
        {
            drawBatch.Draw(tag, texture, position, GetSourceRectangle(), new Vector2(1), rotation, origin, Color.White, depth, SpriteEffects.None, affectedByLight, lightBleedThrough);
            Increment();
        }
        public void Draw(DrawBatch drawBatch, DrawBatch.DrawCall.Tag tag, Vector2 position, float rotation, Vector2 origin, float scale, float depth)
        {
            drawBatch.Draw(tag, texture, position, GetSourceRectangle(), new Vector2(scale), rotation, origin, Color.White, depth, SpriteEffects.None, affectedByLight, lightBleedThrough);
            Increment();
        }
        public void Draw(DrawBatch drawBatch, DrawBatch.DrawCall.Tag tag, Vector2 position, float rotation, Vector2 origin, Vector2 scale, float depth)
        {
            drawBatch.Draw(tag, texture, position, GetSourceRectangle(), scale, rotation, origin, Color.White, depth, SpriteEffects.None, affectedByLight, lightBleedThrough);
            Increment();
        }
        public void Draw(DrawBatch drawBatch, DrawBatch.DrawCall.Tag tag, Vector2 position, float rotation, Vector2 origin, Vector2 scale, Color color, float depth)
        {
            drawBatch.Draw(tag, texture, position, GetSourceRectangle(), scale, rotation, origin, color, depth, SpriteEffects.None, affectedByLight, lightBleedThrough);
            Increment();
        }
        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle, Point sourceRectangleOffset, Point sourceRectangleMeasurements, Color color)
        {
            Rectangle sourceRectangle = GetSourceRectangle();
            sourceRectangle.Location = sourceRectangle.Location + sourceRectangleOffset;
            sourceRectangle.Size = sourceRectangleMeasurements;
            spriteBatch.Draw(texture, rectangle, sourceRectangle, color);
            Increment();
        }
    }
}
