"""
- Author: Pollazzon Stefano (ported)
- Project: ZDoom Navmesh builder
- This module contains the code for managing
- both WAD and PK3 file formats, and supporting
- Doom, Hexen and UDMF map formats
"""

import io
import os
import struct
import zipfile

from Structures import (
    TMapVertex,
    TMapLinedef,
    TMapSidedef,
    TMapSector,
    TMapThing,
    TMapDefinition,
    TMapNamespace,
)
from Parsers import TUdmfParser


class TArchiveEntry:
    def __init__(self, EntryName, EntryOffset, EntrySize):
        self.EntryName = EntryName
        self.EntryOffset = int(EntryOffset)
        self.EntrySize = int(EntrySize)


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
        "ZNODES",
    ]

    def __init__(self):
        self.MapDefinitions = []
        self.FileName = ""
        self.FileSpec = ""
        self.FileSize = 0
        self.LastUpdate = 0.0

    # --------------------------------------------------
    # Helpers
    # --------------------------------------------------

    def AddMap(self, MapDefinition):
        self.MapDefinitions.append(MapDefinition)

    def SortMap(self):
        self.MapDefinitions.sort(key=lambda m: m.MapName)

    # --------------------------------------------------
    # Text maps (UDMF only)
    # --------------------------------------------------

    def LoadFromText(self, FileSpec):
        if not os.path.exists(FileSpec):
            return False

        self.FileSpec = FileSpec
        self.FileName = os.path.basename(FileSpec)
        self.FileSize = os.path.getsize(FileSpec)
        self.LastUpdate = os.path.getmtime(FileSpec)

        with open(FileSpec, "r", encoding="utf-8") as reader:
            text_map = reader.read()

        udmf_parser = TUdmfParser(text_map)
        map_definition = TMapDefinition(os.path.splitext(os.path.basename(FileSpec))[0])
        udmf_parser.ParseMap(map_definition)
        self.AddMap(map_definition)
        return True

    # --------------------------------------------------
    # Binary lump readers
    # --------------------------------------------------

    def LoadVertexes(self, MapNamespaceValue, LumpContent, MapVertex):
        MapVertex.clear()
        count = len(LumpContent) // 4
        offset = 0
        for _ in range(count):
            x, y = struct.unpack_from("<hh", LumpContent, offset)
            offset += 4
            MapVertex.append(TMapVertex(x, y))

    def LoadLinedefs(self, MapNamespaceValue, LumpContent, MapLinedef):
        MapLinedef.clear()
        offset = 0

        if MapNamespaceValue == TMapNamespace.MapNamespaceDoom:
            count = len(LumpContent) // 14
            for _ in range(count):
                v1, v2, flags, special, arg0, side_front, side_back = struct.unpack_from(
                    "<hhhhhhh", LumpContent, offset
                )
                offset += 14

                linedef = TMapLinedef(
                    V1=v1,
                    V2=v2,
                    ID=0,
                    Blocking=((flags & 0x0003) != 0),
                    TwoSided=((flags & 0x0004) != 0),
                    SideFront=side_front,
                    SideBack=side_back,
                    Special=special,
                    Arg0=arg0,
                    Arg1=0,
                    Arg2=0,
                    Arg3=0,
                    Arg4=0,
                    MonsterUse=False,
                    RepeatableSpecial=False,
                    Ignored=False,
                )
                MapLinedef.append(linedef)
        else:
            count = len(LumpContent) // 16
            for _ in range(count):
                v1, v2, flags = struct.unpack_from("<hhh", LumpContent, offset)
                offset += 6

                special, arg0, arg1, arg2, arg3, arg4 = struct.unpack_from("<BBBBBB", LumpContent, offset)
                offset += 6

                side_front, side_back = struct.unpack_from("<hh", LumpContent, offset)
                offset += 4

                linedef = TMapLinedef(
                    V1=v1,
                    V2=v2,
                    ID=0,
                    Blocking=((flags & 0x0003) != 0),
                    TwoSided=((flags & 0x0004) != 0),
                    SideFront=side_front,
                    SideBack=side_back,
                    Special=special,
                    Arg0=arg0,
                    Arg1=arg1,
                    Arg2=arg2,
                    Arg3=arg3,
                    Arg4=arg4,
                    MonsterUse=False,
                    RepeatableSpecial=False,
                    Ignored=False,
                )
                MapLinedef.append(linedef)

    def LoadSidedefs(self, MapNamespaceValue, LumpContent, MapSidedef):
        MapSidedef.clear()
        count = len(LumpContent) // 30
        offset = 0

        for _ in range(count):
            # skip texture / offset data, sector index is last 2 bytes
            sector = struct.unpack_from("<h", LumpContent, offset + 28)[0]
            offset += 30
            MapSidedef.append(TMapSidedef(sector))

    def LoadSectors(self, MapNamespaceValue, LumpContent, MapSector):
        MapSector.clear()
        count = len(LumpContent) // 26
        offset = 0

        for _ in range(count):
            height_floor, height_ceiling = struct.unpack_from("<hh", LumpContent, offset)
            offset += 4

            # skip 18 bytes of floor/ceiling textures + light + type-dependent data before special/tag
            offset += 18

            special, sector_id = struct.unpack_from("<hh", LumpContent, offset)
            offset += 4

            MapSector.append(TMapSector(height_floor, height_ceiling, sector_id, special, False))

    def LoadThings(self, MapNamespaceValue, LumpContent, MapThing):
        MapThing.clear()
        offset = 0

        if MapNamespaceValue == TMapNamespace.MapNamespaceDoom:
            count = len(LumpContent) // 10
            for _ in range(count):
                x, y = struct.unpack_from("<hh", LumpContent, offset)
                offset += 4

                offset += 2  # angle
                thing_type = struct.unpack_from("<h", LumpContent, offset)[0]
                offset += 2
                offset += 2  # flags

                MapThing.append(TMapThing(x, y, 0, thing_type, 0, 0, 0, 0, 0, 0, 0))
        else:
            count = len(LumpContent) // 20
            for _ in range(count):
                thing_id, x, y = struct.unpack_from("<hhh", LumpContent, offset)
                offset += 6

                offset += 4  # height + angle
                thing_type = struct.unpack_from("<h", LumpContent, offset)[0]
                offset += 2

                offset += 2  # flags
                special, arg0, arg1, arg2, arg3, arg4 = struct.unpack_from("<BBBBBB", LumpContent, offset)
                offset += 6

                MapThing.append(
                    TMapThing(x, y, 0, thing_type, thing_id, special, arg0, arg1, arg2, arg3, arg4)
                )

    # --------------------------------------------------
    # WAD loading
    # --------------------------------------------------

    def LoadFromWadStream(self, ArchiveFileStream):
        map_lump_names = list(self.MapLumpNamesDefs)

        reader = ArchiveFileStream
        reader.seek(0)

        header_signature = reader.read(4)
        header_signature_text = header_signature.decode("ascii", errors="ignore")

        if header_signature_text not in ("IWAD", "PWAD"):
            return False

        header_number_of_entries = struct.unpack("<i", reader.read(4))[0]
        header_directory_offset = struct.unpack("<i", reader.read(4))[0]

        reader.seek(header_directory_offset)
        archive_entries = []

        for _ in range(header_number_of_entries):
            entry_offset, entry_size = struct.unpack("<ii", reader.read(8))
            entry_name_raw = reader.read(8)
            entry_name_text = entry_name_raw.decode("ascii", errors="ignore").replace("\x00", "")
            archive_entries.append(TArchiveEntry(entry_name_text, entry_offset, entry_size))

        map_found = False
        namespace = TMapNamespace.MapNamespaceDoom
        text_map = None

        map_vertex = []
        map_linedef = []
        map_sidedef = []
        map_sector = []
        map_thing = []

        entry_number = header_number_of_entries - 1
        while entry_number >= 0:
            entry = archive_entries[entry_number]

            if entry.EntryName in map_lump_names:
                map_found = True

                if entry.EntryName == "BEHAVIOR":
                    namespace = TMapNamespace.MapNamespaceHexen

                if entry.EntryName == "TEXTMAP":
                    saved_position = reader.tell()
                    reader.seek(entry.EntryOffset)
                    lump_content = reader.read(entry.EntrySize)
                    text_map = lump_content.decode("utf-8", errors="ignore")
                    reader.seek(saved_position)

                if entry.EntryName == "VERTEXES":
                    saved_position = reader.tell()
                    reader.seek(entry.EntryOffset)
                    lump_content = reader.read(entry.EntrySize)
                    self.LoadVertexes(namespace, lump_content, map_vertex)
                    reader.seek(saved_position)

                if entry.EntryName == "LINEDEFS":
                    saved_position = reader.tell()
                    reader.seek(entry.EntryOffset)
                    lump_content = reader.read(entry.EntrySize)
                    self.LoadLinedefs(namespace, lump_content, map_linedef)
                    reader.seek(saved_position)

                if entry.EntryName == "SIDEDEFS":
                    saved_position = reader.tell()
                    reader.seek(entry.EntryOffset)
                    lump_content = reader.read(entry.EntrySize)
                    self.LoadSidedefs(namespace, lump_content, map_sidedef)
                    reader.seek(saved_position)

                if entry.EntryName == "SECTORS":
                    saved_position = reader.tell()
                    reader.seek(entry.EntryOffset)
                    lump_content = reader.read(entry.EntrySize)
                    self.LoadSectors(namespace, lump_content, map_sector)
                    reader.seek(saved_position)

                if entry.EntryName == "THINGS":
                    saved_position = reader.tell()
                    reader.seek(entry.EntryOffset)
                    lump_content = reader.read(entry.EntrySize)
                    self.LoadThings(namespace, lump_content, map_thing)
                    reader.seek(saved_position)

            else:
                if map_found:
                    map_name = entry.EntryName
                    map_definition = TMapDefinition(map_name)

                    if text_map:
                        udmf_parser = TUdmfParser(text_map)
                        udmf_parser.ParseMap(map_definition)
                        map_definition.MapNamespace = udmf_parser.MapNamespace
                        self.AddMap(map_definition)
                        text_map = None
                    else:
                        map_definition.MapNamespace = namespace
                        for item in map_vertex:
                            map_definition.AddMapVertex(item)
                        for item in map_linedef:
                            map_definition.AddMapLinedef(item)
                        for item in map_sidedef:
                            map_definition.AddMapSidedef(item)
                        for item in map_sector:
                            map_definition.AddMapSector(item)
                        for item in map_thing:
                            map_definition.AddMapThing(item)
                        self.AddMap(map_definition)

                    map_found = False
                    namespace = TMapNamespace.MapNamespaceDoom
                    map_vertex = []
                    map_linedef = []
                    map_sidedef = []
                    map_sector = []
                    map_thing = []

            entry_number -= 1

        self.SortMap()
        return True

    def LoadFromWad(self, FileSpec):
        if not os.path.exists(FileSpec):
            return False

        self.FileSpec = FileSpec
        self.FileName = os.path.basename(FileSpec)
        self.FileSize = os.path.getsize(FileSpec)
        self.LastUpdate = os.path.getmtime(FileSpec)

        with open(FileSpec, "rb") as archive_file_stream:
            return self.LoadFromWadStream(archive_file_stream)

    # --------------------------------------------------
    # ZIP / PK3 loading
    # --------------------------------------------------

    def LoadFromZip(self, FileSpec):
        if not os.path.exists(FileSpec):
            return False

        self.FileSpec = FileSpec
        self.FileName = os.path.basename(FileSpec)
        self.FileSize = os.path.getsize(FileSpec)
        self.LastUpdate = os.path.getmtime(FileSpec)

        with zipfile.ZipFile(FileSpec, "r") as archive:
            for archive_entry in archive.infolist():
                full_name = archive_entry.filename

                if full_name.upper().startswith("MAPS/") and full_name.upper().endswith(".WAD"):
                    with archive.open(archive_entry, "r") as map_stream:
                        seekable_map_stream = io.BytesIO(map_stream.read())
                        seekable_map_stream.seek(0)
                        self.LoadFromWadStream(seekable_map_stream)

        return True

    # --------------------------------------------------
    # Dispatch
    # --------------------------------------------------

    def Load(self, FileSpec):
        if not os.path.exists(FileSpec):
            return False

        file_extension = os.path.splitext(FileSpec)[1].upper()

        # reset maps on each explicit Load call
        self.MapDefinitions = []

        if file_extension == ".TXT":
            return self.LoadFromText(FileSpec)

        if file_extension == ".WAD":
            return self.LoadFromWad(FileSpec)

        if file_extension in (".PK3", ".ZIP"):
            return self.LoadFromZip(FileSpec)

        return False