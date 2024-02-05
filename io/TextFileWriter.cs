using slicer.stl;
using System;
using System.Collections.Generic;
using System.IO;

namespace slicer.io
{
    public class FileWriter
    {
        private static string filePath;

        /// <summary>
        /// Создает экземпляр класса FileWriter для записи G-code в файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу для записи G-code.</param>

        /// <summary>
        /// Записывает G-code для перемещения головки 3D принтера по указанным координатам.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <param name="z">Координата Z.</param>
        public static void GoTo1(double x, double y, double z)
        {
            WriteToFile($"G1 X{(int)(1000 * x)} Y{(int)(1000 * y)} Z{(int)(1000 * z)};");

        }

        public static void GoTo0(double x, double y, double z)
        {
            WriteToFile($"G0 X{(int)(1000 * x)} Y{(int)(1000 * y)} Z{(int)(1000 * z)};");
        }

        /// <summary>
        /// Записывает строку G-code в файл.
        /// </summary>
        /// <param name="gCode">Строка G-code для записи.</param>
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

        /// <summary>
        /// Записывает список вершин в G-code, перемещая головку 3D принтера по каждой вершине.
        /// </summary>
        /// <param name="vertices">Список вершин для обработки.</param>
        public static void WriteSolid(List<Vertex> vertices, String filePath)
        {
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.Close();
            FileWriter.filePath = filePath;
            GoTo0(vertices[0].x, vertices[0].y, vertices[0].z);
            vertices.RemoveAt(0);
            for (int i = 0; i < vertices.Count(); i++)
            {
                if (i > 0 && vertices[i - 1].z < vertices[i].z)
                {
                    GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                }
                GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
            }
        }

        public static void WriteNotSolid(List<Vertex> vertices, String filePath)
        {
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.Close();
            FileWriter.filePath = filePath;
            bool flag = false;
            for (int i = 0; i < vertices.Count(); i++)
            {
                if (flag)
                {
                    GoTo1(vertices[i].x, vertices[i].y, vertices[i].z);
                } else
                {
                    GoTo0(vertices[i].x, vertices[i].y, vertices[i].z);
                }
                flag = !flag;
            }
        }
    }
}
