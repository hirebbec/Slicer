using slicer.construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slicer.stl
{
    public class Vertex : ICloneable
    {
        public double x, y, z;
        public bool flag; // Если true, то переход без выключения

        public Vertex(Double3 coord, bool flag = true)
        {
            this.x = coord.x;
            this.y = coord.y;
            this.z = coord.z;
            this.flag = flag;
        }

        public Vertex(double[] coord, bool flag = true)
        {
            this.x = coord[0];
            this.y = coord[1];
            this.z = coord[2];
            this.flag = flag;
        }

        public Vertex(double coord1, double coord2, double coord3, bool flag = true)
        {
            this.x = coord1;
            this.y = coord2;
            this.z = coord3;
            this.flag = flag;
        }

        public object Clone()
        {
            return new Vertex(x, y, z, flag);
        }
    }
}
