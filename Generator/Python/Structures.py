"""
 * Author: Pollazzon Stefano
 * Project: ZDoom Navmesh builder
 * This module contains the map entities used in ZDoom
"""
from enum import Enum

# Class TMapEntity is the base class and common ancestor of all map entities.
class TMapEntity:
	def __init__(self, Index = 0):
		self.Index = Index

# Class TMapVertex represents a single VERTEX in the map.
class TMapVertex(TMapEntity):
	def __init__(self, X, Y):
		super().__init__(self)
		self.X = X
		self.Y = Y

# Class TMapLinedef represents a single LINEDEF in the map.
class TMapLinedef(TMapEntity):
	def __init__(self, V1, V2, ID = 0, Blocking = False, TwoSided = False, SideFront = 0, SideBack = 0, Special = 0, Arg0 = 0, Arg1 = 0, Arg2 = 0, Arg3 = 0, Arg4 = 0, MonsterUse = False, RepeatableSpecial = False, Ignored = False):
		super().__init__(self)
		self.V1 = V1
		self.V2 = V2
		self.ID = ID
		self.Blocking = Blocking
		self.TwoSided = TwoSided
		self.SideFront = SideFront
		self.SideBack = SideBack
		self.Special = Special
		self.Arg0 = Arg0
		self.Arg1 = Arg1
		self.Arg2 = Arg2
		self.Arg3 = Arg3
		self.Arg4 = Arg4
		self.MonsterUse = MonsterUse
		self.RepeatableSpecial = RepeatableSpecial
		self.Ignored = Ignored

# Class TMapSidedef represents a single SIDEDEF in the map.
class TMapSidedef(TMapEntity):
	def __init__(self, Sector):
		super().__init__(self)
		self.Sector = Sector

# Class TMapSector represents a single SECTOR in the map.
class TMapSector(TMapEntity):
	def __init__(self, HeightFloor, HeightCeiling, ID = 0, Special = 0, Ignored = False):
		super().__init__(self)
		self.HeightFloor = HeightFloor
		self.HeightCeiling = HeightCeiling
		self.ID = ID
		self.Special = Special
		self.Ignored = Ignored

# Class TMapThing represents a single THING in the map.
class TMapThing(TMapEntity):
	def __init__(self, X, Y, Z = 0, ThingType = 0, ID = 0, Special = 0, Arg0 = 0, Arg1 = 0, Arg2 = 0, Arg3 = 0, Arg4 = 0):
		super().__init__(self)
		self.X = X
		self.Y = Y
		self.Z = Z
		self.ThingType = ThingType
		self.ID = ID
		self.Special = Special
		self.Arg0 = Arg0
		self.Arg1 = Arg1
		self.Arg2 = Arg2
		self.Arg3 = Arg3
		self.Arg4 = Arg4

# Class TMapNamespace represents the possible namespaces of a map.
# The action specials of LINEDEF and SECTOR depends on the map's namespace.
# In binary maps we have Doom and Hexen namespaces, depending if the map has a BEHAVIOR lump; in text maps we have all values stored in the text stream.
class TMapNamespace(Enum):
	MapNamespaceDoom = 0
	MapNamespaceHexen = 1
	MapNamespaceZDoom = 2

class TMapDefinition:
	def __init__(self, MapName):
		self.MapName = MapName
		self.MapNamespace = None
		self.MapVertex = []
		self.MapLinedef = []
		self.MapSidedef = []
		self.MapSector = []
		self.MapThing = []

	def MapNamespaceText(self):
		if self.MapNamespace == TMapNamespace.MapNamespaceDoom:
			return "Doom"
		elif self.MapNamespace == TMapNamespace.MapNamespaceHexen:
			return "Hexen"
		elif self.MapNamespace == TMapNamespace.MapNamespaceZDoom:
			return "ZDoom"
		else:
			return ""

	def AddMapVertex(self, MapVertex):
		if not MapVertex is None:
			MapVertex.Index = len(self.MapVertex)
			self.MapVertex.append(MapVertex)

	def AddMapLinedef(self, MapLinedef):
		if not MapLinedef is None:
			MapLinedef.Index = len(self.MapLinedef)
			self.MapLinedef.append(MapLinedef)
			
	def AddMapSidedef(self, MapSidedef):
		if not MapSidedef is None:
			MapSidedef.Index = len(self.MapSidedef)
			self.MapSidedef.append(MapSidedef)

	def AddMapSector(self, MapSector):
		if not MapSector is None:
			MapSector.Index = len(self.MapSector)
			self.MapSector.append(MapSector)

	def AddMapThing(self, MapThing):
		if not MapThing is None:
			MapThing.Index = len(self.MapThing)
			self.MapThing.append(MapThing)
