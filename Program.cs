using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 2;
double heightStep = 2;


Robot robot = new Robot(overlap, heightStep);
String name = "piston";

Stl snake = new Stl(name + ".stl");
Builder.init(snake, robot);
Builder.AlongX();
FileWriter.WriteVertices(Builder.globalVertex, name + ".txt");
