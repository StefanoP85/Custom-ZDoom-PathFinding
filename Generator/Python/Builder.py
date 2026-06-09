"""
- Author: Pollazzon Stefano (ported)
- Project: ZDoom Navmesh builder
- This module contains the navmesh generation algorithms
"""

from __future__ import annotations

import math
from enum import Enum
from typing import Dict, List, Optional

from Structures import (
    TMapDefinition,
    TMapLinedef,
    TMapNamespace,
    TMapSector,
    TMapVertex,
)


# --------------------------------------------------
# Core geometry / navmesh classes
# --------------------------------------------------

class TOrientation(Enum):
    Clockwise = -1
    Collinear = 0
    CounterClockwise = 1


class TNavMeshPoint:
    def __init__(self, X: int, Y: int):
        self.X = int(X)
        self.Y = int(Y)

    def __eq__(self, other):
        if other is None or not isinstance(other, TNavMeshPoint):
            return False
        return self.X == other.X and self.Y == other.Y

    def __ne__(self, other):
        return not self.__eq__(other)


class TNavMeshLine:
    def __init__(self, A: TNavMeshPoint, B: TNavMeshPoint):
        self.A = A
        self.B = B
        self.Portal = -1
        self.MapLinedef = -1

    def __eq__(self, other):
        if other is None or not isinstance(other, TNavMeshLine):
            return False
        return self.A == other.A and self.B == other.B

    def __ne__(self, other):
        return not self.__eq__(other)


class TNavMeshPolygon:
    def __init__(self):
        self.Lines: List[TNavMeshLine] = []
        self.HeightFloor = 0
        self.HeightCeiling = 0
        self.LineFirst = 0
        self.LineCount = 0
        self.MapSector = -1


class TPoint:
    def __init__(self, X: int = 0, Y: int = 0):
        self.X = int(X)
        self.Y = int(Y)

    def __eq__(self, other):
        if other is None or not isinstance(other, TPoint):
            return False
        return self.X == other.X and self.Y == other.Y

    def __ne__(self, other):
        return not self.__eq__(other)


class TPolygon:
    def __init__(self, P1: Optional[TPoint] = None, P2: Optional[TPoint] = None, P3: Optional[TPoint] = None):
        self.Points: List[TPoint] = []
        self._hole = False
        if P1 is not None and P2 is not None and P3 is not None:
            self.Points = [P1, P2, P3]

    def GetOrientation(self) -> TOrientation:
        area = 0.0
        for i1 in range(len(self.Points)):
            i2 = i1 + 1
            if i2 == len(self.Points):
                i2 = 0
            area += self.Points[i1].X * self.Points[i2].Y - self.Points[i1].Y * self.Points[i2].X
        if area > 0:
            return TOrientation.CounterClockwise
        if area < 0:
            return TOrientation.Clockwise
        return TOrientation.Collinear

    def SetOrientation(self, Orientation: TOrientation):
        actual_orientation = self.GetOrientation()
        if actual_orientation != TOrientation.Collinear and actual_orientation != Orientation:
            self.Points.reverse()

    def SetHole(self, AHole: bool):
        self._hole = bool(AHole)
        if self._hole:
            self.SetOrientation(TOrientation.Clockwise)
        else:
            self.SetOrientation(TOrientation.CounterClockwise)

    @property
    def Hole(self) -> bool:
        return self._hole

    @Hole.setter
    def Hole(self, value: bool):
        self.SetHole(value)


class TPartitionVertex:
    def __init__(self, APoint: TPoint):
        self.Point = APoint
        self.IsActive = False
        self.IsConvex = False
        self.IsEar = False
        self.Angle = 0.0
        self.Previous: TPartitionVertex = self
        self.Next: TPartitionVertex = self


