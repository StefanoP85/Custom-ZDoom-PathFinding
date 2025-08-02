import sys
import Archives
import Builder

Archive = Archives.TArchive()
Archive.Load("Arena.pk3")
Map = Archive.MapDefinitions[0]
NavMeshSettings = Builder.TNavMeshSettings(64, 32)
NavMesh = Builder.TNavMesh()
NavMesh.Build(NavMeshSettings, Map)
print(NavMesh, file = sys.stdout)
print(NavMesh.GetMessages(), file = sys.stderr)
