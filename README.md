# Custom-ZDoom-PathFinding
Custom path finding module for ZDoom.
The ZDoom folder contains the C# application, that creates the navigation mesh data; the NavMesh folder contains the ZDoom's ZScript code and an example.
I'm using Inkoalawetrust <https://github.com/inkoalawetrust> work for the AI of the custom navigation agents.
If you want to know how it works, you can see the [Wiki](https://github.com/StefanoP85/Custom-ZDoom-PathFinding/wiki).

IN DEVELOPMENT
This "dev" branch aims to write a NavMesh builder, that doesn't require any external tool, to build the navmesh; it will use ZScript instead, to do all the work.
Here's how I studied it:

Step 1: a map is loaded, the "TNavMeshHandler" event handler triggers the loading of the navmesh.

Step 2: if a NavMesh is found in the "navmeshes" folder, as in the "main" branch, then reads it, as usual, otherwhise go to step 3.

Step 3: allocate a Thinker, that iterates through all SECTORs in the map in fixed slices of time, using Object.MSTime() or Object.MSTimef(), didn't started yet. The fixed time slice is useful, to minimize impacts and performance issues between different machines: fast machines will simply require less game tics to complete the task.

Step 4: for each map SECTOR:
Step 4.1: extract the "Polygon groups" (TPolygonGroup) of the SECTOR. Work in progress.

Step 4.2: for each "Polygon group":
Step 4.2.1: remove the "holes", function RemoveHoles is ready to be tested.

Step 4.2.2: partition each "Polygon" in the "Polygon group", the functions ConvexPartition_HM and Triangulate_EC are ready to be tested.

Step 4.2.3: if possible, consider the 3D sectors.

Step 4.3: store the generated polygons.

Step 4.4: process the "per-sector chains of polygons". Work in progress, almost finished.

Step 5: build the "cells partition system", replacing the "per-sector chains of polygons", for searching the polygons.

The "cells partition system" is the previous method used too search the polygons from the points, and it's fast and space efficient; during the time needed for the step 4, the navigation mesh will still be partially usable, as the iteration goes on: I simply need a data structure, that's dynamic but allocation-efficient, ordered but not slow to update, and that can grow very rapidly. I think that a system like the FAT data structure for the file system could be useful; I write and tested almost all the code, and for efficiency reasons, the allocation proceeds by 1024 elements at once (8192 bytes, because there's 2 ints in each "FAT entry"). When the algorithm finishes, the "cells partition system" could be computed and the "FAT chains" be discarded.
