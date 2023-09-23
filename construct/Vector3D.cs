using System;

namespace slicer.construct
{
    internal class Vector3D
    {
        public double x;
        public double y;
        public double z;

        /// <summary>
        /// Constructor with zero default arguments
        /// </summary>
        public Vector3D(double x = 0.0, double y = 0.0, double z = 0.0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        ///  Overloaded constructor to pass a tupe as an argument
        /// </summary>
        /// <param name="xyz">A tuple of three entries</param>
        public Vector3D((double x, double y, double z) xyz)
        {
            x = xyz.x;
            y = xyz.y;
            z = xyz.z;
        }

        /// <summary>
        ///  Overloaded constructor to pass an array as an argument
        /// </summary>
        /// <param name="xyz">An array of three elements</param>
        public Vector3D(double[] xyz)
        {
            x = xyz[0];
            y = xyz[1];
            z = xyz[2];
        }

        /// <summary>
        ///  Overloaded constructor to pass an array as an argument
        /// </summary>
        /// <param name="xyz">An array of three elements</param>
        public Vector3D(int[] xyz)
        {
            x = Convert.ToDouble(xyz[0]);
            y = Convert.ToDouble(xyz[1]);
            z = Convert.ToDouble(xyz[2]);
        }

        /// <summary>
        /// Overloaded constructor to make uniform vector
        /// </summary>
        /// <param name="uniform"></param>
        public Vector3D(double uniform)
        {
            x = uniform;
            y = uniform;
            z = uniform;
        }

        #region Overloaded basic math operations

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3D operator +(Vector3D a, double b)
        {
            return new Vector3D(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3D operator -(Vector3D a, double b)
        {
            return new Vector3D(a.x - b, a.y - b, a.z - b);
        }

        /// <summary>
        /// Coordinate-wise multiplication
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3D operator *(Vector3D a, double b)
        {
            return new Vector3D(a.x * b, a.y * b, a.z * b);
        }

        /// <summary>
        /// Coordinate-wise division
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3D operator /(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3D operator /(Vector3D a, double b)
        {
            return new Vector3D(a.x / b, a.y / b, a.z / b);
        }

        /// <summary>
        /// Cross product
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Vector3D Cross(Vector3D b)
        {
            return new Vector3D(y * b.z - z * b.y,
                                z * b.x - x * b.z,
                                x * b.y - y * b.x);
        }

        /// <summary>
        /// Dot product
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public double Dot(Vector3D b)
        {
            return x * b.x + y * b.y + z * b.z;
        }

        #endregion

        public Vector3D Cloned
        {
            get
            {
                return new Vector3D(x, y, z);
            }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(Dot(this));
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

        public T[] AsArray<T>()
        {
            return new T[] {
                (T)Convert.ChangeType(x, typeof(T)),
                (T)Convert.ChangeType(y, typeof(T)),
                (T)Convert.ChangeType(z, typeof(T))
            };
        }

        public override bool Equals(object obj) => Equals(obj as Vector3D);

        public override int GetHashCode() => (x, y, z).GetHashCode();

        public static bool operator ==(Vector3D a, Vector3D b)
        {
            if (a is null)
            {
                if (b is null)
                {
                    return true;
                }
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(Vector3D a, Vector3D b) => !(a == b);

        public override string ToString()
        {
            return $"({x}; {y}; {z})";
        }
    }
}
