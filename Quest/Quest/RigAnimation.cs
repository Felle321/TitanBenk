using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Quest
{
    public class RigAnimation
    {
        public struct KeyFrame
        {
            public float key;
            public Dictionary<string, Vector4> values;
            public Dictionary<string, TripleBool> invertRotation;
            public Dictionary<string, MathTransformations.Quadruple> transformations;



            public KeyFrame(float key, Dictionary<string, Vector4> values)
            {
                this.key = key;
                this.values = values;
                this.invertRotation = new Dictionary<string, TripleBool>();
                this.transformations = new Dictionary<string, MathTransformations.Quadruple>();

                foreach (var item in values)
                {
                    if (!invertRotation.ContainsKey(item.Key))
                        invertRotation.Add(item.Key, TripleBool.False);
                    if (!transformations.ContainsKey(item.Key))
                        transformations.Add(item.Key, new MathTransformations.Quadruple(MathTransformations.Type.Linear));
                }
            }
            public KeyFrame(float key, Dictionary<string, Vector4> values, Dictionary<string, TripleBool> invertRotation)
            {
                this.key = key;
                this.values = values;
                this.invertRotation = invertRotation;
                this.transformations = new Dictionary<string, MathTransformations.Quadruple>();

                foreach (var item in values)
                {
                    if (!invertRotation.ContainsKey(item.Key))
                        invertRotation.Add(item.Key, TripleBool.False);
                    if (!transformations.ContainsKey(item.Key))
                        transformations.Add(item.Key, new MathTransformations.Quadruple(MathTransformations.Type.Linear));
                }
            }
            public KeyFrame(float key, Dictionary<string, Vector4> values, Dictionary<string, TripleBool> invertRotation, Dictionary<string, MathTransformations.Quadruple> transformation)
            {
                this.key = key;
                this.values = values;
                this.invertRotation = invertRotation;
                this.transformations = transformation;

                if (invertRotation.Count < values.Count)
                {
                    foreach (var item in values)
                    {
                        if (!invertRotation.ContainsKey(item.Key))
                            invertRotation.Add(item.Key, TripleBool.False);
                    }
                }
            }

            public static KeyFrame Empty { get { return new KeyFrame(); } }
            public override string ToString()
            {
                return key.ToString() + ": " + invertRotation[Game1.selectedLimb].ToString() + ": " + transformations[Game1.selectedLimb].ToString();
            }

            internal string ExportData()
            {
                string ret = "";
                ret += key + "_";

                string[] keyes = values.Keys.ToArray();
                for (int j = 0; j < keyes.Length; j++)
                {
                    ret += keyes[j] + " " + values[keyes[j]].X + " " + values[keyes[j]].Y + " " + values[keyes[j]].Z + " " + values[keyes[j]].W;
                    if (j < keyes.Length - 1)
                        ret += " ";
                }

                ret += "_";

                for (int j = 0; j < keyes.Length; j++)
                {
                    ret += keyes[j] + " " + invertRotation[keyes[j]].ToString();
                    if (j < keyes.Length - 1)
                        ret += " ";
                }

                ret += "_";

                for (int j = 0; j < keyes.Length; j++)
                {
                    ret += keyes[j] + " " + transformations[keyes[j]].x + " " + transformations[keyes[j]].y + " " + transformations[keyes[j]].z + " " + transformations[keyes[j]].w;
                    if (j < keyes.Length - 1)
                        ret += " ";
                }

                return ret;
            }
            public static KeyFrame GetKeyFrameFromData(string data)
            {
                string[] datas = data.Split('_');
                string[] values = datas[1].Split(' ');
                string[] inverts = datas[2].Split(' ');
                string[] transformations = datas[3].Split(' ');
                Dictionary<string, Vector4> valuesDictionary = new Dictionary<string, Vector4>();
                Dictionary<string, TripleBool> invertRotationDictionary = new Dictionary<string, TripleBool>();
                Dictionary<string, MathTransformations.Quadruple> transformationDictionary = new Dictionary<string, MathTransformations.Quadruple>();

                for (int i = 0; i < values.Length; i += 5)
                {
                    valuesDictionary.Add(values[i], new Vector4(float.Parse(values[i + 1]), float.Parse(values[i + 2]), float.Parse(values[i + 3]), float.Parse(values[i + 4])));
                }

                for (int i = 0; i < inverts.Length; i += 4)
                {
                    invertRotationDictionary.Add(inverts[i], TripleBool.Parse(inverts[i + 1]));
                }

                for (int i = 0; i < transformations.Length; i += 5)
                {
                    transformationDictionary.Add(transformations[i], new MathTransformations.Quadruple((
                        MathTransformations.Type)Enum.Parse(typeof(MathTransformations.Type), transformations[i + 1])
                        , (MathTransformations.Type)Enum.Parse(typeof(MathTransformations.Type), transformations[i + 2])
                        , (MathTransformations.Type)Enum.Parse(typeof(MathTransformations.Type), transformations[i + 3])
                        , (MathTransformations.Type)Enum.Parse(typeof(MathTransformations.Type), transformations[i + 4])));

                }

                return new KeyFrame(float.Parse(datas[0]), valuesDictionary, invertRotationDictionary, transformationDictionary);
            }
        }

        public Rig.Type rigType;
        public string name;
        public float length = 0;
        public float currentTime = 0;
        int currentFrame = 0, frameCount;
        public List<KeyFrame> keyFrames = new List<KeyFrame>();
        public float speed = 1 / 60f;
        public Dictionary<string, float> priorities = new Dictionary<string, float>();
        public bool finished;
        public bool loop;

        public static string animationLibraryPath;

        public static Dictionary<Rig.Type, Dictionary<string, RigAnimation>> animationLibrary = new Dictionary<Rig.Type, Dictionary<string, RigAnimation>>();

        public RigAnimation(Rig.Type rigType, string name, bool loop)
        {
            this.rigType = rigType;
            this.name = name;
            this.loop = loop;
        }
        public RigAnimation(Rig.Type rigType, string name, bool loop, Dictionary<string, float> priorities)
        {
            this.rigType = rigType;
            this.name = name;
            this.loop = loop;
            this.priorities = priorities;
        }

        public static void LoadContent(ContentManager content)
        {
            string[] names = Enum.GetNames(typeof(Rig.Type));
            for (int i = 0; i < names.Length; i++)
            {
                animationLibrary.Add((Rig.Type)i, new Dictionary<string, RigAnimation>());
            }
            animationLibraryPath = System.IO.Path.GetFullPath(content.RootDirectory) + "\\RigAnimations\\AnimationLibrary.txt";
            ImportAnimations(animationLibraryPath);
        }

        public void SetPriority(string limb, float value)
        {
            if (priorities.ContainsKey(limb))
                priorities[limb] = value;
            else
                priorities.Add(limb, value);
        }

        public KeyFrame GetKeyFrame(float key)
        {
            KeyFrame ret = new KeyFrame();
            for (int i = 0; i < keyFrames.Count; i++)
            {
                if (keyFrames[i].key == key)
                {
                    ret = keyFrames[i];
                    break;
                }
            }
            return ret;
        }

        public bool ContainsKey(float key)
        {
            for (int i = 0; i < keyFrames.Count; i++)
            {
                if (keyFrames[i].key == key)
                {
                    return true;
                }
            }
            return false;
        }
        public bool ContainsKey(float key, out int index)
        {
            index = -1;
            for (int i = 0; i < keyFrames.Count; i++)
            {
                if (keyFrames[i].key == key)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        public void SetKeyFrame(KeyFrame keyFrame)
        {
            int index;
            if (ContainsKey(keyFrame.key, out index))
            {
                keyFrames.RemoveAt(index);
            }

            AddFrame(keyFrame);
        }

        public void AddFrame(KeyFrame frame)
        {
            int index = keyFrames.Count;
            for (int i = 0; i < keyFrames.Count; i++)
            {
                if (keyFrames[i].key < frame.key)
                    index = i;
            }

            if (index + 1 > keyFrames.Count)
                index = 0;
            else index++;

            if (keyFrames.Count > 0 && index == keyFrames.Count)
                length = Math.Abs(keyFrames[0].key - frame.key);

            keyFrames.Insert(index, frame);
            frameCount = keyFrames.Count;
        }

        /// <summary>
        /// Returns a customized keyframe, ready to be applied to a rig
        /// </summary>
        /// <param name="blockedLimbs"></param>
        public KeyFrame Update(string[] blockedLimbs)
        {
            if (!finished)
            {
                SetTime(currentTime + speed);

                return GetTransformationFrame(blockedLimbs);
            }

            return KeyFrame.Empty;
        }

        public void SetTime(float time)
        {
            if (time >= length)
            {
                if (loop)
                    time -= length;
                else
                    finished = true;
            }

            currentTime = time;

            currentFrame = GetCurrentFrame();
        }

        public int GetCurrentFrame()
        {
            int ret = 0;
            for (int i = 0; i < keyFrames.Count - 1; i++)
            {
                if (currentTime >= keyFrames[i].key && currentTime < keyFrames[i + 1].key)
                {
                    ret = i;
                    break;
                }
            }

            if (currentTime >= keyFrames[keyFrames.Count - 1].key)
                ret = keyFrames.Count - 1;

            return ret;
        }
        public int GetNextFrame()
        {
            int ret = currentFrame + 1;
            if (ret >= keyFrames.Count)
                ret = 0;
            return ret;
        }

        public KeyFrame GetTransformationFrame(string[] blockedLimbs)
        {
            if (keyFrames.Count <= 0)
            {
                return new KeyFrame();
            }
            else if (keyFrames.Count == 1)
            {
                return LerpKeyFrames(keyFrames[0], keyFrames[0], 0, blockedLimbs);
            }

            KeyFrame transformationFrame;
            if (currentFrame < keyFrames.Count - 1)
                transformationFrame = LerpKeyFrames(keyFrames[currentFrame], keyFrames[currentFrame + 1], (currentTime - keyFrames[currentFrame].key) / (keyFrames[currentFrame + 1].key - keyFrames[currentFrame].key), blockedLimbs);
            else
                transformationFrame = LerpKeyFrames(keyFrames[currentFrame], keyFrames[0], (currentTime - keyFrames[currentFrame].key) / (length - keyFrames[currentFrame].key), blockedLimbs);

            return (transformationFrame);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameA"></param>
        /// <param name="frameB"></param>
        /// <param name="amount"></param>
        /// <param name="blockedKeys">Refers to keys not allowed to compute. Will probably be used by another animation with a higher priority</param>
        /// <returns></returns>
        public KeyFrame LerpKeyFrames(KeyFrame frameA, KeyFrame frameB, float amount, string[] blockedKeys)
        {
            if (amount >= 1f)
                return frameB;
            else if (amount <= 0f)
                return frameA;
            Dictionary<string, Vector4> values = new Dictionary<string, Vector4>();
            foreach (KeyValuePair<string, Vector4> pair in frameA.values)
            {
                if (!blockedKeys.Contains(pair.Key) && frameB.values.ContainsKey(pair.Key))
                    values.Add(pair.Key, new Vector4(Extensions.LerpAngles(frameA.values[pair.Key].X, frameB.values[pair.Key].X, MathTransformations.Transform(frameB.transformations[pair.Key].x, amount), frameB.invertRotation[pair.Key].a)
                                                    , Extensions.LerpAngles(frameA.values[pair.Key].Y, frameB.values[pair.Key].Y, MathTransformations.Transform(frameB.transformations[pair.Key].y, amount), frameB.invertRotation[pair.Key].b)
                                                    , Extensions.LerpAngles(frameA.values[pair.Key].Z, frameB.values[pair.Key].Z, MathTransformations.Transform(frameB.transformations[pair.Key].z, amount), frameB.invertRotation[pair.Key].c)
                                                    , Extensions.LerpFloat(frameA.values[pair.Key].W, frameB.values[pair.Key].W, MathTransformations.Transform(frameB.transformations[pair.Key].w, amount))));
            }

            return new KeyFrame(currentTime, values, frameA.invertRotation, frameA.transformations);
        }

        public override string ToString()
        {
            return name;
        }

        internal RigAnimation Duplicate()
        {
            RigAnimation ret = new RigAnimation(rigType, name, loop);
            ret.finished = finished;
            ret.frameCount = frameCount;
            ret.keyFrames = keyFrames;
            ret.length = length;
            ret.priorities = priorities;
            ret.speed = speed;

            return ret;
        }

        public static RigAnimation GetAnimationFromLibrary(Rig.Type rigType, string name)
        {
            return animationLibrary[rigType][name].Duplicate();
        }

        public static void ImportAnimations(string path)
        {
            if (!File.Exists(animationLibraryPath))
            {
                //File.Create(animationLibraryPath);
                return;
            }

            StreamReader sr = new StreamReader(path);
            string data;
            RigAnimation animation;

            while (!sr.EndOfStream)
            {
                data = sr.ReadLine();

                if (data != null && data != "")
                {
                    animation = GetAnimationFromData(data);

                    AddAnimationToLibrary(animation);
                }
            }
            sr.Close();
        }

        static void AddAnimationToLibrary(RigAnimation animation)
        {
            animationLibrary[animation.rigType].Add(animation.name, animation);
        }

        public static void ExportLibraryToPath(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            StreamWriter sw = new StreamWriter(path);

            string[] names = Enum.GetNames(typeof(Rig.Type));
            for (int i = 0; i < names.Length; i++)
            {
                foreach (KeyValuePair<string, RigAnimation> pair in animationLibrary[(Rig.Type)i])
                {
                    sw.WriteLine(pair.Value.ExportData());
                }
            }

            sw.Close();
        }

        public string ExportData()
        {
            string ret = "";

            ret += this.rigType + " ";
            ret += this.name + " ";
            ret += this.loop + " ";
            ret += this.length + " ";
            ret += this.speed + ":";

            string[] prir = priorities.Keys.ToArray();

            for (int i = 0; i < prir.Length; i++)
            {
                ret += prir[i] + " " + priorities[prir[i]];
                if (i < prir.Length - 1)
                    ret += " ";
            }

            ret += ":";

            for (int i = 0; i < keyFrames.Count; i++)
            {
                ret += keyFrames[i].ExportData();
                if (i < keyFrames.Count)
                    ret += ";";
            }

            return ret;
        }

        public static RigAnimation GetAnimationFromData(string data)
        {
            string[] datas = data.Split(':');
            string[] animationVariables = datas[0].Split(' ');

            RigAnimation animation = new RigAnimation((Rig.Type)Enum.Parse(typeof(Rig.Type), animationVariables[0]), animationVariables[1], bool.Parse(animationVariables[2]));

            animation.length = float.Parse(animationVariables[3]);
            animation.speed = float.Parse(animationVariables[4]);

            #region Priorities
            string[] priorities = datas[1].Split(' ');

            for (int i = 0; i < priorities.Length; i += 2)
            {
                if (priorities.Length - 1 > i + 1)
                    animation.priorities.Add(priorities[i], float.Parse(priorities[i + 1]));
            }
            #endregion

            #region KeyFrames
            string[] keyframes = datas[2].Split(';');
            for (int i = 0; i < keyframes.Length; i++)
            {
                if (keyframes[i] != null && keyframes[i] != "")
                    animation.keyFrames.Add(KeyFrame.GetKeyFrameFromData(keyframes[i]));
            }
            #endregion

            return animation;
        }
    }
}
