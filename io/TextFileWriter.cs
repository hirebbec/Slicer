using System;
using System.Collections.Generic;
using System.IO;

namespace slicer.io
{
    class TextFileWriter
    {
        public static void LinesToFile(List<string> lines, string filePath)
        {
            try
            {
                StreamWriter sw = new StreamWriter(filePath);

                foreach (string line in lines)
                {
                    sw.WriteLine(line);
                }

                // close file handle
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void LinesToFile(string allLines, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, allLines); // faster
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
