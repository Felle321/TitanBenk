using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
    public class RigLimb
    {
        public float length;
        public Vector3 position, endPosition;
        public Vector3 rotation;
        public List<string> limbs = new List<string>();
        public string name, parent;
        public bool drawTexture = true;

        public RigLimb(string name, string parent, Vector3 rotation, float length)
        {
            this.name = name;
            this.parent = parent;
            this.rotation = rotation;
            this.length = length;
        }

        public void AddLimb(string limb)
        {
            this.limbs.Add(limb);
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public void SetRotation(Vector3 rotation)
        {
            this.rotation = rotation;
        }

        public void UpdateEndPosition(float scale)
        {
            endPosition = GetEndPosition(position, rotation, length * scale);
        }
        public static Vector3 GetEndPosition(Vector3 startPos, Vector3 rotation, float length)
        {
            return startPos + new Vector3((float)(Math.Cos(rotation.X) * Math.Cos(rotation.Y)) * length, (float)(Math.Sin(rotation.X) * Math.Cos(rotation.Y)) * length, (float)Math.Sin(rotation.Y) * length);
        }

        /// <summary>
        /// Returns the line to be displayed in 2D
        /// </summary>
        /// <param name="orientation">In radians, 0 being to the right</param>
        /// <returns></returns>
        public Vector4 Get2DLine(float orientation)
        {
            Vector2 a, b;
            a = Get2DPosition(position, orientation);
            b = Get2DPosition(endPosition, orientation);
            return new Vector4(a.X, a.Y, b.X, b.Y);
        }

        /// <summary>
        /// Returns the position as it would be displayed in 2D
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 Get2DPosition(Vector3 position, float orientation)
        {
            //if (orientation < 0)
            //    orientation = - orientation;
            orientation %= (float)Math.PI * 2;

            /*
            float a, b, c, d;
            a = 0;
            b = (float)Math.PI / 2;
            c = (float)Math.PI;
            d = c + b;

            Vector2 ret = Vector2.Zero;
            Vector2 right, up, left, down;

            right = new Vector2(position.X, position.Z);
            up = new Vector2(position.Y, position.Z);
            left = new Vector2(-position.X, position.Z);
            down = new Vector2(-position.Y, position.Z);

            if (orientation < b)
                ret = Extensions.LerpVector2(right, up, (float)Math.Sin(orientation));
            else if (orientation < c)
                ret = Extensions.LerpVector2(up, left, Math.Abs((float)Math.Cos(orientation)));
            else if (orientation < d)
                ret = Extensions.LerpVector2(left, down, Math.Abs((float)Math.Sin(orientation)));
            else if (orientation >= d)
                ret = Extensions.LerpVector2(down, right, (float)Math.Cos(orientation));

            return ret;
            */

            Vector2 ret;

            ret = new Vector2(((float)Math.Cos(orientation) * position.X + (float)Math.Sin(orientation) * position.Y), position.Z + ((float)Math.Sin(-orientation) * position.X + (float)Math.Cos(orientation) * position.Y) / 2);

            return ret;
        }

        /// <summary>
        /// Returns the angle of the observation of a limb. Meaning it's front or backside
        /// </summary>
        /// <returns></returns>
        public static float GetObservedAngle(float cameraOrientation, float xLimbRotation)
        {
            return xLimbRotation - cameraOrientation;
        }

        internal float GetInternalDepth(float orientation)
        {
            Vector3 newPos = GetEndPosition(position, new Vector3(rotation.X + orientation, rotation.Y, rotation.Z), length);
            Vector3 normalPos = GetEndPosition(position, rotation, length);
            float oldDepth = (float)Math.Sin(orientation) * normalPos.X + (float)Math.Cos(orientation) * normalPos.Y;
            float newDepth = (float)Math.Sin(orientation) * position.X + (float)Math.Cos(orientation) * position.Y;
            return newDepth;
        }
    }
}
