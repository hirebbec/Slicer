﻿using slicer.construct;
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

        /// <summary>
        /// Создается экземпляр Vertex с координатами x,y,z
        /// </summary>
        /// <param name="coord"> Координаты вершины Vertex</param>
        public Vertex(Double3 coord)
        {
            this.x = coord.x;
            this.y = coord.y;
            this.z = coord.z;
        }

        /// <summary>
        /// Перегрузка конструктора класса
        /// </summary>
        /// <param name="coord"> Координаты вершины Vertex</param>
        public Vertex(double[] coord)
        {
            this.x = coord[0];
            this.y = coord[1];
            this.z = coord[2];
        }
    }
}
