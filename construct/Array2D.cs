using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace slicer.construct
{
    class Array2D
    {
        public double[] Data1D;
        private int _rows;
        private int _cols;
        private int _totalElements;

        public Array2D(int rows, int cols, double? defaultVal = null)
        {
            _rows = rows;
            _cols = cols;
            _totalElements = _rows * _cols;

            // init a 1D flat array
            Data1D = new double[TotalElements];
            if ((object)defaultVal is not null) // double is not nullable, so convert it to object
            {
                Data1D = Enumerable.Range(0, TotalElements).
                                Select(v => (double)defaultVal).
                                ToArray();
            }
        }

        /// <summary>
        /// Independent copy
        /// </summary>
        public Array2D Cloned
        {
            get
            {
                Array2D clone = new Array2D(Rows, Cols);
                Array.Copy(Data1D, clone.Data1D, TotalElements);
                return clone;
            }
        }

        /// <summary>
        /// An empty Array2D of the same dimensions as this one
        /// </summary>
        public Array2D ClonedShape
        {
            get
            {
                return new Array2D(Rows, Cols);
            }
        }

        public int Rows
        {
            get
            {
                return _rows;
            }
        }

        public int Cols
        {
            get
            {
                return _cols;
            }
        }

        public int TotalElements
        {
            get
            {
                return _totalElements;
            }
        }

        /// <summary>
        /// Convert 2D indices into 1D index of the flat array
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public int ID1(int row, int col)
        {
            return col + row * Cols;
        }

        /// <summary>
        /// Convert 1D index of the flat array to 2D (row, col) indices
        /// </summary>
        /// <param name="id1"></param>
        /// <returns></returns>
        public (int, int) ID2(int id1)
        {
            return (Convert.ToInt32(Math.Floor(id1 / (double)Cols)), id1 % Cols);
        }

        public string PrintableID1
        {
            get
            {
                string str = "";

                for (int ir = 0; ir < Rows; ir++)
                {
                    for (int ic = 0; ic < Cols; ic++)
                    {
                        str += $"[{ir}, {ic}] -> [{ID1(ir, ic)}] \n";
                    }
                }

                return str;
            }
        }

        public string PrintableID2
        {
            get
            {
                string str = "";

                for (int i = 0; i < TotalElements; i++)
                {
                    (int row, int col) = ID2(i);
                    str += $"[{i}] -> [{row}, {col}] \n";
                }

                return str;
            }
        }
    }
}
