using slicer.construct;
using slicer.stl;
using System.Diagnostics;

namespace slicer.Bulder
{
    public class Builder
    {
        public static double minX, maxX, minY, maxY, minZ, maxZ;
        public static List<Vertex> globalVertex = new List<Vertex>();
        public static List<Vertex> cache = new List<Vertex>();
        public static List<List<Vertex>> smartCache = new List<List<Vertex>>();
        public static Stl stl;
        public static Vertex currentPosition;
        public static Settings settings;
        public static void init(Stl stl, Settings settings)
        {
            Builder.stl = stl;
            Builder.settings = settings;
            goHome();
        }
        public static void BuildPlaneX()
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
            bool flag = false;

            while (currentPosition.z < maxZ)
            {
                for (int i = 0; i < stl.Facets.Count(); i++)
                {
                    if (stl.Facets[i].vertex1.z < currentPosition.z && stl.Facets[i].vertex2.z < currentPosition.z && stl.Facets[i].vertex3.z < currentPosition.z)
                    {
                        stl.Facets.Remove(stl.Facets[i]);
                        i--;
                    }
                }

                stopwatch.Restart();
                AlongX(stl.getFacets(), ref settings, ref currentPosition);
                if (flag)
                {
                    cache.Reverse();
                }
                flag = !flag;
                UpdateData();
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = minX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + settings.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / settings.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        public static void BuildPlaneReverseX()
        {
            globalVertex.Clear();
            goHome();

            Stopwatch stopwatch = new Stopwatch();
            int iterationCount = 0;
            double totalTime = 0;
            DateTime startTime = DateTime.Now;
            stopwatch.Start();

            // Задаем начальные координаты робота
            Builder.currentPosition = new Vertex(maxX, minY, minZ);
            bool flag = false;

            while (currentPosition.z < maxZ)
            {
                for (int i = 0; i < stl.Facets.Count(); i++)
                {
                    if (stl.Facets[i].vertex1.z < currentPosition.z && stl.Facets[i].vertex2.z < currentPosition.z && stl.Facets[i].vertex3.z < currentPosition.z)
                    {
                        stl.Facets.Remove(stl.Facets[i]);
                        i--;
                    }
                }

                stopwatch.Restart();
                AlongReverseX(stl.getFacets(), ref settings, ref currentPosition);
                if (flag)
                {
                    cache.Reverse();
                }
                flag = !flag;
                UpdateData();
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = maxX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + settings.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / settings.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        private static void rayX(List<Facet> facets)
        {
            Vertex rayOrigin = currentPosition;
            Vertex rayEnd = new Vertex(maxX, currentPosition.y, currentPosition.z);
            List<Vertex> tmp = new List<Vertex>();

            foreach (Facet facet in facets)
            {
                if (RayIntersectsTriangle(rayOrigin, rayEnd, facet))
                {
                    Vertex v = CoordinateIntersection(rayOrigin, rayEnd, facet);
                    if (v != null)
                        tmp.Add(v);
                }
            }
            tmp.Sort(delegate (Vertex one, Vertex two) { return one.x.CompareTo(two.x); });
            cache.AddRange(tmp);
        }
        private static void rayReverseX(List<Facet> facets)
        {
            Vertex rayOrigin = currentPosition;
            Vertex rayEnd = new Vertex(minX, currentPosition.y, currentPosition.z);
            List<Vertex> tmp = new List<Vertex>();

            foreach (Facet facet in facets)
            {
                if (RayIntersectsTriangle(rayOrigin, rayEnd, facet))
                {
                    Vertex v = CoordinateIntersection(rayOrigin, rayEnd, facet);
                    if (v != null)
                        tmp.Add(v);
                }
            }
            tmp.Sort(delegate (Vertex one, Vertex two) { return two.x.CompareTo(one.x); });
            cache.AddRange(tmp);
        }
        private static void AlongX(List<Facet> facets, ref Settings robot, ref Vertex currentPosition)
        {
            while (currentPosition.y < maxY)
            {
                for (int i = 0; i < facets.Count(); i++)
                {
                    if (facets[i].vertex1.y < currentPosition.y && facets[i].vertex2.y < currentPosition.y && facets[i].vertex3.y < currentPosition.y)
                    {
                        facets.Remove(facets[i]);
                        i--;
                    }
                }
                rayX(facets);
                currentPosition.y = currentPosition.y + robot.Overlap;
            }
        }
        private static void AlongReverseX(List<Facet> facets, ref Settings robot, ref Vertex currentPosition)
        {
            while (currentPosition.y < maxY)
            {
                for (int i = 0; i < facets.Count(); i++)
                {
                    if (facets[i].vertex1.y < currentPosition.y && facets[i].vertex2.y < currentPosition.y && facets[i].vertex3.y < currentPosition.y)
                    {
                        facets.Remove(facets[i]);
                        i--;
                    }
                }
                rayReverseX(facets);
                currentPosition.y = currentPosition.y + robot.Overlap;
            }
        }
        public static void BuildPlaneY()
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
            bool flag = false;

            while (currentPosition.z < maxZ)
            {
                for (int i = 0; i < stl.Facets.Count(); i++)
                {
                    if (stl.Facets[i].vertex1.z < currentPosition.z && stl.Facets[i].vertex2.z < currentPosition.z && stl.Facets[i].vertex3.z < currentPosition.z)
                    {
                        stl.Facets.Remove(stl.Facets[i]);
                        i--;
                    }
                }

                stopwatch.Restart();
                AlongY(stl.getFacets(), ref settings, ref currentPosition);
                if (flag)
                {
                    cache.Reverse();
                }
                flag = !flag;
                UpdateData();
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = minX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + settings.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / settings.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        public static void BuildPlaneReverseY()
        {
            globalVertex.Clear();
            goHome();

            Stopwatch stopwatch = new Stopwatch();
            int iterationCount = 0;
            double totalTime = 0;
            DateTime startTime = DateTime.Now;
            stopwatch.Start();

            // Задаем начальные координаты робота
            Builder.currentPosition = new Vertex(minX, maxY, minZ);
            bool flag = false;

            while (currentPosition.z < maxZ)
            {
                for (int i = 0; i < stl.Facets.Count(); i++)
                {
                    if (stl.Facets[i].vertex1.z < currentPosition.z && stl.Facets[i].vertex2.z < currentPosition.z && stl.Facets[i].vertex3.z < currentPosition.z)
                    {
                        stl.Facets.Remove(stl.Facets[i]);
                        i--;
                    }
                }

                stopwatch.Restart();
                AlongReverseY(stl.getFacets(), ref settings, ref currentPosition);
                if (flag)
                {
                    cache.Reverse();
                }
                flag = !flag;
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = minX;
                currentPosition.y = maxY;
                currentPosition.z = currentPosition.z + settings.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / settings.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        private static void AlongY(List<Facet> facets, ref Settings robot, ref Vertex currentPosition)
        {
            while (currentPosition.x < maxX)
            {
                for (int i = 0; i < facets.Count(); i++)
                {
                    if (facets[i].vertex1.x < currentPosition.x && facets[i].vertex2.x < currentPosition.x && facets[i].vertex3.x < currentPosition.x)
                    {
                        facets.Remove(facets[i]);
                        i--;
                    }
                }
                rayY(facets);
                currentPosition.x = currentPosition.x + robot.Overlap;
            }
        }
        private static void AlongReverseY(List<Facet> facets, ref Settings robot, ref Vertex currentPosition)
        {
            while (currentPosition.x < maxX)
            {
                for (int i = 0; i < facets.Count(); i++)
                {
                    if (facets[i].vertex1.x < currentPosition.x && facets[i].vertex2.x < currentPosition.x && facets[i].vertex3.x < currentPosition.x)
                    {
                        facets.Remove(facets[i]);
                        i--;
                    }
                }
                rayReverseY(facets);
                currentPosition.x = currentPosition.x + robot.Overlap;
            }
        }
        private static void rayY(List<Facet> facets)
        {
            // going right (parallel to X)
            Vertex rayOrigin = currentPosition;
            Vertex rayEnd = new Vertex(currentPosition.x, maxY, currentPosition.z);
            List<Vertex> tmp = new List<Vertex>();

            // finding intersection
            foreach (Facet facet in facets)
            {
                if (RayIntersectsTriangle(rayOrigin, rayEnd, facet))
                {
                    Vertex v = CoordinateIntersection(rayOrigin, rayEnd, facet);
                    if (v != null)
                        tmp.Add(v);
                }
            }
            tmp.Sort(delegate (Vertex one, Vertex two) { return one.y.CompareTo(two.y); });
            cache.AddRange(tmp);
        }
        private static void rayReverseY(List<Facet> facets)
        {
            Vertex rayOrigin = currentPosition;
            Vertex rayEnd = new Vertex(currentPosition.x, minY, currentPosition.z);
            List<Vertex> tmp = new List<Vertex>();
            foreach (Facet facet in facets)
            {
                if (RayIntersectsTriangle(rayOrigin, rayEnd, facet))
                {
                    Vertex v = CoordinateIntersection(rayOrigin, rayEnd, facet);
                    if (v != null)
                        tmp.Add(v);
                }
            }
            tmp.Sort(delegate (Vertex one, Vertex two) { return two.y.CompareTo(one.y); });
            cache.AddRange(tmp);
        }
        public static void BuildPlaneCrossToCross()
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

            while (currentPosition.z < maxZ)
            {
                for (int j = 0; j < stl.Facets.Count(); j++)
                {
                    if (stl.Facets[j].vertex1.z < currentPosition.z && stl.Facets[j].vertex2.z < currentPosition.z && stl.Facets[j].vertex3.z < currentPosition.z)
                    {
                        stl.Facets.Remove(stl.Facets[j]);
                        j--;
                    }
                }
                stopwatch.Restart();
                if (i % 2 == 0)
                {
                    AlongX(stl.getFacets(), ref settings, ref currentPosition);
                } else
                {
                    AlongY(stl.getFacets(), ref settings, ref currentPosition);
                }
                i++;
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;
                UpdateData();

                currentPosition.x = minX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + settings.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / settings.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        public static void BuildPlaneSnakeX()
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
            bool flag = false;

            while (currentPosition.z < maxZ)
            {
                for (int j = 0; j < stl.Facets.Count(); j++)
                {
                    if (stl.Facets[j].vertex1.z < currentPosition.z && stl.Facets[j].vertex2.z < currentPosition.z && stl.Facets[j].vertex3.z < currentPosition.z)
                    {
                        stl.Facets.Remove(stl.Facets[j]);
                        j--;
                    }
                }
                stopwatch.Restart();
                SnakeX(stl.getFacets(), ref settings, ref currentPosition);
                if (flag)
                {
                    cache.Reverse();
                }
                flag = !flag;
                UpdateData();
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = minX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + settings.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / settings.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        private static void SnakeX(List<Facet> facets, ref Settings robot, ref Vertex currentPosition)
        {
            int j = 0;
            while (currentPosition.y < maxY)
            {
                for (int i = 0; i < facets.Count(); i++)
                {
                    if (facets[i].vertex1.y < currentPosition.y && facets[i].vertex2.y < currentPosition.y && facets[i].vertex3.y < currentPosition.y)
                    {
                        facets.Remove(facets[i]);
                        i--;
                    }
                }
                if (j % 2 == 0)
                {
                    rayX(facets);
                    currentPosition.x = maxX;
                } else
                {
                    rayReverseX(facets);
                    currentPosition.x = minX;
                }
                j++;
                currentPosition.y = currentPosition.y + robot.Overlap;
            }
        }
        public static void BuildPlaneSnakeY()
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
            bool flag = false;

            while (currentPosition.z < maxZ)
            {
                for (int j = 0; j < stl.Facets.Count(); j++)
                {
                    if (stl.Facets[j].vertex1.z < currentPosition.z && stl.Facets[j].vertex2.z < currentPosition.z && stl.Facets[j].vertex3.z < currentPosition.z)
                    {
                        stl.Facets.Remove(stl.Facets[j]);
                        j--;
                    }
                }
                stopwatch.Restart();
                SnakeY(stl.getFacets(), ref settings, ref currentPosition);
                if (flag)
                {
                    cache.Reverse();
                }
                flag = !flag;
                UpdateData();
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = minX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + settings.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / settings.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        private static void SnakeY(List<Facet> facets, ref Settings robot, ref Vertex currentPosition)
        {
            int j = 0;
            while (currentPosition.x < maxX)
            {
                for (int i = 0; i < facets.Count(); i++)
                {
                    if (facets[i].vertex1.x < currentPosition.x && facets[i].vertex2.x < currentPosition.x && facets[i].vertex3.x < currentPosition.x)
                    {
                        facets.Remove(facets[i]);
                        i--;
                    }
                }
                if (j % 2 == 0)
                {
                    rayY(facets);
                    currentPosition.y = maxY;
                }
                else
                {
                    rayReverseY(facets);
                    currentPosition.y = minY;
                }
                j++;
                currentPosition.x = currentPosition.x + robot.Overlap;
            }
        }

        public static void BuildPlaneSmartSnakeX()
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
            bool flag = false;

            while (currentPosition.z < maxZ)
            {
                for (int j = 0; j < stl.Facets.Count(); j++)
                {
                    if (stl.Facets[j].vertex1.z < currentPosition.z && stl.Facets[j].vertex2.z < currentPosition.z && stl.Facets[j].vertex3.z < currentPosition.z)
                    {
                        stl.Facets.Remove(stl.Facets[j]);
                        j--;
                    }
                }
                stopwatch.Restart();
                SmartSnakeX(stl.getFacets(), ref settings, ref currentPosition, flag);
                flag = !flag;
                UpdateData();
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalSeconds;

                currentPosition.x = minX;
                currentPosition.y = minY;
                currentPosition.z = currentPosition.z + settings.HeightStep;

                iterationCount++;

                double averageTimePerIteration = totalTime / iterationCount;
                double iterationsCount = (maxZ - currentPosition.z) / settings.HeightStep;
                double estimatedTimeLeft = averageTimePerIteration * iterationsCount;

                Console.Write("\rПримерное время ожидания: {0} секунд   ", Math.Round(estimatedTimeLeft), 2);
            } // end while (currentZ < maxZ)

            stopwatch.Stop();

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            Console.WriteLine($"\rВремя выполнения: {elapsedTime.TotalSeconds} секунд                              ");
            Console.WriteLine($"Количество итераций: {iterationCount}");
        }
        private static void SmartSnakeX(List<Facet> facets, ref Settings robot, ref Vertex currentPosition, bool flag)
        {
            int j = 0;
            while (currentPosition.y < maxY)
            {
                for (int i = 0; i < facets.Count(); i++)
                {
                    if (facets[i].vertex1.y < currentPosition.y && facets[i].vertex2.y < currentPosition.y && facets[i].vertex3.y < currentPosition.y)
                    {
                        facets.Remove(facets[i]);
                        i--;
                    }
                }
                rayX(facets);
                if (cache.Count() > 1)
                {
                    if (cache.Count() % 2 == 0)
                    {
                        for (int i = 0; i < cache.Count(); i += 2)
                        {
                            if (smartCache.Count() <= i / 2)
                            {
                                smartCache.Add(new List<Vertex>());
                            }
                            if (j % 2 == 0)
                            {
                                smartCache[i / 2].Add(cache[i]);
                                smartCache[i / 2].Add(cache[i + 1]);
                            }
                            else
                            {
                                smartCache[i / 2].Add(cache[i + 1]);
                                smartCache[i / 2].Add(cache[i]);
                            }
                        }
                    }
                }
                cache.Clear();
                j++;
                currentPosition.y = currentPosition.y + robot.Overlap;
            }
            if (flag)
                smartCache.Reverse();
            cache.Clear();
            for (int i = 0; i < smartCache.Count(); i++)
            {
                if (flag)
                {
                    smartCache[i].Reverse();
                }
                smartCache[i][0].flag = false;
                cache.AddRange(smartCache[i]);
            }
            smartCache.Clear();
        }
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
            minX = stl.MinX - 2 * settings.Overlap; maxX = stl.MaxX + 2 * settings.Overlap;
            minY = stl.MinY - 2 * settings.Overlap; maxY = stl.MaxY + 2 * settings.Overlap;
            minZ = stl.MinZ - 2 * settings.Overlap; maxZ = stl.MaxZ + 2 * settings.Overlap;
        }
    }
}
