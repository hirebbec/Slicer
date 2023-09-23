namespace slicer.io
{
    class TextFileReader
    {
        public static List<string> LinesFromFileOld(string filePath)
        {
            // init an empty list of text lines
            List<string> lines = new List<string>();

            // read the file
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string line;
                // read a file line-by-line until the end of the file is reached
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
                // close file handle
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return lines;
        }

        public static List<string> LinesFromFile(string filePath)
        {
            return new List<string>(File.ReadAllLines(filePath));
        }

        public static string StringFromFile(string filePath)
        {
            string lines = "";

            // read the file
            try
            {
                lines = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return lines;
        }
    }
}
