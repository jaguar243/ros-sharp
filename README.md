# [![ROS#](https://github.com/siemens/ros-sharp/wiki/img/ROSsharpLogo.jpg "ROS#")](https://github.com/siemens/ros-sharp) #
... is a set of open source software libraries and tools in [C\# ](https://docs.microsoft.com/de-de/dotnet/csharp/csharp) for communicating with [ROS](http://www.ros.org/) from .[NET](https://www.microsoft.com/net) applications, in particular [Unity3D](https://unity3d.com/).

## Contents: ##

### [RosBridgeClient](https://github.com/siemens/ros-sharp/tree/master/RosBridgeClient) ###
... provides a [.NET](https://www.microsoft.com/net) API to [ROS](http://www.ros.org/) via [rosbridge_suite](http://wiki.ros.org/rosbridge_suite).

### [UrdfImporter](https://github.com/siemens/ros-sharp/tree/master/UrdfImporter) ###
... provides a [URDF](http://wiki.ros.org/urdf) file parser for [.NET](https://www.microsoft.com/net) applications.

### [file_server](https://github.com/siemens/ros-sharp/tree/master/file_server) ###
... provides a [ROS](http://www.ros.org/) node with a service to send file contents.

 ... is required for receiving meshes and textures referenced in a  [URDF](http://wiki.ros.org/urdf) via [UrdfImporter](https://github.com/siemens/ros-sharp/tree/master/UrdfImporter).
 
### [Unity3D](https://github.com/siemens/ros-sharp/tree/master/Unity3D) ###
... is a [Unity3D](https://unity3d.com/) reference project providing [Unity3D](https://unity3d.com/)-specifc extensions to 
* [RosBridgeClient](https://github.com/siemens/ros-sharp/tree/master/RosBridgeClient)
* [UrdfImporter](https://github.com/siemens/ros-sharp/tree/master/UrdfImporter)

### [Release](https://github.com/siemens/ros-sharp/tree/master/Release) ###
... contains [RosSharp.unitypackage](https://github.com/siemens/ros-sharp/tree/master/Release/RosSharp.unitypackage)
the latest package of [ROS#](https://github.com/siemens/ros-sharp) assets from the [Unity3D](https://github.com/siemens/ros-sharp/tree/master/Unity3D) project.

### [Tutorials](https://github.com/siemens/ros-sharp/tree/master/Tutorials) ###
... contains Unity3D tutorial projects described described in the [Wiki](https://github.com/siemens/ros-sharp/wiki).

## Licensing: ##

ROS# is open source under the Apache 2.0 license and is free for commercial use.

## External Dependencies: ##

[RosBridgeClient](https://github.com/siemens/ros-sharp/tree/master/RosBridgeClient) uses the following 3rd party libraries:
* `websocket-sharp.dll` from the project [websocket-sharp](https://github.com/sta/websocket-sharp) provided under The MIT License.
* `NewtonSoft.Json.dll` from the project [NewtonSoft Json.Net](http://www.newtonsoft.com/json) provided under The MIT License.

## Special Thanks: ##

* [Rahul Warrier](https://github.com/jaguar243) for adjusting the code to enable its open source publication.
* [Verena Roehrl](https://github.com/roehrlverena) for providing the Wiki pages and the tutorial projects.
* [Karl Henkel](https://github.com/karl-) for providing the [reference](https://github.com/karl-/pb_Stl) for the Unity3D STL mesh importer used in this project.

### Please see the [Wiki](https://github.com/siemens/ros-sharp/wiki) for further info. ###

## Issues: ##

1. In `URDFSharp/URDFImporter/OdomoteryPatcher.cs`, the name of the root node needs to be generalized - either pull it from the urdf, or specify as a requirement that the root node be named "world".
2. In `URDFSharp/URDFImporterEditor/UrdfImporterEditorWindow.cs`, `urdfAssetPath` needs to be generalized using full path to Project folder (Windows method of finding current folder does not work in Mac OSX).

---

© Siemens AG, 2017

Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)
Revised: Rahul B. Warrier (10/21/2017)
