using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public static class Extensions
    {
        public static List<KeyValuePair<string, Vector4>> ToKeyValuePairList(this Dictionary<string, Vector4> dictionary)
        {
            List<KeyValuePair<string, Vector4>> ret = new List<KeyValuePair<string, Vector4>>();
            foreach (var item in dictionary)
            {
                ret.Add(new KeyValuePair<string, Vector4>(item.Key, item.Value));
            }

            return ret;
        }

        /// <summary>
        /// Returns the angle of the Vector2 in radians
        /// </summary>
        /// <param name="v">this Vector2</param>
        /// <returns></returns>
        public static float GetAngle(this Vector2 v)
        {

            v.Normalize();
            float ret = 0f;

            if (float.IsNaN(v.X) || float.IsNaN(v.Y))
                return ret;

            if (v.X == 0)
            {
                if (v.Y == 0)
                    return ret;
                if (v.Y == 1)
                    ret = (float)Math.PI * .5f;
                else if (v.Y == -1)
                    ret = -(float)Math.PI * .5f;
            }
            else if (v.Y == 0)
            {
                if (v.X == 1)
                    ret = 0f;
                else if (v.X == -1)
                    ret = -(float)Math.PI;
            }
            else
            {
                ret = (float)Math.Atan2(v.Y, v.X);
            }

            return ret;
        }

        /// <summary>
        /// Returns the angle of the Vector2 in radians
        /// </summary>
        /// <param name="v">this Vector2</param>
        /// <returns></returns>
        public static float GetAngle(float x, float y)
        {
            return new Vector2(x, y).GetAngle();
        }

        /// <summary>
        /// Keeps an angle in the spectrum of 2-Pi
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float NormalizeRadiansF(float angle)
        {
            if (angle > 0)
            {
                return angle % (float)(Math.PI * 2);
            }
            else if (angle < 0)
            {
                return angle % (float)(Math.PI * 2) + 2 * (float)(Math.PI * 2);
            }
            else
                return 0f;
        }
        /// <summary>
        /// Keeps an angle in the spectrum of 2-Pi
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float NormalizeRadians(this float angle)
        {
            //if (angle < 0)
            //angle *= -1f;

            //return angle % (float)(Math.PI * 2);

            if (angle > 0)
            {
                return angle % (float)(Math.PI * 2);
            }
            else if (angle < 0)
            {
                while (angle < 0)
                {
                    angle += (float)(Math.PI * 2);
                }
                return angle;
            }
            else
                return 0f;
        }

        public static Vector2 GetVector2(float angle, float length)
        {
            Vector2 ret = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            ret = ret.SetLength(length);
            return ret;
        }


        public static Vector3 GetVector3(Vector3 rotation, float length)
        {
            Vector3 ret = new Vector3((float)Math.Cos(rotation.X) * length, (float)Math.Cos(rotation.Y) * length, (float)Math.Cos(rotation.Z) * length);
            return ret;
        }

        public static Vector2 SetLength(this Vector2 v, float length)
        {
            float L = v.Length();
            Vector2 ret = ((length / L) * v);
            return ret;
        }

        public static Vector4 GetVector4(Vector2 a, Vector2 b)
        {
            return new Vector4(a.X, a.Y, b.X, b.Y);
        }

        public static bool ContainsX(this List<Point> list, int x)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].X == x)
                    return true;
            }

            return false;
        }

        public static float LerpFloat(float a, float b, float magnitude)
        {
            if (magnitude <= 0)
                magnitude = a;
            else if (magnitude >= 1)
                magnitude = b;
            return a * (1 - magnitude) + b * magnitude;
        }

        public static float LerpAngles(float a, float b, float magnitude, bool invertRotation)
        {
            if (a == b) { return a; }
            /*
            bool aToB = true;
            float delta = 0f;

            if (magnitude <= 0)
                magnitude = a;
            else if (magnitude >= 1)
                magnitude = b;


            delta = b - a;

            if (delta > Math.PI)
                aToB = false;

            if (invertRotation)
                aToB = !aToB;

            if (!aToB)
                delta = (float)Math.PI - delta;

            a += delta * magnitude;

            while (a < 0)
                a += (float)Math.PI * 2;

            return a;*/
            float delta;

            if (invertRotation)
                invertRotation = invertRotation;

            a = NormalizeAngles(a);
            b = NormalizeAngles(b);

            //if (!invertRotation)
            //{
            //    if (b < a)
            //        delta = (float)Math.PI * 2 - (a - b);
            //    else
            //        delta = b - a;
            //}
            //else
            //{
            //    if (b < a)
            //        delta = -(a - b);
            //    else
            //        delta = (float)-Math.PI * 2 + (b - a);
            //}

            /*
            if (b > a)
            {
                if (b - a > (float)Math.PI)
                    delta = (float)Math.PI*2 - b + a;
                else
                    delta = b - a;
            }
            else
            {
                if (a - b > (float)Math.PI)
                    delta = (float)Math.PI * 2 - a + b;
                else
                    delta = a - b;
            }
            
            {
                if (b <= a)
                    delta = -delta;
            }

            if (invertRotation && delta > 0)
                delta = -(delta / Math.Abs(delta) * ((float)Math.PI - Math.Abs(delta)));
            else if (invertRotation)
                delta = ((float)Math.PI - Math.Abs(delta));
            else
                delta = delta;

            */

            delta = b - a;

            if (b > a)
            {
                if (delta > (float)Math.PI)
                    delta = (float)-(Math.PI * 2 - delta);
                else
                    a = a;
            }
            else
            {
                if (delta > (float)Math.PI)
                    delta = (float)Math.PI * 2 + delta;
            }

            if (invertRotation)
            {
                if (delta < 0)
                    delta += (float)Math.PI * 2;
                else
                    delta -= (float)Math.PI * 2;
            }

            return a + delta * magnitude;


        }

        public static float NormalizeAngles(float a)
        {
            while (a < 0)
                a += (float)Math.PI * 2;
            while (a > (float)Math.PI * 2)
                a -= (float)Math.PI * 2;

            return a;
        }

        public static Vector2 LerpVector2(Vector2 a, Vector2 b, float magnitude)
        {
            return new Vector2(LerpFloat(a.X, b.X, magnitude), LerpFloat(a.Y, b.Y, magnitude));
        }
        internal static Vector3 LerpVector3(Vector3 a, Vector3 b, float magnitude)
        {
            return new Vector3(LerpFloat(a.X, b.X, magnitude), LerpFloat(a.Y, b.Y, magnitude), LerpFloat(a.Z, b.Z, magnitude));
        }
    }
}
