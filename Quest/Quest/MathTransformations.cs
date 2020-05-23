using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public class MathTransformations
    {
        public class Quadruple
        {
            public Type x, y, z, w;

            public Quadruple(Type a)
            {
                this.x = a;
                this.y = a;
                this.z = a;
                this.w = a;
            }
            public Quadruple(Type a, Type b, Type c, Type d)
            {
                this.x = a;
                this.y = b;
                this.z = c;
                this.w = d;
            }

            public override string ToString()
            {
                return x.ToString() + "," + y.ToString() + "," + z.ToString() + "," + z.ToString();
            }

            public static Quadruple Parse(string text)
            {
                string[] texts = text.Split(',');
                return new Quadruple((Type)Enum.Parse(typeof(Type), texts[0]), (Type)Enum.Parse(typeof(Type), texts[1]), (Type)Enum.Parse(typeof(Type), texts[2]), (Type)Enum.Parse(typeof(Type), texts[3]));
            }
        }

        public enum Type
        {
            Linear,
            NormalizedSmoothStop,
            NormalizedSmoothStart,
            NormalizedSmoothStop2,
            NormalizedSmoothStart2,
            NormalizedSmoothStartStop,
            NormalizedSmoothStartStop2
        }
        public static float Transform(Type type, float x)
        {
            switch (type)
            {
                case (Type.NormalizedSmoothStop):
                    if (x < 0)
                        return 0;
                    else if (x > 1)
                        return 1;

                    return 1 - (x - 1) * (x - 1);
                case (Type.NormalizedSmoothStart):
                    if (x < 0)
                        return 0;
                    else if (x > 1)
                        return 1;

                    return (x * x);
                case (Type.NormalizedSmoothStop2):
                    x = Transform(Type.NormalizedSmoothStop, x);
                    return x * x;
                case (Type.NormalizedSmoothStart2):
                    x = Transform(Type.NormalizedSmoothStart, x);
                    return x * x;
                case (Type.NormalizedSmoothStartStop):
                    return Lerp(Type.NormalizedSmoothStart, Type.NormalizedSmoothStop, x, x);
                case (Type.NormalizedSmoothStartStop2):
                    return Lerp(Type.NormalizedSmoothStart2, Type.NormalizedSmoothStop2, x, x);
                default:
                    return x;
            }
        }

        public static float Lerp(Type a, Type b, float x, float magnitude)
        {
            return Transform(a, x) * (1 - magnitude) + Transform(b, x) * magnitude;
        }
    }
}
