/*
 * Author: Pollazzon Stefano
 * Project: ZDoom Navmesh builder
 * This module contains the code for managing 
 * both WAD and PK3 file formats, and supporting
 * Doom, Hexen and UDMF map formats
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NavmeshBuilder;

internal class TArchiveEntry : Object
{
    internal int EntryOffset { get; set; }
    internal int EntrySize { get; set; }
    internal string EntryName { get; set; }
    internal TArchiveEntry(string EntryName, int EntryOffset, int EntrySize)
    {
        this.EntryName = EntryName;
        this.EntryOffset = EntryOffset;
        this.EntrySize = EntrySize;
    }
}
/// <summary>
/// Class <c>TArchive</c> represents an archive, that can contain multiple maps.
/// Stores all maps in a sorted list; read files from .TXT (UDMF only), .WAD or .PK3 / .ZIP.
/// </summary>
public class TArchive : Object
{
    private List<TMapDefinition> FMapDefinitions;
    public string? FileName { get; private set; }
    public string? FileSpec { get; private set; }
    public long FileSize { get; private set; }
    public DateTime LastUpdate { get; private set; }
    /// <summary>
    /// Property <c>MapDefinitions</c> gives access to the maps in the archive.
    /// </summary>
    public List<TMapDefinition> MapDefinitions { get => FMapDefinitions; }
    private static readonly string[] MapLumpNamesDefs = new string[] {
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
    };
    public TArchive()
    {
        FMapDefinitions = new List<TMapDefinition>();
    }
    /// <summary>
    /// Function <c>AddMap</c> adds a map.
    /// </summary>
    /// <param name="MapDefinition"><c>MapDefinition</c> is the map to be added.</param>
    public void AddMap(TMapDefinition MapDefinition)
    {
        FMapDefinitions.Add(MapDefinition);
    }
    /// <summary>
    /// Function <c>SortMap</c> sorts the maps in alphabetical order on map names.
    /// </summary>
    /// <param name="MapDefinition"><c>MapDefinition</c> is the map to be added.</param>
    public void SortMap()
    {
        FMapDefinitions.Sort(delegate (TMapDefinition A, TMapDefinition B) { return String.Compare(A.MapName, B.MapName); });
    }
    /// <summary>
    /// Function <c>LoadFromText</c> reads a .TXT file, parsing the UDMF namespace and map entities
    /// </summary>
    /// <param name="FileSpec"><c>FileSpec</c> is the file to be read.</param>
    internal bool LoadFromText(string FileSpec)
    {
        if (File.Exists(FileSpec))
            using (FileStream ArchiveFileStream = File.OpenRead(FileSpec))
            {
                this.FileSpec = FileSpec;
                FileName = Path.GetFileName(FileSpec);
                FileSize = ArchiveFileStream.Length;
                LastUpdate = File.GetLastWriteTime(FileSpec);
                using (StreamReader Reader = new StreamReader(ArchiveFileStream))
                {
                    TUdmfParser UdmfParser = new TUdmfParser(Reader.ReadToEnd());
                    TMapDefinition MapDefinition = new TMapDefinition(Path.GetFileNameWithoutExtension(FileSpec));
                    UdmfParser.ParseMap(MapDefinition);
                    AddMap(MapDefinition);
                }
                return true;
            }
        else
            return false;
    }
    /// <summary>
    /// Function <c>LoadVertexes</c> reads all the VERTEX data in a map.
    /// </summary>
    /// <param name="MapNamespace"><c>MapNamespace</c> is the namespace of the map.</param>
    /// <param name="LumpContent"><c>LumpContent</c> is the binary content of the lump map.</param>
    /// <param name="MapVertex"><c>MapVertex</c> is the list of VERTEX objects of the map, to append the read data.</param>
    internal void LoadVertexes(TMapNamespace MapNamespace, MemoryStream LumpContent, List<TMapVertex> MapVertex)
    {
        MapVertex.Clear();
        BinaryReader LumpBinaryReader = new BinaryReader(LumpContent);
        int Count = Convert.ToInt32(LumpContent.Length / 4);
        for (int I = Count; I > 0; I--)
        {
            TMapVertex Vertex = new TMapVertex(LumpBinaryReader.ReadInt16(), LumpBinaryReader.ReadInt16());
            MapVertex.Add(Vertex);
        }
    }
    /// <summary>
    /// Function <c>LoadLinedefs</c> reads all the LINEDEF data in a map.
    /// </summary>
    /// <param name="MapNamespace"><c>MapNamespace</c> is the namespace of the map.</param>
    /// <param name="LumpContent"><c>LumpContent</c> is the binary content of the lump map.</param>
    /// <param name="MapLinedef"><c>MapLinedef</c> is the list of LINEDEF objects of the map, to append the read data.</param>
    internal void LoadLinedefs(TMapNamespace MapNamespace, MemoryStream LumpContent, List<TMapLinedef> MapLinedef)
    {
        MapLinedef.Clear();
        BinaryReader LumpBinaryReader = new BinaryReader(LumpContent);
        if (MapNamespace == TMapNamespace.MapNamespaceDoom)
        {
            int Count = Convert.ToInt32(LumpContent.Length / 14);
            for (int I = Count; I > 0; I--)
            {
                short V1 = LumpBinaryReader.ReadInt16();
                short V2 = LumpBinaryReader.ReadInt16();
                short Flags = LumpBinaryReader.ReadInt16();
                short Special = LumpBinaryReader.ReadInt16();
                short Arg0 = LumpBinaryReader.ReadInt16();
                short SideFront = LumpBinaryReader.ReadInt16();
                short SideBack = LumpBinaryReader.ReadInt16();
                TMapLinedef Linedef = new TMapLinedef(V1, V2, 0, Flags, SideFront, SideBack, Special, Arg0, 0, 0, 0, 0, false, false, false);
                MapLinedef.Add(Linedef);
            }
        }
        else
        {
            int Count = Convert.ToInt32(LumpContent.Length / 16);
            for (int I = Count; I > 0; I--)
            {
                short V1 = LumpBinaryReader.ReadInt16();
                short V2 = LumpBinaryReader.ReadInt16();
                short Flags = LumpBinaryReader.ReadInt16();
                short Special = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg0 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg1 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg2 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg3 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg4 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short SideFront = LumpBinaryReader.ReadInt16();
                short SideBack = LumpBinaryReader.ReadInt16();
                TMapLinedef Linedef = new TMapLinedef(V1, V2, 0, Flags, SideFront, SideBack, Special, Arg0, Arg1, Arg2, Arg3, Arg4, false, false, false);
                MapLinedef.Add(Linedef);
            }
        }
    }
    /// <summary>
    /// Function <c>LoadSidedefs</c> reads all the SIDEDEF data in a map.
    /// </summary>
    /// <param name="MapNamespace"><c>MapNamespace</c> is the namespace of the map.</param>
    /// <param name="LumpContent"><c>LumpContent</c> is the binary content of the lump map.</param>
    /// <param name="MapSidedef"><c>MapSidedef</c> is the list of SIDEDEF objects of the map, to append the read data.</param>
    internal void LoadSidedefs(TMapNamespace MapNamespace, MemoryStream LumpContent, List<TMapSidedef> MapSidedef)
    {
        MapSidedef.Clear();
        BinaryReader LumpBinaryReader = new BinaryReader(LumpContent);
        int Count = Convert.ToInt32(LumpContent.Length / 30);
        for (int I = Count; I > 0; I--)
        {
            LumpBinaryReader.BaseStream.Seek(28, SeekOrigin.Current);
            TMapSidedef Sidedef = new TMapSidedef(LumpBinaryReader.ReadInt16());
            MapSidedef.Add(Sidedef);
        }
    }
    /// <summary>
    /// Function <c>LoadSectors</c> reads all the SECTOR data in a map.
    /// </summary>
    /// <param name="MapNamespace"><c>MapNamespace</c> is the namespace of the map.</param>
    /// <param name="LumpContent"><c>LumpContent</c> is the binary content of the lump map.</param>
    /// <param name="MapSector"><c>MapSector</c> is the list of SECTOR objects of the map, to append the read data.</param>
    internal void LoadSectors(TMapNamespace MapNamespace, MemoryStream LumpContent, List<TMapSector> MapSector)
    {
        MapSector.Clear();
        BinaryReader LumpBinaryReader = new BinaryReader(LumpContent);
        int Count = Convert.ToInt32(LumpContent.Length / 26);
        for (int I = Count; I > 0; I--)
        {
            short HeightFloor = LumpBinaryReader.ReadInt16();
            short HeightCeiling = LumpBinaryReader.ReadInt16();
            LumpBinaryReader.BaseStream.Seek(18, SeekOrigin.Current);
            short Special = LumpBinaryReader.ReadInt16();
            short ID = LumpBinaryReader.ReadInt16();
            TMapSector Sector = new TMapSector(HeightFloor, HeightCeiling, ID, Special, false);
            MapSector.Add(Sector);
        }
    }
    /// <summary>
    /// Function <c>LoadThings</c> reads all the THING data in a map.
    /// </summary>
    /// <param name="MapNamespace"><c>MapNamespace</c> is the namespace of the map.</param>
    /// <param name="LumpContent"><c>LumpContent</c> is the binary content of the lump map.</param>
    /// <param name="MapThing"><c>MapThing</c> is the list of THING objects of the map, to append the read data.</param>
    internal void LoadThings(TMapNamespace MapNamespace, MemoryStream LumpContent, List<TMapThing> MapThing)
    {
        MapThing.Clear();
        BinaryReader LumpBinaryReader = new BinaryReader(LumpContent);
        if (MapNamespace == TMapNamespace.MapNamespaceDoom)
        {
            int Count = Convert.ToInt32(LumpContent.Length / 10);
            for (int I = Count; I > 0; I--)
            {
                short X = LumpBinaryReader.ReadInt16();
                short Y = LumpBinaryReader.ReadInt16();
                LumpBinaryReader.BaseStream.Seek(2, SeekOrigin.Current);
                short ThingType = LumpBinaryReader.ReadInt16();
                LumpBinaryReader.BaseStream.Seek(2, SeekOrigin.Current);
                TMapThing Thing = new TMapThing(X, Y, 0, ThingType, 0, 0, 0, 0, 0, 0, 0);
                MapThing.Add(Thing);
            }
        }
        else
        {
            int Count = Convert.ToInt32(LumpContent.Length / 20);
            for (int I = Count; I > 0; I--)
            {
                short ID = LumpBinaryReader.ReadInt16();
                short X = LumpBinaryReader.ReadInt16();
                short Y = LumpBinaryReader.ReadInt16();
                LumpBinaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                short ThingType = LumpBinaryReader.ReadInt16();
                LumpBinaryReader.BaseStream.Seek(2, SeekOrigin.Current);
                short Special = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg0 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg1 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg2 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg3 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                short Arg4 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                TMapThing Thing = new TMapThing(X, Y, 0, ThingType, ID, Special, Arg0, Arg1, Arg2, Arg3, Arg4);
                MapThing.Add(Thing);
            }
        }
    }
    /// <summary>
    /// Function <c>LoadFromWad</c> loads all the maps contained in a .WAD file.
    /// </summary>
    /// <param name="ArchiveFileStream"><c>ArchiveFileStream</c> is a stream, that gives the binary content of the WAD archive.</param>
    internal bool LoadFromWad(Stream ArchiveFileStream)
    {
        List<string> MapLumpNames = new List<string>(MapLumpNamesDefs);
        using (BinaryReader Reader = new BinaryReader(ArchiveFileStream))
        {
            byte[] HeaderSignature = Reader.ReadBytes(4);
            string HeaderSignatureText = Encoding.ASCII.GetString(HeaderSignature);
            if ((string.Compare(HeaderSignatureText, "IWAD", false) != 0) && (string.Compare(HeaderSignatureText, "PWAD", false) != 0))
                return false;
            int HeaderNumberOfEntries = Reader.ReadInt32();
            int HeaderDirectoryOffset = Reader.ReadInt32();
            Reader.BaseStream.Seek(HeaderDirectoryOffset, SeekOrigin.Begin);
            TArchiveEntry[] ArchiveEntries = new TArchiveEntry[HeaderNumberOfEntries];
            for (int I = 0; I < HeaderNumberOfEntries; I++)
            {
                int EntryOffset = Reader.ReadInt32();
                int EntrySize = Reader.ReadInt32();
                byte[] EntryName = Reader.ReadBytes(8);
                string EntryNameText = Encoding.ASCII.GetString(EntryName).Replace("\0", "");
                ArchiveEntries[I] = new TArchiveEntry(EntryNameText, EntryOffset, EntrySize);
            }
            bool MapFound = false;
            TMapNamespace Namespace = TMapNamespace.MapNamespaceDoom;
            string? TextMap = null;
            List<TMapVertex> MapVertex = new List<TMapVertex>();
            List<TMapLinedef> MapLinedef = new List<TMapLinedef>();
            List<TMapSidedef> MapSidedef = new List<TMapSidedef>();
            List<TMapSector> MapSector = new List<TMapSector>();
            List<TMapThing> MapThing = new List<TMapThing>();
            int EntryNumber = HeaderNumberOfEntries - 1;
            while (EntryNumber >= 0)
            {
                if (MapLumpNames.Contains(ArchiveEntries[EntryNumber].EntryName))
                {
                    MapFound = true;
                    if (string.Compare(ArchiveEntries[EntryNumber].EntryName, "BEHAVIOR", false) == 0)
                        Namespace = TMapNamespace.MapNamespaceHexen;
                    if (string.Compare(ArchiveEntries[EntryNumber].EntryName, "TEXTMAP", false) == 0)
                    {
                        long SavedPosition = Reader.BaseStream.Position;
                        Reader.BaseStream.Position = ArchiveEntries[EntryNumber].EntryOffset;
                        byte[] LumpContent = Reader.ReadBytes(ArchiveEntries[EntryNumber].EntrySize);
                        TextMap = Encoding.UTF8.GetString(LumpContent);
                        Reader.BaseStream.Position = SavedPosition;
                    }
                    if (string.Compare(ArchiveEntries[EntryNumber].EntryName, "VERTEXES", false) == 0)
                    {
                        long SavedPosition = Reader.BaseStream.Position;
                        Reader.BaseStream.Position = ArchiveEntries[EntryNumber].EntryOffset;
                        byte[] LumpContent = Reader.ReadBytes(ArchiveEntries[EntryNumber].EntrySize);
                        MemoryStream MemoryLump = new MemoryStream(LumpContent);
                        LoadVertexes(Namespace, MemoryLump, MapVertex);
                        Reader.BaseStream.Position = SavedPosition;
                    }
                    if (string.Compare(ArchiveEntries[EntryNumber].EntryName, "LINEDEFS", false) == 0)
                    {
                        long SavedPosition = Reader.BaseStream.Position;
                        Reader.BaseStream.Position = ArchiveEntries[EntryNumber].EntryOffset;
                        byte[] LumpContent = Reader.ReadBytes(ArchiveEntries[EntryNumber].EntrySize);
                        MemoryStream MemoryLump = new MemoryStream(LumpContent);
                        LoadLinedefs(Namespace, MemoryLump, MapLinedef);
                        Reader.BaseStream.Position = SavedPosition;
                    }
                    if (string.Compare(ArchiveEntries[EntryNumber].EntryName, "SIDEDEFS", false) == 0)
                    {
                        long SavedPosition = Reader.BaseStream.Position;
                        Reader.BaseStream.Position = ArchiveEntries[EntryNumber].EntryOffset;
                        byte[] LumpContent = Reader.ReadBytes(ArchiveEntries[EntryNumber].EntrySize);
                        MemoryStream MemoryLump = new MemoryStream(LumpContent);
                        LoadSidedefs(Namespace, MemoryLump, MapSidedef);
                        Reader.BaseStream.Position = SavedPosition;
                    }
                    if (string.Compare(ArchiveEntries[EntryNumber].EntryName, "SECTORS", false) == 0)
                    {
                        long SavedPosition = Reader.BaseStream.Position;
                        Reader.BaseStream.Position = ArchiveEntries[EntryNumber].EntryOffset;
                        byte[] LumpContent = Reader.ReadBytes(ArchiveEntries[EntryNumber].EntrySize);
                        MemoryStream MemoryLump = new MemoryStream(LumpContent);
                        LoadSectors(Namespace, MemoryLump, MapSector);
                        Reader.BaseStream.Position = SavedPosition;
                    }
                    if (string.Compare(ArchiveEntries[EntryNumber].EntryName, "THINGS", false) == 0)
                    {
                        long SavedPosition = Reader.BaseStream.Position;
                        Reader.BaseStream.Position = ArchiveEntries[EntryNumber].EntryOffset;
                        byte[] LumpContent = Reader.ReadBytes(ArchiveEntries[EntryNumber].EntrySize);
                        MemoryStream MemoryLump = new MemoryStream(LumpContent);
                        LoadThings(Namespace, MemoryLump, MapThing);
                        Reader.BaseStream.Position = SavedPosition;
                    }
                }
                else
                {
                    if (MapFound)
                    {
                        string MapName = ArchiveEntries[EntryNumber].EntryName;
                        TMapDefinition MapDefinition = new TMapDefinition(MapName);
                        if (!string.IsNullOrEmpty(TextMap))
                        {
                            TUdmfParser UdmfParser = new TUdmfParser(TextMap);
                            UdmfParser.ParseMap(MapDefinition);
                            MapDefinition.MapNamespace = UdmfParser.MapNamespace;
                            AddMap(MapDefinition);
                            TextMap = null;
                        }
                        else
                        {
                            MapVertex.ForEach(MapDefinition.AddMapVertex);
                            MapLinedef.ForEach(MapDefinition.AddMapLinedef);
                            MapSidedef.ForEach(MapDefinition.AddMapSidedef);
                            MapSector.ForEach(MapDefinition.AddMapSector);
                            MapThing.ForEach(MapDefinition.AddMapThing);
                            AddMap(MapDefinition);
                        }
                        MapFound = false;
                        Namespace = TMapNamespace.MapNamespaceDoom;
                    }
                }
                EntryNumber--;
            }
        }
        SortMap();
        return true;
    }
    /// <summary>
    /// Function <c>LoadFromWad</c> loads all the maps contained in a .WAD file.
    /// </summary>
    /// <param name="FileSpec"><c>FileSpec</c> is the file to be read.</param>
    internal bool LoadFromWad(string FileSpec)
    {
        if (File.Exists(FileSpec))
            using (FileStream ArchiveFileStream = File.OpenRead(FileSpec))
            {
                this.FileSpec = FileSpec;
                FileName = Path.GetFileName(FileSpec);
                FileSize = ArchiveFileStream.Length;
                LastUpdate = File.GetLastWriteTime(FileSpec);
                return LoadFromWad(ArchiveFileStream);
            }
        else
            return false;
    }
    /// <summary>
    /// Function <c>LoadFromWad</c> loads all the maps contained in a .PK3 / .ZIP file.
    /// All maps must be stored in the /maps/ folder and be in .WAD format.
    /// </summary>
    /// <param name="FileSpec"><c>FileSpec</c> is the file to be read.</param>
    internal bool LoadFromZip(string FileSpec)
    {
        if (File.Exists(FileSpec))
            using (FileStream ArchiveFileStream = File.OpenRead(FileSpec))
            {
                this.FileSpec = FileSpec;
                FileName = Path.GetFileName(FileSpec);
                FileSize = ArchiveFileStream.Length;
                LastUpdate = File.GetLastWriteTime(FileSpec);
                using (ZipArchive Archive = new ZipArchive(ArchiveFileStream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry ArchiveEntry in Archive.Entries)
                        if ((ArchiveEntry.FullName.StartsWith("MAPS/", StringComparison.OrdinalIgnoreCase) && ArchiveEntry.FullName.EndsWith(".WAD", StringComparison.OrdinalIgnoreCase)))
                        {
                            using (Stream MapStream = ArchiveEntry.Open())
                            {
                                MemoryStream SeekableMapStream = new MemoryStream();
                                MapStream.CopyTo(SeekableMapStream);
                                SeekableMapStream.Position = 0;
                                LoadFromWad(SeekableMapStream);
                            }
                        }
                }
                return true;
            }
        else
            return false;
    }
    /// <summary>
    /// Function <c>Load</c> loads all the maps contained in a .PK3 / .ZIP file.
    /// All maps must be stored in the /maps/ folder and be in .WAD format.
    /// </summary>
    /// <param name="FileSpec"><c>FileSpec</c> is the file to be read.</param>
    public bool Load(string FileSpec)
    {
        if (File.Exists(FileSpec))
        {
            string FileExtension = Path.GetExtension(FileSpec);
            if (string.Compare(FileExtension, ".TXT", true) == 0)
                return LoadFromText(FileSpec);
            if (string.Compare(FileExtension, ".WAD", true) == 0)
                return LoadFromWad(FileSpec);
            if ((string.Compare(FileExtension, ".PK3", true) == 0) || (string.Compare(FileExtension, ".ZIP", true) == 0))
                return LoadFromZip(FileSpec);
            return false;
        }
        else
            return false;
    }
}
