using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Quest
{
    public class Path
    {
        Terrain terrain;
        Point target, startingPoint;
        /// <summary>
        /// A collection of Tiles to break the journey up into pieces
        /// </summary>
        public Point[] instructions;
        public int currentTarget = 0;
        public bool finished = false;

        public static Path Empty { get { return new Path(); } }

        public Path()
        {
            this.finished = true;
        }

        /// <summary>
        /// Both values in tile units
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startingPoint"></param>
        public Path(Terrain terrain, Point target, Point startingPoint)
        {
            this.terrain = terrain;
            this.target = target;
            this.startingPoint = startingPoint;
            instructions = GetInstructions(terrain, target, startingPoint);
        }
        struct Node
        {
            public float f;
            public Point parent, position;

            public Node(Point position, float g, float h, Point parent)
            {
                this.position = position;
                f = g + h;
                this.parent = parent;
            }

            public override string ToString()
            {
                return position.ToString() + " : " + f.ToString();
            }
        }

        /*
        public Point[] GetInstructions(Terrain terrain, Point target, Point startingPoint)
        {
            Point[] ret = null;
            List<Node> openNodes = new List<Node>();
            List<Node> closedNodes = new List<Node>();
            int lowestFIndex = 0;
            float lowestF;
            Point currentPosition = startingPoint;
            Point tempPos;
            bool found = false;
            List<Point> instructions = new List<Point>();

            openNodes.Add(new Node(startingPoint, 0, 0, startingPoint));

            while(openNodes.Count > 0)
            {
                lowestF = int.MaxValue;

                for (int i = 0; i < openNodes.Count; i++)
                {
                    if(openNodes[i].f < lowestF)
                    {
                        lowestF = openNodes[i].f;
                        lowestFIndex = i;
                    }
                }

                //Explore Node
                currentPosition = openNodes[lowestFIndex].position;
                closedNodes.Add(new Node(openNodes[lowestFIndex].position, openNodes[lowestFIndex].f, 0, openNodes[lowestFIndex].parent));
                openNodes.RemoveAt(lowestFIndex);

                for (int i = 1; i < 3; i += 2)
                {
                    if (found)
                        break;
                    for (int j = 0; j < 3; j += 1)
                    {
                        if (j < 3) i %= 3;
                        tempPos = new Point(currentPosition.X + i, currentPosition.Y + j);

                        if (!terrain.occupied[tempPos.X, tempPos.Y])
                        {
                            if(tempPos == target)
                            {
                                found = true;

                                closedNodes.Add(new Node(tempPos, (float)startingPoint.Distance(tempPos), (float)tempPos.Distance(target), currentPosition));

                                currentPosition = tempPos;
                                break;
                            }
                            if (!NodeListContainsPosition(openNodes, tempPos) && !NodeListContainsPosition(closedNodes, tempPos))
                                openNodes.Add(new Node(tempPos, (float)startingPoint.Distance(tempPos), (float)tempPos.Distance(target), currentPosition));
                        }
                    }
                }


                if(found)
                {
                    //Loop your way back to the starting point and add all the positions into a new list

                    openNodes.Clear();

                    while(currentPosition != startingPoint)
                    {
                        openNodes.Add(closedNodes[GetNode(closedNodes, currentPosition)]);
                        if(currentPosition == startingPoint)
                        {
                            break;
                        }
                        else
                        {
                            currentPosition = openNodes[openNodes.Count - 1].parent;
                        }
                    }

                    //Shortening with ray-casting
                    for (int i = 1; i < openNodes.Count; i++)
                    {
                        if(terrain.RayCastObstacle(currentPosition, openNodes[i].position, false))
                        {
                            currentPosition = openNodes[i - 1].position;
                            instructions.Add(new Point(currentPosition.X, currentPosition.Y));
                        }
                    }

                    instructions.Add(target);

                    return instructions.ToArray();
                }
            }

            return ret;
        }
        */

        public Point[] GetInstructions(Terrain terrain, Point target, Point startingPoint)
        {
            List<Node> openNodes = new List<Node>();
            List<Node> closedNodes = new List<Node>();
            Point currentPosition = startingPoint;
            Point tempPos = startingPoint;
            bool found = false;
            int currentNode;
            List<Point> ret = new List<Point>();

            openNodes.Add(new Node(startingPoint, 0, (float)startingPoint.Distance(target), startingPoint));

            while (openNodes.Count > 0 && !found)
            {
                currentPosition = openNodes[0].position;

                //Add neighbouring nodes to the open list
                for (int i = -1; i < 2; i++)
                {
                    if (found)
                        break;
                    for (int j = -1; j < 2; j++)
                    {
                        if (Math.Abs(i) != Math.Abs(j) && !(i == 0 && j == 0) &&
                            (currentPosition.X + i >= 0 && currentPosition.Y + j >= 0) &&
                            (currentPosition.X + i < terrain.size.X && currentPosition.Y + j < terrain.size.Y))
                        {
                            tempPos = currentPosition + new Point(i, j);
                            if (tempPos == target)
                            {
                                //Neighbour is the target
                                found = true;
                                closedNodes.Add(new Node(tempPos, 0, 0, currentPosition));
                            }
                            else
                            {
                                //Add node to open Node list
                                if (!NodeListContainsPosition(openNodes, tempPos) && !NodeListContainsPosition(closedNodes, tempPos) && !terrain.IsTileBlocked(tempPos.X, tempPos.Y))
                                    AddToOpenNodes(ref openNodes, new Node(tempPos, (float)startingPoint.Distance(tempPos), (float)tempPos.Distance(target), currentPosition));
                            }
                        }
                    }
                }

                //Move current node to closed list
                currentNode = GetNode(openNodes, currentPosition);

                if (found)
                    closedNodes.Insert(closedNodes.Count - 1, new Node(currentPosition, 0, 0, openNodes[currentNode].parent));
                else
                    closedNodes.Add(new Node(currentPosition, 0, 0, openNodes[currentNode].parent));

                openNodes.RemoveAt(currentNode);
            }

            if (found)
            {
                int i = closedNodes.Count - 1;
                ret.Add(closedNodes[i].position);

                //Retrace steps
                while (currentPosition != startingPoint && i >= 0)
                {
                    i = GetNode(closedNodes, closedNodes[i].parent);
                    currentPosition = closedNodes[i].position;
                    ret.Add(currentPosition);
                }

                ret.Reverse();
                Point[] returnValue = ShortenInstructionsRayCast(terrain, ret.ToArray());
                return returnValue;
            }

            //Did not find a viable path
            return null;
        }

        void AddToOpenNodes(ref List<Node> list, Node node)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (node.f <= list[i].f)
                {
                    list.Insert(i, node);
                    return;
                }
            }

            list.Add(node);
        }

        bool NodeListContainsPosition(List<Node> list, Point position)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].position == position)
                    return true;
            }
            return false;
        }

        int GetNode(List<Node> list, Point position)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].position == position)
                    return i;
            }

            return -1;
        }

        public Point[] ShortenInstructionsRayCast(Terrain terrain, Point[] instructions)
        {
            int i = 1;
            List<Point> ret = new List<Point>();
            ret.Add(instructions[0]);
            Point currentPoint = instructions[0];
            Point p;
            List<Point> liste;

            while (i < instructions.Length)
            {
                if (i == instructions.Length - 1)
                {
                    ret.Add(instructions[instructions.Length - 1]);
                    break;
                }
                if (terrain.RayCastObstacle(currentPoint.ScaleRet(World.tileSize).ToVector2() + new Vector2(World.tileSize / 2, World.tileSize / 2),
                    instructions[i + 1].ScaleRet(World.tileSize).ToVector2() + new Vector2(World.tileSize / 2, World.tileSize / 2), out p, out liste))
                {
                    ret.Add(instructions[i]);
                    currentPoint = instructions[i];
                }

                i++;
            }

            return ret.ToArray();
        }
    }
}
