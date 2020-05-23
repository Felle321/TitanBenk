using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
    public abstract class TileObject
    {
        /// <summary>
        /// Position and size in resolution of tile
        /// </summary>
        public Rectangle hitBox;
        public string name;
        public Point origin;
        public enum Type
        {
            Null,
            Tree
        }
        public Type type;
        internal bool physicsBlock;
        internal Point tileOrigin;

        public Point Position
        {
            get
            {
                return hitBox.Location;
            }
            set
            {
                hitBox.Location = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="hitBox"></param>
        /// <param name="name"></param>
        /// <param name="origin"></param>
        /// <param name="tileOrigin">The point in the hitBox where the actual position of the object is located</param>
        /// <param name="physicsBlock"></param>
        public TileObject(Type type, Rectangle hitBox, string name, Point origin, Point tileOrigin, bool physicsBlock)
        {
            this.type = type;
            this.hitBox = hitBox;
            this.name = name;
            this.origin = origin;
            this.tileOrigin = tileOrigin;
            this.physicsBlock = physicsBlock;
        }

        public virtual void Draw(DrawBatch drawBatch)
        {
        }

        public override string ToString()
        {
            return name;
        }

        internal static void LoadContent(ContentManager Content)
        {
            Tree.LoadContent(Content);
        }
    }
}
