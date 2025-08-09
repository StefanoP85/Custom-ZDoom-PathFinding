"""
 * Author: Pollazzon Stefano
 * Project: ZDoom Navmesh builder
 * This module contains the navmesh generation algorythms
"""
from enum import Enum
import math
from typing import List, Optional
from Archives import *

# Exception used as a "goto" replacement.
class GotoException(Exception):
	pass

# Enum TOrientation represents the possible ways of aligning 3 Points in the 2D Euclidean space.
class TOrientation(Enum):
	Clockwise = -1
	Collinear = 0
	CounterClockwise = 1

# Class TPoint represents A simple Point in 2D Euclidean space.
class TPoint:
	def __init__(self, X, Y):
		self.X = int(X)
		self.Y = int(Y)

	def __eq__(self, other):
		if other is None:
			return False
		if self is other:
			return True
		if not isinstance(other, TPoint):
			return False
		return (self.X == other.X) and (self.Y == other.Y)

# Class TLine represents A simple Line in 2D Euclidean space.
class TLine:
	def __init__(self, A, B):
		if A is None or B is None:
			raise ValueError("A and B cannot be None")
		self.A = A
		self.B = B
		self.Portal = -1
		self.Flags = 0
		self.MapLinedef = -1

	def __eq__(self, other):
		if other is None:
			return False
		if self is other:
			return True
		if not isinstance(other, TLine):
			return False
		return (self.A == other.A) and (self.B == other.B)

# Class TPolygon represents A simple Polygon without Holes in 2D Euclidean space.
class TPolygon:
	def __init__(self):
		self.Lines = []
		self.LineFirst = 0
		self.LineCount = 0
		self.Flags = 0
		self.MapSector = -1
		self.BlockingLines = []
		self.HeightFloor = 0
		self.HeightCeiling = 0

	def AddLine(self, Line):
		self.Lines.append(Line)

	def IsClosed(self):
		if len(self.Lines) < 3:
			return False
		CheckedLine = [False] * len(self.Lines)
		FirstLine = 0
		CurrentLine = FirstLine
		while True:
			CheckedLine[CurrentLine] = True
			CurrentLine = next((I for I, Line in enumerate(self.Lines) if Line.A == self.Lines[CurrentLine].B), -1)
			if CurrentLine < 0 or (CheckedLine[CurrentLine] and CurrentLine != FirstLine):
				return False
			if CurrentLine == 0:
				break
		return True

	def SortLines(self):
		if len(self.Lines) < 3:
			return
		LinesIndexes = []
		CurrentIndex = 0
		LinesIndexes.append(CurrentIndex)
		while True:
			CurrentLine = self.Lines[CurrentIndex]
			CurrentIndex = next((I for I, Line in enumerate(self.Lines) if Line.A == CurrentLine.B), -1)
			if CurrentIndex < 0:
				return
			if CurrentIndex > 0:
				LinesIndexes.append(CurrentIndex)
## CHECK THIS
			if CurrentIndex == 0:
				break;
		for I in range(len(self.Lines)):
			if LinesIndexes[I] != I:
				A = self.Lines[LinesIndexes[I]]
				B = self.Lines[I]
				self.Lines[LinesIndexes[I]] = B
				self.Lines[I] = A
				IndexA = LinesIndexes[LinesIndexes[I]]
				IndexB = LinesIndexes[I]
				LinesIndexes[LinesIndexes[I]] = IndexB
				LinesIndexes[I] = IndexA

# Class TPPLPoint represents A Point in 2D Euclidean space, but with floating Point arithmetic.
class TPPLPoint:
	def __init__(self, X = 0, Y = 0):
		self.X = X
		self.Y = Y

	def __eq__(self, other):
		if other is None:
			return False
		if self is other:
			return True
		if not isinstance(other, TPPLPoint):
			return False
		return (self.X == other.X) and (self.Y == other.Y)

	def Clone(self):
		return TPPLPoint(self.X, self.Y)

	def __add__(self, other):
		return TPPLPoint(self.X + other.X, self.Y + other.Y)

	def __sub__(self, other):
		return TPPLPoint(self.X - other.X, self.Y - other.Y)

	def __mul__(self, F):
		return TPPLPoint(self.X * F, self.Y * F)

	def __truediv__(self, F):
		return TPPLPoint(self.X / F, self.Y / F)

# Class TPPLPolygon represents A Polygon in 2D Euclidean space, but with floating Point arithmetic.
# It also sorts the Points in clockwise or counterclockwise order, with the following rule:
# If the Polygon represents A valid Polygon: counterclockwise order is used
# If the Polygon represents A Hole: clockwise order is used
class TPPLPolygon:
	def __init__(self, P1 = None, P2 = None, P3 = None):
		self.Points = []
		self.FHole = False
		if P1 and P2 and P3:
			self.Points = [P1, P2, P3]

	def Clone(self):
		Result = TPPLPolygon()
		for Point in self.Points:
			Result.AddPoint(Point)
		Result.Hole = self.FHole
		return Result

	def AddPoint(self, Point):
		self.Points.append(Point)

	def GetOrientation(self):
		Area = 0
		for i1 in range(len(self.Points)):
			i2 = (i1 + 1) % len(self.Points)
			Area += self.Points[i1].X * self.Points[i2].Y - self.Points[i1].Y * self.Points[i2].X
		if Area > 0:
			return TOrientation.CounterClockwise
		elif Area < 0:
			return TOrientation.Clockwise
		else:
			return TOrientation.Collinear

	def SetOrientation(self, Orientation):
		ActualOrientation = self.GetOrientation()
		if ActualOrientation != TOrientation.Collinear and ActualOrientation != Orientation:
			self.Points.reverse()

	def SetHole(self, AHole):
		self.FHole = AHole
		if self.FHole:
			self.SetOrientation(TOrientation.Clockwise)
		else:
			self.SetOrientation(TOrientation.CounterClockwise)

	@property
	def Hole(self):
		return self.FHole

	@Hole.setter
	def Hole(self, value):
		self.SetHole(value)

# Class TPartitionVertex is used in the triangulation process.
class TPartitionVertex:
	def __init__(self, APoint):
		self.Point = APoint
		self.IsActive = False
		self.IsConvex = False
		self.IsEar = False
		self.Angle = 0
		self.Previous = self
		self.Next = self

