"""
- Author: Pollazzon Stefano (ported)
- Python port aligned with C# Structures
"""

from enum import Enum


# --------------------------------------------------
# Base class
# --------------------------------------------------

class TMapEntity:
    def __init__(self, Index: int = -1):
        self.Index = Index


# --------------------------------------------------
# Map entities
# --------------------------------------------------

class TMapVertex(TMapEntity):
    def __init__(self, X, Y):
        super().__init__()
        self.X = int(X)
        self.Y = int(Y)


class TMapLinedef(TMapEntity):
    def __init__(
        self,
        V1,
        V2,
        ID=0,
        Blocking=False,
        TwoSided=False,
        SideFront=-1,
        SideBack=-1,
        Special=0,
        Arg0=0,
        Arg1=0,
        Arg2=0,
        Arg3=0,
        Arg4=0,
        MonsterUse=False,
        RepeatableSpecial=False,
        Ignored=False,
    ):
        super().__init__()
        self.V1 = int(V1)
        self.V2 = int(V2)
        self.ID = int(ID)
        self.Blocking = bool(Blocking)
        self.TwoSided = bool(TwoSided)
        self.SideFront = int(SideFront)
        self.SideBack = int(SideBack)
        self.Special = int(Special)
        self.Arg0 = int(Arg0)
        self.Arg1 = int(Arg1)
        self.Arg2 = int(Arg2)
        self.Arg3 = int(Arg3)
        self.Arg4 = int(Arg4)
        self.MonsterUse = bool(MonsterUse)
        self.RepeatableSpecial = bool(RepeatableSpecial)
        self.Ignored = bool(Ignored)


class TMapSidedef(TMapEntity):
    def __init__(self, Sector):
        super().__init__()
        self.Sector = int(Sector)


class TMapSector(TMapEntity):
    def __init__(self, HeightFloor, HeightCeiling, ID=0, Special=0, Ignored=False):
        super().__init__()
        self.HeightFloor = int(HeightFloor)
        self.HeightCeiling = int(HeightCeiling)
        self.ID = int(ID)
        self.Special = int(Special)
        self.Ignored = bool(Ignored)


class TMapThing(TMapEntity):
    def __init__(
        self,
        X,
        Y,
        Z=0,
        ThingType=0,
        ID=0,
        Special=0,
        Arg0=0,
        Arg1=0,
        Arg2=0,
        Arg3=0,
        Arg4=0,
    ):
        super().__init__()
        self.X = int(X)
        self.Y = int(Y)
        self.Z = int(Z)
        self.ThingType = int(ThingType)
        self.ID = int(ID)
        self.Special = int(Special)
        self.Arg0 = int(Arg0)
        self.Arg1 = int(Arg1)
        self.Arg2 = int(Arg2)
        self.Arg3 = int(Arg3)
        self.Arg4 = int(Arg4)


# --------------------------------------------------
# Namespace enum
# --------------------------------------------------

class TMapNamespace(Enum):
    MapNamespaceDoom = 0
    MapNamespaceHexen = 1
    MapNamespaceZDoom = 2


# --------------------------------------------------
# Map definition container
# --------------------------------------------------

class TMapDefinition:
    def __init__(self, MapName):
        self.MapName = MapName
        self.MapNamespace = TMapNamespace.MapNamespaceDoom

        self.MapVertex = []
        self.MapLinedef = []
        self.MapSidedef = []
        self.MapSector = []
        self.MapThing = []

    # ---------- helpers ----------

    def MapNamespaceText(self):
        if self.MapNamespace == TMapNamespace.MapNamespaceDoom:
            return "Doom"
        if self.MapNamespace == TMapNamespace.MapNamespaceHexen:
            return "Hexen"
        if self.MapNamespace == TMapNamespace.MapNamespaceZDoom:
            return "ZDoom"
        return ""

    # ---------- add methods ----------

    def AddMapVertex(self, item):
        if item is None:
            return
        item.Index = len(self.MapVertex)
        self.MapVertex.append(item)

    def AddMapLinedef(self, item):
        if item is None:
            return
        item.Index = len(self.MapLinedef)
        self.MapLinedef.append(item)

    def AddMapSidedef(self, item):
        if item is None:
            return
        item.Index = len(self.MapSidedef)
        self.MapSidedef.append(item)

    def AddMapSector(self, item):
        if item is None:
            return
        item.Index = len(self.MapSector)
        self.MapSector.append(item)

    def AddMapThing(self, item):
        if item is None:
            return
        item.Index = len(self.MapThing)
        self.MapThing.append(item)