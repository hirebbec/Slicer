using slicer.construct;
using slicer.stl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace slicer.Bulder
{
    public class Builder
    {
        public static double minX, maxX, minY, maxY, minZ, maxZ;
        public static List<Vertex> globalVertex = new List<Vertex>();
        public static List<Vertex> cache = new List<Vertex>();
        public static Stl stl;
        public static Vertex currentPosition;
        public static Robot robot;
        public static void init(Stl stl, Robot robot)
        {
            Builder.stl = stl;
            Builder.robot = robot;
            goHome();
        }

        public static void AlongX()
        {            
            globalVertex.Clear();
            goHome();

            Stopwatch stopwatch = new Stopwatch();
            int iterationCount = 0;
            double totalTime = 0;
            DateTime startTime = DateTime.Now;
            stopwatch.Start();

            // Задаем начальные координаты робота
            Builder.currentPosition = new Vertex(minX, minY, minZ);

            while (currentPosition.z < maxZ + robot.HeightStep)
            {
                stopwatch.Restart();
                BuildPlaneZigzagX(stl.Facets, ref robot, ref currentPosition);
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = minX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + robot.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / robot.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }

        public static void AlongY()
        {
            globalVertex.Clear();
            goHome();

            Stopwatch stopwatch = new Stopwatch();
            int iterationCount = 0;
            double totalTime = 0;
            DateTime startTime = DateTime.Now;
            stopwatch.Start();

            // Задаем начальные координаты робота
            Builder.currentPosition = new Vertex(minX, minY, minZ);

            while (currentPosition.z < maxZ + robot.HeightStep)
            {
                stopwatch.Restart();
                BuildPlaneZigzagY(stl.Facets, ref robot, ref currentPosition);
                stopwatch.Stop();
                currentPosition.x = minX + robot.Overlap;
                currentPosition.y = minY + robot.Overlap;
                currentPosition.z = currentPosition.z + robot.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / robot.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное кол-во итераций: {0}", iterationsCount > 0 ? iterationsCount : 0);
                Console.Write("\rПримерное время ожидания: {0} секунд", estimatedTimeLeft);
            } // end while (currentZ < maxZ)
            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        /// <summary>
        /// Sawtooth path
        /// </summary>
        /// <param name="stl"></param>
        /// <param name="robot"></param>
        /// <param name="currentPosition"></param>
        private static void BuildPlaneZigzagY(List<Facet> facets, ref Robot robot, ref Vertex currentPosition)
        {
            while (currentPosition.x < maxX + robot.Overlap)
            {
                // going up
                Vertex rayOrigin = currentPosition;
                Vertex rayEnd = new Vertex(currentPosition.x, maxY + robot.Overlap, currentPosition.z);

                // finding intersection
                for (int i = 0; i < facets.Count(); i++)
                {
                    Facet facet = facets[i];
                    if (facet.vertex1.z < currentPosition.z && facet.vertex2.z < currentPosition.z && facet.vertex3.z < currentPosition.z)
                    {
                        facets.Remove(facet);
                    }
                    if (facet.vertex1.x < currentPosition.x && facet.vertex2.x < currentPosition.x && facet.vertex3.x < currentPosition.x)
                    {
                        facets.Remove(facet);
                    }
                    if (RayIntersectsTriangle(rayOrigin, rayEnd, facet))
                    {
                        Vertex v = CoordinateIntersection(rayOrigin, rayEnd, facet);
                        if (v != null)
                            cache.Add(v);
                    }
                }
                cache.Sort(delegate (Vertex one, Vertex two) { return one.y.CompareTo(two.y); });
                UpdateData();
                currentPosition.x = currentPosition.x + robot.Overlap;

                // going down
                rayOrigin = currentPosition;
                rayEnd = new Vertex(currentPosition.x, minY - robot.Overlap, currentPosition.z);

                // finding intersection
                foreach (Facet facet in facets)
                {
                    if (RayIntersectsTriangle(rayOrigin, rayEnd, facet))
                    {
                        Vertex v = CoordinateIntersection(rayOrigin, rayEnd, facet);
                        if (v != null)
                            cache.Add(v);
                    }
                }
                cache.Sort(delegate (Vertex one, Vertex two) { return one.y.CompareTo(two.y); });
                cache.Reverse();
                UpdateData();
                currentPosition.x = currentPosition.x + robot.Overlap;
            } // end while (currentPosition.x < maxX)
        }

        private static void BuildPlaneZigzagX(List<Facet> facets, ref Robot robot, ref Vertex currentPosition)
        {
            while (currentPosition.y < maxY + robot.Overlap)
            {
                // going right (parallel to X)
                Vertex rayOrigin = currentPosition;
                Vertex rayEnd = new Vertex(maxX + robot.Overlap, currentPosition.y, currentPosition.z);

                // finding intersection
                for (int i = 0; i < facets.Count(); i++)
                {
                    Facet facet = facets[i];
                    if (facet.vertex1.z < currentPosition.z && facet.vertex2.z < currentPosition.z && facet.vertex3.z < currentPosition.z)
                    {
                        facets.Remove(facet);
                    }
                    if (facet.vertex1.y < currentPosition.y && facet.vertex2.y < currentPosition.y && facet.vertex3.y < currentPosition.y)
                    {
                        facets.Remove(facet);
                    }
                    if (RayIntersectsTriangle(rayOrigin, rayEnd, facet))
                    {
                        Vertex v = CoordinateIntersection(rayOrigin, rayEnd, facet);
                        if (v != null)
                            cache.Add(v);
                    }
                }
                cache.Sort(delegate (Vertex one, Vertex two) { return one.x.CompareTo(two.x); });
                UpdateData();
                currentPosition.y = currentPosition.y + robot.Overlap;

                // going left (parallel to X)
                rayOrigin = currentPosition;
                rayEnd = new Vertex(minX - robot.Overlap, currentPosition.y, currentPosition.z);

                // finding intersection
                foreach (Facet facet in facets)
                {
                    if (RayIntersectsTriangle(rayOrigin, rayEnd, facet))
                    {
                        Vertex v = CoordinateIntersection(rayOrigin, rayEnd, facet);
                        if (v != null)
                            cache.Add(v);
                    }
                }
                cache.Sort(delegate (Vertex one, Vertex two) { return one.x.CompareTo(two.x); });
                cache.Reverse();
                UpdateData();
                currentPosition.y = currentPosition.y + robot.Overlap;
            } // end while (currentPosition.y < maxY)
        }

        public static void CrossToCross()
        {
            globalVertex.Clear();
            goHome();

            Stopwatch stopwatch = new Stopwatch();
            int iterationCount = 0;
            double totalTime = 0;
            DateTime startTime = DateTime.Now;
            stopwatch.Start();

            // Задаем начальные координаты робота
            Builder.currentPosition = new Vertex(minX, minY, minZ);
            int i = 0;

            while (currentPosition.z < maxZ + robot.HeightStep)
            {
                stopwatch.Restart();
                if (i % 2 == 0)
                {
                    BuildPlaneZigzagX(stl.getFacets(), ref robot, ref currentPosition);
                } else
                {
                    BuildPlaneZigzagY(stl.getFacets(), ref robot, ref currentPosition);
                }
                i++;
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = minX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + robot.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / robot.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }

        /// <summary>
        /// Find intersection coordinates
        /// </summary>
        /// <param name="rayOrigin">Ray start</param>
        /// <param name="prayEnd">Ray end</param>
        /// <param name="facet">Any facet's vertex</param>
        /// <param name="normal">Normal to facet</param>
        /// <returns>Coordinate of intersection</returns>
        private static Vertex CoordinateIntersection(Vertex rayOrigin, Vertex rayEnd, Facet facet)
        {
            Double3 normal = facet.normal;
            Vertex f = facet.vertex1;
            double tDenom = normal.x * (rayOrigin.x - rayEnd.x) + normal.y * (rayOrigin.y - rayEnd.y) + normal.z * (rayOrigin.z - rayEnd.z);
            if (Math.Abs(tDenom) < 0.00001) return null; // handle division by zero
            double d = -(normal.x * f.x + normal.y * f.y + normal.z * f.z);
            var t = (normal.x * rayOrigin.x + normal.y * rayOrigin.y + normal.z * rayOrigin.z + d) / tDenom;

            return new Vertex(new Double3(rayOrigin.x + t * (rayEnd.x - rayOrigin.x), rayOrigin.y + t * (rayEnd.y - rayOrigin.y), rayOrigin.z + t * (rayEnd.z - rayOrigin.z)));
        }



        /// <summary>
        /// Check whether the ray (or line patch) intersects the triangle
        /// </summary>
        /// <param name="rayOrigin">Ray start</param>
        /// <param name="rayEnd">Ray end</param>
        /// <param name="triVertCoords">Vertex coordinates of the triangle</param>
        /// <returns>Intersects or not</returns>
        private static bool RayIntersectsTriangle(Vertex rayEnd, Vertex rayOrigin, Facet facet)
        {
            double[] sv = new double[5]; // allocate storage for 5 instances of signed volumes
            bool intersects = false;

            Double3 rayOriginp = new Double3(rayOrigin.x, rayOrigin.y, rayOrigin.z);
            Double3 rayEndp = new Double3(rayEnd.x, rayEnd.y, rayEnd.z);

            // triangle vertices
            Double3[] p = new Double3[3];
            p[0].x = facet.vertex1.x; p[0].y = facet.vertex1.y; p[0].z = facet.vertex1.z;
            p[1].x = facet.vertex2.x; p[1].y = facet.vertex2.y; p[1].z = facet.vertex2.z;
            p[2].x = facet.vertex3.x; p[2].y = facet.vertex3.y; p[2].z = facet.vertex3.z;


            // ray (origin and end point)
            Double3[] r = new Double3[] { rayOriginp, rayEndp };

            sv[0] = Builder.SignedVolume(r[0], p[0], p[1], p[2]);
            sv[1] = Builder.SignedVolume(r[1], p[0], p[1], p[2]);

            sv[2] = Builder.SignedVolume(r[0], r[1], p[0], p[1]);
            sv[3] = Builder.SignedVolume(r[0], r[1], p[1], p[2]);
            sv[4] = Builder.SignedVolume(r[0], r[1], p[2], p[0]);

            if ((Math.Sign(sv[0]) * Math.Sign(sv[1])) <= 0)
            {
                intersects =
                    ((Math.Sign(sv[2]) == Math.Sign(sv[3])))
                    &&
                    ((Math.Sign(sv[3]) == Math.Sign(sv[4])));
            }

            return intersects;
        }

        /// <summary>
        /// Signed volume calculation
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        private static double SignedVolume(Double3 p, Double3 p1, Double3 p2, Double3 p3)
        {
            Double3 pp1 = p1 - p;
            Double3 pp2 = p2 - p;
            Double3 pp3 = p3 - p;

            double[][] edges = { pp1.ToArray<double>(), pp2.ToArray<double>(), pp3.ToArray<double>() };

            double det = (edges[0][0] * edges[1][1] * edges[2][2])
                        + (edges[0][1] * edges[1][2] * edges[2][0])
                        + (edges[0][2] * edges[1][0] * edges[2][1])
                        - (edges[0][2] * edges[1][1] * edges[2][0])
                        - (edges[0][0] * edges[1][2] * edges[2][1])
                        - (edges[0][1] * edges[1][0] * edges[2][2]);
            return (1.0 / 6.0 * det);
        }

        /// <summary>
        /// Add data in global list and clear cache
        /// </summary>
        private static void UpdateData()
        {
            if (cache.Count() % 2 == 0)
            {
                for (int i = 0; i < cache.Count; i++)
                {
                    Vertex added = cache[i];
                    globalVertex.Add(added);
                }
            }
            cache.Clear();
        }

        private static void goHome()
        {
            // Задаем координаты коробки с отступом от детали в половину Overlap
            minX = stl.MinX - robot.Overlap / 2.0; maxX = stl.MaxX + robot.Overlap / 2.0;
            minY = stl.MinY - robot.Overlap / 2.0; maxY = stl.MaxY + robot.Overlap / 2.0;
            minZ = stl.MinZ - robot.Overlap / 2.0; maxZ = stl.MaxZ + robot.Overlap / 2.0;
        }
    }
}
