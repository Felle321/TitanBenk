using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public class DroppedItem : GameObject
    {
        public  Item item;
        public DroppedItem(Vector3 position, Vector3 movement, Item item) : base(Type.DroppedItem, "DroppedItem")
        {
            this.position = position;
            this.movement = movement;
            useGravity = true;
            usePhysics = true;
            this.item = item;
            size = new Point(10);
        }

        public override void Draw(DrawBatch drawBatch)
        {
            DrawName(drawBatch, .6f);
            item.DrawIcon(drawBatch, new Rectangle(PositionXY.ToPoint() - new Point(8), new Point(16)), DrawBatch.CalculateDepth(PositionXY));
            base.Draw(drawBatch);
        }

        public virtual void DrawName(DrawBatch drawBatch, float scale)
        {
            Vector2 measurements = (Game1.fontDebug.MeasureString(item.name) * scale);
            float depth = DrawBatch.CalculateDepth(PositionXY);
            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, Game1.pixel, new Rectangle((int)(position.X - measurements.X / 2), (int)(position.Y + 20 - position.Z - measurements.Y / 2), (int)measurements.X, (int)measurements.Y), Color.DarkBlue, depth);
            drawBatch.DrawString(DrawBatch.DrawCall.Tag.GameObject, item.name, Game1.fontDebug, new Vector2(position.X, position.Y + 20 - position.Z), scale, 0f, measurements / 2, Color.White, depth);
        }
    }
}
