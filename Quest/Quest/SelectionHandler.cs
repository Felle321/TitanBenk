using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest
{
    public static class SelectionHandler
    {
        public enum Target
        {
            Tile,
            GameObject
        }
        private static Target target;
        public static Target CurrentTarget
        {
            get { return target; }
            set
            {
                switch (value)
                {
                    case SelectionHandler.Target.Tile:
                        selectedID = -1;
                        break;
                    case SelectionHandler.Target.GameObject:
                        selectedTile = Point.Zero;
                        selectedTileSecond = Point.Zero;
                        break;
                    default:
                        break;
                }

                target = value;
            }
        }


        public static GameObject.Type tag;
        static bool trySelect = false;
        public static bool TrySelect
        {
            get
            {
                return trySelect;
            }
            set
            {
                DeSelect();
                trySelect = value;
            }
        }
        public static int selectedID = -1;
        public static Point selectedTile = Point.Zero;
        public static Point selectedTileSecond = Point.Zero;
        public static Point selectionPoint = Point.Zero;
        public static Point selectionPointSecond = Point.Zero;


        public static List<Point> tileObjects = new List<Point>();


        /// <summary>
        /// Selected rectangle in the grid
        /// </summary>
        public static Rectangle TileRectangle
        {
            get
            {
                Rectangle ret = Rectangle.Empty;

                if (selectedTile.X < selectedTileSecond.X)
                    ret.X = selectedTile.X;
                else
                    ret.X = selectedTileSecond.X;

                if (selectedTile.Y < selectedTileSecond.Y)
                    ret.Y = selectedTile.Y;
                else
                    ret.Y = selectedTileSecond.Y;

                ret.Width = Math.Abs(selectedTile.X - selectedTileSecond.X) + 1;
                ret.Height = Math.Abs(selectedTile.Y - selectedTileSecond.Y) + 1;

                return ret;
            }
        }

        /// <summary>
        /// Selected rectangle
        /// </summary>
        public static Rectangle SelectionRectangle
        {
            get
            {
                Rectangle ret = Rectangle.Empty;

                if (selectionPoint.X < selectionPointSecond.X)
                    ret.X = selectionPoint.X;
                else
                    ret.X = selectionPointSecond.X;

                if (selectionPoint.Y < selectionPointSecond.Y)
                    ret.Y = selectionPoint.Y;
                else
                    ret.Y = selectionPointSecond.Y;

                ret.Width = Math.Abs(selectionPoint.X - selectionPointSecond.X);
                ret.Height = Math.Abs(selectionPoint.Y - selectionPointSecond.Y);

                return ret;
            }
        }


        internal static void Update()
        {
            Point mousePosWorld = Game1.mousePosInWorld.ToPoint();

            if (Game1.MouseButtonPressedLeft())
            {
                TrySelect = true;
                if (CurrentTarget == Target.GameObject)
                    selectionPoint = mousePosWorld;
            }

            if (Game1.MouseButtonDownLeft(true))
            {
                if (CurrentTarget == Target.Tile)
                {
                    if ((Game1.mousePosInWorld.X >= 0 && Game1.mousePosInWorld.X < Game1.terrain.size.X * World.tileSize) && (Game1.mousePosInWorld.Y >= 0 && Game1.mousePosInWorld.Y < Game1.terrain.size.Y * World.tileSize))
                        selectedTileSecond = Game1.mouseTile;

                    tileObjects.Clear();
                    tileObjects.AddRange(Game1.terrain.GetTileObjectsInsideRect(TileRectangle, TileObject.Type.Tree));
                }
            }

            if (Game1.MouseButtonDownLeft(false))
            {
                if (CurrentTarget == Target.GameObject)
                {
                    selectionPointSecond = mousePosWorld;
                }
            }

            if (Game1.MouseButtonPressedRight())
            {
                if (CurrentTarget == Target.Tile)
                {
                    selectedTile = new Point(-1, -1);
                    selectedTileSecond = selectedTile;
                    tileObjects.Clear();
                }
            }

            if (TrySelect)
            {
                if (CurrentTarget == Target.Tile)
                {
                    if ((Game1.mousePosInWorld.X >= 0 && Game1.mousePosInWorld.X < Game1.terrain.size.X * World.tileSize) && (Game1.mousePosInWorld.Y >= 0 && Game1.mousePosInWorld.Y < Game1.terrain.size.Y * World.tileSize))
                    {
                        SelectTile(Game1.mouseTile);
                        if (Game1.MouseButtonDownRight(true))
                            selectedTileSecond = selectedTile;
                    }
                    else
                        DeSelect();
                }
            }


            if (Game1.MouseButtonUpLeft(false) && CurrentTarget == Target.GameObject)
            {
                selectionPoint = Point.Zero;
                selectionPointSecond = Point.Zero;
                trySelect = false;
            }
        }


        public static void SelectGameObject(int id, GameObject.Type tagToAdd)
        {
            tag = tagToAdd;
            selectedID = id;
            trySelect = false;
            target = Target.GameObject;
        }

        internal static void DeSelect()
        {
            trySelect = false;
            tag = GameObject.Type.Null;
            selectedID = -1;
            selectedTile = new Point(-1, -1);
            selectedTileSecond = new Point(-1, -1);
            tileObjects.Clear();
        }

        internal static void SelectTile(Point mouseTile)
        {
            selectedTile = mouseTile;
            selectedTileSecond = mouseTile;
            trySelect = false;
            CurrentTarget = Target.Tile;
        }

        internal static void SelectSecondTile(Point mouseTile)
        {
            selectedTileSecond = mouseTile;
        }


        public static bool TileObjectsContains(Point point)
        {
            for (int i = 0; i < tileObjects.Count; i++)
            {
                if (tileObjects[i] == point)
                    return true;
            }
            return false;
        }
    }
}
