using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quest
{
    public abstract class Ability
    {
        public static int iconSize = 64;
        public static Texture2D iconSheet;

        public static void LoadContent(ContentManager content)
        {
            iconSheet = content.Load<Texture2D>("Ability\\Icons");
        }

        #region Classes
        public struct ExecutionParamaters
        {
            public Vector3 position;
            public float direction;
            public Agent.Tag agentToIgnore;

            public ExecutionParamaters(Vector3 position, float direction, Agent.Tag agentToIgnore)
            {
                this.position = position;
                this.direction = direction;
                this.agentToIgnore = agentToIgnore;
            }
        }

        public static Rectangle GetIconSourceRectangle(Type type)
        {
            return new Rectangle(((((int)type) - 1) * iconSize) % iconSheet.Width, (((((int)type) - 1) * iconSize) / iconSheet.Width) * 64, iconSize, iconSize);
        }

        /// <summary>
        /// Executes the ability and allows the ability to be "instantiaded"
        /// </summary>
        public class Executer
        {
            public Type type = Type.Null;
            float timer = 0;
            bool executed = false;
            float coolDown = 0;
            bool activated = false;
            public float level = 1;

            public float CastingProgress
            {
                get { return timer / Ability.abilities[type].GetExecutingTime(level); }
            }
            public float CoolDownProgress
            {
                get { return coolDown / Ability.abilities[type].GetCoolDownTime(level); }
            }

            public bool Ready
            {
                get { return (!activated); }
            }

            public Executer(Type type)
            {
                this.type = type;
            }

            public void Activate(float level)
            {
                this.level = level;
                activated = true;
            }

            public void Update(ExecutionParamaters executionParamaters, float castingTimeRate, float coolDownRate)
            {
                if (activated)
                {
                    timer += castingTimeRate;
                    if (timer >= abilities[type].GetExecutingTime(level))
                    {
                        if (executed)
                        {
                            if (coolDown <= 0)
                                Reset();
                            else
                                coolDown -= coolDownRate;
                        }
                        else
                            Execute(executionParamaters);
                    }
                }
            }
            /// <summary>
            /// Draws an Icon inside the confines of given rectangle, applying state-controlled colors and effects
            /// </summary>
            /// <param name="spriteBatch"></param>
            /// <param name="rectangle"></param>
            /// <param name="color"></param>
            public void DrawIcon(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
            {
                if (activated)
                {
                    if (executed)
                    {
                        if (color == null || color == Color.White)
                            color = Color.Lerp(Color.White, Color.Blue, CoolDownProgress);
                        else
                            color = Color.Lerp(color, Color.Blue, CoolDownProgress);
                    }
                    else
                    {
                        if (color == null || color == Color.White) 
                            color = Color.Lerp(Color.White, Color.Black, CastingProgress);
                        else
                            color = Color.Lerp(color, Color.Black, CastingProgress);
                    }
                }
                else
                    color = Color.White;

                spriteBatch.Draw(iconSheet, rectangle, GetIconSourceRectangle(type), color);
            }

            void Execute(ExecutionParamaters executionParamaters)
            {
                Ability.Execute(type, executionParamaters, level);
                executed = true;
                coolDown = abilities[type].GetCoolDownTime(level);
            }

            void Reset()
            {
                activated = false;
                coolDown = 0;
                timer = 0;
                executed = false;
            }
        }

        public class AbilityHandler
        {
            public Dictionary<Type, Executer> executers = new Dictionary<Type, Executer>();

            public void DrawIcon(Type type, SpriteBatch spriteBatch, Rectangle rectangle, Color color)
            {
                if (executers.ContainsKey(type))
                {
                    executers[type].DrawIcon(spriteBatch, rectangle, color);
                }
            }

            public void Update(ExecutionParamaters executionParamaters, float castingTimeRate, float coolDownRate)
            {
                foreach (var item in executers)
                {
                    item.Value.Update(executionParamaters, castingTimeRate, coolDownRate);
                }
            }

            public void ActivateAbility(Type type, float level)
            {
                if (!executers.ContainsKey(type))
                    executers.Add(type, new Executer(type));
                else if(!executers[type].Ready)
                    return;

                executers[type].Activate(level);
            }
        }

        public static Dictionary<Type, Ability> abilities = new Dictionary<Type, Ability>();
        public enum Type
        {
            Null,
            Fireball,
            IronBall
        }

        public static void Initialize()
        {
            Type[] types = (Type[])Enum.GetValues(typeof(Type));
            for (int i = 0; i < types.Length; i++)
            {
                Ability abilityToAdd = Ability.Empty;
                switch (types[i])
                {
                    case Type.Fireball:
                        abilityToAdd = new Ability.FireBall();
                        break;
                    case Type.IronBall:
                        abilityToAdd = new Ability.IronBall();
                        break;
                }
                abilities.Add(types[i], abilityToAdd);
            }
        }

        public static Ability Empty
        {
            get { return new Ability.Null(); }
        }
        #endregion

        //Virtual Class

        public Type type;
        public virtual float GetExecutingTime(float level)
        {
            return level * 30;
        }
        public virtual float GetCoolDownTime(float level)
        {
            return level * 30;
        }
        public virtual float GetManaCost(float level)
        {
            return level * 10;
        }
        public virtual float GetEnergyCost(float level)
        {
            return level * 10;
        }

        public static void Execute(Type type, ExecutionParamaters parameters, float level)
        {
            switch (type)
            {
                case Type.Fireball:
                    ((FireBall)abilities[type]).Execute(parameters, level);
                    break;
                case Type.IronBall:
                    ((IronBall)abilities[type]).Execute(parameters, level);
                    break;
                default:
                    return;
            }
        }

        public class Null : Ability
        {
            public Null()
            {
                type = Type.Null;
            }
        }

        public class FireBall : Ability
        {
            float GetSpeed(float level)
            {
                return level * 7f;
            }

            public FireBall()
            {
                type = Type.Fireball;
            }

            public void Execute(ExecutionParamaters parameters, float level)
            {
                Game1.gameObjects.Add(new Projectile.Fireball(parameters.position, Extensions.GetVector2(parameters.direction, GetSpeed(level)).ToVector3(0), parameters.agentToIgnore));
            }
        }
        public class IronBall : Ability
        {
            public IronBall()
            {
                type = Type.IronBall;
            }
            float GetSpeed(float level)
            {
                return level * 7f;
            }

            public void Execute(ExecutionParamaters parameters, float level)
            {
                Game1.gameObjects.Add(new Projectile.BouncingBall(parameters.position, Extensions.GetVector2(parameters.direction, GetSpeed(level)).ToVector3(0), parameters.agentToIgnore));
            }
        }
    }
}
