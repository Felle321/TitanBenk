using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Quest
{
    public abstract class Collider
    {
        public enum Type
        {
            Circle,
            Point,
            AABB,
            Line
        }
        public Type type;
        public Vector2 Position
        {
            get { return new Vector2(vector4.X, vector4.Y); }
            set { vector4.X = value.X; vector4.Y = value.Y; }
        }
        Vector4 vector4;
        Vector2 measurements;
        bool terrainCollision;
        Vector2 activeArea;
        bool useAngles;
        public float positionZ = 0, heightZ = 1;


        public float Width
        {
            get
            {
                switch (type)
                {
                    case Type.Circle:
                        return Radius * 2;
                    case Type.Point:
                        return 1;
                    case Type.AABB:
                        return vector4.Z;
                    case Type.Line:
                        return vector4.Z - vector4.X;
                    default:
                        return -1;
                }
            }
        }
        public float Height
        {
            get
            {
                switch (type)
                {
                    case Type.Circle:
                        return Radius * 2;
                    case Type.Point:
                        return 1;
                    case Type.AABB:
                        return vector4.W;
                    case Type.Line:
                        return vector4.W - vector4.Y;
                    default:
                        return -1;
                }
            }
        }

        /// <summary>
        /// Provided that this object is of the type cicle. Returns the squared value of the radius
        /// </summary>
        public float RadiusSqrd
        {
            get
            {
                if (type == Type.Circle)
                    return vector4.Z * vector4.Z;
                else if (type == Type.Point)
                    return 1;
                else
                    return 0;
            }
        }
        /// <summary>
        /// Provided that this object is of the type cicle. Returns the value of the radius
        /// </summary>
        public float Radius
        {
            get
            {
                if (type == Type.Circle)
                    return vector4.Z;
                else if (type == Type.Point)
                    return 1;
                else
                    return 0;
            }
            set
            {
                if (type == Type.Circle)
                    vector4.Z = value;
            }
        }

        public Vector4 GetVector4()
        { return vector4; }

        /// <summary>
        /// Creates a new collider, specifying if it collides with the terrain and the position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="collideWithTerrain">If true will collide with tileobjects and othe occupied tiles on the map</param>
        public Collider(Vector2 position, bool collideWithTerrain)
        {
            this.Position = position;
            this.terrainCollision = collideWithTerrain;
        }
        /// <summary>
        /// Creates a new collider, specifying if it collides with the terrain and the position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="collideWithTerrain">If true will collide with tileobjects and othe occupied tiles on the map</param>
        public Collider(Vector2 position,float positionZ, bool collideWithTerrain)
        {
            this.Position = position;
            this.positionZ = positionZ;
            this.terrainCollision = collideWithTerrain;
        }

        /// <summary>
        /// In the case of a line it represents the secondary point, in case of an AABB it represents the measurements
        /// </summary>
        /// <param name="vector"></param>
        public virtual void SetSecondaryValues(Vector2 vector)
        {
            switch (type)
            {
                case Type.Circle:
                    break;
                case Type.Point:
                    break;
                case Type.AABB:
                    vector4.Z = vector.X;
                    vector4.W = vector.Y;
                    break;
                case Type.Line:
                    vector4.Z = vector.X;
                    vector4.W = vector.Y;
                    break;
                default:
                    break;
            }
        }
        public virtual void SetPosition(Vector2 vector)
        {
            this.Position = vector;
        }

        /// <summary>
        /// Only works for Circle type. Sets the radius of the collider
        /// </summary>
        /// <param name="radius"></param>
        public virtual void SetRadius(float radius)
        {
            if (type == Type.Circle)
                this.vector4.Z = radius;
        }

        internal virtual void SetActiveAreaCircle(float f0, float f1)
        {
            if (type != Type.Circle)
                return;

            activeArea.X = f0.NormalizeRadians();
            activeArea.Y = f1.NormalizeRadians();
            useAngles = true;

            if (activeArea.X > activeArea.Y)
            {
                float temp = activeArea.Y;
                activeArea.Y = activeArea.X;
                activeArea.X = temp;
            }
        }

        internal virtual void ClearActiveAreaCircle()
        {
            activeArea = new Vector2(float.NaN);
            useAngles = false;
        }

        /// <summary>
        /// Returns true if collision is true on Z-axis
        /// </summary>
        /// <param name="z"></param>
        /// <param name="zHeight"></param>
        /// <returns></returns>
        public virtual bool CollisionCheckZ(float z, float zHeight)
        {
            return ((z < positionZ && z + zHeight > positionZ + heightZ)
                || (z >= positionZ && z < positionZ + zHeight)
                || (z + zHeight >= positionZ && z + zHeight < positionZ + heightZ));
        }

        /// <summary>
        /// Returns true if it collides with the given collider
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="includeZ">Determines if the function includes or ignores the Z axis</param>
        /// <returns></returns>
        public virtual bool CollisionCheck(Collider collider, bool includeZ)
        {
            if (collider.type == Type.Point)
                return CollisionCheckPoint(collider.Position);
            return false;
        }

        public virtual void Draw(DrawBatch drawBatch, float Thickness, Color color)
        {

        }
        public virtual bool CollisionCheckPoint(Vector2 position)
        {
            return false;
        }
        public virtual bool CollisionCheckLine(Vector4 a)
        {
            return false;
        }

        public static bool CollisionCheckLines(Vector4 a, Vector4 b)
        {
            return CollisionCheckLines(new Vector2(a.X, a.Y), new Vector2(a.Z, a.W), new Vector2(b.X, b.Y), new Vector2(b.Z, b.W));
        }

        public static bool RectangleContainsPoint(Vector4 rectangle, Vector2 point)
        {
            return (point.X < rectangle.X + rectangle.Z && point.X >= rectangle.X && point.Y < rectangle.Y + rectangle.W && point.Y >= rectangle.Y);
        }

        public static bool CollisionCheckLines(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
            float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            // Detect coincident lines (has a problem, read below)
            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }

        public static bool CollisionCheckLinePoint(Vector4 v, Vector2 p)
        {

            // get distance from the point to the two ends of the line
            float d1 = new Vector2(v.X - p.X, v.Y - p.Y).Length();
            float d2 = new Vector2(v.Z - p.X, v.W - p.Y).Length();

            // get the length of the line
            float lineLen = new Vector2(v.Z - v.X, v.W - v.Y).Length();

            // since floats are so minutely accurate, add
            // a little buffer zone that will give collision
            float buffer = 0.1f;    // higher # = less accurate

            // if the two distances are equal to the line's
            // length, the point is on the line!
            // note we use the buffer here to give a range,
            // rather than one #
            if (d1 + d2 >= lineLen - buffer && d1 + d2 <= lineLen + buffer)
            {
                return true;
            }
            return false;
        }

        public class Circle : Collider
        {

            /// <summary>
            /// Creates a new collider, specifying if it collides with the terrain and the position
            /// </summary>
            /// <param name="position"></param>
            /// <param name="radius">The radius of the circle</param>
            /// <param name="collideWithTerrain">If true will collide with tileobjects and othe occupied tiles on the map</param>
            public Circle(Vector2 position, float positionZ, float radius, bool collideWithTerrain) : base(position, collideWithTerrain)
            {
                this.vector4.Z = radius;
                this.positionZ = positionZ;
                useAngles = false;
            }

            internal Vector2 GetPosition()
            {
                return Position;
            }

            /// <summary>
            /// Creates a new collider, specifying if it collides with the terrain and the position
            /// </summary>
            /// <param name="position"></param>
            /// <param name="radius">The radius of the circle</param>
            /// <param name="collideWithTerrain">If true will collide with tileobjects and othe occupied tiles on the map</param>
            /// <param name="activeArea">The active area represented by two angles in radians</param>
            public Circle(Vector2 position, float radius, bool collideWithTerrain, Vector2 activeArea) : base(position, collideWithTerrain)
            {
                this.vector4.Z = radius;
                SetActiveAreaCircle(activeArea.X, activeArea.Y);
            }

            public override void Draw(DrawBatch drawBatch, float thickness, Color color)
            {
                Vector2 first, second;

                if (!useAngles)
                    for (int i = 0; i < 10; i++)
                    {
                        first = Position + Extensions.GetVector2(i * 2 * (float)Math.PI / 10, vector4.Z);
                        second = Position + Extensions.GetVector2((i + 1) * 2 * (float)Math.PI / 10, vector4.Z);

                        Game1.DrawLine(drawBatch, first, second, thickness, color, DrawBatch.CalculateDepth(Position));
                    }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        float activeLength = activeArea.Y - activeArea.X;

                        first = Position + Extensions.GetVector2(activeArea.X + i * activeLength / 10, vector4.Z);
                        second = Position + Extensions.GetVector2(activeArea.X + (i + 1) * activeLength / 10, vector4.Z);

                        Game1.DrawLine(drawBatch, first, second, thickness, color, DrawBatch.CalculateDepth(Position));
                    }

                    Game1.DrawLine(drawBatch, Position, Position + Extensions.GetVector2(activeArea.X, vector4.Z), thickness, color, DrawBatch.CalculateDepth(Position));
                    Game1.DrawLine(drawBatch, Position, Position + Extensions.GetVector2(activeArea.Y, vector4.Z), thickness, color, DrawBatch.CalculateDepth(Position));
                }
            }

            /// <summary>
            /// Returns true if the collider collides this object
            /// </summary>
            /// <param name="collider"></param>
            /// <param name="includeZ">Determines if the z-axis is accounted for</param>
            /// <returns></returns>
            public override bool CollisionCheck(Collider collider, bool includeZ)
            {
                if (collider == null)
                    return false;

                if (includeZ)
                    if (!CollisionCheckZ(collider.positionZ, collider.heightZ))
                        return false;

                Vector2 delta = new Vector2(Position.X - collider.Position.X, Position.Y - collider.Position.Y);
                float length = delta.Length();

                switch (collider.type)
                {
                    case Type.Circle:
                        if (length > Radius + collider.Radius)
                            return false;

                        Circle circleCollider = Circle.Parse(collider);
                        if (useAngles || collider.useAngles)
                        {
                            Vector2 intersection0, intersection1;
                            float angle00, angle01, angle10 = 0, angle11 = 0;
                            int amountOfIntersections = FindCircleCircleIntersections(Position, Radius, collider.Position, collider.Radius, out intersection0, out intersection1);

                            if (collider.CollisionCheckPoint(Position) || CollisionCheckPoint(collider.Position))
                                return true;

                            //If angles are used by both we can speed up the collision checking by checking for collisions with the edges
                            if (useAngles && collider.useAngles)
                            {
                                Vector4 edgeLineA0 = Extensions.GetVector4(Position, Position + Extensions.GetVector2(activeArea.X, Radius));
                                Vector4 edgeLineA1 = Extensions.GetVector4(Position, Position + Extensions.GetVector2(activeArea.Y, Radius));

                                Vector4 edgeLineB0 = Extensions.GetVector4(collider.Position, collider.Position + Extensions.GetVector2(collider.activeArea.X, collider.Radius));
                                Vector4 edgeLineB1 = Extensions.GetVector4(collider.Position, collider.Position + Extensions.GetVector2(collider.activeArea.Y, collider.Radius));

                                if (CollisionCheckLine(edgeLineB0) || CollisionCheckLine(edgeLineB1)
                                || collider.CollisionCheckLine(edgeLineA0) || collider.CollisionCheckLine(edgeLineA1))
                                    return true;

                                //if (CollisionCheckLines(edgeLineA0, edgeLineB0) || CollisionCheckLines(edgeLineA1, edgeLineB0)
                                //    || CollisionCheckLines(edgeLineA0, edgeLineB1) || CollisionCheckLines(edgeLineA1, edgeLineB1))
                                //    return true;

                                //if (collider.CollisionCheckPoint(new Vector2(edgeLineA0.X, edgeLineA0.Y)) || collider.CollisionCheckPoint(new Vector2(edgeLineA1.X, edgeLineA1.Y))
                                //    || CollisionCheckPoint(new Vector2(edgeLineB0.X, edgeLineB0.Y)) || CollisionCheckPoint(new Vector2(edgeLineB1.X, edgeLineB1.Y)))
                                //    return true;
                            }


                            if (amountOfIntersections <= 0)
                            {
                                break;
                            }
                            else
                            {
                                if (useAngles)
                                {
                                    //Compared to this circle
                                    angle00 = (intersection0 - Position).GetAngle();

                                    if (AngleInsideActiveArea(angle00))
                                    {
                                        angle00 = -1;
                                    }

                                    if (amountOfIntersections > 1)
                                    {
                                        //Compared to this circle
                                        angle10 = (intersection1 - Position).GetAngle();

                                        if (AngleInsideActiveArea(angle10))
                                        {
                                            angle10 = -1;
                                        }
                                    }
                                }
                                else
                                {
                                    angle00 = -1;
                                    angle10 = -1;
                                }

                                if (collider.useAngles)
                                {
                                    //Compared to the other circle
                                    angle01 = (intersection0 - collider.Position).GetAngle();

                                    if (circleCollider.AngleInsideActiveArea(angle01))
                                    {
                                        angle01 = -1;
                                    }

                                    if (amountOfIntersections > 1)
                                    {
                                        //Compared to the other circle
                                        angle11 = (intersection1 - collider.Position).GetAngle();

                                        if (circleCollider.AngleInsideActiveArea(angle11))
                                        {
                                            angle11 = -1;
                                        }
                                    }
                                }
                                else
                                {
                                    angle01 = -1;
                                    angle11 = -1;
                                }

                                if (amountOfIntersections > 0)
                                {
                                    if (amountOfIntersections == 1)
                                        return (angle00 == -1 && angle10 == -1);
                                    else if (amountOfIntersections == 2)
                                        return ((angle00 == -1 || angle10 == -1) && (angle01 == -1 || angle11 == -1));
                                    //|| ((((angle00 != -1 && angle10 != -1) && (angle01 == -1 && angle11 == -1))
                                    //|| ((angle00 == -1 && angle10 == -1) && (angle01 != -1 && angle11 != -1)))
                                    //&& AngleInsideActiveArea((collider.Position - Position).GetAngle()));
                                }

                                return false;
                            }
                        }

                        if (length <= Radius + collider.Radius)
                            return true;

                        return false;
                    case Type.Point:
                        CollisionCheckPoint(collider.Position);
                        break;
                    case Type.AABB:
                        break;
                    default:
                        break;
                }

                return base.CollisionCheck(collider, includeZ);
            }

            public override bool CollisionCheckPoint(Vector2 position)
            {
                Vector2 delta = new Vector2(position.X - this.Position.X, position.Y - this.Position.Y);
                float length = delta.Length();
                if (length < Radius)
                    if (!useAngles || AngleInsideActiveArea(delta.GetAngle()))
                        return true;
                return base.CollisionCheckPoint(position);
            }

            public override bool CollisionCheckLine(Vector4 v)
            {
                // is either end INSIDE the circle?
                // if so, return true immediately
                bool inside1 = CollisionCheckPoint(new Vector2(v.X, v.Y));
                bool inside2 = CollisionCheckPoint(new Vector2(v.Z, v.W));
                if (inside1 || inside2) return true;

                // if line collides with either edge of activeArea return true
                if (useAngles)
                {
                    Vector4 edgeLine0 = Extensions.GetVector4(Position, Position + Extensions.GetVector2(activeArea.X, Radius));
                    Vector4 edgeLine1 = Extensions.GetVector4(Position, Position + Extensions.GetVector2(activeArea.Y, Radius));

                    if (CollisionCheckLines(v, edgeLine0) || CollisionCheckLines(v, edgeLine1))
                        return true;
                }

                // get length of the line
                float distX = v.X - v.Z;
                float distY = v.Y - v.W;
                float len = (float)Math.Sqrt((distX * distX) + (distY * distY));

                // get dot product of the line and circle
                float dot = (((Position.X - v.X) * (v.Z - v.X)) + ((Position.Y - v.Y) * (v.W - v.Y))) / (len * len);

                // find the closest point on the line
                float closestX = v.X + (dot * (v.Z - v.X));
                float closestY = v.Y + (dot * (v.W - v.Y));

                // is this point actually on the line segment?
                // if so keep going, but if not, return false
                bool onSegment = CollisionCheckLinePoint(v, new Vector2(closestX, closestY));
                if (!onSegment) return false;


                // get distance to closest point
                distX = closestX - Position.X;
                distY = closestY - Position.Y;

                if (CollisionCheckPoint(new Vector2(closestX, closestY)))
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Returns the two angles representing the minimum (x) and maximum (y) of the active area
            /// </summary>
            /// <returns></returns>
            internal Vector2 GetActiveArea()
            {
                if (type == Type.Circle && useAngles)
                    return activeArea;
                else
                    return Vector2.Zero;
            }

            private static Circle Parse(Collider collider)
            {
                Circle ret;
                if (!collider.useAngles)
                    ret = new Circle(collider.Position, collider.positionZ, collider.Radius, collider.terrainCollision);
                else
                    ret = new Circle(collider.Position, collider.Radius, collider.terrainCollision, collider.activeArea);

                return ret;
            }

            public bool AngleInsideActiveArea(float angle)
            {
                float x, y, a;
                a = angle.NormalizeRadians();
                x = activeArea.X.NormalizeRadians();
                y = activeArea.Y.NormalizeRadians();

                return (a >= x && a < y);
            }

            /// <summary>
            /// Finds the intersection points of two circles. Intersecting points will be float.NaN if intersection  not found
            /// </summary>
            /// <param name="v0">The position of the first circle</param>
            /// <param name="radius0">The Radius of the first circle</param>
            /// <param name="v1">The position of the first circle</param>
            /// <param name="radius1">The Radius of the first circle</param>
            /// <param name="intersection1">The first intersecting point</param>
            /// <param name="intersection2">The second intersecting point</param>
            /// <returns>Returns the amount of intersections</returns>
            public int FindCircleCircleIntersections(
                Vector2 v0, float radius0,
                Vector2 v1, float radius1,
                out Vector2 intersection1, out Vector2 intersection2)
            {
                return FindCircleCircleIntersections(v0.X, v0.Y, radius0, v1.X, v1.Y, radius1, out intersection1, out intersection2);
            }

            // Find the points where the two circles intersect.
            /// <summary>
            /// Finds the intersection points of two circles
            /// </summary>
            /// <param name="cx0"></param>
            /// <param name="cy0"></param>
            /// <param name="radius0"></param>
            /// <param name="cx1"></param>
            /// <param name="cy1"></param>
            /// <param name="radius1"></param>
            /// <param name="intersection1"></param>
            /// <param name="intersection2"></param>
            /// <returns></returns>
            public int FindCircleCircleIntersections(
                float cx0, float cy0, float radius0,
                float cx1, float cy1, float radius1,
                out Vector2 intersection1, out Vector2 intersection2)
            {
                // Find the distance between the centers.
                float dx = cx0 - cx1;
                float dy = cy0 - cy1;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                // See how many solutions there are.
                if (dist > radius0 + radius1)
                {
                    // No solutions, the circles are too far apart.
                    intersection1 = new Vector2(float.NaN, float.NaN);
                    intersection2 = new Vector2(float.NaN, float.NaN);
                    return 0;
                }
                else if (dist < Math.Abs(radius0 - radius1))
                {
                    // No solutions, one circle contains the other.
                    intersection1 = new Vector2(float.NaN, float.NaN);
                    intersection2 = new Vector2(float.NaN, float.NaN);
                    return 0;
                }
                else if ((dist == 0) && (radius0 == radius1))
                {
                    // No solutions, the circles coincide.
                    intersection1 = new Vector2(float.NaN, float.NaN);
                    intersection2 = new Vector2(float.NaN, float.NaN);
                    return 0;
                }
                else
                {
                    // Find a and h.
                    double a = (radius0 * radius0 -
                        radius1 * radius1 + dist * dist) / (2 * dist);
                    double h = Math.Sqrt(radius0 * radius0 - a * a);

                    // Find P2.
                    double cx2 = cx0 + a * (cx1 - cx0) / dist;
                    double cy2 = cy0 + a * (cy1 - cy0) / dist;

                    // Get the points P3.
                    intersection1 = new Vector2(
                        (float)(cx2 + h * (cy1 - cy0) / dist),
                        (float)(cy2 - h * (cx1 - cx0) / dist));
                    intersection2 = new Vector2(
                        (float)(cx2 - h * (cy1 - cy0) / dist),
                        (float)(cy2 + h * (cx1 - cx0) / dist));

                    // See if we have 1 or 2 solutions.
                    if (dist == radius0 + radius1) return 1;
                    return 2;
                }
            }
        }

        public class Point : Collider
        {
            public Point(Vector2 position, bool collideWithTerrain) : base(position, collideWithTerrain)
            {
                type = Type.Point;
            }

            /// <summary>
            /// Returns true if it collides with the given collider
            /// </summary>
            /// <param name="collider"></param>
            /// <param name="includeZ">Determines if the function includes or ignores the Z axis</param>
            /// <returns></returns>
            public override bool CollisionCheck(Collider collider, bool includeZ)
            {
                if (includeZ)
                    if (!CollisionCheckZ(collider.positionZ, collider.heightZ))
                        return false;

                return collider.CollisionCheckPoint(Position);
            }
            public override void Draw(DrawBatch drawBatch, float thickness, Color color)
            {
                drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, Game1.pixel, Position, thickness, 0f, new Vector2(.5f), color, DrawBatch.CalculateDepth(Position));
            }
        }
        public class Line : Collider
        {
            public Line(Vector2 position, Vector2 end, bool collideWithTerrain) : base(position, collideWithTerrain)
            {
                type = Type.Line;
                vector4.Z = end.X;
                vector4.W = end.Y;
            }
            public Line(Vector4 line, bool collideWithTerrain) : base(new Vector2(line.X, line.Y), collideWithTerrain)
            {
                type = Type.Line;
                vector4.Z = line.Z;
                vector4.W = line.W;
            }

            /// <summary>
            /// Returns true if it collides with the given collider
            /// </summary>
            /// <param name="collider"></param>
            /// <param name="includeZ">Determines if the function includes or ignores the Z axis</param>
            /// <returns></returns>
            public override bool CollisionCheck(Collider collider, bool includeZ)
            {
                if (includeZ)
                    if (!CollisionCheckZ(collider.positionZ, collider.heightZ))
                        return false;

                switch (collider.type)
                {
                    case Type.Circle:
                        return collider.CollisionCheckLine(vector4);
                    case Type.Point:
                        return CollisionCheckLinePoint(vector4, collider.Position);
                    case Type.AABB:
                        return collider.CollisionCheckLine(vector4);
                    case Type.Line:
                        return CollisionCheckLines(vector4, collider.GetVector4());
                    default:
                        break;
                }

                return false;
            }

            public override bool CollisionCheckPoint(Vector2 position)
            {
                return CollisionCheckLinePoint(vector4, position);
            }

            public override void Draw(DrawBatch drawBatch, float thickness, Color color)
            {
                Game1.DrawLine(drawBatch, vector4, thickness, color, DrawBatch.CalculateDepth(Position));
            }
        }
        public class AABB : Collider
        {
            public AABB(Vector2 position, Vector2 measurements, bool collideWithTerrain) : base(position, collideWithTerrain)
            {
                type = Type.AABB;
                vector4.Z = measurements.X;
                vector4.W = measurements.Y;
            }
            public AABB(Vector4 vector, bool collideWithTerrain) : base(new Vector2(vector.X, vector.Y), collideWithTerrain)
            {
                type = Type.AABB;
                vector4.Z = vector.Z;
                vector4.W = vector.W;
            }

            /// <summary>
            /// Returns true if it collides with the given collider
            /// </summary>
            /// <param name="collider"></param>
            /// <param name="includeZ">Determines if the function includes or ignores the Z axis</param>
            /// <returns></returns>
            public override bool CollisionCheck(Collider collider, bool includeZ)
            {
                if (includeZ)
                    if (!CollisionCheckZ(collider.positionZ, collider.heightZ))
                        return false;

                switch (collider.type)
                {
                    case Type.Circle:
                        return (collider.CollisionCheckLine(new Vector4(Position.X, Position.Y, Position.X + Width, Position.Y))
                            || collider.CollisionCheckLine(new Vector4(Position.X + Width, Position.Y, Position.X + Width, Position.Y + Height))
                            || collider.CollisionCheckLine(new Vector4(Position.X + Width, Position.Y + Height, Position.X, Position.Y + Height))
                            || collider.CollisionCheckLine(new Vector4(Position.X, Position.Y + Height, Position.X, Position.Y))
                            || RectangleContainsPoint(vector4, collider.Position));
                    case Type.Point:
                        return CollisionCheckPoint(collider.Position);
                    case Type.AABB:
                        return (CollisionCheckPoint(new Vector2(collider.Position.X, collider.Position.Y))
                            || CollisionCheckPoint(new Vector2(collider.Position.X + collider.Width, collider.Position.Y))
                            || CollisionCheckPoint(new Vector2(collider.Position.X + collider.Width, collider.Position.Y + collider.Height))
                            || CollisionCheckPoint(new Vector2(collider.Position.X, collider.Position.Y + collider.Height)));
                    case Type.Line:
                        return CollisionCheckLine(collider.GetVector4());
                    default:
                        break;
                }

                return false;
            }

            public override bool CollisionCheckPoint(Vector2 position)
            {
                return RectangleContainsPoint(vector4, position);
            }

            public override bool CollisionCheckLine(Vector4 a)
            {
                return (RectangleContainsPoint(vector4, new Vector2(a.X, a.Y)) || RectangleContainsPoint(vector4, new Vector2(a.Z, a.W)))
                    || CollisionCheckLines(new Vector4(Position.X, Position.Y, Position.X + Width, Position.Y), a)
                    || CollisionCheckLines(new Vector4(Position.X + Width, Position.Y, Position.X + Width, Position.Y + Height), a)
                    || CollisionCheckLines(new Vector4(Position.X + Width, Position.Y + Height, Position.X, Position.Y + Height), a)
                    || CollisionCheckLines(new Vector4(Position.X, Position.Y + Height, Position.X, Position.Y), a);
            }

            public override void Draw(DrawBatch drawBatch, float thickness, Color color)
            {
                Game1.DrawLine(drawBatch, new Vector4(Position.X, Position.Y, Position.X + Width, Position.Y), thickness, color, DrawBatch.CalculateDepth(Position));
                Game1.DrawLine(drawBatch, new Vector4(Position.X + Width, Position.Y, Position.X + Width, Position.Y + Height), thickness, color, DrawBatch.CalculateDepth(Position));
                Game1.DrawLine(drawBatch, new Vector4(Position.X + Width, Position.Y + Height, Position.X, Position.Y + Height), thickness, color, DrawBatch.CalculateDepth(Position));
                Game1.DrawLine(drawBatch, new Vector4(Position.X, Position.Y + Height, Position.X, Position.Y), thickness, color, DrawBatch.CalculateDepth(Position));
            }
        }
    }
}
