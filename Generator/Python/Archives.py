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
		self.MapDefinitions.sort()

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
					self.MapDefinitions.Add(MapDefinition)

	def	LoadVertexes(self, MapNamespace, LumpContent, MapVertex):
		MapVertex.clear()
		LumpBinaryReader = BinaryIO(LumpContent)
		count = int(LumpContent.length / 4)
		for index in range(count, 0, -1):
			X = LumpBinaryReader.read_int16()
			Y = LumpBinaryReader.read_int16()
			MapVertex.Add(TMapVertex(X, Y))
			
	def	LoadLinedefs(self, MapNamespace, LumpContent, MapLinedef):
		MapLinedef.clear()
		LumpBinaryReader = BinaryIO(LumpContent)
		if MapNamespace == TMapNamespace.MapNamespaceDoom:
			count = int(LumpContent.length / 14)
			for index in range(count, 0, -1):
				V1 = LumpBinaryReader.read_int16()
				V2 = LumpBinaryReader.read_int16()
				Flags = LumpBinaryReader.read_int16()
				Special = LumpBinaryReader.read_int16()
				Arg0  = LumpBinaryReader.read_int16()
				SideFront = LumpBinaryReader.read_int16()
				SideBack = LumpBinaryReader.read_int16()
				Blocking = (Flags & 0x0003) != 0
				TwoSided = (Flags & 0x0004) != 0
				MapLinedef.Add(TMapLinedef(V1, V2, 0, Blocking, TwoSided, SideFront, SideBack, Special, Arg0, 0, 0, 0, 0, False, False, False))
		else:
			count = int(LumpContent.length / 16)
			for index in range(count, 0, -1):
				V1 = LumpBinaryReader.read_int16()
				V2 = LumpBinaryReader.read_int16()
				Flags = LumpBinaryReader.read_int16()
				Special = LumpBinaryReader.read(1)
				Arg0  = LumpBinaryReader.read(1)
				Arg1  = LumpBinaryReader.read(1)
				Arg2  = LumpBinaryReader.read(1)
				Arg3  = LumpBinaryReader.read(1)
				Arg4  = LumpBinaryReader.read(1)
				SideFront = LumpBinaryReader.read_int16()
				SideBack = LumpBinaryReader.read_int16()
				Blocking = (Flags & 3) > 0
				TwoSided = (Flags & 4) > 0
				MapLinedef.Add(TMapLinedef(V1, V2, 0, Blocking, TwoSided, SideFront, SideBack, Special, Arg0, Arg1, Arg2, Arg3, Arg4, False, False, False))
				
	def	LoadSidedefs(self, MapNamespace, LumpContent, MapSidedef):
		MapSidedef.clear()
		LumpBinaryReader = BinaryIO(LumpContent)
		count = int(LumpContent.length / 30)
		for index in range(count, 0, -1):
			Ignored = LumpBinaryReader.read(28)
			Sector = LumpBinaryReader.read_int16()
			MapSidedef.Add(TMapSidedef(Sector))
			
	def	LoadSectors(self, MapNamespace, LumpContent, MapSector):
		MapSector.clear()
		LumpBinaryReader = BinaryIO(LumpContent)
		count = int(LumpContent.length / 26)
		for index in range(count, 0, -1):
			HeightFloor = LumpBinaryReader.read_int16()
			HeightCeiling = LumpBinaryReader.read_int16()
			Ignored = LumpBinaryReader.read(18)
			Special = LumpBinaryReader.read_int16()
			ID = LumpBinaryReader.read_int16()
			MapSector.Add(TMapSector(HeightFloor, HeightCeiling, ID, Special, False))
			
	def	LoadThings(self, MapNamespace, LumpContent, MapThing):
		MapThing.clear()
		LumpBinaryReader = BinaryIO(LumpContent)
		if MapNamespace == TMapNamespace.MapNamespaceDoom:
			count = int(LumpContent.length / 10)
			for index in range(count, 0, -1):
				X = LumpBinaryReader.read_int16()
				Y = LumpBinaryReader.read_int16()
				Ignored = LumpBinaryReader.read(2)
				ThingType = LumpBinaryReader.read_int16()
				Ignored = LumpBinaryReader.read(2)
				MapThing.Add(TMapThing(X, Y, 0, ThingType, 0, 0, 0, 0, 0 ,0, 0))
		else:
			count = int(LumpContent.length / 20)
			for index in range(count, 0, -1):
				ID = LumpBinaryReader.read_int16()
				X = LumpBinaryReader.read_int16()
				Y = LumpBinaryReader.read_int16()
				Ignored = LumpBinaryReader.read(4)
				ThingType = LumpBinaryReader.read_int16()
				Ignored = LumpBinaryReader.read(2)
				Special = LumpBinaryReader.read(1)
				Arg0 = LumpBinaryReader.read(1)
				Arg1 = LumpBinaryReader.read(1)
				Arg2 = LumpBinaryReader.read(1)
				Arg3 = LumpBinaryReader.read(1)
				Arg4 = LumpBinaryReader.read(1)
				MapThing.Add(TMapThing(X, Y, 0, ThingType, ID, Special, Arg0, Arg1, Arg2, Arg3, Arg4))
	
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
								LoadVertexes(self, Namespace, MemoryLump, MapVertex)
								Reader.seek(SavedPosition)
							if ArchiveEntries[EntryNumber].EntryName == "LINEDEFS":
								SavedPosition = Reader.tell()
								Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
								LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
								MemoryLump = io.BytesIO(LumpContent)
								LoadLinedefs(self, Namespace, MemoryLump, MapVertex)
								Reader.seek(SavedPosition)
							if ArchiveEntries[EntryNumber].EntryName == "SIDEDEFS":
								SavedPosition = Reader.tell()
								Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
								LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
								MemoryLump = io.BytesIO(LumpContent)
								LoadSidedefs(self, Namespace, MemoryLump, MapVertex)
								Reader.seek(SavedPosition)
							if ArchiveEntries[EntryNumber].EntryName == "SECTORS":
								SavedPosition = Reader.tell()
								Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
								LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
								MemoryLump = io.BytesIO(LumpContent)
								LoadSectors(self, Namespace, MemoryLump, MapVertex)
								Reader.seek(SavedPosition)
							if ArchiveEntries[EntryNumber].EntryName == "THINGS":
								SavedPosition = Reader.tell()
								Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
								LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
								MemoryLump = io.BytesIO(LumpContent)
								LoadThings(self, Namespace, MemoryLump, MapVertex)
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
									MapDefinition.MapVertex = [] + MapVertex
									MapDefinition.MapLinedef = [] + MapLinedef
									MapDefinition.MapSidedef = [] + MapSidedef
									MapDefinition.MapSector = [] + MapSector
									MapDefinition.MapThing = [] + MapThing
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
				with io.BufferedReader(ArchiveFileStream) as Reader:
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
						if ArchiveEntries[EntryNumber].EntryName in MapLumpNamesDefs:
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
								LoadVertexes(self, Namespace, MemoryLump, MapVertex)
								Reader.seek(SavedPosition)
							if ArchiveEntries[EntryNumber].EntryName == "LINEDEFS":
								SavedPosition = Reader.tell()
								Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
								LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
								MemoryLump = io.BytesIO(LumpContent)
								LoadLinedefs(self, Namespace, MemoryLump, MapVertex)
								Reader.seek(SavedPosition)
							if ArchiveEntries[EntryNumber].EntryName == "SIDEDEFS":
								SavedPosition = Reader.tell()
								Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
								LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
								MemoryLump = io.BytesIO(LumpContent)
								LoadSidedefs(self, Namespace, MemoryLump, MapVertex)
								Reader.seek(SavedPosition)
							if ArchiveEntries[EntryNumber].EntryName == "SECTORS":
								SavedPosition = Reader.tell()
								Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
								LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
								MemoryLump = io.BytesIO(LumpContent)
								LoadSectors(self, Namespace, MemoryLump, MapVertex)
								Reader.seek(SavedPosition)
							if ArchiveEntries[EntryNumber].EntryName == "THINGS":
								SavedPosition = Reader.tell()
								Reader.seek(ArchiveEntries[EntryNumber].EntryOffset)
								LumpContent = Reader.read(ArchiveEntries[EntryNumber].EntrySize)
								MemoryLump = io.BytesIO(LumpContent)
								LoadThings(self, Namespace, MemoryLump, MapVertex)
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
									MapDefinition.MapVertex = [] + MapVertex
									MapDefinition.MapLinedef = [] + MapLinedef
									MapDefinition.MapSidedef = [] + MapSidedef
									MapDefinition.MapSector = [] + MapSector
									MapDefinition.MapThing = [] + MapThing
								self.AddMap(MapDefinition);
								MapFound = False
								Namespace = TMapNamespace.MapNamespaceDoom
						EntryNumber -= 1

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
