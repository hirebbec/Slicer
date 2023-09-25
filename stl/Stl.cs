using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using slicer.construct;
using slicer.io;


namespace slicer.stl
{
    public class Stl
    {
        private double _minX, _minY, _minZ, _maxX, _maxY, _maxZ;
        private List<Facet> _facets;

        /// <summary>
        /// Формирование экземпляра класса STl
        /// </summary>
        /// <param name="filePath"> Путь к STL детали</param>
        public Stl(string filePath)
        {
            StructureForming(filePath);
            FindBoxCoords();
        }

        public double MinX 
        {
            get { return _minX; }
        }

        public double MinY
        {
            get { return _minY; }
        }

        public double MinZ
        {
            get { return _minZ; }
        }

        public double MaxX
        {
            get { return _maxX; }
        }

        public double MaxY
        {
            get { return _maxY; }
        }

        public double MaxZ
        {
            get { return _maxZ; }
        }

        /// <summary>
        /// Формирует структуру STL детали
        /// </summary>
        /// <param name="filePath"></param>
        public void StructureForming(string filePath)
        {

            List<string> lines = TextFileReader.LinesFromFile(filePath); // All lines from stl file
            List<Facet> facets = new List<Facet>();
            int number = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Split(' ')[0] == "facet")
                {
                    double cn1 = double.Parse(lines[i].Split(' ')[2], CultureInfo.InvariantCulture);
                    double cn2 = double.Parse(lines[i].Split(' ')[3], CultureInfo.InvariantCulture);
                    double cn3 = double.Parse(lines[i].Split(' ')[4], CultureInfo.InvariantCulture);
                    Double3 normal = new Double3(cn1, cn2, cn3);
                    Vertex v1 = new Vertex(Utils.DoubleFromLine(lines[i + 2], ' '));
                    Vertex v2 = new Vertex(Utils.DoubleFromLine(lines[i + 3], ' '));
                    Vertex v3 = new Vertex(Utils.DoubleFromLine(lines[i + 4], ' '));
                    Facet f = new Facet(v1, v2, v3, normal, number);
                    facets.Add(f);
                    number++;
                }
            }
            _facets = facets;

        }

        /// <summary>
        /// Метод для нахождения минимальных и максимальных координат STL детали.
        /// </summary>
        public void FindBoxCoords()
        {
            double minx = _facets[0].vertex1.x;
            double maxx = _facets[0].vertex1.x;

            double miny = _facets[0].vertex1.y;
            double maxy = _facets[0].vertex1.y;

            double minz = _facets[0].vertex1.z;
            double maxz = _facets[0].vertex1.z;

            foreach (Facet f in _facets)
            {
                // sort min element
                if (f.vertex1.x < minx) minx = f.vertex1.x;
                if (f.vertex2.x < minx) minx = f.vertex2.x;
                if (f.vertex3.x < minx) minx = f.vertex3.x;

                if (f.vertex1.y < miny) miny = f.vertex1.y;
                if (f.vertex2.y < miny) miny = f.vertex2.y;
                if (f.vertex3.y < miny) miny = f.vertex3.y;

                if (f.vertex1.z < minz) minz = f.vertex1.z;
                if (f.vertex2.z < minz) minz = f.vertex2.z;
                if (f.vertex3.z < minz) minz = f.vertex3.z;

                // sort max element
                if (f.vertex1.x > maxx) maxx = f.vertex1.x;
                if (f.vertex2.x > maxx) maxx = f.vertex2.x;
                if (f.vertex3.x > maxx) maxx = f.vertex3.x;

                if (f.vertex1.y > maxy) maxy = f.vertex1.y;
                if (f.vertex2.y > maxy) maxy = f.vertex2.y;
                if (f.vertex3.y > maxy) maxy = f.vertex3.y;

                if (f.vertex1.z > maxz) maxz = f.vertex1.z;
                if (f.vertex2.z > maxz) maxz = f.vertex2.z;
                if (f.vertex3.z > maxz) maxz = f.vertex3.z;
            }

            _minX = minx; _minY = miny; _minZ = minz;
            _maxX = maxx; _maxY = maxy; _maxZ = maxz;
        }
    }
}
