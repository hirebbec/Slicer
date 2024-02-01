using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 0.025;
double heightStep = 0.025;


Robot robot = new Robot(overlap, heightStep);
String name = "airplane";

Stl snake = new Stl("./../../../models/" + name + ".stl");
Builder.init(snake, robot);
Builder.CrossToCross();
FileWriter.WriteVertices(Builder.globalVertex, "./../../../results/" + name + ".txt");
