using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    class WeightMatrix
    {
        /// <summary>
        /// This function obtains the position where the 0 is between two weight nodes
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double GetZeroCrossingCoordinate(Point point1, Point point2)
        {// here point is (x1,y1) and (x2,y2), where y is the weight and x is the index
            // we will interpolate and find the point where we actually have the 0

            // define a line as (x;y) = (Vx;Vy)*t + (Px;Py)
            double Vx = point2.x - point1.x;
            double Vy = point2.y - point1.y;

            double Px = point1.x;
            double Py = point1.y;
            
            // solve for t when y = 0 to get the zero-crossing
            double t = -Py / Vy;
            // get the x that will have y = 0
            double x = t * Vx + Px;

            return x;
        }

        /// <summary>
        /// This function goes through the matrix and finds all the zeroes that are between the weight nodes. We loop through each weight node and look at the 4 neighbors to see if there is a sign change. If there is one, we interpolate for the actual position.
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static List<Node> FindZeroes(double[,] weights)
        {
            // results list
            List<Node> nodes = new List<Node>();

            // go through each point
            // if node is positive, place a node at each neighbor that is negative
            int rows = weights.GetLength(0);
            for (int i = 0; i < weights.GetLength(0); i++)
            {
                for (int j = 0; j < weights.GetLength(1); j++)
                {
                    if (weights[i, j] == 0)
                    {

                    }
                    else if (weights[i, j] >= 0)
                    {// check neighbors for negative
                        if ((i-1>=0) && weights[i - 1, j] < 0) // top neighbor
                        {
                            double interpolatedCoordinate =
                                GetZeroCrossingCoordinate(new Point(i - 1, weights[i - 1, j]),
                                                new Point(i, weights[i, j]));
                            nodes.Add(new Node(j, interpolatedCoordinate,Direction.Horizontal));

                        }

                        if ((j-1>=0) && weights[i, j - 1] < 0) // left neighbor
                        {
                            double interpolatedCoordinate =
                                GetZeroCrossingCoordinate(new Point(j - 1, weights[i, j - 1]),
                                                new Point(j, weights[i, j]));
                            nodes.Add(new Node(interpolatedCoordinate, i,Direction.Vertical));
                        }

                        if ((i+1<weights.GetLength(0)) && weights[i + 1, j] < 0) // bottom neighbor
                        {
                            double interpolatedCoordinate =
                                GetZeroCrossingCoordinate(new Point(i, weights[i, j]),
                                                new Point(i + 1, weights[i + 1, j]));
                            nodes.Add(new Node(j, interpolatedCoordinate,Direction.Horizontal));
                        }

                        if (j+1<weights.GetLength(1) && weights[i, j + 1] < 0) // right neighbor
                        {
                            double interpolatedCoordinate =
                                GetZeroCrossingCoordinate(new Point(j, weights[i, j]),
                                                new Point(j + 1, weights[i, j + 1]));
                            nodes.Add(new Node(interpolatedCoordinate, i,Direction.Vertical));
                        }
                    }
                }
            }

            // purge the nodes that are identical
            for (int i = 0; i < nodes.Count; i++)
            {
                int j = i + 1; // start from the node we want to compare
                while (j < nodes.Count) // stop when there is no more nodes
                {
                    if (Node.Identical(nodes.ElementAt(i), nodes.ElementAt(j)))
                    {// remove the node j
                        nodes.RemoveAt(j);
                    }
                    else
                    {// don't remove node, and advance to next node
                        j++;
                    }
                }
            }

            // the function will return a list of nodes of the path
            return nodes;
        }

        /// <summary>
        /// this rebuilds the list of segments as an ordered list of segments 
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<Segment> NodesToSegmentsConnectivity(List<Node> nodes)
        {
            // take the first point
            Node originNode = nodes.ElementAt(0);
            List<Segment> segments = new List<Segment>();
            // search around it clockwise with initial angle 0 (to the right)
            int foundAngle = 0;
            Node startNode = originNode;
            Node endNode = originNode;

            // we put a limit of loops in case something goes wrong, then we don't get stuck in an infinite loop
            // 2 x the number of nodes should be enough to walk back on a path in case of dead-ends
            for (int i = 0;i<nodes.Count() *2;i++) 
            {
                // first time, run with entry angle 0
                endNode = SearchSegmentEnd(nodes, startNode, AngleExitToEntry(foundAngle),out foundAngle);
                segments.Add(new Segment(startNode, endNode));
            
                if (Node.Identical(originNode,endNode))
                {// do until we come back to the first point
                    // we've completed the loop
                    return segments;
                }

                // setup for next iteration of loop
                startNode = endNode;
            }

            // if we get here, it is because we could not find a loop. We return the path anyways
            return segments;
        }

        /// <summary>
        /// This function converts an angle of exit to angle of entry, from one segment to another, which is just 180degrees added
        /// </summary>
        /// <param name="exitAngle"></param>
        /// <returns></returns>
        public static int AngleExitToEntry(int exitAngle)
        {
            return (exitAngle + 180) % 360; // if a node is entered at angle 45degrees for example, the end node sees it as entry angle from 225degrees
        }

        // angles we can look through
        static int[] Angles = { 0, 45, 90, 135, 180, 225, 270, 315 };

        /// <summary>
        /// this looks at the array of angles and finds the index in the array
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static int GetAngleIndex(int angle)
        {
            bool found = false;
            int index = -1;
            for (int i = 0; i < Angles.Count() && found==false; i++)
            {
                if (angle == Angles[i])
                {// this will break us out of the loop
                    found = true;
                    index = i;
                }
            }

            if (found == true)
            {
                return index;
            }
            else
            {   // error code for not finding 
                return -1;
            }
        }

        /// <summary>
        /// This searches through the list of nodes and sees if any of the nodes is in the segment between the weight nodes we are looking at
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="startX"></param>
        /// <param name="endX"></param>
        /// <param name="startY"></param>
        /// <param name="endY"></param>
        /// <param name="foundIndex"></param>
        /// <returns></returns>
        public static bool SearchNodes(List<Node> nodes, int startX, int endX, int startY, int endY, out int foundIndex)
        {
            foundIndex = -1;
            for (int i = 0;i< nodes.Count();i++)
            {
                Node node = nodes.ElementAt(i);
                if ((node.x > startX && node.x < endX) || Node.IdenticalCoord(node.x,startX))
                {
                    if ((node.y > startY && node.y < endY) || Node.IdenticalCoord(node.y, startY))
                    {
                        foundIndex = i;
                        return true;
                    }
                }
            }

            // if not, we return false, should not happen if we have a closed loop path
            return false;
        }

        /// <summary>
        /// This function looks through the intersections of a square between weight nodes. It searches clockwise from the angle of entrance from the last segment
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="startNode"></param>
        /// <param name="startAngle"></param>
        /// <param name="foundAngle"></param>
        /// <returns></returns>
        public static Node SearchSegmentEnd(List<Node> nodes, Node startNode, int startAngle, out int foundAngle)
        {
            // find the end of the segment which starts at "startNode" and had angle coming in "startAngle"
            bool found = false;
            int foundIndex = -1;
            foundAngle = -1;
            int i = 0;

            // by making this loop run by the number of times there are angles, we permit dead-ends for the segment to go back on itself
            for (i = (GetAngleIndex(startAngle) + Angles.Count() - 1) % Angles.Count(); i <= Angles.Count() && found == false; i = (i + Angles.Count() - 1) % Angles.Count())
            {
                int x = (int)startNode.x;
                int ceilX = (int)Math.Ceiling(startNode.x);
                int floorX = (int)Math.Floor(startNode.x);
                int y = (int)startNode.y;
                int ceilY = (int)Math.Ceiling(startNode.y);
                int floorY = (int)Math.Floor(startNode.y);

                switch (Angles[i])
                {
                    case 0:
                        if (startNode.direction == Direction.Horizontal)
                        {
                            found = SearchNodes(nodes, x + 1, -1, floorY, ceilY, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 0;
                            }

                        }
                        else
                        {//  when vertical we don't have 0 degree search case
                            found = false;
                        }    
                        break;
                    case 45:
                        if (startNode.direction == Direction.Horizontal)
                        {
                            found = SearchNodes(nodes, x, x + 1, floorY, -1, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 45;
                            }
                        }
                        else
                        {//  when vertical we don't have 0 degree search case
                            found = SearchNodes(nodes, ceilX, -1, y - 1, y, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 45;
                            }
                        }    
                        break;
                    case 90:
                        if (startNode.direction == Direction.Horizontal)
                        {
                            found = false;
                        }
                        else
                        {//  when vertical we don't have 0 degree search case
                            found = SearchNodes(nodes, floorX, ceilX, y - 1, -1, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 90;
                            }
                        }    
                        break;
                    case 135:
                        if (startNode.direction == Direction.Horizontal)
                        {
                            found = SearchNodes(nodes, x - 1, x, floorY, -1, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 135;
                            }
                        }
                        else
                        {//  when vertical we don't have 0 degree search case
                            found = SearchNodes(nodes, floorX, -1, y - 1, y, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 135;
                            }
                        }    
                        break;
                    case 180:
                        if (startNode.direction == Direction.Horizontal)
                        {
                            found = SearchNodes(nodes, x - 1, -1, floorY, ceilY, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 180;
                            }
                        }
                        else
                        {//  when vertical we don't have 0 degree search case
                            found = false;
                        }    
                        break;
                    case 225:
                        if (startNode.direction == Direction.Horizontal)
                        {
                            found = SearchNodes(nodes, x - 1, x, ceilY, -1, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 225;
                            }
                        }
                        else
                        {//  when vertical we don't have 0 degree search case
                            found = SearchNodes(nodes, floorX, -1, y, y + 1, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 225;
                            }
                        }    
                        break;
                    case 270:
                        if (startNode.direction == Direction.Horizontal)
                        {
                            found = false;
                        }
                        else
                        {//  when vertical we don't have 0 degree search case
                            found = SearchNodes(nodes, floorX, ceilX, y + 1, -1, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 270;
                            }
                        }    
                        break;
                    case 315:
                        if (startNode.direction == Direction.Horizontal)
                        {
                            found = SearchNodes(nodes, x, x + 1, ceilY, -1, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 315;
                            }
                        }
                        else
                        {//  when vertical we don't have 0 degree search case
                            found = SearchNodes(nodes, ceilX, -1, y, y + 1, out foundIndex);
                            if ( found == true)
                            {
                                foundAngle = 315;
                            }
                        }    
                        break;
                }
            }

            if (found == true)
            {
                return nodes.ElementAt(foundIndex);
            }
            else
            {
                return null;
            }
        }
    }
}