# Class TPPLPartition is A static class, that handles three very important tasks.
# 1. Hole management.
# 2. Triangulation by Ear clipping.
# 3. Convex partitioning with the Hertel-Mehlhorn algorithm.
class TPPLPartition:
	def IsConvex(self, P1, P2, P3):
		Val = (P3.Y - P1.Y) * (P2.X - P1.X) - (P3.X - P1.X) * (P2.Y - P1.Y)
		return Val > 0

	def IsReflex(self, P1, P2, P3):
		Val = (P3.Y - P1.Y) * (P2.X - P1.X) - (P3.X - P1.X) * (P2.Y - P1.Y)
		return Val < 0

	def IsInside(self, P1, P2, P3, Point):
		if self.IsConvex(P1, Point, P2):
			return False
		if self.IsConvex(P2, Point, P3):
			return False
		if self.IsConvex(P3, Point, P1):
			return False
		return True

	def InCone(self, P1, P2, P3, Point):
		Convex = self.IsConvex(P1, P2, P3)
		if Convex:
			if not self.IsConvex(P1, P2, Point):
				return False
			if not self.IsConvex(P2, P3, Point):
				return False
			return True
		else:
			if self.IsConvex(P1, P2, Point):
				return True
			if self.IsConvex(P2, P3, Point):
				return True
			return False

	def Intersects(self, P11, P12, P21, P22):
		if (P11.X == P21.X and P11.Y == P21.Y) or (P11.X == P22.X and P11.Y == P22.Y) or (P12.X == P21.X and P12.Y == P21.Y) or (P12.X == P22.X and P12.Y == P22.Y):
			return False
		V1Ort = TPPLPoint()
		V2Ort = TPPLPoint()
		V1Ort.X = P12.Y - P11.Y
		V1Ort.Y = P11.X - P12.X
		V2Ort.X = P22.Y - P21.Y
		V2Ort.Y = P21.X - P22.X
		V = TPPLPoint(P21.X - P11.X, P21.Y - P11.Y)
		Dot21 = V.X * V1Ort.X + V.Y * V1Ort.Y
		V = TPPLPoint(P22.X - P11.X, P22.Y - P11.Y)
		Dot22 = V.X * V1Ort.X + V.Y * V1Ort.Y
		V = TPPLPoint(P11.X - P21.X, P11.Y - P21.Y)
		Dot11 = V.X * V2Ort.X + V.Y * V2Ort.Y
		V = TPPLPoint(P12.X - P21.X, P12.Y - P21.Y)
		Dot12 = V.X * V2Ort.X + V.Y * V2Ort.Y
		if Dot11 * Dot12 > 0 or Dot21 * Dot22 > 0:
			return False
		return True

	def Normalize(self, Point):
		Result = TPPLPoint()
		Val = math.sqrt(Point.X * Point.X + Point.Y * Point.Y)
		if Val != 0:
			Result.X = Point.X / Val
			Result.Y = Point.Y / Val
		else:
			Result.X = 0
			Result.Y = 0
		return Result

	def UpdateVertex(self, Vertex, Vertices, NumVertices):
		V1 = Vertex.Previous
		V3 = Vertex.Next
		Vertex.IsConvex = self.IsConvex(V1.Point, Vertex.Point, V3.Point)
		Vec1 = self.Normalize(TPPLPoint(V1.Point.X - Vertex.Point.X, V1.Point.Y - Vertex.Point.Y))
		Vec3 = self.Normalize(TPPLPoint(V3.Point.X - Vertex.Point.X, V3.Point.Y - Vertex.Point.Y))
		Vertex.Angle = Vec1.X * Vec3.X + Vec1.Y * Vec3.Y
		if Vertex.IsConvex:
			Vertex.IsEar = True
			for I in range(NumVertices):
				if (Vertices[I].Point.X == Vertex.Point.X and Vertices[I].Point.Y == Vertex.Point.Y) or (Vertices[I].Point.X == V1.Point.X and Vertices[I].Point.Y == V1.Point.Y) or (Vertices[I].Point.X == V3.Point.X and Vertices[I].Point.Y == V3.Point.Y):
					continue
				if self.IsInside(V1.Point, Vertex.Point, V3.Point, Vertices[I].Point):
					Vertex.IsEar = False
					break
		else:
			Vertex.IsEar = False

# Function RemoveHoles is A simple heuristic procedure for removing Holes from A list of Polygons.
# It works by creating A diagonal from the right-most Hole to some other visible vertex.
# Vertices of all non-Hole polys have to be in counter-clockwise order.<br/>
# Vertices of all Hole polys have to be in clockwise order.</param>
	def RemoveHoles(self, InputPolygons, OutputPolygons):
		Hole = None
		Poly = None
		BestPolyPoint = None
		HolePointIndex = 0
		PolyPointIndex = 0
		HasHoles = False
		for Polygon in InputPolygons:
			if Polygon.Hole:
				HasHoles = True
				break
		if not HasHoles:
			OutputPolygons.extend(InputPolygons)
			return True
		Polygons = [Polygon.Clone() for Polygon in InputPolygons]
		while True:
			HasHoles = False
			for Polygon in Polygons:
				if not Polygon.Hole:
					continue
				if not HasHoles:
					HasHoles = True
					Hole = Polygon
					HolePointIndex = 0
				for I in range(len(Polygon.Points)):
					if Polygon.Points[I].X > Hole.Points[HolePointIndex].X:
						Hole = Polygon
						HolePointIndex = I
			if not HasHoles:
				break
			HolePoint = Hole.Points[HolePointIndex]
			PointFound = False
			for Polygon1 in Polygons:
				if Polygon1.Hole:
					continue
				for I in range(len(Polygon1.Points)):
					if Polygon1.Points[I].X <= HolePoint.X:
						continue
					if not self.InCone(Polygon1.Points[(I - 1) % len(Polygon1.Points)], Polygon1.Points[I], Polygon1.Points[(I + 1) % len(Polygon1.Points)], HolePoint):
						continue
					PolyPoint = Polygon1.Points[I]
					if PointFound:
						V1 = self.Normalize(PolyPoint - HolePoint)
						V2 = self.Normalize(BestPolyPoint - HolePoint)
						if V2.X > V1.X:
							continue
					PointVisible = True
					for Polygon2 in Polygons:
						if Polygon2.Hole:
							continue
						for J in range(len(Polygon2.Points)):
							LineP1 = Polygon2.Points[J]
							LineP2 = Polygon2.Points[(J + 1) % len(Polygon2.Points)]
							if self.Intersects(HolePoint, PolyPoint, LineP1, LineP2):
								PointVisible = False
								break
						if not PointVisible:
							break
					if PointVisible:
						PointFound = True
						BestPolyPoint = PolyPoint
						Poly = Polygon1
						PolyPointIndex = I
			if not PointFound:
				return False
			NewPoly = TPPLPolygon()
			for I in range(PolyPointIndex + 1):
				NewPoly.AddPoint(Poly.Points[I])
			for I in range(len(Hole.Points)):
				NewPoly.AddPoint(Hole.Points[(I + HolePointIndex) % len(Hole.Points)])
			for I in range(PolyPointIndex, len(Poly.Points)):
				NewPoly.AddPoint(Poly.Points[I])
			Polygons.remove(Hole)
			Polygons.remove(Poly)
			Polygons.append(NewPoly)
		OutputPolygons.extend(Polygons)
		return True

