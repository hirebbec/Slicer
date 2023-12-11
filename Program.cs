using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

string path = $"bottle.stl";
double overlap = 0.1;
double heightStep = 0.1;

Stl stl = new Stl(path);
Robot robot = new Robot(overlap, heightStep);

Builder.StartBuid(stl, robot);
FileWriter.WriteVertices(Builder.globalVertex, "test.txt");

Console.WriteLine("Program complete!");