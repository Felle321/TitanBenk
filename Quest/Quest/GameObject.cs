using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
    public abstract class GameObject
    {
        /// <summary>
        /// Actual position in the game world
        /// </summary>
        public Vector3 position = Vector3.Zero;
        /// <summary>
        /// Same as position but the previous frame
        /// </summary>
        public Vector3 positionPrev = Vector3.Zero;
        /// <summary>
        /// Velocity in world units
        /// </summary>
        public Vector3 movement = Vector3.Zero;
        /// <summary>
        /// A rectangle surrounding the object for selections
        /// </summary>
        Rectangle selectionHitbox = Rectangle.Empty;
        /// <summary>
        /// A rectangle surrounding the object for selections
        /// </summary>
        public Rectangle SelectionHitBox
        {
            get { return selectionHitbox; }
        }
        /// <summary>
        /// Name of the GameObject
        /// </summary>
        public string name = "";
        public Point origin = Point.Zero;
        /// <summary>
        /// Used mainly for SelectionHitbox
        /// </summary>
        public Point size = Point.Zero;
        public Type type = Type.Null;
        public Collider collider;
        private bool ground = false;
        public bool OnGround
        {
            get
            {
                return ground;
            }
            set
            {
                ground = value;
            }
        }
        public List<Type> collisionTypesToIgnore = new List<Type>();
        public List<Agent.Tag> agentTagsToIgnore = new List<Agent.Tag>();
        public bool usePhysics = false;
        public bool collideWithTiles = false;
        public bool remove = false;
        public bool useGravity = true;

        public float bounceFactor = .9f, friction = .9f, airResistance = .95f;

        public enum Type
        {
            Null,
            Agent,
            Projectile,
            Particle,
            DroppedItem
        }
        int id;
        public int ID
        {
            get { return id; }
        }

        public Point currentTile = Point.Zero;
        public Point CurrentTile
        {
            get { return currentTile; }
        }

        public Vector2 PositionXY { get { return new Vector2(position.X, position.Y); } set { position.X = value.X; position.Y = value.Y; } }
        public Vector2 MovementXY { get { return new Vector2(movement.X, movement.Y); } set { movement.X = value.X; movement.Y = value.Y; } }

        public struct Collision
        {
            public Type type;
            public int ID;

            public Collision(Type type, int id)
            {
                this.type = type;
                this.ID = id;
            }
        }

        /// <summary>
        /// All the found collisions this frame
        /// </summary>
        public List<Collision> collisions = new List<Collision>();

        public GameObject(Type tag, string name)
        {
            this.type = tag;
            this.name = name;

            if (tag == Type.Agent)
                id = Game1.NextIDAgent;
            else
                id = Game1.NextIDGameObject;
        }

        public virtual void Update()
        {
            positionPrev = position;
            
            if (usePhysics)
            {
                PhysicsUpdate();
            }
            else
            {
                position += movement;
            }

            if (Math.Abs(movement.X) < .1f)
                movement.X = 0;
            if (Math.Abs(movement.Y) < .1f)
                movement.Y = 0;
            if (Math.Abs(movement.Z) < .1f)
                movement.Z = 0;

            selectionHitbox = new Rectangle((int)Math.Round(position.X) - origin.X, (int)Math.Round(position.Y) - origin.Y, size.X, size.Y);

            if (SelectionHandler.TrySelect && selectionHitbox.Contains(Game1.mousePosInWorld))
            {
                SelectionHandler.SelectGameObject(id, type);
            }

            currentTile = Terrain.WorldPosToTile(PositionXY.ToPoint());

            if(collider != null)
                UpdateColliderPosition();
        }

        public virtual void UpdateColliderPosition()
        {
            collider.Position = PositionXY;
            collider.positionZ = position.Z;
        }

        public virtual void Draw(DrawBatch drawBatch)
        {

        }
        public virtual void PhysicsUpdate()
        {
            position += movement;
            if(!OnGround)
            {
                if(useGravity)
                    movement.Z -= World.gravity;

                position.Z += movement.Z;
                if(position.Z < 0)
                {
                    if (-movement.Z < .1)
                    {
                        OnGround = true;
                        position.Z = 0;
                        movement.Z = 0;
                    }
                    else
                    {
                        GroundCollisionTrigger();
                        position.Z = -position.Z;
                        movement.Z *= -bounceFactor;
                    }
                }

                movement.Z *= airResistance;
                movement *= airResistance;
            }
            else
            {
                movement *= friction;
            }

            GetCollisions();

            CollisionResolve();
        }

        public virtual void GetCollisions()
        {
            collisions.Clear();

            if (collider == null)
                return;

            for (int i = 0; i < Game1.gameObjects.Count; i++)
            {
                if ((type == Type.Agent || Game1.gameObjects[i].id != id) && !collisionTypesToIgnore.Contains(Game1.gameObjects[i].type) && collider.CollisionCheck(Game1.gameObjects[i].collider, true))
                { 
                        collisions.Add(new Collision(Game1.gameObjects[i].type, Game1.gameObjects[i].ID));
                }
            }

            if(!collisionTypesToIgnore.Contains(Type.Agent))
                for (int i = 0; i < Game1.agents.Count; i++)
                {
                    if((type != Type.Agent || Game1.agents[i].id != id) && !agentTagsToIgnore.Contains(Game1.agents[i].tag) && collider.CollisionCheck(Game1.agents[i].collider, true))
                        collisions.Add(new Collision(Game1.agents[i].type, Game1.agents[i].ID));
                }
        }

        /// <summary>
        /// Gets triggered everytime the object COLLIDES with the ground.
        /// Not when OnGround is true
        /// </summary>
        public virtual void GroundCollisionTrigger()
        {

        }

        public virtual void CollisionResolve()
        {

        }

        public virtual Agent.Tag GetAgentTag()
        {
            return Agent.Tag.Null;
        }
    }
}
