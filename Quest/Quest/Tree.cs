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
    public class Tree : TileObject
    {
        static Texture2D texture;
        public Tree(Point position) : base(Type.Tree, new Rectangle(position, new Point(1, 1)), "Tree", new Point(24, 76), Point.Zero, true)
        {

        }

        public static void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("Tree");
        }

        public override void Draw(DrawBatch drawBatch)
        {
            if (SelectionHandler.TileObjectsContains(Position))
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, texture, (hitBox.Location.ScaleRet(World.tileSize) + new Point(World.tileSize / 2) - origin).ToVector2(), Color.Red, DrawBatch.CalculateDepth(new Vector2(Position.X * 16 + 8, Position.Y * 16 + 8)));
            else
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, texture, (hitBox.Location.ScaleRet(World.tileSize) + new Point(World.tileSize / 2) - origin).ToVector2(), Color.White, DrawBatch.CalculateDepth(new Vector2(Position.X * 16 + 8, Position.Y * 16 + 8)));

            base.Draw(drawBatch);
        }
    }
}
