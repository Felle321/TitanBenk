using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
    public class Agent : GameObject
    {
        public enum Tag
        {
            Player,
            Enemy,
            Null
        }
        public Tag tag;
        Texture2D texture = Game1.pixel;
        public Point targetTile;
        GUI.ProgressBar healthBar;
        public float level = 1;

        public float GetCurrentHealth()
        { return characterStats.currentHealth; }
        public void SetCurrentHealth(float value)
        { characterStats.currentHealth = value; }
        public void AddToCurrentHealth(float value)
        { characterStats.currentHealth += value; }

        public CharacterStats characterStats;

        /// <summary>
        /// The direction of the entity in radians
        /// </summary>
        float direction = 0f;
        Point tempTileTarget = Point.Zero;
        public State state = State.Idle;
        public enum State
        {
            Walking,
            Running,
            Idle
        }
        float walkSpeed = 5f;
        float runSpeed = 10f;
        public Path path = Path.Empty;

        public override Tag GetAgentTag()
        {
            return tag;
        }
        public virtual void SetState(State state)
        {
            Vector2 distance;
            switch (state)
            {
                case State.Idle:
                    this.state = state;
                    break;
                case State.Walking:
                    distance = new Vector2((tempTileTarget.X * World.tileSize + World.tileSize / 2) - position.X, (tempTileTarget.Y * World.tileSize + World.tileSize / 2) - position.Y);
                    movement = new Vector3(walkSpeed * (distance.X / (Math.Abs(distance.Y) + Math.Abs(distance.X))), walkSpeed * (distance.Y / (Math.Abs(distance.X) + Math.Abs(distance.Y))), 0);
                    this.state = state;
                    break;
                case State.Running:
                    distance = new Vector2((tempTileTarget.X * World.tileSize + World.tileSize / 2) - position.X, (tempTileTarget.Y * World.tileSize + World.tileSize / 2) - position.Y);
                    movement = new Vector3(runSpeed * (distance.X / (Math.Abs(distance.Y) + Math.Abs(distance.X))), runSpeed * (distance.Y / (Math.Abs(distance.X) + Math.Abs(distance.Y))), 0);
                    this.state = state;
                    break;
                default:
                    movement = Vector3.Zero;
                    this.state = state;
                    break;
            }
        }

        public virtual State GetState()
        {
            return state;
        }

        public Agent(Vector2 pos, string name, Tag tag, CharacterStats characterStats) : base(Type.Agent, name)
        {
            this.PositionXY = pos;
            this.characterStats = characterStats;
            this.tag = tag;
            size = new Point(8);
            origin = size.ScaleRet(new Vector2(.5f, .5f));
            type = Type.Agent;
            collider = new Collider.Circle(PositionXY, 0, 10, false);
            collider.heightZ = 32;
            healthBar = new GUI.ProgressBar(Rectangle.Empty, Color.Red, 1f, Game1.pixel, true, false);
        }

        public override void Update()
        {
            characterStats.Update();

            healthBar.progress = GetCurrentHealth() / characterStats.GetValue(CharacterStats.ValueType.Health);

            if(tag != Tag.Player)
                switch (state)
            {
                case (State.Walking):
                    if (currentTile == tempTileTarget)
                    {
                        movement = Vector3.Zero;
                        if (path.finished)
                            SetState(State.Idle);
                        else
                        {
                            tempTileTarget = GetNextPathTile();
                            SetState(State.Walking);
                        }

                    }
                    break;
                case (State.Running):
                    if (currentTile == tempTileTarget)
                    {
                        movement = Vector3.Zero;
                        if (path.finished)
                            SetState(State.Idle);
                        else
                        {
                            tempTileTarget = GetNextPathTile();
                            SetState(State.Running);
                        }
                    }
                    break;
                default:
                    break;
            }

            collider.SetPosition(PositionXY);
            base.Update();
        }

        public override void Draw(DrawBatch drawBatch)
        {
            if (SelectionHandler.selectedID == ID)
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, texture, SelectionHitBox, Color.Blue, DrawBatch.CalculateDepth(PositionXY));
            else if(tag == Tag.Enemy)
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, texture, SelectionHitBox, Color.Red, DrawBatch.CalculateDepth(PositionXY));
            else if(tag == Tag.Player)
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, texture, SelectionHitBox, Color.White, DrawBatch.CalculateDepth(PositionXY));

            drawBatch.DrawString(DrawBatch.DrawCall.Tag.GameObject, GetCurrentHealth().ToString(), Game1.fontDebug, PositionXY - new Vector2(10), Color.Red, DrawBatch.CalculateDepth(PositionXY));

            healthBar.drawRectangle = new Rectangle((int)(position.X - 16), (int)(position.Y + 10), 32, 8);

            healthBar.Draw(DrawBatch.DrawCall.Tag.GameObject, drawBatch, DrawBatch.CalculateDepth(PositionXY), (short)2, 0f);

            //collider.Draw(spriteBatch, 2f, Color.Cyan);
        }

        public virtual void TakeDamage(float dmg)
        {
            AddToCurrentHealth(-dmg);
        }

        public virtual void Die()
        {
        }
        public void PathFind(Terrain terrain, Point target)
        {
            targetTile = target;
            path = new Path(terrain, target, currentTile);
            WalkTowardsTile(GetNextPathTile());
        }

        public void WalkTowardsTile(Point target)
        {
            tempTileTarget = target;
            SetState(State.Walking);
        }

        public Point GetNextPathTile()
        {
            if (path.currentTarget == path.instructions.Length)
            {
                path.finished = true;
                SetState(State.Idle);
            }
            else
            {
                Point ret = path.instructions[path.currentTarget];
                path.currentTarget++;
                return ret;
            }

            return currentTile;
        }
    }
}
