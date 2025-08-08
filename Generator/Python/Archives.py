"""
 * Author: Pollazzon Stefano
 * Project: ZDoom Navmesh builder
 * This module contains the code for managing 
 * both WAD and PK3 file formats, and supporting
 * Doom, Hexen and UDMF map formats
"""
import io
import os
import struct
import typing
import zipfile
from Parsers import *

class TArchiveEntry:
	def __init__(self, EntryName, EntryOffset, EntrySize):
		self.EntryName = EntryName
		self.EntryOffset = EntryOffset
		self.EntrySize = EntrySize

class TArchive:
	MapLumpNamesDefs = [
		"BEHAVIOR",
		"BLOCKMAP",
		"DIALOGUE",
		"ENDMAP",
		"LINEDEFS",
		"NODES",
		"REJECT",
		"SCRIPTS",
		"SECTORS",
		"SEGS",
		"SIDEDEFS",
		"SSECTORS",
		"TEXTMAP",
		"THINGS",
		"VERTEXES",
		"ZNODES"
	]
	def __init__(self):
		self.MapDefinitions = []
		self.FileName = ""
		self.FileSpec = ""
		self.FileSize = 0
		self.LastUpdate = 0

	def AddMap(self, MapDefinition):
		self.MapDefinitions.append(MapDefinition)

	def SortMap(self):
		self.MapDefinitions.sort(key = lambda MapDefinition: MapDefinition.MapName)

	def LoadFromText(self, FileSpec):
		if os.path.exists(FileSpec):
			self.FileSpec = FileSpec
			self.FileName = os.path.basename(FileSpec)
			self.FileSize = os.path.getsize(FileSpec)
			self.LastUpdate = os.path.getmtime(FileSpec)
			with open(FileSpec, 'rb') as ArchiveFileStream:
				with open(ArchiveFileStream, 'r') as Reader:
					MapDefinition = TMapDefinition(os.path.splitext(FileName)[0])
					UdmfParser = TUdmfParser(Reader.read())
					UdmfParser.ParseMap(MapDefinition)
					self.MapDefinitions.append(MapDefinition)

	def	LoadVertexes(self, MapNamespace, LumpContent, MapVertex):
		MapVertex.clear()
		count = int(len(LumpContent.read()) / 4)
		LumpContent.seek(0)
		for index in range(count, 0, -1):
			X = struct.unpack('<h', LumpContent.read(2))[0]
			Y = struct.unpack('<h', LumpContent.read(2))[0]
			MapVertex.append(TMapVertex(X, Y))
			
	def	LoadLinedefs(self, MapNamespace, LumpContent, MapLinedef):
		MapLinedef.clear()
		if MapNamespace == TMapNamespace.MapNamespaceDoom:
			count = int(len(LumpContent.read()) / 14)
			LumpContent.seek(0)
			for index in range(count, 0, -1):
				V1 = struct.unpack('<H', LumpContent.read(2))[0]
				V2 = struct.unpack('<H', LumpContent.read(2))[0]
				Flags = struct.unpack('<H', LumpContent.read(2))[0]
				Special = struct.unpack('<H', LumpContent.read(2))[0]
				Arg0  = struct.unpack('<H', LumpContent.read(2))[0]
				SideFront = struct.unpack('<h', LumpContent.read(2))[0]
				SideBack = struct.unpack('<h', LumpContent.read(2))[0]
				Blocking = (Flags & 0x0003) != 0
				TwoSided = (Flags & 0x0004) != 0
				MapLinedef.append(TMapLinedef(V1, V2, 0, Blocking, TwoSided, SideFront, SideBack, Special, Arg0, 0, 0, 0, 0, False, False, False))
		else:
			count = int(len(LumpContent.read()) / 16)
			LumpContent.seek(0)
			for index in range(count, 0, -1):
				V1 = struct.unpack('<H', LumpContent.read(2))[0]
				V2 = struct.unpack('<H', LumpContent.read(2))[0]
				Flags = struct.unpack('<H', LumpContent.read(2))[0]
				Special = LumpContent.read(1)
				Arg0  = LumpContent.read(1)
				Arg1  = LumpContent.read(1)
				Arg2  = LumpContent.read(1)
				Arg3  = LumpContent.read(1)
				Arg4  = LumpContent.read(1)
				SideFront = struct.unpack('<H', LumpContent.read(2))[0]
				SideBack = struct.unpack('<H', LumpContent.read(2))[0]
				Blocking = (Flags & 3) > 0
				TwoSided = (Flags & 4) > 0
				MapLinedef.append(TMapLinedef(V1, V2, 0, Blocking, TwoSided, SideFront, SideBack, Special, Arg0, Arg1, Arg2, Arg3, Arg4, False, False, False))
				
	def	LoadSidedefs(self, MapNamespace, LumpContent, MapSidedef):
		MapSidedef.clear()
		count = int(len(LumpContent.read()) / 30)
		LumpContent.seek(0)
		for index in range(count, 0, -1):
			Ignored = LumpContent.read(28)
			Sector = struct.unpack('<H', LumpContent.read(2))[0]
			MapSidedef.append(TMapSidedef(Sector))
			
	def	LoadSectors(self, MapNamespace, LumpContent, MapSector):
		MapSector.clear()
		count = int(len(LumpContent.read()) / 26)
		LumpContent.seek(0)
		for index in range(count, 0, -1):
			HeightFloor = struct.unpack('<H', LumpContent.read(2))[0]
			HeightCeiling = struct.unpack('<H', LumpContent.read(2))[0]
			Ignored = LumpContent.read(18)
			Special = struct.unpack('<H', LumpContent.read(2))[0]
			ID = struct.unpack('<H', LumpContent.read(2))[0]
			MapSector.append(TMapSector(HeightFloor, HeightCeiling, ID, Special, False))
			
	def	LoadThings(self, MapNamespace, LumpContent, MapThing):
		MapThing.clear()
		if MapNamespace == TMapNamespace.MapNamespaceDoom:
			count = int(len(LumpContent.read()) / 10)
			LumpContent.seek(0)
			for index in range(count, 0, -1):
				X = struct.unpack('<h', LumpContent.read(2))[0]
				Y = struct.unpack('<h', LumpContent.read(2))[0]
				Ignored = LumpContent.read(2)
				ThingType = struct.unpack('<H', LumpContent.read(2))[0]
				Ignored = LumpContent.read(2)
				MapThing.append(TMapThing(X, Y, 0, ThingType, 0, 0, 0, 0, 0 ,0, 0))
		else:
			count = int(len(LumpContent.read()) / 20)
			LumpContent.seek(0)
			for index in range(count, 0, -1):
				ID = struct.unpack('<H', LumpContent.read(2))[0]
				X = struct.unpack('<H', LumpContent.read(2))[0]
				Y = struct.unpack('<H', LumpContent.read(2))[0]
				Ignored = LumpContent.read(4)
				ThingType = struct.unpack('<H', LumpContent.read(2))[0]
				Ignored = LumpContent.read(2)
				Special = LumpContent.read(1)
				Arg0 = LumpContent.read(1)
				Arg1 = LumpContent.read(1)
				Arg2 = LumpContent.read(1)
				Arg3 = LumpContent.read(1)
				Arg4 = LumpContent.read(1)
				MapThing.append(TMapThing(X, Y, 0, ThingType, ID, Special, Arg0, Arg1, Arg2, Arg3, Arg4))
	
	def LoadFromBinary(self, Stream):
		with io.BufferedReader(Stream) as Reader:
			HeaderSignature = Reader.read(4)
			HeaderSignatureText = HeaderSignature.decode('ascii')
			if HeaderSignatureText not in ["IWAD", "PWAD"]:
				return False
			HeaderNumberOfEntries = struct.unpack('<I', Reader.read(4))[0]
			HeaderDirectoryOffset = struct.unpack('<I', Reader.read(4))[0]
			Reader.seek(HeaderDirectoryOffset)
			ArchiveEntries = []
			for _ in range(HeaderNumberOfEntries):
				EntryOffset = struct.unpack('<I', Reader.read(4))[0]
				EntrySize = struct.unpack('<I', Reader.read(4))[0]
				EntryName = Reader.read(8).decode('ascii').rstrip('\0')
				ArchiveEntries.append(TArchiveEntry(EntryName, EntryOffset, EntrySize))
			MapFound = False
			Namespace = TMapNamespace.MapNamespaceDoom
			TextMap = None
			MapVertex = []
			MapLinedef = []
			MapSidedef = []
			MapSector = []
			MapThing = []
			EntryNumber = HeaderNumberOfEntries - 1
			while EntryNumber >= 0:
				if ArchiveEntries[EntryNumber].EntryName in self.MapLumpNamesDefs:
					MapFound = True
					if ArchiveEntries[EntryNumber].EntryName == "BEHAVIOR":
						Namespace = TMapNamespace.MapNamespaceHexen
					if ArchiveEntries[EntryNumber].EntryName == "TEXTMAP":
						SavedPosition = Reader.tell()
						Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
						LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
						TextMap = LumpContent.decode('utf-8')
						Reader.seek(SavedPosition)
					if ArchiveEntries[EntryNumber].EntryName == "VERTEXES":
						SavedPosition = Reader.tell()
						Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
						LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
						MemoryLump = io.BytesIO(LumpContent)
						self.LoadVertexes(Namespace, MemoryLump, MapVertex)
						Reader.seek(SavedPosition)
					if ArchiveEntries[EntryNumber].EntryName == "LINEDEFS":
						SavedPosition = Reader.tell()
						Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
						LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
						MemoryLump = io.BytesIO(LumpContent)
						self.LoadLinedefs(Namespace, MemoryLump, MapLinedef)
						Reader.seek(SavedPosition)
					if ArchiveEntries[EntryNumber].EntryName == "SIDEDEFS":
						SavedPosition = Reader.tell()
						Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
						LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
						MemoryLump = io.BytesIO(LumpContent)
						self.LoadSidedefs(Namespace, MemoryLump, MapSidedef)
						Reader.seek(SavedPosition)
					if ArchiveEntries[EntryNumber].EntryName == "SECTORS":
						SavedPosition = Reader.tell()
						Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
						LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
						MemoryLump = io.BytesIO(LumpContent)
						self.LoadSectors(Namespace, MemoryLump, MapSector)
						Reader.seek(SavedPosition)
					if ArchiveEntries[EntryNumber].EntryName == "THINGS":
						SavedPosition = Reader.tell()
						Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
						LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
						MemoryLump = io.BytesIO(LumpContent)
						self.LoadThings(Namespace, MemoryLump, MapThing)
						Reader.seek(SavedPosition)
				else:
					if MapFound:
						MapName = ArchiveEntries[EntryNumber].EntryName
						MapDefinition = TMapDefinition(MapName)
						if TextMap:
							UdmfParser = TUdmfParser(TextMap)
							UdmfParser.ParseMap(MapDefinition)
							MapDefinition.MapNamespace = UdmfParser.MapNamespace;
							TextMap = None
						else:
							for Item in MapVertex:
								MapDefinition.AddMapVertex(Item)
							for Item in MapLinedef:
								MapDefinition.AddMapLinedef(Item)
							for Item in MapSidedef:
								MapDefinition.AddMapSidedef(Item)
							for Item in MapSector:
								MapDefinition.AddMapSector(Item)
							for Item in MapThing:
								MapDefinition.AddMapThing(Item)
						self.AddMap(MapDefinition);
						MapFound = False
						Namespace = TMapNamespace.MapNamespaceDoom
				EntryNumber -= 1

	def LoadFromWad(self, FileSpec):
		if os.path.exists(FileSpec):
			self.FileSpec = FileSpec
			self.FileName = os.path.basename(FileSpec)
			self.FileSize = os.path.getsize(FileSpec)
			self.LastUpdate = os.path.getmtime(FileSpec)
			with open(FileSpec, 'rb') as ArchiveFileStream:
				self.LoadFromBinary(ArchiveFileStream)
			self.SortMap()
			return True

	def LoadFromZip(self, FileSpec):
		if os.path.exists(FileSpec):
			self.FileSpec = FileSpec
			self.FileName = os.path.basename(FileSpec)
			self.FileSize = os.path.getsize(FileSpec)
			self.LastUpdate = os.path.getmtime(FileSpec)
			with open(FileSpec, 'rb') as ArchiveFileStream:
				with zipfile.ZipFile(ArchiveFileStream) as Archive:
					for ArchiveEntry in Archive.namelist():
						if ArchiveEntry.lower().startswith("maps/") and ArchiveEntry.lower().endswith(".wad"):
							with Archive.open(ArchiveEntry) as MapStream:
								SeekableMapStream = io.BytesIO()
								SeekableMapStream.write(MapStream.read())
								SeekableMapStream.seek(0)
								self.LoadFromBinary(SeekableMapStream)
			self.SortMap()
			return True
		else:
			return False
	
	def Load(self, FileSpec):
		if os.path.exists(FileSpec):
			FileExtension = os.path.splitext(FileSpec)[1][1:]
			if FileExtension.lower() == "txt":
				return self.LoadFromText(FileSpec)
			elif FileExtension.lower() == "wad":
				return self.LoadFromWad(FileSpec)
			elif FileExtension.lower() == "pk3":
				return self.LoadFromZip(FileSpec)
			else:
				return False
		else:
			return False
