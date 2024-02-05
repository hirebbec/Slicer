using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 0.05;
double heightStep = 0.05;


Robot robot = new Robot(overlap, heightStep);
String name = "Cylinder";

Stl snake = new Stl("./../../../models/" + name + ".stl");
Builder.init(snake, robot);
Builder.BuildPlaneSnakeX();
FileWriter.WriteSolid(Builder.globalVertex, "./../../../results/" + name + ".txt");
