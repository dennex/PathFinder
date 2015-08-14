using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    class WeightMatrix
    {
        public static double GetZeroCrossing(Point point1, Point point2)
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
        /// The trick here is that if we assume the weights are linearly interpolated, we just need to find the exact positions of the zeroes
        /// in between the nodes, then link the segments. This function finds the positions of the zeroes.
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
                    if (weights[i, j] >= 0)
                    {// check neighbors for negative
                        if ((i-1>=0) && weights[i - 1, j] < 0) // top neighbor
                        {
                            double interpolatedCoordinate =
                                GetZeroCrossing(new Point(i - 1, weights[i - 1, j]),
                                                new Point(i, weights[i, j]));
                            nodes.Add(new Node(j, interpolatedCoordinate,Direction.Horizontal));
                        }

                        if ((j-1>=0) && weights[i, j - 1] < 0) // left neighbor
                        {
                            double interpolatedCoordinate =
                                GetZeroCrossing(new Point(j - 1, weights[i, j - 1]),
                                                new Point(j, weights[i, j]));
                            nodes.Add(new Node(interpolatedCoordinate, i,Direction.Vertical));
                        }

                        if ((i+1<weights.GetLength(0)) && weights[i + 1, j] < 0) // bottom neighbor
                        {
                            double interpolatedCoordinate =
                                GetZeroCrossing(new Point(i, weights[i, j]),
                                                new Point(i + 1, weights[i + 1, j]));
                            nodes.Add(new Node(j, interpolatedCoordinate,Direction.Horizontal));
                        }

                        if (j+1<weights.GetLength(1) && weights[i, j + 1] < 0) // right neighbor
                        {
                            double interpolatedCoordinate =
                                GetZeroCrossing(new Point(j, weights[i, j]),
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
    }
}
