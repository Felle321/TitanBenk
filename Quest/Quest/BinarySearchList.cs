using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    class BinarySearchList<T>
    {
        List<float> keys = new List<float>();
        List<T> values = new List<T>();
        public int Count
        {
            get { return keys.Count; }
        }

        public void Add(float key, T value)
        {
            int index = GetIndexOfKey(key, true);
            /*
            while ((index < keys.Count - 1) && keys[index] == keys[index + 1])
                index++;
            */


            keys.Insert(index, key);
            values.Insert(index, value);
        }

        public T GetValue(float key, bool getLastIndex)
        {
            int i = GetIndexOfKey(key, getLastIndex);

            if (i < 0)
                return default(T);

            return values[i];
        }

        public KeyValuePair<float, T> GetKeyValuePairFromIndex(int i)
        {
            KeyValuePair<float, T> ret = new KeyValuePair<float, T>(keys[i], values[i]);
            return ret;
        }

        public T GetValue(int i)
        {
            return values[i];
        }
        public float GetKeyFromIndex(int i)
        {
            return keys[i];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getLastIndex">If true, insists the returned index is the last of the lot</param>
        /// <returns></returns>
        public int GetIndexOfKey(float key, bool getLastIndex)
        {
            int previousIndex, index;
            index = BinarySearchRecursive(key, 0, values.Count, out previousIndex);
            if (!getLastIndex)
                return index;

            //Get the last
            if (index != keys.Count - 1 && previousIndex != index && keys[index + 1] == keys[index])
            {
                BinarySearchRecursive(key, index + 1, previousIndex, out previousIndex);
            }

            return index;
        }

        public bool ContainsKey(float key)
        {
            return (keys[GetIndexOfKey(key, false)] == key);
        }

        public bool ContainsValue(T value)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].Equals(value))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the index of the first item with given 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected int BinarySearchRecursive(float key, int min, int max, out int previousIndex)
        {
            previousIndex = max;
            if (min > max)
            {
                return -1;
            }
            else if (min == max)
            {
                return min;
            }
            else
            {
                int mid = (min + max) / 2;
                if (key == keys[mid])
                {
                    previousIndex = max;
                    return mid;
                }
                else if (key < keys[mid])
                {
                    if (mid - 1 < min)
                    {
                        previousIndex = mid;
                        return mid;
                    }
                    return BinarySearchRecursive(key, min, mid, out previousIndex);
                }
                else
                {
                    if (mid + 1 > max)
                    {
                        previousIndex = mid;
                        return mid;
                    }
                    return BinarySearchRecursive(key, mid + 1, max, out previousIndex);
                }
            }
        }

        public override int GetHashCode()
        {
            return keys.GetHashCode() * values.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            else
            {
                return this.ToString() == obj.ToString();
            }
        }

        public override string ToString()
        {
            string ret = "";
            if (values.Count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    ret += "(" + keys[i].ToString() + "," + values[i].ToString() + ");";
                    if (i < values.Count - 1)
                        ret += "\n";
                }
            }

            return ret;
        }

        internal void Clear()
        {
            keys.Clear();
            values.Clear();
        }
    }
}