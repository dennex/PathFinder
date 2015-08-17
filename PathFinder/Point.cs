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

        public string ToString()
        {
            string myString = String.Format("({0:0.00},{1:0.00})", x,y);
            return myString;
        }
    }

    public class Node:Point
    {// a node is a point that has a concept of direction
        // the direction of a node is either horizontal or vertical
        public Direction direction;
        public int previousNode;
        public int nextNode;

        public Node(double x, double y, Direction direction):base(x,y)
        {
            this.direction = direction;
        }

        

        public static bool Identical(Node A, Node B)
        {// need to make this better
            if (IdenticalCoord(A.x,B.x) && IdenticalCoord(A.y,B.y)) // x and y very close, then point is identical
                return true;
            else 
                return false;
        }

        public static bool IdenticalCoord(double a, double b)
        {// this is to compare one coordinate, x or y
            if (Math.Abs(a - b) < 0.00001)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        public Segment(Node node1, Node node2)
        {
            this.point1 = new Point(node1.x, node1.y);
            this.point2 = new Point(node2.x, node2.y);

        }

        public string ToString()
        {
            string myString = String.Format("({0:0.00},{1:0.00})-({2:0.00},{3:0.00})", point1.x, point1.y, point2.x, point2.y);
            return myString;
        }
    }
}
