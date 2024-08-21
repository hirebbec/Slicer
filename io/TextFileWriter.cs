using slicer.stl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace slicer.io
{
    public class FileWriter
    {
        private static string filePath;
        private static Settings settings;
        private static StreamWriter writer;
        private static NumberFormatInfo nfi;


        public static void init(String filePath, Settings settings)
        {
            if (writer != null) { writer.Close(); }
            FileWriter.writer = new StreamWriter(filePath, false);
            FileWriter.writer.Close();
            FileWriter.settings = settings;
            FileWriter.filePath = filePath;
            FileWriter.nfi = new CultureInfo("en-US", false).NumberFormat;


            WriteToFile("M82"); // Установка экструдера в абсолютный режим
            WriteToFile("G90"); // Включение абсолютного позиционирования 
            WriteToFile("M71"); // ??
            WriteToFile("M72"); // ??
            WriteToFile($"G4 P" + Math.Round(FileWriter.settings.Delay, 3).ToString(nfi)); // Пауза
            WriteToFile($"F" + Math.Round(FileWriter.settings.FeedSpeed, 3).ToString(nfi)); // Установка скорости подачи (мм/с)
        }
        public static void GoToFirst(double x, double y, double z)
        {
            GoTo1(x, y, z);
            WriteToFile("M61"); // Включение и выключение чего-то (мб подачи порошка)
            WriteToFile("M91"); // Включает абсолютное позиционирование для подачи экструдера
            WriteToFile($"G4 P" + Math.Round(FileWriter.settings.Delay, 3).ToString(nfi)); // Пауза
            WriteToFile("M63"); // ??
        }
        public static void GoTo1(double x, double y, double z)
        {
            WriteToFile("G1 X" + Math.Round(x, 3).ToString(nfi) + " Y" + Math.Round(y, 3).ToString(nfi) + " Z" + Math.Round(z, 3).ToString(nfi));
        }

        public static void GoTo0(double x, double y, double z)
        {
            WriteToFile("M60");
            WriteToFile("M62");
            WriteToFile("M99");
            WriteToFile("G0 X" + Math.Round(x, 3).ToString(nfi) + " Y" + Math.Round(y, 3).ToString(nfi) + " Z" + Math.Round(z, 3).ToString(nfi));
            WriteToFile("M61");
            WriteToFile("M91");
            WriteToFile($"G4 P" + Math.Round(FileWriter.settings.Delay, 3).ToString(nfi));
            WriteToFile("M63");
        }

        public static void End()
        {
            WriteToFile("M79");
            WriteToFile("E");
        }
        private static void WriteToFile(string gCode)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(gCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи в файл: {ex.Message}");
            }
        }
        public static void WriteSolid(List<Vertex> vertices)
        {
            if (vertices.Count != 0)
            {
                GoToFirst(vertices[0].x, vertices[0].y, vertices[0].z);
                vertices.RemoveAt(0);
                for (int i = 0; i < vertices.Count(); i++)
                {
                    if (i > 0 && vertices[i - 1].z < vertices[i].z)
                    {
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else
                    {
                        GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                }
            }
        }

        public static void WriteNotSolid(List<Vertex> vertices)
        {
            bool flag = false;
            if (vertices.Count != 0)
            {
                GoToFirst(vertices[0].x, vertices[0].y, vertices[0].z);
                vertices.RemoveAt(0);
                for (int i = 0; i < vertices.Count(); i++)
                {
                    if (flag)
                    {
                        GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else
                    {
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    flag = !flag;
                }
            }
        }

        public static void WriteSnakeX(List<Vertex> vertices)
        {
            if (vertices.Count != 0)
            {
                GoToFirst(vertices[0].x, vertices[0].y, vertices[0].z);
                vertices.RemoveAt(0);
                for (int i = 0; i < vertices.Count(); i++)
                {
                    if (i > 0 && vertices[i - 1].y == vertices[i].y)
                    {
                        bool flag = true;
                        while (i < vertices.Count() && vertices[i - 1].y == vertices[i].y)
                        {
                            if (vertices[i - 1].z < vertices[i].z)
                            {
                                GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                                flag = false;
                            }
                            else if (flag)
                            {
                                GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                            }
                            else
                            {
                                GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                            }
                            flag = !flag;
                            i++;
                        }
                        i--;
                    }
                    else if (i > 0 && (vertices[i - 1].z < vertices[i].z || Math.Abs(Math.Abs(vertices[i - 1].y) - Math.Abs(vertices[i].y)) > 1.1 * FileWriter.settings.Overlap))
                    {
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else
                    {
                        GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                }
            }
        }

        public static void WriteSmartSnakeX(List<Vertex> vertices)
        {
            if (vertices.Count != 0)
            {
                GoToFirst(vertices[0].x, vertices[0].y, vertices[0].z);
                vertices.RemoveAt(0);
                for (int i = 0; i < vertices.Count(); i++)
                {
                    if (i > 0 && vertices[i - 1].z < vertices[i].z)
                    {
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else if (!vertices[i].flag)
                    {
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else if (i > 0 && Math.Abs(Math.Abs(vertices[i - 1].y) - Math.Abs(vertices[i].y)) > 1.1 * FileWriter.settings.Overlap)
                    {
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else if (i > 0 && (Math.Abs(Math.Abs(vertices[i - 1].x) - Math.Abs(vertices[i].x)) > 1.1 * FileWriter.settings.Overlap && vertices[i].y != vertices[i - 1].y))
                    {
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else
                    {
                        GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                }
            }
        }

        public static void WriteSnakeY(List<Vertex> vertices)
        {
            if (vertices.Count != 0)
            {
                GoToFirst(vertices[0].x, vertices[0].y, vertices[0].z);
                vertices.RemoveAt(0);
                for (int i = 0; i < vertices.Count(); i++)
                {
                    if (i > 0 && vertices[i - 1].x == vertices[i].x)
                    {
                        bool flag = true;
                        while (i < vertices.Count() && vertices[i - 1].x == vertices[i].x)
                        {
                            if (vertices[i - 1].z < vertices[i].z)
                            {
                                GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                                flag = false;
                            }
                            else if (flag)
                            {
                                GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                            }
                            else
                            {
                                GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                            }
                            flag = !flag;
                            i++;
                        }
                        i--;
                    }
                    else if (i > 0 && vertices[i - 1].z < vertices[i].z)
                    {   
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else
                    {
                        GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                }
            }
        }

        public static void WriteCrossToCrossSnake(List<Vertex> vertices)
        {
            if (vertices.Count != 0)
            {
                GoToFirst(vertices[0].x, vertices[0].y, vertices[0].z);
                vertices.RemoveAt(0);
                for (int i = 0; i < vertices.Count(); i++)
                {
                    if (i > 0 && vertices[i - 1].z < vertices[i].z)
                    {
                        GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                    else
                    {
                        GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                    }
                }
            }
        }
    }
}