# Function Triangulate_EC triangulates A Polygon by Ear clipping.
# Vertices have to be in counter-clockwise order.</param>
	def Triangulate_EC(self, InputPolygon, OutputPolygons):
		if len(InputPolygon.Points) < 3:
			return False
		if len(InputPolygon.Points) == 3:
			OutputPolygons.append(InputPolygon)
			return True
		NumVertices = len(InputPolygon.Points)
		Vertices = [TPartitionVertex(InputPolygon.Points[I]) for I in range(NumVertices)]
		Ear = None
		for I in range(NumVertices):
			Vertices[I].IsActive = True
			Vertices[I].Next = Vertices[0] if I == (NumVertices - 1) else Vertices[I + 1]
			Vertices[I].Previous = Vertices[NumVertices - 1] if I == 0 else Vertices[I - 1]
		for I in range(NumVertices):
			self.UpdateVertex(Vertices[I], Vertices, NumVertices)
		for I in range(NumVertices - 3):
			EarFound = False
			for J in range(NumVertices):
				if not Vertices[J].IsActive:
					continue
				if not Vertices[J].IsEar:
					continue
				if not EarFound:
					EarFound = True
					Ear = Vertices[J]
				else:
					if Vertices[J].Angle > Ear.Angle:
						Ear = Vertices[J]
			if not EarFound:
				return False
			Triangle = TPPLPolygon(Ear.Previous.Point, Ear.Point, Ear.Next.Point)
			OutputPolygons.append(Triangle)
			Ear.IsActive = False
			Ear.Previous.Next = Ear.Next
			Ear.Next.Previous = Ear.Previous
			if I == NumVertices - 4:
				break
			self.UpdateVertex(Ear.Previous, Vertices, NumVertices)
			self.UpdateVertex(Ear.Next, Vertices, NumVertices)
		for I in range(NumVertices):
			if Vertices[I].IsActive:
				Triangle = TPPLPolygon(Vertices[I].Previous.Point, Vertices[I].Point, Vertices[I].Next.Point)
				OutputPolygons.append(Triangle)
				break
		return True

# Function ConvexPartition_HM partitions A Polygon into convex Polygons by using the
# Hertel-Mehlhorn algorithm. The algorithm gives at most four times the number of parts as the optimal algorithm, 
# however, in practice it works much better than that and often gives optimal partition.
# It uses triangulation obtained by Ear clipping as intermediate Result.
# Vertices have to be in counter-clockwise order.</param>
	def ConvexPartition_HM(self, InputPolygon, OutputPolygons):
		if len(InputPolygon.Points) < 3:
			return False
		Triangles = []
		NumReflex = 0
		for I11 in range(len(InputPolygon.Points)):
			I12 = len(InputPolygon.Points) - 1 if I11 == 0 else I11 - 1
			I13 = 0 if I11 == (len(InputPolygon.Points) - 1) else I11 + 1
			if self.IsReflex(InputPolygon.Points[I12], InputPolygon.Points[I11], InputPolygon.Points[I13]):
				NumReflex = 1
				break
		if NumReflex == 0:
			OutputPolygons.append(InputPolygon)
			return True
		if not self.Triangulate_EC(InputPolygon, Triangles):
			return False
		TriangleIndex1 = 0
		TriangleIndex2 = 0
		while TriangleIndex1 < len(Triangles):
			Polygon1 = Triangles[TriangleIndex1]
			Polygon2 = None
			for I11 in range(len(Polygon1.Points)):
				D1 = Polygon1.Points[I11]
				I12 = (I11 + 1) % len(Polygon1.Points)
				D2 = Polygon1.Points[I12]
				IsDiagonal = False
				TriangleIndex2 = TriangleIndex1
				while TriangleIndex2 < len(Triangles):
					if TriangleIndex1 != TriangleIndex2:
						try:
							Polygon2 = Triangles[TriangleIndex2]
							for I21 in range(len(Polygon2.Points)):
								if (D2.X != Polygon2.Points[I21].X) or (D2.Y != Polygon2.Points[I21].Y):
									#break
									raise GotoException
								I22 = (I21 + 1) % len(Polygon2.Points)
								if (D1.X != Polygon2.Points[I22].X) or (D1.Y != Polygon2.Points[I22].Y):
									#break
									raise GotoException
								IsDiagonal = True
								break
							if IsDiagonal:
								break
						except GotoException as E:
							pass
					TriangleIndex2 += 1
				if not IsDiagonal:
					continue
				P2 = Polygon1.Points[I11]
				I13 = len(Polygon1.Points) - 1 if I11 == 0 else I11 - 1
				P1 = Polygon1.Points[I13]
				I23 = 0 if I22 == (len(Polygon2.Points) - 1) else I22 + 1
				P3 = Polygon2.Points[I23]
				if not self.IsConvex(P1, P2, P3):
					continue
				P2 = Polygon1.Points[I12]
				I13 = 0 if I12 == (len(Polygon1.Points) - 1) else I12 + 1
				P3 = Polygon1.Points[I13]
				I23 = len(Polygon2.Points) - 1 if I21 == 0 else I21 - 1
				P1 = Polygon2.Points[I23]
				if not self.IsConvex(P1, P2, P3):
					continue
				NewPolygon = TPPLPolygon()
				for J in range(I12, I11):
					J = J % len(Polygon1.Points)
					NewPolygon.AddPoint(Polygon1.Points[J % len(Polygon1.Points)])
				for J in range(I22, I21):
					J = J % len(Polygon1.Points)
					NewPolygon.AddPoint(Polygon2.Points[J % len(Polygon2.Points)])
				Triangles.pop(TriangleIndex2)
				Triangles[TriangleIndex1] = NewPolygon
				Polygon1 = NewPolygon
				I11 = -1
				continue
			TriangleIndex1 += 1
		for Polygon in Triangles:
			OutputPolygons.append(Polygon)
		return True

	def Triangulate(self, InputPolygons, OutputPolygons):
		Polygons = [] 
		if not self.RemoveHoles(InputPolygons, Polygons):
			return False
		for Polygon in Polygons:
			if not self.Triangulate_EC(Polygon, OutputPolygons):
				return False
		return True

	def ConvexPartition(self, InputPolygons, OutputPolygons):
		Polygons = []
		if not self.RemoveHoles(InputPolygons, Polygons):
			return False
		for Polygon in Polygons:
			if not self.ConvexPartition_HM(Polygon, OutputPolygons):
				return False
		return True

