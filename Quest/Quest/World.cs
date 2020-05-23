using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Quest
{
    public static class World
    {
        public static int tileSize = 16;
        public const float gravity = 1f;

        public static void Scale(this Point p, float scale)
        {
            p = new Point((int)Math.Round(p.X * scale), (int)Math.Round(p.Y * scale));
        }
        public static Point ScaleRet(this Point p, float scale)
        {
            return new Point((int)Math.Round(p.X * scale), (int)Math.Round(p.Y * scale));
        }
        public static Point ScaleRet(this Point p, Vector2 scale)
        {
            return new Point((int)Math.Round(p.X * scale.X), (int)Math.Round(p.Y * scale.Y));
        }

        public static double Distance(this Point a, Point b)
        {
            Point p = b - a;
            return Length(p);
        }

        public static double Length(this Point p)
        {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }

        public static double LengthSquared(this Point p)
        {
            return p.X * p.X + p.Y * p.Y;
        }

        public static Vector3 ToVector3(this Vector2 v, float z)
        {
            return new Vector3(v, z);
        }
    }
}