class TPartition:
    @staticmethod
    def IsConvex(*args) -> bool:
        if len(args) == 3 and isinstance(args[0], TPoint):
            P1, P2, P3 = args
            return (P3.Y - P1.Y) * (P2.X - P1.X) - (P3.X - P1.X) * (P2.Y - P1.Y) > 0
        elif len(args) == 6:
            P1X, P1Y, P2X, P2Y, P3X, P3Y = args
            return (P3Y - P1Y) * (P2X - P1X) - (P3X - P1X) * (P2Y - P1Y) > 0
        raise TypeError("Invalid IsConvex overload")

    @staticmethod
    def IsReflex(P1: TPoint, P2: TPoint, P3: TPoint) -> bool:
        return (P3.Y - P1.Y) * (P2.X - P1.X) - (P3.X - P1.X) * (P2.Y - P1.Y) < 0

    @staticmethod
    def IsInside(P1: TPoint, P2: TPoint, P3: TPoint, Point: TPoint) -> bool:
        if TPartition.IsConvex(P1, Point, P2):
            return False
        if TPartition.IsConvex(P2, Point, P3):
            return False
        if TPartition.IsConvex(P3, Point, P1):
            return False
        return True

    @staticmethod
    def InCone(*args) -> bool:
        if len(args) == 4 and isinstance(args[0], TPoint):
            P1, P2, P3, Point = args
            if TPartition.IsConvex(P1, P2, P3):
                if not TPartition.IsConvex(P1, P2, Point):
                    return False
                if not TPartition.IsConvex(P2, P3, Point):
                    return False
                return True
            else:
                if TPartition.IsConvex(P1, P2, Point):
                    return True
                if TPartition.IsConvex(P2, P3, Point):
                    return True
                return False

        elif len(args) == 8:
            P1X, P1Y, P2X, P2Y, P3X, P3Y, PointX, PointY = args
            if TPartition.IsConvex(P1X, P1Y, P2X, P2Y, P3X, P3Y):
                if not TPartition.IsConvex(P1X, P1Y, P2X, P2Y, PointX, PointY):
                    return False
                if not TPartition.IsConvex(P2X, P2Y, P3X, P3Y, PointX, PointY):
                    return False
                return True
            else:
                if TPartition.IsConvex(P1X, P1Y, P2X, P2Y, PointX, PointY):
                    return True
                if TPartition.IsConvex(P2X, P2Y, P3X, P3Y, PointX, PointY):
                    return True
                return False

        raise TypeError("Invalid InCone overload")

    @staticmethod
    def Intersects(P11: TPoint, P12: TPoint, P21: TPoint, P22: TPoint) -> bool:
        if (P11.X == P21.X) and (P11.Y == P21.Y):
            return False
        if (P11.X == P22.X) and (P11.Y == P22.Y):
            return False
        if (P12.X == P21.X) and (P12.Y == P21.Y):
            return False
        if (P12.X == P22.X) and (P12.Y == P22.Y):
            return False

        V1OrtX = P12.Y - P11.Y
        V1OrtY = P11.X - P12.X
        V2OrtX = P22.Y - P21.Y
        V2OrtY = P21.X - P22.X

        Dot21 = (P21.X - P11.X) * V1OrtX + (P21.Y - P11.Y) * V1OrtY
        Dot22 = (P22.X - P11.X) * V1OrtX + (P22.Y - P11.Y) * V1OrtY
        Dot11 = (P11.X - P21.X) * V2OrtX + (P11.Y - P21.Y) * V2OrtY
        Dot12 = (P12.X - P21.X) * V2OrtX + (P12.Y - P21.Y) * V2OrtY

        if Dot11 * Dot12 > 0:
            return False
        if Dot21 * Dot22 > 0:
            return False
        return True

    @staticmethod
    def GetUnitVector(X: int, Y: int):
        length = math.sqrt(X * X + Y * Y)
        return (X / length, Y / length)

    @staticmethod
    def GetAngle(V1: TPoint, V2: TPoint, V3: TPoint) -> float:
        Vector1X, Vector1Y = TPartition.GetUnitVector(V1.X - V2.X, V1.Y - V2.Y)
        Vector3X, Vector3Y = TPartition.GetUnitVector(V3.X - V2.X, V3.Y - V2.Y)
        return Vector1X * Vector3X + Vector1Y * Vector3Y

    @staticmethod
    def UpdateVertex(Vertex: TPartitionVertex, Vertices: List[TPartitionVertex], NumVertices: int):
        V1 = Vertex.Previous
        V3 = Vertex.Next
        Vertex.IsConvex = TPartition.IsConvex(V1.Point, Vertex.Point, V3.Point)
        Vertex.Angle = TPartition.GetAngle(V1.Point, Vertex.Point, V3.Point)

        if Vertex.IsConvex:
            Vertex.IsEar = True
            for I in range(NumVertices):
                if (Vertices[I].Point.X == Vertex.Point.X) and (Vertices[I].Point.Y == Vertex.Point.Y):
                    continue
                if (Vertices[I].Point.X == V1.Point.X) and (Vertices[I].Point.Y == V1.Point.Y):
                    continue
                if (Vertices[I].Point.X == V3.Point.X) and (Vertices[I].Point.Y == V3.Point.Y):
                    continue
                if TPartition.IsInside(V1.Point, Vertex.Point, V3.Point, Vertices[I].Point):
                    Vertex.IsEar = False
                    break
        else:
            Vertex.IsEar = False

    @staticmethod
    def RemoveHoles(Polygon: TPolygon, Holes: List[TPolygon]) -> bool:
        if len(Holes) == 0:
            return True

        while len(Holes) > 0:
            selected_hole = Holes[0]
            hole_point_index = 0
            poly_point_index = 0

            for hole in Holes:
                for i in range(len(hole.Points)):
                    if hole.Points[i].X > selected_hole.Points[hole_point_index].X:
                        selected_hole = hole
                        hole_point_index = i

            hole_point = selected_hole.Points[hole_point_index]
            point_found = False
            best_poly_point = Polygon.Points[0]

            for i in range(len(Polygon.Points)):
                if Polygon.Points[i].X <= hole_point.X:
                    continue
                if not TPartition.InCone(
                    Polygon.Points[(i + len(Polygon.Points) - 1) % len(Polygon.Points)],
                    Polygon.Points[i],
                    Polygon.Points[(i + 1) % len(Polygon.Points)],
                    hole_point
                ):
                    continue

                poly_point = Polygon.Points[i]
                if point_found:
                    UnitX, UnitY = TPartition.GetUnitVector(poly_point.X - hole_point.X, poly_point.Y - hole_point.Y)
                    BestX, BestY = TPartition.GetUnitVector(best_poly_point.X - hole_point.X, best_poly_point.Y - hole_point.Y)
                    if BestX > UnitX:
                        continue

                point_visible = True
                for j in range(len(Polygon.Points)):
                    line_p1 = Polygon.Points[j]
                    line_p2 = Polygon.Points[(j + 1) % len(Polygon.Points)]
                    if TPartition.Intersects(hole_point, poly_point, line_p1, line_p2):
                        point_visible = False
                        break

                if point_visible:
                    point_found = True
                    best_poly_point = poly_point
                    poly_point_index = i

            if not point_found:
                return False

            for i in range(len(selected_hole.Points) + 1):
                Polygon.Points.insert(poly_point_index + i + 1, selected_hole.Points[(i + hole_point_index) % len(selected_hole.Points)])
            Polygon.Points.insert(poly_point_index + len(selected_hole.Points) + 2, Polygon.Points[poly_point_index])
            Holes.remove(selected_hole)

        return True

    @staticmethod
    def Triangulate_EC(InputPolygon: TPolygon, OutputPolygons: List[TPolygon]) -> bool:
        if len(InputPolygon.Points) < 3:
            return False
        if len(InputPolygon.Points) == 3:
            OutputPolygons.append(InputPolygon)
            return True

        num_vertices = len(InputPolygon.Points)
        vertices = [TPartitionVertex(InputPolygon.Points[i]) for i in range(num_vertices)]
        ear = vertices[0]

        for i in range(num_vertices):
            vertices[i].IsActive = True
            vertices[i].Next = vertices[0] if i == (num_vertices - 1) else vertices[i + 1]
            vertices[i].Previous = vertices[num_vertices - 1] if i == 0 else vertices[i - 1]

        for i in range(num_vertices):
            TPartition.UpdateVertex(vertices[i], vertices, num_vertices)

        for i in range(num_vertices - 3):
            ear_found = False
            for j in range(num_vertices):
                if not vertices[j].IsActive:
                    continue
                if not vertices[j].IsEar:
                    continue
                if not ear_found:
                    ear_found = True
                    ear = vertices[j]
                else:
                    if vertices[j].Angle > ear.Angle:
                        ear = vertices[j]

            if not ear_found:
                return False

            triangle = TPolygon(ear.Previous.Point, ear.Point, ear.Next.Point)
            OutputPolygons.append(triangle)

            ear.IsActive = False
            ear.Previous.Next = ear.Next
            ear.Next.Previous = ear.Previous

            if i == num_vertices - 4:
                break

            TPartition.UpdateVertex(ear.Previous, vertices, num_vertices)
            TPartition.UpdateVertex(ear.Next, vertices, num_vertices)

        for i in range(num_vertices):
            if vertices[i].IsActive:
                triangle = TPolygon(vertices[i].Previous.Point, vertices[i].Point, vertices[i].Next.Point)
                OutputPolygons.append(triangle)
                break

        return True

    @staticmethod
    def ConvexPartition_HM(InputPolygon: TPolygon, OutputPolygons: List[TPolygon]) -> bool:
        if len(InputPolygon.Points) < 3:
            return False

        triangles: List[TPolygon] = []

        # already convex?
        num_reflex = 0
        for i11 in range(len(InputPolygon.Points)):
            i12 = len(InputPolygon.Points) - 1 if i11 == 0 else i11 - 1
            i13 = 0 if i11 == (len(InputPolygon.Points) - 1) else i11 + 1
            if TPartition.IsReflex(InputPolygon.Points[i12], InputPolygon.Points[i11], InputPolygon.Points[i13]):
                num_reflex = 1
                break

        if num_reflex == 0:
            OutputPolygons.append(InputPolygon)
            return True

        if not TPartition.Triangulate_EC(InputPolygon, triangles):
            return False

        triangle_index1 = 0
        while triangle_index1 < len(triangles):
            polygon1 = triangles[triangle_index1]
            polygon2: Optional[TPolygon] = None

            i11 = 0
            while i11 < len(polygon1.Points):
                d1 = polygon1.Points[i11]
                i12 = (i11 + 1) % len(polygon1.Points)
                d2 = polygon1.Points[i12]
                is_diagonal = False
                triangle_index2 = triangle_index1

                while triangle_index2 < len(triangles):
                    if triangle_index1 != triangle_index2:
                        polygon2 = triangles[triangle_index2]
                        for i21 in range(len(polygon2.Points)):
                            if (d2.X != polygon2.Points[i21].X) or (d2.Y != polygon2.Points[i21].Y):
                                continue
                            i22 = (i21 + 1) % len(polygon2.Points)
                            if (d1.X != polygon2.Points[i22].X) or (d1.Y != polygon2.Points[i22].Y):
                                continue
                            is_diagonal = True
                            break
                        if is_diagonal:
                            break
                    triangle_index2 += 1

                if not is_diagonal:
                    i11 += 1
                    continue

                p2 = polygon1.Points[i11]
                i13 = len(polygon1.Points) - 1 if i11 == 0 else i11 - 1
                p1 = polygon1.Points[i13]
                i23 = 0 if i22 == (len(polygon2.Points) - 1) else i22 + 1
                p3 = polygon2.Points[i23]
                if not TPartition.IsConvex(p1, p2, p3):
                    i11 += 1
                    continue

                p2 = polygon1.Points[i12]
                i13 = 0 if i12 == (len(polygon1.Points) - 1) else i12 + 1
                p3 = polygon1.Points[i13]
                i23 = len(polygon2.Points) - 1 if i21 == 0 else i21 - 1
                p1 = polygon2.Points[i23]
                if not TPartition.IsConvex(p1, p2, p3):
                    i11 += 1
                    continue

                new_polygon = TPolygon()
                j = i12
                while j != i11:
                    new_polygon.Points.append(polygon1.Points[j])
                    j = (j + 1) % len(polygon1.Points)

                j = i22
                while j != i21:
                    new_polygon.Points.append(polygon2.Points[j])
                    j = (j + 1) % len(polygon2.Points)

                triangles.pop(triangle_index2)
                triangles[triangle_index1] = new_polygon
                polygon1 = new_polygon
                i11 = -1

                i11 += 1

            triangle_index1 += 1

        for polygon in triangles:
            OutputPolygons.append(polygon)

        return True


