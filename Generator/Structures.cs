/*
 * Author: Pollazzon Stefano
 * Project: ZDoom Navmesh builder
 * This module contains the map entities used in ZDoom
 */
using System;
using System.Collections.Generic;

namespace NavmeshBuilder;

/// <summary>
/// Class <c>TMapEntity</c> is the base class and common ancestor of all map entities.
/// </summary>
public abstract record class TMapEntity : Object
{
    public int Index { get; set; }
}
/// <summary>
/// Class <c>TMapVertex</c> represents a single VERTEX in the map.
/// </summary>
/// <remarks>
/// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
/// </remarks>
public sealed record class TMapVertex : TMapEntity
{
    /// <summary>
    /// <c>X</c> represents the X coordinate of the VERTEX.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
    /// </remarks>
    public int X { get; init; }
    /// <summary>
    /// <c>Y</c> represents the Y coordinate of the VERTEX.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
    /// </remarks>
    public int Y { get; init; }
    public TMapVertex(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }
}
/// <summary>
/// Class <c>TMapLinedef</c> represents a single LINEDEF in the map.
/// </summary>
/// <remarks>
/// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
/// </remarks>
public sealed record class TMapLinedef : TMapEntity
{
    /// <summary>
    /// <c>V1</c> represents the index of the starting VERTEX of the LINEDEF.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
    /// </remarks>
    public int V1 { get; init; }
    /// <summary>
    /// <c>V2</c> represents the index of the ending VERTEX of the LINEDEF.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
    /// </remarks>
    public int V2 { get; init; }
    /// <summary>
    /// <c>ID</c> represents the TAG ID (TID) of the LINEDEF.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
    /// </remarks>
    public int ID { get; init; }
    /// <summary>
    /// <c>Blocking</c> specifies that this LINEDEF blocks monsters.
    /// There are two flags that sets this value to true: Blocking (monsters and players) and BlockMonsters (only monsters are blocked).
    /// </summary>
    public bool Blocking { get; init; }
    /// <summary>
    /// <c>TwoSided</c> specifies that this LINEDEF is to be rendered as a transparent portal.
    /// </summary>
    public bool TwoSided { get; init; }
    /// <summary>
    /// <c>SideFront</c> is the index of the first SIDEDEF, the "front" one.
    /// </summary>
    public int SideFront { get; init; }
    /// <summary>
    /// <c>SideFront</c> is the index of the second SIDEDEF, the "back" one.
    /// </summary>
    public int SideBack { get; init; }
    /// <summary>
    /// <c>Special</c> specifies the action special (door, teleporter...), or zero if not present.
    /// </summary>
    public short Special;
    /// <summary>
    /// <c>Arg0</c> specifies the first argument of the action special, or zero if not present.
    /// </summary>
    public short Arg0 { get; init; }
    /// <summary>
    /// <c>Arg1</c> specifies the second argument of the action special, or zero if not present.
    /// </summary>
    public short Arg1 { get; init; }
    /// <summary>
    /// <c>Arg2</c> specifies the third argument of the action special, or zero if not present.
    /// </summary>
    public short Arg2 { get; init; }
    /// <summary>
    /// <c>Arg3</c> specifies the fourth argument of the action special, or zero if not present.
    /// </summary>
    public short Arg3 { get; init; }
    /// <summary>
    /// <c>Arg4</c> specifies the fifth argument of the action special, or zero if not present.
    /// </summary>
    public short Arg4 { get; init; }
    /// <summary>
    /// <c>MonsterUse</c> is a flag, that enables monsters to execute the line special.
    /// </summary>
    public bool MonsterUse { get; init; }
    /// <summary>
    /// <c>RepeatableSpecial</c> is a flag, that enables the action special to be executed multiple times.
    /// </summary>
    public bool RepeatableSpecial { get; init; }
    /// <summary>
    /// <c>Ignored</c> is a flag, that exclude this line in the navmesh generation process.
    /// You can enable this flag, by applying the custom UDMF field "user_nocast" and setting it to the boolean value TRUE.
    /// </summary>
    public bool Ignored { get; set; }
    public TMapLinedef(int V1, int V2, int ID, bool Blocking, bool TwoSided, int SideFront, int SideBack, short Special, short Arg0, short Arg1, short Arg2, short Arg3, short Arg4, bool MonsterUse, bool RepeatableSpecial, bool Ignored)
    {
        this.V1 = V1;
        this.V2 = V2;
        this.ID = ID;
        this.Blocking = Blocking;
        this.TwoSided = TwoSided;
        this.SideFront = SideFront;
        this.SideBack = SideBack;
        this.Special = Special;
        this.Arg0 = Arg0;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
        this.Arg4 = Arg4;
        this.MonsterUse = MonsterUse;
        this.RepeatableSpecial = RepeatableSpecial;
        this.Ignored = Ignored;
    }
    public TMapLinedef(int V1, int V2, int ID, short Flags, int SideFront, int SideBack, short Special, short Arg0, short Arg1, short Arg2, short Arg3, short Arg4, bool MonsterUse, bool RepeatableSpecial, bool Ignored)
    {
        this.V1 = V1;
        this.V2 = V2;
        this.ID = ID;
        this.Blocking = (Flags & 0x0003) != 0;
        this.TwoSided = (Flags & 0x0004) != 0;
        this.SideFront = SideFront;
        this.SideBack = SideBack;
        this.Special = Special;
        this.Arg0 = Arg0;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
        this.Arg4 = Arg4;
        this.MonsterUse = MonsterUse;
        this.RepeatableSpecial = RepeatableSpecial;
        this.Ignored = Ignored;
    }
}
/// <summary>
/// Class <c>TMapSidedef</c> represents a single SIDEDEF in the map.
/// </summary>
/// <remarks>
/// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
/// </remarks>
public sealed record class TMapSidedef : TMapEntity
{
    /// <summary>
    /// <c>Sector</c> represents the index of the SECTOR, this SIDEDEF belongs to.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
    /// </remarks>
    public int Sector { get; init; }
    public TMapSidedef(int Sector)
    {
        this.Sector = Sector;
    }
}
/// <summary>
/// Class <c>TMapSector</c> represents a single SECTOR in the map.
/// </summary>
/// <remarks>
/// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
/// </remarks>
public sealed record class TMapSector : TMapEntity
{
    /// <summary>
    /// <c>HeightFloor</c> represents the height of the floor.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
    /// </remarks>
    public int HeightFloor { get; init; }
    /// <summary>
    /// <c>HeightCeiling</c> represents the height of the ceiling.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
    /// </remarks>
    public int HeightCeiling { get; init; }
    /// <summary>
    /// <c>ID</c> represents the TAG ID (TID) of the SECTOR.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit unsigned int for convenience purposes only.
    /// </remarks>
    public int ID { get; init; }
    /// <summary>
    /// <c>Special</c> specifies the action special (secret, damaging floor...), or zero if not present.
    /// </summary>
    public short Special { get; init; }
    /// <summary>
    /// <c>Ignored</c> is a flag, that exclude this sector in the navmesh generation process.
    /// You can enable this flag, by applying the custom UDMF field "user_nocast" and setting it to the boolean value TRUE.
    /// </summary>
    public bool Ignored { get; set; }
    public TMapSector(int HeightFloor, int HeightCeiling, int ID, short Special, bool Ignored)
    {
        this.HeightFloor = HeightFloor;
        this.HeightCeiling = HeightCeiling;
        this.ID = ID;
        this.Special = Special;
        this.Ignored = Ignored;
    }
}
/// <summary>
/// Class <c>TMapThing</c> represents a single THING in the map.
/// </summary>
/// <remarks>
/// I'm using 32 bit signed int instead of the true data types for convenience purposes only.
/// </remarks>
public sealed record class TMapThing : TMapEntity
{
    /// <summary>
    /// <c>X</c> represents the X coordinate of the VERTEX.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
    /// </remarks>
    public int X { get; init; }
    /// <summary>
    /// <c>Y</c> represents the Y coordinate of the VERTEX.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
    /// </remarks>
    public int Y { get; init; }
    /// <summary>
    /// <c>Y</c> represents the Z coordinate of the VERTEX.
    /// </summary>
    /// <remarks>
    /// I'm using 32 bit signed int instead of 16 bit signed int for convenience purposes only.
    /// </remarks>
    public int Z { get; init; }
    /// <summary>
    /// <c>ThingType</c> specifies the type of the THING.
    /// </summary>
    public int ThingType { get; init; }
    /// <summary>
    /// <c>ID</c> represents the TAG ID (TID) of the THING.
    /// </summary>
    public int ID { get; init; }
    /// <summary>
    /// <c>Special</c> specifies the action special (door, teleporter...), or zero if not present.
    /// </summary>
    public short Special { get; init; }
    /// <summary>
    /// <c>Arg0</c> specifies the first argument of the action special, or zero if not present.
    /// </summary>
    public short Arg0 { get; init; }
    /// <summary>
    /// <c>Arg1</c> specifies the second argument of the action special, or zero if not present.
    /// </summary>
    public short Arg1 { get; init; }
    /// <summary>
    /// <c>Arg2</c> specifies the third argument of the action special, or zero if not present.
    /// </summary>
    public short Arg2 { get; init; }
    /// <summary>
    /// <c>Arg3</c> specifies the fourth argument of the action special, or zero if not present.
    /// </summary>
    public short Arg3 { get; init; }
    /// <summary>
    /// <c>Arg4</c> specifies the fifth argument of the action special, or zero if not present.
    /// </summary>
    public short Arg4 { get; init; }
    public TMapThing(int X, int Y, int Z, int ThingType, int ID, short Special, short Arg0, short Arg1, short Arg2, short Arg3, short Arg4)
    {
        this.X = X;
        this.Y = Y;
        this.Z = Z;
        this.ThingType = ThingType;
        this.ID = ID;
        this.Special = Special;
        this.Arg0 = Arg0;
        this.Arg1 = Arg1;
        this.Arg2 = Arg2;
        this.Arg3 = Arg3;
        this.Arg4 = Arg4;
    }
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
    public TMapDefinition(string AMapName) : base()
    {
        FMapVertex = new List<TMapVertex>();
        FMapLinedef = new List<TMapLinedef>();
        FMapSidedef = new List<TMapSidedef>();
        FMapSector = new List<TMapSector>();
        FMapThing = new List<TMapThing>();
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
        if (MapVertex is null)
            return;
        MapVertex.Index = FMapVertex.Count;
        FMapVertex.Add(MapVertex);
    }
    /// <summary>
    /// Function <c>AddMapLinedef</c> adds a LINEDEF to the map.
    /// </summary>
    /// <param name="MapLinedef"><c>MapLinedef</c> is the LINEDEF to be added.</param>
    public void AddMapLinedef(TMapLinedef MapLinedef)
    {
        if (MapLinedef is null)
            return;
        MapLinedef.Index = FMapLinedef.Count;
        FMapLinedef.Add(MapLinedef);
    }
    /// <summary>
    /// Function <c>AddMapSidedef</c> adds a SIDEDEF to the map.
    /// </summary>
    /// <param name="MapSidedef"><c>MapSidedef</c> is the SIDEDEF to be added.</param>
    public void AddMapSidedef(TMapSidedef MapSidedef)
    {
        if (MapSidedef is null)
            return;
        MapSidedef.Index = FMapSidedef.Count;
        FMapSidedef.Add(MapSidedef);
    }
    /// <summary>
    /// Function <c>AddMapSector</c> adds a SECTOR to the map.
    /// </summary>
    /// <param name="MapSector"><c>MapSector</c> is the SECTOR to be added.</param>
    public void AddMapSector(TMapSector MapSector)
    {
        if (MapSector is null)
            return;
        MapSector.Index = FMapSector.Count;
        FMapSector.Add(MapSector);
    }
    /// <summary>
    /// Function <c>AddMapThing</c> adds a THING to the map.
    /// </summary>
    /// <param name="MapThing"><c>MapThing</c> is the THING to be added.</param>
    public void AddMapThing(TMapThing MapThing)
    {
        if (MapThing is null)
            return;
        MapThing.Index = FMapThing.Count;
        FMapThing.Add(MapThing);
    }
}