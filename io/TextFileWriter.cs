﻿using slicer.stl;
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
        public static void GoTo(double x, double y, double z, bool flag)
        {
            string gCode = "";
            if (flag)
            {
                gCode = $"G1 X{(int)(10 * x)} Y{(int)(10 * y)} Z{(int)(10 * z)};";

            } else
            {
                gCode = $"G0 X{(int)(10 * x)} Y{(int)(10 * y)} Z{(int)(10 * z)};";
            }
            WriteToFile(gCode);
            //WriteToFile("M0"); TODO
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
        public static void WriteVertices(List<Vertex> vertices, String filePath)
        {
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.Close();
            FileWriter.filePath = filePath;
            bool flag = false;
            foreach (var vertex in vertices)
            {
                GoTo(vertex.x, vertex.y, vertex.z, flag);
                flag = !flag;
            }
        }
    }
}