# --------------------------------------------------
# Map helpers
# --------------------------------------------------

class TMapSector3D:
    def __init__(self, ControlLinedef: TMapLinedef, ControlSector: TMapSector):
        self.ControlLinedef = ControlLinedef
        self.ControlSector = ControlSector
        self.SectorTag = ControlLinedef.Arg0


class TGridList:
    class TGridPage:
        def __init__(self):
            self.Values = [0] * 1024
            self.Chains = [0] * 1024

    def __init__(self):
        self.GridPages: List[Optional[TGridList.TGridPage]] = [None] * 1024
        self.GridChains = [-1] * 65536
        self.GridCount = 0

    def Clear(self):
        self.GridCount = 0
        for i in range(65536):
            self.GridChains[i] = -1

    def Add(self, GridX: int, GridY: int, Value: int):
        page_number = self.GridCount >> 10
        page_index = self.GridCount & 1023
        if page_index == 0:
            self.GridPages[page_number] = TGridList.TGridPage()

        page = self.GridPages[page_number]
        page.Values[page_index] = Value
        page.Chains[page_index] = -1

        chain_number = (GridY << 8) + GridX
        last_chain = self.GridChains[chain_number]

        if self.GridChains[chain_number] >= 0:
            while True:
                page_number = last_chain >> 10
                page_index = last_chain & 1023
                next_chain = self.GridPages[page_number].Chains[page_index]
                if next_chain < 0:
                    break
                last_chain = next_chain
            self.GridPages[page_number].Chains[page_index] = self.GridCount
        else:
            self.GridChains[chain_number] = self.GridCount

        self.GridCount += 1

    def FillOneCell(self, GridX: int, GridY: int, Result: Dict[int, int]):
        chain_number = (GridY << 8) + GridX
        last_chain = self.GridChains[chain_number]

        while last_chain >= 0:
            page_number = last_chain >> 10
            page_index = last_chain & 1023
            value = self.GridPages[page_number].Values[page_index]
            if value not in Result:
                Result[value] = value
            last_chain = self.GridPages[page_number].Chains[page_index]

    def Fill(self, MinX: int, MaxX: int, MinY: int, MaxY: int, Result: Dict[int, int]):
        for y in range(MinY, MaxY + 1):
            for x in range(MinX, MaxX + 1):
                self.FillOneCell(x, y, Result)


# --------------------------------------------------
# Compatibility wrapper
# --------------------------------------------------

class TNavMeshSettings:
    def __init__(self, ActorHeight: int, ActorRadius: int):
        self.ActorHeight = int(ActorHeight)
        self.ActorRadius = int(ActorRadius)


# --------------------------------------------------
# Main navmesh builder
# --------------------------------------------------

