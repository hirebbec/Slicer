using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 0.01;
double heightStep = 0.05;


Robot robot = new Robot(overlap, heightStep);
String name = "Tower";

Stl snake = new Stl(name + ".stl");
Builder.init(snake, robot);
Builder.CrossToCross();
FileWriter.WriteVertices(Builder.globalVertex, name + ".txt");
