using slicer;
using slicer.Bulder;
using slicer.stl;

string path = $"test.stl";
double overlap = 0.1;
double heightStep = 0.1;

Stl stl = new Stl(path);
Robot robot = new Robot(overlap, heightStep);

Builder.StartBuid(stl, robot);

Console.WriteLine("Program complete!");