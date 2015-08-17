using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathFinder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // create a test 2D matrix
            double[,] weights = new double[,]{{-3,-3,-3,-3,-2,-2,-2,-3,-3,-3},
            {-3,-2,-2,-1,-0.7,-1,-0.7,-1.1,-2,-3},
            {-3,-2,-1.1,-0.7,1,0.7,0.5,-0.3,-1.5,-3},
            {-3,-2,-0.7,0.5,1.5,1.5,1.2,0.9,-1.5,-3},
            {-3,-1.5,0.1,0.7,1.5,2,0.8,-0.9,-1.1,-3},
            {-3,-0.7,-0.5,0.3,1.5,1.5,-0.7,-1,-2,-3},
            {-3,-1,-0.7,-0.1,1,-1,-1,-1.5,-2,-3},
            {-3,-2,-1,-0.8,-0.9,-1,-2,-2,-2,-3},
            {-3,-2,-2,-2,-2,-2,-2,-2,-2,-3},
            {-3,-3,-3,-3,-3,-3,-3,-3,-3,-3}};

            
            List<Node>nodes = WeightMatrix.FindZeroes(weights);

            // this method gives us an ordered list of segments
            List<Segment> segmentsOrdered = WeightMatrix.NodesToSegmentsConnectivity(nodes);

        }

        
    }
}
