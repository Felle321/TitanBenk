using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public class TripleBool
    {
        public bool a, b, c;

        public static TripleBool False
        {
            get { return new TripleBool(false); }
        }
        public static TripleBool True
        {
            get { return new TripleBool(true); }
        }

        public bool Value
        {
            get { return a && b && c; }
            set
            {
                a = value;
                b = value;
                c = value;
            }
        }

        public TripleBool()
        {
            this.a = false;
            this.b = false;
            this.c = false;
        }
        public TripleBool(bool a)
        {
            this.a = a;
            this.b = a;
            this.c = a;
        }
        public TripleBool(bool a, bool b, bool c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        #region Operators
        public static bool operator ==(TripleBool a, bool x)
        {
            return a.Value == x;
        }
        public static bool operator !=(TripleBool a, bool x)
        {
            return a.Value != x;
        }
        public static bool operator ==(TripleBool a, TripleBool x)
        {
            return a.Value == x.Value;
        }
        public static bool operator !=(TripleBool a, TripleBool x)
        {
            return a.Value != x.Value;
        }

        public static bool operator |(TripleBool a, bool x)
        {
            return a.Value || x;
        }
        public static bool operator &(TripleBool a, bool x)
        {
            return a.Value && x;
        }
        public static bool operator |(TripleBool a, TripleBool x)
        {
            return a.Value || x.Value;
        }
        public static bool operator &(TripleBool a, TripleBool x)
        {
            return a.Value && x.Value;
        }
        #endregion

        public static TripleBool Parse(Object obj)
        {
            if (obj.GetType() == typeof(TripleBool))
            {
                return obj as TripleBool;
            }
            else if (obj.GetType() == typeof(string))
            {
                string text = obj as string;
                string[] texts = text.Split(',');
                return new TripleBool(bool.Parse(texts[0]), bool.Parse(texts[1]), bool.Parse(texts[2]));
            }
            else
                return null;
        }

        public override string ToString()
        {
            return a.ToString() + "," + b.ToString() + "," + c.ToString();
        }
    }
}
