using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slicer.construct
{
    abstract public class Array3DBase
    {
        // possible slicing directions
        public enum SlicePlane
        {
            XY, YZ, ZX
        }

        public static readonly int LARGE_ITEMS_COUNT = 5 * 5 * 5;
        // protected (accessible in derived classes) members
        protected int _lenX, _lenY, _lenZ; // lengths in each direction
        protected int _lenXY, _lenYZ, _lenZX; // elements in each slice planes
        protected Double3 _len; // packed (_lenX, _lenY, _lenZ)
        protected int _totalElements; // length of 1D data array
        protected ParallelOptions _llopt;

        private void Init(int lengthX, int lengthY, int lengthZ)
        {
            _lenX = lengthX;
            _lenY = lengthY;
            _lenZ = lengthZ;

            _lenXY = _lenX * _lenY;
            _lenYZ = _lenY * _lenZ;
            _lenZX = _lenZ * _lenX;

            _len = new Double3(_lenX, _lenY, _lenZ);
            _totalElements = _lenX * _lenY * _lenZ;
        }

        /// <summary>
        /// Flat data storage with 3D indexing wrapper optimized for parallel calls:
        /// 
        ///     z (depth)
        ///    /
        ///   /
        ///   --------- x (horizontal)
        ///   |
        ///   |
        ///   |
        ///   y (vertical)
        /// 
        /// </summary>
        /// <param name="lengthX"></param>
        /// <param name="lengthY"></param>
        /// <param name="lengthZ"></param>
        /// <param name="treatAs"></param>
        public Array3DBase(int lengthX, int lengthY, int lengthZ)
        {
            Init(lengthX, lengthY, lengthZ);
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="lengthXYZ"></param>
        /// <param name="treatAs"></param>
        public Array3DBase(int[] lengthXYZ)
        {
            Init(lengthXYZ[0], lengthXYZ[1], lengthXYZ[2]);
        }

        public ParallelOptions llopt
        {
            get
            {
                return _llopt;
            }
        }

        /// <summary>
        /// Check if items count exceeds some predefined threshold
        /// </summary>
        public bool IsLarge
        {
            get
            {
                return Count > LARGE_ITEMS_COUNT;
            }
        }

        /// <summary>
        /// Elements count in X direction
        /// </summary>
        public int LengthX
        {
            get
            {
                return _lenX;
            }
        }

        /// <summary>
        /// Elements count in Y direction
        /// </summary>
        public int LengthY
        {
            get
            {
                return _lenY;
            }
        }

        /// <summary>
        /// Elements count in Z direction
        /// </summary>
        public int LengthZ
        {
            get
            {
                return _lenZ;
            }
        }

        /// <summary>
        /// A vector containing the elements count in each direction
        /// </summary>
        public Double3 Length
        {
            get
            {
                return _len;
            }
        }

        /// <summary>
        /// Total count of stored elements including all directions
        /// </summary>
        public int Count
        {
            get
            {
                return _totalElements;
            }
        }

        /// <summary>
        /// Elements count in a XY-plane slice
        /// </summary>
        public int LengthXY
        {
            get
            {
                return _lenXY;
            }
        }

        /// <summary>
        /// Elements count in a YZ-plane slice
        /// </summary>
        public int LengthYZ
        {
            get
            {
                return _lenYZ;
            }
        }

        /// <summary>
        /// Elements count in a ZX-plane slice
        /// </summary>
        public int LengthZX
        {
            get
            {
                return _lenZX;
            }
        }

        /// <summary>
        /// 3D indices to 1D index regardless the actual data of the Array3DofDouble
        /// </summary>
        /// <param name="ix"></param>
        /// <param name="iy"></param>
        /// <param name="iz"></param>
        /// <param name="length">Array3DBase length in X,Y,Z directions</param>
        /// <returns></returns>
        public static int ID1(int ix, int iy, int iz, Double3 length)
        {
            return ix + (int)length.x * (iy + iz * (int)length.y);
        }

        /// <summary>
        /// 3D indices to 1D index
        /// </summary>
        /// <param name="ix"></param>
        /// <param name="iy"></param>
        /// <param name="iz"></param>
        /// <returns></returns>
        public int ID1(int ix, int iy, int iz)
        {
            return ID1(ix, iy, iz, Length);
            //return ix + this.LengthX * (iy + iz * this.LengthY);
        }

        /// <summary>
        /// 1D index to 3D indices
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public (int, int, int) ID3(int i)
        {
            var iOverX = i / LengthX;
            int ix = i % LengthX;
            int iy = iOverX % LengthY;
            int iz = iOverX / LengthY;
            return (ix, iy, iz);
        }

        /// <summary>
        /// Checks whether current global 3D index of the element is on the array's boundary planes
        /// </summary>
        /// <param name="ix"></param>
        /// <param name="iy"></param>
        /// <param name="iz"></param>
        /// <returns></returns>
        public bool IsOnBoundary(int ix, int iy, int iz)
        {
            return ix == 0 || ix == LengthX - 1
                || iy == 0 || iy == LengthY - 1
                || iz == 0 || iz == LengthZ - 1;
        }

        /// <summary>
        /// Checks whether current global 1D index of the element is on the array's boundary planes
        /// </summary>
        /// <param name="ix"></param>
        /// <param name="iy"></param>
        /// <param name="iz"></param>
        /// <returns></returns>
        public bool IsOnBoundary(int i)
        {
            (int ix, int iy, int iz) = ID3(i);
            return ix == 0 || ix == LengthX - 1
                || iy == 0 || iy == LengthY - 1
                || iz == 0 || iz == LengthZ - 1;
        }

        /// <summary>
        /// 1D global indices of all elements in a slice plane sampled in a given level
        /// </summary>
        /// <param name="slicePlane">Slice plane type</param>
        /// <param name="sliceLevel">Slice level (its id in a direction perpendicular to slice plane)</param>
        /// <returns></returns>
        public int[] SliceID1(SlicePlane slicePlane, int sliceLevel)
        {
            int len2d = 0; // elements count in the slice
            int[] len; // elements count in each of two directions of the slice plane
            int[] ids = null; // returned 1D ids of the elements in the slice plane
            int[] order = null; // how do 'len0', 'len1' and the 'sliceLevel' should be arranged

            len = new int[2];

            switch (slicePlane)
            {
                case SlicePlane.XY:
                    len2d = LengthXY;
                    len[0] = LengthX;
                    len[1] = LengthY;
                    order = new int[] { 0, 1, 2 };
                    break;
                case SlicePlane.YZ:
                    len2d = LengthYZ;
                    len[0] = LengthY;
                    len[1] = LengthZ;
                    order = new int[] { 2, 0, 1 };
                    break;
                case SlicePlane.ZX:
                    len2d = LengthZX;
                    len[0] = LengthZ;
                    len[1] = LengthX;
                    order = new int[] { 1, 2, 0 };
                    break;
                default:
                    break;
            }

            // return nothing
            if (len2d == 0)
            {
                return null;
            }

            // allocate a new empty array
            ids = new int[len2d];

            // a local run-time-subroutine
            // to convert local 1D (pseudo-2D) indices within a slice plane
            // to global 1D indices of elements within this 3D array
            Func<int, int> local1DToGlobal1D = (i) =>
            {
                // convert local 1D index to local 2D ones
                var iOver1 = i / len[1];
                int i1 = i % len[1];
                int i0 = iOver1 % len[0];
                // Example: let it be the YZ plane
                // then order = new int[] { 2, 0, 1 };
                // and should be id3o = { sliceLevel, i0, i1 } = { id3[2], id3[0], ide3[1] }
                int[] id3 = new int[] { i0, i1, sliceLevel }; // unordered 3-ids
                int[] id3o = new int[] { id3[order[0]], id3[order[1]], id3[order[2]] }; // ordered 3-ids
                // convert local 3D ids to the global 1D id
                return ID1(id3o[0], id3o[1], id3o[2]);
            };
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = local1DToGlobal1D(i);
            }

            return ids;
        }

        /// <summary>
        /// Show a 3D -> 1D -> 3D indices mapping
        /// </summary>
        public string PrintableIDMapping
        {
            get
            {
                string str = "";

                /*
                for (int iz = 0; iz < LengthZ; iz++)
                {
                    for (int iy = 0; iy < LengthY; iy++)
                    {
                        for (int ix = 0; ix < LengthX; ix++)
                        {
                            int i = ID1(ix, iy, iz);
                            (int cix, int ciy, int ciz) = ID3(i);
                            str += $"[{ix}, {iy}, {iz}] -> [{i}] -> [{cix}, {ciy}, {ciz}] \n";
                        }
                    }
                }
                */

                for (int i = 0; i < Count; i++)
                {
                    (int ix, int iy, int iz) = ID3(i);
                    int ci = ID1(ix, iy, iz);
                    (int cix, int ciy, int ciz) = ID3(ci);

                    str += $"[{i}] -> [{ix}, {iy}, {iz}] -> [{ci}] -> [{cix}, {ciy}, {ciz}] \n";
                }

                return str;
            }
        }
    }
}
