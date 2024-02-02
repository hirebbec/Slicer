using slicer.construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slicer.stl
{
    public class Facet: ICloneable
    {
        public Double3 normal;
        public Vertex vertex1, vertex2, vertex3;
        public int number;

        public Facet(Vertex vertex1, Vertex vertex2, Vertex vertex3, Double3 normal, int number)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
            this.normal = normal;
            this.number = number;
        }

        public object Clone()
        {
            return new Facet(vertex1, vertex2, vertex3, normal, number);
        }
    }
}
