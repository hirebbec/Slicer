using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 0.05;
double heightStep = 0.05;


Robot robot = new Robot(overlap, heightStep);

Stl Cub = new Stl("Cub.stl");
Builder.init(Cub, robot);
Builder.AlongX();
FileWriter.WriteVertices(Builder.globalVertex, "CubAlongX.txt");
Builder.AlongY();
FileWriter.WriteVertices(Builder.globalVertex, "CubAlongY.txt");

Stl Cylinder = new Stl("Cylinder.stl");
Builder.init(Cylinder, robot);
Builder.AlongX();
FileWriter.WriteVertices(Builder.globalVertex, "CylinderAlongX.txt");
Builder.AlongY();
FileWriter.WriteVertices(Builder.globalVertex, "CylinderAlongY.txt");