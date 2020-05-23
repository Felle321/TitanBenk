using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
    public abstract class Rig
    {
        public Vector3 position;
        public Vector2 drawPos;
        public Dictionary<string, RigLimb> limbCollection = new Dictionary<string, RigLimb>();
        public Dictionary<string, RigLimbDrawer> limbDrawerCollection = new Dictionary<string, RigLimbDrawer>();
        public Dictionary<string, float> directions = new Dictionary<string, float>();
        List<string> limbs = new List<string>();
        public bool drawLines = false;
        public bool drawLimbs = true;

        public Dictionary<string, RigAnimation> activeAnimations = new Dictionary<string, RigAnimation>();
        public Dictionary<string, KeyValuePair<string, float>> prioritizedAnimations = new Dictionary<string, KeyValuePair<string, float>>();

        public DrawHandler drawHandler;

        public float scale = 1f;

        public enum Type
        {
            Humanoid,
            Arrows
        }
        public Type type;

        public bool DrawLimbs
        {
            get
            {
                return drawLimbs;
            }
            internal set
            {
                drawLimbs = value;
                string[] array = limbDrawerCollection.Keys.ToArray();
                for (int i = 0; i < array.Length; i++)
                {
                    limbCollection[array[i]].drawTexture = value;
                }
            }
        }

        public Rig(Vector3 position)
        {
            this.position = position;
            UpdateLimbPositions();
        }

        public static void LoadContent(ContentManager content)
        {
        }

        public virtual void Update()
        {
            UpdatePriorities();
            List<string> limbsToBlock = new List<string>();
            RigAnimation.KeyFrame keyFrameToApply;
            string[] animationKeys = activeAnimations.Keys.ToArray();

            for (int i = 0; i < activeAnimations.Count; i++)
            {
                limbsToBlock.Clear();

                foreach (KeyValuePair<string, KeyValuePair<string, float>> pair in prioritizedAnimations)
                {
                    if (pair.Value.Key != activeAnimations[animationKeys[i]].name)
                        limbsToBlock.Add(pair.Key);
                }

                keyFrameToApply = activeAnimations[animationKeys[i]].Update(limbsToBlock.ToArray());

                SetValues(keyFrameToApply.values.ToList());
            }

            UpdateLimbPositions();

            for (int i = 0; i < activeAnimations.Count; i++)
            {
                if (activeAnimations[animationKeys[i]].finished)
                {
                    activeAnimations.Remove(animationKeys[i]);
                    i--;
                }
            }
        }

        public void UpdatePriorities()
        {
            prioritizedAnimations.Clear();

            string[] animationKeys, priorityKeys;
            animationKeys = activeAnimations.Keys.ToArray();
            for (int i = 0; i < activeAnimations.Count; i++)
            {
                priorityKeys = activeAnimations[animationKeys[i]].priorities.Keys.ToArray();

                for (int j = 0; j < priorityKeys.Length; j++)
                {
                    if (!prioritizedAnimations.ContainsKey(priorityKeys[j]) || prioritizedAnimations[priorityKeys[j]].Value > activeAnimations[animationKeys[i]].priorities[priorityKeys[j]])
                    {
                        prioritizedAnimations.Add(priorityKeys[j], new KeyValuePair<string, float>(activeAnimations[animationKeys[i]].name, activeAnimations[animationKeys[i]].priorities[priorityKeys[j]]));
                    }
                }
            }
        }

        /// <summary>
        /// Sets both rotation and length
        /// </summary>
        public void SetValues(List<KeyValuePair<string, Vector4>> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                limbCollection[values[i].Key].rotation = (new Vector3(values[i].Value.X, values[i].Value.Y, values[i].Value.Z));
                limbCollection[values[i].Key].length = values[i].Value.W;
            }

            UpdateLimbPositions();
        }

        public void SetRotation(string key, Vector3 value)
        {
            limbCollection[key].SetRotation(value);

            UpdateLimbPositions();
        }

        public void SetRotations(List<KeyValuePair<string, Vector3>> values)
        {
            for (int i = 0; i < values.Count; i++)
            {
                limbCollection[values[i].Key].SetRotation(values[i].Value);
            }

            UpdateLimbPositions();
        }

        public void SetDirection(float direction)
        {
            SetDirection(limbCollection.Keys.ToArray(), direction);
        }

        public void SetDirection(string limb, float direction)
        {
            directions[limb] = direction;
        }

        public void SetDirection(string[] limbs, float direction)
        {
            for (int i = 0; i < limbs.Length; i++)
            {
                SetDirection(limbs[i], direction);
            }
        }

        /// <summary>
        /// Updates every limbs position
        /// </summary>
        public void UpdateLimbPositions()
        {
            for (int i = 0; i < limbs.Count; i++)
            {
                RigLimb currentLimb = limbCollection[limbs[i]];
                limbCollection[limbs[i]].SetPosition(Vector3.Zero);
                UpdateLimbPositions(currentLimb.name);
            }
        }

        internal void AddAnimation(string name)
        {
            if (activeAnimations.ContainsKey(name))
                activeAnimations.Remove(name);
            activeAnimations.Add(name, RigAnimation.GetAnimationFromLibrary(type, name));
        }

        /// <summary>
        /// Updates the positions of all Limbs connected to given limb
        /// </summary>
        /// <param name="limb"></param>
        public void UpdateLimbPositions(string limb)
        {
            limbCollection[limb].UpdateEndPosition(scale);
            for (int i = 0; i < limbCollection[limb].limbs.Count; i++)
            {
                string currentLimb = limbCollection[limb].limbs[i];
                limbCollection[currentLimb].SetPosition(limbCollection[limb].endPosition);
                {
                    UpdateLimbPositions(currentLimb);
                }
            }
        }

        /// <summary>
        /// Adds a limb to the collection and a reference in the parent
        /// </summary>
        /// <param name="limb"></param>
        public void AddLimb(RigLimb limb)
        {
            limbCollection.Add(limb.name, limb);
            limbDrawerCollection.Add(limb.name, new RigLimbDrawer("TestSheet"));

            if (limb.parent == "Rig")
                limbs.Add(limb.name);
            else
                limbCollection[limb.parent].AddLimb(limb.name);
        }

        public void RemoveLimb(string limb)
        {
            List<string> toRemove = new List<string>();
            List<string> open = new List<string>();

            open.Add(limb);

            while (open.Count > 0)
            {
                open.AddRange(limbCollection[open[0]].limbs);
                toRemove.Add(open[0]);
                open.RemoveAt(0);
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                limbCollection.Remove(toRemove[i]);
            }

            limbs.Remove(limb);
        }

        public virtual void Draw(DrawBatch drawBatch, float orientation, string selectedLimb, Vector2 offset, float depth)
        {
            drawPos = new Vector2(position.X, position.Y);

            List<float> internalDepths = new List<float>();
            List<string> internalKeys = new List<string>();
            int i;
            float depthToAdd;
            foreach (var item in limbDrawerCollection)
            {
                i = 0;
                depthToAdd = limbCollection[item.Key].GetInternalDepth(orientation);

                if (internalDepths.Count == 0)
                {
                    internalDepths.Add(depthToAdd);
                    internalKeys.Add(item.Key);
                }
                else
                {
                    while (i < internalDepths.Count && depthToAdd >= internalDepths[i])
                    {
                        i++;
                    }
                    internalDepths.Insert(i, depthToAdd);
                    internalKeys.Insert(i, item.Key);
                }
            }

            for (i = 0; i < internalDepths.Count; i++)
            {
                limbDrawerCollection[internalKeys[i]].Draw(drawLines, drawPos + offset, limbCollection[internalKeys[i]], drawBatch, depth, orientation);
            }

            /*

            spriteBatch.Draw(Game1.pixel, new Rectangle((int)position.X, (int)position.Y, 4, 4), Color.AliceBlue);

            Dictionary<string, Vector4> lines = Get2DLines(orientation);
            Vector4 value;

            foreach (KeyValuePair<string, Vector4> pair in lines)
            {
                value = new Vector4(pair.Value.X + drawPos.X + offset.X, pair.Value.Y + drawPos.Y + offset.Y, pair.Value.Z + drawPos.X + offset.X, pair.Value.W + drawPos.Y + offset.Y);
                if(selectedLimb == pair.Key)
                    Game1.DrawLine(spriteBatch, value, 2f, Color.Red, 0f);
                else
                    Game1.DrawLine(spriteBatch, value, 2f, Color.Black, 0f);
            }
            */
        }

        public virtual Dictionary<string, Vector4> Get2DLines(float orientation)
        {
            Dictionary<string, Vector4> ret = new Dictionary<string, Vector4>();
            string[] keys = limbCollection.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                ret.Add(keys[i], limbCollection[keys[i]].Get2DLine(orientation));
            }

            return ret;
        }
        public virtual Dictionary<string, Vector4> GetLimbValues()
        {
            Dictionary<string, Vector4> ret = new Dictionary<string, Vector4>();
            string[] keys = limbCollection.Keys.ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                ret.Add(keys[i], new Vector4(limbCollection[keys[i]].rotation.X, limbCollection[keys[i]].rotation.Y, limbCollection[keys[i]].rotation.Z, limbCollection[keys[i]].length));
            }

            return ret;
        }



        public class DrawHandler
        {

        }





        public class Humanoid : Rig
        {
            public Humanoid(Vector3 position) : base(position)
            {
                this.type = Type.Humanoid;
                AddLimb(new RigLimb("Neck", "Rig", new Vector3(0, (float)-Math.PI / 2, 0), 80f));


                AddLimb(new RigLimb("RightHip", "Rig", new Vector3((float)Math.PI / 2, 0, 0), 10f));
                AddLimb(new RigLimb("LeftHip", "Rig", new Vector3(-(float)Math.PI / 2, 0, 0), 10f));
                AddLimb(new RigLimb("Head", "Neck", new Vector3(0, -(float)Math.PI / 2, 0), 10f));

                AddLimb(new RigLimb("RightShoulder", "Neck", new Vector3((float)Math.PI / 2, 0, 0), 10f));
                AddLimb(new RigLimb("LeftShoulder", "Neck", new Vector3(-(float)Math.PI / 2, 0, 0), 10f));

                limbCollection["RightShoulder"].drawTexture = false;
                limbCollection["LeftShoulder"].drawTexture = false;
                limbCollection["RightHip"].drawTexture = false;
                limbCollection["LeftHip"].drawTexture = false;

                AddLimb(new RigLimb("UpperRightArm", "RightShoulder", new Vector3(0, (float)Math.PI / 2, 0), 40f));
                AddLimb(new RigLimb("LowerRightArm", "UpperRightArm", new Vector3(0, (float)Math.PI / 2, 0), 30f));
                AddLimb(new RigLimb("UpperLeftArm", "LeftShoulder", new Vector3(0, (float)Math.PI / 2, 0), 40f));
                AddLimb(new RigLimb("LowerLeftArm", "UpperLeftArm", new Vector3(0, (float)Math.PI / 2, 0), 30f));

                AddLimb(new RigLimb("UpperRightLeg", "RightHip", new Vector3(0, (float)Math.PI / 2, 0), 40f));
                AddLimb(new RigLimb("LowerRightLeg", "UpperRightLeg", new Vector3(0, (float)Math.PI / 2, 0), 30f));
                AddLimb(new RigLimb("UpperLeftLeg", "LeftHip", new Vector3(0, (float)Math.PI / 2, 0), 40f));
                AddLimb(new RigLimb("LowerLeftLeg", "UpperLeftLeg", new Vector3(0, (float)Math.PI / 2, 0), 30f));


                base.Update();
            }
        }

        public class Arrows : Rig
        {
            public Arrows(Vector3 position) : base(position)
            {
                this.type = Type.Arrows;

                AddLimb(new RigLimb("X", "Rig", new Vector3(0, 0, 0), 45f));
                AddLimb(new RigLimb("Y", "Rig", new Vector3(0, (float)Math.PI / 2, 0), 45f));
                AddLimb(new RigLimb("Z", "Rig", new Vector3((float)Math.PI / 2, 0, 0), 45f));

                base.Update();
            }

            public override void Draw(DrawBatch drawBatch, float orientation, string selectedLimb, Vector2 offset, float depth)
            {
                drawPos = new Vector2(position.X, position.Y);

                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject ,Game1.pixel, new Rectangle((int)position.X, (int)position.Y, 4, 4), Color.AliceBlue, DrawBatch.CalculateDepth(new Vector2(position.X, position.Y)));

                Dictionary<string, Vector4> lines = Get2DLines(orientation);
                Vector4 value;

                foreach (KeyValuePair<string, Vector4> pair in lines)
                {
                    value = new Vector4(pair.Value.X + drawPos.X + offset.X, pair.Value.Y + drawPos.Y + offset.Y, pair.Value.Z + drawPos.X + offset.X, pair.Value.W + drawPos.Y + offset.Y);
                    if ("X" == pair.Key)
                        Game1.DrawLine(drawBatch, value, 3f, Color.Blue, 0f);
                    else if ("Y" == pair.Key)
                        Game1.DrawLine(drawBatch, value, 3f, Color.Red, 0f);
                    else if ("Z" == pair.Key)
                        Game1.DrawLine(drawBatch, value, 3f, Color.Green, 0f);
                }
            }

        }
    }
}
