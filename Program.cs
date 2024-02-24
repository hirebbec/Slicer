using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 0.1;
double heightStep = 0.1;
double delay = 1.00;
double feedSpeed = 0.85;


Robot robot = new Robot(overlap, heightStep);
String name = "3Tower";
Stl snake = new Stl("./../../../models/" + name + ".stl");

Builder.init(snake, robot);
Builder.BuildPlaneSnakeX();

FileWriter.init("./../../../results/" + name + ".txt", delay, feedSpeed, overlap, heightStep);
FileWriter.WriteSnakeX(Builder.globalVertex);
FileWriter.End();