#Class TPolygonGroup represents A Polygon in the map, obtained from A Sector, with Sidedef, Linedef and VERTEX data.
class TPolygonGroup:
	def __init__(self, Polygon):
		self.Polygon = Polygon
		self.Holes = []

# Class TMapDoor stores the doors, that will be included of the mesh.
class TMapDoor:
	def __init__(self, DoorSector, SideLine1, SideLine2, SideSector1, SideSector2):
		self.DoorSector = DoorSector
		self.SideLine1 = SideLine1
		self.SideLine2 = SideLine2
		self.SideSector1 = SideSector1
		self.SideSector2 = SideSector2
		self.DoorHeightCeiling = min(SideSector1.HeightCeiling, SideSector2.HeightCeiling) - 8

# Class TMapSector3D stores the 3D Sectors, that will be included of the mesh.
class TMapSector3D:
	def __init__(self, ControlLinedef, ControlSector, Arg0, Arg1):
		self.ControlLinedef = ControlLinedef
		self.ControlSector = ControlSector
		self.SectorTag = Arg0
		self.Swimmable = True if (ControlLinedef.Arg1 & 3) == 1 else False

# Class TMapTeleporter stores the teleporters, that will be included of the mesh.
class TMapTeleporter:
	def __init__(self, MapLinedef, DestinationThing, SectorTag):
		self.MapLinedef = MapLinedef
		self.DestinationThing = DestinationThing
		self.SectorTag = SectorTag

# Class TNavMeshSettings is used to configure the navigation mesh generation.
class TNavMeshSettings:
	def __init__(self, ActorHeight, ActorRadius):
		self.ActorHeight = ActorHeight
		self.ActorRadius = ActorRadius

