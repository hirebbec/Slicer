using slicer.construct;
using slicer.stl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slicer.Bulder
{
    public class Builder
    {
        internal Vertex CoordinateIntersection(Vertex p1, Vertex p2, Vertex f, Double3 normal)
        {
            var tDenom = normal.x * (p1.x - p2.x) + normal.y * (p1.y - p2.y) + normal.z * (p1.z - p2.z);
            if (tDenom == 0) return null;
            var d = -(f.x + f.y + f.z);
            var t = -(normal.x * p1.x + normal.y * p1.y + normal.z * p1.z + d) / tDenom;

            return new Vertex(new Double3(p1.x + t * (p2.x - p1.x), p1.y + t * (p2.y - p1.y), p1.z + t * (p2.z - p1.z)));
        }
    }
}
