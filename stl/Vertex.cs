using slicer.construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slicer.stl
{
    internal class Vertex
    {
        public double x, y, z;
        public Vertex(Double3 coord)
        {
            x = coord.x;
            y = coord.y;
            z = coord.z;
        }

        public Vertex(double[] coord)
        {
            x = coord[0];
            y = coord[1];
            z = coord[2];
        }
    }
}
