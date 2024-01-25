using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

string path = "3D.stl";
double overlap = 0.1;
double heightStep = 0.1;

Stl stl = new Stl(path);
Robot robot = new Robot(overlap, heightStep);

Builder.init(stl, robot);
Builder.AlongX();
FileWriter.WriteVertices(Builder.globalVertex, "CylinderAlongX.txt");
Builder.AlongY();
FileWriter.WriteVertices(Builder.globalVertex, "CylinderAlongY.txt");

Console.WriteLine("Program complete!");