# Class TNavMesh represents the navigation mesh of the processing map.
class TNavMesh:
	GridOffset = 32768

	def __init__(self):
		self.Lines = []
		self.Polygons = []
		self.Cells = None
		self.OffsetCellX = 0
		self.OffsetCellY = 0
		self.NumCellX = 0
		self.NumCellY = 0
		self.MapDoors = []
		self.MapTeleporters = []
		self.MapSectors3D = []
		self.Messages = []

	def CalcDirection(self, A, B, C):
		if A is None or B is None or C is None:
			raise ValueError("Arguments cannot be None")
		CrossProduct = (B.Y - A.Y) * (C.X - B.X) - (B.X - A.X) * (C.Y - B.Y)
		if CrossProduct == 0:
			return TOrientation.Collinear
		elif CrossProduct < 0:
			return TOrientation.CounterClockwise
		else:
			return TOrientation.Clockwise

	def PointOnLine(self, Line, Point):
		if Point is None:
			raise ValueError("Point cannot be None")
		return (Point.X <= max(Line.A.X, Line.B.X) and
				Point.X >= min(Line.A.X, Line.B.X) and
				Point.Y <= max(Line.A.Y, Line.B.Y) and
				Point.Y >= min(Line.A.Y, Line.B.Y))

	def LineIntersectLine(self, Line1, Line2):
		if Line1 is None or Line2 is None:
			raise ValueError("Lines cannot be None")
		Direction1 = self.CalcDirection(Line1.A, Line1.B, Line2.A)
		Direction2 = self.CalcDirection(Line1.A, Line1.B, Line2.B)
		Direction3 = self.CalcDirection(Line2.A, Line2.B, Line1.A)
		Direction4 = self.CalcDirection(Line2.A, Line2.B, Line1.B)
		if (Direction1 != Direction2) and (Direction3 != Direction4):
			return True
		if (Direction1 == TOrientation.Collinear and self.PointOnLine(Line1, Line2.A)):
			return True
		if (Direction2 == TOrientation.Collinear and self.PointOnLine(Line1, Line2.B)):
			return True
		if (Direction3 == TOrientation.Collinear and self.PointOnLine(Line2, Line1.A)):
			return True
		if (Direction4 == TOrientation.Collinear and self.PointOnLine(Line2, Line1.B)):
			return True
		return False

	def PointInsidePolygon(self, Polygon, Point):
		if len(Polygon.Lines) < 3:
			return False
		Result = False
		P1 = Polygon.Lines[0].A
		for Line in Polygon.Lines:
			P2 = Line.B
			if Point.Y > min(P1.Y, P2.Y):
				if Point.Y <= max(P1.Y, P2.Y):
					if Point.X <= max(P1.X, P2.X):
						IntersectionX = (Point.Y - P1.Y) * (P2.X - P1.X) / (P2.Y - P1.Y) + P1.X
						if (P1.X == P2.X) or (Point.X <= IntersectionX):
							Result = not Result
			P1 = P2
		return Result

	def PolygonInsidePolygon(self, OuterPolygon, TestPolygon):
		if len(OuterPolygon.Lines) < 3 or len(TestPolygon.Lines) < 3:
			return False
		return (self.PointInsidePolygon(OuterPolygon, TestPolygon.Lines[0].A) and
				self.PointInsidePolygon(OuterPolygon, TestPolygon.Lines[0].B))

	def CheckLinedefCrushing(self, MapLinedef):
		Result = False
		if ((not MapLinedef.Blocking) and (MapLinedef.SideFront >= 0) and (MapLinedef.SideBack >= 0)):
			if (self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceDoom):
				if ((MapLinedef.Special == 6)
				or (MapLinedef.Special == 25)
				or (MapLinedef.Special == 49)
				or (MapLinedef.Special == 55)
				or (MapLinedef.Special == 56)
				or (MapLinedef.Special == 57)
				or (MapLinedef.Special == 65)
				or (MapLinedef.Special == 73)
				or (MapLinedef.Special == 74)
				or (MapLinedef.Special == 77)
				or (MapLinedef.Special == 94)
				or (MapLinedef.Special == 141)
				or (MapLinedef.Special == 150)
				or (MapLinedef.Special == 164)
				or (MapLinedef.Special == 165)
				or (MapLinedef.Special == 168)
				or (MapLinedef.Special == 183)
				or (MapLinedef.Special == 184)
				or (MapLinedef.Special == 185)
				or (MapLinedef.Special == 188)
				or ((MapLinedef.Special >= 0x2F80) and (MapLinedef.Special <= 0x2FFF))
				or ((MapLinedef.Special >= 0x4000) and (MapLinedef.Special <= 0x5FFF))
				or ((MapLinedef.Special >= 0x6000) and (MapLinedef.Special <= 0x7FFF))):
					Result = True
			elif (self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceZDoom):
				if ((MapLinedef.MonsterUse) and (MapLinedef.RepeatableSpecial)
				and ((MapLinedef.Special == 28)
				or (MapLinedef.Special == 42)
				or (MapLinedef.Special == 43)
				or (MapLinedef.Special == 45)
				or (MapLinedef.Special == 97)
				or (MapLinedef.Special == 99)
				or (MapLinedef.Special == 104)
				or (MapLinedef.Special == 168)
				or (MapLinedef.Special == 169)
				or (MapLinedef.Special == 195)
				or (MapLinedef.Special == 196)
				or (MapLinedef.Special == 197)
				or (MapLinedef.Special == 205)
				or (MapLinedef.Special == 284))):
					Result = True
		return Result

	def CheckLinedefDoor(self, MapLinedef):
		Result = False
		if ((not MapLinedef.Blocking) and (MapLinedef.SideFront >= 0) and (MapLinedef.SideBack >= 0)):
			if (self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceDoom):
				if ((MapLinedef.Special == 1) or (MapLinedef.Special == 32) or (MapLinedef.Special == 33) or (MapLinedef.Special == 34)):
					Result = True
			elif (self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceZDoom):
				if ((MapLinedef.MonsterUse) and (MapLinedef.RepeatableSpecial)):
					if ((MapLinedef.Special == 11) or (MapLinedef.Special == 12)):
						Result = True
		return Result

	def CheckLinedefTeleporter(self, MapLinedef):
		Result = False
		if ((not MapLinedef.Blocking) and (MapLinedef.SideFront >= 0) and (MapLinedef.SideBack >= 0)):
			if (self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceDoom):
				if ((MapLinedef.Special == 97) or (MapLinedef.Special == 126)):
					Result = True
			elif (self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceZDoom):
				if ((MapLinedef.MonsterUse) and (MapLinedef.RepeatableSpecial)):
					if ((MapLinedef.Special == 70) or (MapLinedef.Special == 71)):
						Result = True
		return Result

	def CheckLinedef3D(self, MapLinedef):
		Result = False
		if (self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceZDoom):
			if ((MapLinedef.Special == 160) and ((((MapLinedef.Arg1 & 0x0003) == 0x0001) or ((MapLinedef.Arg1 & 0x0003) == 0x0002)))):
				Result = True
		return Result
	
	def PreProcessMapData(self):
		for MapSector in self.MapDefinition.MapSector:
			if MapSector.Special == 10:
				MapSector.Ignored = True
		for MapLinedef in self.MapDefinition.MapLinedef:
			if self.CheckLinedefCrushing(MapLinedef):
				if MapLinedef.Arg0 == 0:
					if MapLinedef.SideBack >= 0:
						SectorIndex = self.MapDefinition.MapSidedef[MapLinedef.SideBack].Sector
						self.MapDefinition.MapSector[SectorIndex].Ignored = True
				else:
					for MapSector in self.MapDefinition.MapSector:
						if MapSector.ID == MapLinedef.Arg0:
							MapSector.Ignored = True
			if self.CheckLinedefDoor(MapLinedef):
				DoorSide1Sector = self.MapDefinition.MapSidedef[MapLinedef.SideFront].Sector
				DoorSector = self.MapDefinition.MapSidedef[MapLinedef.SideBack].Sector
				OtherLinedef = -1
				for SearchSidedef in filter(lambda Sidedef: Sidedef.Sector == DoorSector and Sidedef.Index != MapLinedef.SideBack, self.MapDefinition.MapSidedef):
					for SearchLinedef in self.MapDefinition.MapLinedef:
						if SearchLinedef.SideBack == SearchSidedef.Index and self.CheckLinedefDoor(SearchLinedef):
							OtherLinedef = SearchLinedef.Index
							break
				if OtherLinedef >= 0:
					DoorSide2Sector = self.MapDefinition.MapSidedef[self.MapDefinition.MapLinedef[OtherLinedef].SideFront].Sector
					MapDoor = TMapDoor(self.MapDefinition.MapSector[DoorSector], MapLinedef, self.MapDefinition.MapLinedef[OtherLinedef], self.MapDefinition.MapSector[DoorSide1Sector], self.MapDefinition.MapSector[DoorSide2Sector])
					self.MapDoors.append(MapDoor)
			if self.CheckLinedefTeleporter(MapLinedef):
				MapTeleporter = None
				if self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceDoom:
					MapTeleporter = TMapTeleporter(MapLinedef, None, MapLinedef.Arg0)
					self.MapTeleporters.append(MapTeleporter)
				elif self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceHexen:
					DestinationCount = 0
					ThingIndex = 0
					for MapThing in self.MapDefinition.MapThing:
						if MapThing.ThingType == 14 and MapThing.ID == MapLinedef.Arg0:
							ThingIndex = MapThing.Index
							DestinationCount += 1
							if DestinationCount > 1:
								break
					if DestinationCount == 1:
						MapTeleporter = TMapTeleporter(MapLinedef, self.MapDefinition.MapThing[ThingIndex], MapLinedef.Arg1)
						self.MapTeleporters.append(MapTeleporter)
				elif self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceZDoom:
					DestinationCount = 0
					ThingIndex = 0
					for MapThing in self.MapDefinition.MapThing:
						if (MapThing.ThingType == 14 or MapThing.ThingType == 9044) and MapThing.ID == MapLinedef.Arg0:
							ThingIndex = MapThing.Index
							DestinationCount += 1
							if DestinationCount > 1:
								break
					if DestinationCount == 1:
						MapTeleporter = TMapTeleporter(MapLinedef, self.MapDefinition.MapThing[ThingIndex], MapLinedef.Arg1)
						self.MapTeleporters.append(MapTeleporter)
			if self.CheckLinedef3D(MapLinedef):
				ControlSector = self.MapDefinition.MapSidedef[MapLinedef.SideFront].Sector
				MapSector3D = TMapSector3D(MapLinedef, self.MapDefinition.MapSector[ControlSector])
				self.MapSectors3D.append(MapSector3D)

	def GetPolygonGroups(self, Sector) -> Optional[List['TPolygonGroup']]:
		if len(Sector.Lines) < 3:
			return None
		SectorPolygons = []
		Sectors = []
		CheckedLine = [False] * len(Sector.Lines)
		while True:
			FirstLine = next((I for I, Line in enumerate(CheckedLine) if not Line), -1)
			if FirstLine >= 0:
				Polygon = TPolygon()
				Polygon.HeightFloor = Sector.HeightFloor
				Polygon.HeightCeiling = Sector.HeightCeiling
				Polygon.MapSector = Sector.MapSector
				Polygon.AddLine(Sector.Lines[FirstLine])
				CheckedLine[FirstLine] = True
				CurrentLine = FirstLine
				while True:
					NextLine = next((I for I, Line in enumerate(Sector.Lines) if Line.A == Sector.Lines[CurrentLine].B), -1)
					if NextLine >= 0 and CheckedLine[NextLine]:
						break
					if NextLine >= 0 and NextLine != FirstLine:
						Polygon.AddLine(Sector.Lines[NextLine])
						CheckedLine[NextLine] = True
						CurrentLine = NextLine
				if Polygon.IsClosed():
					Polygon.SortLines()
					Sectors.append(Polygon)
			else:
				break
		if not Sectors:
			return None
		if len(Sectors) == 1:
			SectorPolygon = TPolygonGroup(Sectors[0])
			SectorPolygons.append(SectorPolygon)
		else:
			PolygonDepthLevel = [0] * len(Sectors)
			for I in range(len(Sectors)):
				for J in range(len(Sectors)):
					if I != J and self.PolygonInsidePolygon(Sectors[I], Sectors[J]):
						PolygonDepthLevel[J] += 1
			MaxDepthLevel = max(PolygonDepthLevel)
			for DepthLevel in range(0, MaxDepthLevel + 1, 2):
				for I in range(len(Sectors)):
					if PolygonDepthLevel[I] == DepthLevel:
						SectorPolygon = TPolygonGroup(Sectors[I])
						for J in range(len(Sectors)):
							if I != J and self.PolygonInsidePolygon(Sectors[I], Sectors[J]):
								IgnoreHole = False
								FirstHoleLinedef = self.MapDefinition.MapLinedef[Sectors[J].Lines[0].MapLinedef]
								if FirstHoleLinedef.SideFront >= 0 and FirstHoleLinedef.SideBack >= 0:
									HoleSector = self.MapDefinition.MapSidedef[FirstHoleLinedef.SideFront].Sector
									if HoleSector == Sector.MapSector:
										HoleSector = self.MapDefinition.MapSidedef[FirstHoleLinedef.SideBack].Sector
									IgnoreHole = self.MapDefinition.MapSector[HoleSector].Ignored
								if not IgnoreHole:
									SectorPolygon.Holes.append(Sectors[J])
						SectorPolygons.append(SectorPolygon)
		return SectorPolygons

	def ProcessPolygonMesh(self, Polygon, MapSector):
		MapDoorIndex = -1
		try:
			MapDoorIndex = map(lambda MapDoor: MapDoor.DoorSector.Index == MapSector, self.MapDoors).index(True)
		except:
			pass
		if MapDoorIndex >= 0:
			Polygon.HeightCeiling = self.MapDefinition.MapDoors[MapDoorIndex].DoorHeightCeiling
		if (Polygon.HeightCeiling - Polygon.HeightFloor) < self.NavMeshSettings.ActorHeight:
			return
		for Line in Polygon.Lines:
			MapLinedefIndex = -1
			try:
				MapLinedefIndex = map(lambda Linedef: (Line.A.X == self.MapDefinition.MapVertex[Linedef.V1].X and Line.A.Y == self.MapDefinition.MapVertex[Linedef.V1].Y and Line.B.X == self.MapDefinition.MapVertex[Linedef.V2].X and Line.B.Y == self.MapDefinition.MapVertex[Linedef.V2].Y) or (Line.A.X == self.MapDefinition.MapVertex[Linedef.V2].X and Line.A.Y == self.MapDefinition.MapVertex[Linedef.V2].Y and Line.B.X == self.MapDefinition.MapVertex[Linedef.V1].X and Line.B.Y == self.MapDefinition.MapVertex[Linedef.V1].Y), self.MapDefinition.MapLinedef).index(True)
			except:
				pass
			Line.MapLinedef = MapLinedefIndex
			CurrentPolygonIndex = 0
			CurrentPolygonLine = 0
			for I in range(len(self.Lines)):
				if ((Line.A == self.Lines[I].A and Line.B == self.Lines[I].B) or (Line.A == self.Lines[I].B and Line.B == self.Lines[I].A)):
					LineIsPortal = True
					if MapLinedefIndex >= 0:
						MapLinedef = self.MapDefinition.MapLinedef[MapLinedefIndex]
						if (MapLinedef.Blocking or MapLinedef.SideFront < 0 or MapLinedef.SideBack < 0 or MapLinedef.Ignored):
							LineIsPortal = False
						HeightFloorDifference = abs(Polygon.HeightFloor - self.Polygons[CurrentPolygonIndex].HeightFloor)
						if HeightFloorDifference > 24:
							LineIsPortal = False
						VerticalSpace = min(Polygon.HeightCeiling, self.Polygons[CurrentPolygonIndex].HeightCeiling) - max(Polygon.HeightFloor, self.Polygons[CurrentPolygonIndex].HeightFloor)
						if VerticalSpace < self.NavMeshSettings.ActorHeight:
							LineIsPortal = False
					else:
						HeightFloorDifference = abs(Polygon.HeightFloor - self.Polygons[CurrentPolygonIndex].HeightFloor)
						if HeightFloorDifference > 24:
							LineIsPortal = False
						VerticalSpace = min(Polygon.HeightCeiling, self.Polygons[CurrentPolygonIndex].HeightCeiling) - max(Polygon.HeightFloor, self.Polygons[CurrentPolygonIndex].HeightFloor)
						if VerticalSpace < self.NavMeshSettings.ActorHeight:
							LineIsPortal = False
					if LineIsPortal:
						Line.Portal = CurrentPolygonIndex
						self.Lines[I].Portal = len(self.Polygons)
				CurrentPolygonLine += 1
				if CurrentPolygonLine == self.Polygons[CurrentPolygonIndex].LineCount:
					CurrentPolygonLine = 0
					CurrentPolygonIndex += 1
		Polygon.LineFirst = len(self.Lines)
		Polygon.LineCount = len(Polygon.Lines)
		self.Polygons.append(Polygon)
		self.Lines.extend(Polygon.Lines)

	def ProcessTeleporters(self):
		V1 = TPoint(0, 0)
		V2 = TPoint(0, 0)
		TeleporterDestinationCenter = TPoint(0, 0)
		for Line in self.Lines:
			V1.X = Line.A.X
			V1.Y = Line.A.Y
			V2.X = Line.B.X
			V2.Y = Line.B.Y
			for MapTeleporter in self.MapTeleporters:
				if MapTeleporter.DestinationThing is not None:
					if (Line.A == V1 and Line.B == V2) or (Line.A == V2 and Line.B == V1):
						TeleporterDestinationCenter.X = MapTeleporter.DestinationThing.X
						TeleporterDestinationCenter.Y = MapTeleporter.DestinationThing.Y
						PolygonIndex = 0
						NotFound = True
						while NotFound and PolygonIndex < len(self.Polygons):
							if PointInsidePolygon(self.Polygons[PolygonIndex], TeleporterDestinationCenter):
								if (self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceZDoom and 
									MapTeleporter.DestinationThing.ThingType == 9044):
									if (MapTeleporter.DestinationThing.Z >= self.Polygons[PolygonIndex].HeightFloor and 
										MapTeleporter.DestinationThing.Z <= self.Polygons[PolygonIndex].HeightCeiling):
										NotFound = False
							PolygonIndex += 1
						if not NotFound:
							Line.Portal = PolygonIndex

	def ProcessCells(self):
		MaxX = None
		MaxY = None
		MinX = None
		MinY = None
		for Line in self.Lines:
			if MaxX == None or Line.A.X > MaxX:
				MaxX = Line.A.X
			if MaxY == None or Line.A.Y > MaxY:
				MaxY = Line.A.Y
			if MaxX == None or Line.B.X > MaxX:
				MaxX = Line.B.X
			if MaxY == None or Line.B.Y > MaxY:
				MaxY = Line.B.Y
			if MinX == None or Line.A.X < MinX:
				MinX = Line.A.X
			if MinY == None or Line.A.Y < MinY:
				MinY = Line.A.Y
			if MinX == None or Line.B.X < MinX:
				MinX = Line.B.X
			if MinY == None or Line.B.Y < MinY:
				MinY = Line.B.Y
		self.OffsetCellX = (MinX + self.GridOffset) >> 8
		self.OffsetCellY = (MinY + self.GridOffset) >> 8
		LastCellX = (MaxX + self.GridOffset) >> 8
		LastCellY = (MaxY + self.GridOffset) >> 8
		self.NumCellX = LastCellX - self.OffsetCellX + 1
		self.NumCellY = LastCellY - self.OffsetCellY + 1
		self.Cells = [[list() for _ in range(self.NumCellX)] for _ in range(self.NumCellY)]
		for PolygonIndex in range(len(self.Polygons)):
			MaxX = None
			MaxY = None
			MinX = None
			MinY = None
			for Line in self.Polygons[PolygonIndex].Lines:
				if MaxX == None or Line.A.X > MaxX:
					MaxX = Line.A.X
				if MaxY == None or Line.A.Y > MaxY:
					MaxY = Line.A.Y
				if MaxX == None or Line.B.X > MaxX:
					MaxX = Line.B.X
				if MaxY == None or Line.B.Y > MaxY:
					MaxY = Line.B.Y
				if MinX == None or Line.A.X < MinX:
					MinX = Line.A.X
				if MinY == None or Line.A.Y < MinY:
					MinY = Line.A.Y
				if MinX == None or Line.B.X < MinX:
					MinX = Line.B.X
				if MinY == None or Line.B.Y < MinY:
					MinY = Line.B.Y
			MinX = (MinX + self.GridOffset) >> 8
			MinY = (MinY + self.GridOffset) >> 8
			MaxX = (MaxX + self.GridOffset) >> 8
			MaxY = (MaxY + self.GridOffset) >> 8
			for Y in range(MinY, MaxY + 1):
				for X in range(MinX, MaxX + 1):
					self.Cells[Y - self.OffsetCellY][X - self.OffsetCellX].append(PolygonIndex)
	
	def ProcessPolygons(self, Polygons, HeightFloor, HeightCeiling, MapSector, Flags = 0):
		for Polygon in Polygons:
			Mesh = TPolygon()
			Mesh.HeightFloor = HeightFloor
			Mesh.HeightCeiling = HeightCeiling
			Mesh.MapSector = MapSector
			Mesh.Flags = Flags 
			for I in range(len(Polygon.Points)):
				J = (I + 1) % len(Polygon.Points)
				Mesh.Lines.append(
					TLine(
						TPoint(int(Polygon.Points[I].X), int(Polygon.Points[I].Y)),
						TPoint(int(Polygon.Points[J].X), int(Polygon.Points[J].Y))
					)
				)
			Polygon.SetOrientation(TOrientation.CounterClockwise)
			self.ProcessPolygonMesh(Mesh, MapSector)
			
	def ProcessMapData(self):
		PolygonGroups = None
		InputPolygons = []
		OutputPolygons = []
		for MapSector in self.MapDefinition.MapSector:
			if not MapSector.Ignored:
				SectorPolygon = TPolygon()
				SectorPolygon.MapSector = MapSector.Index
				for MapLinedef in self.MapDefinition.MapLinedef:
					if (MapLinedef.SideFront >= 0) and (self.MapDefinition.MapSidedef[MapLinedef.SideFront].Sector == MapSector.Index):
						Line = TLine(
							TPoint(self.MapDefinition.MapVertex[MapLinedef.V1].X, self.MapDefinition.MapVertex[MapLinedef.V1].Y),
							TPoint(self.MapDefinition.MapVertex[MapLinedef.V2].X, self.MapDefinition.MapVertex[MapLinedef.V2].Y)
						)
						Line.MapLinedef = MapLinedef.Index
						SectorPolygon.AddLine(Line)
				for MapLinedef in self.MapDefinition.MapLinedef:
					if (MapLinedef.SideBack >= 0) and (self.MapDefinition.MapSidedef[MapLinedef.SideBack].Sector == MapSector.Index):
						Line = TLine(
							TPoint(self.MapDefinition.MapVertex[MapLinedef.V2].X, self.MapDefinition.MapVertex[MapLinedef.V2].Y),
							TPoint(self.MapDefinition.MapVertex[MapLinedef.V1].X, self.MapDefinition.MapVertex[MapLinedef.V1].Y)
						)
						Line.MapLinedef = MapLinedef.Index
						SectorPolygon.AddLine(Line)
				PolygonGroups = self.GetPolygonGroups(SectorPolygon)
				if PolygonGroups is not None:
					for PolygonGroup in PolygonGroups:
						if self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceDoom:
							for MapTeleporter in MapTeleporters:
								if MapTeleporter.SectorTag == MapSector.id:
									TeleporterDestinationCenter = TPoint(0, 0)
									ThingIndex = 0
									NotFound = True
									while NotFound and (ThingIndex < len(self.MapDefinition.MapThing)):
										if self.MapDefinition.MapThing[ThingIndex].ThingType == 14:
											DestinationInSector = False
											DestinationInHoles = False
											TeleporterDestinationCenter.X = self.MapDefinition.MapThing[ThingIndex].X
											TeleporterDestinationCenter.Y = self.MapDefinition.MapThing[ThingIndex].Y
											if self.PointInsidePolygon(PolygonGroup.Polygon, TeleporterDestinationCenter):
												DestinationInSector = True
												for HolePolygon in PolygonGroup.Holes:
													if self.PointInsidePolygon(HolePolygon, TeleporterDestinationCenter):
														DestinationInHoles = True
														break
											if DestinationInSector and not DestinationInHoles:
												NotFound = False
											else:
												ThingIndex += 1
										else:
											ThingIndex += 1
									if NotFound:
										MapTeleporter.DestinationThing = None
									else:
										MapTeleporter.DestinationThing = self.MapDefinition.MapThing[ThingIndex]
						InputPolygons.clear()
						Polygon = TPPLPolygon()
						for Line in PolygonGroup.Polygon.Lines:
							Polygon.AddPoint(TPPLPoint(Line.A.X, Line.A.Y))
						Polygon.Hole = False
						InputPolygons.append(Polygon)
						for Sector in PolygonGroup.Holes:
							Hole = TPPLPolygon()
							for Line in Sector.Lines:
								Hole.AddPoint(TPPLPoint(Line.A.X, Line.A.Y))
							Hole.Hole = True
							InputPolygons.append(Hole)
						OutputPolygons.clear()
						PPLPartition = TPPLPartition()
						if not PPLPartition.ConvexPartition(InputPolygons, OutputPolygons):
							if not PPLPartition.Triangulate(InputPolygons, OutputPolygons):
								self.Messages.append(f"Map Sector # {MapSector.Index} could not be processed.")
						if len(OutputPolygons) > 0:
							Sector3D = next((I for I, S in enumerate(self.MapSectors3D) if S.SectorTag == MapSector.ID), -1)
							if Sector3D >= 0:
								MiddleFloor = MapSectors3D[Sector3D].ControlSector.HeightFloor
								MiddleCeiling = MapSectors3D[Sector3D].ControlSector.HeightCeiling
								if MiddleFloor > MapSector.HeightFloor:
									self.ProcessPolygons(OutputPolygons, MapSector.HeightFloor, MiddleFloor, MapSector.Index, 0)
								if MiddleCeiling < MapSector.HeightCeiling:
									self.ProcessPolygons(OutputPolygons, MiddleCeiling, MapSector.HeightCeiling, MapSector.Index, 0x0003 if MapSectors3D[Sector3D].swimmable else 0x0001)
							else:
								self.ProcessPolygons(OutputPolygons, MapSector.HeightFloor, MapSector.HeightCeiling, MapSector.Index)
	
	def Build(self, NavMeshSettings, MapDefinition):
		self.MapDefinition = MapDefinition
		self.NavMeshSettings = NavMeshSettings
		self.PreProcessMapData()
		self.ProcessMapData()
		if (len(self.Polygons) > 0):
			self.ProcessTeleporters()
			self.ProcessCells()
	
	def GetMessages(self):
		StringBuilder = []
		for index in range(len(self.Messages)):
			StringBuilder.append(self.Messages[index] + "\n")
		return "".join(StringBuilder)
	
	def GetText(self):
		StringBuilder = []
		StringBuilder.append("# ZDOOMNAVMESH\n\n")
		StringBuilder.append("# lines\n")
		for index in range(len(self.Lines)):
			StringBuilder.append("l {} {} {} {} {} {} {}\n".format(
					self.Lines[index].A.X, 
					self.Lines[index].A.Y, 
					self.Lines[index].B.X, 
					self.Lines[index].B.Y, 
					self.Lines[index].Portal, 
					self.Lines[index].MapLinedef, 
					self.Lines[index].Flags
			))
		StringBuilder.append("\n")
		StringBuilder.append("# polygons\n")
		for index in range(len(self.Polygons)):
			StringBuilder.append("p {} {} {} {} {} {}\n".format(
					self.Polygons[index].HeightFloor, 
					self.Polygons[index].HeightCeiling, 
					self.Polygons[index].LineFirst, 
					self.Polygons[index].LineCount, 
					self.Polygons[index].MapSector, 
					self.Polygons[index].Flags
			))
		StringBuilder.append("\n")
		StringBuilder.append("# cells space partitioning\n")
		StringBuilder.append("o {} {} {} {}\n".format(
				self.OffsetCellX, 
				self.OffsetCellY, 
				self.NumCellX, 
				self.NumCellY
		))
		for Y in range(self.NumCellY):
			for X in range(self.NumCellX):
				if len(self.Cells[Y][X]) > 0:
					StringBuilder.append("c {}".format(Y * self.NumCellX + X))
					for PolygonIndex in self.Cells[Y][X]:
						StringBuilder.append(" {}".format(PolygonIndex))
					StringBuilder.append("\n")
		return "".join(StringBuilder)
	
	def __str__(self):
		return self.GetText()
