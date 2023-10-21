/*
 * Author: Pollazzon Stefano
 * Project: ZDoom Navmesh builder
 * This module contains the map entities used in ZDoom,
 * supporting both WAD and PK3 file formats, and supporting
 * Doom, Hexen and UDMF map formats
 */
using NavmeshBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ZDoomNavmesh
{
    /// <summary>
    /// Class <c>TMapEntity</c> is the base class and common ancestor of all map entities.
    /// </summary>
    public abstract class TMapEntity : Object
    {
        public int Index;
    }
    /// <summary>
    /// Class <c>TMapVertex</c> represents a single VERTEX in the map.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
    /// </remarks>
    public sealed class TMapVertex : TMapEntity
    {
        /// <summary>
        /// <c>X</c> represents the X coordinate of the VERTEX.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
        /// </remarks>
        public int X;
        /// <summary>
        /// <c>Y</c> represents the Y coordinate of the VERTEX.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
        /// </remarks>
        public int Y;
    }
    /// <summary>
    /// Class <c>TMapLinedef</c> represents a single LINEDEF in the map.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
    /// </remarks>
    public sealed class TMapLinedef : TMapEntity
    {
        /// <summary>
        /// <c>V1</c> represents the index of the starting VERTEX of the LINEDEF.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
        /// </remarks>
        public int V1;
        /// <summary>
        /// <c>V2</c> represents the index of the ending VERTEX of the LINEDEF.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
        /// </remarks>
        public int V2;
        /// <summary>
        /// <c>ID</c> represents the TAG ID (TID) of the LINEDEF.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
        /// </remarks>
        public int ID;
        /// <summary>
        /// <c>Blocking</c> specifies that this LINEDEF blocks monsters.
        /// There are two flags that sets this value to true: Blocking (monsters and players) and BlockMonsters (only monsters are blocked).
        /// </summary>
        public bool Blocking;
        /// <summary>
        /// <c>TwoSided</c> specifies that this LINEDEF is to be rendered as a transparent portal.
        /// </summary>
        public bool TwoSided;
        /// <summary>
        /// <c>SideFront</c> is the index of the first SIDEDEF, the "front" one.
        /// </summary>
        public int SideFront;
        /// <summary>
        /// <c>SideFront</c> is the index of the second SIDEDEF, the "back" one.
        /// </summary>
        public int SideBack;
        /// <summary>
        /// <c>Special</c> specifies the action special (door, teleporter...), or zero if not present.
        /// </summary>
        public short Special;
        /// <summary>
        /// <c>Arg0</c> specifies the first argument of the action special, or zero if not present.
        /// </summary>
        public short Arg0;
        /// <summary>
        /// <c>Arg1</c> specifies the second argument of the action special, or zero if not present.
        /// </summary>
        public short Arg1;
        /// <summary>
        /// <c>Arg2</c> specifies the third argument of the action special, or zero if not present.
        /// </summary>
        public short Arg2;
        /// <summary>
        /// <c>Arg3</c> specifies the fourth argument of the action special, or zero if not present.
        /// </summary>
        public short Arg3;
        /// <summary>
        /// <c>Arg4</c> specifies the fifth argument of the action special, or zero if not present.
        /// </summary>
        public short Arg4;
        /// <summary>
        /// <c>MonsterUse</c> is a flag, that enables monsters to execute the line special.
        /// </summary>
        public bool MonsterUse;
        /// <summary>
        /// <c>RepeatableSpecial</c> is a flag, that enables the action special to be executed multiple times.
        /// </summary>
        public bool RepeatableSpecial;
        /// <summary>
        /// <c>Ignored</c> is a flag, that exclude this line in the navmesh generation process.
        /// You can enable this flag, by applying the custom UDMF field "user_nocast" and setting it to the boolean value TRUE.
        /// </summary>
        public bool Ignored;
    }
    /// <summary>
    /// Class <c>TMapSidedef</c> represents a single SIDEDEF in the map.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
    /// </remarks>
    public sealed class TMapSidedef : TMapEntity
    {
        /// <summary>
        /// <c>Sector</c> represents the index of the SECTOR, this SIDEDEF belongs to.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
        /// </remarks>
        public int Sector;
    }
    /// <summary>
    /// Class <c>TMapSector</c> represents a single SECTOR in the map.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
    /// </remarks>
    public sealed class TMapSector : TMapEntity
    {
        /// <summary>
        /// <c>HeightFloor</c> represents the height of the floor.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
        /// </remarks>
        public int HeightFloor;
        /// <summary>
        /// <c>HeightCeiling</c> represents the height of the ceiling.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
        /// </remarks>
        public int HeightCeiling;
        /// <summary>
        /// <c>ID</c> represents the TAG ID (TID) of the SECTOR.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
        /// </remarks>
        public int ID;
        /// <summary>
        /// <c>Special</c> specifies the action special (secret, damaging floor...), or zero if not present.
        /// </summary>
        public short Special;
        /// <summary>
        /// <c>Ignored</c> is a flag, that exclude this sector in the navmesh generation process.
        /// You can enable this flag, by applying the custom UDMF field "user_nocast" and setting it to the boolean value TRUE.
        /// </summary>
        public bool Ignored;
    }
    /// <summary>
    /// Class <c>TMapThing</c> represents a single THING in the map.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
    /// </remarks>
    public sealed class TMapThing : TMapEntity
    {
        /// <summary>
        /// <c>X</c> represents the X coordinate of the VERTEX.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
        /// </remarks>
        public int X;
        /// <summary>
        /// <c>Y</c> represents the Y coordinate of the VERTEX.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
        /// </remarks>
        public int Y;
        /// <summary>
        /// <c>Y</c> represents the Z coordinate of the VERTEX.
        /// </summary>
        /// <remarks>
        /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
        /// </remarks>
        public int Z;
        /// <summary>
        /// <c>ThingType</c> specifies the type of the THING.
        /// </summary>
        public int ThingType;
        /// <summary>
        /// <c>ID</c> represents the TAG ID (TID) of the THING.
        /// </summary>
        public int ID;
        /// <summary>
        /// <c>Special</c> specifies the action special (door, teleporter...), or zero if not present.
        /// </summary>
        public short Special;
        /// <summary>
        /// <c>Arg0</c> specifies the first argument of the action special, or zero if not present.
        /// </summary>
        public short Arg0;
        /// <summary>
        /// <c>Arg1</c> specifies the second argument of the action special, or zero if not present.
        /// </summary>
        public short Arg1;
        /// <summary>
        /// <c>Arg2</c> specifies the third argument of the action special, or zero if not present.
        /// </summary>
        public short Arg2;
        /// <summary>
        /// <c>Arg3</c> specifies the fourth argument of the action special, or zero if not present.
        /// </summary>
        public short Arg3;
        /// <summary>
        /// <c>Arg4</c> specifies the fifth argument of the action special, or zero if not present.
        /// </summary>
        public short Arg4;
    }
    /// <summary>
    /// Class <c>TMapNamespace</c> represents the possible namespaces of a map.
    /// The action specials of LINEDEF and SECTOR depends on the map's namespace.
    /// In binary maps we have Doom and Hexen namespaces, depending if the map has a BEHAVIOR lump; in text maps we have all values stored in the text stream.
    /// </summary>
    public enum TMapNamespace
    {
        MapNamespaceDoom,
        MapNamespaceHexen,
        MapNamespaceZDoom
    }
    /// <summary>
    /// Class <c>TMapDefinition</c> stores all necessary lump data.
    /// </summary>
    public sealed class TMapDefinition : Object
    {
        private List<TMapVertex> FMapVertex;
        private List<TMapLinedef> FMapLinedef;
        private List<TMapSidedef> FMapSidedef;
        private List<TMapSector> FMapSector;
        private List<TMapThing> FMapThing;
        public string MapName { get; set; }
        public TMapNamespace MapNamespace { get; set; }
        public List<TMapVertex> MapVertex { get => FMapVertex; }
        public List<TMapLinedef> MapLinedef { get => FMapLinedef; }
        public List<TMapSidedef> MapSidedef { get => FMapSidedef; }
        public List<TMapSector> MapSector { get => FMapSector; }
        public List<TMapThing> MapThing { get => FMapThing; }
        public TMapDefinition()
        {
            FMapVertex = new List<TMapVertex>();
            FMapLinedef = new List<TMapLinedef>();
            FMapSidedef = new List<TMapSidedef>();
            FMapSector = new List<TMapSector>();
            FMapThing = new List<TMapThing>();
        }
        public TMapDefinition(string AMapName) : this()
        {
            MapName = AMapName;
        }
        /// <summary>
        /// Function <c>MapNamespaceText</c> returns a string representation of the map's namespace.
        /// </summary>
        /// <returns>
        /// A string representing the namespace of the map without any leading, trailing, or embedded whitespace.
        /// </returns>
        public string MapNamespaceText()
        {
            switch (MapNamespace)
            {
                case TMapNamespace.MapNamespaceDoom:
                    return "Doom";
                case TMapNamespace.MapNamespaceHexen:
                    return "Hexen";
                case TMapNamespace.MapNamespaceZDoom:
                    return "ZDoom";
                default:
                    return "";
            }
        }
        /// <summary>
        /// Function <c>AddMapVertex</c> adds a VERTEX to the map.
        /// </summary>
        /// <param name="MapVertex"><c>MapVertex</c> is the VERTEX to be added.</param>
        public void AddMapVertex(TMapVertex MapVertex)
        {
            MapVertex.Index = FMapVertex.Count;
            FMapVertex.Add(MapVertex);
        }
        /// <summary>
        /// Function <c>AddMapLinedef</c> adds a LINEDEF to the map.
        /// </summary>
        /// <param name="MapLinedef"><c>MapLinedef</c> is the LINEDEF to be added.</param>
        public void AddMapLinedef(TMapLinedef MapLinedef)
        {
            MapLinedef.Index = FMapLinedef.Count;
            FMapLinedef.Add(MapLinedef);
        }
        /// <summary>
        /// Function <c>AddMapSidedef</c> adds a SIDEDEF to the map.
        /// </summary>
        /// <param name="MapSidedef"><c>MapSidedef</c> is the SIDEDEF to be added.</param>
        public void AddMapSidedef(TMapSidedef MapSidedef)
        {
            MapSidedef.Index = FMapSidedef.Count;
            FMapSidedef.Add(MapSidedef);
        }
        /// <summary>
        /// Function <c>AddMapSector</c> adds a SECTOR to the map.
        /// </summary>
        /// <param name="MapSector"><c>MapSector</c> is the SECTOR to be added.</param>
        public void AddMapSector(TMapSector MapSector)
        {
            MapSector.Index = FMapSector.Count;
            FMapSector.Add(MapSector);
        }
        /// <summary>
        /// Function <c>AddMapThing</c> adds a THING to the map.
        /// </summary>
        /// <param name="MapThing"><c>MapThing</c> is the THING to be added.</param>
        public void AddMapThing(TMapThing MapThing)
        {
            MapThing.Index = FMapThing.Count;
            FMapThing.Add(MapThing);
        }
    }
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
        private string FFileName;
        private string FFileSpec;
        private long FFileSize;
        private DateTime FLastUpdate;
        private List<TMapDefinition> FMapDefinitions;
        public string FileName { get => FFileName; }
        public string FileSpec { get => FFileSpec; }
        public long FileSize { get => FFileSize; }
        public DateTime LastUpdate { get => FLastUpdate; }
        /// <summary>
        /// Property <c>MapDefinitions</c> gives access to the maps in the archive.
        /// </summary>
        public List<TMapDefinition> MapDefinitions { get => FMapDefinitions; }
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
            FMapDefinitions.Sort(delegate(TMapDefinition A, TMapDefinition B) { return String.Compare(A.MapName, B.MapName); });
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
                    FFileSpec = FileSpec;
                    FFileName = Path.GetFileName(FileSpec);
                    FFileSize = ArchiveFileStream.Length;
                    FLastUpdate = File.GetLastWriteTime(FileSpec);
                    using (StreamReader Reader = new StreamReader(ArchiveFileStream))
                    {
                        TUdmfParser UdmfParser = new TUdmfParser(Reader.ReadToEnd());
                        TMapDefinition MapDefinition = new TMapDefinition();
                        MapDefinition.MapName = Path.GetFileNameWithoutExtension(FileSpec);
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
                TMapVertex Vertex = new TMapVertex();
                Vertex.X = LumpBinaryReader.ReadInt16();
                Vertex.Y = LumpBinaryReader.ReadInt16();
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
                    TMapLinedef Linedef = new TMapLinedef();
                    Linedef.V1 = LumpBinaryReader.ReadInt16();
                    Linedef.V2 = LumpBinaryReader.ReadInt16();
                    short Flags = LumpBinaryReader.ReadInt16();
                    Linedef.Blocking = (Flags & 0x0003) != 0;
                    Linedef.TwoSided = (Flags & 0x0004) != 0;
                    Linedef.Special = LumpBinaryReader.ReadInt16();
                    Linedef.Arg0 = LumpBinaryReader.ReadInt16();
                    Linedef.Arg1 = 0;
                    Linedef.Arg2 = 0;
                    Linedef.Arg3 = 0;
                    Linedef.Arg4 = 0;
                    Linedef.SideFront = LumpBinaryReader.ReadInt16();
                    Linedef.SideBack = LumpBinaryReader.ReadInt16();
                    Linedef.Ignored = false;
                    MapLinedef.Add(Linedef);
                }
            }
            else
            {
                int Count = Convert.ToInt32(LumpContent.Length / 16);
                for (int I = Count; I > 0; I--)
                {
                    TMapLinedef Linedef = new TMapLinedef();
                    Linedef.V1 = LumpBinaryReader.ReadInt16();
                    Linedef.V2 = LumpBinaryReader.ReadInt16();
                    short Flags = LumpBinaryReader.ReadInt16();
                    Linedef.Blocking = (Flags & 0x0003) != 0;
                    Linedef.TwoSided = (Flags & 0x0004) != 0;
                    Linedef.Special = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Linedef.Arg0 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Linedef.Arg1 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Linedef.Arg2 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Linedef.Arg3 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Linedef.Arg4 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Linedef.SideFront = LumpBinaryReader.ReadInt16();
                    Linedef.SideBack = LumpBinaryReader.ReadInt16();
                    Linedef.Ignored = false;
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
                TMapSidedef Sidedef = new TMapSidedef();
                LumpBinaryReader.BaseStream.Seek(28, SeekOrigin.Current);
                Sidedef.Sector = LumpBinaryReader.ReadInt16();
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
                TMapSector Sector = new TMapSector();
                Sector.HeightFloor = LumpBinaryReader.ReadInt16();
                Sector.HeightCeiling = LumpBinaryReader.ReadInt16();
                LumpBinaryReader.BaseStream.Seek(18, SeekOrigin.Current);
                Sector.Special = LumpBinaryReader.ReadInt16();
                Sector.ID = LumpBinaryReader.ReadInt16();
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
                    TMapThing Thing = new TMapThing();
                    Thing.X = LumpBinaryReader.ReadInt16();
                    Thing.Y = LumpBinaryReader.ReadInt16();
                    LumpBinaryReader.BaseStream.Seek(2, SeekOrigin.Current);
                    Thing.ThingType = LumpBinaryReader.ReadInt16();
                    LumpBinaryReader.BaseStream.Seek(2, SeekOrigin.Current);
                    Thing.ID = 0;
                    Thing.Special = 0;
                    Thing.Arg0 = 0;
                    Thing.Arg1 = 0;
                    Thing.Arg2 = 0;
                    Thing.Arg3 = 0;
                    Thing.Arg4 = 0;
                    MapThing.Add(Thing);
                }
            }
            else
            {
                int Count = Convert.ToInt32(LumpContent.Length / 20);
                for (int I = Count; I > 0; I--)
                {
                    TMapThing Thing = new TMapThing();
                    Thing.ID = LumpBinaryReader.ReadInt16();
                    Thing.X = LumpBinaryReader.ReadInt16();
                    Thing.Y = LumpBinaryReader.ReadInt16();
                    LumpBinaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                    Thing.ThingType = LumpBinaryReader.ReadInt16();
                    LumpBinaryReader.BaseStream.Seek(2, SeekOrigin.Current);
                    Thing.Special = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Thing.Arg0 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Thing.Arg1 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Thing.Arg2 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Thing.Arg3 = Convert.ToInt16(LumpBinaryReader.ReadByte());
                    Thing.Arg4 = Convert.ToInt16(LumpBinaryReader.ReadByte());
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
            List<string> MapLumpNames = new List<string>(new string[]
            {
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
            });
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
                string TextMap = null;
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
                                MapVertex.ForEach((Vertex) => MapDefinition.AddMapVertex(Vertex));
                                MapLinedef.ForEach((Linedef) => MapDefinition.AddMapLinedef(Linedef));
                                MapSidedef.ForEach((Sidedef) => MapDefinition.AddMapSidedef(Sidedef));
                                MapSector.ForEach((Sector) => MapDefinition.AddMapSector(Sector));
                                MapThing.ForEach((Thing) => MapDefinition.AddMapThing(Thing));
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
                    FFileSpec = FileSpec;
                    FFileName = Path.GetFileName(FileSpec);
                    FFileSize = ArchiveFileStream.Length;
                    FLastUpdate = File.GetLastWriteTime(FileSpec);
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
                    FFileSpec = FileSpec;
                    FFileName = Path.GetFileName(FileSpec);
                    FFileSize = ArchiveFileStream.Length;
                    FLastUpdate = File.GetLastWriteTime(FileSpec);
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
}
