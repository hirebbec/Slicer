using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slicer
{
    internal class Utils
    {
        /// <summary>
        /// Метод для извлечения координат Vertex
        /// </summary>
        /// <param name="line"> Строка, над которой ведется работа</param>
        /// <param name="trimmer"> Символ, по которому происходит разделение</param>
        /// <returns></returns>
        public static double[] DoubleFromLine (string line, char trimmer)
        {
            double[] result = new double[3];

            string[] newline = line.Split(trimmer);
            result[0] = double.Parse(newline[1], CultureInfo.InvariantCulture);
            result[1] = double.Parse(newline[2], CultureInfo.InvariantCulture);
            result[2] = double.Parse(newline[3], CultureInfo.InvariantCulture);
            return result;

        }

    }
}
