using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 1;
double heightStep = 1;
double delay = 1000;
double feedSpeed = 0.85;
String filename = "Lopast";


Settings settings = new Settings(overlap, heightStep, delay, feedSpeed);

Stl model = new Stl("models/" + filename + ".stl");

Builder.init(model, settings);
Builder.BuildPlaneCrossToCrossSnake();

FileWriter.init("results/" + filename + ".ncc", settings);
FileWriter.WriteCrossToCrossSnake(Builder.globalVertex);
FileWriter.End();
