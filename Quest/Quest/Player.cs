using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Quest
{
    public class Player : Agent
    {
        float acceleration = .1f;
        float deAcceleration = .75f;
        float deAccelerationAboveTopSpeed = .9f;
        float topSpeed = 3.2f;
        float dashSpeed = 10f;
        public bool[] hotBarInputs = new bool[] { false, false, false, false, false, false, false, false, false };

        //RIG
        Rig rig;

        /// <summary>
        /// -1 if character is availible
        /// </summary>
        public int currentlyCastingAbility = -1;

        public Ability.Type[] hotBar = new Ability.Type[9];

        public Ability.AbilityHandler abilityHandler = new Ability.AbilityHandler();
        float exp = 232;
        public float requiredExp = 1000;

        float orientation = 0f;

        public float EXP
        {
            get { return exp; }
            set 
            {
                if (value >= requiredExp)
                {
                    exp = value - requiredExp;
                    LevelUp();
                }
                else
                    exp = value;
            }
        }

        private void LevelUp()
        {
            level++;
        }

        public Player(Vector2 pos) : base(pos, "Player", Tag.Player, CharacterStats.Default)
        {
            for (int i = 0; i < hotBar.Length; i++)
            {
                hotBar[i] = Ability.Type.Null;
            }

            hotBar[0] = Ability.Type.Fireball;
            hotBar[1] = Ability.Type.IronBall;

            rig = new Rig.Humanoid(new Vector3(pos, 16));
            rig.AddAnimation("running");
            rig.drawLines = true;
            rig.scale = .25f;
        }

        public void UpdateInputs()
        {
            for (int i = 0; i < hotBarInputs.Length; i++)
            {
                hotBarInputs[i] = false;
            }

            #region movement
            bool keyW = Game1.KeyDown(Controls.keys[Controls.KeyBind.MoveUp]);
            bool keyA = Game1.KeyDown(Controls.keys[Controls.KeyBind.MoveLeft]);
            bool keyS = Game1.KeyDown(Controls.keys[Controls.KeyBind.MoveDown]);
            bool keyD = Game1.KeyDown(Controls.keys[Controls.KeyBind.MoveRight]);

            if (keyA)
            {
                if (movement.X > -topSpeed)
                    movement.X += (-topSpeed - movement.X) * acceleration;
                else if((movement.X < -topSpeed))
                    movement.X *= deAccelerationAboveTopSpeed;
            }
            else if (keyD)
            {
                if (movement.X < topSpeed)
                    movement.X += (topSpeed - movement.X) * acceleration;
                else if ((movement.X > topSpeed))
                    movement.X *= deAccelerationAboveTopSpeed;
            }
            else
                movement.X *= deAcceleration;

            if (keyW)
            {
                if (movement.Y > -topSpeed)
                    movement.Y += (-topSpeed - movement.Y) * acceleration;
                else if ((movement.Y < -topSpeed))
                    movement.Y *= deAccelerationAboveTopSpeed;
            }
            else if (keyS)
            {
                if (movement.Y < topSpeed)
                    movement.Y += (topSpeed - movement.Y) * acceleration;
                else if ((movement.Y > topSpeed))
                    movement.Y *= deAccelerationAboveTopSpeed;
            }
            else
                movement.Y *= deAcceleration;

            if (keyA || keyS || keyD || keyW)
            {
                rig.activeAnimations["running"].speed = 0.025f;

                if (keyA)
                {
                    if (keyW)
                    {
                        orientation = .75f * 3.14f;
                    }
                    else if (keyS)
                    {
                        orientation = 1.25f * 3.14f;
                    }
                    else
                    {
                        orientation = 3.14f;
                    }
                }
                else if (keyD)
                {
                    if (keyW)
                    {
                        orientation = .25f * 3.14f;
                    }
                    else if (keyS)
                    {
                        orientation = 1.75f * 3.14f;
                    }
                    else
                    {
                        orientation = 0f;
                    }
                }
                else
                {
                    if (keyW)
                    {
                        orientation = .5f * 3.14f;
                    }
                    else if (keyS)
                    {
                        orientation = 1.5f * 3.14f;
                    }
                }
            }
            else
                rig.activeAnimations["running"].speed = 0f;
            #endregion

            if (Game1.MouseButtonPressedLeft())
            {

            }

            #region Dash
            if (Game1.KeyPressed(Controls.keys[Controls.KeyBind.Dash]))
            {
                if (keyA)
                {
                    if (keyW)
                    {
                        movement = new Vector2(-.7f * dashSpeed, -.7f * dashSpeed).ToVector3(0);
                    }
                    else if (keyS)
                    {
                        movement = new Vector2(-.7f * dashSpeed, .7f * dashSpeed).ToVector3(0);
                    }
                    else
                    {
                        movement.X = -dashSpeed;
                    }
                }
                else if (keyD)
                {
                    if (keyW)
                    {
                        movement = new Vector2(.7f * dashSpeed, -.7f * dashSpeed).ToVector3(0);
                    }
                    else if (keyS)
                    {
                        movement = new Vector2(.7f * dashSpeed, .7f * dashSpeed).ToVector3(0);
                    }
                    else
                    {
                        movement.X = dashSpeed;
                    }
                }
                else
                {
                    if (keyW)
                    {
                        movement = new Vector2(0, -.7f * dashSpeed).ToVector3(0);
                    }
                    else if (keyS)
                    {
                        movement = new Vector2(0, .7f * dashSpeed).ToVector3(0);
                    }
                    //else
                        //movement = Extensions.GetVector2((Game1.mousePosInWorld - (origin.ToVector2() + PositionXY)).GetAngle(), dashSpeed).ToVector3(0);
                }
            }
            #endregion

            #region Abilities
            if (Game1.KeyPressed(Controls.keys[Controls.KeyBind.Ability1]))
                hotBarInputs[0] = true;
            if (Game1.KeyPressed(Controls.keys[Controls.KeyBind.Ability2]))
                hotBarInputs[1] = true;

            for (int i = 0; i < hotBarInputs.Length; i++)
            {
                if(hotBarInputs[i])
                {
                    if(TryCastAbility(i))
                    {
                        characterStats.currentMana -= Ability.abilities[hotBar[currentlyCastingAbility]].GetManaCost(abilityHandler.executers[hotBar[currentlyCastingAbility]].level);
                        characterStats.currentEnergy-= Ability.abilities[hotBar[currentlyCastingAbility]].GetEnergyCost(abilityHandler.executers[hotBar[currentlyCastingAbility]].level);
                    }
                }
            }
            #endregion
        }

        public override void Update()
        {
            if (currentlyCastingAbility >= 0 && abilityHandler.executers[hotBar[currentlyCastingAbility]].Ready)
                currentlyCastingAbility = -1;

            UpdateInputs();
            abilityHandler.Update(new Ability.ExecutionParamaters(position - new Vector3(0, 0, 8), (Game1.mousePosInWorld - PositionXY).GetAngle(), Tag.Player), 1, 1);
            base.Update();

            rig.position = new Vector3(PositionXY, 16);
            rig.Update();
        }

        bool TryCastAbility(int hotbarIndex)
        {
            if (currentlyCastingAbility == -1 
                && characterStats.currentMana >= Ability.abilities[hotBar[hotbarIndex]].GetManaCost(1)
                && characterStats.currentEnergy >= Ability.abilities[hotBar[hotbarIndex]].GetEnergyCost(1))
            {
                abilityHandler.ActivateAbility(hotBar[hotbarIndex], 1f);
                currentlyCastingAbility = hotbarIndex;
                return true;
            }
            return false;
        }

        public override void Draw(DrawBatch drawBatch)
        {
            rig.Draw(drawBatch, orientation, "Neck", new Vector2(0, -16), DrawBatch.CalculateDepth(PositionXY));
            base.Draw(drawBatch);
        }
    }
}
