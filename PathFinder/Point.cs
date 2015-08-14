using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    public enum Direction { Horizontal, Vertical };
    public enum Origin { Left, Right, Top, Bottom, Undefined };

    public class Point
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Node:Point
    {// a node is a point that has a concept of direction
        // the direction of a node is either horizontal or vertical
        public Direction direction;
        public Origin origin;// variable which states where the path got into the node

        public Node(double x, double y, Direction direction,Origin origin):base(x,y)
        {
            this.direction = direction;
            this.origin = origin;
        }

        public Node(double x, double y, Direction direction)
            : base(x, y)
        {
            this.direction = direction;
            this.origin = Origin.Undefined;
        }

        public static bool Identical(Node A, Node B)
        {
            if ((A.x == B.x) && (A.y == B.y)) // x and y are equal, then point is identical
                return true;
            else 
                return false;
        }
    }

    public class Segment
    {
        public Point point1;
        public Point point2;

        public Segment(Node node1)
        {
            if (node1.direction == Direction.Horizontal)
            {// horizontal node
                point1 = new Point(node1.x - 0.5, node1.y);
                point2 = new Point(node1.x + 0.5, node1.y);
            }
            else
            {// horizontal node
                point1 = new Point(node1.x, node1.y-0.5);
                point2 = new Point(node1.x, node1.y+0.5);
            }
        }
    }
}
