using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Quest
{
    public class Terrain
    {
        /// <summary>
        /// The size of the terrain in tiles
        /// </summary>
        public Point size = new Point(64);
        public enum Material
        {
            Dirt, Grass, Stone, TilledDirt
        }
        static Texture2D sheet, sheetEdges;

        public Material[,] materials;
        Point[,] tileReferences;
        TileObject[,] tileObjects;

        static int[] materialEdgePriority;

        public Terrain()
        {
            tileReferences = new Point[size.X, size.Y];
            tileObjects = new TileObject[size.X, size.Y];
            materials = new Material[size.X, size.Y];

            for (int i = 0; i < size.X; i++)
            {
                for (int j = 0; j < size.Y; j++)
                {
                    materials[i, j] = (Material)(Game1.random.Next(Enum.GetNames(typeof(Material)).Length));
                    tileReferences[i, j] = new Point(-1);
                }
            }
        }

        public static void LoadContent(ContentManager Content)
        {
            sheet = Content.Load<Texture2D>("Materials");
            sheetEdges = Content.Load<Texture2D>("MaterialEdges");

            materialEdgePriority = new int[Enum.GetNames(typeof(Material)).Length];
            materialEdgePriority[0] = 1;
            materialEdgePriority[1] = 2;
            materialEdgePriority[2] = 4;
            materialEdgePriority[3] = 3;
        }

        public void Draw(DrawBatch drawBatch)
        {
            Point edgeOffset = Point.Zero;
            float rotation = 0f;

            //Draw Materials
            for (int i = 0; i < size.X; i++)
            {
                for (int j = 0; j < size.Y; j++)
                {
                    if (Game1.camera.Rectangle.Contains(new Rectangle(i * World.tileSize, j * World.tileSize, size.X, size.Y)))
                    {
                        drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, sheet, new Rectangle(i * World.tileSize, j * World.tileSize, World.tileSize, World.tileSize), GetMaterialTextureSrcRect(materials[i, j]), 0, Vector2.Zero, Color.White, 0f, SpriteEffects.None, 1, 0f);

                        /*
                        //Draw edges
                        if (i > 0 && j > 0 && i < size.X - 1 && j < size.Y - 1)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                switch (k)
                                {
                                    case 0:
                                        edgeOffset = new Point(1, 0);
                                        break;
                                    case 1:
                                        edgeOffset = new Point(0, -1);
                                        break;
                                    case 2:
                                        edgeOffset = new Point(-1, 0);
                                        break;
                                    case 3:
                                        edgeOffset = new Point(0, 1);
                                        break;
                                    default:
                                        break;
                                }

                                if (materialEdgePriority[(int)materials[i, j]] > materialEdgePriority[(int)materials[i + edgeOffset.X, j + edgeOffset.Y]])
                                    switch (k)
                                    {
                                        case 0:
                                            rotation = 0f;
                                            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, sheetEdges, new Rectangle(i * World.tileSize + World.tileSize, j * World.tileSize, 4, World.tileSize), GetMaterialEdgeTextureSrcRect(materials[i, j]), rotation, Vector2.Zero, Color.White, 0f, SpriteEffects.None, 1, 0f);
                                            break;
                                        case 1:
                                            rotation = 4.71f;
                                            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, sheetEdges, new Rectangle(i * World.tileSize, j * World.tileSize, 4, World.tileSize), GetMaterialEdgeTextureSrcRect(materials[i, j]), rotation, Vector2.Zero, Color.White, 0f, SpriteEffects.None, 1, 0f);
                                            break;
                                        case 2:
                                            rotation = 0f;
                                            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, sheetEdges, new Rectangle(i * World.tileSize - 4, j * World.tileSize, 4, World.tileSize), GetMaterialEdgeTextureSrcRect(materials[i, j]), rotation, Vector2.Zero, Color.White, 0f, SpriteEffects.FlipHorizontally, 1, 0f);
                                            break;
                                        case 3:
                                            rotation = 4.71f;
                                            drawBatch.Draw(DrawBatch.DrawCall.Tag.GameObject, sheetEdges, new Rectangle(i * World.tileSize, j * World.tileSize + World.tileSize + 4, 4, World.tileSize), GetMaterialEdgeTextureSrcRect(materials[i, j]), rotation, Vector2.Zero, Color.White, 0f, SpriteEffects.FlipHorizontally, 1, 0f);
                                            break;
                                        default:
                                            break;
                                    }
                            }
                        }
                                    */
                            
                        
                    }

                }
            }

            /*
            for (int i = 0; i < tileReferences.GetLength(0); i++)
            {
                for (int j = 0; j < tileReferences.GetLength(1); j++)
                {
                    if (tileReferences[i, j] != null && tileReferences[i, j] != new Point(-1, -1))
                    {
                        if (tileReferences[i, j] == new Point(i, j))
                            spriteBatch.Draw(Game1.pixel, new Rectangle(i * World.tileSize, j * World.tileSize, World.tileSize, World.tileSize), Color.Lerp(Color.Cyan, Color.Transparent, .5f));
                        else
                            spriteBatch.Draw(Game1.pixel, new Rectangle(i * World.tileSize, j * World.tileSize, World.tileSize, World.tileSize), Color.Lerp(Color.ForestGreen, Color.Transparent, .5f));
                    }
                }
            }
            */

            //Draw TileObjects
            for (int i = 0; i < tileObjects.GetLength(0); i++)
            {
                for (int j = 0; j < tileObjects.GetLength(1); j++)
                {
                    if (tileObjects[i, j] != null)
                        tileObjects[i, j].Draw(drawBatch);
                }
            }
        }

        Rectangle GetMaterialTextureSrcRect(Material material)
        {
            return new Rectangle(World.tileSize * (int)material, 0, World.tileSize, World.tileSize);
        }

        Rectangle GetMaterialEdgeTextureSrcRect(Material material)
        {
            return new Rectangle((int)(World.tileSize * .25f * (int)material), 0, 4, World.tileSize);
        }

        /// <summary>
        /// Returns a valid position within the constraints of the terrain in the resolution of the grid
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Point WorldPosToTile(Point position)
        {
            if (position.X < 0)
                position.X -= World.tileSize;
            if (position.Y < 0)
                position.Y -= World.tileSize;

            return new Point((int)position.X - (int)(position.X % World.tileSize), (int)position.Y - (int)(position.Y % World.tileSize)).ScaleRet(1 / (float)World.tileSize);
        }
        public static Point WorldPosToTile(Vector2 position)
        {
            if (position.X < 0)
                position.X -= World.tileSize;
            if (position.Y < 0)
                position.Y -= World.tileSize;

            return new Point((int)position.X - (int)(position.X % World.tileSize), (int)position.Y - (int)(position.Y % World.tileSize)).ScaleRet(1 / (float)World.tileSize);
        }

        internal void TryCreateTileObject(TileObject tileObject)
        {
            if (IsTilesOccupied(tileObject.hitBox))
                return;

            for (int i = 0; i < tileObject.hitBox.Width; i++)
            {
                for (int j = 0; j < tileObject.hitBox.Height; j++)
                {
                    tileReferences[tileObject.hitBox.X + i - tileObject.tileOrigin.X, tileObject.hitBox.Y + j - tileObject.tileOrigin.Y] = tileObject.hitBox.Location;
                }
            }

            tileObjects[tileObject.hitBox.X, tileObject.hitBox.Y] = tileObject;
        }

        internal void RemoveTileObject(Point location)
        {
            TileObject tileObject = tileObjects[location.X, location.Y];

            for (int i = 0; i < tileObject.hitBox.Width; i++)
            {
                for (int j = 0; j < tileObject.hitBox.Height; j++)
                {
                    tileReferences[tileObject.hitBox.X + i - tileObject.tileOrigin.X, tileObject.hitBox.Y + j - tileObject.tileOrigin.Y] = new Point(-1);
                }
            }

            tileObjects[location.X, location.Y] = null;
        }

        private bool IsTilesOccupied(Rectangle hitBox)
        {
            for (int i = 0; i < hitBox.Width; i++)
            {
                for (int j = 0; j < hitBox.Height; j++)
                {
                    if (IsTileOccupied(hitBox.X + i, hitBox.Y + j))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Refers to gameobjects' collision
        /// </summary>
        /// <returns></returns>
        public bool IsTileBlocked(int x, int y)
        { return IsTileBlocked(new Point(x, y)); }
        /// <summary>
        /// Refers to gameobjects' collision
        /// </summary>
        /// <returns></returns>
        public bool IsTileBlocked(Point xy)
        {
            if (IsTileOccupied(xy))
            {
                if (tileReferences[xy.X, xy.Y] != xy)
                    xy = tileReferences[xy.X, xy.Y];

                return tileObjects[xy.X, xy.Y].physicsBlock;
            }

            return false;
        }

        /// <summary>
        /// Refers to construction, not physics affecting gameobjects
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsTileOccupied(int x, int y)
        {
            return IsTileOccupied(new Point(x, y));
        }
        /// <summary>
        /// Refers to construction, not physics affecting gameobjects
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsTileOccupied(Point xy)
        {
            if (xy.X < 0 || xy.X >= size.X || xy.Y < 0 || xy.Y >= size.Y)
                return false;
            if (tileReferences[xy.X, xy.Y] == new Point(-1))
                return false;

            return true;
        }

        public TileObject GetTileObject(int x, int y)
        {
            return tileObjects[x, y];
        }



        /*
        /// <summary>
        /// Returns true if any obstacle is found on the way
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="water">Wether water acts as an obstacle</param>
        /// <returns></returns>
        public bool RayCastObstacle(Point start, Point end, bool water)
        {
            start.Scale(16);
            end.Scale(16);

            start.X += 8;
            start.Y += 8;
            end.X += 8;
            end.Y += 8;

            end -= start;
            float k = end.Y / (float)end.X;

            Vector2 position = Vector2.Zero;

            while(Math.Abs(end.X * 16 - position.X) >= 16 || Math.Abs(end.Y * 16 - position.Y) >= 16)
            {
                if(16 - (position.X % 16) < 16 - (position.Y % 16))
                {
                    //X is the closest
                    if (end.X > 0)
                    {
                        position.X += 16 - (position.X % 16);
                        position.Y += (16 - (position.X % 16)) * k;
                    }
                    else
                    {
                        position.X -= (position.X % 16);
                        position.Y += (16 - (position.X % 16)) * k;
                    }
                }
                else
                {
                    if (end.Y > 0)
                    {
                        //Y is the closest
                        position.Y += 16 - (position.Y % 16);
                        position.X += (16 - (position.Y % 16)) / k;
                    }
                    else
                    {
                        position.Y -= (position.Y % 16);
                        position.X += (16 - (position.Y % 16)) / k;
                    }
                }

                //CurrentTile
                start = GetTile(position.ToPoint());

                if (occupied[start.X / 16, start.Y / 16] || (water && materials[start.X / 16, start.Y / 16] == Material.Water))
                    return true;
            }

            return false;
        }
        */
        /*
        /// <summary>
        /// Returns true if any obstacle is found on the way
        /// </summary>
        /// <param name="position">World position</param>
        /// <param name="end">World position</param>
        /// <returns></returns>
        public bool RayCastObstacle(Vector2 position, Vector2 end, out Point collisionPoint)
        {
            collisionPoint = Point.Zero;
            return true;

            Point currentTile = WorldPosToTile(position);
            Point targetTile = WorldPosToTile(end);
            Vector2 delta = end - position;
            float k = delta.Y / delta.X;
            float distanceX, distanceY;
            collisionPoint = Point.Zero;
            bool collision = false;

            while (!collision && currentTile != targetTile && position.X > 0
                && position.X < size.X * World.tileSize && position.Y < size.Y * World.tileSize
                && position.X >= 0 && position.Y >= 0)
            {
                delta = end - position;

                if (delta.X < 0)
                {
                    distanceX = Math.Abs(position.X) % World.tileSize;
                    if (distanceX == 0)
                        distanceX = 16;
                }
                else
                    distanceX = World.tileSize - position.X % World.tileSize;

                if (delta.Y < 0)
                {
                    distanceY = Math.Abs(position.Y) % World.tileSize;
                    if (distanceY == 0)
                        distanceY = 16;
                }
                else
                    distanceY = World.tileSize - position.Y % World.tileSize;


                if (distanceX * Math.Abs(k) == distanceY)
                {
                    if (IsTileBlocked(currentTile.X, currentTile.Y + (int)(delta.Y / Math.Abs(delta.Y))) && IsTileBlocked(currentTile.X + (int)(delta.X / Math.Abs(delta.X)), currentTile.Y))
                    {
                        collision = true;
                        collisionPoint = new Point(currentTile.X, currentTile.Y + (int)(delta.Y / delta.Y));
                    }
                }
                if (distanceX * Math.Abs(k) < distanceY)
                {
                    //Next intersection is X
                    if (delta.X < 0)
                    {
                        position.X -= distanceX;
                        currentTile.X--;
                        position.Y += k * distanceX;
                    }
                    else
                    {
                        position.X += distanceX;
                        currentTile.X++;
                        position.Y += k * (distanceX * (int)(delta.X / Math.Abs(delta.X)));
                    }
                }
                else
                {
                    //Next intersection is Y
                    if (delta.Y < 0)
                    {
                        position.Y -= distanceY;
                        currentTile.Y--;
                        position.X += distanceY / k;
                    }
                    else
                    {
                        position.Y += distanceY;
                        currentTile.Y++;
                        position.X += (distanceY * (int)(delta.Y / Math.Abs(delta.Y))) / k;
                    }

                    if (distanceX * Math.Abs(k) == distanceY)
                        currentTile.X += (int)(delta.X / Math.Abs(delta.X));
                }

                if (IsTileBlocked(currentTile.X, currentTile.Y))
                {
                    collision = true;
                    collisionPoint = currentTile;
                }
            }

            return collision;
        }
        */

        public struct RayCrossing
        {
            float position;
            bool x;

            public RayCrossing(float position, bool x)
            {
                this.position = position;
                this.x = x;
            }
            public float Position
            {
                get { return position; }
                set { position = value; }
            }
            public bool X
            {
                get
                {
                    return x;
                }
            }
            public bool Y
            {
                get
                {
                    return !x;
                }
            }
            public RayCrossing Copy()
            {
                return new RayCrossing(position, x);
            }
        }

        #region fuck my life 3
        /*
        /// <summary>
        /// Returns true if any obstacle is found on the way
        /// </summary>
        /// <param name="position">World position</param>
        /// <param name="end">World position</param>
        /// <returns></returns>
        public bool RayCastObstacle(Vector2 position, Vector2 end, out Point collisionPoint, out List<Point> intersectingTiles)
        {
            intersectingTiles = new List<Point>();
            collisionPoint = Point.Zero;

            if (!(position.X > 0 && position.Y > 0 && end.X > 0 && end.Y > 0))
                return true;

            Vector2 delta, current, distanceToBorder = Vector2.Zero;
            float k, mx, my;
            List<RayCrossing> crossings = new List<RayCrossing>();

            current = position;

            delta = end - position;
            k = delta.Y / delta.X;

            Point currentTile = WorldPosToTile(current);

            intersectingTiles.Add(currentTile);

            if (delta.X > 0)
                mx = current.X % World.tileSize;
            else
                mx = World.tileSize - current.X % World.tileSize;
            if (delta.Y > 0)
                my = current.Y % World.tileSize;
            else
                my = World.tileSize - current.Y % World.tileSize;

            if (mx < my)
            {
                my -= mx * k;
                mx = -mx;
            }
            else
            {
                mx -= my / k;
                my = -my;
            }

            float activeK = 1 / k;

            for (int i = 0; i < Math.Abs(delta.X / 16 + delta.Y / 16 - 1); i++)
            {
                if (activeK > 1)
                {
                    currentTile.X += (int)(delta.X / Math.Abs(delta.X));
                    activeK--;
                }
                else
                {
                    currentTile.Y += (int)(delta.Y / Math.Abs(delta.Y));
                    activeK += 1 / k;
                }

                intersectingTiles.Add(currentTile);
            }

            return false;
        }
        */
#endregion

        #region Second Backup
        /*
        /// <summary>
        /// Returns true if any obstacle is found on the way
        /// </summary>
        /// <param name="position">World position</param>
        /// <param name="end">World position</param>
        /// <returns></returns>
        public bool RayCastObstacle(Vector2 position, Vector2 end, out Point collisionPoint, out List<Point> intersectingTiles)
        {
            intersectingTiles = new List<Point>();
            collisionPoint = Point.Zero;

            if (!(position.X > 0 && position.Y > 0 && end.X > 0 && end.Y > 0))
                return true;

            Vector2 delta, current, distanceToBorder = Vector2.Zero;
            float k;
            List<RayCrossing> crossings = new List<RayCrossing>();

            current = position;

            delta = end - position;
            k = delta.Y / delta.X;

            if (delta.X > 0 && delta.Y < 0)
                delta.X = delta.X;

            while (!IsTileOccupied(WorldPosToTile(current)) && WorldPosToTile(current) != WorldPosToTile(end))
            {
                intersectingTiles.Add(WorldPosToTile(current));

                if (delta.X > 0)
                    distanceToBorder.X = World.tileSize - current.X % World.tileSize;
                else
                {
                    if (current.X < 0)
                        distanceToBorder.X = 16 + current.X % World.tileSize;
                    else
                        distanceToBorder.X = -current.X % World.tileSize;

                    if (distanceToBorder.X == 0)
                        distanceToBorder.X -= .1f;
                }

                if (delta.Y > 0)
                    distanceToBorder.Y = World.tileSize - current.Y % World.tileSize;
                else
                {
                    if (current.Y < 0)
                        distanceToBorder.Y = 16 + current.Y % World.tileSize;
                    else
                        distanceToBorder.Y = -current.Y % World.tileSize;

                    if (distanceToBorder.Y == 0)
                        distanceToBorder.Y -= .1f;
                }


                //Exceptions
                if (delta.Y == 0)
                {
                    current.X += distanceToBorder.X;
                }
                else if (delta.X == 0)
                {
                    current.Y += distanceToBorder.Y;
                }
                else
                {
                    if (Math.Abs(distanceToBorder.X) < Math.Abs(distanceToBorder.Y) / Math.Abs(k))
                    {
                        //X närmst
                        current.X += distanceToBorder.X;
                        current.Y += distanceToBorder.X * k;
                    }
                    else
                    {
                        //Y närmst
                        current.X += (distanceToBorder.Y / Math.Abs(k)) * delta.X / Math.Abs(delta.X);
                        current.Y += distanceToBorder.Y;
                    }
                }
            }

            if (IsTileOccupied(WorldPosToTile(current)))
            {
                collisionPoint = WorldPosToTile(current);
                return true;
            }
            else
            {
                intersectingTiles.Add(WorldPosToTile(current));
                if (WorldPosToTile(current) != WorldPosToTile(end))
                    throw (new Exception("Terrain, RayCast - End point not reached"));
            }

            return false;
        }
        */
        #endregion

        #region latest and greatest backup. functioning +x +y

        /// <summary>
        /// Returns true if any obstacle is found on the way
        /// </summary>
        /// <param name="position">World position</param>
        /// <param name="end">World position</param>
        /// <returns></returns>
        public bool RayCastObstacle(Vector2 position, Vector2 end, out Point collisionPoint, out List<Point> intersectingTiles)
    {
        intersectingTiles = new List<Point>();
        collisionPoint = Point.Zero;
        Vector2 distanceToBorder, delta;
        float k;
        List<RayCrossing> crossings = new List<RayCrossing>();

        delta = end - position;
        k = Math.Abs(delta.Y / delta.X);
        float xToAdd;

        if (delta.X < 0)
            distanceToBorder.X = -position.X % World.tileSize;
        else
            distanceToBorder.X = World.tileSize - position.X % World.tileSize;
        if (delta.Y < 0)
            distanceToBorder.Y = -position.Y % World.tileSize;
        else
            distanceToBorder.Y = World.tileSize - position.Y % World.tileSize;


        if (delta.X < 0 && delta.Y < 0)
        {
            //end.X = (int)Math.Round(end.X / 16) * 16 - 8;
            //end.Y = (int)Math.Round(end.Y / 16) * 16 - 8;
            //position.X = (int)Math.Round(position.X / 16) * 16 + distanceToBorder.X;
            position.Y = (int)Math.Round(position.Y / 16) * 16 + distanceToBorder.Y;
        }
        else if (delta.X > 0 == delta.Y < 0 || delta.X < 0 == delta.Y > 0)
            {
                position.X = (int)Math.Round(position.X / 16) * 16 + distanceToBorder.X;
                position.Y = (int)Math.Round(position.Y / 16) * 16 + distanceToBorder.Y;
            }

        ////Temp
        //if (delta.Y < 0)
        //    return false;
        if (delta.X > 0 && delta.Y > 0 || delta.X < 0 && delta.Y < 0)
        {
            for (int i = 0; i < (int)Math.Abs((delta.X - Math.Abs(distanceToBorder.X) + 16) / World.tileSize); i++)
            {
                    xToAdd = position.X + distanceToBorder.X + 16 * i;
                    if (delta.X < 0 && delta.Y < 0)
                        xToAdd += 16;
                    crossings.Add(new RayCrossing(xToAdd, true));
            }

            for (int i = 0; i < (int)Math.Abs((delta.Y - Math.Abs(distanceToBorder.Y) + 16) / World.tileSize); i++)
            {
                xToAdd = position.X + Math.Abs(distanceToBorder.Y / k) + (16 * i) / k;
                if (delta.X < 0 && delta.Y < 0)
                   xToAdd -= 16;
                crossings.Add(new RayCrossing(xToAdd, false));
            }
        }
        else
        {
            for (int i = 0; i < (int)Math.Abs((delta.X + (distanceToBorder.X)) / World.tileSize); i++)
            {
                    xToAdd = position.X + distanceToBorder.X + 16 * i;
                    if (delta.X < 0 && delta.Y < 0)
                        xToAdd += 16;
                    crossings.Add(new RayCrossing(position.X + distanceToBorder.X + 16 * i, true));
            }

            for (int i = 0; i < (int)Math.Abs((delta.Y - Math.Abs(distanceToBorder.Y) + 16) / World.tileSize); i++)
            {
                xToAdd = position.X + Math.Abs(distanceToBorder.Y / k) + (16 * i) / k;
                if (delta.X < 0)
                    xToAdd -= 16;
                crossings.Add(new RayCrossing(xToAdd, false));
            }
        }

        Point currentTile = WorldPosToTile(position);

        intersectingTiles.Add(currentTile);

        SortCrossingsByPosition(ref crossings);

        for (int i = 0; i < crossings.Count; i++)
        {
            if (i < crossings.Count - 1 && crossings[i].Position == crossings[i + 1].Position)
            {
                if (IsTileBlocked(new Point(currentTile.X, currentTile.Y + (int)(delta.Y / Math.Abs(delta.Y))))
                    || IsTileBlocked(new Point((int)(currentTile.X + delta.X / Math.Abs(delta.X)), currentTile.Y)))
                {
                    collisionPoint = currentTile;
                    return true;
                }
                else
                    i++;

                currentTile.X += (int)(delta.X / Math.Abs(delta.X));
                currentTile.Y += (int)(delta.Y / Math.Abs(delta.Y));
            }
            else
            {
                if (crossings[i].X)
                    currentTile.X += (int)(delta.X / Math.Abs(delta.X));

                if (crossings[i].Y)
                    currentTile.Y += (int)(delta.Y / Math.Abs(delta.Y));
            }

            intersectingTiles.Add(currentTile);

            if (IsTileBlocked(currentTile))
            {
                collisionPoint = currentTile;
                return true;
            }
        }

        return false;
    }
        #endregion


        /// <summary>
        /// Sorts the list of RayCrossings by the position variable. The position on the X-axis
        /// </summary>
        /// <param name="crossings"></param>
        public static void SortCrossingsByPosition(ref List<RayCrossing> crossings)
        {
            bool sorted = false;
            RayCrossing temp;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < crossings.Count - 1; i++)
                {
                    if (crossings[i].Position > crossings[i + 1].Position)
                    {
                        //Switch
                        sorted = false;
                        temp = crossings[i + 1].Copy();
                        crossings[i + 1] = crossings[i].Copy();
                        crossings[i] = temp.Copy();
                    }
                }
            }
        }

        public Point[] GetTileObjectsInsideRect(Rectangle rect, TileObject.Type type)
        {
            if (rect.Right > size.X - 1)
                rect.Width = (size.X - 1) - rect.X;
            if (rect.Right > size.Y - 1)
                rect.Width = (size.Y - 1) - rect.Y;

            if (rect.X < 0)
            {
                rect.Width += rect.X;
                rect.X = 0;
            }
            if (rect.Y < 0)
            {
                rect.Height += rect.Y;
                rect.Y = 0;
            }

            List<Point> ret = new List<Point>();
            Point p;
            for (int i = rect.X; i < rect.Right; i++)
            {
                for (int j = rect.Y; j < rect.Bottom; j++)
                {
                    if (tileReferences[i, j] != new Point(i, j))
                    {
                        //Reference
                        p = tileReferences[i, j];
                    }
                    else
                        p = new Point(i, j);

                    if (p != new Point(-1, -1) && tileObjects[p.X, p.Y] != null && tileObjects[p.X, p.Y].type == type)
                        ret.Add(new Point(p.X, p.Y));
                }
            }

            return ret.ToArray();
        }
    }
}
