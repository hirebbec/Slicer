using slicer;
using slicer.Bulder;
using slicer.io;
using slicer.stl;

double overlap = 1;
double heightStep = 1;


Robot robot = new Robot(overlap, heightStep);

/*Stl Cub = new Stl("Cub.st/*l");
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
FileWriter.WriteVertices(Builder.globalVertex, "CylinderAlongY.txt");*/

Stl snake = new Stl("bracket.stl");
Builder.init(snake, robot);
Builder.AlongY();
FileWriter.WriteVertices(Builder.globalVertex, "bracket.txt");
