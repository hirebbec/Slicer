using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

string path = $"3D.stl";
double overlap = 0.1;
double heightStep = 0.1;

Stl stl = new Stl(path);
Robot robot = new Robot(overlap, heightStep);

Builder.init(stl, robot);
//Builder.ZigzagBuild();
//FileWriter.WriteVertices(Builder.globalVertex, "Zigzag.txt");
Builder.CrossToCrossBuild();
FileWriter.WriteVertices(Builder.globalVertex, "CrossToCross.txt");

Console.WriteLine("Program complete!");