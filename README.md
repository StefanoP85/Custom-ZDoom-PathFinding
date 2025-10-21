# Custom-ZDoom-PathFinding
Custom path finding module for ZDoom.
The ZDoom folder contains the C# application, that creates the navigation mesh data; the NavMesh folder contains the ZDoom's ZScript code and an example.
I'm using Inkoalawetrust <https://github.com/inkoalawetrust> work for the AI of the custom navigation agents.
If you want to know how it works, you can see the [Wiki](https://github.com/StefanoP85/Custom-ZDoom-PathFinding/wiki).

## NavMesh reader
The examples contains two identical MAPS, that I took from the project (https://github.com/disasteroftheuniverse/zdoom-pathfinding).
MAP01 uses a navigation mesh stored in the mod, in the NAVMESHES folder; the navigation mesh is read with the TNavMeshReader class at MAP startup.

## NavMesh builder
MAP02, which is identical to MAP01, has no navigation mesh available in the NAVMESHES folder: this time, the function Read from class TNavMeshReader fails and returns False.
The TNavMeshBuilder class is activated, to build a new navigation mesh from the current map's geometry. The actual heuristics are showed in the Wiki.
Since the process of building can be very long, the actual work is done through a TNavMeshBuilderWorker instance, a thinker, that runs every tick: the thinker tries to process SECTORs, and stops after 5 milliseconds, allowing other code to run. During the creation process, the navigation mesh uses a "Per-sector polygon chain", a "FAT-like" structure, that is fast and can grow rapidly without needing for sorting and other difficulties; when the process is completed, the "FAT-like" structure is replaced by the standard "cells space partitioning system", which is faster and more efficient.
This causes an interesting situation: different computers will complete the process in different times, and even the same computer running the same MAP will complete sooner or later, depending on the resources availables. The thinker TNavMeshBuilderWorker will definitely render demo playback non-deterministic and cause interesting situations!
The builder class is a giant structure with large statically allocated arrays: in order to be performant, almost everything is statically allocated. The only dynamically allocated objects are the TNavMeshLine and the TNavMeshPolygon objects, which are allocated 1024 items at a time; the "FAT-like" arrays are also allocated, 1024 items at a time. Once completion, almost all objects are deallocated, freeing precious RAM.
My special thanks to Inkoalawetrust for the help received.

## Issues
Slow: some sectors require more than 10 milliseconds, and even much more; I think to change the "Worker" thinker to perform sub-sector management.
Still some polygons are not partitioned correctly, I don't know why.
