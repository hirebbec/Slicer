using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slicer.construct
{
    public struct Double3
    {
        public double x;
        public double y;
        public double z;

        /// <summary>
        /// A simple struct of 3 doubles pretty similar to Vector3D, but can utilize 100% of CPU power in ll mode
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="cz"></param>
        public Double3(double cx, double cy, double cz)
        {
            (x, y, z) = (cx, cy, cz);
        }

        public Double3(double uniform)
        {
            (x, y, z) = (uniform, uniform, uniform);
        }

        public Double3(double[] xyz)
        {
            (x, y, z) = (xyz[0], xyz[1], xyz[2]);
        }

        public Double3(int[] xyz)
        {
            x = Convert.ToDouble(xyz[0]);
            y = Convert.ToDouble(xyz[1]);
            z = Convert.ToDouble(xyz[2]);
        }

        public Double3((int tx, int ty, int tz) xyzTuple)
        {
            x = Convert.ToDouble(xyzTuple.tx);
            y = Convert.ToDouble(xyzTuple.ty);
            z = Convert.ToDouble(xyzTuple.tz);
        }

        /*public Double3 Plus(Double3 b)
        {
            return new Double3(this.x + b.x, this.y + b.y, this.z + b.z);
        }

        public Double3 Times(Double3 b)
        {
            return new Double3(this.x * b.x, this.y * b.y, this.z * b.z);
        }*/

        /// <summary>
        /// A vector with coordinates that are randomly distributed in [0..1]
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        public static Double3 Random(Random rand)
        {
            return new Double3(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());
        }

        /// <summary>
        /// A random vector with coords in range [min..max]
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="min">Min in range</param>
        /// <param name="max">Max in range</param>
        /// <returns></returns>
        public static Double3 RandomRange(Random rand, Double3 min, Double3 max)
        {
            return min + (max - min) * new Double3(rand.NextDouble(), rand.NextDouble(), rand.NextDouble());
        }

        #region Math, Double3

        public static Double3 operator +(Double3 a, Double3 b)
        {
            return new(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Double3 operator -(Double3 a, Double3 b)
        {
            return new(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Double3 operator *(Double3 a, Double3 b)
        {
            return new(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Double3 operator /(Double3 a, Double3 b)
        {
            return new(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        #endregion

        #region Math, double

        public static Double3 operator +(Double3 a, double b)
        {
            return new(a.x + b, a.y + b, a.z + b);
        }

        public static Double3 operator -(Double3 a, double b)
        {
            return new(a.x - b, a.y - b, a.z - b);
        }

        public static Double3 operator *(Double3 a, double b)
        {
            return new(a.x * b, a.y * b, a.z * b);
        }

        public static Double3 operator /(Double3 a, double b)
        {
            return new(a.x / b, a.y / b, a.z / b);
        }

        #endregion

        #region Math, vector-like operators

        /// <summary>
        /// Cross (vector) product
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Double3 Cross(Double3 b)
        {
            return new Double3(
                y * b.z - z * b.y,
                z * b.x - x * b.z,
                x * b.y - y * b.x);
        }

        /// <summary>
        /// Dot (scalar) product
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public double Dot(Double3 b)
        {
            return x * b.x + y * b.y + z * b.z;
        }

        /// <summary>
        /// Magnitude of the vector
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(Dot(this));
            }
        }

        public void FixNaN(Double3 healer)
        {
            if (IsNaN)
            {
                x = healer.x;
                y = healer.y;
                z = healer.z;
            }
        }

        public void FixNaN(double defaultVal = 0.0)
        {
            if (IsNaN)
            {
                x = defaultVal;
                y = defaultVal;
                z = defaultVal;
            }
        }

        public bool IsNaN
        {
            get
            {
                return double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z) ||
                        double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(z);
            }
        }

        public double Mean
        {
            get
            {
                return (x + y + z) / 3.0;
            }
        }

        #endregion

        public bool IsEqualTo(Double3 b)
        {
            return x == b.x && y == b.y && z == b.z;
        }

        public bool IsNotEqualTo(Double3 b)
        {
            return !IsEqualTo(b);
        }

        public Double3 Cloned
        {
            get
            {
                return new(x, y, z);
            }
        }

        /// <summary>
        /// Round-down to the nearest lower integer
        /// </summary>
        /// <returns></returns>
        public Double3 Floor
        {
            get
            {
                return new Double3(Math.Floor(x), Math.Floor(y), Math.Floor(z));
            }
        }

        /// <summary>
        /// Round-up to the nearest higher integer
        /// </summary>
        /// <returns></returns>
        public Double3 Ceil
        {
            get
            {
                return new Double3(Math.Ceiling(x), Math.Ceiling(y), Math.Ceiling(z));
            }
        }

        /// <summary>
        /// Call a lambda-expression 'fn' on each componnet of this vector
        /// </summary>
        /// <param name="fn">Lambda-function on each component</param>
        /// <returns>Modified vector</returns>
        public Double3 ComputeExpression(Func<double, double> fn)
        {
            return new Double3(fn(x), fn(y), fn(z));
        }

        public T[] ToArray<T>()
        {
            return new T[] {
                (T)Convert.ChangeType(x, typeof(T)),
                (T)Convert.ChangeType(y, typeof(T)),
                (T)Convert.ChangeType(z, typeof(T))
            };
        }
    }
}
