using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 1;
double heightStep = 1;
double delay = 1000;
double feedSpeed = 0.85;
String filename = "bracket";


Settings settings = new Settings(overlap, heightStep, delay, feedSpeed);

Stl model = new Stl("models/" + filename + ".stl");

Builder.init(model, settings);
Builder.BuildPlaneSmartSnakeX();

FileWriter.init("results/" + filename + ".ncc", settings);
FileWriter.WriteSmartSnakeX(Builder.globalVertex);
FileWriter.End();