class TNavMesh:
    GridOffset = 32768

    def __init__(self, MapDefinition: Optional[TMapDefinition] = None):
        self.MapDefinition = MapDefinition
        self.ActorHeight = 0
        self.ActorRadius = 0
        self.MapSectors3D: List[TMapSector3D] = []
        self.SectorLines: Dict[int, List[int]] = {}
        self.GridLinedef = TGridList()
        self.GridNavMeshLine = TGridList()
        self.NavMeshLines: List[TNavMeshLine] = []
        self.NavMeshPolygons: List[TNavMeshPolygon] = []
        self.Messages: List[str] = []
        self.Cells = [[[]]]
        self.OffsetCellX = 0
        self.OffsetCellY = 0
        self.NumCellX = 0
        self.NumCellY = 0

    # compatibility aliases for your GUI
    @property
    def Lines(self):
        return self.NavMeshLines

    @property
    def Polygons(self):
        return self.NavMeshPolygons

    def GetText(self):
        return str(self)

    def GetMessages(self):
        return "\n".join(self.Messages)

    # --------------------------------------------------

    @staticmethod
    def PointInsidePolygon(Polygon: TPolygon, Point: TPoint) -> bool:
        if len(Polygon.Points) < 3:
            return False

        result = False
        p1 = Polygon.Points[0]
        for i in range(len(Polygon.Points)):
            p2 = Polygon.Points[(i + 1) % len(Polygon.Points)]
            if Point.Y > min(p1.Y, p2.Y):
                if Point.Y <= max(p1.Y, p2.Y):
                    if Point.X <= max(p1.X, p2.X):
                        intersection_x = (Point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X
                        if (p1.X == p2.X) or (Point.X <= intersection_x):
                            result = not result
            p1 = p2
        return result

    @staticmethod
    def PolygonInsidePolygon(OuterPolygon: TPolygon, TestPolygon: TPolygon) -> bool:
        if len(OuterPolygon.Points) < 3 or len(TestPolygon.Points) < 3:
            return False
        return (
            TNavMesh.PointInsidePolygon(OuterPolygon, TestPolygon.Points[0]) and
            TNavMesh.PointInsidePolygon(OuterPolygon, TestPolygon.Points[1])
        )

    def CheckLinedef3D(self, MapLinedef: TMapLinedef) -> bool:
        result = False
        if self.MapDefinition.MapNamespace == TMapNamespace.MapNamespaceZDoom:
            if (MapLinedef.Special == 160) and ((((MapLinedef.Arg1 & 0x0003) == 0x0001) or ((MapLinedef.Arg1 & 0x0003) == 0x0002))):
                result = True
        return result

    @staticmethod
    def GetGridExtent(X1: int, Y1: int, X2: int, Y2: int):
        if X1 > X2:
            max_x = X1
            min_x = X2
        else:
            max_x = X2
            min_x = X1

        if Y2 > Y1:
            max_y = Y2
            min_y = Y1
        else:
            max_y = Y1
            min_y = Y2

        min_x = (min_x + TNavMesh.GridOffset) >> 8
        max_x = (max_x + TNavMesh.GridOffset) >> 8
        min_y = (min_y + TNavMesh.GridOffset) >> 8
        max_y = (max_y + TNavMesh.GridOffset) >> 8

        return min_x, max_x, min_y, max_y

    def PreProcessMapData(self):
        self.SectorLines.clear()
        self.GridLinedef.Clear()
        self.GridNavMeshLine.Clear()
        self.MapSectors3D.clear()

        for map_sector in self.MapDefinition.MapSector:
            if map_sector.Special == 10:
                map_sector.Ignored = True
            if map_sector.Ignored:
                continue
            self.SectorLines[map_sector.Index] = []

        for map_linedef in self.MapDefinition.MapLinedef:
            if map_linedef.SideFront >= 0:
                side_sector = self.MapDefinition.MapSidedef[map_linedef.SideFront].Sector
                if side_sector in self.SectorLines:
                    self.SectorLines[side_sector].append(map_linedef.Index)
                if self.CheckLinedef3D(map_linedef):
                    self.MapSectors3D.append(TMapSector3D(map_linedef, self.MapDefinition.MapSector[side_sector]))

            if map_linedef.SideBack >= 0:
                side_sector = self.MapDefinition.MapSidedef[map_linedef.SideBack].Sector
                if side_sector in self.SectorLines:
                    self.SectorLines[side_sector].append(map_linedef.Index)

            min_x, max_x, min_y, max_y = self.GetGridExtent(
                self.MapDefinition.MapVertex[map_linedef.V1].X,
                self.MapDefinition.MapVertex[map_linedef.V1].Y,
                self.MapDefinition.MapVertex[map_linedef.V2].X,
                self.MapDefinition.MapVertex[map_linedef.V2].Y,
            )
            for y in range(min_y, max_y + 1):
                for x in range(min_x, max_x + 1):
                    self.GridLinedef.Add(x, y, map_linedef.Index)

    def ProcessPolygonMesh(self, Polygon: TNavMeshPolygon):
        lines: Dict[int, int] = {}

        for line in Polygon.Lines:
            lines.clear()
            min_x, max_x, min_y, max_y = self.GetGridExtent(line.A.X, line.A.Y, line.B.X, line.B.Y)
            self.GridLinedef.Fill(min_x, max_x, min_y, max_y, lines)

            not_found = True
            map_linedef_index = 0
            sorted_keys = sorted(lines.keys())

            while not_found and map_linedef_index < len(sorted_keys):
                v1 = self.MapDefinition.MapVertex[self.MapDefinition.MapLinedef[sorted_keys[map_linedef_index]].V1]
                v2 = self.MapDefinition.MapVertex[self.MapDefinition.MapLinedef[sorted_keys[map_linedef_index]].V2]
                if (
                    (line.A.X == v1.X and line.A.Y == v1.Y and line.B.X == v2.X and line.B.Y == v2.Y)
                    or
                    (line.A.X == v2.X and line.A.Y == v2.Y and line.B.X == v1.X and line.B.Y == v1.Y)
                ):
                    not_found = False
                else:
                    map_linedef_index += 1

            if not not_found:
                line.MapLinedef = sorted_keys[map_linedef_index]

            lines.clear()
            self.GridNavMeshLine.Fill(min_x, max_x, min_y, max_y, lines)

            for index in range(len(lines)):
                i = sorted(lines.keys())[index]
                current_polygon_index = 0
                current_polygon_start_line = 0
                polygon_not_found = True

                while polygon_not_found and current_polygon_index < len(self.NavMeshPolygons):
                    next_polygon_start = current_polygon_start_line + self.NavMeshPolygons[current_polygon_index].LineCount
                    if next_polygon_start > i:
                        polygon_not_found = False
                    else:
                        current_polygon_start_line = next_polygon_start
                        current_polygon_index += 1

                if current_polygon_index >= len(self.NavMeshPolygons):
                    continue

                if (
                    (line.A == self.NavMeshLines[i].A and line.B == self.NavMeshLines[i].B)
                    or
                    (line.A == self.NavMeshLines[i].B and line.B == self.NavMeshLines[i].A)
                ):
                    line_is_portal = True

                    if abs(Polygon.HeightFloor - self.NavMeshPolygons[current_polygon_index].HeightFloor) > 24:
                        line_is_portal = False

                    if min(Polygon.HeightCeiling, self.NavMeshPolygons[current_polygon_index].HeightCeiling) - \
                       max(Polygon.HeightFloor, self.NavMeshPolygons[current_polygon_index].HeightFloor) < self.ActorHeight:
                        line_is_portal = False

                    if line.MapLinedef >= 0:
                        map_linedef = self.MapDefinition.MapLinedef[line.MapLinedef]
                        if map_linedef.Blocking or map_linedef.SideFront < 0 or map_linedef.SideBack < 0 or map_linedef.Ignored:
                            line_is_portal = False

                    if line_is_portal:
                        line.Portal = current_polygon_index
                        self.NavMeshLines[i].Portal = len(self.NavMeshPolygons)

        Polygon.LineFirst = len(self.NavMeshLines)
        Polygon.LineCount = len(Polygon.Lines)
        self.NavMeshPolygons.append(Polygon)

        for line in Polygon.Lines:
            min_x, max_x, min_y, max_y = self.GetGridExtent(line.A.X, line.A.Y, line.B.X, line.B.Y)
            for y in range(min_y, max_y + 1):
                for x in range(min_x, max_x + 1):
                    self.GridNavMeshLine.Add(x, y, len(self.NavMeshLines))
            self.NavMeshLines.append(line)

    def ProcessPolygons(self, MapSector: TMapSector, Polygon: TPolygon, Holes: List[TPolygon],
                        PolyhedronCeilings: List[int], PolyhedronFloors: List[int]):
        polygons: List[TPolygon] = []

        if TPartition.RemoveHoles(Polygon, Holes):
            if TPartition.ConvexPartition_HM(Polygon, polygons):
                for split_polygon in polygons:
                    for polyhedron_index in range(len(PolyhedronCeilings)):
                        navmesh_polygon = TNavMeshPolygon()
                        navmesh_polygon.HeightFloor = PolyhedronFloors[polyhedron_index]
                        navmesh_polygon.HeightCeiling = PolyhedronCeilings[polyhedron_index]
                        navmesh_polygon.MapSector = MapSector.Index

                        for i in range(len(split_polygon.Points)):
                            j = (i + 1) % len(split_polygon.Points)
                            navmesh_polygon.Lines.append(
                                TNavMeshLine(
                                    TNavMeshPoint(split_polygon.Points[i].X, split_polygon.Points[i].Y),
                                    TNavMeshPoint(split_polygon.Points[j].X, split_polygon.Points[j].Y),
                                )
                            )

                        self.ProcessPolygonMesh(navmesh_polygon)
            else:
                self.Messages.append(f"Map SECTOR # {MapSector.Index} could not be processed.")
        else:
            self.Messages.append(f"Map SECTOR # {MapSector.Index} has holes, that could not be processed.")

    def ProcessSector(self, MapSector: TMapSector, VertexBegin: List[int], VertexEnd: List[int],
                      PolyhedronCeilings: List[int], PolyhedronFloors: List[int]) -> bool:
        # direct mechanical port of the current C# algorithm

        if len(VertexBegin) < 3:
            return False

        polygons: List[TPolygon] = []
        max_stack = 128

        points = [0] * (len(VertexBegin) * 2)
        lines_a = [0] * len(VertexBegin)
        lines_b = [0] * len(VertexBegin)

        point_count = 0
        line_count = 0

        for i in range(len(VertexBegin)):
            not_found = True
            j = 0
            while not_found and j < point_count:
                if points[j] == VertexBegin[i]:
                    not_found = False
                else:
                    j += 1
            if not_found:
                points[point_count] = VertexBegin[i]
                lines_a[line_count] = point_count
                point_count += 1
            else:
                lines_a[line_count] = j

            not_found = True
            j = 0
            while not_found and j < point_count:
                if points[j] == VertexEnd[i]:
                    not_found = False
                else:
                    j += 1
            if not_found:
                points[point_count] = VertexEnd[i]
                lines_b[line_count] = point_count
                point_count += 1
            else:
                lines_b[line_count] = j

            line_count += 1

        point_cardinality = [0] * (point_count * 2)
        open_points = [True] * point_count
        open_lines = [True] * line_count

        point_queue = [0] * point_count
        mesh_stack_points = [[0 for _ in range(point_count * 4)] for _ in range(max_stack)]
        mesh_stack_count = [0] * max_stack

        outer_perimeter_points = [0] * point_count
        inner_chord_points = [0] * point_count
        outer_chord_points = [0] * point_count

        def get_x(vertex_index: int) -> int:
            return self.MapDefinition.MapVertex[vertex_index].X

        def get_y(vertex_index: int) -> int:
            return self.MapDefinition.MapVertex[vertex_index].Y

        next_line = 0
        infinite_loop_line = -1

        while True:
            first_point = -1
            for i in range(point_count):
                if open_points[i]:
                    if first_point < 0:
                        first_point = i
                    else:
                        if (get_x(points[i]) < get_x(points[first_point])) or (
                            get_x(points[i]) == get_x(points[first_point]) and get_y(points[i]) < get_y(points[first_point])
                        ):
                            first_point = i

            point_queue[0] = first_point
            point_queue_head = 0
            point_queue_tail = 1

            while point_queue_tail > point_queue_head:
                current_point = point_queue[point_queue_tail - 1]

                if point_queue_tail == 1:
                    next_difference = 0
                    next_point = -1
                    for i in range(line_count):
                        if open_lines[i] and ((lines_a[i] == current_point) or (lines_b[i] == current_point)):
                            test_point = lines_b[i] if lines_a[i] == current_point else lines_a[i]
                            test_difference = get_y(points[test_point]) - get_y(points[current_point])

                            if next_point < 0:
                                next_point = test_point
                                next_difference = test_difference
                                next_line = i
                            else:
                                if test_difference < next_difference:
                                    next_point = test_point
                                    next_difference = test_difference
                                    next_line = i
                else:
                    previous_point = point_queue[point_queue_tail - 2]
                    previous_angle = math.atan2(
                        get_y(points[current_point]) - get_y(points[previous_point]),
                        get_x(points[current_point]) - get_x(points[previous_point]),
                    )
                    next_point = -1
                    next_angle = 0.0

                    for i in range(line_count):
                        if open_lines[i] and ((lines_a[i] == current_point) or (lines_b[i] == current_point)):
                            test_point = lines_b[i] if lines_a[i] == current_point else lines_a[i]
                            if test_point != previous_point:
                                test_angle = math.atan2(
                                    get_y(points[test_point]) - get_y(points[current_point]),
                                    get_x(points[test_point]) - get_x(points[current_point]),
                                ) - previous_angle
                                if test_angle < -math.pi:
                                    test_angle += math.pi * 2
                                if next_point < 0:
                                    next_point = test_point
                                    next_angle = test_angle
                                    next_line = i
                                else:
                                    if test_angle < next_angle:
                                        next_point = test_point
                                        next_angle = test_angle
                                        next_line = i

                if next_point >= 0:
                    not_found = True
                    outer_perimeter_point_start = point_queue_head

                    while not_found and outer_perimeter_point_start < point_queue_tail:
                        if point_queue[outer_perimeter_point_start] == next_point:
                            not_found = False
                        else:
                            outer_perimeter_point_start += 1

                    if not_found:
                        point_queue[point_queue_tail] = next_point
                        point_queue_tail += 1
                    else:
                        outer_perimeter_point_count = 0
                        for i in range(outer_perimeter_point_start, point_queue_tail):
                            outer_perimeter_points[outer_perimeter_point_count] = point_queue[i]
                            outer_perimeter_point_count += 1

                        mesh_stack_tos = 0
                        mesh_stack_count[mesh_stack_tos] = outer_perimeter_point_count
                        for i in range(outer_perimeter_point_count):
                            mesh_stack_points[mesh_stack_tos][i] = outer_perimeter_points[i]

                        while mesh_stack_tos >= 0:
                            for i in range(mesh_stack_count[mesh_stack_tos]):
                                point_cardinality[i] = 2

                            for i in range(mesh_stack_count[mesh_stack_tos]):
                                current_point = mesh_stack_points[mesh_stack_tos][i]
                                next_point = mesh_stack_points[mesh_stack_tos][(i + 1) % mesh_stack_count[mesh_stack_tos]]
                                previous_point = mesh_stack_points[mesh_stack_tos][(i + mesh_stack_count[mesh_stack_tos] - 1) % mesh_stack_count[mesh_stack_tos]]

                                for j in range(line_count):
                                    if (
                                        ((lines_a[j] == current_point) and (lines_b[j] != next_point) and (lines_b[j] != previous_point))
                                        or
                                        ((lines_b[j] == current_point) and (lines_a[j] != next_point) and (lines_a[j] != previous_point))
                                    ):
                                        test_point = lines_b[j] if lines_a[j] == current_point else lines_a[j]
                                        if TPartition.InCone(
                                            get_x(points[previous_point]), get_y(points[previous_point]),
                                            get_x(points[current_point]), get_y(points[current_point]),
                                            get_x(points[next_point]), get_y(points[next_point]),
                                            get_x(points[test_point]), get_y(points[test_point]),
                                        ):
                                            point_cardinality[i] += 1

                            inner_chord_point_loop = -1
                            first_point = -1

                            for i in range(mesh_stack_count[mesh_stack_tos]):
                                if point_cardinality[i] > 2:
                                    test_point = mesh_stack_points[mesh_stack_tos][i]
                                    if first_point < 0:
                                        first_point = test_point
                                    else:
                                        if (get_x(points[test_point]) < get_x(points[first_point])) or (
                                            get_x(points[test_point]) == get_x(points[first_point]) and
                                            get_y(points[test_point]) < get_y(points[first_point])
                                        ):
                                            first_point = test_point

                            if first_point >= 0:
                                inner_chord_points[0] = first_point
                                inner_chord_point_count = 1
                                has_more_iterations = True

                                while has_more_iterations:
                                    current_point = inner_chord_points[inner_chord_point_count - 1]

                                    if inner_chord_point_count == 1:
                                        not_found = True
                                        i = 0
                                        while not_found and i < mesh_stack_count[mesh_stack_tos]:
                                            if mesh_stack_points[mesh_stack_tos][i] == first_point:
                                                not_found = False
                                            else:
                                                i += 1

                                        other_point = mesh_stack_points[mesh_stack_tos][(i + 1) % mesh_stack_count[mesh_stack_tos]]
                                        previous_point = mesh_stack_points[mesh_stack_tos][(i + mesh_stack_count[mesh_stack_tos] - 1) % mesh_stack_count[mesh_stack_tos]]

                                        previous_angle = math.atan2(
                                            get_y(points[current_point]) - get_y(points[previous_point]),
                                            get_x(points[current_point]) - get_x(points[previous_point]),
                                        )
                                        next_angle = math.atan2(
                                            get_y(points[other_point]) - get_y(points[current_point]),
                                            get_x(points[other_point]) - get_x(points[current_point]),
                                        )
                                        next_point = -1

                                        for i in range(line_count):
                                            if open_lines[i] and ((lines_a[i] == current_point) or (lines_b[i] == current_point)):
                                                test_point = lines_b[i] if lines_a[i] == current_point else lines_a[i]
                                                if (
                                                    (test_point != previous_point)
                                                    and (test_point != other_point)
                                                    and TPartition.InCone(
                                                        get_x(points[previous_point]), get_y(points[previous_point]),
                                                        get_x(points[current_point]), get_y(points[current_point]),
                                                        get_x(points[other_point]), get_y(points[other_point]),
                                                        get_x(points[test_point]), get_y(points[test_point]),
                                                    )
                                                ):
                                                    test_angle = math.atan2(
                                                        get_y(points[test_point]) - get_y(points[first_point]),
                                                        get_x(points[test_point]) - get_x(points[first_point]),
                                                    ) - previous_angle
                                                    if test_angle < -math.pi:
                                                        test_angle += math.pi * 2

                                                    if next_point < 0:
                                                        next_point = test_point
                                                        next_angle = test_angle
                                                        next_line = i
                                                    else:
                                                        if test_angle < next_angle:
                                                            next_point = test_point
                                                            next_angle = test_angle
                                                            next_line = i
                                    else:
                                        previous_point = inner_chord_points[inner_chord_point_count - 2]
                                        previous_angle = math.atan2(
                                            get_y(points[current_point]) - get_y(points[previous_point]),
                                            get_x(points[current_point]) - get_x(points[previous_point]),
                                        )
                                        next_point = -1
                                        next_angle = 0.0

                                        for i in range(line_count):
                                            if open_lines[i] and ((lines_a[i] == current_point) or (lines_b[i] == current_point)):
                                                test_point = lines_b[i] if lines_a[i] == current_point else lines_a[i]
                                                if test_point != previous_point:
                                                    test_angle = math.atan2(
                                                        get_y(points[test_point]) - get_y(points[first_point]),
                                                        get_x(points[test_point]) - get_x(points[first_point]),
                                                    ) - previous_angle
                                                    if test_angle < -math.pi:
                                                        test_angle += math.pi * 2

                                                    if next_point < 0:
                                                        next_point = test_point
                                                        next_angle = test_angle
                                                        next_line = i
                                                    else:
                                                        if test_angle < next_angle:
                                                            next_point = test_point
                                                            next_angle = test_angle
                                                            next_line = i

                                    if next_point >= 0:
                                        open_lines[next_line] = False
                                        inner_chord_points[inner_chord_point_count] = next_point
                                        inner_chord_point_count += 1

                                        not_found = True
                                        i = 0
                                        while not_found and i < mesh_stack_count[mesh_stack_tos]:
                                            if mesh_stack_points[mesh_stack_tos][i] == next_point:
                                                not_found = False
                                            else:
                                                i += 1

                                        if not_found:
                                            not_found = True
                                            i = 1
                                            while not_found and i < inner_chord_point_count - 1:
                                                if inner_chord_points[i] == next_point:
                                                    not_found = False
                                                else:
                                                    i += 1
                                            if not_found:
                                                open_points[next_point] = False
                                            else:
                                                inner_chord_point_loop = i
                                                has_more_iterations = False
                                        else:
                                            has_more_iterations = False
                                    else:
                                        if next_line == infinite_loop_line:
                                            return False
                                        open_lines[next_line] = False
                                        infinite_loop_line = next_line
                                        all_points_visited = True
                                        for i in range(line_count):
                                            if open_lines[i] and ((lines_a[i] == current_point) or (lines_b[i] == current_point)):
                                                all_points_visited = False
                                        if all_points_visited:
                                            open_points[current_point] = False
                                            inner_chord_point_count += 1
                                        if inner_chord_point_count == 0:
                                            has_more_iterations = False

                                if inner_chord_point_count > 0:
                                    if inner_chord_point_loop > 0:
                                        return False
                                    else:
                                        chord_begin = 0
                                        while mesh_stack_points[mesh_stack_tos][chord_begin] != inner_chord_points[0]:
                                            chord_begin += 1

                                        previous_point = mesh_stack_points[mesh_stack_tos][(chord_begin + mesh_stack_count[mesh_stack_tos] - 1) % mesh_stack_count[mesh_stack_tos]]

                                        chord_end = mesh_stack_count[mesh_stack_tos] - 1
                                        while mesh_stack_points[mesh_stack_tos][chord_end] != inner_chord_points[inner_chord_point_count - 1]:
                                            chord_end -= 1

                                        next_point = mesh_stack_points[mesh_stack_tos][(chord_end + 1) % mesh_stack_count[mesh_stack_tos]]

                                        outer_chord_point_count = chord_end - chord_begin + 1
                                        if outer_chord_point_count < 0:
                                            outer_chord_point_count = outer_chord_point_count + mesh_stack_count[mesh_stack_tos]
                                            for i in range(chord_begin, chord_end + mesh_stack_count[mesh_stack_tos] + 1):
                                                outer_chord_points[i - chord_begin] = mesh_stack_points[mesh_stack_tos][i % mesh_stack_count[mesh_stack_tos]]
                                        else:
                                            for i in range(chord_begin, chord_end + 1):
                                                outer_chord_points[i - chord_begin] = mesh_stack_points[mesh_stack_tos][i]

                                        if inner_chord_point_count > outer_chord_point_count:
                                            mesh_stack_count[mesh_stack_tos] = mesh_stack_count[mesh_stack_tos] - outer_chord_point_count + inner_chord_point_count
                                            for i in range(mesh_stack_count[mesh_stack_tos] - 1, chord_end - 1, -1):
                                                src = i
                                                dst = i + inner_chord_point_count - outer_chord_point_count
                                                if 0 <= src < len(mesh_stack_points[mesh_stack_tos]) and 0 <= dst < len(mesh_stack_points[mesh_stack_tos]):
                                                    mesh_stack_points[mesh_stack_tos][dst] = mesh_stack_points[mesh_stack_tos][src]

                                        elif inner_chord_point_count < outer_chord_point_count:
                                            mesh_stack_count[mesh_stack_tos] = mesh_stack_count[mesh_stack_tos] - outer_chord_point_count + inner_chord_point_count
                                            if chord_begin > chord_end:
                                                for i in range(chord_end, chord_begin + 1):
                                                    mesh_stack_points[mesh_stack_tos][i - chord_end] = mesh_stack_points[mesh_stack_tos][i]
                                            else:
                                                for i in range(chord_begin + 1, mesh_stack_count[mesh_stack_tos]):
                                                    mesh_stack_points[mesh_stack_tos][i] = mesh_stack_points[mesh_stack_tos][i + outer_chord_point_count - inner_chord_point_count]

                                        for i in range(1, inner_chord_point_count - 1):
                                            mesh_stack_points[mesh_stack_tos][(chord_begin + i - 1) % mesh_stack_count[mesh_stack_tos]] = inner_chord_points[i]

                                        mesh_stack_tos += 1
                                        mesh_stack_count[mesh_stack_tos] = inner_chord_point_count + outer_chord_point_count - 2

                                        test_angle = math.atan2(
                                            get_y(points[next_point]) - get_y(points[inner_chord_points[inner_chord_point_count - 1]]),
                                            get_x(points[next_point]) - get_x(points[inner_chord_points[inner_chord_point_count - 1]]),
                                        )

                                        if test_angle < 0:
                                            for i in range(outer_chord_point_count):
                                                mesh_stack_points[mesh_stack_tos][i] = outer_chord_points[i]
                                            for i in range(1, inner_chord_point_count - 1):
                                                mesh_stack_points[mesh_stack_tos][outer_chord_point_count + i - 1] = inner_chord_points[inner_chord_point_count - i - 1]
                                        else:
                                            for i in range(inner_chord_point_count):
                                                mesh_stack_points[mesh_stack_tos][i] = inner_chord_points[inner_chord_point_count - i - 1]
                                            for i in range(1, outer_chord_point_count - 1):
                                                mesh_stack_points[mesh_stack_tos][inner_chord_point_count + i - 1] = outer_chord_points[i]
                                else:
                                    return False
                            else:
                                polygon = TPolygon()
                                for i in range(mesh_stack_count[mesh_stack_tos]):
                                    polygon.Points.append(TPoint(
                                        get_x(points[mesh_stack_points[mesh_stack_tos][i]]),
                                        get_y(points[mesh_stack_points[mesh_stack_tos][i]])
                                    ))
                                polygons.append(polygon)
                                mesh_stack_tos -= 1

                        for j in range(outer_perimeter_point_start, point_queue_tail + 1):
                            for i in range(line_count):
                                if (
                                    ((lines_a[i] == outer_perimeter_points[(j + outer_perimeter_point_count - 1) % outer_perimeter_point_count]) and
                                     (lines_b[i] == outer_perimeter_points[j % outer_perimeter_point_count]))
                                    or
                                    ((lines_a[i] == outer_perimeter_points[j % outer_perimeter_point_count]) and
                                     (lines_b[i] == outer_perimeter_points[(j + outer_perimeter_point_count - 1) % outer_perimeter_point_count]))
                                ):
                                    open_lines[i] = False

                        for j in range(outer_perimeter_point_count):
                            all_points_visited = True
                            for i in range(line_count):
                                if open_lines[i] and ((lines_a[i] == outer_perimeter_points[j]) or (lines_b[i] == outer_perimeter_points[j])):
                                    all_points_visited = False
                            if all_points_visited:
                                open_points[outer_perimeter_points[j]] = False

                        point_queue_tail = outer_perimeter_point_start
                else:
                    if next_line == infinite_loop_line:
                        return False
                    open_lines[next_line] = False
                    infinite_loop_line = next_line

                    all_points_visited = True
                    for i in range(line_count):
                        if open_lines[i] and ((lines_a[i] == current_point) or (lines_b[i] == current_point)):
                            all_points_visited = False
                            break
                    if all_points_visited:
                        open_points[current_point] = False
                        point_queue_tail -= 1

            all_points_visited = True
            for i in range(point_count):
                if open_points[i]:
                    all_points_visited = False
                    break

            if all_points_visited:
                break

        if len(polygons) == 0:
            return False

        holes: List[TPolygon] = []
        if len(polygons) == 1:
            polygons[0].Hole = False
            self.ProcessPolygons(MapSector, polygons[0], holes, PolyhedronCeilings, PolyhedronFloors)
        else:
            polygon_depth_level = [0] * len(polygons)
            max_depth_level = 0

            for i in range(len(polygons)):
                polygon_depth_level[i] = 0

            for i in range(len(polygons)):
                for j in range(len(polygons)):
                    if i != j:
                        if self.PolygonInsidePolygon(polygons[i], polygons[j]):
                            polygon_depth_level[j] += 1
                            if polygon_depth_level[j] > max_depth_level:
                                max_depth_level = polygon_depth_level[j]

            for depth_level in range(0, max_depth_level + 1, 2):
                for i in range(len(polygons)):
                    if polygon_depth_level[i] == depth_level:
                        holes.clear()
                        for j in range(len(polygons)):
                            if (polygon_depth_level[j] == (polygon_depth_level[i] + 1)) and self.PolygonInsidePolygon(polygons[i], polygons[j]):
                                polygons[j].Hole = True
                                holes.append(polygons[j])
                        polygons[i].Hole = False
                        self.ProcessPolygons(MapSector, polygons[i], holes, PolyhedronCeilings, PolyhedronFloors)

        return True

    def ProcessMapData(self):
        plane_ceilings: List[int] = []
        plane_floors: List[int] = []
        polyhedron_ceilings: List[int] = []
        polyhedron_floors: List[int] = []
        vertex_begin: List[int] = []
        vertex_end: List[int] = []

        for map_sector in self.MapDefinition.MapSector:
            if map_sector.Ignored:
                continue
            if (map_sector.HeightCeiling - map_sector.HeightFloor) < self.ActorHeight:
                continue

            plane_ceilings.clear()
            plane_floors.clear()

            for map_sector3d in self.MapSectors3D:
                if map_sector.ID == map_sector3d.SectorTag:
                    plane_ceilings.append(map_sector3d.ControlSector.HeightCeiling)
                    plane_floors.append(map_sector3d.ControlSector.HeightFloor)

            for i in range(len(plane_ceilings) - 1):
                for j in range(i + 1, len(plane_ceilings)):
                    if plane_ceilings[i] > plane_ceilings[j]:
                        plane_ceilings[j], plane_ceilings[i] = plane_ceilings[i], plane_ceilings[j]
                        plane_floors[j], plane_floors[i] = plane_floors[i], plane_floors[j]

            polyhedron_ceilings.clear()
            polyhedron_floors.clear()

            if len(plane_ceilings) > 0:
                start_floor = map_sector.HeightFloor
                for i in range(len(plane_floors)):
                    if (plane_floors[i] - start_floor) > self.ActorHeight:
                        polyhedron_ceilings.append(plane_floors[i])
                        polyhedron_floors.append(start_floor)
                    start_floor = plane_ceilings[i]
                if (map_sector.HeightCeiling - start_floor) > self.ActorHeight:
                    polyhedron_ceilings.append(map_sector.HeightCeiling)
                    polyhedron_floors.append(start_floor)
            else:
                polyhedron_ceilings.append(map_sector.HeightCeiling)
                polyhedron_floors.append(map_sector.HeightFloor)

            vertex_begin.clear()
            vertex_end.clear()

            if map_sector.Index in self.SectorLines:
                cached_lines = self.SectorLines[map_sector.Index]
                for map_linedef_index in cached_lines:
                    front_sector = -1
                    if self.MapDefinition.MapLinedef[map_linedef_index].SideFront >= 0:
                        front_sector = self.MapDefinition.MapSidedef[self.MapDefinition.MapLinedef[map_linedef_index].SideFront].Sector

                    back_sector = -2
                    if self.MapDefinition.MapLinedef[map_linedef_index].SideBack >= 0:
                        back_sector = self.MapDefinition.MapSidedef[self.MapDefinition.MapLinedef[map_linedef_index].SideBack].Sector

                    if front_sector == map_sector.Index:
                        vertex_begin_index = self.MapDefinition.MapLinedef[map_linedef_index].V1
                        vertex_end_index = self.MapDefinition.MapLinedef[map_linedef_index].V2
                    else:
                        vertex_begin_index = self.MapDefinition.MapLinedef[map_linedef_index].V2
                        vertex_end_index = self.MapDefinition.MapLinedef[map_linedef_index].V1

                    not_found = True
                    i = 0
                    while not_found and i < len(vertex_begin):
                        if (vertex_begin[i] == vertex_begin_index) and (vertex_end[i] == vertex_end_index):
                            not_found = False
                        else:
                            i += 1

                    if not_found:
                        # IMPORTANT: keep oriented endpoints, not raw V1/V2
                        vertex_begin.append(vertex_begin_index)
                        vertex_end.append(vertex_end_index)
            else:
                continue

            if len(vertex_begin) < 3:
                self.Messages.append(f"Map SECTOR # {map_sector.Index} has less than 3 LINEDEFs.")
                continue

            if not self.ProcessSector(map_sector, vertex_begin, vertex_end, polyhedron_ceilings, polyhedron_floors):
                self.Messages.append(f"Map SECTOR # {map_sector.Index} could not be split into regions.")

    def ProcessCells(self):
        max_x = -2**31
        max_y = -2**31
        min_x = 2**31 - 1
        min_y = 2**31 - 1

        for line in self.NavMeshLines:
            max_x = max(max_x, line.A.X, line.B.X)
            max_y = max(max_y, line.A.Y, line.B.Y)
            min_x = min(min_x, line.A.X, line.B.X)
            min_y = min(min_y, line.A.Y, line.B.Y)

        self.OffsetCellX = (min_x + self.GridOffset) >> 8
        self.OffsetCellY = (min_y + self.GridOffset) >> 8

        last_cell_x = (max_x + self.GridOffset) >> 8
        last_cell_y = (max_y + self.GridOffset) >> 8

        self.NumCellX = last_cell_x - self.OffsetCellX + 1
        self.NumCellY = last_cell_y - self.OffsetCellY + 1

        self.Cells = [[[] for _ in range(self.NumCellX)] for _ in range(self.NumCellY)]

        for polygon_index in range(len(self.NavMeshPolygons)):
            max_x = -2**31
            max_y = -2**31
            min_x = 2**31 - 1
            min_y = 2**31 - 1

            for line in self.NavMeshPolygons[polygon_index].Lines:
                max_x = max(max_x, line.A.X, line.B.X)
                max_y = max(max_y, line.A.Y, line.B.Y)
                min_x = min(min_x, line.A.X, line.B.X)
                min_y = min(min_y, line.A.Y, line.B.Y)

            min_x = (min_x + self.GridOffset) >> 8
            min_y = (min_y + self.GridOffset) >> 8
            max_x = (max_x + self.GridOffset) >> 8
            max_y = (max_y + self.GridOffset) >> 8

            for y in range(min_y, max_y + 1):
                for x in range(min_x, max_x + 1):
                    self.Cells[y - self.OffsetCellY][x - self.OffsetCellX].append(polygon_index)

    def Build(self, ActorHeight, ActorRadius=None):
        """
        Supports both:
            Build(int_height, int_radius)
        and:
            Build(TNavMeshSettings(height, radius), map_definition)
        """
        if isinstance(ActorHeight, TNavMeshSettings):
            settings = ActorHeight
            self.ActorHeight = settings.ActorHeight
            self.ActorRadius = settings.ActorRadius
            self.MapDefinition = ActorRadius
        else:
            self.ActorHeight = int(ActorHeight)
            self.ActorRadius = int(ActorRadius)

        if self.MapDefinition is None:
            raise ValueError("MapDefinition is required")

        self.MapSectors3D.clear()
        self.SectorLines.clear()
        self.GridLinedef.Clear()
        self.GridNavMeshLine.Clear()
        self.NavMeshLines.clear()
        self.NavMeshPolygons.clear()
        self.Messages.clear()
        self.Cells = [[[]]]
        self.OffsetCellX = 0
        self.OffsetCellY = 0
        self.NumCellX = 0
        self.NumCellY = 0

        self.PreProcessMapData()
        self.ProcessMapData()
        self.ProcessCells()

    def __str__(self):
        lines = []
        lines.append("# ZDOOMNAVMESH")
        lines.append("")
        lines.append("# lines")
        for i in range(len(self.NavMeshLines)):
            line = self.NavMeshLines[i]
            lines.append(f"l {line.A.X} {line.A.Y} {line.B.X} {line.B.Y} {line.Portal} {line.MapLinedef}")
        lines.append("")
        lines.append("# polygons")
        for i in range(len(self.NavMeshPolygons)):
            polygon = self.NavMeshPolygons[i]
            lines.append(f"p {polygon.HeightFloor} {polygon.HeightCeiling} {polygon.LineFirst} {polygon.LineCount} {polygon.MapSector}")
        lines.append("")
        lines.append("# cells space partitioning")
        lines.append(f"o {self.OffsetCellX} {self.OffsetCellY} {self.NumCellX} {self.NumCellY}")
        for y in range(self.NumCellY):
            for x in range(self.NumCellX):
                if len(self.Cells[y][x]) > 0:
                    parts = [f"c {y * self.NumCellX + x}"]
                    for polygon_index in self.Cells[y][x]:
                        parts.append(str(polygon_index))
                    lines.append(" ".join(parts))
        return "\n".join(lines)